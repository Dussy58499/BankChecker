using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Repository.Data;

namespace Service.Services
{
    public class DailyNotificationService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyNotificationService> _logger;
        private Timer _timer;

        public DailyNotificationService(IServiceProvider serviceProvider, ILogger<DailyNotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily Notification Service is starting.");
            _timer = new Timer(SendDailyNotifications, null, TimeSpan.Zero, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void SendDailyNotifications(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var users = userManager.Users.Where(u => u.ReceiveDailyNotifications).ToList();

                foreach (var user in users)
                {
                    var email = await userManager.GetEmailAsync(user);
                    var exchangeRates = GetDailyExchangeRates(dbContext);
                    var message = $"Today's exchange rates:\n{exchangeRates}";

                    await emailSender.SendEmailAsync(email, "Daily Exchange Rates", message);
                }
            }
        }

        private string GetDailyExchangeRates(AppDbContext context)
        {
            var rates = context.FullExchangeRates
                .Select(rate => $"{rate.BankName} -> {rate.CurrencyUahName} - {rate.CurrencyName}: {rate.BuyRate}/{rate.SellRate} ")
                .ToList();

            return string.Join("\n", rates);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily Notification Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
