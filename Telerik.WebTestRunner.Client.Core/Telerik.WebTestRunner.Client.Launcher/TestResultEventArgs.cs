using System;
using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Launcher
{
	public class TestResultEventArgs : EventArgs
	{
		private RunnerTest test;

		private RunnerTestResult testResult;

		public RunnerTest Test
		{
			get
			{
				return test;
			}
			set
			{
				test = value;
			}
		}

		public RunnerTestResult TestResult
		{
			get
			{
				return testResult;
			}
			set
			{
				testResult = value;
			}
		}

		public TestResultEventArgs(RunnerTestResult testResult, RunnerTest test)
		{
			this.test = test;
			this.testResult = testResult;
		}
	}
}
