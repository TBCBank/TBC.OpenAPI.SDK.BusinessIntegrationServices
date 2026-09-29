# TBC Open API Business Integration Services .NET Client

[![NuGet Version](https://img.shields.io/nuget/v/TBC.OpenAPI.SDK.BusinessIntegrationServices)](https://www.nuget.org/packages/TBC.OpenAPI.SDK.BusinessIntegrationServices)

[TBC.OpenAPI.SDK.BusinessIntegrationServices](https://www.nuget.org/packages/TBC.OpenAPI.SDK.BusinessIntegrationServices) is a .NET client SDK for the TBC Bank **Business Integration Services (BIS)** API. It provides typed access to account balances, statements, movements, and single/batch transfer operations, with built-in OAuth2 client-credentials authentication and token management.

The SDK is built on top of [TBC.OpenAPI.SDK.Core](https://www.nuget.org/packages/TBC.OpenAPI.SDK.Core) and is compatible with `.netstandard2.0` and `.net10.0`.

## Prerequisites

In order to use the SDK it is mandatory to have an **apikey** and **client secret** from TBC Bank's OpenAPI Devportal.

[See more details how to get apikey and secret](https://developers.tbcbank.ge/docs/get-apikey-and-secret)

Your account must be granted the relevant scopes:

* `bab_accounts` — for account balance, statement, and movement operations
* `bab_transfers` — for transfer operations

## Configuration

The client is configured through `BusinessIntegrationServicesClientOptions`:

* **BaseUrl** (string) — *Optional* \
  The BIS API root endpoint. Defaults to **production** (`https://api.tbcbank.ge/`) when not supplied, so you normally only need to provide credentials.
  * Production (default): `https://api.tbcbank.ge/`
  * Test: `https://test-api.tbcbank.ge/`
* **ApiKey** (string) — **Required** \
  Your API key (consumer key) provided by TBC Bank.
* **ClientSecret** (string) — **Required** \
  Your API secret (consumer secret) provided by TBC Bank.

## .NET Core Usage

First, configure the `appsettings.json` file with the TBC portal **apikey** and **client secret**. `BaseUrl` is optional and defaults to production — supply it only to target the test environment:

```json
{
  "BusinessIntegrationServices": {
    "ApiKey": "{apikey}",
    "ClientSecret": "{clientSecret}"
  }
}
```

Then register the client as a dependency injection service in `Program.cs`:

```csharp
using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions;

builder.Services.AddBusinessIntegrationServicesClient(
    builder.Configuration.GetSection("BusinessIntegrationServices").Get<BusinessIntegrationServicesClientOptions>())
    .UseInMemoryCache();
```

The client caches OAuth access tokens per scope. You must pick a cache backend when registering the
client (there is no implicit default). See [OAuth token caching](#oauth-token-caching) for the options.

After the two steps above, the setup is done and `IBusinessIntegrationServicesClient` can be injected into any container class:

```csharp
private readonly IBusinessIntegrationServicesClient _client;

public BankingController(IBusinessIntegrationServicesClient client)
{
    _client = client;
}
```

## Factory Usage (non-DI / .NET Framework)

For scenarios without a DI container, build a singleton factory (for example in `Global.asax` `Application_Start`):

```csharp
using TBC.OpenAPI.SDK.Core;
using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions;

var factory = new OpenApiClientFactoryBuilder()
    .AddBusinessIntegrationServicesClient(new BusinessIntegrationServicesClientOptions
    {
        ApiKey = "{apikey}",
        ClientSecret = "{clientSecret}"
        // BaseUrl defaults to production; set it to target the test environment:
        // BaseUrl = "https://test-api.tbcbank.ge/"
    })
    .UseInMemoryCache()
    .Build();

var client = factory.GetBusinessIntegrationServicesClient();
```

## OAuth Token Caching

The client authenticates with OAuth2 client credentials and caches the resulting access tokens per
scope. You must explicitly choose where those tokens are cached when registering the client; nothing
is selected on your behalf. Call exactly one of the following on the builder returned by
`AddBusinessIntegrationServicesClient(...)`:

* **UseInMemoryCache()** \
  Caches tokens in a private in-memory store dedicated to this client. The cache is not shared across
  processes, so in a multi-instance deployment every instance requests and caches its own tokens.

* **UseRegisteredDistributedCache()** \
  Uses the `IDistributedCache` registered in the container (for example Redis or SQL Server). Prefer
  this when running more than one instance so all instances share cached tokens.

* **UseDistributedCache(cache)** / **UseDistributedCache(factory)** \
  Uses the supplied `IDistributedCache` instance (or one built by the supplied factory).

```csharp
// Share tokens across instances using a distributed cache registered in the container:
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = "localhost:6379");

builder.Services.AddBusinessIntegrationServicesClient(options)
    .UseRegisteredDistributedCache();
```

## Retrying on 401

The client attaches a cached OAuth token to every request. When the API answers `401 Unauthorized`
the SDK **evicts** the cached token so the *next* request fetches a fresh one, but it does **not**
retry the failed request and it never renews proactively. A `401` therefore surfaces as a failed
call unless you add a retry.

The SDK deliberately ships **no retry logic** and takes **no dependency on Polly** or any resilience
library — you choose the mechanism and hook it into DI. `AddBusinessIntegrationServicesClient(...)`
takes an optional `configurePipeline` parameter for exactly this: any handler it registers is placed
*outside* the SDK's OAuth handler, which is the only position from which a retried attempt re-enters
token handling and picks up the freshly acquired token after the eviction.

> [!IMPORTANT]
> A retry handler **must clone the request on every attempt**: the OAuth handler consumes an internal
> scope marker header and the request content is consumed once it is sent, so re-sending the same
> `HttpRequestMessage` fails. `Microsoft.Extensions.Http.Resilience` clones automatically. Scope the
> retry to `401 Unauthorized`.

**With `Microsoft.Extensions.Http.Resilience` (Polly):**

```csharp
using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Polly;

builder.Services.AddBusinessIntegrationServicesClient(
        builder.Configuration.GetSection("BusinessIntegrationServices").Get<BusinessIntegrationServicesClientOptions>(),
        configurePipeline: pipeline =>
            pipeline.AddResilienceHandler("bab-401-retry", b =>
                b.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 1,
                    ShouldHandle = args => ValueTask.FromResult(
                        args.Outcome.Result?.StatusCode == HttpStatusCode.Unauthorized)
                })))
    .UseInMemoryCache();
```

The `configurePipeline` parameter is also available on the factory overload:

```csharp
var factory = new OpenApiClientFactoryBuilder()
    .AddBusinessIntegrationServicesClient(
        options,
        configurePipeline: pipeline =>
            pipeline.AddResilienceHandler("bab-401-retry", b =>
                b.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 1,
                    ShouldHandle = args => ValueTask.FromResult(
                        args.Outcome.Result?.StatusCode == HttpStatusCode.Unauthorized)
                })))
    .UseInMemoryCache()
    .Build();
```

## Account Statement Methods

* **GetAccountStatement** \
  Retrieve an account statement for a given account and currency over a date range.

  ```csharp
  var statement = await client.GetAccountStatement(
      accountNumber: "GE00TB0000000000000000",
      accountCurrencyCode: "GEL",
      periodFrom: DateTime.Today.AddDays(-30),
      periodTo: DateTime.Today,
      cancellationToken);
  ```

## Account Balance Methods

* **GetAccountBalances** \
  Retrieve the current and available balances, overdraft limit, and currency for an account.

  ```csharp
  var balance = await client.GetAccountBalances(
      accountNumber: "GE00TB0000000000000000",
      currency: "GEL",
      cancellationToken);
  ```

## Account Movement Methods

* **GetAccountMovements** \
  Retrieve a paged list of account movements (transactions). All filter parameters except paging are optional (pass `null` to omit).

  ```csharp
  var movements = await client.GetAccountMovements(
      accountNumber: "GE00TB0000000000000000",
      accountCurrencyCode: "GEL",
      periodFrom: DateTime.Today.AddDays(-7),
      periodTo: DateTime.Today,
      lastMovementTimeStamp: null,
      pageIndex: 0,
      pageSize: 50,
      cancellationToken);
  ```

* **GetAccountMovementById** \
  Retrieve a single account movement by its identifier.

  ```csharp
  var movement = await client.GetAccountMovementById("movement-id", cancellationToken);
  ```

## Transfer Methods

* **ImportSingleTransfers** \
  Import one or more single transfer orders for processing.

  ```csharp
  var result = await client.ImportSingleTransfers(new ImportSingleTransfersRequest
  {
      SingleTransferOrders = new SingleTransferOrder[]
      {
          new WithinBankTransferOrder
          {
              TransferExternalId = "ext-001",
              DebitAccount = new AccountIdentification
              {
                  AccountNumber = "GE00TB0000000000000000",
                  AccountCurrencyCode = "GEL"
              },
              CreditAccount = new AccountIdentification
              {
                  AccountNumber = "GE00TB1111111111111111",
                  AccountCurrencyCode = "GEL"
              },
              Amount = new Money { Amount = 12.60m, Currency = "GEL" },
              BeneficiaryName = "Jane Doe",
              Description = "Invoice #254"
          }
      }
  }, cancellationToken);
  ```

* **ImportBatchTransfer** \
  Import a batch transfer order containing multiple transfers grouped under a single batch.

  ```csharp
  var result = await client.ImportBatchTransfer(request, cancellationToken);
  ```

* **GetSingleTransferStatus** \
  Get the current status of a single transfer by its bank transfer id.

  ```csharp
  var status = await client.GetSingleTransferStatus(transferId, cancellationToken);
  ```

* **GetBatchTransferStatus** \
  Get the current status of a batch transfer by its bank batch id.

  ```csharp
  var status = await client.GetBatchTransferStatus(batchId, cancellationToken);
  ```

* **GetSingleTransferId** \
  Resolve the bank-assigned single transfer id from your own external id.

  ```csharp
  var id = await client.GetSingleTransferId("ext-001", cancellationToken);
  ```

* **GetBatchTransferId** \
  Resolve the bank-assigned batch transfer id from your own external id.

  ```csharp
  var id = await client.GetBatchTransferId("batch-ext-001", cancellationToken);
  ```

## Error Handling

The SDK throws `TBC.OpenAPI.SDK.Core.Exceptions.OpenApiException` when an API call fails. Wrap calls in try-catch blocks:

```csharp
using TBC.OpenAPI.SDK.Core.Exceptions;

try
{
    var statement = await client.GetAccountStatement(
        accountNumber, currencyCode, periodFrom, periodTo, cancellationToken);
    // Process successful response
}
catch (OpenApiException ex)
{
    _logger.LogError(ex, "TBC Business Integration Services error: {Message}", ex.Message);
    // Handle API error
}
```

## Requirements

- .NET Standard 2.0 compatible runtime (.NET Framework 4.6.1+ / .NET Core 2.0+ / .NET 5.0 or higher)
- Active TBC Bank Business Integration Services account with API credentials (`bab_accounts` and/or `bab_transfers` scopes)
