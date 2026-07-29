// Copyright (C) TBC Bank. All Rights Reserved.

using System.Globalization;
using TBC.OpenAPI.SDK.Core;
using TBC.OpenAPI.SDK.Core.Authentication;
using TBC.OpenAPI.SDK.Core.Exceptions;
using TBC.OpenAPI.SDK.Core.Models;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class BusinessIntegrationServicesClient : IBusinessIntegrationServicesClient
    {
        private const string DatePattern = "yyyy-MM-dd";
        private const string DateTimePattern = "yyyy-MM-ddTHH:mm:ss";

        private readonly IHttpHelper<BusinessIntegrationServicesClient> _http;

        public BusinessIntegrationServicesClient(IHttpHelper<BusinessIntegrationServicesClient> http)
        {
            _http = http;
        }

        public Task<GetAccountStatementResponse> GetAccountStatement(
            string accountNumber,
            string accountCurrencyCode,
            DateTime periodFrom,
            DateTime periodTo,
            CancellationToken cancellationToken = default)
        {
            var query = new QueryParamCollection
            {
                ["AccountNumber"] = accountNumber,
                ["AccountCurrencyCode"] = accountCurrencyCode,
                ["PeriodFrom"] = periodFrom.ToString(DatePattern, CultureInfo.InvariantCulture),
                ["PeriodTo"] = periodTo.ToString(DatePattern, CultureInfo.InvariantCulture)
            };

            return CallGetAsync<GetAccountStatementResponse>(
                $"{Constants.ApiPathPrefix}/accounts/statements",
                query,
                Constants.AccountsScope,
                cancellationToken);
        }

        public Task<GetAccountMovementsResponse> GetAccountMovements(
            string accountNumber,
            string accountCurrencyCode,
            DateTime? periodFrom,
            DateTime? periodTo,
            DateTime? lastMovementTimeStamp,
            int? pageIndex,
            int? pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = new QueryParamCollection
            {
                ["AccountNumber"] = accountNumber,
                ["AccountCurrencyCode"] = accountCurrencyCode,
                ["PageIndex"] = pageIndex,
                ["PageSize"] = pageSize
            };

            if (periodFrom.HasValue)
            {
                query["PeriodFrom"] = periodFrom.Value.ToString(DatePattern, CultureInfo.InvariantCulture);
            }

            if (periodTo.HasValue)
            {
                query["PeriodTo"] = periodTo.Value.ToString(DatePattern, CultureInfo.InvariantCulture);
            }

            if (lastMovementTimeStamp.HasValue)
            {
                query["LastMovementTimeStamp"] = lastMovementTimeStamp.Value.ToString(DateTimePattern, CultureInfo.InvariantCulture);
            }

            return CallGetAsync<GetAccountMovementsResponse>(
                $"{Constants.ApiPathPrefix}/accounts/movements",
                query,
                Constants.AccountsScope,
                cancellationToken);
        }

        public Task<GetAccountMovementByIdResponse> GetAccountMovementById(
            string id,
            CancellationToken cancellationToken = default)
            => CallGetAsync<GetAccountMovementByIdResponse>(
                $"{Constants.ApiPathPrefix}/accounts/movements/{id}",
                null,
                Constants.AccountsScope,
                cancellationToken);

        public Task<ImportSingleTransfersResponse> ImportSingleTransfers(
            ImportSingleTransfersRequest request,
            CancellationToken cancellationToken = default)
            => CallPostAsync<ImportSingleTransfersRequest, ImportSingleTransfersResponse>(
                $"{Constants.ApiPathPrefix}/transfers/single",
                request,
                Constants.TransfersScope,
                cancellationToken);

        public Task<ImportBatchTransferResponse> ImportBatchTransfer(
            ImportBatchTransferRequest request,
            CancellationToken cancellationToken = default)
            => CallPostAsync<ImportBatchTransferRequest, ImportBatchTransferResponse>(
                $"{Constants.ApiPathPrefix}/transfers/batch",
                request,
                Constants.TransfersScope,
                cancellationToken);

        public Task<GetTransferOrderStatusResponse> GetSingleTransferStatus(
            long transferId,
            CancellationToken cancellationToken = default)
            => CallGetAsync<GetTransferOrderStatusResponse>(
                $"{Constants.ApiPathPrefix}/transfers/single/{transferId.ToString(CultureInfo.InvariantCulture)}/status",
                null,
                Constants.TransfersScope,
                cancellationToken);

        public Task<GetTransferOrderStatusResponse> GetBatchTransferStatus(
            long batchId,
            CancellationToken cancellationToken = default)
            => CallGetAsync<GetTransferOrderStatusResponse>(
                $"{Constants.ApiPathPrefix}/transfers/batch/{batchId.ToString(CultureInfo.InvariantCulture)}/status",
                null,
                Constants.TransfersScope,
                cancellationToken);

        public Task<GetSingleTransferIdResponse> GetSingleTransferId(
            string transferExternalId,
            CancellationToken cancellationToken = default)
            => CallGetAsync<GetSingleTransferIdResponse>(
                $"{Constants.ApiPathPrefix}/transfers/single/{transferExternalId}",
                null,
                Constants.TransfersScope,
                cancellationToken);

        public Task<GetBatchTransferIdResponse> GetBatchTransferId(
            string batchTransferExternalId,
            CancellationToken cancellationToken = default)
            => CallGetAsync<GetBatchTransferIdResponse>(
                $"{Constants.ApiPathPrefix}/transfers/batch/{batchTransferExternalId}",
                null,
                Constants.TransfersScope,
                cancellationToken);

        private async Task<TResult> CallGetAsync<TResult>(
            string path,
            QueryParamCollection query,
            string scope,
            CancellationToken cancellationToken)
        {
            var headers = BuildScopeHeaders(scope);
            var response = await _http.GetJsonAsync<TResult>(path, query, headers, cancellationToken).ConfigureAwait(false);

            return EnsureSuccess(response);
        }

        private async Task<TResult> CallPostAsync<TRequest, TResult>(
            string path,
            TRequest data,
            string scope,
            CancellationToken cancellationToken)
        {
            var headers = BuildScopeHeaders(scope);
            var response = await _http.PostJsonAsync<TRequest, TResult>(path, data, null, headers, cancellationToken).ConfigureAwait(false);

            return EnsureSuccess(response);
        }

        private static HeaderParamCollection BuildScopeHeaders(string scope)
        {
            return new HeaderParamCollection
            {
                [OAuthConstants.ScopeHeaderName] = scope
            };
        }

        private static TResult EnsureSuccess<TResult>(ApiResponse<TResult> response)
        {
            if (!response.IsSuccess)
            {
                throw new OpenApiException(
                    response.Problem?.Title ?? "Unexpected error occurred",
                    response.Exception);
            }

            return response.Data;
        }
    }
}
