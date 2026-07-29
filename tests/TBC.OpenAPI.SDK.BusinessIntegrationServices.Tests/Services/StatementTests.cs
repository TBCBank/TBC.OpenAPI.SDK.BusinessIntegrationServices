// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.DependencyInjection;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Services;

[Collection(IntegrationTestCollection.Name)]
public class StatementTests : ServiceIntegrationTest
{
    public StatementTests(IntegrationTestHostFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Statement_ValidRequest1_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountStatement(
            accountNumber: "GE48TB7873440574631292",
            accountCurrencyCode: "GEL",
            periodFrom: new DateTime(2025, 01, 01),
            periodTo: new DateTime(2025, 03, 01),
            CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(new DateTime(2025, 01, 01), response.OpeningDate),
            () => Assert.Equal(1000.50M, response.OpeningBalance),
            () => Assert.Equal(new DateTime(2025, 03, 01), response.ClosingDate),
            () => Assert.Equal(1250.00M, response.ClosingBalance),
            () => Assert.Equal(500.25M, response.CreditSum),
            () => Assert.Equal(250.75M, response.DebitSum),
            () => Assert.Equal("GEL", response.Currency),
            () => Assert.Equal("GE48TB7873440574631292", response.AccountNumber)
        );
    }

    [Fact]
    public async Task Statement_ValidRequest2_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountStatement(
            accountNumber: "GE48TB7873440574631292",
            accountCurrencyCode: "USD",
            periodFrom: new DateTime(2025, 03, 01),
            periodTo: new DateTime(2025, 06, 01),
            CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(new DateTime(2025, 03, 01), response.OpeningDate),
            () => Assert.Equal(new DateTime(2025, 06, 01), response.ClosingDate),
            () => Assert.Equal(0, response.OpeningBalance),
            () => Assert.Equal(0, response.ClosingBalance),
            () => Assert.Equal(0, response.CreditSum),
            () => Assert.Equal(0, response.DebitSum),
            () => Assert.Equal("USD", response.Currency),
            () => Assert.Equal("GE48TB7873440574631292", response.AccountNumber)
        );
    }
}
