// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class TransferStatusInfo
    {
        public int Position { get; set; }

        public string TransferId { get; set; }

        public string TransferStatus { get; set; }

        public string ErrorDetailEN { get; set; }

        public string ErrorDetailGE { get; set; }

        public string FailureReasonEn { get; set; }

        public string FailureReasonGe { get; set; }
    }
}
