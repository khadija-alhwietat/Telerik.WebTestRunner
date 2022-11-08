using System;

namespace Telerik.WebTestRunner.Cmd
{
	[Flags]
	public enum ExitCode
	{
		Success = 0x0,
		Partially = 0x1,
		UnrecognizedResult = 0x2,
		NothingSuccess = 0xA
	}
}
