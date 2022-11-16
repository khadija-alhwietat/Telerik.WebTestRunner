using Microsoft.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web;
using Telerik.Sitefinity.HttpClientCore.HttpClients;
using Telerik.WebTestRunner.Client.Configuration;
using Telerik.WebTestRunner.Client.Model;
using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Launcher
{
	public class MultilingualTestsBatchConfigurator : ITestsBatchConfigurator
	{
		protected ISitefinityRestHttpAuthenticateClient client;

		private const string LocalizatonConfigurationSettingsServiceUrl = "/Sitefinity/Services/Configuration/ConfigSectionItems.svc/localization/";

		private const string MultisiteServiceUrl = "/Sitefinity/Services/Multisite/Multisite.svc/";

		private const string MultisiteServiceDefaultQuery = "?sortExpression=Name&skip=0&take=50";

		private const string CookieAuthPath = "/retrieveAuthCookie";

		private List<CultureInfo> culturesToConfigure;

		public ITestsBatchConfigurator Next
		{
			get;
			set;
		}

		public List<CultureInfo> CulturesToConfigure
		{
			get
			{
				if (culturesToConfigure == null)
				{
					ConfigureDefaultCultures();
				}
				return culturesToConfigure;
			}
			set
			{
				culturesToConfigure = value;
			}
		}

		public CultureInfo DefaultCulture
		{
			get;
			set;
		}

		public MultilingualTestsBatchConfigurator(ISitefinityRestHttpAuthenticateClient client)
		{
			this.client = client;
		}

		public void Configure(RunnerTestBatch batch)
		{
			if (batch == null)
			{
				throw new ArgumentNullException("batch");
			}
			if (batch.IsMultilingual && batch.Tests.Count > 0)
			{
				RunnerCredentials activeUserCredentials = RunnerConfig.GetActiveUserCredentials();
				client.ForceAuthenticateToServer(activeUserCredentials.Provider, activeUserCredentials.UserName, activeUserCredentials.Password);
				ConfigureClientAuthCookies();
				WcfLocalizationSettingsModel localizationSettings = GetLocalizationSettings();
				ApplyChangesIfNeeded(localizationSettings, batch);
				client.Logout(activeUserCredentials.Provider, activeUserCredentials.UserName, activeUserCredentials.Password);
			}
			this.ConfigureNext(batch);
		}

		protected virtual WcfLocalizationSettingsModel GetLocalizationSettings()
		{
			string uriString = RemoveTrailingSlash(client.BaseUrl) + "/Sitefinity/Services/Configuration/ConfigSectionItems.svc/localization/";
			HttpResponseMessage httpResponseMessage = client.Get(new Uri(uriString));
			httpResponseMessage.EnsureStatusIsSuccessful();
			return httpResponseMessage.Content.ReadAsJsonDataContract<WcfLocalizationSettingsModel>();
		}

		protected virtual void ApplyChangesIfNeeded(WcfLocalizationSettingsModel settings, RunnerTestBatch batch)
		{
			if (settings.Cultures == null || settings.Cultures.Count() == 1)
			{
				List<WcfCultureViewModel> list = new List<WcfCultureViewModel>(CulturesToConfigure.Count);
				foreach (CultureInfo item in CulturesToConfigure)
				{
					WcfCultureViewModel wcfCultureViewModel = new WcfCultureViewModel(item);
					if (DefaultCulture.LCID == item.LCID)
					{
						wcfCultureViewModel.IsDefault = true;
					}
					list.Add(wcfCultureViewModel);
				}
				if (IsMultisuteEnabled())
				{
					SaveMultisiteLocalizationChanges(list);
					return;
				}
				settings.Cultures = list.ToArray();
				SaveChanges(settings);
			}
		}

		protected virtual void SaveChanges(WcfLocalizationSettingsModel settings)
		{
			HttpClient httpClient = client as HttpClient;
			HttpContent body = httpClient.CreateContentAsJsonFrom(new WcfItemContext<WcfLocalizationSettingsModel>
			{
				Item = settings
			});
			string uriString = RemoveTrailingSlash(httpClient.BaseAddress.ToString()) + "/Sitefinity/Services/Configuration/ConfigSectionItems.svc/localization/" + Guid.Empty;
			httpClient.Put(new Uri(uriString), body).EnsureStatusIsSuccessful();
		}

		protected virtual void SaveMultisiteLocalizationChanges(List<WcfCultureViewModel> cultureProxies)
		{
			HttpClient httpClient = client as HttpClient;
			Guid defaultWebsiteId = GetDefaultWebsiteId();
			string text = RemoveTrailingSlash(httpClient.BaseAddress.ToString());
			string uriString = text + "/Sitefinity/Services/Multisite/Multisite.svc/" + defaultWebsiteId + "/";
			WcfSiteViewModel siteViewModel = GetSiteViewModel(defaultWebsiteId);
			siteViewModel.PublicContentCultures = cultureProxies;
			if (siteViewModel.StagingUrl == null)
			{
				siteViewModel.StagingUrl = "";
			}
			HttpContent body = httpClient.CreateContentAsJsonFrom(siteViewModel);
			httpClient.Put(new Uri(uriString), body).EnsureStatusIsSuccessful();
		}

		protected virtual WcfSiteViewModel GetSiteViewModel(Guid siteId)
		{
			string text = RemoveTrailingSlash(client.BaseUrl);
			string uriString = text + "/Sitefinity/Services/Multisite/Multisite.svc/" + siteId + "/";
			HttpResponseMessage httpResponseMessage = client.Get(new Uri(uriString));
			httpResponseMessage.EnsureStatusIsSuccessful();
			return httpResponseMessage.Content.ReadAsJsonDataContract<WcfSiteViewModel>();
		}

		protected virtual Guid GetDefaultWebsiteId()
		{
			Guid empty = Guid.Empty;
			HttpClient httpClient = client as HttpClient;
			string text = RemoveTrailingSlash(httpClient.BaseAddress.ToString()) + "/Sitefinity/Services/Multisite/Multisite.svc/" + "?sortExpression=Name&skip=0&take=50";
			HttpResponseMessage httpResponseMessage = httpClient.Get(new Uri(text));
			httpResponseMessage.EnsureStatusIsSuccessful();
			WcfCollectionContext<WcfSiteGridViewModel> wcfCollectionContext = httpResponseMessage.Content.ReadAsJsonDataContract<WcfCollectionContext<WcfSiteGridViewModel>>();
			if (wcfCollectionContext.Items.Count() > 0)
			{
				WcfSiteGridViewModel wcfSiteGridViewModel = wcfCollectionContext.Items.Where((WcfSiteGridViewModel item) => item.IsDefault).FirstOrDefault();
				if (wcfSiteGridViewModel != null)
				{
					return wcfSiteGridViewModel.Id;
				}
				throw new Exception($"There isn't a default multisite item. The request was {text}");
			}
			throw new Exception($"There aren't multisite items found after a request to the service. The request was {text}");
		}

		protected virtual bool IsMultisuteEnabled()
		{
			HttpClient httpClient = client as HttpClient;
			string uri = RemoveTrailingSlash(httpClient.BaseAddress.ToString()) + "/Sitefinity/Services/Multisite/Multisite.svc/";
			if (httpClient.Get(uri).StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}
			return true;
		}

		protected virtual void ConfigureDefaultCultures()
		{
			culturesToConfigure = new List<CultureInfo>
			{
				new CultureInfo("en-US"),
				new CultureInfo("tr-TR"),
				new CultureInfo("ar-MA"),
				new CultureInfo("bg-BG"),
				new CultureInfo("fr-fr"),
				new CultureInfo("de-de"),
				new CultureInfo("sr-Cyrl-BA")
			};
			DefaultCulture = new CultureInfo("en-US");
		}

		private void ConfigureClientAuthCookies()
		{
			string text = RemoveTrailingSlash(client.BaseUrl);
			string cookieHeader = client.Get(new Uri(text + "/retrieveAuthCookie" + "?forceLogin=true")).Headers.SetCookie.ToString();
			CookieContainer cookieContainer = new CookieContainer();
			cookieContainer.SetCookies(new Uri(text), cookieHeader);
			client.TransportSettings.Cookies = cookieContainer;
		}

		private string RemoveTrailingSlash(string value)
		{
			return VirtualPathUtility.RemoveTrailingSlash(value);
		}
	}
}
