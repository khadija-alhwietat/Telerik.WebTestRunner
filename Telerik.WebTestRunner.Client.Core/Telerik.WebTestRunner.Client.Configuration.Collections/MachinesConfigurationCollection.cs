using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Elements;

namespace Telerik.WebTestRunner.Client.Configuration.Collections
{
	public class MachinesConfigurationCollection : ConfigurationElementCollection<string, MachineConfigElement>
	{
		public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

		protected override string ElementName => "machine";

		protected override string GetElementKey(MachineConfigElement element)
		{
			return element.Name;
		}
	}
}
