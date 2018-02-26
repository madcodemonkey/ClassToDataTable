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
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<ClassPropertyToDataTableColumnMapperTestData1>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

            // Assert
            Assert.Fail("OutputType Cannot Be An Array!");
        }

        [TestMethod]        
        public void Map_ClassCanHaveArrayPropertyTypeWithoutIgnoreIfConfiguratoinSetttingIsUsed_NoException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<ClassPropertyToDataTableColumnMapperTestData7>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration() { IgnoreInvalidTypes = true } );

            // Assert
            Assert.AreEqual(1, mapList.Count);
            TestDataType(theTable, "SomeTestProperty", typeof(string));
        }

        [TestMethod]
        public void Map_ClassCanHaveClassPropertyTypeWithoutIgnoreIfConfiguratoinSetttingIsUsed_NoException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<ClassPropertyToDataTableColumnMapperTestData8>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration() { IgnoreInvalidTypes = true });

            // Assert
            Assert.AreEqual(1, mapList.Count);
            TestDataType(theTable, "SomeTestProperty", typeof(string));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OutputTypeTest_CannotBeAClass_ResultsInAnException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<ClassPropertyToDataTableColumnMapperTestData2>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

            // Assert
            Assert.Fail("OutputType Cannot Be a class!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InterfaceTest_ConvertersMustImplement_IClassToDataTableConverter_ResultsInAnException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<ClassPropertyToDataTableColumnMapperTestData3>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

            // Assert
            Assert.Fail("Converter must implement IClassToDataTableConverter!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClassAttributeTest_NotSpecifyingTargetPropertyType_ResultsInAnException()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<ClassPropertyToDataTableColumnMapperTestData5>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

            // Assert
            Assert.Fail("You should get an error for not specifying a TargetPropertyType!");
        }


        [TestMethod]
        public void ConverterOnProperty_ValidConverterWorks_NoExceptions()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<ClassPropertyToDataTableColumnMapperTestData4>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

            // Assert
            Assert.AreEqual(2, theTable.Columns.Count, "Column count is wrong in the DataTable");
            TestDataType(theTable, "SomeTestProperty", typeof(decimal));
            TestDataType(theTable, "SomeIntProperty", typeof(int));
        }

        [TestMethod]
        public void ConverterOnClass_ValidConverterWorks_NoExceptions()
        {
            // Arrange
            var theTable = new DataTable();
            var classUnderTest = new ClassPropertyToDataTableColumnMapper<ClassPropertyToDataTableColumnMapperTestData6>();

            // Act
            List<ClassPropertyToDataTableColumnMap> mapList = classUnderTest.Map(theTable, new ClassToDataTableConfiguration());

            // Assert
            Assert.AreEqual(2, theTable.Columns.Count, "Column count is wrong in the DataTable");
            TestDataType(theTable, "SomeTestProperty", typeof(decimal));
            TestDataType(theTable, "SomeIntProperty", typeof(int));
        }


        private void TestDataType(DataTable theTable, string columnName, Type columnType)
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

  

    internal class ClassPropertyToDataTableColumnMapperTestData1
    {
        public int SomeIntProperty { get; set; }

        [ClassToDataTableConverter(typeof(MapperTestConverterWithArrayOutput))]
        public string SomeStringProperty { get; set; }
    }

    internal class ClassPropertyToDataTableColumnMapperTestData2
    {
        [ClassToDataTableConverter(typeof(MapperTestConverterWithClassOutput))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class ClassPropertyToDataTableColumnMapperTestData3
    {
        [ClassToDataTableConverter(typeof(NotRealConverter))]
        public int SomeIntProperty { get; set; }
        public string SomeStringProperty { get; set; }
    }

    internal class ClassPropertyToDataTableColumnMapperTestData4
    {
        [ClassToDataTableConverter(typeof(PercentCtodTypeConverter))]
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }

    [ClassToDataTableConverter(typeof(PercentCtodTypeConverter))]
    internal class ClassPropertyToDataTableColumnMapperTestData5
    {
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }


    [ClassToDataTableConverter(typeof(PercentCtodTypeConverter), TargetPropertyType =typeof(string))]
    internal class ClassPropertyToDataTableColumnMapperTestData6
    {
        public string SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }


    internal class ClassPropertyToDataTableColumnMapperTestData7
    {
        public string SomeTestProperty { get; set; }
        public int[] SomeIntArrayProperty { get; set; }
    }


    internal class ClassPropertyToDataTableColumnMapperTestData8
    {
        public ClassPropertyToDataTableColumnMapperTestData7 SomeTestProperty { get; set; }
        public int SomeIntProperty { get; set; }
    }



}
