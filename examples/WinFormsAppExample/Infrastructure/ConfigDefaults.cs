// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.Configuration;

namespace WinFormsAppExample.Infrastructure;

internal sealed record ConfigDefaults(string? BaseUrl, string? ApiKey, string? ClientSecret)
{
    // Same section and keys as the integration tests: user secrets, then environment variables
    // (e.g. BusinessIntegrationServices__ApiKey).
    public static ConfigDefaults Load()
    {
        var section = new ConfigurationBuilder()
            .AddUserSecrets(typeof(ConfigDefaults).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build()
            .GetSection("BusinessIntegrationServices");

        return new ConfigDefaults(section["BaseUrl"], section["ApiKey"], section["ClientSecret"]);
    }
}
