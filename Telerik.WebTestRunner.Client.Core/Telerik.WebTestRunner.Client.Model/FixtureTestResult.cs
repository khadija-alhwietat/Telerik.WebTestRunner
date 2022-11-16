using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Model
{
	public class FixtureTestResult
	{
		public string FixtureName
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public WcfFixtureResultEnum Result
		{
			get;
			set;
		}
	}
}
