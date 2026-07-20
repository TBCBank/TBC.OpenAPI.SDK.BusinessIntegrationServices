// Copyright (C) TBC Bank. All Rights Reserved.

using System.Text.Json.Serialization;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransferType
    {
        TransferToOwnAccount,
        TransferWithinBank,
        TransferToOtherBankNationalCurrency,
        TransferToOtherBankForeignCurrency,
        TreasuryTransfer
    }
}
