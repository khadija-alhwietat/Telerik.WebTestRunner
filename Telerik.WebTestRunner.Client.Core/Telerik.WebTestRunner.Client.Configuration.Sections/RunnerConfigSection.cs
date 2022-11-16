using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Elements;

namespace Telerik.WebTestRunner.Client.Configuration.Sections
{
	public class RunnerConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("runner", IsRequired = true)]
		public RunnerConfigElement RunnerConfig
		{
			get
			{
				return (RunnerConfigElement)base["runner"];
			}
			set
			{
				base["runner"] = value;
			}
		}

		[ConfigurationProperty("defaultFilter", IsRequired = false)]
		public string DefaultFilter
		{
			get
			{
				return (string)base["defaultFilter"];
			}
			set
			{
				base["defaultFilter"] = value;
			}
		}
	}
}
