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

        /// <summary>Specifies an alternate type.  This should be left as null unless
        /// you hare also specified a converter and plan to convert the data into the 
        /// alternate type.</summary>
        public Type AltType { get; set; }

        /// <summary>An optional converter in case you don't want the property converted as is default type.
        /// Specify the type of a class that implements the IClassToDataTableTypeConverter interface.</summary>
        public Type TypeConverter { get; set; }
    }
}
