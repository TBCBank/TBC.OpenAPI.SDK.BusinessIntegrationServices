// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class TreasuryTransferOrder : SingleTransferOrder
    {
        public string TaxpayerCode { get; set; }

        public string TaxpayerName { get; set; }

        public string TreasuryCode { get; set; }
    }
}
