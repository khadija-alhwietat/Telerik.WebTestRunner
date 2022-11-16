using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Core.Logger
{
	public abstract class TestResultXmlLogger
	{
		protected string LogFilePath
		{
			get;
			set;
		}

		protected XDocument XDocument
		{
			get;
			set;
		}

		public TestResultXmlLogger(string logFilePath)
		{
			if (string.IsNullOrEmpty(logFilePath))
			{
				throw new ArgumentNullException("logFilePath");
			}
			LogFilePath = logFilePath;
			DirectoryInfo directory = new FileInfo(LogFilePath).Directory;
			if (!directory.Exists)
			{
				Directory.CreateDirectory(directory.FullName);
			}
			if (File.Exists(logFilePath))
			{
				XDocument = XDocument.Load(LogFilePath);
			}
		}

		public abstract void Start(string testRunName);

		public abstract void Finish();

		public abstract void Log(RunnerTestResult result, bool isToSave = false);

		protected virtual void Save(XDocument document, string logFilePath)
		{
			try
			{
				document.Save(logFilePath, SaveOptions.OmitDuplicateNamespaces);
			}
			catch (Exception ex)
			{
				throw new Exception($"The document file could not be saved. Please check the log for more details, Exeption: {ex.Message}, Stack Trace{ex.StackTrace})");
			}
		}

		protected virtual string GetFormattedDateTimeNowAsFormattedString()
		{
			return DateTime.Now.ToString("M/d/yyyy hh:mm:ss", new CultureInfo("en-US"));
		}
	}
}
