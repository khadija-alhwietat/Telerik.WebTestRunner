using Telerik.WebTestRunner.Client.Model;

namespace Telerik.WebTestRunner.Client.Launcher
{
	public static class TestBatchConfiguratorExtensions
	{
		public static void ConfigureNext(this ITestsBatchConfigurator configurator, RunnerTestBatch batch)
		{
			if (configurator.Next != null)
			{
				configurator.Next.Configure(batch);
			}
		}
	}
}
