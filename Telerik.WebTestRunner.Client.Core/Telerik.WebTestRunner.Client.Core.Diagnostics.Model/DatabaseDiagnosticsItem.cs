using System.Collections.Generic;

namespace Telerik.WebTestRunner.Client.Core.Diagnostics.Model
{
	internal class DatabaseDiagnosticsItem
	{
		private Dictionary<string, int> tablesDictionary = new Dictionary<string, int>();

		public Dictionary<string, int> DatabaseTables
		{
			get
			{
				return tablesDictionary;
			}
			set
			{
				tablesDictionary = value;
			}
		}

		public string TestName
		{
			get;
			set;
		}
	}
}
