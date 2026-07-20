// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class ImportSingleTransfersRequest
    {
        public IEnumerable<SingleTransferOrder> SingleTransferOrders { get; set; }
    }
}
