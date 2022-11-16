using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class TestLists
	{
		[XmlElement("TestList")]
		public virtual TestList TestList
		{
			get;
			set;
		}
	}
}
