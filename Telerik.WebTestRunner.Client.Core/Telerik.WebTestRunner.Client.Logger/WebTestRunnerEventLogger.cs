using System;
using System.Diagnostics;

namespace Telerik.WebTestRunner.Client.Logger
{
	public class WebTestRunnerEventLogger
	{
		public static void LogException(Exception exception, string messsage)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			string message = $"Message {messsage}, Detailed Error 'Exception : {exception.Message}, InnerException: {exception.InnerException}, StackTrace: {exception.StackTrace}'";
			EventLog.WriteEntry("Telerik.WebTestRunner.Client", message, EventLogEntryType.Error);
		}
	}
}
