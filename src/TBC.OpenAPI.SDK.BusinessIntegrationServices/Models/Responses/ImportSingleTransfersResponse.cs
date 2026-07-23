// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class ImportSingleTransfersResponse
    {
        public IEnumerable<SingleTransferResult> Succeeded { get; set; }

        public IEnumerable<FailedSingleTransferResult> Failed { get; set; }
    }
}
