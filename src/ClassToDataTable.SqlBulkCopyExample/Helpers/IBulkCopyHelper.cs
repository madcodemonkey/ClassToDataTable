using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassToDataTable;
using Microsoft.Data.SqlClient;

namespace SqlBulkCopyExample
{
    /// <summary>Uses SqlBulkCopy to copy data to a server.</summary>
    public interface IBulkCopyHelper<T> : IDisposable
    {
        /// <summary>The size of the batch you would like to send (set it in the Initialize method)</summary>
        int BatchSize { get; }

        /// <summary>Configuration</summary>
        IClassToDataTableConfiguration Configuration { get; }

        /// <summary>The current number of records queued (not yet sent...waiting for BatchSize to be reached)</summary>
        int QueueCount { get; }

        /// <summary>The total number of records that BulkCopy was able to push to the database without error.</summary>
        int TotalWrittenCount { get; set; }

        /// <summary>Adds one row to the converter.  Once BatchSize is reached, data is written to the server.  If you have
        /// no more data to add, call Flush to push the remaining data to the server.</summary>
        /// <param name="record">A record to send to the server. </param>
        /// <remarks> I used object here rather than T so that I can use the interface when dynamically creating BulkCopyHelper of T via reflection.
        /// After obtaining the dynamically created BulkCopyHelper of T, I can cast it to this interface and use it in code.</remarks>
        Task AddRow(T record);

        /// <summary>Adds a list of rows to the converter.  Once BatchSize is reached, data is written to the server.  If you have
        /// no more data to add, call Flush to push the remaining data to the server.</summary>
        /// <param name="records">A list of records to send to the server. </param>
        /// <remarks> I used List of T here rather than T so that I can use the interface when dynamically creating BulkCopyHelper of T via reflection.
        /// After obtaining the dynamically created BulkCopyHelper of T, I can cast it to this interface and use it in code.</remarks>
        Task AddRows(List<T> records);

        /// <summary>Flushes any remaining records to the server.  Call this once you are done adding rows so that the last records in the queue
        /// can be flushed to the server.  It's OK to call this method if there is nothing in the queue.</summary>
        Task Flush();

        /// <summary>Initialize the SqlBulkCopy object</summary>
        /// <param name="destinationConnection">A connection string that you have created, opened and eventually YOU will dispose of.</param>
        /// <param name="tableSchema">The schema of the target table.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="batchSize">The size of the batch you plan to send.</param>
        /// <param name="bulkCopyTimeoutInSeconds">Number of seconds for the operation to complete before it times out.</param>
        void Initialize(SqlConnection destinationConnection, string tableSchema, string tableName, int batchSize, int bulkCopyTimeoutInSeconds = 60);
    }
}