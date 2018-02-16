using System;
using System.Reflection;

namespace ClassToDataTable.TypeConverters
{
    public class PercentCtodTypeConverter : IClassToDataTableTypeConverter
    {
        public object GetValue(PropertyInfo propInformation, object sourceObject)
        {
            object data = propInformation.GetValue(sourceObject);
            if (data == null)
                return null;
            if (data.GetType() != typeof(string))
                throw new ArgumentException($"The {nameof(PercentCtodTypeConverter)} converter can only process strings.  The '{propInformation.Name}' field is is a '{data.GetType().ToString()}'");
            
            string stringData = data as string;
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
    }
}
