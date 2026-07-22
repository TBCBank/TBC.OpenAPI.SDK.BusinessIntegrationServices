// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class BatchTransferOrder
    {
        public TransferType? TransferType { get; set; }

        public string TransferExternalId { get; set; }

        public AccountIdentification CreditAccount { get; set; }

        public long? DocumentNumber { get; set; }

        public Money Amount { get; set; }

        public int? Position { get; set; }

        public string Description { get; set; }

        public string AdditionalDescription { get; set; }

        public string PersonalNumber { get; set; }

        public string BeneficiaryName { get; set; }

        public string BeneficiaryTaxCode { get; set; }

        public string BeneficiaryAddress { get; set; }

        public string BeneficiaryBankCode { get; set; }

        public string BeneficiaryBankName { get; set; }

        public string IntermediaryBankCode { get; set; }

        public string IntermediaryBankName { get; set; }

        public string ChargeDetails { get; set; }

        public string TaxpayerCode { get; set; }

        public string TaxpayerName { get; set; }

        public string TreasuryCode { get; set; }
    }
}
