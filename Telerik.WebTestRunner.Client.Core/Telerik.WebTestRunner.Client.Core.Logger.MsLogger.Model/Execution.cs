using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class Execution
	{
		[XmlAttribute("id")]
		public string Id
		{
			get;
			set;
		}
	}
}
