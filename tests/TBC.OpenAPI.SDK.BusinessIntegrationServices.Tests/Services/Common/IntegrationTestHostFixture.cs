// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Services;

/// <summary>
/// Builds the integration-test host once and shares it across every test in the
/// collection. Because the OAuth token cache is registered as a singleton within
/// the host, sharing a single host lets all tests reuse the same cached access
/// token instead of requesting a fresh one per test (which caused random
/// network/SSL failures against the real back-end service).
/// </summary>
public sealed class IntegrationTestHostFixture : IDisposable
{
    public IHost App { get; }

    public IntegrationTestHostFixture()
    {
        App = BuildHost();
    }

    public void Dispose()
    {
        App.Dispose();
    }

    private static IHost BuildHost()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Configuration.AddUserSecrets<IntegrationTestHostFixture>();
        builder.Configuration.AddEnvironmentVariables();

        var options = builder.Configuration
            .GetSection("BusinessIntegrationServices")
            .Get<BusinessIntegrationServicesClientOptions>()!;

        builder.Services.AddBusinessIntegrationServicesClient(options)
            .UseInMemoryCache();

        return builder.Build();
    }
}
