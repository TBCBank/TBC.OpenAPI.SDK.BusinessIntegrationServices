// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.BusinessIntegrationServices;
using TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions;
using TBC.OpenAPI.SDK.Core;

namespace WinFormsAppExample.Infrastructure;

internal sealed class ClientHolder
{
    private IBusinessIntegrationServicesClient? _client;

    public event EventHandler? Connected;

    public bool IsConnected => _client is not null;

    public IBusinessIntegrationServicesClient Client
        => _client ?? throw new InvalidOperationException("Not connected. Enter credentials and press Connect.");

    public static string NormalizeBaseUrl(string baseUrl)
    {
        var trimmed = baseUrl.Trim();
        return trimmed.EndsWith('/') ? trimmed : trimmed + "/";
    }

    public void Connect(string baseUrl, string apiKey, string clientSecret)
    {
        var factory = new OpenApiClientFactoryBuilder()
            .AddBusinessIntegrationServicesClient(new BusinessIntegrationServicesClientOptions
            {
                BaseUrl = NormalizeBaseUrl(baseUrl),
                ApiKey = apiKey,
                ClientSecret = clientSecret
            })
            .UseInMemoryCache()
            .Build();

        _client = factory.GetBusinessIntegrationServicesClient();
        Connected?.Invoke(this, EventArgs.Empty);
    }
}
