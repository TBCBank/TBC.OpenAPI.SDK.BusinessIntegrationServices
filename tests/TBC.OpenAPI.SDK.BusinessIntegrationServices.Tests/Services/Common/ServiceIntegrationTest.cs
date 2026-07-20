// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TBC.OpenAPI.SDK.BusinessIntegrationServices.Extensions;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Services;

public abstract class ServiceIntegrationTest : IDisposable
{
    protected IHost App { get; init; }

    protected ServiceIntegrationTest()
    {
        App = BuildHost();
    }

    public void Dispose()
    {
        App.Dispose();
        GC.SuppressFinalize(this);
    }

    private static IHost BuildHost()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Configuration.AddUserSecrets<ServiceIntegrationTest>();
        builder.Configuration.AddEnvironmentVariables();

        var options = builder.Configuration
            .GetSection("BusinessIntegrationServices")
            .Get<BusinessIntegrationServicesClientOptions>()!;

        builder.Services.AddBusinessIntegrationServicesClient(options);

        return builder.Build();
    }
}
