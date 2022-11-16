using Microsoft.Http;
using System;
using System.Net;
using System.Threading;

namespace Telerik.WebTestRunner.Client.Core.Utilities
{
	public class SitefinityHelper
	{
		public static void WaitForSitefinityToStart(string testingInstanceUrl, double totalWaitSeconds)
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = default(TimeSpan);
			HttpResponseMessage httpResponseMessage;
			do
			{
				httpResponseMessage = new HttpClient().Get(testingInstanceUrl + "/appstatus");
				Thread.Sleep(1000);
				timeSpan = DateTime.UtcNow.Subtract(utcNow);
			}
			while (httpResponseMessage.StatusCode == HttpStatusCode.OK && timeSpan.TotalSeconds < totalWaitSeconds);
			if (timeSpan.TotalSeconds > totalWaitSeconds)
			{
				throw new Exception("Sitefinity did not start in less than " + totalWaitSeconds / 60.0 + " minultes.");
			}
		}
	}
}
