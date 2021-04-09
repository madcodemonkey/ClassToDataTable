using System;
using System.Collections.Generic;
using System.Data;
using ClassToDataTable.Mapper;

namespace ClassToDataTable
{
    /// <summary>Converts a type of T to to rows in a DataTable.</summary>
    public class ClassToDataTableService<T> : IClassToDataTableService<T>
    {
        private readonly List<ClassPropertyToDataTableColumnMap> _propertyMapList;
        public ClassToDataTableService()
        {
            _propertyMapList = new ClassPropertyToDataTableColumnMapper<T>().Map(Table, Configuration);
        }

        /// <summary>Configuration settings.</summary>
        public ClassToDataTableConfiguration Configuration { get; private set; } = new ClassToDataTableConfiguration();
        
        /// <summary>The DataTable that you are building by using the AddRow or AddRows methods.</summary>
        public DataTable Table { get; set; } = new DataTable();
 
        /// <summary>Add one row to the DataTable.</summary>
        public void AddRow(T source)
        {
            if (source == null)
                throw new ArgumentNullException("Please pass in data and not nulls!");

            object[] values = new object[_propertyMapList.Count];
            foreach (var column in _propertyMapList)
            {
                object someValue = (column.Converter == null) ? column.PropInformation.GetValue(source) :
                    column.Converter.Convert(column.PropInformation, source);

                // DataSet does not support System.Nullable<> so we use DBNull.Value to specify that we have a null.
                // See https://forums.asp.net/t/1796259.aspx?how+to+solve+this+DataSet+does+not+support+System+Nullable+
                values[column.ColumnIndex] = someValue ?? DBNull.Value;
            }

            Table.Rows.Add(values);
        }

        /// <summary>Add many rows to the DataTable.</summary>
        /// <param name="list">Rows to add</param>
        public void AddRows(IEnumerable<T> list)
        {
            foreach (T source in list)
            {
               AddRow(source);
            }            
        }

        public int Count => Table.Rows.Count;

        /// <summary>Clears data in the DataTable (leaves column defs alone)</summary>
        public void Clear()
        {
            Table.Clear();            
        }  
    }
}
