// Copyright (C) TBC Bank. All Rights Reserved.

using System.Net;
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

        public HttpHelperMocks()
        {
            _mockServer = WireMockServer.Start();
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri($"{_mockServer.Urls[0]}/")
            };

            AddOAuthMocks();
            AddStatementMocks();
            AddMovementMocks();
        }

        private void AddOAuthMocks()
        {
            // OAuth token endpoint - always succeeds. Keys must be snake_case to match
            // the [JsonPropertyName] attributes on TokenResponse.
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
