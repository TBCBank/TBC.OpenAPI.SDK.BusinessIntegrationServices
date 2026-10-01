// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using WinFormsAppExample.Controls;
using WinFormsAppExample.Infrastructure;

namespace WinFormsAppExample.Tabs;

internal sealed class ImportBatchTransferTab : OperationTabBase
{
    private readonly TextBox _batchExternalId = new() { Text = Guid.NewGuid().ToString() };
    private readonly TextBox _debitAccountNumber = new();
    private readonly TextBox _debitCurrency = new() { Text = "GEL" };
    private readonly TextBox _batchName = new();
    private readonly TextBox _validateReceiver = new();
    private readonly TransferOrderList _orders = new(batchMode: true);

    public ImportBatchTransferTab(ClientHolder holder)
        : base(holder, "Import batch transfer")
    {
        AddRow("Batch external id", _batchExternalId);
        AddRow("Debit account number", _debitAccountNumber);
        AddRow("Debit account currency", _debitCurrency);
        AddRow("Batch name", _batchName);
        AddRow("Validate receiver identifier", _validateReceiver);
        AddWide(new Label { Text = "Transfer orders", AutoSize = true, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(3, 12, 3, 3) });
        AddWide(_orders);
    }

    protected override string? GetValidationError()
    {
        if (string.IsNullOrWhiteSpace(_batchExternalId.Text)
            || string.IsNullOrWhiteSpace(_debitAccountNumber.Text)
            || string.IsNullOrWhiteSpace(_debitCurrency.Text))
        {
            return "Batch external id and debit account number and currency are required.";
        }

        return _orders.Count == 0 ? "Add at least one transfer order to the list." : null;
    }

    protected override async Task<object?> ExecuteAsync(IBusinessIntegrationServicesClient client, CancellationToken cancellationToken)
        => await client.ImportBatchTransfer(
            new ImportBatchTransferRequest
            {
                BatchTransferExternalId = _batchExternalId.Text.Trim(),
                DebitAccount = new AccountIdentification
                {
                    AccountNumber = _debitAccountNumber.Text.Trim(),
                    AccountCurrencyCode = _debitCurrency.Text.Trim()
                },
                BatchName = NullIfEmpty(_batchName.Text),
                ValidateReceiverIdentifier = NullIfEmpty(_validateReceiver.Text),
                TransferOrders = _orders.Orders<BatchTransferOrder>()
            },
            cancellationToken);

    private static string? NullIfEmpty(string text)
        => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}
