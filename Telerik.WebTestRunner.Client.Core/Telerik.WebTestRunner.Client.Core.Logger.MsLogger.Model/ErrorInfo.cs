using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class ErrorInfo
	{
		[XmlElement("Message")]
		public virtual string Message
		{
			get;
			set;
		}

		[XmlElement("StackTrace")]
		public virtual string StackTrace
		{
			get;
			set;
		}
	}
}
