using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class Output
	{
		[XmlElement("ErrorInfo")]
		public virtual ErrorInfo ErrorInfo
		{
			get;
			set;
		}
	}
}
