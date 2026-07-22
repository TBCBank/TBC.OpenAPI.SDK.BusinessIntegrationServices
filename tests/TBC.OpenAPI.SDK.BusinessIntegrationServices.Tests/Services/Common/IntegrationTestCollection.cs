// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Services;

/// <summary>
/// Groups all integration tests into a single xUnit collection so they run
/// sequentially. These tests make real calls to the back-end service and
/// parallel requests usually fail.
/// </summary>
[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection
{
    public const string Name = "Business Integration Services";
}
