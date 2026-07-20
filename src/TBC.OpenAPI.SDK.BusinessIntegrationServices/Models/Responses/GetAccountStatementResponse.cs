// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class GetAccountStatementResponse
    {
        public DateTime OpeningDate { get; set; }

        public decimal OpeningBalance { get; set; }

        public DateTime ClosingDate { get; set; }

        public decimal ClosingBalance { get; set; }

        public decimal CreditSum { get; set; }

        public decimal DebitSum { get; set; }

        public string Currency { get; set; }

        public string AccountNumber { get; set; }
    }
}
