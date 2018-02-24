using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using ClassToDataTable.Mapper;
using ClassToDataTable.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassToDataTable.Tests
{
    [TestClass]
    public class Mapper_ClassToDataTableConverterAttribute_Tests
    { 
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OutputType_CannotBeAnArray_ResultsInAnException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyOutputTest1>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.Fail("OutputType Cannot Be An Array!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OutputTypeTest_CannotBeAClass_ResultsInAnException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyOutputTest2>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.Fail("OutputType Cannot Be a class!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterfaceTest_ConvertersMustImplement_IClassToDataTableConverter_ResultsInAnException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyInterfaceTest1>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.Fail("Converter must implement IClassToDataTableConverter!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassAttributeTest_NotSpecifyingTargetPropertyType_ResultsInAnException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PercentageTest2>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.Fail("You should get an error for not specifying a TargetPropertyType!");
        }


        [TestMethod]
        public void ConverterOnProperty_ValidConverterWorks_NoExceptions()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PercentageTest1>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.AreEqual(2, theTable.Columns.Count, "Column count is wrong in the DataTable");
            TestColumn(theTable, "SomeTestProperty", typeof(decimal));
            TestColumn(theTable, "SomeIntProperty", typeof(int));
        }

        [TestMethod]
        public void ConverterOnClass_ValidConverterWorks_NoExceptions()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PercentageTest3>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.AreEqual(2, theTable.Columns.Count, "Column count is wrong in the DataTable");
            TestColumn(theTable, "SomeTestProperty", typeof(decimal));
            TestColumn(theTable, "SomeIntProperty", typeof(int));
        }


        private void TestColumn(DataTable theTable, string columnName, Type columnType)
        {
            DataColumn column = theTable.Columns[columnName];
            Assert.IsNotNull(column, $"The '{columnName}' column was not found!");
            Assert.AreEqual(column.DataType, columnType, $"The '{columnName}' column is NOT a {columnType.Name}. It is a {column.DataType.Name}!");
        }

    }

    internal class MapperTestConverterWithArrayOutput : IClassToDataTableTypeConverter
    {
        public Type OutputType => typeof(int[]);

        public bool CanConvert(Type inputType)
        {
            return typeof(string) == inputType;
        }

        public object Convert(PropertyInfo propInfo, object sourceObject)
        {
            return new int[] { 0, 1, 2 };
        }

        public void Initialize(ClassToDataTableConverterAttribute attribute)
        {
        }
    }

    internal class MapperTestConverterWithClassOutput : IClassToDataTableTypeConverter
    {
        public Type OutputType => typeof(NotRealConverter);

        public bool CanConvert(Type inputType)
        {
            return typeof(string) == inputType;
        }

        public object Convert(PropertyInfo propInfo, object sourceObject)
        {
            return new NotRealConverter();
        }

        public void Initialize(ClassToDataTableConverterAttribute attribute)
        {
        }
    }

    internal class NotRealConverter
    {

    }

  

    internal class PropertyOutputTest1
    {
        public int SomeIntProperty { get; set; }

        [ClassToDataTableConverter(typeof(MapperTestConverterWithArrayOutput))]
        public string SomeStringProperty { get; set; }
    }

    internal class PropertyOutputTest2
    {
        [ClassToDataTableConverter(typeof(MapperTestConverterWithClassOutput))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PropertyInterfaceTest1
    {
        [ClassToDataTableConverter(typeof(NotRealConverter))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PercentageTest1
    {
        [ClassToDataTableConverter(typeof(PercentCtodTypeConverter))]
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }

    [ClassToDataTableConverter(typeof(PercentCtodTypeConverter))]
    internal class PercentageTest2
    {
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }


    [ClassToDataTableConverter(typeof(PercentCtodTypeConverter), TargetPropertyType =typeof(string))]
    internal class PercentageTest3
    {
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }
}
