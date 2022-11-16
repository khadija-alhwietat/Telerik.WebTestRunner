using Microsoft.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Telerik.Sitefinity.HttpClientCore.HttpClients;
using Telerik.WebTestRunner.Client.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Sections;
using Telerik.WebTestRunner.Client.Core;
using Telerik.WebTestRunner.Client.Core.Exceptions;
using Telerik.WebTestRunner.Client.Core.Utilities;
using Telerik.WebTestRunner.Client.Model;
using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.DataSource
{
	public class ServiceTestsDataSource
	{
		private string testingInstanceUrl;

		private string categoriesFilter;

		private string testsFilter;

		private List<RunnerTest> tests;

		private const string TestRunnerServiceUrl = "IntegrationTests/TestRunnerService.svc";

		private const string GetTestsMethodName = "GetTests";

		private const string GetTestsByTestNamesMethodName = "GetTestsByTestNames";

		private const string GetTestsByTestNamesJsonFormat = "\r\n        {{\r\n            \"tests\":\"{0}\",\r\n            \"assembly\":\"{1}\"\r\n        }}";

		private const string DefaultAuthenticationEndPoint = "Sitefinity/Login";

		private const string ClaimsSWTAuthenticationEndPoint = "/Sitefinity/Authenticate/SWT";

		private const string ClaimsOpenIdAuthenticationEndPoint = "/Sitefinity/Authenticate/OpenID";

		private ISitefinityRestHttpAuthenticateClient client;

		private ISitefinityRestHttpAuthenticateClient Client
		{
			get
			{
				if (client == null)
				{
					ServicePointManager.ServerCertificateValidationCallback = ((object _003Cp0_003E, X509Certificate _003Cp1_003E, X509Chain _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true);
					WcfAuthenticationMode authenticationMode = GetAuthenticationMode(TestingInstanceUrl);
					SitefinityRestHttpClientFactory sitefinityRestHttpClientFactory = new SitefinityRestHttpClientFactory();
					client = sitefinityRestHttpClientFactory.GetClient(authenticationMode);
					SetHttpClientProperties();
				}
				return client;
			}
		}

		public string TestingInstanceUrl
		{
			get
			{
				return testingInstanceUrl;
			}
			set
			{
				testingInstanceUrl = value;
			}
		}

		public string CategoriesFilter
		{
			get
			{
				return categoriesFilter;
			}
			set
			{
				categoriesFilter = value;
			}
		}

		public string TestsFilter
		{
			get
			{
				return testsFilter;
			}
			set
			{
				testsFilter = value;
			}
		}

		public string AssemblyNameFilter
		{
			get;
			set;
		}

		public ServiceTestsDataSource()
			: this(string.Empty)
		{
		}

		public ServiceTestsDataSource(string testingInstanceUrl)
		{
			this.testingInstanceUrl = testingInstanceUrl;
		}

		public virtual void InitializeTestAndFixtures()
		{
			HttpResponseMessage httpResponseMessage = TryGetTests();
			TryAuthenticateConfiguredUser();
			List<RunnerTest> list = tests = (from t in httpResponseMessage.Content.ReadAsJsonDataContract<WcfTestInfoArray>().Tests
				select RunnerModelMapper.CreateRunnerTest(t) into t
				orderby t.FixtureName, t.TestMethodName
				select t).ToList();
		}

		public List<RunnerTest> GetTests()
		{
			if (tests == null)
			{
				InitializeTestAndFixtures();
			}
			return tests;
		}

		private WcfAuthenticationMode GetAuthenticationMode(string baseUrl)
		{
			if (RunnerConfig.TfisAuthEnabled)
			{
				return WcfAuthenticationMode.Tfis;
			}
			WebRequest webRequest = WebRequest.Create(VirtualPathUtility.RemoveTrailingSlash(baseUrl) + "/Sitefinity");
			webRequest.Timeout = 240000;
			using (WebResponse webResponse = webRequest.GetResponse())
			{
				string absolutePath = webResponse.ResponseUri.AbsolutePath;
				WcfAuthenticationMode result = WcfAuthenticationMode.Default;
				if (absolutePath.EndsWith("Sitefinity/Login"))
				{
					result = WcfAuthenticationMode.Default;
				}
				else if (absolutePath.Contains("/Sitefinity/Authenticate/SWT"))
				{
					result = WcfAuthenticationMode.Claims;
				}
				else if (absolutePath.Contains("/Sitefinity/Authenticate/OpenID"))
				{
					result = WcfAuthenticationMode.OpenId;
				}
				return result;
			}
		}

		private string RemoveTrailingSlash(string value)
		{
			return VirtualPathUtility.RemoveTrailingSlash(value);
		}

		private void SetHttpClientProperties()
		{
			client.BaseUrl = TestingInstanceUrl;
			client.TransportSettings.ConnectionTimeout = new TimeSpan(0, 0, 240);
		}

		private HttpResponseMessage TryGetTests()
		{
			string empty = string.Empty;
			HttpContent httpContent = null;
			string defaultFilter = CategoriesFilter;
			if (string.IsNullOrEmpty(defaultFilter))
			{
				defaultFilter = GetDefaultFilter();
			}
			TestingInstanceUrl = RemoveTrailingSlash(TestingInstanceUrl);
			if (string.IsNullOrEmpty(TestsFilter))
			{
				empty = ((string.IsNullOrEmpty(defaultFilter) && string.IsNullOrEmpty(AssemblyNameFilter)) ? string.Format("{0}/{1}/{2}", TestingInstanceUrl, "IntegrationTests/TestRunnerService.svc", "GetTests") : string.Format("{0}/{1}/{2}?categories={3}&assembly={4}", TestingInstanceUrl, "IntegrationTests/TestRunnerService.svc", "GetTests", defaultFilter, AssemblyNameFilter));
			}
			else
			{
				empty = string.Format("{0}/{1}/{2}", TestingInstanceUrl, "IntegrationTests/TestRunnerService.svc", "GetTestsByTestNames");
				string s = $"\r\n        {{\r\n            \"tests\":\"{TestsFilter}\",\r\n            \"assembly\":\"{AssemblyNameFilter}\"\r\n        }}";
				httpContent = HttpContent.Create(Encoding.UTF8.GetBytes(s), "application/json");
			}
			try
			{
				HttpResponseMessage httpResponseMessage = (httpContent != null) ? Client.Post(empty, httpContent) : Client.Get(empty);
				httpResponseMessage.EnsureStatusIsSuccessful();
				return httpResponseMessage;
			}
			catch (Exception ex)
			{
				throw new GettingTestsException("There is an error trying to get tests. Please check test url", ex);
			}
		}

		private string GetDefaultFilter()
		{
			return ((RunnerConfigSection)ConfigurationManager.GetSection("runnerConfiguration"))?.DefaultFilter;
		}

		private void TryAuthenticateConfiguredUser()
		{
			RunnerModelMapper.DefaultCredentials = RunnerConfig.GetActiveUserCredentials();
			try
			{
				SitefinityHelper.WaitForSitefinityToStart(TestingInstanceUrl, 600.0);
				Client.ForceAuthenticateToServer(RunnerModelMapper.DefaultCredentials.Provider, RunnerModelMapper.DefaultCredentials.UserName, RunnerModelMapper.DefaultCredentials.Password);
				Client.Logout(RunnerModelMapper.DefaultCredentials.Provider, RunnerModelMapper.DefaultCredentials.UserName, RunnerModelMapper.DefaultCredentials.Password);
			}
			catch (Exception ex)
			{
				throw new GettingTestsException("There is an error trying to get tests. UserName or Password is incorrect", ex);
			}
		}
	}
}
