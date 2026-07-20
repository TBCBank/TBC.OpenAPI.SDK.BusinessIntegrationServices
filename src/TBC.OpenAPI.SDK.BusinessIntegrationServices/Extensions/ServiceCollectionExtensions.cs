// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.DependencyInjection;
using TBC.OpenAPI.SDK.Core.Extensions;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessIntegrationServicesClient(
            this IServiceCollection services,
            BusinessIntegrationServicesClientOptions options)
            => AddBusinessIntegrationServicesClient(services, options, null, null);

        public static IServiceCollection AddBusinessIntegrationServicesClient(
            this IServiceCollection services,
            BusinessIntegrationServicesClientOptions options,
            Action<HttpClient> configureClient = null,
            Func<HttpClientHandler> configureHttpMessageHandler = null)
        {
            services.AddOpenApiClient<IBusinessIntegrationServicesClient, BusinessIntegrationServicesClient, BusinessIntegrationServicesClientOptions>(
                options,
                configureClient,
                configureHttpMessageHandler);

            return services;
        }
    }
}
