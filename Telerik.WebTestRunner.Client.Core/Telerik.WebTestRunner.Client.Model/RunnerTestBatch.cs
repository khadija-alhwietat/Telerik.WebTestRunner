using System.Collections.Generic;

namespace Telerik.WebTestRunner.Client.Model
{
	public class RunnerTestBatch
	{
		private List<RunnerTest> tests;

		public List<RunnerTest> Tests
		{
			get
			{
				if (tests == null)
				{
					tests = new List<RunnerTest>();
				}
				return tests;
			}
		}

		public bool IsMultilingual
		{
			get;
			set;
		}
	}
}
