using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Telerik.WebTestRunner.Client.Core.Diagnostics.Model;
using Telerik.WebTestRunner.Client.Model;
using Telerik.WebTestRunner.Model;

namespace Telerik.WebTestRunner.Client.Core.Diagnostics
{
	internal class DatabaseTableRowsProfiler
	{
		private DatabaseDiagnosticsItem previousDiagnosticItem;

		private string connectionString;

		public DatabaseTableRowsProfiler(string server, string database)
		{
			connectionString = $"data source={server};Integrated Security=SSPI;initial catalog={database}";
		}

		public void LogTestDatabaseData(RunnerTestResult testResult)
		{
			if (testResult == null)
			{
				throw new ArgumentNullException("testResult");
			}
			if (testResult.Result == WcfTestResultEnum.Ignored || testResult.Result == WcfTestResultEnum.NotRun)
			{
				return;
			}
			List<DatabaseTable> dataTables = BuildDatabaseTableItem();
			DatabaseDiagnosticsItem databaseDiagnosticItemForTest = GetDatabaseDiagnosticItemForTest(testResult.TestMethodName, dataTables);
			if (previousDiagnosticItem == null)
			{
				previousDiagnosticItem = databaseDiagnosticItemForTest;
				return;
			}
			int value = 0;
			List<Tuple<string, int>> list = new List<Tuple<string, int>>();
			List<string> list2 = new List<string>();
			foreach (KeyValuePair<string, int> databaseTable in databaseDiagnosticItemForTest.DatabaseTables)
			{
				if (previousDiagnosticItem.DatabaseTables.TryGetValue(databaseTable.Key, out value))
				{
					if (value != databaseTable.Value)
					{
						list.Add(new Tuple<string, int>(databaseTable.Key, databaseTable.Value - value));
					}
				}
				else
				{
					list2.Add(databaseTable.Key);
				}
			}
			if (list.Any() || list2.Any())
			{
				DatabaseDiagnosticsXmlLogger.Instance.Log(testResult, list, list2);
			}
			previousDiagnosticItem = databaseDiagnosticItemForTest;
		}

		private DatabaseDiagnosticsItem GetDatabaseDiagnosticItemForTest(string test, List<DatabaseTable> dataTables)
		{
			DatabaseDiagnosticsItem databaseDiagnosticsItem = new DatabaseDiagnosticsItem();
			databaseDiagnosticsItem.TestName = test;
			foreach (DatabaseTable dataTable in dataTables)
			{
				if (!databaseDiagnosticsItem.DatabaseTables.ContainsKey(dataTable.TableName))
				{
					databaseDiagnosticsItem.DatabaseTables.Add(dataTable.TableName, dataTable.TableRows);
				}
			}
			return databaseDiagnosticsItem;
		}

		private List<DatabaseTable> BuildDatabaseTableItem()
		{
			List<DatabaseTable> list = new List<DatabaseTable>();
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				SqlCommand sqlCommand = new SqlCommand("SELECT ta.name TableName,SUM(pa.rows) RowCnt FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.OBJECT_ID = ta.OBJECT_ID INNER JOIN sys.schemas sc ON ta.schema_id = sc.schema_id WHERE ta.is_ms_shipped = 0 AND pa.index_id IN (1,0) GROUP BY sc.name,ta.name ORDER BY SUM(pa.rows) DESC", sqlConnection);
				sqlConnection.Open();
				SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
				while (sqlDataReader.HasRows)
				{
					while (sqlDataReader.Read())
					{
						list.Add(new DatabaseTable
						{
							TableName = sqlDataReader[0].ToString(),
							TableRows = Convert.ToInt32(sqlDataReader[1])
						});
					}
					sqlDataReader.NextResult();
				}
				return list;
			}
		}
	}
}
