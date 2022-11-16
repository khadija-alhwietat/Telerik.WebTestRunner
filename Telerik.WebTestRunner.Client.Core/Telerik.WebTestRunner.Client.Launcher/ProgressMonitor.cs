using System;

namespace Telerik.WebTestRunner.Client.Launcher
{
	public class ProgressMonitor<TLauncherType> : IProgressMonitor<TLauncherType> where TLauncherType : TestLauncher
	{
		private Action<TestLauncher> testLaunchersAction;

		private TesetResultEventHandler tesetResultEventHandler;

		public IProgressPresenter ProgressPresenter
		{
			get;
			set;
		}

		public ProgressMonitor(IProgressPresenter presenter)
		{
			ProgressPresenter = presenter;
		}

		public void Subscribe(TLauncherType testLauncherToSubscribeTo)
		{
			testLaunchersAction = TestLauncher_TestRunStarting;
			testLauncherToSubscribeTo.TestRunStarting += testLaunchersAction;
			tesetResultEventHandler = TestLauncher_TestExecutionFinished;
			testLauncherToSubscribeTo.TestExecuted += tesetResultEventHandler;
		}

		public void Unsubscribe(TLauncherType testLauncherToSubscribeTo)
		{
			testLauncherToSubscribeTo.TestRunStarting -= testLaunchersAction;
			testLauncherToSubscribeTo.TestExecuted -= tesetResultEventHandler;
		}

		private void TestLauncher_TestExecutionFinished(object sender, TestResultEventArgs e)
		{
			ProgressPresenter.ReportProgress(1);
		}

		private void TestLauncher_TestRunStarting(TestLauncher testLauncher)
		{
			ProgressPresenter.Start(testLauncher.TestsToExecute.Count);
		}
	}
}
