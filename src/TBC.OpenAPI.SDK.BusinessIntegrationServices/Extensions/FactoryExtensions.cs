// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.DependencyInjection;
using TBC.OpenAPI.SDK.Core;
using TBC.OpenAPI.SDK.Core.Authentication;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions
{
    public static class FactoryExtensions
    {
        public static OpenApiClientOAuthTokenCachingBuilder<BusinessIntegrationServicesClient> AddBusinessIntegrationServicesClient(
            this OpenApiClientFactoryBuilder builder,
            BusinessIntegrationServicesClientOptions options)
            => AddBusinessIntegrationServicesClient(builder, options, null, null, null);

        public static OpenApiClientOAuthTokenCachingBuilder<BusinessIntegrationServicesClient> AddBusinessIntegrationServicesClient(
            this OpenApiClientFactoryBuilder builder,
            BusinessIntegrationServicesClientOptions options,
            Action<HttpClient> configureClient = null,
            Func<HttpClientHandler> configureHttpMessageHandler = null,
            Action<IHttpClientBuilder> configurePipeline = null)
        {
#if NET
            ArgumentNullException.ThrowIfNull(builder);
#else
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
#endif

            return builder.AddClient<IBusinessIntegrationServicesClient, BusinessIntegrationServicesClient, BusinessIntegrationServicesClientOptions>(
                options,
                configureClient,
                configureHttpMessageHandler)
                .AddOAuthTokenCaching<BusinessIntegrationServicesClient>(configurePipeline);
        }

        public static IBusinessIntegrationServicesClient GetBusinessIntegrationServicesClient(
            this OpenApiClientFactory factory)
        {
#if NET
            ArgumentNullException.ThrowIfNull(factory);
#else
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));
#endif

            return factory.GetOpenApiClient<IBusinessIntegrationServicesClient>();
        }
    }
}
