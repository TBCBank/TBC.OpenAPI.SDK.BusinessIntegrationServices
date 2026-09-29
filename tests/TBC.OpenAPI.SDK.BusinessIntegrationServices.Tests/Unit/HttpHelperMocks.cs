// Copyright (C) TBC Bank. All Rights Reserved.

using System.Net;
using TBC.OpenAPI.SDK.BusinessIntegrationServices.Models.Responses;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Unit
{
    public class HttpHelperMocks : IDisposable
    {
        public const string ErrorMessage = "Error Message For Mock";
        public const string AccessToken = "mock-access-token";
        public const string StatementAccountNumber = "GE00TB0000000000000000";

        private readonly WireMockServer _mockServer;

        public HttpClient HttpClient { get; }

        /// <summary>
        /// Base URL of the mock server (with a trailing slash) for wiring up an
        /// <see cref="HttpClient"/> through the real DI pipeline.
        /// </summary>
        public string BaseUrl { get; }

        public HttpHelperMocks()
        {
            _mockServer = WireMockServer.Start();
            BaseUrl = $"{_mockServer.Urls[0]}/";
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            AddOAuthMocks();
            AddStatementMocks();
            AddBalanceMocks();
            AddMovementMocks();
        }

        private void AddOAuthMocks()
        {
            // OAuth token endpoint - always succeeds. Keys are snake_case to match the OAuth
            // token response contract consumed by the SDK core token handler.
            _mockServer
                .Given(Request.Create().WithPath("/oauth/token").UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new
                        {
                            access_token = AccessToken,
                            token_type = "Bearer",
                            expires_in = 3600,
                            scope = "bab_accounts bab_transfers"
                        }));
        }

        private void AddStatementMocks()
        {
            _mockServer
                .Given(Request.Create().WithPath("/bab/v1/accounts/statements").UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new GetAccountStatementResponse
                        {
                            AccountNumber = StatementAccountNumber,
                            Currency = "GEL",
                            OpeningBalance = 100.50m,
                            ClosingBalance = 250.75m,
                            CreditSum = 200.25m,
                            DebitSum = 50.00m
                        }));
        }

        private void AddBalanceMocks()
        {
            _mockServer
                .Given(Request.Create().WithPath($"/bab/v1/accounts/{StatementAccountNumber}/balances/GEL").UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new GetAccountBalanceResponse
                        {
                            CurrentBalance = 250.75m,
                            AvailableBalance = 300.25m,
                            OverdraftLimit = 50.00m,
                            Currency = "GEL"
                        }));
        }

        private void AddMovementMocks()
        {
            // Non-authorization error path -> should surface as OpenApiException.
            _mockServer
                .Given(Request.Create().WithPath("/bab/v1/accounts/movements/BAD").UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(400)
                        .WithBodyAsJson(new
                        {
                            title = ErrorMessage,
                            detail = ErrorMessage,
                            type = "error_type",
                            status = (int)HttpStatusCode.BadRequest
                        }));

            // 401 on first attempt then success on retry (token refresh path).
            _mockServer
                .Given(Request.Create().WithPath("/bab/v1/accounts/movements/REFRESH").UsingGet())
                .InScenario("token-refresh")
                .WillSetStateTo("Refreshed")
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(401)
                        .WithBodyAsJson(new
                        {
                            title = "Unauthorized",
                            status = (int)HttpStatusCode.Unauthorized
                        }));

            _mockServer
                .Given(Request.Create().WithPath("/bab/v1/accounts/movements/REFRESH").UsingGet())
                .InScenario("token-refresh")
                .WhenStateIs("Refreshed")
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new GetAccountMovementByIdResponse
                        {
                            Transaction = new AccountMovementTransaction
                            {
                                MovementId = "REFRESH.1",
                                IsDebit = true
                            }
                        }));

            // Same 401-then-200 shape as REFRESH, but on a dedicated path and scenario so a caller
            // supplied retry handler can consume it exactly once, independently of the REFRESH tests.
            _mockServer
                .Given(Request.Create().WithPath("/bab/v1/accounts/movements/RETRY").UsingGet())
                .InScenario("retry-token-refresh")
                .WillSetStateTo("Refreshed")
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(401)
                        .WithBodyAsJson(new
                        {
                            title = "Unauthorized",
                            status = (int)HttpStatusCode.Unauthorized
                        }));

            _mockServer
                .Given(Request.Create().WithPath("/bab/v1/accounts/movements/RETRY").UsingGet())
                .InScenario("retry-token-refresh")
                .WhenStateIs("Refreshed")
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new GetAccountMovementByIdResponse
                        {
                            Transaction = new AccountMovementTransaction
                            {
                                MovementId = "RETRY.1",
                                IsDebit = true
                            }
                        }));
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            _mockServer.Stop();
            _mockServer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
