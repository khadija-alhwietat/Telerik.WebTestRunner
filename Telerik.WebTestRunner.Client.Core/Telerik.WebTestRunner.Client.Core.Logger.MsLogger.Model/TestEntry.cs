using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class TestEntry
	{
		[XmlAttribute("testId")]
		public virtual string TestId
		{
			get;
			set;
		}

		[XmlAttribute("executionId")]
		public virtual string ExecutionId
		{
			get;
			set;
		}

		[XmlAttribute("testListId")]
		public virtual string TestListId
		{
			get;
			set;
		}
	}
}
