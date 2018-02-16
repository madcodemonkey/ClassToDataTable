using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using ClassToDataTable.Shared;
using ClassToDataTable.TypeConverters;

namespace ClassToDataTable.Mapper
{
    public class ClassPropertyToDataTableColumnMapper<T>
    {
        public List<ClassPropertyToDataTableColumnMap> Map(DataTable table)
        {
            var mapList = ReadClassProperties();
            CreateDataTableColumns(mapList, table);
            return mapList;
        }

        private void CreateDataTableColumns(List<ClassPropertyToDataTableColumnMap> mapList, DataTable table)
        {
            // Create columns in the table
            int index = 0;
            foreach (ClassPropertyToDataTableColumnMap oneMap in mapList)
            {
                oneMap.ColumnIndex = index++;

                // DataSet does not support System.Nullable<> so convert it to the underlying Type.
                // See https://forums.asp.net/t/1796259.aspx?how+to+solve+this+DataSet+does+not+support+System+Nullable+
                if (oneMap.AltType == null)
                {
                    // Just use the property's type
                    table.Columns.Add(new DataColumn(oneMap.ColumnName,
                        Nullable.GetUnderlyingType(oneMap.PropInformation.PropertyType) ?? oneMap.PropInformation.PropertyType));
                }
                else
                {
                    // Alternative type specified
                    table.Columns.Add(new DataColumn(oneMap.ColumnName, Nullable.GetUnderlyingType(oneMap.AltType) ?? oneMap.AltType));
                }
            }
        }

        private List<ClassPropertyToDataTableColumnMap> ReadClassProperties()
        {
            var mapList = new List<ClassPropertyToDataTableColumnMap>();
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                var newMap = new ClassPropertyToDataTableColumnMap()
                {
                    Order = 99999,
                    ColumnIndex = 0,
                    ColumnName = info.Name, // use property name by default
                    PropInformation = info
                };

                // If the property has a dislay name attribute, use it as the column name.
                ClassToDataTableAttribute ctdtAttribute = info.HelpFindAttribute<ClassToDataTableAttribute>();
                if (ctdtAttribute != null)
                {
                    // Should we add this column?
                    if (ctdtAttribute.Ignore)
                    {
                        continue;
                    }

                    // Is there an override for column name?  If not, we assigned the property name above.
                    if (string.IsNullOrWhiteSpace(ctdtAttribute.ColumnName) == false)
                        newMap.ColumnName = ctdtAttribute.ColumnName;

                    // Did the user specify a column order?
                    if (ctdtAttribute.Order > 0)
                        newMap.Order = ctdtAttribute.Order;

                    // Has converter a class that implments the IClassToDataTableConverter interface?
                    if (ctdtAttribute.TypeConverter != null)
                    {
                        newMap.Converter = ctdtAttribute.TypeConverter.HelpCreateAndCastToInterface<IClassToDataTableTypeConverter>(
                            $"The '{info.Name}' property specified a converter, but there is a problem!");
                    }

                    // Alternate type?
                    if (ctdtAttribute.AltType != null)
                    {
                        // Is this a valid type?
                        if (TypeIsAllowed(ctdtAttribute.AltType) == false)
                        {
                            throw new ArgumentException($"The {info.Name} property specified an AltType ({ctdtAttribute.AltType.Name}) " +
                                  $"using the ClassToDataTable Attribute.  The {ctdtAttribute.AltType.Name} type is NOT allowed since we " +
                                  "do not know how to convert it to a DataTable column!");
                        }

                        if (newMap.Converter == null)
                        {
                            throw new ArgumentException($"The {info.Name} property specified an AltType ({ctdtAttribute.AltType.Name}) " +
                                $"using the ClassToDataTable Attribute; however, no coverter was specified.  You cannot use AltType " +
                                "unless you specify your own coverter using the ClassToDataTable Attribute!");
                        }

                        newMap.AltType = ctdtAttribute.AltType;
                    }
                }

                if (string.IsNullOrWhiteSpace(newMap.ColumnName))
                    newMap.ColumnName = info.Name;

                // If no AltType was specified, ignore properties with invalid types
                if ((ctdtAttribute == null || ctdtAttribute.AltType == null) && TypeIsAllowed(info.PropertyType) == false)
                    continue;

                mapList.Add(newMap);
            }

            // Sort the columns the way the user wants them sorted or by column name
            return mapList.OrderBy(o => o.Order).ThenBy(o => o.ColumnName).ToList();            
        }

        private bool TypeIsAllowed(Type theType)
        {
            // Ignore properties that are classes and arrays            
            // Note: IsClass is true for string and array properties as well 
            if ((theType != typeof(string) && theType.IsClass))
                return false;

            return true;
        }
    }
}
