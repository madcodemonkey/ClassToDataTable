using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using ClassToDataTable;
using Microsoft.Data.SqlClient;

namespace SqlBulkCopyExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "Your connection string here";
            string tableSchema = "dbo";
            string tableName = "Person";
            int batchSize = 5000;
            int bulkCopyTimeoutInSeconds = 220;
            var people = Person.CreatePeople((int) (batchSize * 1.5));
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                await SqlCopyWithoutHelperAsync(sqlConnection, tableSchema, tableName, batchSize, bulkCopyTimeoutInSeconds, people);
                // await SqlCopyWithHelperAsync(sqlConnection, tableSchema, tableName, batchSize, bulkCopyTimeoutInSeconds, people);
            }

            stopWatch.Stop();
            Console.WriteLine($"Done in {stopWatch.Elapsed.TotalSeconds} seconds!");
        }

        /// <summary>This examples shows how to use the ClassToDataTable service and SQLBulkCopy together.</summary>
        private static async Task SqlCopyWithoutHelperAsync(SqlConnection sqlConnection, string tableSchema, 
            string tableName, int batchSize, int bulkCopyTimeoutInSeconds, List<Person> people)
        {
            var classToDataTableService = new ClassToDataTableService<Person>();

            var myBulkCopy = new SqlBulkCopy(sqlConnection)
            {
                DestinationTableName = $"{tableSchema}.{tableName}", 
                BatchSize = batchSize, 
                BulkCopyTimeout = bulkCopyTimeoutInSeconds
            };

            // Column mappings for SQLBulkCopy
            foreach (DataColumn column in classToDataTableService.Table.Columns)
            {
                myBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            foreach (var item in people)
            {
                classToDataTableService.AddRow(item);

                if (classToDataTableService.Count % myBulkCopy.BatchSize == 0)
                {
                    // WRITE to SERVER! 
                    await myBulkCopy.WriteToServerAsync(classToDataTableService.Table);
                    classToDataTableService.Clear();
                }
            }

            // Flush any remaining items.
            if (classToDataTableService.Count > 0)
            {
                // WRITE to SERVER!
                await myBulkCopy.WriteToServerAsync(classToDataTableService.Table);
            }
        }

        /// <summary>This examples shows how to use a helper that wraps the ClassToDataTable service and SQLBulkCopy working together (see the Helper folder in this example).</summary>
        private static async Task SqlCopyWithHelperAsync(SqlConnection sqlConnection, string tableSchema,
            string tableName, int batchSize, int bulkCopyTimeoutInSeconds, List<Person> people)
        {
            using(var bulkHelper =  new BulkCopyHelper<Person>())
            {
                bulkHelper.Initialize(sqlConnection, tableSchema, tableName, batchSize, bulkCopyTimeoutInSeconds);

                await bulkHelper.AddRows(people);

                await bulkHelper.Flush();
            }
        }
    }
}
