// Copyright (C) TBC Bank. All Rights Reserved.

using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions;
using TBC.OpenAPI.SDK.Core.Exceptions;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Unit
{
    public class BusinessIntegrationServicesClientTests : IClassFixture<HttpHelperMocks>, IDisposable
    {
        private readonly ServiceProvider _provider;
        private readonly IBusinessIntegrationServicesClient _client;
        private readonly HttpHelperMocks _mocks;
        
        public BusinessIntegrationServicesClientTests(HttpHelperMocks mocks)
        {
            _mocks = mocks;
            var options = new BusinessIntegrationServicesClientOptions
            {
                BaseUrl = mocks.BaseUrl,
                ApiKey = "test-api-key",
                ClientSecret = "test-client-secret"
            };

            _provider = new ServiceCollection()
                .AddBusinessIntegrationServicesClient(options)
                .UseInMemoryCache()
                .BuildServiceProvider();

            _client = _provider.GetRequiredService<IBusinessIntegrationServicesClient>();
        }

        [Fact]
        public async Task GetAccountStatement_WhenSuccess_ReturnsData()
        {
            var response = await _client.GetAccountStatement(
                accountNumber: HttpHelperMocks.StatementAccountNumber,
                accountCurrencyCode: "GEL",
                periodFrom: new DateTime(2026, 01, 01),
                periodTo: new DateTime(2026, 01, 31),
                cancellationToken: CancellationToken.None);

            using var _ = new AssertionScope();
            response.Should().NotBeNull();
            response.AccountNumber.Should().Be(HttpHelperMocks.StatementAccountNumber);
            response.Currency.Should().Be("GEL");
            response.ClosingBalance.Should().Be(250.75m);
        }

        [Fact]
        public async Task GetAccountMovementById_WhenErrorResponse_ThrowsOpenApiException()
        {
            var act = async () => await _client.GetAccountMovementById("BAD", CancellationToken.None);

            var exception = await act.Should().ThrowAsync<OpenApiException>();
            exception.Which.Message.Should().Be(HttpHelperMocks.ErrorMessage);
        }

        [Fact]
        public async Task GetAccountMovementById_WhenUnauthorized_ThrowsAndInvalidatesTokenSoNextRequestSucceeds()
        {
            // First attempt: the server returns 401. The request surfaces an OpenApiException and
            // the cached token is invalidated.
            var act = async () => await _client.GetAccountMovementById("REFRESH", CancellationToken.None);
            await act.Should().ThrowAsync<OpenApiException>();

            // Second attempt: the token is regenerated and the request now succeeds.
            var response = await _client.GetAccountMovementById("REFRESH", CancellationToken.None);

            using var _ = new AssertionScope();
            response.Should().NotBeNull();
            response.Transaction.Should().NotBeNull();
            response.Transaction.MovementId.Should().Be("REFRESH.1");
            response.Transaction.IsDebit.Should().BeTrue();
        }

        [Fact]
        public async Task GetAccountMovementById_WhenUnauthorizedAndRetryConfigured_RetriesOnceAndSucceeds()
        {
            // A caller-supplied retry handler, registered through the configurePipeline hook, sits
            // outside the OAuth handler. On the 401 the OAuth handler evicts the cached token; the
            // retry re-enters token handling, acquires a fresh token, and the second attempt succeeds
            // - all within a single call, with no retry logic or Polly dependency in the SDK.
            var options = new BusinessIntegrationServicesClientOptions
            {
                BaseUrl = _mocks.BaseUrl,
                ApiKey = "test-api-key",
                ClientSecret = "test-client-secret"
            };

            var retryHandler = new SingleRetryOnUnauthorizedHandler();

            using var provider = new ServiceCollection()
                .AddBusinessIntegrationServicesClient(
                    options,
                    configurePipeline: builder => builder.AddHttpMessageHandler(() => retryHandler))
                .UseInMemoryCache()
                .BuildServiceProvider();

            var client = provider.GetRequiredService<IBusinessIntegrationServicesClient>();

            var response = await client.GetAccountMovementById("RETRY", CancellationToken.None);

            using var _ = new AssertionScope();
            retryHandler.RetryCount.Should().Be(1);
            response.Should().NotBeNull();
            response.Transaction.Should().NotBeNull();
            response.Transaction.MovementId.Should().Be("RETRY.1");
            response.Transaction.IsDebit.Should().BeTrue();
        }

        public void Dispose()
        {
            _provider.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
