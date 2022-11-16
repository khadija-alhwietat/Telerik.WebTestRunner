using IdentityModel.Client;
using Microsoft.Http;
using Microsoft.Http.Headers;
using System;
using System.Collections.Generic;
using System.Web;

namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public class SitefinityRestHttpClientToken : SitefinityRestHttpClientBase, ISitefinityRestHttpAuthenticateClient, ISitefinityRestHttpClient
	{
		private const string AuthorizationHeader = "Authorization";

		private const int TokenResponseSecondsOffset = 1800;

		private const string MembershipProviderParameter = "membershipProvider";

		private Uri authenticationServiceUrl;

		public string ClientId
		{
			get;
			set;
		}

		public string ClientSecret
		{
			get;
			set;
		}

		public string Scopes
		{
			get;
			set;
		}

		public string TokenUrl
		{
			get;
			set;
		}

		public DateTime TokenExpiresOn
		{
			get;
			set;
		}

		public Credentials UserCredentials
		{
			get;
			set;
		}

		public Uri AuthenticationServiceUrl
		{
			get
			{
				if (authenticationServiceUrl == null)
				{
					string str = VirtualPathUtility.RemoveTrailingSlash(base.BaseUrl);
					authenticationServiceUrl = new Uri(str + TokenUrl);
				}
				return authenticationServiceUrl;
			}
		}

		public SitefinityRestHttpClientToken(string clientId, string clientSecret, string scopes, string tokenUrl)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
			Scopes = scopes;
			TokenUrl = tokenUrl;
		}

		public SitefinityRestHttpClientToken(string baseUrl, string clientId, string clientSecret, string scopes, string tokenUrl)
			: this(clientId, clientSecret, scopes, tokenUrl)
		{
			base.BaseUrl = baseUrl;
		}

		public SitefinityRestHttpClientToken(string baseUrl, string clientId, string clientSecret, string scopes, string tokenUrl, HttpWebRequestTransportSettings transportSettings)
			: this(baseUrl, clientId, clientSecret, scopes, tokenUrl)
		{
			base.TransportSettings = transportSettings;
		}

		public bool Logout()
		{
			base.DefaultHeaders["Authorization"] = null;
			TokenExpiresOn = DateTime.MinValue;
			return true;
		}

		public bool Logout(string membershipProvider, string userName, string password = null)
		{
			return Logout();
		}

		public override HttpResponseMessage Get(Uri requestUri)
		{
			GetNewTokenIfExpired();
			return base.Get(requestUri);
		}

		public override HttpResponseMessage Get(string requestUri)
		{
			GetNewTokenIfExpired();
			return base.Get(requestUri);
		}

		public override HttpResponseMessage Post(string uri, HttpContent body)
		{
			GetNewTokenIfExpired();
			return base.Post(uri, body);
		}

		public override HttpResponseMessage Post(string uri, string contentType, HttpContent body)
		{
			GetNewTokenIfExpired();
			return base.Post(uri, contentType, body);
		}

		public override HttpResponseMessage Delete(string requestUri)
		{
			GetNewTokenIfExpired();
			return base.Delete(requestUri);
		}

		public override HttpResponseMessage Put(string uri, HttpContent body)
		{
			GetNewTokenIfExpired();
			return base.Put(uri, body);
		}

		protected override UserLoggingReason AuthenticateToServerInternal(string membershipProvider, string userName, string password, bool rememberMe, bool logoutOtherUsersIfNeeded)
		{
			if (DateTime.UtcNow.AddSeconds(1800.0) > TokenExpiresOn)
			{
				if (UserCredentials == null)
				{
					UserCredentials = new Credentials(userName, password, membershipProvider);
				}
				else
				{
					UserCredentials.Email = userName;
					UserCredentials.Password = password;
					UserCredentials.MembershipProvider = membershipProvider;
				}
				Dictionary<string, string> extra = new Dictionary<string, string>
				{
					{
						"membershipProvider",
						membershipProvider
					}
				};
				TokenResponse result = new TokenClient(AuthenticationServiceUrl.AbsoluteUri, ClientId, ClientSecret).RequestResourceOwnerPasswordAsync(userName, password, Scopes, extra).Result;
				if (result.IsError)
				{
					throw new ApplicationException("Couldn't get access token. Error: " + result.Error);
				}
				SetBearerToken(result.AccessToken);
				TokenExpiresOn = DateTime.UtcNow + TimeSpan.FromSeconds(result.ExpiresIn);
			}
			return UserLoggingReason.Success;
		}

		private void SetBearerToken(string token)
		{
			base.DefaultHeaders["Authorization"] = "Bearer " + token;
		}

		private void GetNewTokenIfExpired()
		{
			if (TokenExpiresOn > DateTime.MinValue && DateTime.UtcNow.AddSeconds(1800.0) > TokenExpiresOn)
			{
				ForceAuthenticateToServer(UserCredentials.MembershipProvider, UserCredentials.Email, UserCredentials.Password);
			}
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
