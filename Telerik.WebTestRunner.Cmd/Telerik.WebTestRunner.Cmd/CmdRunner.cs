#define TRACE
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Telerik.WebTestRunner.Client.Configuration;
using Telerik.WebTestRunner.Cmd.Commands;

namespace Telerik.WebTestRunner.Cmd
{
	public class CmdRunner
	{
		private static RunCommand command;
        public ExitCode exitCode;

        public static void Main(string[] args)
		{
			var test = RunnerConfig.GetSingleTestExecutionTimeout();
            ServicePointManager.ServerCertificateValidationCallback = ((object _003Cp0_003E, X509Certificate _003Cp1_003E, X509Chain _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true);
			ExitCode exitCode;
			try
			{
				exitCode = ExecuteCommands(args);
			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
				Trace.Flush();
			//	Console.ReadLine();
				throw ex;
			}
			//Console.WriteLine(exitCode.ToString());
			//Console.ReadLine();
			Environment.Exit((int)exitCode);
		}

		private static ExitCode ExecuteCommands(string[] args)
		{
			if (args == null || args.Length < 1)
			{
				throw new ArgumentOutOfRangeException("Invalid arguments.");
			}
			if (string.Compare(args[0].Trim(), "Run", ignoreCase: true) == 0)
			{
				command = new RunCommand(args);
				new Timer(TimerCallback, null, 0, 90000);
				command.Execute();
				return GetResult();
			}
			throw new Exception("The command could not be processed. Please make sure that supplied parameters are valid");
		}

		private static ExitCode GetResult()
		{
			ExitCode result = ExitCode.UnrecognizedResult;
			ResultStatistics resultStatistics = command.ResultStatistics;
			if (resultStatistics != null)
			{
				Console.WriteLine($"Total Tests:{resultStatistics.TotalTests}, Passed Tests:{resultStatistics.PassedTests}, Failed Tests:{resultStatistics.FailedTests}, Ignored Tests:{resultStatistics.IgnoredTests}");
				if (resultStatistics.FailedTests > 0 && resultStatistics.PassedTests > 0)
				{
					result = ExitCode.Partially;
				}
				else if (resultStatistics.FailedTests > 0 && resultStatistics.PassedTests == 0)
				{
					result = ExitCode.NothingSuccess;
				}
				else if (resultStatistics.PassedTests > 0 && resultStatistics.FailedTests == 0)
				{
					result = ExitCode.Success;
				}
			}
			return result;
		}

		private static void TimerCallback(object o)
		{
			Console.WriteLine("*************Running tests in progress*************");
			GC.Collect();
		}
	}
}
