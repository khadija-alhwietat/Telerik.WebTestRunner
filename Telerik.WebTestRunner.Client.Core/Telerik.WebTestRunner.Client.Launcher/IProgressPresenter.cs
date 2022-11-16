namespace Telerik.WebTestRunner.Client.Launcher
{
	public interface IProgressPresenter
	{
		void Start(int totalAmountOfWork);

		void ReportProgress(int amountOfWorkCompleted);

		void Finish();
	}
}
