using Microsoft.Http;
using System.Text;

namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public static class SitefinityRestHttpClientHelper
	{
		private const string credentialsFormat = "\r\n        {{\r\n            \"MembershipProvider\":\"{0}\",\r\n            \"UserName\":\"{1}\",\r\n            \"Password\":\"{2}\",\r\n            \"Persistent\":{3}\r\n        }}";

		public const string AuthenticateMethodName = "Authenticate";

		public const string UsersServiceUrl = "/Sitefinity/Services/Security/Users.svc/";

		public static HttpContent GetCredentials(string membershipProvider, string userName, string password, bool rememberMe)
		{
			string s = $"\r\n        {{\r\n            \"MembershipProvider\":\"{membershipProvider}\",\r\n            \"UserName\":\"{userName}\",\r\n            \"Password\":\"{password}\",\r\n            \"Persistent\":{rememberMe.ToString().ToLower()}\r\n        }}";
			return HttpContent.Create(Encoding.UTF8.GetBytes(s), "application/json");
		}
	}
}
