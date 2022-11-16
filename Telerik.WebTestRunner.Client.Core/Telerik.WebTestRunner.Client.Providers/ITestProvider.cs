using System;
using System.Collections.Generic;
using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Providers
{
	public interface ITestProvider
	{
		string TestingInstanceUrl
		{
			get;
			set;
		}

		TimeSpan SingleTestTimeout
		{
			get;
			set;
		}

		string AssemblyName
		{
			get;
			set;
		}

		string CategoriesFilter
		{
			get;
			set;
		}

		string TestsFilter
		{
			get;
			set;
		}

		string DatabaseServer
		{
			get;
			set;
		}

		string DatabaseName
		{
			get;
			set;
		}

		event Action<RunnerTest> TestUpdated;

		event Action<OperationState> StateChanged;

		event Action<RunnerTestResult> TestResultReceived;

		event Func<Exception, bool> ErrorOccured;

		event Action<TestsLoadedEventArgs> TestsLoaded;

		void ExecuteTest(RunnerTest test);

		void ExecuteAllTests();

		IEnumerable<RunnerTest> GetTests();
	}
}
