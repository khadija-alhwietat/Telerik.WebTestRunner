using System.Xml.Linq;
using System.Xml.Serialization;
using Telerik.WebTestRunner.Client.Core.Logger;
using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Logger
{
	public class SitefinityTestResultXmlLogger : TestResultXmlLogger
	{
		private XElement currentTestRunElement;

		public SitefinityTestResultXmlLogger(string logFilePath)
			: base(logFilePath)
		{
			if (base.XDocument == null)
			{
				base.XDocument = new XDocument();
				base.XDocument.Add(new XElement("TestResults"));
			}
			EnsureCurrentTestRun();
		}

		public override void Log(RunnerTestResult result, bool isToSave = false)
		{
			XElement content = new XmlSerializer(typeof(RunnerTestResult)).SerializeToXElement(result);
			currentTestRunElement.Add(content);
			if (isToSave)
			{
				Save(base.XDocument, base.LogFilePath);
			}
		}

		public override void Start(string testRunName)
		{
			EnsureCurrentTestRun();
			string formattedDateTimeNowAsFormattedString = GetFormattedDateTimeNowAsFormattedString();
			currentTestRunElement.Add(new XAttribute("started", formattedDateTimeNowAsFormattedString));
			currentTestRunElement.Add(new XAttribute("name", testRunName));
		}

		public override void Finish()
		{
			string formattedDateTimeNowAsFormattedString = GetFormattedDateTimeNowAsFormattedString();
			currentTestRunElement.Add(new XAttribute("finished", formattedDateTimeNowAsFormattedString));
			Save(base.XDocument, base.LogFilePath);
			currentTestRunElement = null;
		}

		private void EnsureCurrentTestRun()
		{
			if (currentTestRunElement == null)
			{
				currentTestRunElement = new XElement("testRun");
				base.XDocument.Root.Add(currentTestRunElement);
			}
		}
	}
}
