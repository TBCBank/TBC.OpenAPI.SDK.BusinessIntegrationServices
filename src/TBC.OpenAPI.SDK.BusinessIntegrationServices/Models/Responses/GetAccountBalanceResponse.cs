// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Models.Responses
{
    public class GetAccountBalanceResponse
    {
        public decimal CurrentBalance { get; set; }

        public decimal AvailableBalance { get; set; }

        public decimal? OverdraftLimit { get; set; }

        public string Currency { get; set; }
    }
}
