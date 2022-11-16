using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Collections;

namespace Telerik.WebTestRunner.Client.Configuration.Sections
{
	public class TeamConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("teams", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(TeamsConfigurationCollection))]
		public TeamsConfigurationCollection Teams => (TeamsConfigurationCollection)base["teams"];
	}
}
