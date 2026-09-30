// Copyright (C) TBC Bank. All Rights Reserved.

using System.Globalization;
using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using WinFormsAppExample.Controls;
using WinFormsAppExample.Infrastructure;

namespace WinFormsAppExample.Tabs;

internal sealed class TransferStatusTab : OperationTabBase
{
    private readonly RadioButton _single = new() { Text = "Single transfer", Checked = true, AutoSize = true };
    private readonly RadioButton _batch = new() { Text = "Batch transfer", AutoSize = true };
    private readonly TextBox _id = new();

    public TransferStatusTab(ClientHolder holder)
        : base(holder, "Get status")
    {
        var kind = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        kind.Controls.AddRange([_single, _batch]);

        AddRow("Kind", kind);
        AddRow("Transfer / batch id", _id);
    }

    protected override string? GetValidationError()
        => long.TryParse(_id.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
            ? null
            : "The id must be a whole number.";

    protected override async Task<object?> ExecuteAsync(IBusinessIntegrationServicesClient client, CancellationToken cancellationToken)
    {
        var id = long.Parse(_id.Text.Trim(), CultureInfo.InvariantCulture);

        return _single.Checked
            ? await client.GetSingleTransferStatus(id, cancellationToken)
            : await client.GetBatchTransferStatus(id, cancellationToken);
    }
}
