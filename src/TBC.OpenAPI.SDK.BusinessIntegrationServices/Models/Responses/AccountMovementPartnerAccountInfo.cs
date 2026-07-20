// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class AccountMovementPartnerAccountInfo
    {
        public string AccountIban { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string TreasuryCode { get; set; }
        public string PartnerName { get; set; }
        public int? DocTypeId { get; set; }
        public string PersonalNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string TaxCode { get; set; }
    }
}
