using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Collections;

namespace Telerik.WebTestRunner.Client.Configuration.Sections
{
	public class MachineSpecificSection : ConfigurationSection
	{
		[ConfigurationProperty("machines", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(MachinesConfigurationCollection))]
		public MachinesConfigurationCollection Machines => (MachinesConfigurationCollection)base["machines"];
	}
}
