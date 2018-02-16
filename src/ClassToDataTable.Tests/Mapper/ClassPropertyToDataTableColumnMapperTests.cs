using System;
using System.Collections.Generic;
using System.Data;
using ClassToDataTable.Mapper;
using ClassToDataTable.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassToDataTable.Tests
{
    [TestClass]
    public class ClassPropertyToDataTableColumnMapperTests
    {
        [TestMethod]
        public void CanMapPrimitiveTypesToDataTable()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyMapPrimitiveTest>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.AreEqual(9, theTable.Columns.Count, "Column count is wrong in the DataTable");
            TestColumn(theTable, "SomeIntProperty", typeof(int));
            TestColumn(theTable, "SomeNullableIntProperty", typeof(int));  // Converted to underlying type!
            TestColumn(theTable, "SomeStringProperty", typeof(string));
            TestColumn(theTable, "SomeDoubleProperty", typeof(double));
            TestColumn(theTable, "SomeNullableDoubleProperty", typeof(double));   // Converted to underlying type!
            TestColumn(theTable, "SomeDecimalProperty", typeof(decimal));
            TestColumn(theTable, "SomeNullableDecimalProperty", typeof(decimal));   // Converted to underlying type!
            TestColumn(theTable, "SomeCharProperty", typeof(char));
            TestColumn(theTable, "SomeNullableCharProperty", typeof(char));   // Converted to underlying type!
        }

        [TestMethod]
        public void WillNotMapArraysOrClasses()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyMapArrayTest>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.AreEqual(1, theTable.Columns.Count, "Column count is wrong in the DataTable");
            TestColumn(theTable, "SomeIntProperty", typeof(int));
        }

        [TestMethod]
        public void CanIgnoreClassProperty()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyMapIgnoreTest>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.AreEqual(1, theTable.Columns.Count, "Column count is wrong in the DataTable");
            TestColumn(theTable, "SomeIntProperty", typeof(int));
            Assert.IsNull(theTable.Columns["SomeIntArrayProperty"], "The SomeIntArrayProperty property should have been ignored!");
            Assert.IsNull(theTable.Columns["SomeClassProperty"], "The SomeClassProperty property should have been ignored!");
        }


        private void TestColumn(DataTable theTable, string columnName, Type columnType)
        {
            DataColumn column = theTable.Columns[columnName];
            Assert.IsNotNull(column, $"The '{columnName}' column was not found!");
            Assert.AreEqual(column.DataType, columnType, $"The '{columnName}' column is NOT a {columnType.Name}. It is a {column.DataType.Name}!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AltTypeRequiresConverter()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyAltTypeTest1>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.Fail("You must specify a converter if you are using AltType!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AltTypeCannotBeAnArray()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyAltTypeTest2>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.Fail("AltType Cannot Be An Array!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AltTypeCannotBeAClass()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyAltTypeTest3>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.Fail("AltType Cannot Be a class!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertersMustImplement_IClassToDataTableConverter()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyAltTypeTest4>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.Fail("Converter must implement IClassToDataTableConverter!");
        }
        
        [TestMethod]
        public void YouCanUseAnAltTypeWithValidConverter()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyAltTypeTest5>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable);

            // Assert
            Assert.AreEqual(2, theTable.Columns.Count, "Column count is wrong in the DataTable");
            TestColumn(theTable, "SomeTestProperty", typeof(double));
            TestColumn(theTable, "SomeIntProperty", typeof(int));
        }
    }

    internal class PropertyMapPrimitiveTest
    {
        public int SomeIntProperty { get; set; }
        public int? SomeNullableIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
        public double SomeDoubleProperty { get; set; }
        public double? SomeNullableDoubleProperty { get; set; }
        public decimal SomeDecimalProperty { get; set; }
        public decimal? SomeNullableDecimalProperty { get; set; }
        public char SomeCharProperty { get; set; }
        public char? SomeNullableCharProperty { get; set; }
    }


    internal class PropertyMapIgnoreTest
    {
        public int SomeIntProperty { get; set; }

        [ClassToDataTable(Ignore =true)]
        public int SomeIntOtherProperty { get; set; }
    }
    internal class PropertyMapArrayTest
    {
        public int SomeIntProperty { get; set; }
        public int[] SomeIntArrayProperty { get; set; }
        public PropertyMapPrimitiveTest SomeClassProperty { get; set; }
    }

    internal class PropertyAltTypeTest1
    {
        [ClassToDataTable(AltType =typeof(string))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PropertyAltTypeTest2
    {
        [ClassToDataTable(AltType = typeof(int[]))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PropertyAltTypeTest3
    {
        [ClassToDataTable(AltType = typeof(PropertyAltTypeTest1))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PropertyAltTypeTest4
    {
        [ClassToDataTable(AltType = typeof(int), TypeConverter=typeof(PropertyAltTypeTest1))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class PropertyAltTypeTest5
    {
        [ClassToDataTable(AltType = typeof(double), TypeConverter = typeof(PercentCtodTypeConverter))]
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }
}
