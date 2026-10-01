// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using WinFormsAppExample.Controls;
using WinFormsAppExample.Infrastructure;

namespace WinFormsAppExample.Tabs;

internal sealed class ImportSingleTransfersTab : OperationTabBase
{
    private readonly TransferOrderList _orders = new(batchMode: false);

    public ImportSingleTransfersTab(ClientHolder holder)
        : base(holder, "Import single transfers")
    {
        AddWide(_orders);
    }

    protected override string? GetValidationError()
        => _orders.Count == 0 ? "Add at least one transfer order to the list." : null;

    protected override async Task<object?> ExecuteAsync(IBusinessIntegrationServicesClient client, CancellationToken cancellationToken)
        => await client.ImportSingleTransfers(
            new ImportSingleTransfersRequest { SingleTransferOrders = _orders.Orders<SingleTransferOrder>() },
            cancellationToken);
}
