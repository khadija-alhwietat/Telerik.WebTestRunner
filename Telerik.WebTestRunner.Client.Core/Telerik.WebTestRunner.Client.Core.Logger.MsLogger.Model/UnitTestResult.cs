using System;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[Serializable]
	public class UnitTestResult
	{
		[XmlAttribute("executionId")]
		public virtual string ExecutionId
		{
			get;
			set;
		}

		[XmlAttribute("duration")]
		public virtual string Duration
		{
			get;
			set;
		}

		[XmlAttribute("testId")]
		public virtual string TestId
		{
			get;
			set;
		}

		[XmlAttribute("testName")]
		public virtual string TestName
		{
			get;
			set;
		}

		[XmlAttribute("testType")]
		public virtual string TestType
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

		[XmlAttribute("outcome")]
		public virtual string Outcome
		{
			get;
			set;
		}

		[XmlAttribute("computerName")]
		public virtual string ComputerName
		{
			get;
			set;
		}

		[XmlAttribute("startTime")]
		public virtual DateTime StartTime
		{
			get;
			set;
		}

		[XmlAttribute("endTime")]
		public virtual DateTime EndTime
		{
			get;
			set;
		}

		[XmlElement("Output")]
		public virtual Output Output
		{
			get;
			set;
		}
	}
}
