// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class GetTransferOrderStatusResponse
    {
        public string Status { get; set; }

        public TransferStatusInfo SingleTransferData { get; set; }

        public IEnumerable<TransferStatusInfo> BatchTransferData { get; set; }
    }
}
