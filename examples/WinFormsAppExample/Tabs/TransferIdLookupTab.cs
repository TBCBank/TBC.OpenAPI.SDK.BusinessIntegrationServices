// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using WinFormsAppExample.Controls;
using WinFormsAppExample.Infrastructure;

namespace WinFormsAppExample.Tabs;

internal sealed class TransferIdLookupTab : OperationTabBase
{
    private readonly RadioButton _single = new() { Text = "Single transfer", Checked = true, AutoSize = true };
    private readonly RadioButton _batch = new() { Text = "Batch transfer", AutoSize = true };
    private readonly TextBox _externalId = new();

    public TransferIdLookupTab(ClientHolder holder)
        : base(holder, "Get id")
    {
        var kind = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        kind.Controls.AddRange([_single, _batch]);

        AddRow("Kind", kind);
        AddRow("External id", _externalId);
    }

    protected override string? GetValidationError()
        => string.IsNullOrWhiteSpace(_externalId.Text) ? "External id is required." : null;

    protected override async Task<object?> ExecuteAsync(IBusinessIntegrationServicesClient client, CancellationToken cancellationToken)
    {
        var externalId = _externalId.Text.Trim();

        return _single.Checked
            ? await client.GetSingleTransferId(externalId, cancellationToken)
            : await client.GetBatchTransferId(externalId, cancellationToken);
    }
}
