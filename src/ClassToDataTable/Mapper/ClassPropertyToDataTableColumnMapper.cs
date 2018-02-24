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
            var mapList = CreateColumnMaps();
            CreateDataTableColumns(mapList, table);
            return mapList;
        }

        private ValidDataTableDataTypes _validDataTableDataTypes = new ValidDataTableDataTypes();

        /// <summary>Adds one converter to the column map.</summary>
        /// <param name="newMap">The column map</param>
        /// <param name="oneCustomAttribute">The attribute that specified the type converter.  We need to use it to initialize the converter.</param>
        private void AddOneConverterToTheMap(ClassPropertyToDataTableColumnMap newMap, ClassToDataTableConverterAttribute oneCustomAttribute)
        {
            newMap.Converter = oneCustomAttribute.TypeConverter.HelpCreateAndCastToInterface<IClassToDataTableTypeConverter>(            
                $"The '{newMap.PropInformation.Name}' property specified a converter, but there is a problem!");
                        
            newMap.OutputType = newMap.Converter.OutputType;

            if (_validDataTableDataTypes.IsValidType(newMap.OutputType) == false)
            {
                throw new ArgumentException($"The {newMap.PropInformation.Name} property is using a converter with an output type " +
                    $"of  {newMap.OutputType.Name}, which is NOT supported by DataTable.  Please refer to this link for more " +
                    $"information about valid DataTable types: https://msdn.microsoft.com/en-us/library/system.data.datacolumn.datatype(v=vs.110).aspx");
            }

            if (newMap.Converter.CanConvert(newMap.PropInformation.PropertyType) == false)
            {
                throw new ArgumentException($"The {newMap.PropInformation.Name} property has a {nameof(ClassToDataTableConverterAttribute)},  " +
                    $"but it cannot convert a data of the {newMap.PropInformation.PropertyType.Name} type!");
            }

            newMap.Converter.Initialize(oneCustomAttribute);
        }

        /// <summary>Creates the property maps</summary>
        private List<ClassPropertyToDataTableColumnMap> CreateColumnMaps()
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

                bool ignoreColumn = FindClassToDataTableAttributesOnOneProperty(newMap);
                if (ignoreColumn)
                    continue;

                FindConvertersOnOneProperty(newMap);

                // If no converter was specified, ignore properties that the DataTable cannot convert!
                if (newMap.Converter == null && _validDataTableDataTypes.IsValidType(info.PropertyType) == false)
                {
                    throw new ArgumentException($"The {newMap.PropInformation.Name} property has a type of  {info.PropertyType.Name},  " +
                        $"which is NOT supported by DataTable.  Please mark the column as Ignored using the " +
                        $"{nameof(ClassToDataTableAttribute)} attribute.  Please refer to this link for more information about " +
                        "valid DataTable types: https://msdn.microsoft.com/en-us/library/system.data.datacolumn.datatype(v=vs.110).aspx");

                }

                mapList.Add(newMap);
            }

            FindConvertersOnTheClass(mapList);

            // Sort the columns the way the user wants them sorted or by column name
            return mapList.OrderBy(o => o.Order).ThenBy(o => o.ColumnName).ToList();
        }

        /// <summary> Creates columns in a DataTable based on what we've discovered from reading the
        /// properties and any attributes on the properties.</summary>
        private void CreateDataTableColumns(List<ClassPropertyToDataTableColumnMap> mapList, DataTable table)
        {
            int index = 0;
            foreach (ClassPropertyToDataTableColumnMap oneMap in mapList)
            {
                oneMap.ColumnIndex = index++;

                // DataSet does not support System.Nullable<> so convert it to the underlying Type.
                // See https://forums.asp.net/t/1796259.aspx?how+to+solve+this+DataSet+does+not+support+System+Nullable+
                if (oneMap.OutputType == null)
                {
                    // Just use the property's type
                    table.Columns.Add(new DataColumn(oneMap.ColumnName,
                        Nullable.GetUnderlyingType(oneMap.PropInformation.PropertyType) ?? oneMap.PropInformation.PropertyType));
                }
                else
                {
                    // Alternative type specified
                    table.Columns.Add(new DataColumn(oneMap.ColumnName, Nullable.GetUnderlyingType(oneMap.OutputType) ?? oneMap.OutputType));
                }
            }
        }

        /// <summary>Finds all the ClassToDataTableConverterAttribute that are decorating the class.</summary>
        private void FindConvertersOnTheClass(List<ClassPropertyToDataTableColumnMap> mapList)
        {
            Type theClassType = typeof(T);
            // Find pre-converters attributes on the class
            var attributeList = theClassType.HelpFindAllClassAttributes<ClassToDataTableConverterAttribute>();

            foreach (var oneAttribute in attributeList)
            {
                if (oneAttribute.TargetPropertyType == null)
                {
                    throw new ArgumentException($"A {nameof(ClassToDataTableConverterAttribute)} was placed on the {theClassType.Name} " +
                        $"class, but a {nameof(ClassToDataTableConverterAttribute.TargetPropertyType)} was NOT specified.");
                }

                var oneTypeConverter = oneAttribute.TypeConverter.HelpCreateAndCastToInterface<IClassToDataTableTypeConverter>(
                    $"The {typeof(T).Name} class has specified a {nameof(ClassToDataTableConverterAttribute)}, but there is a problem!");

                foreach (var map in mapList)
                {
                    // If it already has a converter OR the target property type doesn't match, skip it.
                    if (map.Converter != null || oneAttribute.TargetPropertyType != map.PropInformation.PropertyType)
                        continue;
             
                    AddOneConverterToTheMap(map, oneAttribute);
                }
            }
        }

        /// <summary>Finds all the ClassToDataTableConverterAttribute that are decorating the properties.</summary>
        private void FindConvertersOnOneProperty(ClassPropertyToDataTableColumnMap newMap)
        {
            var attributeList = newMap.PropInformation.HelpFindAllAttributes<ClassToDataTableConverterAttribute>();
            if (attributeList.Count == 1)
            {
                AddOneConverterToTheMap(newMap, attributeList[0]);
            }
            else if (attributeList.Count > 1)
            {
                throw new ArgumentException($"The {newMap.PropInformation.Name} property specified nore than one {nameof(ClassToDataTableConverterAttribute)} " +
                    $"attribute and is allowed per property on a class.  Please remove one of the attributes!");
            }
        }

        /// <summary>Finds all the ClassToDataTableAttribute that are decorating the properties.</summary>
        private bool FindClassToDataTableAttributesOnOneProperty(ClassPropertyToDataTableColumnMap newMap)
        {
            ClassToDataTableAttribute ctdtAttribute = newMap.PropInformation.HelpFindAttribute<ClassToDataTableAttribute>();

            // Should we add this column?
            if (ctdtAttribute != null)
            {
                if (ctdtAttribute.Ignore)
                    return true;

                // Is there an override for column name?  If not, we assigned the property name above.
                if (string.IsNullOrWhiteSpace(ctdtAttribute.ColumnName) == false)
                    newMap.ColumnName = ctdtAttribute.ColumnName;

                // Did the user specify a column order?
                if (ctdtAttribute.Order > 0)
                    newMap.Order = ctdtAttribute.Order;
            }

            return false;
        } 
    }
}
