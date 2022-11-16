using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class TestEntries
	{
		[XmlElement("TestEntry")]
		public virtual TestEntry TestEntry
		{
			get;
			set;
		}
	}
}
