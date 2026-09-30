// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using WinFormsAppExample.Controls;
using WinFormsAppExample.Infrastructure;

namespace WinFormsAppExample.Tabs;

internal sealed class BalancesTab : OperationTabBase
{
    private readonly TextBox _accountNumber = new();
    private readonly TextBox _currency = new() { Text = "GEL" };

    public BalancesTab(ClientHolder holder)
        : base(holder, "Get balances")
    {
        AddRow("Account number", _accountNumber);
        AddRow("Currency", _currency);
    }

    protected override string? GetValidationError()
        => string.IsNullOrWhiteSpace(_accountNumber.Text) || string.IsNullOrWhiteSpace(_currency.Text)
            ? "Account number and currency are required."
            : null;

    protected override async Task<object?> ExecuteAsync(IBusinessIntegrationServicesClient client, CancellationToken cancellationToken)
        => await client.GetAccountBalances(
            _accountNumber.Text.Trim(),
            _currency.Text.Trim(),
            cancellationToken);
}
