using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class Results
	{
		[XmlElement("UnitTestResult")]
		public virtual UnitTestResult UnitTestResult
		{
			get;
			set;
		}
	}
}
