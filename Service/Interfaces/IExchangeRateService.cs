using System.Collections.Generic;
using System.Threading.Tasks;
using Repository.Models.Domain;

namespace Service.Interfaces
{
    public interface IExchangeRateService
    {
        Task<List<ExchangeRate>> GetExchangeRatesAsync();
        Task<ExchangeRate> GetRateByBankAndCurrencyAsync(string bankName, string currencyCode);
    }
}
