using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Model
{
	public class RunnerTestResult : WcfTestResult
	{
		private string identificationKey;

		public string IdentificationKey
		{
			get
			{
				return identificationKey;
			}
			set
			{
				identificationKey = value;
			}
		}

		public RunnerTestResult(RunnerTest test)
		{
			IdentificationKey = test.IdentificationKey;
			base.AuthorName = test.AuthorName;
			base.FixtureName = test.FixtureName;
			base.TestMethodName = test.TestMethodName;
			base.AssemblyName = test.AssemblyName;
		}

		public RunnerTestResult()
		{
		}
	}
}
