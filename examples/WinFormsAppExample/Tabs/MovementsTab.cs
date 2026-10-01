// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using WinFormsAppExample.Controls;
using WinFormsAppExample.Infrastructure;

namespace WinFormsAppExample.Tabs;

internal sealed class MovementsTab : OperationTabBase
{
    private readonly TextBox _accountNumber = new();
    private readonly TextBox _currency = new() { Text = "GEL" };
    private readonly DateTimePicker _from = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Checked = false };
    private readonly DateTimePicker _to = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Checked = false };
    private readonly DateTimePicker _lastTimestamp = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm:ss", ShowCheckBox = true, Checked = false };
    private readonly NumericUpDown _pageIndex = new() { Maximum = 100000 };
    private readonly CheckBox _usePageIndex = new() { Text = "Use", AutoSize = true };
    private readonly NumericUpDown _pageSize = new() { Minimum = 1, Maximum = 1000, Value = 100 };
    private readonly CheckBox _usePageSize = new() { Text = "Use", AutoSize = true };

    public MovementsTab(ClientHolder holder)
        : base(holder, "Get movements")
    {
        AddRow("Account number", _accountNumber);
        AddRow("Currency", _currency);
        AddRow("Period from (optional)", _from);
        AddRow("Period to (optional)", _to);
        AddRow("Last movement timestamp (optional)", _lastTimestamp);
        AddRow("Page index", WithCheckBox(_pageIndex, _usePageIndex));
        AddRow("Page size", WithCheckBox(_pageSize, _usePageSize));
    }

    protected override string? GetValidationError()
        => string.IsNullOrWhiteSpace(_accountNumber.Text) || string.IsNullOrWhiteSpace(_currency.Text)
            ? "Account number and currency are required."
            : null;

    protected override async Task<object?> ExecuteAsync(IBusinessIntegrationServicesClient client, CancellationToken cancellationToken)
        => await client.GetAccountMovements(
            _accountNumber.Text.Trim(),
            _currency.Text.Trim(),
            _from.Checked ? _from.Value.Date : null,
            _to.Checked ? _to.Value.Date : null,
            _lastTimestamp.Checked ? _lastTimestamp.Value : null,
            _usePageIndex.Checked ? (int)_pageIndex.Value : null,
            _usePageSize.Checked ? (int)_pageSize.Value : null,
            cancellationToken);

    private static FlowLayoutPanel WithCheckBox(NumericUpDown numeric, CheckBox check)
    {
        var panel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        check.Margin = new Padding(3, 6, 3, 3);
        panel.Controls.AddRange([numeric, check]);
        return panel;
    }
}
