using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Service.Helpers;

namespace Tests
{
    [TestClass]
    public class EmailSenderTests
    {
        private Mock<IConfiguration> _configurationMock;

        [TestInitialize]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.SetupGet(c => c["EmailSettings:SmtpServer"]).Returns("smtp.gmail.com");
            _configurationMock.SetupGet(c => c["EmailSettings:Port"]).Returns("587");
            _configurationMock.SetupGet(c => c["EmailSettings:Username"]).Returns("ivankiv.test@gmail.com");
            _configurationMock.SetupGet(c => c["EmailSettings:Password"]).Returns("xxqyufbsbrhqtuel");
            _configurationMock.SetupGet(c => c["EmailSettings:FromEmail"]).Returns("ivankiv.test@gmail.com");
        }

        [TestMethod]
        public async Task SendEmailAsync_SendsEmail_OK()
        {
            // Arrange
            var emailSender = new EmailSender(_configurationMock.Object);

            // Act
            await emailSender.SendEmailAsync("test@gmail.com", "Test Subject", "Test Body");

            //Assert
            Assert.IsTrue(true);
        }
    }
}