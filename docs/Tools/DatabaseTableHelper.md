# DatabaseTableHelper

This tool is used to do some basic work against database tables.  Here are the primary methods:
- ConvertFieldsToClass - Produces a C# class based on table field definitions.
- LoadFields - Loads field definitions from the database table.
- LoadTableNames - Loads a list of user tables from the database.
- TruncateTables - Truncates a list of tables.
- TruncateTable - Truncates one table.


Here is an example that uses most of these methods:
```c#
private void DbTablesToFiles(string directoryName)
{            
	string myConnectionString = ConfigurationManager.ConnectionStrings["myDbConnectionString"].ConnectionString;
    if (string.IsNullOrWhiteSpace(myConnectionString))
        throw new ArgumentException("Please put a connection string in the app.config file.");

	using (SqlConnection destinationConnection = new SqlConnection(myConnectionString))
	{
		destinationConnection.Open();
		var newHelper = new DatabaseTableHelper(destinationConnection);
		foreach (DatabaseTable table in newHelper.LoadTableNames())
		{
			if (_DbTablesToFilesCancelToken.IsCancellationRequested)
				break;

			// Load table fields
			var tableNameWithSchema = table.ToString();
			var tableFields = newHelper.LoadFields(tableNameWithSchema);

			// Create class in memory
			string someClass = newHelper.ConvertFieldsToClass(tableFields, table.TableName, "SampleNamespace");

			// Save class to file system
			string fileName = Path.Combine(directoryName, $"{table.TableName}.cs");
			using (FileStream fs = File.Create(fileName))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				sw.WriteLine(someClass);
			}
		}
	}
}
```

Note
- [This code can be found in Github here](https://github.com/madcodemonkey/ClassToDataTable/blob/master/src/ClassToDataTable.AdvExample1/MainWindow.xaml.cs) - If you run this code, don't forget to update the connection string in the app.config file to some database YOU own.