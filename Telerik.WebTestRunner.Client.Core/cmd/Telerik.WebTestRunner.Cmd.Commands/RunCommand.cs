using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Telerik.Sitefinity.CommandLineParsing;
using Telerik.WebTestRunner.Client.Configuration;
using Telerik.WebTestRunner.Client.Core.Logger;
using Telerik.WebTestRunner.Client.Core.Logger.MsLogger;
using Telerik.WebTestRunner.Client.Logger;
using Telerik.WebTestRunner.Client.Model;
using Telerik.WebTestRunner.Client.Providers;
using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Cmd.Commands
{
	public class RunCommand : Command
	{
		protected ITestProvider provider;

		private string testProviderTypeName;

		private HashSet<string> testToComplete;

		private int timeOutInMinutes;

		public int singleTimeOutInMinutes;

		private ManualResetEvent testExecutionSynchronizer = new ManualResetEvent(initialState: false);

		private TestResultXmlLogger logger;

		private const string DefaultTraceFilePath = "results.trx";

		private const string MultilingualSuffx = "_ML";

		private const string MultilingualUpgradeSuffx = "_ML_Upgrade";

		public string TfisTokenEndpointUrl
		{
			get
			{
				return RunnerConfig.TfisTokenEndpoint;
			}
			set
			{
				RunnerConfig.TfisTokenEndpoint = value;
				RunnerConfig.TfisAuthEnabled = true;
			}
		}

		public string TfisTokenEndpointBasicAuth
		{
			get
			{
				return RunnerConfig.TfisTokenEndpointBasicAuthentication;
			}
			set
			{
				RunnerConfig.TfisTokenEndpointBasicAuthentication = value;
			}
		}

		public string UserName
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public string MembershipProvider
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public ResultStatistics ResultStatistics
		{
			get;
			set;
		}

		public string TestProviderTypeName
		{
			get
			{
				if (string.IsNullOrEmpty(testProviderTypeName))
				{
					testProviderTypeName = typeof(IntegrationTestsProvider).FullName;
				}
				return testProviderTypeName;
			}
			set
			{
				testProviderTypeName = value;
			}
		}

		public int TimeOutInMinutes
		{
			get
			{
				if (timeOutInMinutes == 0)
				{
					timeOutInMinutes = RunnerConfig.GetTestExecutionTimeout();
				}
				return timeOutInMinutes;
			}
			set
			{
				timeOutInMinutes = value;
			}
		}

		public int SingleTestTimeOutInMinutes
		{
			get
			{
				if (singleTimeOutInMinutes == 0)
				{
					singleTimeOutInMinutes = RunnerConfig.GetSingleTestExecutionTimeout();
				}
				return singleTimeOutInMinutes;
			}
			set
			{
				singleTimeOutInMinutes = value;
			}
		}

		public string TraceFilePath
		{
			get;
			set;
		}

		public string AssemblyName
		{
			get;
			set;
		}

		public string RunName
		{
			get;
			set;
		}

		public string CategoriesFilter
		{
			get;
			set;
		}

		public string Tests
		{
			get;
			set;
		}

		public string DatabaseServer
		{
			get;
			set;
		}

		public string DatabaseName
		{
			get;
			set;
		}

		public LoggerType LoggerType
		{
			get;
			set;
		}

		private IEnumerable<RunnerTest> AllTests
		{
			get;
			set;
		}

		public bool WriteTestResultToConsole
		{
			get;
			set;
		}

		public RunCommand(string[] args)
		{
			this.ParseArguments(args.Skip(1));
			ValidateArguments();
			ConfigureProvider();
			ConfigureTestRunner();
			ConfigureLogging(LoggerType);
		}

		public override void Execute()
		{
			provider.CategoriesFilter = CategoriesFilter;
			provider.TestsFilter = Tests;
			AllTests = provider.GetTests();
			testToComplete = new HashSet<string>(AllTests.Select((RunnerTest test) => test.IdentificationKey));
			if (testToComplete.Any())
			{
				logger.Start(RunName);
				ExecuteAllTest();
				WaitTestToComplete();
				CheckForTimeoutCondition();
				logger.Finish();
			}
			else
			{
				Console.WriteLine("There are no found tests for execution.");
				Environment.Exit(0);
			}
		}

		protected virtual void ConfigureTestRunner()
		{
			if (!string.IsNullOrEmpty(UserName))
			{
				string password = (Password == null) ? "" : Password;
				string text = (MembershipProvider == null) ? "" : MembershipProvider;
				RunnerConfig.SaveUserCredentials(UserName, password, text);
			}
		}

		protected virtual void ConfigureProvider()
		{
			provider = (ITestProvider)Activator.CreateInstance(typeof(ITestProvider).Assembly.GetType(TestProviderTypeName));
			provider.DatabaseName = DatabaseName;
			provider.DatabaseServer = DatabaseServer;
			provider.TestingInstanceUrl = Url;
			provider.SingleTestTimeout = TimeSpan.FromMinutes(SingleTestTimeOutInMinutes);
			if (!string.IsNullOrEmpty(AssemblyName))
			{
				provider.AssemblyName = AssemblyName;
			}
			provider.TestResultReceived += Provider_TestResultReceived;
		}

		protected virtual void Provider_TestResultReceived(RunnerTestResult testResult)
		{
			if (testResult.Result != 0)
			{
				RunnerTestResult runnerTestResult = RenameMultilingualTestResult(testResult);
				if (!testToComplete.Remove(runnerTestResult.IdentificationKey))
				{
					throw new Exception($"The following test could not be removed from test to complete: '{testResult.FixtureName}.{testResult.TestMethodName}'");
				}
				logger.Log(testResult, isToSave: true);
				SetResultStatistics(runnerTestResult);
				if (WriteTestResultToConsole)
				{
					WriteResultOutputStream(runnerTestResult);
				}
				if (testToComplete.Count == 0)
				{
					testExecutionSynchronizer.Set();
					Console.WriteLine("TestExecutionFinished");
				}
			}
		}

		private RunnerTestResult RenameMultilingualTestResult(RunnerTestResult testResult)
		{
			if (testResult.IdentificationKey.Contains("[Multilingual]"))
			{
				testResult.FixtureName += "_ML";
				testResult.TestMethodName += "_ML";
			}
			if (testResult.IdentificationKey.Contains("[Multilingual_Upgrade]"))
			{
				testResult.FixtureName += "_ML_Upgrade";
				testResult.TestMethodName += "_ML_Upgrade";
			}
			return testResult;
		}

		private void WriteResultOutputStream(RunnerTestResult runnerTestResult)
		{
			try
			{
				Console.WriteLine($"{runnerTestResult.TestMethodName} -- '{Enum.GetName(typeof(WcfTestResultEnum), runnerTestResult.Result)}'");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Writing result to the console failed - '{ex.Message}';");
			}
		}

		protected virtual void ExecuteAllTest()
		{
			provider.ExecuteAllTests();
		}

		private void CheckForTimeoutCondition()
		{
			if (testToComplete.Count > 0)
			{
				throw new Exception($"The Integration tests execution timed out. The execution of the tests did not complete in the configured time of {TimeOutInMinutes} minutes");
			}
		}

		private void WaitTestToComplete()
		{
			testExecutionSynchronizer.WaitOne(TimeSpan.FromMinutes(TimeOutInMinutes));
		}

		private void ConfigureLogging(LoggerType type)
		{
			switch (type)
			{
			case LoggerType.MsTrx:
				logger = new MsTestResultXmlLogger(TraceFilePath);
				break;
			case LoggerType.SitefinityXml:
				logger = new SitefinityTestResultXmlLogger(TraceFilePath);
				break;
			default:
				logger = new SitefinityTestResultXmlLogger(TraceFilePath);
				break;
			}
		}

		private void ValidateArguments()
		{
			if (string.IsNullOrEmpty(Url))
			{
				throw new ArgumentNullException("Url");
			}
			if (string.IsNullOrEmpty(RunName))
			{
				throw new ArgumentNullException("RunName");
			}
			if (string.IsNullOrEmpty(TraceFilePath))
			{
				TraceFilePath = "results.trx";
			}
			else
			{
				try
				{
					new FileInfo(TraceFilePath);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException("Invalid TraceFilePath value. Please provide a valid log file path, where the test results will be outputted.", innerException);
				}
			}
			if (!string.IsNullOrEmpty(TfisTokenEndpointUrl) && string.IsNullOrEmpty(TfisTokenEndpointBasicAuth))
			{
				throw new ArgumentException("You must provide basic authentication for the tfis token endpoint issuer.");
			}
		}

		private void SetResultStatistics(RunnerTestResult testResult)
		{
			if (ResultStatistics == null)
			{
				ResultStatistics = new ResultStatistics();
				ResetStatisticsCounter();
				ResultStatistics.TotalTests = AllTests.Count();
			}
			if (testResult.Result == WcfTestResultEnum.Passed)
			{
				ResultStatistics.PassedTests += 1;
			}
			else if (testResult.Result == WcfTestResultEnum.Failed)
			{
				ResultStatistics.FailedTests += 1;
			}
			else if (testResult.Result == WcfTestResultEnum.Ignored)
			{
				ResultStatistics.IgnoredTests += 1;
			}
			else
			{
				ResultStatistics.Unrecognized += 1;
			}
		}

		private void ResetStatisticsCounter()
		{
			if (ResultStatistics != null)
			{
				ResultStatistics.TotalTests = 0;
				ResultStatistics.PassedTests = 0;
				ResultStatistics.FailedTests = 0;
				ResultStatistics.IgnoredTests = 0;
				ResultStatistics.Unrecognized = 0;
			}
		}
	}
}
