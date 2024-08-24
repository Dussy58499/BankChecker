using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Service.Interfaces;
using System;
using System.Linq;
using Service.Helpers;
using Repository.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Service.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IExchangeRateRepository _repository;

        public ExchangeRateService(HttpClient httpClient, IExchangeRateRepository repository)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
        {
            var bankApiUrls = new Dictionary<string, string>
            {
                { "PrivatBank", "https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5" },
                { "Monobank", "https://api.monobank.ua/bank/currency" },
                { "NBU", "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json" },
            };

            var allRates = new List<ExchangeRate>();

            foreach (var bank in bankApiUrls)
            {
                var existingRates = await _repository.GetByBankAsync(bank.Key);

                if (!existingRates.Any())
                {
                    var fetchedRates = await ExchangeRateParser(bank.Value, bank.Key);
                    allRates.AddRange(fetchedRates);

                    foreach(var rate in fetchedRates)
                    {
                        await _repository.AddAsync(rate);
                    }
                }
                else
                {
                    allRates.AddRange(existingRates);
                }
            }
            return allRates;
        }

        public async Task<ExchangeRate> GetRateByBankAndCurrencyAsync(string bankName, string currencyCode)
        {
            return await _repository.GetByBankAndCurrencyAsync(bankName, currencyCode);
        }

        public async Task<List<ExchangeRate>> ExchangeRateParser(string url, string bankName)
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
                return rates ?? new List<ExchangeRate>();
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
    }
}
