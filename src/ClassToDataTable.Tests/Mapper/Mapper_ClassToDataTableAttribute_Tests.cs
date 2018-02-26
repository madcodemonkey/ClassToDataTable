using System;
using System.Collections.Generic;
using System.Data;
using ClassToDataTable.Mapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassToDataTable.Tests
{
    [TestClass]
    public class Mapper_ClassToDataTableAttribute_Tests
    {
        [TestMethod]
        public void PropertyTest_CanMapPrimitiveTypesToDataTable_DataTableHasNonNullableVersionOfPrimitiveTypes()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyMapPrimitiveTest>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

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
        [ExpectedException(typeof(ArgumentException))]
        public void PropertyTest_ArraysMustBeMarkedWithIgnore_ResultsInAnException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyMapArrayTest>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

            // Assert
            Assert.Fail($"You must mark properites with array types with the {nameof(ClassToDataTableAttribute)} Ignore property or you get an exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PropertyTest_ClassesMustBeMarkedWithIgnore_ResultsInAnException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyMapClassTest>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

            // Assert
            Assert.Fail($"You must mark properties with class types with the {nameof(ClassToDataTableAttribute)} Ignore property or you get an exception.");
        }


        [TestMethod]
        public void PropertyTest_CanIgnoreClassProperty_DataTableDoesNotContainTheIgnoredField()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<PropertyMapIgnoreTest>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

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
    }

    internal class PropertyMapClassTest
    {
        public int SomeIntProperty { get; set; }
        public PropertyMapPrimitiveTest SomeClassProperty { get; set; }
    }
}
