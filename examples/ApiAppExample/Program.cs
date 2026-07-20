// Copyright (C) TBC Bank. All Rights Reserved.

// Example: registering the Business Integration Services client in a
// Microsoft.Extensions.DependencyInjection container and consuming it from an endpoint.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBusinessIntegrationServicesClient(new BusinessIntegrationServicesClientOptions
{
    // BaseUrl points to the TEST environment. For production you do NOT need to set
    // BaseUrl at all - it defaults to the production URL (https://api.tbcbank.ge/).
    BaseUrl = "https://test-api.tbcbank.ge/",
    ApiKey = "{apikey}",
    ClientSecret = "{clientSecret}"
});

var app = builder.Build();

app.MapGet("/statement", (
    IBusinessIntegrationServicesClient client,
    string accountNumber,
    string currency,
    CancellationToken cancellationToken) =>
        client.GetAccountStatement(
            accountNumber,
            currency,
            DateTime.UtcNow.AddMonths(-1),
            DateTime.UtcNow,
            cancellationToken));

app.MapGet("/movements/{id}", (
    IBusinessIntegrationServicesClient client,
    string id,
    CancellationToken cancellationToken) =>
        client.GetAccountMovementById(id, cancellationToken));

app.Run();
