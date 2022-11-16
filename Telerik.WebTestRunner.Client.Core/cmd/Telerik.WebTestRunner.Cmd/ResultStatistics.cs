namespace Telerik.WebTestRunner.Cmd
{
	public class ResultStatistics
	{
		public int TotalTests
		{
			get;
			set;
		}

		public int PassedTests
		{
			get;
			set;
		}

		public int FailedTests
		{
			get;
			set;
		}

		public int IgnoredTests
		{
			get;
			set;
		}

		public int Unrecognized
		{
			get;
			set;
		}
	}
}
