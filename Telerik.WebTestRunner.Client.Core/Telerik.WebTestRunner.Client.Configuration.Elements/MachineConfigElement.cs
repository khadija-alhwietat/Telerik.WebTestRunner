using System.Configuration;

namespace Telerik.WebTestRunner.Client.Configuration.Elements
{
	public class MachineConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return (string)base["name"];
			}
			set
			{
				base["name"] = value;
			}
		}

		[ConfigurationProperty("testingInstanceUrl", IsRequired = false, IsKey = false)]
		public string TestingInstanceUrl
		{
			get
			{
				return (string)base["testingInstanceUrl"];
			}
			set
			{
				base["testingInstanceUrl"] = value;
			}
		}
	}
}
