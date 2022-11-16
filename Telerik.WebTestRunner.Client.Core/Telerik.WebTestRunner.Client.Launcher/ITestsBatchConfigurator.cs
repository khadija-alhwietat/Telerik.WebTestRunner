using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Launcher
{
	public interface ITestsBatchConfigurator
	{
		ITestsBatchConfigurator Next
		{
			get;
			set;
		}

		void Configure(RunnerTestBatch batch);
	}
}
