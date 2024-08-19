using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Service.Helpers;
using Repository.Models.Domain;
using Service.Mapping;

namespace Tests
{
    [TestClass]
    public class ExchangeRateParserHelperTests
    {
        [TestMethod]
        public void ParseExchangeRates_PrivatBank_OK()
        {
            // Arrange
            var json = "[{\"Ccy\":\"USD\",\"Base_Ccy\":\"UAH\",\"Buy\":27.0,\"Sale\":27.5}]";
            var bankName = "PrivatBank";

            // Act
            var rates = ExchangeRateParserHelper.ParseExchangeRates(json, bankName);

            // Assert
            Assert.AreEqual(1, rates.Count);
            Assert.AreEqual("USD", rates[0].CurrencyName);
            Assert.AreEqual(27.0m, rates[0].BuyRate);
            Assert.AreEqual(27.5m, rates[0].SellRate);
        }

        [TestMethod]
        public void ParseExchangeRates_MonoBank_OK()
        {
            // Arrange
            var json = "[{\"CurrencyCodeA\":\"840\",\"CurrencyCodeB\":\"980\",\"Date\":1609459200,\"RateBuy\":27.2,\"RateSell\":27.8}]";
            var bankName = "Monobank";

            // Act
            var rates = ExchangeRateParserHelper.ParseExchangeRates(json, bankName);

            // Assert
            Assert.AreEqual(1, rates.Count);
            Assert.AreEqual("840", rates[0].CurrencyCode);
            Assert.AreEqual(27.2m, rates[0].BuyRate);
            Assert.AreEqual(27.8m, rates[0].SellRate);
        }

        [TestMethod]
        public void ParseExchangeRates_Nbu_OK()
        {
            // Arrange
            var json = "[{\"R030\": \"840\", \"Txt\": \"Долар США\", \"Rate\": 27.32, \"Cc\": \"USD\", \"ExchangeDate\": \"19.08.2024\"}]";
            var bankName = "nbu";

            // Act
            var rates = ExchangeRateParserHelper.ParseExchangeRates(json, bankName);

            // Assert
            Assert.AreEqual(1, rates.Count);
            Assert.AreEqual("USD", rates[0].CurrencyName);
            Assert.AreEqual(27.32m, rates[0].BuyRate);
            Assert.AreEqual(27.32m, rates[0].SellRate);
        }


        [TestMethod]
        public void ParseExchangeRates_UnsupportedBank_BAD()
        {
            // Arrange
            var json = "[]";
            var bankName = "unknownBank";

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => ExchangeRateParserHelper.ParseExchangeRates(json, bankName));
        }

        [TestMethod]
        public void ParseExchangeRates_InvalidJson_()
        {
            // Arrange
            var json = "invalid json";
            var bankName = "privatbank";

            // Act & Assert
            Assert.ThrowsException<JsonReaderException>(() => ExchangeRateParserHelper.ParseExchangeRates(json, bankName));
        }
    }
}
