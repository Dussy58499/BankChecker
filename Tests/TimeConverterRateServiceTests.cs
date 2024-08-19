using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Helpers;
using System;

namespace Tests
{
    [TestClass]
    public class TimeConverterRateServiceTests
    {
        [TestMethod]
        public void GetDatetimeFromUnixTime_OK()
        {
            // Arrange
            long unixTime = 1622505600; //Unix timestamp for June 1, 2021 00:00:00 UTC
            DateTime expectedDateTime = new DateTime(2021, 6, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            DateTime actualDateTime = TimeConverterHelper.GetDatetimeFromUnixTime(unixTime);

            //Assert
            Assert.AreEqual(expectedDateTime, actualDateTime);
        }

        [TestMethod]
        public void GetDatetimeFromUnixTimeMAX_BAD()
        {
            // Arrange
            long unixTime = DateTimeOffset.MaxValue.ToUnixTimeSeconds() + 90000000000;

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                TimeConverterHelper.GetDatetimeFromUnixTime(unixTime));
        }

        [TestMethod]
        public void GetDatetimeFromUnixTimeMIN_BAD()
        {          
            // Arrange
            long unixTime = DateTimeOffset.MinValue.ToUnixTimeSeconds() - 1;

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                TimeConverterHelper.GetDatetimeFromUnixTime(unixTime));
        }
    }
}
