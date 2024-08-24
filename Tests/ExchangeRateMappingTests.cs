using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Mapping;

namespace Tests
{
    [TestClass]
    public class ExchangeRateMappingTests
    {
        [TestMethod]
        public void GetCurrencyName_Ok_Ma()
        {
            // Arrange
            var code = "840";
            var expectedName = "USD";

            // Act
            var result = ExchangeRateMapping.GetCurrencyName(code);

            // Assert
            Assert.AreEqual(expectedName, result);
        }

        [TestMethod]
        public void GetCurrencyName_OK_SavedInputValue()
        {
            // Arrange
            var code = "999";
            var expectedName = "999";

            // Act
            var result = ExchangeRateMapping.GetCurrencyName(code);

            // Assert
            Assert.AreEqual(expectedName, result);
        }

        [TestMethod]
        public void GetCurrencyCode_Ok_Mapped()
        {
            // Arrange
            var name = "USD";
            var expectedCode = "840";

            // Act
            var result = ExchangeRateMapping.GetCurrencyCode(name);

            // Assert
            Assert.AreEqual(expectedCode, result);
        }

        [TestMethod]
        public void GetCurrencyCode_OK_SavedInputValue()
        {
            // Arrange
            var name = "XYZ";
            var expectedCode = "XYZ";

            // Act
            var result = ExchangeRateMapping.GetCurrencyCode(name);

            // Assert
            Assert.AreEqual(expectedCode, result);
        }
    }
}
