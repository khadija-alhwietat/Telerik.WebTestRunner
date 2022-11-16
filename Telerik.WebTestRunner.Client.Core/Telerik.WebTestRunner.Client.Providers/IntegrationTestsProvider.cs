using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Telerik.WebTestRunner.Client.Core.Utilities;
using Telerik.WebTestRunner.Client.DataSource;
using Telerik.WebTestRunner.Client.Launcher;
using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Providers
{
	public class IntegrationTestsProvider : ITestProvider
	{
		public TimeSpan singleTestTimeout;

		private ServiceTestsDataSource dataSource;

		private string categoriesFilter;

		private string testsFilter;

		private TestLauncher launcher;

		private string testingInstanceUrl;

		private string assemblyName;

		private List<RunnerTest> testsList;

		public string TestingInstanceUrl
		{
			get
			{
				return testingInstanceUrl;
			}
			set
			{
				if (testingInstanceUrl != value)
				{
					testingInstanceUrl = value;
					Launcher.TestInstanceUrl = value;
					DataSource.TestingInstanceUrl = value;
				}
			}
		}

		public TimeSpan SingleTestTimeout
		{
			get
			{
				return singleTestTimeout;
			}
			set
			{
				if (singleTestTimeout != value)
				{
					singleTestTimeout = value;
					Launcher.TestExecutionTimeout = value;
				}
			}
		}

		private TestLauncher Launcher
		{
			get
			{
				if (launcher == null)
				{
					if (string.IsNullOrEmpty(TestingInstanceUrl))
					{
						throw new ArgumentNullException("TestingInstanceUrl", "The testing instance url must be set before accessing the test launcher.");
					}
					InitializeLauncher();
				}
				return launcher;
			}
		}

		private ServiceTestsDataSource DataSource
		{
			get
			{
				if (dataSource == null)
				{
					dataSource = new ServiceTestsDataSource();
				}
				return dataSource;
			}
		}

		public string AssemblyName
		{
			get
			{
				return assemblyName;
			}
			set
			{
				assemblyName = value;
			}
		}

		public string CategoriesFilter
		{
			get
			{
				return categoriesFilter;
			}
			set
			{
				categoriesFilter = value;
			}
		}

		public string TestsFilter
		{
			get
			{
				return testsFilter;
			}
			set
			{
				testsFilter = value;
			}
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

		public IProgressMonitor<TestLauncher> ProgressMonitor
		{
			get;
			set;
		}

		public event Action<RunnerTest> TestExecuted;

		public event Action<RunnerTest> TestUpdated;

		public event Action<OperationState> StateChanged;

		public event Action<RunnerTestResult> TestResultReceived;

		public event Func<Exception, bool> ErrorOccured;

		public event Action<TestsLoadedEventArgs> TestsLoaded;

		public void ExecuteTest(RunnerTest test)
		{
			TryWaitForSitefinityToStart();
			Launcher.ExecuteIgnoredTests = true;
			try
			{
				Launcher.StartExecutingTest(test);
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
		}

		public void ExecuteTests(IEnumerable<RunnerTest> items)
		{
			TryWaitForSitefinityToStart();
			Launcher.ExecuteIgnoredTests = false;
			Launcher.StartExecutingTests(items);
		}

		public void ExecuteAllTests()
		{
			TryWaitForSitefinityToStart();
			Launcher.ExecuteIgnoredTests = false;
			try
			{
				Launcher.StartExecutingTests(GetTests());
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
		}

		public virtual IEnumerable<RunnerTest> GetTests()
		{
			try
			{
				TryWaitForSitefinityToStart();
				DataSource.CategoriesFilter = CategoriesFilter;
				DataSource.TestsFilter = TestsFilter;
				DataSource.AssemblyNameFilter = AssemblyName;
				List<RunnerTest> list = testsList = DataSource.GetTests();
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
			return testsList;
		}

		public virtual void GetTestsAsync()
		{
			ThreadPool.QueueUserWorkItem(GetTests);
		}

		protected virtual void Launcher_TestExecutionFinished(object sender, TestResultEventArgs e)
		{
			RaiseTestResultReceived(e.TestResult);
			UpdateTest(e.TestResult, e.Test);
		}

		protected virtual void Launcher_TestExecuted(object sender, TestResultEventArgs e)
		{
			if (this.TestExecuted != null)
			{
				this.TestExecuted(e.Test);
			}
		}

		protected virtual void Launcher_StateChanged(TestLauncher sender, OperationState operationState)
		{
			RaiseStateChanged(operationState);
		}

		protected virtual bool Launcher_ErrorOccured(Exception ex)
		{
			return RaiseErrorOccured(ex);
		}

		protected void UpdateTest(RunnerTestResult result, RunnerTest test)
		{
			test.UpdateInfoFrom(result);
			RaiseTestUpdated(test);
		}

		protected virtual void RaiseTestUpdated(RunnerTest test)
		{
			if (this.TestUpdated != null)
			{
				this.TestUpdated(test);
			}
		}

		protected void UpdateTestExecuted(RunnerTest test)
		{
			if (this.TestExecuted != null)
			{
				this.TestExecuted(test);
			}
		}

		protected virtual void RaiseTestResultReceived(RunnerTestResult testResult)
		{
			if (this.TestResultReceived != null)
			{
				this.TestResultReceived(testResult);
			}
		}

		protected virtual void RaiseStateChanged(OperationState state)
		{
			if (this.StateChanged != null)
			{
				this.StateChanged(state);
			}
		}

		protected virtual bool RaiseErrorOccured(Exception ex)
		{
			if (this.ErrorOccured != null)
			{
				return this.ErrorOccured(ex);
			}
			return true;
		}

		protected virtual void OnTestsReceivedFromServer(TestsLoadedEventArgs args)
		{
			if (this.TestsLoaded != null)
			{
				this.TestsLoaded(args);
			}
		}

		private void InitializeLauncher()
		{
			launcher = new TestLauncher(TestingInstanceUrl, DatabaseServer, DatabaseName);
			launcher.StateChanged += Launcher_StateChanged;
			launcher.TestExecutionFinished += Launcher_TestExecutionFinished;
			launcher.TestExecuted += Launcher_TestExecuted;
			launcher.ErrorOccured += Launcher_ErrorOccured;
			if (ProgressMonitor != null)
			{
				launcher.ProgressMonitor = ProgressMonitor;
			}
		}

		private void ProcessException(Exception ex)
		{
			if (RaiseErrorOccured(ex))
			{
				throw ex;
			}
		}

		private void GetTests(object stateInfo)
		{
			TryWaitForSitefinityToStart();
			ServiceTestsDataSource serviceTestsDataSource = new ServiceTestsDataSource(testingInstanceUrl);
			try
			{
				List<RunnerTest> tests = serviceTestsDataSource.GetTests();
				OnTestsReceivedFromServer(new TestsLoadedEventArgs(tests));
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
		}

		private void TryWaitForSitefinityToStart()
		{
			try
			{
				ServicePointManager.SecurityProtocol = (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
				SitefinityHelper.WaitForSitefinityToStart(TestingInstanceUrl, 600.0);
			}
			catch (Exception innerException)
			{
				ProcessException(new Exception("Sitefinity did not start in less than " + 10 + " minultes.", innerException));
			}
		}
	}
}
