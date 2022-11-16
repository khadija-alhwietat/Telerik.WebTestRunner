using Microsoft.Http;
using Microsoft.Http.Headers;
using System;

namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public interface ISitefinityRestHttpClient
	{
		string BaseUrl
		{
			get;
			set;
		}

		RequestHeaders DefaultHeaders
		{
			get;
			set;
		}

		Uri BaseAddress
		{
			get;
			set;
		}

		HttpWebRequestTransportSettings TransportSettings
		{
			get;
		}

		HttpResponseMessage Get(Uri requestUri);

		HttpResponseMessage Get(string requestUri);

		HttpResponseMessage Post(string uri, HttpContent body);

		HttpResponseMessage Post(string uri, string contentType, HttpContent body);

		HttpResponseMessage Put(string uri, HttpContent body);

		HttpResponseMessage Put(string uri, string contentType, HttpContent body);

		HttpResponseMessage Delete(string requestUri);
	}
}
