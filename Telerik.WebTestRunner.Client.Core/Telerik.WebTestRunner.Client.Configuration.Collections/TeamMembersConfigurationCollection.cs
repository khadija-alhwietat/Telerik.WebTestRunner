using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Elements;

namespace Telerik.WebTestRunner.Client.Configuration.Collections
{
	public class TeamMembersConfigurationCollection : ConfigurationElementCollection<string, TeamMemberConfigElement>
	{
		public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

		protected override string ElementName => "member";

		protected override string GetElementKey(TeamMemberConfigElement element)
		{
			return element.FullName;
		}
	}
}
