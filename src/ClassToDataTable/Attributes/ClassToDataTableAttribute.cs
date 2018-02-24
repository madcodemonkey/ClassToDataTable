using System;

namespace ClassToDataTable
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ClassToDataTableAttribute : Attribute
    {      
        /// <summary>This is an optional column name in case you don't want the name of the property used.</summary>
        public string ColumnName { get; set; }

        /// <summary>Ignore this property.</summary>
        public bool Ignore { get; set; } = false;

        /// <summary>The order that the column should be written to the data table</summary>
        public int Order { get; set; } = 999999;  
    }
}
