# TBC Open API Business Integration Services .NET Client

[![NuGet Version](https://img.shields.io/nuget/v/TBC.OpenAPI.SDK.BusinessIntegrationServices)](https://www.nuget.org/packages/TBC.OpenAPI.SDK.BusinessIntegrationServices)

[TBC.OpenAPI.SDK.BusinessIntegrationServices](https://www.nuget.org/packages/TBC.OpenAPI.SDK.BusinessIntegrationServices) is a .NET client SDK for the TBC Bank **Business Integration Services (BIS)** API. It provides typed access to account statements, account movements, and single/batch transfer operations, with built-in OAuth2 client-credentials authentication and token management.

The SDK is built on top of [TBC.OpenAPI.SDK.Core](https://www.nuget.org/packages/TBC.OpenAPI.SDK.Core) and is compatible with `.netstandard2.0` and `.net10.0`.

## Prerequisites

In order to use the SDK it is mandatory to have an **apikey** and **client secret** from TBC Bank's OpenAPI Devportal.

[See more details how to get apikey and secret](https://developers.tbcbank.ge/docs/get-apikey-and-secret)

Your account must be granted the relevant scopes:

* `bab_accounts` — for account statement and movement operations
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
    builder.Configuration.GetSection("BusinessIntegrationServices").Get<BusinessIntegrationServicesClientOptions>());
```

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
    .Build();

var client = factory.GetBusinessIntegrationServicesClient();
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
