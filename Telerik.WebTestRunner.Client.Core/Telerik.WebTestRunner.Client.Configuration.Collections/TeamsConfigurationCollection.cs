using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Elements;

namespace Telerik.WebTestRunner.Client.Configuration.Collections
{
	public class TeamsConfigurationCollection : ConfigurationElementCollection<string, TeamConfigElement>
	{
		public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

		protected override string ElementName => "team";

		protected override string GetElementKey(TeamConfigElement element)
		{
			return element.Name;
		}
	}
}
