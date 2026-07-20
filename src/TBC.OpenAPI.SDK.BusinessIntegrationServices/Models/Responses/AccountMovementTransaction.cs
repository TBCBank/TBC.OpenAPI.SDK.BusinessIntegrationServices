// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class AccountMovementTransaction
    {
        public string MovementId { get; set; }
        public string ExternalPaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Purpose { get; set; }
        public bool IsDebit { get; set; }
        public AccountMovementCustomerAccountInfo CustomerAccountInfo { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string ExchangeRate { get; set; }
        public string AdditionalInformation { get; set; }
        public string OpCode { get; set; }
        public AccountMovementPartnerAccountInfo PartnerAccountInfo { get; set; }
        public string IntermediaryBankCode { get; set; }
        public string IntermediaryBankName { get; set; }
        public int StatusCode { get; set; }
        public DateTime? Date { get; set; }
        public long ParentExternalPaymentId { get; set; }
        public string TransactionReference { get; set; }
        public string AdditionalDescription { get; set; }
        public string DocumentNumber { get; set; }
        public string ChargeDetail { get; set; }
        public string TaxpayerCode { get; set; }
        public string TaxpayerName { get; set; }
        public int TransactionType { get; set; }
        public string PaymentId { get; set; }
    }
}
