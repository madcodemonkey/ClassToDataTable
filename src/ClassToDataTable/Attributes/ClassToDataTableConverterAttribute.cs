using System;

namespace ClassToDataTable
{
    /// <summary>Allow multiple because of class level marker otherwise you can't target different types.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class ClassToDataTableConverterAttribute : Attribute
    {
        public ClassToDataTableConverterAttribute(Type typeConverter, Type outputType)
        {
            TypeConverter = typeConverter;
            OutputType = outputType;
        }
        
        /// <summary>The type that should be created in the DataTable.  This is also the type that the 
        /// converter should OUTPUT.</summary>
        public Type OutputType { get; set; }

        /// <summary>An optional converter in case you don't want the property converted as is default type.
        /// Specify the type of a class that implements the IClassToDataTableTypeConverter interface.</summary>
        public Type TypeConverter { get; set; }

        /// <summary>When this attribute is used to decorate a class, you can target a particular target type. 
        /// It is NOT used when decorating a property!  Also, when used on the class level it will NOT override
        /// converters on the actual property.</summary>
        public Type TargetPropertyType { get; set; }
    }
}
