// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.Hosting;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Services;

public abstract class ServiceIntegrationTest
{
    protected IHost App { get; }

    protected ServiceIntegrationTest(IntegrationTestHostFixture fixture)
    {
        App = fixture.App;
    }
}
