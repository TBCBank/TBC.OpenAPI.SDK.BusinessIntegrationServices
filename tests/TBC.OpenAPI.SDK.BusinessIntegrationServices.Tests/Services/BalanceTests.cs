// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.DependencyInjection;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Services;

[Collection(IntegrationTestCollection.Name)]
public class BalanceTests : ServiceIntegrationTest
{
    public BalanceTests(IntegrationTestHostFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task AccountBalance_ByIbanGel_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountBalances(
            accountNumber: "GE48TB7873440574631292",
            currency: "GEL",
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("GEL", response.Currency);
    }
}
