// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class GetAccountMovementsResponse
    {
        public IEnumerable<AccountMovementTransaction> Transactions { get; set; }

        public IEnumerable<AccountMovementsPager> Pager { get; set; }

        public int Total { get; set; }
    }
}
