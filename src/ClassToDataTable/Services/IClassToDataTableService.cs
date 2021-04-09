using System.Collections.Generic;
using System.Data;

namespace ClassToDataTable
{
    /// <summary>Converts a type of T to to rows in a DataTable.</summary>
    public interface IClassToDataTableService<T>
    {
        /// <summary>Configuration settings.</summary>
        ClassToDataTableConfiguration Configuration { get; }

        /// <summary>The number of rows in the <see cref="Table"/> property. </summary>
        int Count { get; }

        /// <summary>The DataTable that you are building by using the AddRow or AddRows methods.</summary>
        DataTable Table { get; set; }

        /// <summary>Add one row to the DataTable.</summary>
        void AddRow(T source);

        /// <summary>Add many rows to the DataTable.</summary>
        /// <param name="list">Rows to add</param>
        void AddRows(IEnumerable<T> list);

        /// <summary>Clears data in the DataTable (leaves column defs alone)</summary>
        void Clear();
    }
}