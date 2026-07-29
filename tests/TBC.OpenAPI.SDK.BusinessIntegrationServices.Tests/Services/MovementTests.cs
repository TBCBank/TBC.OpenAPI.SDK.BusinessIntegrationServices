// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.DependencyInjection;
using TBC.OpenAPI.SDK.Core.Exceptions;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Services;

[Collection(IntegrationTestCollection.Name)]
public class MovementTests : ServiceIntegrationTest
{
    public MovementTests(IntegrationTestHostFixture fixture)
        : base(fixture)
    {
    }

    private const string TestAccountNumber = "GE48TB7873440574631292";

    [Fact]
    public async Task MovementById_Debit_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovementById("018716808825.1", CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transaction);
        Assert.Multiple(
            () => Assert.EndsWith(".1", response.Transaction.MovementId),
            () => Assert.True(response.Transaction.IsDebit));
    }

    [Fact]
    public async Task MovementById_DebitCredit_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovementById("018716808825.2", CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transaction);
        Assert.Multiple(
            () => Assert.EndsWith(".2", response.Transaction.MovementId),
            () => Assert.False(response.Transaction.IsDebit));
    }

    [Fact]
    public async Task MovementById_Single_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovementById("018383341565.1", CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transaction);
        Assert.Multiple(
            () => Assert.EndsWith(".1", response.Transaction.MovementId),
            () => Assert.True(response.Transaction.IsDebit));
    }

    [Fact]
    public async Task Movements_EmptyList_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovements(
            accountNumber: null,
            accountCurrencyCode: null,
            periodFrom: new DateTime(2025, 01, 01),
            periodTo: new DateTime(2025, 01, 31),
            lastMovementTimeStamp: null,
            pageIndex: 0,
            pageSize: 100,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Multiple(
            () => Assert.Equal(0, response.Total),
            () => Assert.Empty(response.Transactions));
    }

    [Fact]
    public async Task Movements_PeriodFilter_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovements(
            accountNumber: null,
            accountCurrencyCode: null,
            periodFrom: new DateTime(2026, 02, 01),
            periodTo: new DateTime(2026, 06, 30),
            lastMovementTimeStamp: null,
            pageIndex: 0,
            pageSize: 100,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transactions);
        AssertPager(response, expectedPageIndex: 0, expectedPageSize: 100);
    }

    [Fact]
    public async Task Movements_WithoutPeriodFilter_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovements(
            accountNumber: null,
            accountCurrencyCode: null,
            periodFrom: null,
            periodTo: null,
            lastMovementTimeStamp: null,
            pageIndex: 0,
            pageSize: 100,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transactions);
        AssertPager(response, expectedPageIndex: 0, expectedPageSize: 100);
    }

    [Fact]
    public async Task Movements_PagingFirstPage_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovements(
            accountNumber: null,
            accountCurrencyCode: null,
            periodFrom: new DateTime(2026, 02, 01),
            periodTo: new DateTime(2026, 06, 30),
            lastMovementTimeStamp: null,
            pageIndex: 0,
            pageSize: 20,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transactions);
        AssertPager(response, expectedPageIndex: 0, expectedPageSize: 20);
    }

    [Fact]
    public async Task Movements_PagingNextPage1_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovements(
            accountNumber: null,
            accountCurrencyCode: null,
            periodFrom: new DateTime(2026, 02, 01),
            periodTo: new DateTime(2026, 06, 30),
            lastMovementTimeStamp: null,
            pageIndex: 1,
            pageSize: 20,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transactions);
        AssertPager(response, expectedPageIndex: 1, expectedPageSize: 20);
    }

    [Fact]
    public async Task Movements_PagingNextPage2_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovements(
            accountNumber: null,
            accountCurrencyCode: null,
            periodFrom: new DateTime(2026, 02, 01),
            periodTo: new DateTime(2026, 06, 30),
            lastMovementTimeStamp: null,
            pageIndex: 2,
            pageSize: 20,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transactions);
        AssertPager(response, expectedPageIndex: 2, expectedPageSize: 20);
    }

    [Fact]
    public async Task Movements_ByIbanGel_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovements(
            accountNumber: TestAccountNumber,
            accountCurrencyCode: "GEL",
            periodFrom: new DateTime(2026, 06, 01),
            periodTo: new DateTime(2026, 06, 30),
            lastMovementTimeStamp: null,
            pageIndex: 0,
            pageSize: 100,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transactions);
        AssertPager(response, expectedPageIndex: 0, expectedPageSize: 100);
    }

    [Fact]
    public async Task Movements_ByIbanUsd_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovements(
            accountNumber: TestAccountNumber,
            accountCurrencyCode: "USD",
            periodFrom: new DateTime(2026, 06, 01),
            periodTo: new DateTime(2026, 06, 30),
            lastMovementTimeStamp: null,
            pageIndex: 0,
            pageSize: 100,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transactions);
        AssertPager(response, expectedPageIndex: 0, expectedPageSize: 100);
    }

    [Fact]
    public async Task Movements_TimeStampFilter_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act
        var response = await client.GetAccountMovements(
            accountNumber: null,
            accountCurrencyCode: null,
            periodFrom: null,
            periodTo: null,
            lastMovementTimeStamp: new DateTime(2026, 06, 09),
            pageIndex: 0,
            pageSize: 100,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Transactions);
        AssertPager(response, expectedPageIndex: 0, expectedPageSize: 100);
    }

    [Fact]
    public async Task Movements_NotAuthorized_ThrowsOpenApiException()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act & Assert
        await Assert.ThrowsAsync<OpenApiException>(
            () => client.GetAccountMovements(
                accountNumber: null,
                accountCurrencyCode: null,
                periodFrom: new DateTime(2024, 01, 01),
                periodTo: new DateTime(2024, 01, 31),
                lastMovementTimeStamp: null,
                pageIndex: 0,
                pageSize: 100,
                CancellationToken.None));
    }

    [Fact]
    public async Task Movements_GeneralError_ThrowsOpenApiException()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        // Act & Assert
        await Assert.ThrowsAsync<OpenApiException>(
            () => client.GetAccountMovements(
                accountNumber: null,
                accountCurrencyCode: null,
                periodFrom: new DateTime(2024, 02, 01),
                periodTo: new DateTime(2024, 03, 31),
                lastMovementTimeStamp: null,
                pageIndex: 0,
                pageSize: 100,
                CancellationToken.None));
    }

    private static void AssertPager(GetAccountMovementsResponse response, int expectedPageIndex, int expectedPageSize)
    {
        Assert.NotNull(response.Pager);
        var pager = Assert.Single(response.Pager);
        Assert.Multiple(
            () => Assert.Equal(expectedPageIndex, pager.PageIndex),
            () => Assert.Equal(expectedPageSize, pager.PageSize));
    }
}
