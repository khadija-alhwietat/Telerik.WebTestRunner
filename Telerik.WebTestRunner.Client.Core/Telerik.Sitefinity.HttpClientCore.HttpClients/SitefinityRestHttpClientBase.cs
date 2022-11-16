using Microsoft.Http;
using Microsoft.Http.Headers;
using System;

namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public abstract class SitefinityRestHttpClientBase : HttpClient, ISitefinityRestHttpClient
	{
		public string BaseUrl
		{
			get
			{
				return base.BaseAddress.ToString();
			}
			set
			{
				base.BaseAddress = new Uri(value);
			}
		}

		public SitefinityRestHttpClientBase()
		{
		}

		public UserLoggingReason ForceAuthenticateToServer(string membershipProvider, string userName, string password, bool rememberMe = false)
		{
			return AuthenticateToServerInternal(membershipProvider, userName, password, rememberMe, logoutOtherUsersIfNeeded: true);
		}

		public UserLoggingReason AuthenticateToServer(string membershipProvider, string userName, string password, bool rememberMe = false)
		{
			try
			{
				return AuthenticateToServerInternal(membershipProvider, userName, password, rememberMe, logoutOtherUsersIfNeeded: false);
			}
			catch (Exception ex)
			{
				throw new Exception($"There is an error trying to log in the curent user. Exception message {ex.Message}, {ex.StackTrace}");
			}
		}

		public virtual HttpResponseMessage Get(Uri requestUri)
		{
			return HttpMethodExtensions.Get(this, requestUri);
		}

		public virtual HttpResponseMessage Get(string requestUri)
		{
			return HttpMethodExtensions.Get(this, requestUri);
		}

		public virtual HttpResponseMessage Post(string uri, HttpContent body)
		{
			return HttpMethodExtensions.Post(this, uri, body);
		}

		public virtual HttpResponseMessage Post(string uri, string contentType, HttpContent body)
		{
			return HttpMethodExtensions.Post(this, uri, contentType, body);
		}

		public virtual HttpResponseMessage Put(string uri, HttpContent body)
		{
			return HttpMethodExtensions.Put(this, uri, body);
		}

		public virtual HttpResponseMessage Put(string uri, string contentType, HttpContent body)
		{
			return HttpMethodExtensions.Put(this, uri, contentType, body);
		}

		public virtual HttpResponseMessage Delete(string requestUri)
		{
			return HttpMethodExtensions.Delete(this, requestUri);
		}

		protected abstract UserLoggingReason AuthenticateToServerInternal(string membershipProvider, string userName, string password, bool rememberMe, bool logoutOtherUsersIfNeeded);

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
