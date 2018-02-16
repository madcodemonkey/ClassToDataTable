using System.Reflection;

namespace ClassToDataTable.TypeConverters
{
    public interface IClassToDataTableTypeConverter
    {
        object GetValue(PropertyInfo propInformation, object sourceObject);
    }
}
