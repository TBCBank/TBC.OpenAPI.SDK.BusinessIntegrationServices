// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using WinFormsAppExample.Controls;
using WinFormsAppExample.Infrastructure;

namespace WinFormsAppExample.Tabs;

internal sealed class MovementByIdTab : OperationTabBase
{
    private readonly TextBox _id = new();

    public MovementByIdTab(ClientHolder holder)
        : base(holder, "Get movement")
    {
        AddRow("Movement id", _id);
    }

    protected override string? GetValidationError()
        => string.IsNullOrWhiteSpace(_id.Text) ? "Movement id is required." : null;

    protected override async Task<object?> ExecuteAsync(IBusinessIntegrationServicesClient client, CancellationToken cancellationToken)
        => await client.GetAccountMovementById(_id.Text.Trim(), cancellationToken);
}
