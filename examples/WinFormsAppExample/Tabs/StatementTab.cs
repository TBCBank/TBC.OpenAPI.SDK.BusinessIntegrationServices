// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using WinFormsAppExample.Controls;
using WinFormsAppExample.Infrastructure;

namespace WinFormsAppExample.Tabs;

internal sealed class StatementTab : OperationTabBase
{
    private readonly TextBox _accountNumber = new();
    private readonly TextBox _currency = new() { Text = "GEL" };
    private readonly DateTimePicker _from = new() { Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
    private readonly DateTimePicker _to = new() { Format = DateTimePickerFormat.Short, Value = DateTime.Today };

    public StatementTab(ClientHolder holder)
        : base(holder, "Get statement")
    {
        AddRow("Account number", _accountNumber);
        AddRow("Currency", _currency);
        AddRow("Period from", _from);
        AddRow("Period to", _to);
    }

    protected override string? GetValidationError()
        => string.IsNullOrWhiteSpace(_accountNumber.Text) || string.IsNullOrWhiteSpace(_currency.Text)
            ? "Account number and currency are required."
            : null;

    protected override async Task<object?> ExecuteAsync(IBusinessIntegrationServicesClient client, CancellationToken cancellationToken)
        => await client.GetAccountStatement(
            _accountNumber.Text.Trim(),
            _currency.Text.Trim(),
            _from.Value.Date,
            _to.Value.Date,
            cancellationToken);
}
