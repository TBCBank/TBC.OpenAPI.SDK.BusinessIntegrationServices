// Copyright (C) TBC Bank. All Rights Reserved.

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices.Tests.Services;

/// <summary>
/// Groups all integration tests into a single xUnit collection so they run
/// sequentially. These tests make real calls to the back-end service and
/// parallel requests usually fail.
/// <para>
/// The collection also shares a single <see cref="IntegrationTestHostFixture"/>
/// so that every test reuses the same host and its singleton OAuth token cache,
/// avoiding a separate token request (and the associated random network
/// failures) per test.
/// </para>
/// </summary>
[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestHostFixture>
{
    public const string Name = "Business Integration Services";
}
