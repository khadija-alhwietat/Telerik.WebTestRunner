using System.Collections.Generic;

namespace Telerik.WebTestRunner.Client.Model
{
	public class RunnerFixture
	{
		private List<RunnerTest> tests;

		private string fullName;

		private string assemblyFullName;

		public string FullName
		{
			get
			{
				return fullName;
			}
			set
			{
				fullName = value;
			}
		}

		public List<RunnerTest> Tests
		{
			get
			{
				if (tests == null)
				{
					tests = new List<RunnerTest>();
				}
				return tests;
			}
			set
			{
				tests = value;
			}
		}

		public string AssemblyName
		{
			get
			{
				return assemblyFullName;
			}
			set
			{
				assemblyFullName = value;
			}
		}

		public RunnerFixture(string fullName)
		{
			this.fullName = fullName;
		}
	}
}
