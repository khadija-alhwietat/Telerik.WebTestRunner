using System.Configuration;

namespace Telerik.WebTestRunner.Client.Configuration.Elements
{
	public class RunnerConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("timeout", IsRequired = true, IsKey = true)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0)]
		public int TotalTestExecutionTimeout
		{
			get
			{
				return (int)base["timeout"];
			}
			set
			{
				base["timeout"] = value;
			}
		}

		[ConfigurationProperty("testTimeout", IsRequired = true, IsKey = true)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0)]
		public int SingleTestExecutionTimeout
		{
			get
			{
				return (int)base["testTimeout"];
			}
			set
			{
				base["testTimeout"] = value;
			}
		}
	}
}
