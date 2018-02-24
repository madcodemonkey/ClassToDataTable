using System;
using System.Collections.Generic;
using System.Data;
using ClassToDataTable.Mapper;

namespace ClassToDataTable
{
    public class ClassToDataTableService<T>
    {
        private List<ClassPropertyToDataTableColumnMap> _propertyMapList;
        public ClassToDataTableService()
        {
            _propertyMapList = new ClassPropertyToDataTableColumnMapper<T>().Map(Table);
        }
        public DataTable Table { get; set; } = new DataTable();
 
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

        public void AddRows(IEnumerable<T> list)
        {
            foreach (T source in list)
            {
               AddRow(source);
            }            
        }

        public int Count
        {
            get { return Table.Rows.Count; }
        }

        /// <summary>Clears data (leaves column defs alone)</summary>
        public void Clear()
        {
            Table.Clear();            
        }  
    }
}
