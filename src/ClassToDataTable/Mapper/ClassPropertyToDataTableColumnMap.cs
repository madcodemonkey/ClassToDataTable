using System;
using System.Reflection;
using ClassToDataTable.TypeConverters;

namespace ClassToDataTable.Mapper
{
    /// <summary>Used to determine how a class property should map to a DataTable column.</summary>
    public class ClassPropertyToDataTableColumnMap
    {
        /// <summary>The actual index of the column in the DataTable.</summary>
        public int ColumnIndex { get; set; }

        /// <summary>The name of the column in the DataTable.  It an either be determined by what the user 
        /// specified with the ClassToDataTableAttribute or if its not specified it will default to the
        /// name of the class property</summary>
        public string ColumnName { get; set; }

        /// <summary>PropertyInfo information about the class property.</summary>
        public PropertyInfo PropInformation { get; set; }
        
        /// <summary>The sort order that the user specified for the column on the class property via the ClassToDataTableAttribute.</summary>
        public int Order { get; set; }

        /// <summary>An alternate Type to use for this column when determining the DataTable column.</summary>
        public Type AltType { get; set; }

        /// <summary>An optional converter in case you don't want the default converter based on the property's type.</summary>
        public IClassToDataTableTypeConverter Converter { get; set; }
    }
}
