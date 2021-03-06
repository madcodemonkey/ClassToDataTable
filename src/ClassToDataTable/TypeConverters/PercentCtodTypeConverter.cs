﻿using System;
using System.Reflection;

namespace ClassToDataTable.TypeConverters
{
    /// <summary>Converts a string with a percentage sign in it into a decimal and then divides it by 100. 
    /// If no percentage symbol is found, it still tries to convert the string into a decimal and divide by 100.
    /// It it cannot parse the string, it throws an exception.  Nulls are ignored.</summary>
    public class PercentCtodTypeConverter : IClassToDataTableTypeConverter
    {
        public Type OutputType => typeof(decimal);

        public bool CanConvert(Type inputType)
        {
            return inputType == typeof(string);
        }

        public object Convert(PropertyInfo propInfo, object sourceObject)
        {
            object data = propInfo.GetValue(sourceObject);
            if (data == null)
                return null;
            
            string stringData = data as string;
            if (stringData == null)
                throw new ArgumentException($"The {nameof(PercentCtodTypeConverter)} converter can only process strings.  " +
                    $"The '{propInfo.Name}' field is is a '{propInfo.PropertyType.Name}'");


            int indexOfPercentSign = stringData.IndexOf("%");
            if (indexOfPercentSign != -1)
            {
                // Remove the percentage sign
                stringData = stringData.Remove(indexOfPercentSign);
            }

            decimal result;
            if (decimal.TryParse(stringData, out result))
                return result/100.0m;

            throw new ArgumentException($"The {nameof(PercentCtodTypeConverter)} converter cannot parse the following string: '{stringData}'");
        }

        public void Initialize(ClassToDataTableConverterAttribute attribute)
        {
            // No data needed from the attribute
        }
    }
}
