// Copyright (C) TBC Bank. All Rights Reserved.

using System.Text.Json.Serialization;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "transferType")]
    [JsonDerivedType(typeof(OwnAccountTransferOrder), nameof(TransferType.TransferToOwnAccount))]
    [JsonDerivedType(typeof(WithinBankTransferOrder), nameof(TransferType.TransferWithinBank))]
    [JsonDerivedType(typeof(OtherBankNationalCurrencyTransferOrder), nameof(TransferType.TransferToOtherBankNationalCurrency))]
    [JsonDerivedType(typeof(OtherBankForeignCurrencyTransferOrder), nameof(TransferType.TransferToOtherBankForeignCurrency))]
    [JsonDerivedType(typeof(TreasuryTransferOrder), nameof(TransferType.TreasuryTransfer))]
    public abstract class SingleTransferOrder
    {
        public string TransferExternalId { get; set; }

        public AccountIdentification DebitAccount { get; set; }

        public long? DocumentNumber { get; set; }

        public Money Amount { get; set; }

        public int? Position { get; set; }

        public string Description { get; set; }

        public string AdditionalDescription { get; set; }
    }
}
