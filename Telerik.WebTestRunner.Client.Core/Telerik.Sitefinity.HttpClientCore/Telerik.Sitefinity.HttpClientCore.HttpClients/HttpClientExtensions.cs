using Microsoft.Http;
using Microsoft.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public static class HttpClientExtensions
	{
		public static HeaderValues<Cookie> CleanExpired(this HeaderValues<Cookie> headerValues)
		{
			DateTime utcNow = DateTime.UtcNow;
			HeaderValues<Cookie> headerValues2 = new HeaderValues<Cookie>();
			foreach (Cookie item in (IEnumerable<Cookie>)headerValues)
			{
				if (!item.Expires.HasValue || item.Expires.Value > utcNow)
				{
					headerValues2.Add(item);
				}
			}
			return headerValues2;
		}

		public static HeaderValues<Cookie> StackRepeatedCookiesByName(this HeaderValues<Cookie> headerValues)
		{
			HeaderValues<Cookie> headerValues2 = new HeaderValues<Cookie>();
			Dictionary<string, Cookie> dictionary = new Dictionary<string, Cookie>(headerValues.Count);
			foreach (Cookie item in (IEnumerable<Cookie>)headerValues)
			{
				string text = item.TryGetName();
				if (!string.IsNullOrEmpty(text))
				{
					Cookie cookie2 = dictionary[text] = Cookie.Parse($"{text}={item[text]};");
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, Cookie> item2 in dictionary)
			{
				stringBuilder.Append(item2.Value);
				stringBuilder.Append("; ");
			}
			headerValues2.Add(Cookie.Parse(stringBuilder.ToString()));
			return headerValues2;
		}

		public static string TryGetName(this Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			string text = null;
			string text2 = cookie.ToString();
			if (!string.IsNullOrEmpty(text2))
			{
				string text3 = text2.Split(';').FirstOrDefault();
				if (!string.IsNullOrEmpty(text3))
				{
					int num = text3.IndexOf('=');
					if (num >= 0)
					{
						text = text3.Substring(0, num).Trim();
						if (IsReservedCookieWord(text))
						{
							text = null;
						}
					}
				}
			}
			return text;
		}

		public static HttpContent CreateContentAsJsonFrom<T>(this HttpClient client, T graph)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(graph.GetType());
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject((Stream)memoryStream, (object)graph);
				memoryStream.Position = 0L;
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Read(array, 0, (int)memoryStream.Length);
				return HttpContent.Create(array, "application/json");
			}
		}

		private static bool IsReservedCookieWord(string cookieName)
		{
			return new string[5]
			{
				"expires",
				"domain",
				"path",
				"secure",
				"httponly"
			}.Contains(cookieName, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
