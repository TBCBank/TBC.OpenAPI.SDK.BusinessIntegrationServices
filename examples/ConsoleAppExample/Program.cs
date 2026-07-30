// Copyright (C) TBC Bank. All Rights Reserved.

// Example: consuming the Business Integration Services client without a DI container,
// using the OpenApiClientFactory builder from TBC.OpenAPI.SDK.Core.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions;
using TBC.OpenAPI.SDK.Core;

var factory = new OpenApiClientFactoryBuilder()
    .AddBusinessIntegrationServicesClient(new BusinessIntegrationServicesClientOptions
    {
        // BaseUrl points to the TEST environment. For production you do NOT need to set
        // BaseUrl at all - it defaults to the production URL (https://api.tbcbank.ge/).
        BaseUrl = "https://test-api.tbcbank.ge/",
        ApiKey = "{apikey}",
        ClientSecret = "{clientSecret}"
    })
    .UseInMemoryCache()
    .Build();

var client = factory.GetBusinessIntegrationServicesClient();

var statement = await client.GetAccountStatement(
    accountNumber: "GE00TB0000000000000000",
    accountCurrencyCode: "GEL",
    periodFrom: DateTime.UtcNow.AddMonths(-1),
    periodTo: DateTime.UtcNow,
    cancellationToken: CancellationToken.None);

Console.WriteLine($"Account: {statement.AccountNumber}");
Console.WriteLine($"Closing balance: {statement.ClosingBalance} {statement.Currency}");
