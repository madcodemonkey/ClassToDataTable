using System;
using System.Reflection;

namespace ClassToDataTable.TypeConverters
{
    public interface IClassToDataTableTypeConverter
    {
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
