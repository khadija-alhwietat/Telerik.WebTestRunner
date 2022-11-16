using System;
using System.Collections.Generic;
using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Model
{
	public class RunnerTest : RunnerTestInfo
	{
		private WcfTestResultEnum executionResult;

		private Dictionary<string, string> parameters;

		public OperationState State
		{
			get;
			set;
		}

		public string IdentificationKey
		{
			get;
			set;
		}

		public RunnerFixture Fixture
		{
			get;
			set;
		}

		public WcfTestResultEnum ExecutionResult
		{
			get
			{
				if (base.IsToIgnore && executionResult == WcfTestResultEnum.NotRun)
				{
					executionResult = WcfTestResultEnum.Ignored;
				}
				return executionResult;
			}
			set
			{
				executionResult = value;
			}
		}

		public string ExecutionMessage
		{
			get;
			set;
		}

		public Dictionary<string, string> Parameters
		{
			get
			{
				if (parameters == null)
				{
					parameters = new Dictionary<string, string>();
				}
				return parameters;
			}
			set
			{
				parameters = value;
			}
		}

		public bool Diagnose
		{
			get;
			set;
		}

		public RunnerTest(RunnerFixture fixture, string testMethodName)
		{
			if (fixture == null)
			{
				throw new ArgumentNullException("fixture");
			}
			Initialize(fixture, testMethodName);
			Fixture = fixture;
			Fixture.Tests.Add(this);
			IdentificationKey = $"{fixture.FullName}.{base.TestMethodName}";
		}

		public RunnerTest()
		{
		}

		public void UpdateInfoFrom(RunnerTestResult testResult)
		{
			ExecutionResult = testResult.Result;
			ExecutionMessage = testResult.Message;
		}
	}
}
