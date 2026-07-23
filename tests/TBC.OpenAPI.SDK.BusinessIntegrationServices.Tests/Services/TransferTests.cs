// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.DependencyInjection;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Services;

[Collection(IntegrationTestCollection.Name)]
public class TransferTests : ServiceIntegrationTest
{
    private const string DebitAccountNumber = "GE29TB7777777777777777";
    private const string OwnCreditAccountNumber = "GE29TB9999999999999999";
    private const string WithinBankCreditAccountNumber = "GE44TB0600051509630891";
    private const string OtherBankNationalCreditAccountNumber = "GE90BG0000000589021505";
    private const string ForeignCreditAccountNumber = "CZ6508000000192000145399";

    [Fact]
    public async Task SingleTransfer_ToOwnAccount_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var request = new ImportSingleTransfersRequest
        {
            SingleTransferOrders =
            [
                new OwnAccountTransferOrder
                {
                    TransferExternalId = NewExternalId(),
                    DocumentNumber = 10001,
                    Position = 1,
                    DebitAccount = new AccountIdentification
                    {
                        AccountNumber = DebitAccountNumber,
                        AccountCurrencyCode = "GEL"
                    },
                    Amount = new Money { Amount = 3.45M, Currency = "GEL" },
                    Description = "Own account transfer",
                    AdditionalDescription = "Test transfer",
                    CreditAccount = new AccountIdentification
                    {
                        AccountNumber = OwnCreditAccountNumber,
                        AccountCurrencyCode = "GEL"
                    }
                }
            ]
        };

        // Act
        var response = await client.ImportSingleTransfers(request, CancellationToken.None);

        // Assert
        AssertSingleTransferCreated(response);
    }

    [Fact]
    public async Task SingleTransfer_Treasury_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var request = new ImportSingleTransfersRequest
        {
            SingleTransferOrders =
            [
                new TreasuryTransferOrder
                {
                    TransferExternalId = NewExternalId(),
                    DocumentNumber = 10001,
                    Position = 1,
                    DebitAccount = new AccountIdentification
                    {
                        AccountNumber = DebitAccountNumber,
                        AccountCurrencyCode = "GEL"
                    },
                    Amount = new Money { Amount = 1, Currency = "GEL" },
                    Description = "Treasury payment",
                    AdditionalDescription = "Test treasury transfer",
                    TreasuryCode = "101001000"
                }
            ]
        };

        // Act
        var response = await client.ImportSingleTransfers(request, CancellationToken.None);

        // Assert
        AssertSingleTransferCreated(response);
    }

    [Fact]
    public async Task SingleTransfer_TreasuryWithTaxpayer_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var request = new ImportSingleTransfersRequest
        {
            SingleTransferOrders =
            [
                new TreasuryTransferOrder
                {
                    TransferExternalId = NewExternalId(),
                    DocumentNumber = 10001,
                    Position = 1,
                    DebitAccount = new AccountIdentification
                    {
                        AccountNumber = DebitAccountNumber,
                        AccountCurrencyCode = "GEL"
                    },
                    Amount = new Money { Amount = 1, Currency = "GEL" },
                    Description = "Treasury payment",
                    AdditionalDescription = "Test treasury transfer",
                    TaxpayerCode = "123456789",
                    TaxpayerName = "Nino Kvimsadze",
                    TreasuryCode = "101001000"
                }
            ]
        };

        // Act
        var response = await client.ImportSingleTransfers(request, CancellationToken.None);

        // Assert
        AssertSingleTransferCreated(response);
    }

    [Fact]
    public async Task SingleTransfer_WithinBank_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var request = new ImportSingleTransfersRequest
        {
            SingleTransferOrders =
            [
                new WithinBankTransferOrder
                {
                    TransferExternalId = NewExternalId(),
                    DocumentNumber = 10001,
                    Position = 1,
                    DebitAccount = new AccountIdentification
                    {
                        AccountNumber = DebitAccountNumber,
                        AccountCurrencyCode = "GEL"
                    },
                    Amount = new Money { Amount = 3.45M, Currency = "GEL" },
                    Description = "Transfer Within Bank",
                    AdditionalDescription = "Test transfer",
                    BeneficiaryName = "Test Beneficiary",
                    CreditAccount = new AccountIdentification
                    {
                        AccountNumber = OwnCreditAccountNumber,
                        AccountCurrencyCode = "GEL"
                    }
                }
            ]
        };

        // Act
        var response = await client.ImportSingleTransfers(request, CancellationToken.None);

        // Assert
        AssertSingleTransferCreated(response);
    }

    [Fact]
    public async Task SingleTransfer_ToOtherBankNationalCurrency_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var request = new ImportSingleTransfersRequest
        {
            SingleTransferOrders =
            [
                new OtherBankNationalCurrencyTransferOrder
                {
                    TransferExternalId = NewExternalId(),
                    DocumentNumber = 10002,
                    Position = 1,
                    DebitAccount = new AccountIdentification
                    {
                        AccountNumber = DebitAccountNumber,
                        AccountCurrencyCode = "GEL"
                    },
                    Amount = new Money { Amount = 100, Currency = "GEL" },
                    Description = "National currency transfer",
                    AdditionalDescription = "Test transfer to other bank",
                    CreditAccount = new AccountIdentification
                    {
                        AccountNumber = OtherBankNationalCreditAccountNumber,
                        AccountCurrencyCode = "GEL"
                    },
                    BeneficiaryName = "Test Beneficiary",
                    BeneficiaryTaxCode = "12345678901"
                }
            ]
        };

        // Act
        var response = await client.ImportSingleTransfers(request, CancellationToken.None);

        // Assert
        AssertSingleTransferCreated(response);
    }

    [Fact]
    public async Task SingleTransfer_ToOtherBankForeignCurrency_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var request = new ImportSingleTransfersRequest
        {
            SingleTransferOrders =
            [
                new OtherBankForeignCurrencyTransferOrder
                {
                    TransferExternalId = NewExternalId(),
                    DocumentNumber = 0,
                    Position = 1,
                    DebitAccount = new AccountIdentification
                    {
                        AccountNumber = DebitAccountNumber,
                        AccountCurrencyCode = "EUR"
                    },
                    Amount = new Money { Amount = 1, Currency = "EUR" },
                    Description = "Test transfer",
                    AdditionalDescription = "Postman test",
                    CreditAccount = new AccountIdentification
                    {
                        AccountNumber = ForeignCreditAccountNumber,
                        AccountCurrencyCode = "EUR"
                    },
                    BeneficiaryName = "Beneficiary Name",
                    BeneficiaryAddress = "Beneficiary Address",
                    BeneficiaryBankCode = "0800",
                    BeneficiaryBankName = "Czech Bank",
                    IntermediaryBankCode = "string",
                    IntermediaryBankName = "string",
                    ChargeDetails = "SHA"
                }
            ]
        };

        // Act
        var response = await client.ImportSingleTransfers(request, CancellationToken.None);

        // Assert
        AssertSingleTransferCreated(response);
    }

    [Fact]
    public async Task BatchTransfer_Import_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var request = CreateBatchTransferRequest(NewExternalId());

        // Act
        var response = await client.ImportBatchTransfer(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.BatchTransferId > 0, "Expected a positive batch transfer id.");
    }

    [Fact]
    public async Task SingleTransfer_GetStatus_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var createResponse = await client.ImportSingleTransfers(
            CreateOwnAccountTransferRequest(NewExternalId()),
            CancellationToken.None);
        var transferId = createResponse.Succeeded.First().TransferId;

        // Act
        var response = await client.GetSingleTransferStatus(transferId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Finished", response.Status);
    }

    [Fact]
    public async Task BatchTransfer_GetStatus_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var createResponse = await client.ImportBatchTransfer(
            CreateBatchTransferRequest(NewExternalId()),
            CancellationToken.None);

        // Act
        var response = await client.GetBatchTransferStatus(createResponse.BatchTransferId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Multiple(
            () => Assert.Equal("FINISHED", response.Status),
            () => Assert.Null(response.SingleTransferData),
            () => Assert.NotNull(response.BatchTransferData),
            () => Assert.NotEmpty(response.BatchTransferData));
        Assert.All(response.BatchTransferData, transferData => Assert.Multiple(
            () => Assert.True(transferData.Position > 0, "Expected a positive position."),
            () => Assert.False(string.IsNullOrEmpty(transferData.TransferId), "Expected a transfer id."),
            () => Assert.Equal("Finished", transferData.TransferStatus),
            () => Assert.Empty(transferData.ErrorDetailEN),
            () => Assert.Empty(transferData.ErrorDetailGE),
            () => Assert.Null(transferData.FailureReasonEn),
            () => Assert.Null(transferData.FailureReasonGe)));
    }

    [Fact]
    public async Task SingleTransfer_GetIdByExternalId_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var externalId = NewExternalId();
        await client.ImportSingleTransfers(
            CreateOwnAccountTransferRequest(externalId),
            CancellationToken.None);

        // Act
        var response = await client.GetSingleTransferId(externalId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.TransferId > 0, "Expected a positive transfer id.");
    }

    [Fact]
    public async Task BatchTransfer_GetIdByExternalId_Succeeds()
    {
        // Arrange
        using var scope = App.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IBusinessIntegrationServicesClient>();

        var externalId = NewExternalId();
        await client.ImportBatchTransfer(
            CreateBatchTransferRequest(externalId),
            CancellationToken.None);

        // Act
        var response = await client.GetBatchTransferId(externalId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.BatchId > 0, "Expected a positive batch id.");
    }

    private static string NewExternalId() => Guid.NewGuid().ToString("N");

    private static void AssertSingleTransferCreated(ImportSingleTransfersResponse response)
    {
        Assert.NotNull(response);
        Assert.NotNull(response.Succeeded);
        var result = Assert.Single(response.Succeeded);
        Assert.Multiple(
            () => Assert.Equal(1, result.Position),
            () => Assert.True(result.TransferId > 0, "Expected a positive transfer id."));
    }

    private static ImportSingleTransfersRequest CreateOwnAccountTransferRequest(string externalId)
        => new()
        {
            SingleTransferOrders =
            [
                new OwnAccountTransferOrder
                {
                    TransferExternalId = externalId,
                    DocumentNumber = 10001,
                    Position = 1,
                    DebitAccount = new AccountIdentification
                    {
                        AccountNumber = DebitAccountNumber,
                        AccountCurrencyCode = "GEL"
                    },
                    Amount = new Money { Amount = 3.45M, Currency = "GEL" },
                    Description = "Own account transfer",
                    AdditionalDescription = "Test transfer",
                    CreditAccount = new AccountIdentification
                    {
                        AccountNumber = OwnCreditAccountNumber,
                        AccountCurrencyCode = "GEL"
                    }
                }
            ]
        };

    private static ImportBatchTransferRequest CreateBatchTransferRequest(string externalId)
        => new()
        {
            BatchTransferExternalId = externalId,
            BatchName = "NKvimsadze",
            DebitAccount = new AccountIdentification
            {
                AccountNumber = DebitAccountNumber,
                AccountCurrencyCode = "GEL"
            },
            TransferOrders =
            [
                new BatchTransferOrder
                {
                    TransferType = TransferType.TransferWithinBank,
                    DocumentNumber = 1,
                    Position = 1,
                    Amount = new Money { Amount = 1, Currency = "GEL" },
                    Description = "transfer within bank",
                    AdditionalDescription = "string",
                    BeneficiaryName = "ალტა1",
                    CreditAccount = new AccountIdentification
                    {
                        AccountNumber = WithinBankCreditAccountNumber,
                        AccountCurrencyCode = "GEL"
                    }
                },
                new BatchTransferOrder
                {
                    TransferType = TransferType.TransferToOwnAccount,
                    DocumentNumber = 2,
                    Position = 2,
                    Amount = new Money { Amount = 2, Currency = "GEL" },
                    Description = "transfer to own account",
                    AdditionalDescription = "string",
                    CreditAccount = new AccountIdentification
                    {
                        AccountNumber = "GE41TB7849963286076183",
                        AccountCurrencyCode = "GEL"
                    }
                },
                new BatchTransferOrder
                {
                    TransferType = TransferType.TransferToOtherBankNationalCurrency,
                    DocumentNumber = 3,
                    Position = 3,
                    Amount = new Money { Amount = 30, Currency = "GEL" },
                    Description = "transfer to other bank",
                    AdditionalDescription = "string",
                    BeneficiaryName = "Other Bank Beneficiary",
                    BeneficiaryTaxCode = "123456789",
                    BeneficiaryBankCode = "TB",
                    BeneficiaryBankName = "TBC Bank",
                    CreditAccount = new AccountIdentification
                    {
                        AccountNumber = OtherBankNationalCreditAccountNumber,
                        AccountCurrencyCode = "GEL"
                    }
                }
            ]
        };
}
