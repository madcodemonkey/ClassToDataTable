using System;
using System.Reflection;
using ClassToDataTable.Shared;
using ClassToDataTable.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassToDataTable.Tests
{
    [TestClass]
    public class ChangeTypeCtodTypeConverterTest
    {
        [DataTestMethod]
        [DataRow(typeof(Int64))]
        [DataRow(typeof(Int32))]
        [DataRow(typeof(Int16))]
        public void CanCovertEnum_EnumIsCastToDifferentIntTypesSpecified(Type targetType)
        {
            // Arrange
            PropertyInfo enumProperty = ReflectionHelper.FindPropertyInfoByName<ChangeTypeCtodTypeConverterPropTest>("Gender");
            Assert.IsNotNull(enumProperty, "Cannot find property");

            var data = new ChangeTypeCtodTypeConverterPropTest() { FirstName = "John", LastName = "Doe", Gender = GenderTestEnum.Male };
            
            var classUnderTest = new ChangeTypeCtodTypeConverter();
            classUnderTest.Initialize(new ClassToDataTableConverterAttribute(typeof(ChangeTypeCtodTypeConverter)){ TargetPropertyType = targetType} );
           
            // Act
            object actualValue = classUnderTest.Convert(enumProperty, data);

            // Assert
            Assert.IsNotNull(actualValue);
            Assert.AreEqual(targetType.Name, actualValue.GetType().Name);
            Assert.AreEqual(Convert.ChangeType(2, targetType),   actualValue);
        }

        [DataTestMethod]
        [DataRow(typeof(Int64?))]
        [DataRow(typeof(Int32?))]
        [DataRow(typeof(Int16?))]
        public void CanCovertEnum_EnumIsCastToDifferentNullableIntTypesSpecified(Type targetType)
        {
            // Arrange
            PropertyInfo enumProperty = ReflectionHelper.FindPropertyInfoByName<ChangeTypeCtodTypeConverterNullPropTest>("Gender");
            Assert.IsNotNull(enumProperty, "Cannot find property");

            var data = new ChangeTypeCtodTypeConverterNullPropTest() { FirstName = "John", LastName = "Doe", Gender = GenderTestEnum.Male };
            
            var classUnderTest = new ChangeTypeCtodTypeConverter();
            classUnderTest.Initialize(new ClassToDataTableConverterAttribute(typeof(ChangeTypeCtodTypeConverter)){ TargetPropertyType = targetType} );
           
            // Act
            object actualValue = classUnderTest.Convert(enumProperty, data);

            // Assert
            Assert.IsNotNull(actualValue);

            var underlyingType = Nullable.GetUnderlyingType(targetType);
            Assert.AreEqual(underlyingType.Name, actualValue.GetType().Name);
            Assert.AreEqual(Convert.ChangeType(2, underlyingType),   actualValue);
        }

    }

    internal enum GenderTestEnum { Female = 1, Male = 2,  Unspecified = 3 }

    internal class ChangeTypeCtodTypeConverterPropTest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [ClassToDataTableConverter(typeof(ChangeTypeCtodTypeConverter), TargetPropertyType = typeof(int))]
        public GenderTestEnum Gender { get; set; }
    }

    internal class ChangeTypeCtodTypeConverterNullPropTest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [ClassToDataTableConverter(typeof(ChangeTypeCtodTypeConverter), TargetPropertyType = typeof(int))]
        public GenderTestEnum? Gender { get; set; }

    }
}
