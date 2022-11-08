using Microsoft.Http;
using Microsoft.Http.Headers;
using System;
using System.Net;
using System.Text;

namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public class SitefinityRestHttpClientTfis : SitefinityRestHttpClientBase, ISitefinityRestHttpAuthenticateClient, ISitefinityRestHttpClient
	{
		private const string TfisJsonRequestCredentialsFormat = "\r\n        {{\r\n            \"grant_type\":\"password\",\r\n            \"username\":\"{0}\",\r\n            \"password\":\"{1}\",\r\n        }}";

		private string tfisTokenEndpoint;

		private string basicAuth;

		public SitefinityRestHttpClientTfis(string tfisTokenEndpoint, string basicAuth)
		{
			this.tfisTokenEndpoint = tfisTokenEndpoint;
			this.basicAuth = basicAuth;
		}

		public bool Logout()
		{
			base.DefaultHeaders["Authorization"] = null;
			return true;
		}

		public bool Logout(string membershipProvider, string userName, string password = null)
		{
			return Logout();
		}

		private static string ParseTfisTokenEndpointResponseToken(string responseContentAsString)
		{
			string[] array = responseContentAsString.Split(new char[1]
			{
				'"'
			}, StringSplitOptions.RemoveEmptyEntries);
			string result = null;
			if (array.Length >= 4)
			{
				result = array[3];
			}
			return result;
		}

		protected override UserLoggingReason AuthenticateToServerInternal(string membershipProvider, string userName, string password, bool rememberMe, bool logoutOtherUsersIfNeeded)
		{
			Uri uri = new Uri(tfisTokenEndpoint);
			HttpContent credentials = GetCredentials(userName, password);
			RequestHeaders requestHeaders = new RequestHeaders();
			requestHeaders.Add("Content-Type", "application/json");
			requestHeaders.Add("Authorization", "Basic " + basicAuth);
			HttpResponseMessage httpResponseMessage = Send(HttpMethod.POST, uri, requestHeaders, credentials);
			if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
			{
				throw new Exception($"The authentication reqeust failed with status code: {httpResponseMessage.StatusCode}");
			}
			string text = httpResponseMessage.Content.ReadAsString();
			if (string.IsNullOrEmpty(text))
			{
				throw new Exception("The authentication service didn't return any response content.");
			}
			string text2 = ParseTfisTokenEndpointResponseToken(text);
			if (!string.IsNullOrEmpty(text2))
			{
				base.DefaultHeaders["Authorization"] = "bearer " + text2;
				base.DefaultHeaders["IsBackendRequest"] = "true";
				return UserLoggingReason.Success;
			}
			throw new Exception("The authentication service did not returned the expected json object with security token.");
		}

		private HttpContent GetCredentials(string userName, string password)
		{
			string s = $"\r\n        {{\r\n            \"grant_type\":\"password\",\r\n            \"username\":\"{userName}\",\r\n            \"password\":\"{password}\",\r\n        }}";
			return HttpContent.Create(Encoding.UTF8.GetBytes(s), "application/json");
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
