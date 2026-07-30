// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class FailedSingleTransferResult
    {
        public int Position { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
