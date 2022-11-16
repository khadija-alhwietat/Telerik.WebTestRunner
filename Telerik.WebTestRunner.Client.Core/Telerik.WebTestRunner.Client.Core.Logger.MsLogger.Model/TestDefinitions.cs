using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class TestDefinitions
	{
		[XmlElement("UnitTest")]
		public virtual UnitTest UnitTest
		{
			get;
			set;
		}
	}
}
