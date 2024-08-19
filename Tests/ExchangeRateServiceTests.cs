using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Repository.Models.Domain;
using Service.Services;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    [TestClass]
    public class ExchangeRateServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private AppDbContext _context;
        private ExchangeRateService _service;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _service = new ExchangeRateService(_httpClient, _context);
        }

        [TestMethod]
        public async Task GetExchangeRatesAsync_OK()
        {
            // Arrange
            var privatBankJson = "[{\"Ccy\":\"USD\",\"Base_Ccy\":\"UAH\",\"Buy\":27.50,\"Sale\":27.90}]";
            var monoBankJson = "[{\"CurrencyCodeA\":\"840\",\"CurrencyCodeB\":\"980\",\"Date\":1626883200,\"RateBuy\":27.60,\"RateSell\":28.00}]";
            var nbuJson = "[{\"R030\":\"840\",\"Cc\":\"USD\",\"Rate\":27.50,\"ExchangeDate\":\"2024-08-19\"}]";

            var privatBankUrl = "https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5";
            var monoBankUrl = "https://api.monobank.ua/bank/currency";
            var nbuUrl = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";

            var privatBankRates = new List<FullExchangeRate>
            {
                new FullExchangeRate
                { Id = Guid.NewGuid(), BankName = "privatbank", CurrencyCode = "USD", BuyRate = 27.50M, SellRate = 27.90M }
            };

            var monoBankRates = new List<FullExchangeRate>
            {
                new FullExchangeRate
                { Id = Guid.NewGuid(), BankName = "monobank", CurrencyCode = "USD", BuyRate = 27.60M, SellRate = 28.00M }
            };

            var nbuRates = new List<FullExchangeRate>
            {
                new FullExchangeRate
                { Id = Guid.NewGuid(), BankName = "nbu", CurrencyCode = "USD", BuyRate = 27.50M, SellRate = 27.50M, Date = DateTime.Parse("2024-08-19") }
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == privatBankUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(privatBankJson),
                })
                .Verifiable();

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == monoBankUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(monoBankJson),
                })
                .Verifiable();

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == nbuUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(nbuJson),
                })
                .Verifiable();

            _context.FullExchangeRates.AddRange(privatBankRates);
            _context.FullExchangeRates.AddRange(monoBankRates);
            _context.FullExchangeRates.AddRange(nbuRates);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetExchangeRatesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(privatBankRates.Count + monoBankRates.Count + nbuRates.Count, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExchangeRateParser_UrlIsEmpty_BAD()
        {
            // Act
            await _service.ExchangeRateParser("", "privatbank");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ExchangeRateParser_UrlIsNotAbsolute_BAD()
        {
            // Act
            await _service.ExchangeRateParser("invalid-url", "privatbank");
        }

        [TestMethod]
        public async Task ExchangeRateParser_PrivatBank_OK()
        {
            // Arrange
            var json = "[{\"Ccy\":\"USD\",\"Base_Ccy\":\"UAH\",\"Buy\":27.50,\"Sale\":27.90}]";
            var expectedRates = new List<FullExchangeRate>
            {
                new FullExchangeRate { CurrencyCode = "840", CurrencyName = "USD", CurrencyUahCode = "980", CurrencyUahName = "UAH", BuyRate = 27.50M, SellRate = 27.90M, BankName = "privatbank" }
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(json),
                })
                .Verifiable();

            // Act
            var result = await _service.ExchangeRateParser("https://valid-url.com", "privatbank");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedRates[0].CurrencyCode, result[0].CurrencyCode);
            Assert.AreEqual(expectedRates[0].BuyRate, result[0].BuyRate);
        }

        [TestMethod]
        public async Task ExchangeRateParser_Monobank_OK()
        {
            // Arrange
            var json = "[{\"CurrencyCodeA\":\"840\",\"CurrencyCodeB\":\"980\",\"Date\":1626883200,\"RateBuy\":27.60,\"RateSell\":28.00}]";
            var expectedRates = new List<FullExchangeRate>
            {
                new FullExchangeRate
                {
                    CurrencyCode = "840",
                    CurrencyName = "USD",
                    CurrencyUahCode = "980",
                    CurrencyUahName = "UAH",
                    BuyRate = 27.60M,
                    SellRate = 28.00M,
                    Date = TimeConverterHelper.GetDatetimeFromUnixTime(1626883200),
                    BankName = "monobank"
                }
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(json),
                })
                .Verifiable();

            // Act
            var result = await _service.ExchangeRateParser("https://valid-url.com", "monobank");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedRates[0].CurrencyCode, result[0].CurrencyCode);
            Assert.AreEqual(expectedRates[0].BuyRate, result[0].BuyRate);
            Assert.AreEqual(expectedRates[0].SellRate, result[0].SellRate);
            Assert.AreEqual(expectedRates[0].Date, result[0].Date);
        }

        [TestMethod]
        public async Task ExchangeRateParser_Nbu_OK()
        {
            // Arrange
            var json = "[{\"R030\":\"840\",\"Cc\":\"USD\",\"Rate\":27.50,\"ExchangeDate\":\"2024-08-19\"}]";
            var expectedRates = new List<FullExchangeRate>
            {
                new FullExchangeRate
                {
                    CurrencyCode = "840",
                    CurrencyName = "USD",
                    CurrencyUahCode = "980",
                    CurrencyUahName = "UAH",
                    BuyRate = 27.50M,
                    SellRate = 27.50M,
                    Date = DateTime.Parse("2024-08-19"),
                    BankName = "nbu"
                }
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(json),
                })
                .Verifiable();

            // Act
            var result = await _service.ExchangeRateParser("https://valid-url.com", "nbu");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedRates[0].CurrencyCode, result[0].CurrencyCode);
            Assert.AreEqual(expectedRates[0].BuyRate, result[0].BuyRate);
            Assert.AreEqual(expectedRates[0].SellRate, result[0].SellRate);
            Assert.AreEqual(expectedRates[0].Date, result[0].Date);
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public async Task ExchangeRateParser_InvalidJson_BAD()
        {
            // Arrange
            var invalidJson = "Invalid Json";
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(invalidJson),
                })
                .Verifiable();

            // Act
            await _service.ExchangeRateParser("https://valid-url.com", "privatbank");
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task ExchangeRateParser_InvalidUrl_BAD()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException())
                .Verifiable();

            // Act
            await _service.ExchangeRateParser("https://invalid-url.com", "privatbank");
        }
    }
}
