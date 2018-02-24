using System;
using System.Reflection;

namespace ClassToDataTable.TypeConverters
{
    public interface IClassToDataTableTypeConverter
    {
        /// <summary>The type of data that the converter outputs.  It should be a valid data type accepted
        /// by the DataTable (see https://msdn.microsoft.com/en-us/library/system.data.datacolumn.datatype(v=vs.110).aspx for more information).</summary>
        Type OutputType { get; }

        /// <summary>This method is called to make sure that the converter can process the type.</summary>
        /// <param name="theType">The class property type of the source.</param>
        bool CanConvert(Type inputType);

        /// <summary>Passess in property and the instantiated object so that the user can pull the data from the
        /// property and convert it to whatever they choose.</summary>
        /// <param name="propInfo">Property Information</param>
        /// <param name="sourceObject">THe instantiated object</param>
        /// <returns></returns>
        object Convert(PropertyInfo propInfo, object sourceObject);

        /// <summary>Used to pass the attribute to the converter in case it needs any optional inputs.</summary>
        void Initialize(ClassToDataTableConverterAttribute attribute);
    }
}
