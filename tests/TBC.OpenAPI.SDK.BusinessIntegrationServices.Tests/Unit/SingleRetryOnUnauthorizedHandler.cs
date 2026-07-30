// Copyright (C) TBC Bank. All Rights Reserved.

using System.Net;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Unit
{
    /// <summary>
    /// A minimal consumer-supplied retry handler used to prove that a caller can turn the SDK's
    /// token eviction on <c>401</c> into an actual retry without the SDK depending on Polly or
    /// implementing any retry logic itself.
    /// <para>
    /// Registered <em>outside</em> the OAuth handler through the client's <c>configurePipeline</c>
    /// hook, so a retried attempt re-enters token handling and picks up a token freshly acquired
    /// after the eviction. Each attempt is sent on a fresh clone of the request because the OAuth
    /// handler consumes the scope marker header and the request content is consumed when it is sent.
    /// </para>
    /// <para>
    /// Being outermost, the handler also sees the token-endpoint requests the OAuth handler triggers;
    /// those return <c>200</c> and so are never retried. Only the business request's <c>401</c>
    /// triggers a retry, which makes <see cref="RetryCount"/> deterministic.
    /// </para>
    /// </summary>
    internal sealed class SingleRetryOnUnauthorizedHandler : DelegatingHandler
    {
        private int _retryCount;

        /// <summary>Number of times a <c>401</c> response triggered a retry.</summary>
        public int RetryCount => _retryCount;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var buffered = await BufferContentAsync(request).ConfigureAwait(false);

            using (var firstAttempt = await CloneAsync(request, buffered).ConfigureAwait(false))
            {
                var response = await base.SendAsync(firstAttempt, cancellationToken).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.Unauthorized)
                {
                    return response;
                }

                response.Dispose();
            }

            Interlocked.Increment(ref _retryCount);

            var retry = await CloneAsync(request, buffered).ConfigureAwait(false);
            return await base.SendAsync(retry, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<byte[]?> BufferContentAsync(HttpRequestMessage request)
        {
            if (request.Content is null)
            {
                return null;
            }

            return await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request, byte[]? bufferedContent)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version
            };

            if (bufferedContent is not null)
            {
                clone.Content = new ByteArrayContent(bufferedContent);
                foreach (var header in request.Content!.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            await Task.CompletedTask.ConfigureAwait(false);
            return clone;
        }
    }
}
