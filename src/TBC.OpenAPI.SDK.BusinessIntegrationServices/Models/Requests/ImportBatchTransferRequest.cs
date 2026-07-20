// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class ImportBatchTransferRequest
    {
        public string BatchTransferExternalId { get; set; }

        public AccountIdentification DebitAccount { get; set; }

        public string BatchName { get; set; }

        public string ValidateReceiverIdentifier { get; set; }

        public IEnumerable<BatchTransferOrder> TransferOrders { get; set; }
    }
}
