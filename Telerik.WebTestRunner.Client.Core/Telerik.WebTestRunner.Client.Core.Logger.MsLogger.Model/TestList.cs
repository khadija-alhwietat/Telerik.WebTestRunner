using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class TestList
	{
		[XmlAttribute("name")]
		public virtual string Name
		{
			get;
			set;
		}

		[XmlAttribute("id")]
		public virtual string Id
		{
			get;
			set;
		}
	}
}
