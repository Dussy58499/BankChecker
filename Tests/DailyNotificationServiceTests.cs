using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Models.Domain;
using Service.Services;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class DailyNotificationServiceTests
    {
        [TestMethod]
        public void GetDailyExchangeRates_Ok()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.ExchangeRates.AddRange(new List<ExchangeRate>
                {
                     new ExchangeRate { BankName = "privatbank", CurrencyUahName = "UAH", CurrencyName = "USD", BuyRate = 27.5M, SellRate = 28.0M },
                     new ExchangeRate { BankName = "monobank", CurrencyUahName = "UAH", CurrencyName = "USD", BuyRate = 32.0M, SellRate = 32.5M }
                });
                context.SaveChanges();
            }

            // Act
            string result;
            using (var context = new AppDbContext(options))
            {
                var service = new DailyNotificationService(null, null);
                result = service.GetDailyExchangeRates(context);
            }

            // Assert
            var expected = "privatbank -> UAH - USD: 27,5/28,0 \nmonobank -> UAH - USD: 32,0/32,5 ";
            Assert.AreEqual(expected, result);
        }
    }
}