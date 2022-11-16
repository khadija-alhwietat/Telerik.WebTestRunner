using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Model
{
	public class RunnerModelMapper
	{
		public static RunnerCredentials DefaultCredentials
		{
			get;
			set;
		}

		public static RunnerTest CreateRunnerTest(WcfTestInfo testInfo)
		{
			return new RunnerTest
			{
				AssemblyName = testInfo.AssemblyName,
				AuthorName = testInfo.AuthorName,
				FixtureName = testInfo.FixtureName,
				IsToIgnore = testInfo.IsToIgnore,
				TestMethodName = testInfo.TestMethodName,
				IdentificationKey = testInfo.IdentificationKey,
				IsMultilingual = testInfo.IsMultilingual,
				ExecutionModeString = testInfo.MultilingualExecutionMode,
				AuthenticationMode = testInfo.AuthenticationMode,
				Parameters = testInfo.Parameters,
				Credentials = (testInfo.HasCustomAuthentication ? new RunnerCredentials(testInfo.Credentials.UserName, testInfo.Credentials.Password, testInfo.Credentials.Provider) : DefaultCredentials)
			};
		}
	}
}
