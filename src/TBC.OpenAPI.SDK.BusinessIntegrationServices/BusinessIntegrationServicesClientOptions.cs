// Copyright (C) TBC Bank. All Rights Reserved.

using TBC.OpenAPI.SDK.Core;

namespace TBC.OpenAPI.SDK.BusinessIntegrationServices
{
    public class BusinessIntegrationServicesClientOptions : BasicAuthOptions
    {
        public BusinessIntegrationServicesClientOptions()
        {
            BaseUrl = Constants.ProductionBaseUrl;
        }
    }
}
