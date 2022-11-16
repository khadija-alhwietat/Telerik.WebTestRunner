using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model
{
	[XmlRoot("TestRun", Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010", IsNullable = false)]
	public class TestRun
	{
		[XmlElement("TestSettings")]
		public virtual TestSettings TestSettings
		{
			get;
			set;
		}

		[XmlElement("ResultSummary")]
		public virtual ResultSummary ResultSummary
		{
			get;
			set;
		}

		[XmlElement("TestDefinitions")]
		public virtual TestDefinitions TestDefinitions
		{
			get;
			set;
		}

		[XmlElement("TestLists")]
		public virtual TestLists TestLists
		{
			get;
			set;
		}

		[XmlElement("TestEntries")]
		public virtual TestEntries TestEntries
		{
			get;
			set;
		}

		[XmlElement("Results")]
		public virtual Results Results
		{
			get;
			set;
		}
	}
}
