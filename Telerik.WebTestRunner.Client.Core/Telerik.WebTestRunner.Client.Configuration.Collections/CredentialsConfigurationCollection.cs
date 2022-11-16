using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Elements;

namespace Telerik.WebTestRunner.Client.Configuration.Collections
{
	public class CredentialsConfigurationCollection : ConfigurationElementCollection<string, CredentialsConfigElement>
	{
		public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

		protected override string ElementName => "credential";

		protected override string GetElementKey(CredentialsConfigElement element)
		{
			return element.Username;
		}
	}
}
