using System;
using System.Linq;
using System.Reflection;
using ClassToDataTable.Shared;
using ClassToDataTable.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassToDataTable.Tests
{
    [TestClass]
    public class PercentCtodTypeConverterTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotHandleNonStringFields()
        {
            // Arrange
            PropertyInfo decimalProperty = ReflectionHelper.FindPropertyInfoByName<PercentConverterTest>("SomeDecimalProperty");
            Assert.IsNotNull(decimalProperty, "Cannot find property");

            var data = new PercentConverterTest() { SomeDecimalProperty = .35m };

            // Act
            var classUnderTest = new PercentCtodTypeConverter();
            decimal? somePercentage = classUnderTest.GetValue(decimalProperty, data) as decimal?;

            // Assert
            throw new Exception("Should have encountered exception above because it cannot handle anything other than a string");            
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotCovertJunkStrings()
        {
            // Arrange
            PropertyInfo stringProperty = ReflectionHelper.FindPropertyInfoByName<PercentConverterTest>("SomeStringProperty");
            Assert.IsNotNull(stringProperty, "Cannot find property");

            var data = new PercentConverterTest() { SomeStringProperty = "3d5%" };

            // Act
            var classUnderTest = new PercentCtodTypeConverter();
            decimal? somePercentage = classUnderTest.GetValue(stringProperty, data) as decimal?;

            // Assert
            throw new Exception("Should have encountered exception above because the string is invalid!");
        }

        [TestMethod]
        public void CanCovertStringWithPercentageSign()
        {
            // Arrange
            PropertyInfo stringProperty = ReflectionHelper.FindPropertyInfoByName<PercentConverterTest>("SomeStringProperty");
            Assert.IsNotNull(stringProperty, "Cannot find property");

            var data = new PercentConverterTest() { SomeStringProperty = "35%"  };

            // Act
            var classUnderTest = new PercentCtodTypeConverter();
            decimal? somePercentage = classUnderTest.GetValue(stringProperty, data) as decimal?;

            // Assert
            Assert.IsNotNull(somePercentage);
            Assert.AreEqual(.35m, somePercentage.Value);
        }

        [TestMethod]
        public void CanCovertStringWithoutPercentageSign()
        {
            // Arrange
            PropertyInfo stringProperty = ReflectionHelper.FindPropertyInfoByName<PercentConverterTest>("SomeStringProperty");
            Assert.IsNotNull(stringProperty, "Cannot find property");

            var data = new PercentConverterTest() { SomeStringProperty = "35" };

            // Act
            var classUnderTest = new PercentCtodTypeConverter();
            decimal? somePercentage = classUnderTest.GetValue(stringProperty, data) as decimal?;

            // Assert
            Assert.IsNotNull(somePercentage);
            Assert.AreEqual(.35m, somePercentage.Value);
        }

    }

    internal class PercentConverterTest
    {
        public string SomeStringProperty { get; set; }
        public decimal SomeDecimalProperty { get; set; }
    }
}
