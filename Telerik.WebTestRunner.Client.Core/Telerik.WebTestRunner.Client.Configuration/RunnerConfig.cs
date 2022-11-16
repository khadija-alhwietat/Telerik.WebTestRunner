using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Telerik.WebTestRunner.Client.Configuration.Collections;
using Telerik.WebTestRunner.Client.Configuration.Elements;
using Telerik.WebTestRunner.Client.Configuration.Sections;
using Telerik.WebTestRunner.Client.Logger;
using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Configuration
{
	public static class RunnerConfig
	{
		public static bool AreUserCredentialsSaved;

		private static string testInstanceUrl;

		public static bool TfisAuthEnabled
		{
			get
			{
				return ((CredentialsConfigSection)GetConfigSection("credentialsConfiguration")).TfisSettings.Enabled;
			}
			set
			{
				((CredentialsConfigSection)GetConfigSection("credentialsConfiguration")).TfisSettings.Enabled = value;
				ActiveConfiguration.Save(ConfigurationSaveMode.Full);
			}
		}

		public static string TfisTokenEndpointBasicAuthentication
		{
			get
			{
				return ((CredentialsConfigSection)GetConfigSection("credentialsConfiguration")).TfisSettings.BasicAuthentication;
			}
			set
			{
				((CredentialsConfigSection)GetConfigSection("credentialsConfiguration")).TfisSettings.BasicAuthentication = value;
				ActiveConfiguration.Save(ConfigurationSaveMode.Full);
			}
		}

		public static string TfisTokenEndpoint
		{
			get
			{
				return ((CredentialsConfigSection)GetConfigSection("credentialsConfiguration")).TfisSettings.TokenEndpoint;
			}
			set
			{
				((CredentialsConfigSection)GetConfigSection("credentialsConfiguration")).TfisSettings.TokenEndpoint = value;
				ActiveConfiguration.Save(ConfigurationSaveMode.Full);
			}
		}

		private static System.Configuration.Configuration ActiveConfiguration
		{
			get;
			set;
		}

		public static string GetTestInstanceUrl()
		{
			if (string.IsNullOrEmpty(testInstanceUrl))
			{
				MachineConfigElement machineConfigElement = (from m in ((MachineSpecificSection)ConfigurationManager.GetSection("machineSpecificConfigurations")).Machines.Cast<MachineConfigElement>().ToList()
					where m.Name == Environment.MachineName
					select m).FirstOrDefault();
				if (machineConfigElement != null)
				{
					testInstanceUrl = machineConfigElement.TestingInstanceUrl;
				}
			}
			return testInstanceUrl;
		}

		public static RunnerCredentials GetActiveUserCredentials()
		{
			CredentialsConfigElement credentialsConfigElement = (from CredentialsConfigElement credential in ((CredentialsConfigSection)ConfigurationManager.GetSection("credentialsConfiguration")).Credentials
				where credential.IsActive
				select credential).FirstOrDefault();
			if (credentialsConfigElement == null)
			{
				throw new ArgumentException("At least one user must be selected as active in the app.config file");
			}
			return new RunnerCredentials(credentialsConfigElement.Username, credentialsConfigElement.Password, credentialsConfigElement.Provider);
		}

		public static void SaveHostUrl(string url)
		{
			try
			{
				MachinesConfigurationCollection machinesConfigurationCollection = (GetConfigSection("machineSpecificConfigurations") as MachineSpecificSection).Machines;
				List<MachineConfigElement> source = machinesConfigurationCollection.Cast<MachineConfigElement>().ToList();
				MachineConfigElement value = new MachineConfigElement
				{
					Name = Environment.MachineName,
					TestingInstanceUrl = url
				};
				MachineConfigElement machineConfigElement = source.Where((MachineConfigElement m) => m.Name == Environment.MachineName).FirstOrDefault();
				if (machineConfigElement != null)
				{
					testInstanceUrl = machineConfigElement.TestingInstanceUrl;
				}
				else
				{
					machinesConfigurationCollection.Add(value);
				}
				ActiveConfiguration.Save(ConfigurationSaveMode.Full);
			}
			catch (Exception exception)
			{
				WebTestRunnerEventLogger.LogException(exception, "Host URL could not be saved.");
			}
		}

		public static void SaveUserCredentials(string username, string password, bool isActive = true)
		{
			SaveUserCredentialsInternal(username, password, string.Empty, isActive);
		}

		public static void SaveUserCredentials(string username, string password, string provider, bool isActive = true)
		{
			SaveUserCredentialsInternal(username, password, provider, isActive);
		}

		public static int GetTestExecutionTimeout()
		{
			return ((RunnerConfigSection)ConfigurationManager.GetSection("runnerConfiguration")).RunnerConfig.TotalTestExecutionTimeout;
		}

		public static int GetSingleTestExecutionTimeout()
		{
			return ((RunnerConfigSection)ConfigurationManager.GetSection("runnerConfiguration")).RunnerConfig.SingleTestExecutionTimeout;
		}

		private static ConfigurationSection GetConfigSection(string configurationSection)
		{
			return (ActiveConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)).GetSection(configurationSection);
		}

		private static void SaveUserCredentialsInternal(string username, string password, string provider, bool isActive = true)
		{
			try
			{
				CredentialsConfigurationCollection credentialsConfigurationCollection = (GetConfigSection("credentialsConfiguration") as CredentialsConfigSection).Credentials;
				CredentialsConfigElement credentialsConfigElement = new CredentialsConfigElement();
				credentialsConfigElement.Username = username;
				credentialsConfigElement.Password = password;
				credentialsConfigElement.Provider = provider;
				credentialsConfigElement.IsActive = isActive;
				foreach (CredentialsConfigElement item in credentialsConfigurationCollection.Cast<CredentialsConfigElement>())
				{
					if (credentialsConfigElement.Username != item.Username)
					{
						item.IsActive = false;
					}
				}
				credentialsConfigurationCollection.Add(credentialsConfigElement);
				ActiveConfiguration.Save(ConfigurationSaveMode.Full);
				ConfigurationManager.RefreshSection("credentialsConfiguration");
				AreUserCredentialsSaved = true;
			}
			catch (Exception exception)
			{
				WebTestRunnerEventLogger.LogException(exception, "UserName and Password could not be saved.");
			}
		}
	}
}
