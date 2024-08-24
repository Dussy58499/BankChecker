using Microsoft.EntityFrameworkCore;
using Repository.Models.Domain;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task<IEnumerable<ExchangeRate>> GetExchangeRateAsync();
        Task<IEnumerable<ExchangeRate>> GetByIdAsync(Guid id);
        Task<IEnumerable<ExchangeRate>> GetByBankAsync(string bankname);
        Task AddRangeAsync(IEnumerable<ExchangeRate> exchangeRates);
        Task AddAsync(ExchangeRate entity);
        Task<ExchangeRate> GetByBankAndCurrencyAsync(string bankName, string currencyCode);
        Task UpdateAsync(ExchangeRate entity);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
