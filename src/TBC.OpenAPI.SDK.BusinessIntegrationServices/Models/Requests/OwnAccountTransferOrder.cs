// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class OwnAccountTransferOrder : SingleTransferOrder
    {
        public AccountIdentification CreditAccount { get; set; }
    }
}
