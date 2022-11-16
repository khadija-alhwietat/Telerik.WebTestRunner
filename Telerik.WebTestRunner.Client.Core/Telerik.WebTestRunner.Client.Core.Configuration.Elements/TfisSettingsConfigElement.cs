using System.Configuration;

namespace Telerik.WebTestRunner.Client.Core.Configuration.Elements
{
	public class TfisSettingsConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("tokenEndpoint", IsRequired = true)]
		public string TokenEndpoint
		{
			get
			{
				return (string)base["tokenEndpoint"];
			}
			set
			{
				base["tokenEndpoint"] = value;
			}
		}

		[ConfigurationProperty("enabled", DefaultValue = false, IsRequired = true)]
		public bool Enabled
		{
			get
			{
				return (bool)base["enabled"];
			}
			set
			{
				base["enabled"] = value;
			}
		}

		[ConfigurationProperty("basicAuthorization", IsRequired = true)]
		public string BasicAuthentication
		{
			get
			{
				return (string)base["basicAuthorization"];
			}
			set
			{
				base["basicAuthorization"] = value;
			}
		}
	}
}
