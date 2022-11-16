using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Telerik.WebTestRunner.Client.Core.Logger.MsLogger.Model;
using Telerik.WebTestRunner.Client.Model;
using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Core.Logger.MsLogger
{
	public class MsTestResultXmlLogger : TestResultXmlLogger
	{
		private const string XmlnsConfigUri = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

		private const string XmlnsPrefix = "xmlns";

		private const string ResultsNotInListName = "Results Not in a List";

		private const string AllLoadedResultsName = "All Loaded Results";

		private const string ResultsNotInListId = "8c84fa94-04c1-424b-9868-57a2d4851a1d";

		private const string AllLoadedResultsId = "19431567-8539-422a-85d7-44ee4e166bda";

		private const string TestType = "13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b";

		public TestRun TestRun
		{
			get;
			set;
		}

		public TestSettings TestSettings
		{
			get;
			set;
		}

		public ResultSummary ResultsSummary
		{
			get;
			set;
		}

		public Counters Counters
		{
			get;
			set;
		}

		public TestDefinitions TestDefinitions
		{
			get;
			set;
		}

		public TestLists TestLists
		{
			get;
			set;
		}

		public TestList TestList
		{
			get;
			set;
		}

		public TestEntries TestEntries
		{
			get;
			set;
		}

		public Results Results
		{
			get;
			set;
		}

		public MsTestResultXmlLogger(string logFilePath)
			: base(logFilePath)
		{
		}

		public void InitializeDefaultProperties()
		{
			if (TestRun == null || TestSettings == null || ResultsSummary == null || Counters == null || TestDefinitions == null || TestLists == null || TestList == null || TestEntries == null || Results == null)
			{
				TestRun = new TestRun();
				TestSettings = new TestSettings
				{
					Name = "Test Settings",
					Id = Guid.NewGuid().ToString()
				};
				ResultsSummary = new ResultSummary();
				Counters = new Counters();
				TestDefinitions = new TestDefinitions();
				TestLists = new TestLists();
				TestEntries = new TestEntries();
				Results = new Results();
			}
		}

		public void InitializeDefaultXml(string testRunName)
		{
			TestRun.TestSettings = TestSettings;
			TestRun.ResultSummary = ResultsSummary;
			TestRun.ResultSummary.Counters = Counters;
			TestRun.TestDefinitions = TestDefinitions;
			TestRun.TestLists = TestLists;
			TestRun.TestEntries = TestEntries;
			TestRun.Results = Results;
			base.XDocument = new XDocument();
			XElement xElement = new XmlSerializer(TestRun.GetType(), "http://microsoft.com/schemas/VisualStudio/TeamTest/2010").SerializeToXElement(TestRun);
			xElement.Add(new XAttribute("id", Guid.NewGuid()));
			xElement.Add(new XAttribute("name", testRunName));
			xElement.Add(new XAttribute("runUser", string.Empty));
			base.XDocument.Add(xElement);
			TestList obj = new TestList
			{
				Name = "All Loaded Results",
				Id = "19431567-8539-422a-85d7-44ee4e166bda"
			};
			SaveData(obj);
			TestList obj2 = new TestList
			{
				Name = "Results Not in a List",
				Id = "8c84fa94-04c1-424b-9868-57a2d4851a1d"
			};
			SaveData(obj2);
		}

		public override void Start(string testRunName)
		{
			InitializeDefaultProperties();
			InitializeDefaultXml(testRunName);
		}

		public override void Finish()
		{
			if (Counters.Total == Counters.Passed)
			{
				ResultsSummary.Outcome = "Passed";
			}
			else
			{
				ResultsSummary.Outcome = "Failed";
			}
			SaveData(ResultsSummary);
		}

		public override void Log(RunnerTestResult result, bool isToSave = false)
		{
			Counters.Total++;
			if (result.Result == WcfTestResultEnum.Ignored || result.Result == WcfTestResultEnum.NotRun)
			{
				Counters.NotExecuted++;
				if (isToSave)
				{
					SaveData(Counters);
				}
				return;
			}
			if (result.Result == WcfTestResultEnum.Passed)
			{
				Counters.Passed++;
			}
			else
			{
				Counters.Failed++;
			}
			UnitTest unitTest = new UnitTest
			{
				Name = result.TestMethodName,
				Storage = Assembly.GetExecutingAssembly().Location,
				Id = Guid.NewGuid().ToString(),
				Execution = new Execution
				{
					Id = Guid.NewGuid().ToString()
				},
				TestMethod = new TestMethod
				{
					ClassName = $"{result.FixtureName}, {result.AssemblyName}",
					Name = result.TestMethodName,
					CodeBase = result.AssemblyName
				}
			};
			UnitTestResult unitTestResult = new UnitTestResult
			{
				TestName = unitTest.Name,
				TestId = unitTest.Id,
				Duration = result.TestDuration.ToString(),
				Outcome = result.Result.ToString(),
				ExecutionId = unitTest.Execution.Id,
				TestListId = "8c84fa94-04c1-424b-9868-57a2d4851a1d",
				TestType = "13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b",
				ComputerName = Environment.MachineName,
				StartTime = result.StartTime,
				EndTime = result.EndTime,
				Output = new Output
				{
					ErrorInfo = new ErrorInfo
					{
						Message = result.Message
					}
				}
			};
			TestEntry testEntry = new TestEntry
			{
				TestId = unitTest.Id,
				ExecutionId = unitTest.Execution.Id,
				TestListId = "8c84fa94-04c1-424b-9868-57a2d4851a1d"
			};
			TestDefinitions.UnitTest = unitTest;
			Results.UnitTestResult = unitTestResult;
			TestEntries.TestEntry = testEntry;
			if (isToSave)
			{
				SaveData(unitTest);
				SaveData(unitTestResult);
				SaveData(testEntry);
				SaveData(Counters);
			}
		}

		private void SaveData(object obj)
		{
			if (base.XDocument == null || obj == null)
			{
				return;
			}
			Type type = obj.GetType();
			if (type == typeof(UnitTest))
			{
				UpdateLogDocument(obj, "TestDefinitions");
				return;
			}
			if (type == typeof(UnitTestResult))
			{
				UpdateLogDocument(obj, "Results");
				return;
			}
			if (type == typeof(TestEntry))
			{
				UpdateLogDocument(obj, "TestEntries");
				return;
			}
			if (type == typeof(TestList))
			{
				UpdateLogDocument(obj, "TestLists");
				return;
			}
			if (type == typeof(ResultSummary))
			{
				ReplaceElement(obj, TestRun);
				return;
			}
			if (type == typeof(Counters))
			{
				ReplaceElement(obj, ResultsSummary);
				return;
			}
			throw new ArgumentException("Unexpected type: " + obj.GetType());
		}

		private void UpdateLogDocument(object obj, string elementName)
		{
			XElement xElementByLocalName = GetXElementByLocalName(elementName);
			if (xElementByLocalName != null && obj != null)
			{
				XElement content = new XmlSerializer(obj.GetType(), "http://microsoft.com/schemas/VisualStudio/TeamTest/2010").SerializeToXElement(obj);
				xElementByLocalName.Add(content);
				Save(base.XDocument, base.LogFilePath);
			}
		}

		private void ReplaceElement(object obj, object parent)
		{
			string elementName = (from p in parent.GetType().GetProperties()
				where p.PropertyType == obj.GetType()
				select p).FirstOrDefault().GetCustomAttributes<XmlElementAttribute>().FirstOrDefault()
				.ElementName;
			XElement xElementByLocalName = GetXElementByLocalName(elementName);
			if (xElementByLocalName != null && obj != null)
			{
				XElement content = new XmlSerializer(obj.GetType(), "http://microsoft.com/schemas/VisualStudio/TeamTest/2010").SerializeToXElement(obj);
				xElementByLocalName.ReplaceWith(content);
				Save(base.XDocument, base.LogFilePath);
			}
		}

		private XElement GetXElementByLocalName(string descendant)
		{
			return base.XDocument.Descendants().SingleOrDefault((XElement d) => d.Name.LocalName == descendant);
		}

		private XElement SetXmlnsValue(XElement element)
		{
			XmlElement xmlElement = element.ToXmlElement();
			xmlElement.SetAttribute("xmlns", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
			return XElement.Parse(xmlElement.OuterXml);
		}
	}
}
