using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Collections;
using Telerik.WebTestRunner.Client.Core.Configuration.Elements;

namespace Telerik.WebTestRunner.Client.Configuration.Sections
{
	public class CredentialsConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("credentials", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(CredentialsConfigurationCollection))]
		public CredentialsConfigurationCollection Credentials => (CredentialsConfigurationCollection)base["credentials"];

		[ConfigurationProperty("tfisSettings", IsRequired = false)]
		public TfisSettingsConfigElement TfisSettings => (TfisSettingsConfigElement)base["tfisSettings"];
	}
}
