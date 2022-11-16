using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Core.Diagnostics
{
	public class DatabaseDiagnosticsXmlLogger
	{
		private XElement currentTestRunElement;

		private static DatabaseDiagnosticsXmlLogger instance;

		public static DatabaseDiagnosticsXmlLogger Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new DatabaseDiagnosticsXmlLogger();
				}
				return instance;
			}
		}

		public string LogFilePath
		{
			get;
			set;
		}

		private XDocument XDocument
		{
			get;
			set;
		}

		private DatabaseDiagnosticsXmlLogger()
		{
		}

		public void Start(string testRunName, string logfilePath)
		{
			InitializeLogger(logfilePath);
			EnsureCurrentTestRun();
			string formattedDateTimeNowAsFormattedString = GetFormattedDateTimeNowAsFormattedString();
			currentTestRunElement.Add(new XAttribute("started", formattedDateTimeNowAsFormattedString));
			currentTestRunElement.Add(new XAttribute("name", testRunName));
		}

		private void InitializeLogger(string logfilePath)
		{
			LogFilePath = logfilePath;
			DirectoryInfo directory = new FileInfo(LogFilePath).Directory;
			if (!directory.Exists)
			{
				Directory.CreateDirectory(directory.FullName);
			}
			if (File.Exists(LogFilePath))
			{
				XDocument = XDocument.Load(LogFilePath);
			}
			else
			{
				XDocument = new XDocument();
			}
			XDocument.Add(new XElement("TestResults"));
			EnsureCurrentTestRun();
		}

		public void Finish()
		{
			Save(XDocument, LogFilePath);
		}

		public void Log(RunnerTestResult result, List<Tuple<string, int>> changes, List<string> newTables)
		{
			EnsureCurrentTestRun();
			XElement xElement = new XElement("test");
			xElement.Add(new XAttribute("name", result.TestMethodName));
			xElement.Add(new XAttribute("overallResult", result.Result));
			xElement.Add(new XAttribute("owner", result.AuthorName));
			xElement.Add(new XAttribute("fixture", result.FixtureName));
			XElement xElement2 = new XElement("tables");
			foreach (Tuple<string, int> change in changes)
			{
				XElement xElement3 = new XElement("table");
				xElement3.Add(new XAttribute("name", change.Item1));
				xElement3.Add(new XAttribute("rows", Math.Abs(change.Item2)));
				string value = (change.Item2 > 0) ? "added" : "removed";
				xElement3.Add(new XAttribute("kind", value));
				xElement2.Add(xElement3);
			}
			foreach (string newTable in newTables)
			{
				XElement xElement4 = new XElement("table");
				xElement4.Add(new XAttribute("name", newTable));
				xElement4.Add(new XAttribute("isNewTable", "true"));
				xElement2.Add(xElement4);
			}
			xElement.Add(xElement2);
			currentTestRunElement.Add(xElement);
			Save(XDocument, LogFilePath);
		}

		private void Save(XDocument document, string logFilePath)
		{
			try
			{
				document.Save(logFilePath);
			}
			catch (Exception ex)
			{
				throw new Exception($"The document file could not be saved. Please check the log for more details, Exeption: {ex.Message}, Stack Trace{ex.StackTrace})");
			}
		}

		private string GetFormattedDateTimeNowAsFormattedString()
		{
			return DateTime.Now.ToString("M/d/yyyy hh:mm:ss", new CultureInfo("en-US"));
		}

		private void EnsureCurrentTestRun()
		{
			if (currentTestRunElement == null)
			{
				currentTestRunElement = new XElement("testRun");
				XDocument.Root.Add(currentTestRunElement);
			}
		}
	}
}
