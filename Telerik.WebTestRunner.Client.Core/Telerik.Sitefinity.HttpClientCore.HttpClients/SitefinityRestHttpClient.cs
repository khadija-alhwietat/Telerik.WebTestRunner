using Microsoft.Http;
using Microsoft.Http.Headers;
using System;
using System.Web;

namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public class SitefinityRestHttpClient : SitefinityRestHttpClientBase, ISitefinityRestHttpAuthenticateClient, ISitefinityRestHttpClient
	{
		private const string LogoutCurrentUserMethodName = "Logout";

		private const string LogoutUserWithCredentialsMethod = "LogoutCredentials";

		public SitefinityRestHttpClient()
		{
			base.DefaultHeaders.ContentType = "application/json";
		}

		public SitefinityRestHttpClient(string baseUrl)
			: this()
		{
			base.BaseUrl = baseUrl;
		}

		public bool Logout()
		{
			HttpResponseMessage httpResponseMessage = Get("/Sitefinity/Services/Security/Users.svc/Logout");
			httpResponseMessage.EnsureStatusIsSuccessful();
			return httpResponseMessage.Content.ReadAsString().ToLower() == "true";
		}

		public bool Logout(string membershipProvider, string userName, string password)
		{
			Uri uri = new Uri(VirtualPathUtility.RemoveTrailingSlash(base.BaseUrl) + "/Sitefinity/Services/Security/Users.svc/LogoutCredentials");
			HttpResponseMessage httpResponseMessage = this.Post(uri, SitefinityRestHttpClientHelper.GetCredentials(membershipProvider, userName, password, rememberMe: false));
			httpResponseMessage.EnsureStatusIsSuccessful();
			return httpResponseMessage.Content.ReadAsString().ToLower() == "true";
		}

		protected override UserLoggingReason AuthenticateToServerInternal(string membershipProvider, string userName, string password, bool rememberMe, bool logoutOtherUsersIfNeeded)
		{
			HttpContent credentials = SitefinityRestHttpClientHelper.GetCredentials(membershipProvider, userName, password, rememberMe);
			string str = VirtualPathUtility.RemoveTrailingSlash(base.BaseUrl);
			Uri uri = new Uri(str + "/Sitefinity/Services/Security/Users.svc/Authenticate");
			HttpResponseMessage httpResponseMessage = this.Post(uri, credentials);
			httpResponseMessage.EnsureStatusIsSuccessful();
			if (!Enum.TryParse(httpResponseMessage.Content.ReadAsString(), out UserLoggingReason result))
			{
				throw new Exception("The user service returned a result that is not a valid value of UserLoggingReason enum.");
			}
			switch (result)
			{
			case UserLoggingReason.Success:
				SetDefaultCookiesFrom(httpResponseMessage);
				return result;
			case UserLoggingReason.UserLoggedFromDifferentIp:
			case UserLoggingReason.UserLoggedFromDifferentComputer:
			case UserLoggingReason.UserAlreadyLoggedIn:
				if (logoutOtherUsersIfNeeded)
				{
					Uri uri2 = new Uri(str + "/Sitefinity/Services/Security/Users.svc/Authenticate");
					if (Logout(membershipProvider, userName, password))
					{
						HttpResponseMessage httpResponseMessage2 = this.Post(uri2, SitefinityRestHttpClientHelper.GetCredentials(membershipProvider, userName, password, rememberMe));
						string text = httpResponseMessage2.EnsureStatusIsSuccessful().Content.ReadAsString();
						if (text == "0")
						{
							SetDefaultCookiesFrom(httpResponseMessage2);
							return UserLoggingReason.Success;
						}
						return (UserLoggingReason)Enum.Parse(typeof(UserLoggingReason), text);
					}
				}
				return result;
			default:
				return result;
			}
		}

		private void SetDefaultCookiesFrom(HttpResponseMessage response)
		{
			base.DefaultHeaders.Cookie = response.Headers.SetCookie.CleanExpired().StackRepeatedCookiesByName();
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
