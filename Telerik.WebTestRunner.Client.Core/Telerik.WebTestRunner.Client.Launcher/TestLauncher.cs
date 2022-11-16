using Microsoft.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using Telerik.Sitefinity.HttpClientCore.HttpClients;
using Telerik.WebTestRunner.Client.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Sections;
using Telerik.WebTestRunner.Client.Core;
using Telerik.WebTestRunner.Client.Core.Utilities;
using Telerik.WebTestRunner.Client.Model;
using Telerik.WebTestRunner.Client.Providers;
using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Launcher
{
	public class TestLauncher
	{
		private ITestsBatchConfigurator batchConfiguratorsStartingPoint;

		private ISitefinityRestHttpAuthenticateClient client;

		private IProgressMonitor<TestLauncher> progressMonitor;

		private string testInstanceUrl;

		private List<RunnerTest> testsToExecute;

		private TimeSpan? testExecutionTimeout;

		private readonly int defaultTestTimeout = 2;

		private const string TestRunnerServiceUrl = "IntegrationTests/TestRunnerService.svc";

		private const string RunTestMethodName = "RunTest";

		private OperationState state;

		private const string MultilingualModeParamValue = "Multilingual";

		private const string MonolingualModeParamValue = "Monolingual";

		private const string UpgradeModeParamValue = "Multilingual_Upgrade";

		private const string RunFixtureSetup = "RunFixtureSetup";

		private const string RunFixtureTearDown = "RunFixtureTearDown";

		private const string FormsAuthenticationEndPoint = "Sitefinity/Login";

		private const string ClaimsSWTAuthenticationEndPoint = "/Sitefinity/Authenticate/SWT";

		private const string ClaimsOpenIdAuthenticationEndPoint = "/Sitefinity/Authenticate/OpenID";

		public ISitefinityRestHttpAuthenticateClient Client
		{
			get
			{
				if (client == null)
				{
					AuthMode = GetAuthMode(TestInstanceUrl);
					SitefinityRestHttpClientFactory sitefinityRestHttpClientFactory = new SitefinityRestHttpClientFactory();
					client = sitefinityRestHttpClientFactory.GetClient(AuthMode);
					client.BaseUrl = TestInstanceUrl;
					client.TransportSettings.ConnectionTimeout = TestExecutionTimeout;
				}
				return client;
			}
			set
			{
				client = value;
			}
		}

		public TimeSpan TestExecutionTimeout
		{
			get
			{
				if (!testExecutionTimeout.HasValue)
				{
					RunnerConfigSection runnerConfigSection = (RunnerConfigSection)ConfigurationManager.GetSection("runnerConfiguration");
					if (runnerConfigSection == null)
					{
						testExecutionTimeout = TimeSpan.FromMinutes(defaultTestTimeout);
						return testExecutionTimeout.Value;
					}
					int num = (runnerConfigSection.RunnerConfig.SingleTestExecutionTimeout == 0) ? defaultTestTimeout : runnerConfigSection.RunnerConfig.SingleTestExecutionTimeout;
					testExecutionTimeout = TimeSpan.FromMinutes(num);
				}
				return testExecutionTimeout.Value;
			}
			set
			{
				testExecutionTimeout = value;
				if (client != null)
				{
					client.TransportSettings.ConnectionTimeout = value;
				}
			}
		}

		public List<RunnerTest> TestsToExecute
		{
			get
			{
				return testsToExecute;
			}
			set
			{
				if (State == OperationState.Running)
				{
					throw new InvalidOperationException("Tests to execute can't be changed while the test execution is in progress.");
				}
				testsToExecute = value;
			}
		}

		public int LevelOfParallelism
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public string TestInstanceUrl
		{
			get
			{
				return testInstanceUrl;
			}
			set
			{
				if (State == OperationState.Running)
				{
					throw new InvalidOperationException("Can't change the url of the launcher while it is executing tests");
				}
				testInstanceUrl = value;
				if (client != null)
				{
					client.BaseUrl = value;
				}
			}
		}

		public OperationState State
		{
			get
			{
				return state;
			}
			private set
			{
				if (state != value)
				{
					state = value;
					RaiseStateChanged();
				}
			}
		}

		public IProgressMonitor<TestLauncher> ProgressMonitor
		{
			get
			{
				return progressMonitor;
			}
			set
			{
				if (progressMonitor != null)
				{
					progressMonitor.Unsubscribe(this);
				}
				value.Subscribe(this);
				progressMonitor = value;
			}
		}

		public bool ExecuteIgnoredTests
		{
			get;
			set;
		}

		public WcfAuthenticationMode AuthMode
		{
			get;
			set;
		}

		public ITestsBatchConfigurator BatchConfiguratorsChainStartingPoint
		{
			get
			{
				if (batchConfiguratorsStartingPoint == null)
				{
					SetDefaultBatchConfigurationChain();
				}
				return batchConfiguratorsStartingPoint;
			}
			set
			{
				batchConfiguratorsStartingPoint = value;
			}
		}

		public event Action<TestLauncher, OperationState> StateChanged;

		public event Action<TestLauncher> TestRunStarting;

		public event Action<RunnerTest> TestExecuting;

		public event TesetResultEventHandler TestExecutionFinished;

		public event TesetResultEventHandler TestExecuted;

		public event Func<Exception, bool> ErrorOccured;

		public event Action<Action<TestsLoadedEventArgs>, IEnumerable<RunnerTest>> TestsLoaded;

		public TestLauncher(string testingInstanceUrl, string databaseServer = null, string databaseName = null)
		{
			if (string.IsNullOrEmpty(testingInstanceUrl))
			{
				throw new ArgumentNullException("testingInstanceUrl");
			}
			testInstanceUrl = testingInstanceUrl;
		}

		public void StartExecutingTest(RunnerTest test)
		{
			if (TestsToExecute == null)
			{
				TestsToExecute = new List<RunnerTest>();
			}
			TestsToExecute.Clear();
			TestsToExecute.Add(test);
			AsynchLaunch();
		}

		public void StartExecutingTests(IEnumerable<RunnerTest> tests)
		{
			TestsToExecute = new List<RunnerTest>(tests);
			AsynchLaunch();
		}

		public void AsynchLaunch()
		{
			ThreadPool.QueueUserWorkItem(LaunchCallback);
		}

		public void Launch()
		{
			RaiseTestRunStarting();
			State = OperationState.Running;
			List<RunnerTestBatch> list = DefineTestBatches();
			try
			{
				foreach (RunnerTestBatch item in list)
				{
					ConfigureTestInstanceForBatch(item);
					List<IGrouping<string, RunnerTest>> collection = (from t in item.Tests
						where t.Diagnose
						orderby t.FixtureName, t.TestMethodName
						group t by t.FixtureName).ToList();
					Dictionary<RunnerTestResult, RunnerTest> dictionary = new Dictionary<RunnerTestResult, RunnerTest>();
					Dictionary<string, RunnerTest> dictionary2 = new Dictionary<string, RunnerTest>();
					IEnumerable<IGrouping<string, RunnerTest>> enumerable = from t in item.Tests
						orderby t.FixtureName, t.TestMethodName
						group t by t.FixtureName;
					List<IGrouping<string, RunnerTest>> list2 = new List<IGrouping<string, RunnerTest>>();
					foreach (IGrouping<string, RunnerTest> item2 in enumerable)
					{
						list2.Add(item2);
						list2.AddRange(collection);
					}
					bool flag = false;
					string text = string.Empty;
					List<string> list3 = new List<string>();
					foreach (IGrouping<string, RunnerTest> item3 in list2)
					{
						Dictionary<RunnerTestResult, RunnerTest> dictionary3 = new Dictionary<RunnerTestResult, RunnerTest>();
						text = (item3.Any((RunnerTest t) => !t.Diagnose) ? item3.Key : text);
						list3.Add(item3.Key);
						RunnerCredentials activeUserCredentials = RunnerConfig.GetActiveUserCredentials();
						RunnerTest test = new RunnerTest();
						if (!TryAuthenticate(activeUserCredentials, test, out RunnerTestResult result))
						{
							TrySignOut(activeUserCredentials, test, result);
							TryAuthenticate(activeUserCredentials, test, out result);
						}
						FixtureTestResult fixtureTestResult = TryExecuteFixtureSetup(item3.Key);
						new RunnerTestResult(test);
						if (fixtureTestResult.Result == WcfFixtureResultEnum.Passed)
						{
							List<RunnerTest> list4 = item3.Where((RunnerTest t) => t.Diagnose).ToList();
							List<RunnerTest> list5 = new List<RunnerTest>();
							List<RunnerTest> list6 = item3.Where((RunnerTest t) => !t.Diagnose).ToList();
							if (list4.Count > 0 && list6.Count > 0)
							{
								foreach (RunnerTest item4 in list6)
								{
									list5.Add(item4);
									list5.AddRange(list4);
								}
							}
							else
							{
								list5 = item3.ToList();
							}
							foreach (RunnerTest item5 in list5)
							{
								RaiseTestExecuting(item5);
								RunnerTestResult testResult = GetTestResult(item5);
								if (item5.Diagnose && testResult.Result == WcfTestResultEnum.Failed)
								{
									dictionary.Add(testResult, item5);
									if (dictionary.Count > 1)
									{
										List<string> list7 = new List<string>(list3);
										list7.Add("\nTests execution trace from current fixture:\n");
										list7.AddRange(dictionary.Select((KeyValuePair<RunnerTestResult, RunnerTest> f) => f.Value.TestMethodName));
										list3 = list7;
									}
									string text2 = testResult.Message = GetDiagnoseMessage(testResult.Message, text, list3);
									RiseTestExecuted(testResult, item5);
									flag = true;
									break;
								}
								string identificationKey = item5.IdentificationKey;
								if (!dictionary2.ContainsKey(identificationKey))
								{
									dictionary2.Add(identificationKey, item5);
									dictionary.Add(testResult, item5);
									RiseTestExecuted(testResult, item5);
								}
							}
						}
						else
						{
							if (item3.Any((RunnerTest t) => t.Diagnose))
							{
								string text3 = fixtureTestResult.Message = GetDiagnoseMessage(fixtureTestResult.Message, text, list3);
								flag = true;
							}
							FailAllTestsOnFixtureSetupFailure(item3, fixtureTestResult.Message, dictionary3);
						}
						TryAuthenticate(activeUserCredentials, test, out result);
						FixtureTestResult fixtureTestResult2 = TryExecuteFixtureTearDown(item3.Key);
						if (fixtureTestResult2.Result == WcfFixtureResultEnum.Passed)
						{
							if (dictionary.Count > 0)
							{
								InvokeTestExecutionFinished(dictionary, null);
							}
							if (dictionary3.Count > 0)
							{
								InvokeTestExecutionFinished(dictionary3, null);
							}
						}
						else
						{
							if (item3.Any((RunnerTest t) => t.Diagnose))
							{
								string text4 = fixtureTestResult.Message = GetDiagnoseMessage(fixtureTestResult.Message, text, list3);
								flag = true;
							}
							FailAllTestsOnFixtureTearDown(item3, fixtureTestResult, dictionary3, fixtureTestResult2, dictionary);
						}
						if (flag)
						{
							return;
						}
					}
				}
			}
			catch (AggregateException ex)
			{
				StringBuilder stringBuilder = new StringBuilder("AggregateException occurred:");
				foreach (Exception innerException in ex.Flatten().InnerExceptions)
				{
					stringBuilder.Append($"Message: {innerException.Message}, StackTrace: {innerException.StackTrace}");
				}
				throw new Exception(stringBuilder.ToString());
			}
			catch (Exception ex2)
			{
				if (RaiseErrorOccured(ex2))
				{
					StringBuilder stringBuilder2 = new StringBuilder("Exception occurred:");
					stringBuilder2.Append($"Message: {ex2.Message}, StackTrace: {ex2.StackTrace}");
					if (ex2.InnerException != null)
					{
						stringBuilder2.Append($"InnerException Message: {ex2.InnerException.Message}, StackTrace: {ex2.InnerException.StackTrace}");
					}
					throw new Exception(stringBuilder2.ToString());
				}
			}
			finally
			{
				State = OperationState.Finished;
			}
		}

		protected virtual RunnerTestResult GetTestResult(RunnerTest test)
		{
			WcfAuthenticationMode authenticationMode = test.AuthenticationMode;
			bool flag = false;
			flag = true;
			if ((ExecuteIgnoredTests || !test.IsToIgnore) && (flag || authenticationMode == AuthMode))
			{
				RunnerCredentials credentials = test.Credentials;
				if (TryAuthenticate(credentials, test, out RunnerTestResult result))
				{
					return ExecuteTest(test);
				}
				return result;
			}
			return GetIgnoredTestResult(test);
		}

		protected virtual bool TryAuthenticate(RunnerCredentials credentials, RunnerTest test, out RunnerTestResult result)
		{
			try
			{
				SitefinityHelper.WaitForSitefinityToStart(TestInstanceUrl, 600.0);
				Client.ForceAuthenticateToServer(credentials.Provider, credentials.UserName, credentials.Password, rememberMe: true);
				result = null;
				return true;
			}
			catch (Exception ex)
			{
				Exception ex2 = new Exception($"Test Authentication failed. Exception is {ex}, Message: {ex.Message}, InnerException: {ex.InnerException}, Source {ex.Source}, StackTrace {ex.StackTrace}, ");
				result = GetTestFailedTestResult(test, ex2);
				return false;
			}
		}

		private bool TrySignOut(RunnerCredentials credentials, RunnerTest test, RunnerTestResult result)
		{
			try
			{
				Client.Logout(credentials.Provider, credentials.UserName, credentials.Password);
				return true;
			}
			catch (Exception ex)
			{
				Exception ex2 = new Exception($"Test Sign out failed. Exception is {ex}, Message: {ex.Message}, InnerException: {ex.InnerException}, Source {ex.Source}, StackTrace {ex.StackTrace}, ");
				result = GetTestFailedTestResult(test, ex2);
				return false;
			}
		}

		private RunnerTestResult GetTestFailedTestResult(RunnerTest test, Exception ex)
		{
			RunnerTestResult runnerTestResult = new RunnerTestResult(test);
			runnerTestResult.Result = WcfTestResultEnum.Failed;
			runnerTestResult.Message = $"The test failed. Original test result is {test.ExecutionResult.ToString()}. Exception details:{test.ExecutionMessage}, {ex.GetType().Name}, {ex.ToString()}";
			return runnerTestResult;
		}

		protected virtual FixtureTestResult TryExecuteFixtureSetup(string fixtureName)
		{
			string text = HttpUtility.UrlEncode(fixtureName);
			string text2 = string.Format("{0}/{1}/{2}?fixtureTypeFullName={3}", Client.BaseAddress.ToString().TrimEnd('/'), "IntegrationTests/TestRunnerService.svc", "RunFixtureSetup", text);
			FixtureTestResult fixtureTestResult = new FixtureTestResult();
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				httpResponseMessage = Client.Get(text2);
				if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception($"The service response status code was: {(int)httpResponseMessage.StatusCode}: {Enum.GetName(typeof(HttpStatusCode), httpResponseMessage.StatusCode)}. The url was: {text2}");
				}
				httpResponseMessage.Content.LoadIntoBuffer();
				FixtureTestResult fixtureTestResult2 = httpResponseMessage.Content.ReadAsJsonDataContract<FixtureTestResult>();
				fixtureTestResult.FixtureName = fixtureTestResult2.FixtureName;
				fixtureTestResult.Message = fixtureTestResult2.Message;
				fixtureTestResult.Result = fixtureTestResult2.Result;
				return fixtureTestResult;
			}
			catch (SerializationException ex)
			{
				fixtureTestResult.FixtureName = fixtureName;
				fixtureTestResult.Result = WcfFixtureResultEnum.Failed;
				fixtureTestResult.Message = $"Fixture Setup service call failed with the following exception: {ex.GetType().Name}, {ex.Message}";
				return fixtureTestResult;
			}
			catch (Exception ex2)
			{
				fixtureTestResult.FixtureName = fixtureName;
				fixtureTestResult.Result = WcfFixtureResultEnum.Failed;
				fixtureTestResult.Message = $"Fixture Setup service call failed with the following exception: {ex2.GetType().Name}, {ex2.Message}";
				if (httpResponseMessage == null)
				{
					return fixtureTestResult;
				}
				fixtureTestResult.Message += $"Response content:{httpResponseMessage.Content.ReadAsString()}";
				return fixtureTestResult;
			}
		}

		protected virtual FixtureTestResult TryExecuteFixtureTearDown(string fixtureName)
		{
			string text = HttpUtility.UrlEncode(fixtureName);
			string text2 = string.Format("{0}/{1}/{2}?fixtureTypeFullName={3}", Client.BaseAddress.ToString().TrimEnd('/'), "IntegrationTests/TestRunnerService.svc", "RunFixtureTearDown", text);
			HttpResponseMessage httpResponseMessage = null;
			FixtureTestResult fixtureTestResult = new FixtureTestResult();
			try
			{
				httpResponseMessage = Client.Get(text2);
				if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception($"The service response status code was: {(int)httpResponseMessage.StatusCode}: {Enum.GetName(typeof(HttpStatusCode), httpResponseMessage.StatusCode)}. The url was: {text2}");
				}
				httpResponseMessage.Content.LoadIntoBuffer();
				FixtureTestResult fixtureTestResult2 = httpResponseMessage.Content.ReadAsJsonDataContract<FixtureTestResult>();
				fixtureTestResult.FixtureName = fixtureTestResult2.FixtureName;
				fixtureTestResult.Message = fixtureTestResult2.Message;
				fixtureTestResult.Result = fixtureTestResult2.Result;
				return fixtureTestResult;
			}
			catch (SerializationException ex)
			{
				fixtureTestResult.FixtureName = fixtureName;
				fixtureTestResult.Result = WcfFixtureResultEnum.Failed;
				fixtureTestResult.Message = $"Fixture Setup service call failed with the following exception: {ex.GetType().Name}, {ex.Message}";
				return fixtureTestResult;
			}
			catch (Exception ex2)
			{
				fixtureTestResult.FixtureName = fixtureName;
				fixtureTestResult.Result = WcfFixtureResultEnum.Failed;
				fixtureTestResult.Message = $"Fixture Teardown service call failed with the following exception: {ex2.GetType().Name}, {ex2.Message}";
				if (httpResponseMessage == null)
				{
					return fixtureTestResult;
				}
				fixtureTestResult.Message += $"Response content:{httpResponseMessage.Content.ReadAsString()}";
				return fixtureTestResult;
			}
		}

		protected virtual RunnerTestResult ExecuteTest(RunnerTest test)
		{
			string text = HttpUtility.UrlEncode(test.FixtureName);
			string text2 = string.Format("{0}/{1}/{2}?fixtureTypeFullName={3}&testName={4}&assemblyName={5}", Client.BaseAddress.ToString().TrimEnd('/'), "IntegrationTests/TestRunnerService.svc", "RunTest", text, test.TestMethodName, test.AssemblyName);
			RunnerTestResult runnerTestResult;
			try
			{
				HttpResponseMessage httpResponseMessage = Client.Get(text2);
				if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception($"The service response status code was: {(int)httpResponseMessage.StatusCode}: {Enum.GetName(typeof(HttpStatusCode), httpResponseMessage.StatusCode)}. The url was: {text2}");
				}
				string text3 = string.Empty;
				try
				{
					text3 = httpResponseMessage.Content.ReadAsString();
					runnerTestResult = (new DataContractJsonSerializer(typeof(RunnerTestResult)).ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(text3))) as RunnerTestResult);
				}
				catch (SerializationException innerException)
				{
					throw new Exception($"Serialization to RunnerTestResult failed. Actual response content:\n{text3}", innerException);
				}
			}
			catch (Exception ex)
			{
				runnerTestResult = new RunnerTestResult(test);
				runnerTestResult.Result = WcfTestResultEnum.Failed;
				runnerTestResult.Message = $"Calling the integration tests runner service failed with the following exception: {ex.GetType().Name}, {ex.ToString()}";
			}
			return SetRunnerTestResultIdentificationData(test, runnerTestResult);
		}

		protected virtual RunnerTestResult SetRunnerTestResultIdentificationData(RunnerTest test, RunnerTestResult testResult)
		{
			testResult.IdentificationKey = test.IdentificationKey;
			if (string.IsNullOrEmpty(testResult.AssemblyName) && string.IsNullOrEmpty(test.AssemblyName))
			{
				testResult.AssemblyName = test.AssemblyName;
			}
			return testResult;
		}

		protected virtual List<RunnerTestBatch> DefineTestBatches()
		{
			RunnerTestBatch runnerTestBatch = new RunnerTestBatch();
			RunnerTestBatch runnerTestBatch2 = new RunnerTestBatch();
			RunnerTestBatch runnerTestBatch3 = new RunnerTestBatch
			{
				IsMultilingual = true
			};
			RunnerTestBatch runnerTestBatch4 = new RunnerTestBatch
			{
				IsMultilingual = true
			};
			foreach (RunnerTest item in TestsToExecute)
			{
				if (!item.IsMultilingual)
				{
					runnerTestBatch.Tests.Add(item);
				}
				else
				{
					if (item.ExecutionMode == MultilingualExecutionMode.MultiAndMonoLingual)
					{
						if (item.Parameters.ContainsValue("Monolingual"))
						{
							runnerTestBatch.Tests.Add(item);
							continue;
						}
						if (item.Parameters.ContainsValue("Multilingual"))
						{
							runnerTestBatch4.Tests.Add(item);
							continue;
						}
					}
					if (item.IsMultilingual && item.ExecutionMode == MultilingualExecutionMode.Upgrade)
					{
						if (item.Parameters.ContainsValue("Monolingual"))
						{
							runnerTestBatch2.Tests.Add(item);
						}
						else
						{
							if (!item.Parameters.ContainsValue("Multilingual") && !item.Parameters.ContainsValue("Multilingual_Upgrade"))
							{
								throw new Exception("Invalid multilingual tests. Each multilingual tests should have as a parameter its test mode.");
							}
							runnerTestBatch3.Tests.Add(item);
						}
					}
					else if (item.IsMultilingual && item.ExecutionMode == MultilingualExecutionMode.Multilingual)
					{
						runnerTestBatch4.Tests.Add(item);
					}
				}
			}
			return new List<RunnerTestBatch>
			{
				runnerTestBatch,
				runnerTestBatch2,
				runnerTestBatch3,
				runnerTestBatch4
			};
		}

		protected virtual void SetDefaultBatchConfigurationChain()
		{
			batchConfiguratorsStartingPoint = new MultilingualTestsBatchConfigurator(Client);
		}

		private void ConfigureTestInstanceForBatch(RunnerTestBatch batch)
		{
			BatchConfiguratorsChainStartingPoint.Configure(batch);
		}

		protected virtual void RaiseTestExecutionFinished(RunnerTestResult testResult, RunnerTest test)
		{
			if (this.TestExecutionFinished != null)
			{
				this.TestExecutionFinished(this, new TestResultEventArgs(testResult, test));
			}
		}

		protected virtual void RiseTestExecuted(RunnerTestResult testResult, RunnerTest test)
		{
			if (this.TestExecuted != null)
			{
				this.TestExecuted(this, new TestResultEventArgs(testResult, test));
			}
		}

		protected virtual void RaiseStateChanged()
		{
			if (this.StateChanged != null)
			{
				this.StateChanged(this, State);
			}
		}

		protected virtual void RaiseTestRunStarting()
		{
			if (this.TestRunStarting != null)
			{
				this.TestRunStarting(this);
			}
		}

		protected virtual void RaiseTestExecuting(RunnerTest test)
		{
			if (this.TestExecuting != null)
			{
				this.TestExecuting(test);
			}
		}

		protected virtual bool RaiseErrorOccured(Exception ex)
		{
			if (this.ErrorOccured != null)
			{
				return this.ErrorOccured(ex);
			}
			return true;
		}

		private RunnerTestResult GetIgnoredTestResult(RunnerTest test)
		{
			return new RunnerTestResult(test)
			{
				Result = WcfTestResultEnum.Ignored
			};
		}

		private string GetDiagnoseMessage(string originalMessage, string previousFixture, List<string> fullExecutionTrace)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("DIAGNOSE: Fail caused by Fixture: {0}", previousFixture);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Full execution trace:");
			foreach (string item in fullExecutionTrace)
			{
				stringBuilder.AppendLine(item);
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append(originalMessage);
			return stringBuilder.ToString();
		}

		private void LaunchCallback(object threadContext)
		{
			Launch();
		}

		private WcfAuthenticationMode GetAuthMode(string baseUrl)
		{
			if (RunnerConfig.TfisAuthEnabled)
			{
				return WcfAuthenticationMode.Tfis;
			}
			WebRequest webRequest = WebRequest.Create(VirtualPathUtility.RemoveTrailingSlash(baseUrl) + "/Sitefinity");
			webRequest.Timeout = 240000;
			using (WebResponse webResponse = webRequest.GetResponse())
			{
				string absolutePath = webResponse.ResponseUri.AbsolutePath;
				WcfAuthenticationMode result = WcfAuthenticationMode.Default;
				if (absolutePath.EndsWith("Sitefinity/Login"))
				{
					result = WcfAuthenticationMode.Default;
				}
				else if (absolutePath.Contains("/Sitefinity/Authenticate/SWT"))
				{
					result = WcfAuthenticationMode.Claims;
				}
				else if (absolutePath.Contains("/Sitefinity/Authenticate/OpenID"))
				{
					result = WcfAuthenticationMode.OpenId;
				}
				return result;
			}
		}

		private void InvokeTestExecutionFinished(Dictionary<RunnerTestResult, RunnerTest> dictionary, string message)
		{
			foreach (KeyValuePair<RunnerTestResult, RunnerTest> item in dictionary)
			{
				if (!string.IsNullOrEmpty(message))
				{
					item.Key.Message = message;
				}
				RaiseTestExecutionFinished(item.Key, item.Value);
			}
			dictionary.Clear();
		}

		private void FailAllTestsOnFixtureSetupFailure(IGrouping<string, RunnerTest> fixture, string errorMessage, Dictionary<RunnerTestResult, RunnerTest> setupFailedTests)
		{
			foreach (RunnerTest item in fixture)
			{
				RaiseTestExecuting(item);
				RunnerTestResult runnerTestResult = SetRunnerTestResultOnFixtureFailure(item, errorMessage);
				RiseTestExecuted(runnerTestResult, item);
				setupFailedTests.Add(runnerTestResult, item);
			}
		}

		private void FailAllTestsOnFixtureTearDown(IGrouping<string, RunnerTest> fixture, FixtureTestResult setup, Dictionary<RunnerTestResult, RunnerTest> setupFailedTests, FixtureTestResult tearDown, Dictionary<RunnerTestResult, RunnerTest> executedTests)
		{
			foreach (RunnerTest item in fixture)
			{
				RunnerTestResult runnerTestResult = SetRunnerTestResultOnFixtureFailure(item, tearDown.Message);
				runnerTestResult.Message = $"{tearDown.Message}. Original test result is: '{item.ExecutionResult.ToString()};{item.ExecutionMessage}'";
				if (setup.Result == WcfFixtureResultEnum.Passed)
				{
					RaiseTestExecutionFinished(runnerTestResult, item);
					executedTests.Clear();
				}
				else
				{
					InvokeTestExecutionFinished(setupFailedTests, $"'FixtureSetup' and 'FixtureTearDown' failed.  {setup.Message}; {runnerTestResult.Message}");
				}
			}
		}

		private RunnerTestResult SetRunnerTestResultOnFixtureFailure(RunnerTest test, string errorMessage)
		{
			if (!ExecuteIgnoredTests && test.ExecutionResult == WcfTestResultEnum.Ignored)
			{
				return GetIgnoredTestResult(test);
			}
			Exception ex = new Exception(errorMessage);
			return GetTestFailedTestResult(test, ex);
		}
	}
}
