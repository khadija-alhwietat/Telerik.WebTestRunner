using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class TestMethod
	{
		[XmlAttribute("name")]
		public virtual string Name
		{
			get;
			set;
		}

		[XmlAttribute("className")]
		public virtual string ClassName
		{
			get;
			set;
		}

		[XmlAttribute("codeBase")]
		public virtual string CodeBase
		{
			get;
			set;
		}
	}
}
