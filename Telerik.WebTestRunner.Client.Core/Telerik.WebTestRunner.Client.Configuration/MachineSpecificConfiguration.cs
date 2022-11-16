using System.Linq;
using System.Xml.Linq;

namespace Telerik.WebTestRunner.Client.Configuration
{
	public class MachineSpecificConfiguration
	{
		private XElement element;

		public string ConnectionString => element.Descendants("connectionString").Single().Attribute("value")
			.Value;

		public MachineSpecificConfiguration(XElement element)
		{
			this.element = element;
		}
	}
}
