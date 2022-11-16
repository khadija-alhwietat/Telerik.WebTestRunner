using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class ResultSummary
	{
		[XmlAttribute("outcome")]
		public virtual string Outcome
		{
			get;
			set;
		}

		[XmlElement("Counters")]
		public virtual Counters Counters
		{
			get;
			set;
		}
	}
}
