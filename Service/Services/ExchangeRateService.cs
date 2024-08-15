using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Service.Interfaces;
using System;
using System.Linq;
using Service.Helpers;
using Repository.Models.Domain;
using Microsoft.EntityFrameworkCore;


namespace Service.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public ExchangeRateService(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private async Task<List<FullExchangeRate>> ExchangeRateParser(string url, string bankName)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL cannot be null or empty", nameof(url));
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new InvalidOperationException($"The URL {url} is not absolute");
            }

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var rates = ExchangeRateParserHelper.ParseExchangeRates(response, bankName);

                if (rates != null)
                {
                    rates.ForEach(rate => rate.BankName = bankName);
                }
                return rates ?? new List<FullExchangeRate>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error for {bankName}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exchange rates from {bankName}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<FullExchangeRate>> GetExchangeRatesAsync()
        {
            var bankApiUrls = new Dictionary<string, string>
            {
                { "PrivatBank", "https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5" },
                { "Monobank", "https://api.monobank.ua/bank/currency" },
                { "NBU", "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json" },
            };

            var allRates = new List<FullExchangeRate>();

            foreach (var bank in bankApiUrls)
            {
                var existingRates = await _context.FullExchangeRates
                    .Where(x => x.BankName == bank.Key)
                    .ToListAsync();

                if (!existingRates.Any())
                {
                    var fetchedRates = await ExchangeRateParser(bank.Value, bank.Key);
                    allRates.AddRange(fetchedRates);

                    _context.FullExchangeRates.AddRange(fetchedRates);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    allRates.AddRange(existingRates);
                }
            }

            return allRates;
        }
    }
}
