using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Collections;

namespace Telerik.WebTestRunner.Client.Configuration.Elements
{
	public class TeamConfigElement : ConfigurationElement
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

		[ConfigurationProperty("members", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(TeamMembersConfigurationCollection))]
		public TeamMembersConfigurationCollection Members => (TeamMembersConfigurationCollection)base["members"];
	}
}
