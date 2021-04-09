using System;
using System.Reflection;

namespace ClassToDataTable.TypeConverters
{
    /// <summary>Runs the value encountered into the System.Convert.ChangeType method where the type is the TargetPropertyType
    /// from the attribute. If null is found, null is returned.</summary>
    public class ChangeTypeCtodTypeConverter : IClassToDataTableTypeConverter
    {
        private Type ConversionType { get; set; }

        /// <summary>Output type</summary>
        public Type OutputType { get; set; }

        /// <summary>Can Convert type</summary>
        /// <param name="inputType">Type to convert</param>
        /// <returns></returns>
        public bool CanConvert(Type inputType)
        {
            return true;
        }

        /// <summary>Converts the type.</summary>
        /// <param name="propInfo">Property info</param>
        /// <param name="sourceObject">Source object</param>
        public object Convert(PropertyInfo propInfo, object sourceObject)
        {
            object data = propInfo.GetValue(sourceObject);
            if (data == null)
            {
                return null;
            }

            return System.Convert.ChangeType(data, this.ConversionType);
        }

        /// <summary>Initialize</summary>
        public void Initialize(ClassToDataTableConverterAttribute attribute)
        {
            // No data needed from the attribute
            if (attribute.TargetPropertyType == null)
            {
                throw new ArgumentException($"The {nameof(ChangeTypeCtodTypeConverter)} converter will not work unless you specify a TargetPropertyType");
            }

            OutputType = attribute.TargetPropertyType;
            ConversionType = Nullable.GetUnderlyingType(attribute.TargetPropertyType) ?? attribute.TargetPropertyType;
        }
    }
}
