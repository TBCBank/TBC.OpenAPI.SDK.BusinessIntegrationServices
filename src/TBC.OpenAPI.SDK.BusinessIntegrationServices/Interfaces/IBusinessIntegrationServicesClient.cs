// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.Core;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public interface IBusinessIntegrationServicesClient : IOpenApiClient
    {
        Task<GetAccountStatementResponse> GetAccountStatement(
            string accountNumber,
            string accountCurrencyCode,
            DateTime periodFrom,
            DateTime periodTo,
            CancellationToken cancellationToken = default);

        Task<GetAccountMovementsResponse> GetAccountMovements(
            string accountNumber,
            string accountCurrencyCode,
            DateTime? periodFrom,
            DateTime? periodTo,
            DateTime? lastMovementTimeStamp,
            int? pageIndex,
            int? pageSize,
            CancellationToken cancellationToken = default);

        Task<GetAccountMovementByIdResponse> GetAccountMovementById(
            string id,
            CancellationToken cancellationToken = default);

        Task<ImportSingleTransfersResponse> ImportSingleTransfers(
            ImportSingleTransfersRequest request,
            CancellationToken cancellationToken = default);

        Task<ImportBatchTransferResponse> ImportBatchTransfer(
            ImportBatchTransferRequest request,
            CancellationToken cancellationToken = default);

        Task<GetTransferOrderStatusResponse> GetSingleTransferStatus(
            long transferId,
            CancellationToken cancellationToken = default);

        Task<GetTransferOrderStatusResponse> GetBatchTransferStatus(
            long batchId,
            CancellationToken cancellationToken = default);

        Task<GetSingleTransferIdResponse> GetSingleTransferId(
            string transferExternalId,
            CancellationToken cancellationToken = default);

        Task<GetBatchTransferIdResponse> GetBatchTransferId(
            string batchTransferExternalId,
            CancellationToken cancellationToken = default);
    }
}
