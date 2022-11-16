namespace Telerik.WebTestRunner.Client.Launcher
{
	public interface IProgressMonitor<TLauncherType> where TLauncherType : TestLauncher
	{
		IProgressPresenter ProgressPresenter
		{
			get;
			set;
		}

		void Subscribe(TLauncherType testLauncherToSubscribeTo);

		void Unsubscribe(TLauncherType testLauncherToSubscribeTo);
	}
}
