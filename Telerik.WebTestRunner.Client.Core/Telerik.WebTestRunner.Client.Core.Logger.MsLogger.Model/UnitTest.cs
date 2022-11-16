using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class UnitTest
	{
		[XmlAttribute("name")]
		public virtual string Name
		{
			get;
			set;
		}

		[XmlAttribute("storage")]
		public virtual string Storage
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

		[XmlElement("Execution")]
		public virtual Execution Execution
		{
			get;
			set;
		}

		[XmlElement("TestMethod")]
		public virtual TestMethod TestMethod
		{
			get;
			set;
		}
	}
}
