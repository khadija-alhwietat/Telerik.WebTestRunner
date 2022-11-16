using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class Counters
	{
		[XmlAttribute("total")]
		public virtual int Total
		{
			get;
			set;
		}

		[XmlAttribute("passed")]
		public virtual int Passed
		{
			get;
			set;
		}

		[XmlAttribute("failed")]
		public virtual int Failed
		{
			get;
			set;
		}

		[XmlAttribute("notExecuted")]
		public virtual int NotExecuted
		{
			get;
			set;
		}
	}
}
