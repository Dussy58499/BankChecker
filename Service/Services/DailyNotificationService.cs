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
using Repository.Interfaces;

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

            var now = DateTime.Now;
            var firstRun = new DateTime(now.Year, now.Month, now.Day, 10, 26, 0, DateTimeKind.Utc);

            if (now > firstRun)
            {
                firstRun = firstRun.AddDays(1);
            }

            var initialDelay = firstRun - now;
            var period = TimeSpan.FromDays(1);

            _timer = new Timer(async _ => await SendDailyNotifications(), null, initialDelay, period);
            return Task.CompletedTask;
        }

        private async Task SendDailyNotifications()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                var exchangeRateRepository = scope.ServiceProvider.GetRequiredService<IExchangeRateRepository>();

                var users = userManager.Users.Where(u => u.ReceiveDailyNotifications).ToList();
                var exchangeRates = await exchangeRateRepository.GetExchangeRateAsync();

                var message = "Today's exchange rates:\n" +
                           string.Join("\n", exchangeRates.Select(rate =>
                               $"{rate.BankName} -> {rate.CurrencyUahName} - {rate.CurrencyName}: {rate.BuyRate}/{rate.SellRate} ")) +
                           "\n";

                foreach (var user in users)
                {
                    var email = await userManager.GetEmailAsync(user);
                    await emailSender.SendEmailAsync(email, "Daily Exchange Rates", message);
                }
            }
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