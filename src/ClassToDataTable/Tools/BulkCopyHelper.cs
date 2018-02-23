using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ClassToDataTable.Tools
{
    public class BulkCopyHelper<T> : IBulkCopyHelper
    {
        private ClassToDataTableService<T> _converter = new ClassToDataTableService<T>();
        private SqlBulkCopy _bulkCopy;
        private bool _initialized = false;
        public void Dispose()
        {
            if (_bulkCopy != null)
            {
                _bulkCopy.Close();
                _bulkCopy = null;
                _initialized = false;
            }
        }

        /// <summary>The total number of records that BulkCopy was able to push to the database without error.</summary>
        public int TotalWrittenCount { get; set; }

        /// <summary>The size of the batch you would like to send (set it in the Initialize method)</summary>
        public int BatchSize { get; private set; }

        /// <summary>The current number of records queued (not yet sent...waiting for BatchSize to be reached)</summary>
        public int QueueCount { get { return _converter.Count; } }

        /// <summary>Initialize the SqlBulkCopy object</summary>
        /// <param name="destinationConnection">A connection string that you have created, opened and eventually YOU will dispose of.</param>
        /// <param name="tableSchema">The schema of the target table.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="batchSize">The size of the batch you plan to send.</param>
        public void Initialize(SqlConnection destinationConnection, string tableSchema, string tableName, int batchSize, int bulkCopyTimeoutInSeconds = 60)
        {
            _bulkCopy = new SqlBulkCopy(destinationConnection);
            _bulkCopy.DestinationTableName = $"[{tableSchema}].[{tableName}]";
            _bulkCopy.BatchSize = batchSize;
            _bulkCopy.BulkCopyTimeout = bulkCopyTimeoutInSeconds; 
            foreach (DataColumn column in _converter.Table.Columns)
            {
                _bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            TotalWrittenCount = 0;
            BatchSize = batchSize;
            _initialized = true;
        }

        /// <summary>Adds one row to the converter.  Once BatchSize is reached, data is written to the server.  If you have
        /// no more data to add, call Flush to push the remaining data to the server.</summary>
        /// <param name="record">A record to send to the server. </param>
        /// <remarks> I used object here rather than T so that I can use the interface when dynamically creating BulkCopyHelper.BulkCopyHelper of T via 
        /// reflection (see CreateBasedOnType below).</remarks>
        public async Task AddRow(object record)
        {
            if (record == null)
                throw new ArgumentNullException("Please do not pass null records to the AddRow method!");
            if (_initialized == false)
                throw new ArgumentException("Please call initialize before calling AddRow.");

            _converter.AddRow((T) record);
            if (_converter.Count % BatchSize == 0)
                  await Flush();
        }

        /// <summary>Adds a list of rows to the converter.  Once BatchSize is reached, data is written to the server.  If you have
        /// no more data to add, call Flush to push the remaining data to the server.</summary>
        /// <param name="records">A list of records to send to the server. </param>
        /// <remarks> I used List<T> here rather than T so that I can use the interface when dynamically creating BulkCopyHelper.BulkCopyHelper of T via 
        /// reflection (see CreateBasedOnType below).</remarks>
        public async Task AddRows(List<object> records)
        {
            if (records == null)
                throw new ArgumentNullException("Please do not pass a null list to the AddRows method!");
            if (_initialized == false)
                throw new ArgumentException("Please call initialize before calling AddRow.");

            foreach(var record in records)
            {
                await AddRow(record);
            }            
        }

        /// <summary>Flushes any remaining records to the server.  Call this once you are done adding rows so that the last records in the queue
        /// can be flushed to the server.  It's OK to call this method if there is nothing in the queue.</summary>
        public async Task Flush()
        {
            if (_initialized == false)
                throw new ArgumentException("Please call initialize before calling flush.");

            // Is there anything to save?
            if (_converter.Count == 0)
                return;

            // WRITE to SERVER!
            await _bulkCopy.WriteToServerAsync(_converter.Table);

            TotalWrittenCount += _converter.Count;
            _converter.Clear();            
        }
    }

    public class BulkCopyHelper
    {
        public static IBulkCopyHelper CreateBaseOnStrings(string theNamespace, string theClassName, string theAssemblyName)
        {
            // Figure out the type
            string nameOfClass = $"{theNamespace}.{theClassName},{theAssemblyName}";
            var theType = Type.GetType(nameOfClass);

            // Now use the other method 
            return CreateBasedOnType(theType);
        }

        public static IBulkCopyHelper CreateBasedOnType(Type theType)
        {
            Type generic = typeof(BulkCopyHelper<>);
            Type constructed = generic.MakeGenericType(theType);

            return Activator.CreateInstance(constructed) as IBulkCopyHelper;
        }
    }
}
