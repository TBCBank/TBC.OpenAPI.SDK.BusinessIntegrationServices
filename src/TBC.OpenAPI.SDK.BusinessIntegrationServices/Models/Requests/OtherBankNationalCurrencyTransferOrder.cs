// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class OtherBankNationalCurrencyTransferOrder : SingleTransferOrder
    {
        public AccountIdentification CreditAccount { get; set; }

        public string BeneficiaryName { get; set; }

        public string BeneficiaryTaxCode { get; set; }
    }
}
