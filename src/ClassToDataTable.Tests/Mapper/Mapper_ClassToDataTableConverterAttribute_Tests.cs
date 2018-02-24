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
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyAltTypeTest2>();

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
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyAltTypeTest3>();

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
            TestColumn(theTable, "SomeTestProperty", typeof(double));
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
            TestColumn(theTable, "SomeTestProperty", typeof(double));
            TestColumn(theTable, "SomeIntProperty", typeof(int));
        }


        private void TestColumn(DataTable theTable, string columnName, Type columnType)
        {
            DataColumn column = theTable.Columns[columnName];
            Assert.IsNotNull(column, $"The '{columnName}' column was not found!");
            Assert.AreEqual(column.DataType, columnType, $"The '{columnName}' column is NOT a {columnType.Name}. It is a {column.DataType.Name}!");
        }

    }

    internal class MapperTestConverter : IClassToDataTableTypeConverter
    {
        public bool CanConvert(Type inputType)
        {
            return typeof(string) == inputType || typeof(int) == inputType || typeof(int[]) == inputType;
        }

        public object Convert(PropertyInfo propInfo, object sourceObject)
        {
            return 0;
        }

        public void Initialize(ClassToDataTableConverterAttribute attribute)
        {
        }
    }

    internal class NotRealConverter
    {

    }

    internal class PropertyAltTypeTest1
    {
        [ClassToDataTableConverter(typeof(MapperTestConverter), typeof(int))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PropertyAltTypeTest2
    {
        [ClassToDataTableConverter(typeof(MapperTestConverter), typeof(int[]))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PropertyAltTypeTest3
    {
        [ClassToDataTableConverter(typeof(MapperTestConverter), typeof(PropertyAltTypeTest1))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PropertyInterfaceTest1
    {
        [ClassToDataTableConverter(typeof(NotRealConverter), typeof(int))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PercentageTest1
    {
        [ClassToDataTableConverter(typeof(PercentCtodTypeConverter), typeof(double))]
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }

    [ClassToDataTableConverter(typeof(PercentCtodTypeConverter), typeof(double))]
    internal class PercentageTest2
    {
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }


    [ClassToDataTableConverter(typeof(PercentCtodTypeConverter), typeof(double), TargetPropertyType =typeof(string))]
    internal class PercentageTest3
    {
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }
}
