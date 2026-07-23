// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class OtherBankForeignCurrencyTransferOrder : SingleTransferOrder
    {
        public AccountIdentification CreditAccount { get; set; }

        public string BeneficiaryName { get; set; }

        public string BeneficiaryAddress { get; set; }

        public string BeneficiaryBankCode { get; set; }

        public string BeneficiaryBankName { get; set; }

        public string IntermediaryBankCode { get; set; }

        public string IntermediaryBankName { get; set; }

        public string ChargeDetails { get; set; }
    }
}
