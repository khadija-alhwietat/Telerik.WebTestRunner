using System;
using System.Collections.Generic;
using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Providers
{
	public class TestsLoadedEventArgs : EventArgs
	{
		public List<RunnerTest> Tests
		{
			get;
			private set;
		}

		public TestsLoadedEventArgs(IEnumerable<RunnerTest> tests)
		{
			Tests = new List<RunnerTest>(tests);
		}
	}
}
