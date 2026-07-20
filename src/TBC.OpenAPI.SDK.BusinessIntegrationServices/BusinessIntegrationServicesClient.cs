// Copyright (C) TBC Bank. All Rights Reserved.

using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using TBC.OpenAPI.SDK.Core;
using TBC.OpenAPI.SDK.Core.Exceptions;
using TBC.OpenAPI.SDK.Core.Models;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class BusinessIntegrationServicesClient : IBusinessIntegrationServicesClient
    {
        private const string DatePattern = "yyyy-MM-dd";
        private const string DateTimePattern = "yyyy-MM-ddTHH:mm:ss";

        private readonly IHttpHelper<BusinessIntegrationServicesClient> _http;
        private readonly ConcurrentDictionary<string, TokenResponse> _tokens = new ConcurrentDictionary<string, TokenResponse>();

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
            var headers = await BuildAuthHeadersAsync(scope, forceRefresh: false, cancellationToken).ConfigureAwait(false);
            var response = await _http.GetJsonAsync<TResult>(path, query, headers, cancellationToken).ConfigureAwait(false);

            if (IsUnauthorized(response))
            {
                headers = await BuildAuthHeadersAsync(scope, forceRefresh: true, cancellationToken).ConfigureAwait(false);
                response = await _http.GetJsonAsync<TResult>(path, query, headers, cancellationToken).ConfigureAwait(false);
            }

            return EnsureSuccess(response);
        }

        private async Task<TResult> CallPostAsync<TRequest, TResult>(
            string path,
            TRequest data,
            string scope,
            CancellationToken cancellationToken)
        {
            var headers = await BuildAuthHeadersAsync(scope, forceRefresh: false, cancellationToken).ConfigureAwait(false);
            var response = await _http.PostJsonAsync<TRequest, TResult>(path, data, null, headers, cancellationToken).ConfigureAwait(false);

            if (IsUnauthorized(response))
            {
                headers = await BuildAuthHeadersAsync(scope, forceRefresh: true, cancellationToken).ConfigureAwait(false);
                response = await _http.PostJsonAsync<TRequest, TResult>(path, data, null, headers, cancellationToken).ConfigureAwait(false);
            }

            return EnsureSuccess(response);
        }

        private async Task<HeaderParamCollection> BuildAuthHeadersAsync(
            string scope,
            bool forceRefresh,
            CancellationToken cancellationToken)
        {
            TokenResponse token;
            if (forceRefresh || !_tokens.TryGetValue(scope, out token) || string.IsNullOrEmpty(token?.AccessToken))
            {
                token = await UpdateTokenAsync(scope, cancellationToken).ConfigureAwait(false);
            }

            return new HeaderParamCollection
            {
                ["Authorization"] = "Bearer " + token.AccessToken
            };
        }

        private async Task<TokenResponse> UpdateTokenAsync(string scope, CancellationToken cancellationToken)
        {
            var form = new UrlFormCollection
            {
                ["grant_type"] = TokenRequest.GrantType,
                ["scope"] = scope
            };

            var response = await _http
                .PostUrlFormAsync<TokenResponse>(Constants.OAuthTokenPath, form, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccess || string.IsNullOrEmpty(response.Data?.AccessToken))
            {
                throw new OpenApiException(
                    response.Problem?.Title ?? "Error occurred while getting access token",
                    response.Exception);
            }

            _tokens[scope] = response.Data;
            return response.Data;
        }

        private static bool IsUnauthorized(ApiResponseBase response)
            => response?.Problem?.Status == (int)HttpStatusCode.Unauthorized;

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
