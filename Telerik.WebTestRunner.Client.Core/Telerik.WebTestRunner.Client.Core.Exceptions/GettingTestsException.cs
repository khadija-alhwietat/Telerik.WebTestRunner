using System;

namespace Telerik.WebTestRunner.Client.Core.Exceptions
{
	public class GettingTestsException : Exception
	{
		public GettingTestsException(string message)
			: base(message)
		{
		}

		public GettingTestsException(string message, Exception ex)
			: base(message, ex)
		{
		}

		public GettingTestsException()
		{
		}
	}
}
