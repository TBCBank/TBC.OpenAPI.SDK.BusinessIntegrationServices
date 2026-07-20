// Copyright (C) TBC Bank. All Rights Reserved.

using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using TBC.OpenAPI.SDK.Core;
using TBC.OpenAPI.SDK.Core.Exceptions;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Unit
{
    public class BusinessIntegrationServicesClientTests : IClassFixture<HttpHelperMocks>
    {
        private readonly IBusinessIntegrationServicesClient _client;

        public BusinessIntegrationServicesClientTests(HttpHelperMocks mocks)
        {
            var factory = new Mock<IHttpClientFactory>();
            factory
                .Setup(x => x.CreateClient(typeof(BusinessIntegrationServicesClient).FullName!))
                .Returns(mocks.HttpClient);

            var http = new HttpHelper<BusinessIntegrationServicesClient>(factory.Object);
            _client = new BusinessIntegrationServicesClient(http);
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
        public async Task GetAccountMovementById_WhenUnauthorizedThenSuccess_RefreshesTokenAndReturnsData()
        {
            var response = await _client.GetAccountMovementById("REFRESH", CancellationToken.None);

            using var _ = new AssertionScope();
            response.Should().NotBeNull();
            response.Transaction.Should().NotBeNull();
            response.Transaction.MovementId.Should().Be("REFRESH.1");
            response.Transaction.IsDebit.Should().BeTrue();
        }
    }
}
