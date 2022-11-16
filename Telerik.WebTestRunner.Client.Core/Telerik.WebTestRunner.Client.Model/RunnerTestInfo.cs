using System;
using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Model
{
	public class RunnerTestInfo
	{
		private string fixtureName;

		private string assemblyName;

		private string testMethodName;

		private bool isToIgnore;

		private string authorName;

		public string FixtureName
		{
			get
			{
				return fixtureName;
			}
			set
			{
				fixtureName = value;
			}
		}

		public string TestMethodName
		{
			get
			{
				return testMethodName;
			}
			set
			{
				testMethodName = value;
			}
		}

		public string AssemblyName
		{
			get
			{
				return assemblyName;
			}
			set
			{
				assemblyName = value;
			}
		}

		public string AuthorName
		{
			get
			{
				return authorName;
			}
			set
			{
				authorName = value;
			}
		}

		public bool IsToIgnore
		{
			get
			{
				return isToIgnore;
			}
			set
			{
				isToIgnore = value;
			}
		}

		public bool IsMultilingual
		{
			get;
			set;
		}

		public string ExecutionModeString
		{
			get;
			set;
		}

		public MultilingualExecutionMode ExecutionMode => (MultilingualExecutionMode)Enum.Parse(typeof(MultilingualExecutionMode), ExecutionModeString);

		public WcfAuthenticationMode AuthenticationMode
		{
			get;
			set;
		}

		public RunnerCredentials Credentials
		{
			get;
			set;
		}

		public RunnerTestInfo(RunnerFixture fixture, string testMethodName)
		{
			Initialize(fixture, testMethodName);
		}

		protected RunnerTestInfo()
		{
		}

		protected void Initialize(RunnerFixture fixture, string testMethodName)
		{
			if (fixture == null)
			{
				throw new ArgumentNullException("fixture");
			}
			if (string.IsNullOrEmpty(testMethodName))
			{
				throw new ArgumentNullException("testMethodName");
			}
			assemblyName = fixture.AssemblyName;
			fixtureName = fixture.FullName;
			this.testMethodName = testMethodName;
		}
	}
}
