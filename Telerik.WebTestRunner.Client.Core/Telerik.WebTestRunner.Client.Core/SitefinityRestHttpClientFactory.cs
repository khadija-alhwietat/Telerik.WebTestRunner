using System;
using Telerik.Sitefinity.HttpClientCore.HttpClients;
using Telerik.WebTestRunner.Client.Configuration;
using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Core
{
	public class SitefinityRestHttpClientFactory
	{
		public ISitefinityRestHttpAuthenticateClient GetClient(WcfAuthenticationMode authMode)
		{
			switch (authMode)
			{
			case WcfAuthenticationMode.Claims:
				return new SitefinityRestHttpClientOA();
			case WcfAuthenticationMode.Default:
				return new SitefinityRestHttpClientToken("sftestrunner", "secret", "oauth", "/sitefinity/oauth/token");
			case WcfAuthenticationMode.OpenId:
				return new SitefinityRestHttpClientToken("sftestrunner", "secret", "openid", "/Sitefinity/Authenticate/OpenID/connect/token");
			case WcfAuthenticationMode.Tfis:
				return new SitefinityRestHttpClientTfis(RunnerConfig.TfisTokenEndpoint, RunnerConfig.TfisTokenEndpointBasicAuthentication);
			default:
				throw new ArgumentException("Unknown authentication mode");
			}
		}
	}
}
