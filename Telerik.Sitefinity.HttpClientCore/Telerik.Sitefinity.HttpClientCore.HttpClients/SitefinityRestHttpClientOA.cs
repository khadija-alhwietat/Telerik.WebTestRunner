using Microsoft.Http;
using Microsoft.Http.Headers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public class SitefinityRestHttpClientOA : SitefinityRestHttpClientBase, ISitefinityRestHttpAuthenticateClient, ISitefinityRestHttpClient
	{
		public const string SimpleWebTokenServiceUrl = "/Sitefinity/Authenticate/SWT";

		public const string ClaimsLogOutServiceUrl = "/Sitefinity/SignOut";

		public SitefinityRestHttpClientOA()
		{
		}

		public SitefinityRestHttpClientOA(string baseUrl)
			: this()
		{
			base.BaseUrl = baseUrl;
		}

		public SitefinityRestHttpClientOA(string baseUrl, HttpWebRequestTransportSettings transportSettings)
			: this()
		{
			base.BaseUrl = baseUrl;
			base.TransportSettings = transportSettings;
		}

		public bool Logout()
		{
			throw new NotSupportedException();
		}

		public bool Logout(string membershipProvider, string userName, string password = null)
		{
			try
			{
				Uri logoutServiceUrl = GetLogoutServiceUrl();
				HttpQueryString queryString = new HttpQueryString
				{
					new KeyValuePair<string, string>("username", userName),
					new KeyValuePair<string, string>("provider", membershipProvider)
				};
				this.Get(logoutServiceUrl, queryString);
				base.DefaultHeaders["Authorization"] = null;
				return true;
			}
			catch (Exception ex)
			{
				throw new Exception($"There is an error trying to sing out the curent user. Exception message {ex.Message}, {ex.StackTrace}");
			}
		}

		protected override UserLoggingReason AuthenticateToServerInternal(string membershipProvider, string userName, string password, bool rememberMe, bool logoutOtherUsersIfNeeded)
		{
			Uri authenticationServiceUrl = GetAuthenticationServiceUrl();
			string text = $"wrap_name={userName}&wrap_password={password}";
			if (!string.IsNullOrEmpty(membershipProvider))
			{
				text = $"{text}&sf_domain={membershipProvider}";
			}
			HttpContent content = HttpContent.Create(Encoding.UTF8.GetBytes(text));
			RequestHeaders requestHeaders = new RequestHeaders();
			requestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
			HttpResponseMessage httpResponseMessage = Send(HttpMethod.POST, authenticationServiceUrl, requestHeaders, content);
			if (base.TransportSettings.Cookies == null)
			{
				base.TransportSettings.Cookies = new CookieContainer();
				base.TransportSettings.Cookies.SetCookies(base.BaseAddress, httpResponseMessage.Headers.SetCookie.ToString());
			}
			if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
			{
				throw new Exception($"The authentication reqeust failed with status code: {httpResponseMessage.StatusCode}");
			}
			string text2 = httpResponseMessage.Content.ReadAsString();
			if (string.IsNullOrEmpty(text2))
			{
				throw new Exception("The authentication service didn't return any security token.");
			}
			string text3 = HttpUtility.ParseQueryString(text2)["wrap_access_token"];
			if (!string.IsNullOrEmpty(text3))
			{
				base.DefaultHeaders["Authorization"] = "WRAP access_token=\"" + text3 + "\"";
				return UserLoggingReason.Success;
			}
			throw new Exception("The authentication service did not returned the expected token format 'wrap_access_token=(url encoded token)'");
		}

		private Uri GetAuthenticationServiceUrl()
		{
			return GetServiceUrl("/Sitefinity/Authenticate/SWT");
		}

		private Uri GetLogoutServiceUrl()
		{
			return GetServiceUrl("/Sitefinity/SignOut");
		}

		private Uri GetServiceUrl(string servicePath)
		{
			return new Uri(VirtualPathUtility.RemoveTrailingSlash(base.BaseUrl) + servicePath);
		}

		RequestHeaders ISitefinityRestHttpClient.get_DefaultHeaders()
		{
			return base.DefaultHeaders;
		}

		void ISitefinityRestHttpClient.set_DefaultHeaders(RequestHeaders value)
		{
			base.DefaultHeaders = value;
		}

		Uri ISitefinityRestHttpClient.get_BaseAddress()
		{
			return base.BaseAddress;
		}

		void ISitefinityRestHttpClient.set_BaseAddress(Uri value)
		{
			base.BaseAddress = value;
		}

		HttpWebRequestTransportSettings ISitefinityRestHttpClient.get_TransportSettings()
		{
			return base.TransportSettings;
		}
	}
}
