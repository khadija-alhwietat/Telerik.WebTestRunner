using System.Configuration;

namespace Telerik.WebTestRunner.Client.Configuration.Elements
{
	public class CredentialsConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("username", IsRequired = true, IsKey = true)]
		public string Username
		{
			get
			{
				return (string)base["username"];
			}
			set
			{
				base["username"] = value;
			}
		}

		[ConfigurationProperty("password", IsRequired = true, IsKey = false)]
		public string Password
		{
			get
			{
				return (string)base["password"];
			}
			set
			{
				base["password"] = value;
			}
		}

		[ConfigurationProperty("provider", IsRequired = false, IsKey = false)]
		public string Provider
		{
			get
			{
				return (string)base["provider"];
			}
			set
			{
				base["provider"] = value;
			}
		}

		[ConfigurationProperty("isActive", IsRequired = false, IsKey = false)]
		public bool IsActive
		{
			get
			{
				return (bool)base["isActive"];
			}
			set
			{
				base["isActive"] = value;
			}
		}
	}
}
