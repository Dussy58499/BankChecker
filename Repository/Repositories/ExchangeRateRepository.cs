using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Repository.Interfaces;
using Repository.Models.Domain;

namespace Repository.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly AppDbContext _context;

        public ExchangeRateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRateAsync()
        {
            return await _context.ExchangeRates.ToListAsync();
        }

        public async Task<IEnumerable<ExchangeRate>> GetByIdAsync(Guid id)
        {
            var result = await _context.ExchangeRates.Where(i => i.Id == id).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<ExchangeRate>> GetByBankAsync(string bankname)
        {
            return await _context.ExchangeRates.Where(b => b.BankName == bankname).ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ExchangeRate> exchangeRates)
        {
            await _context.ExchangeRates.AddRangeAsync(exchangeRates);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(ExchangeRate entity)
        {
            await _context.ExchangeRates.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<ExchangeRate> GetByBankAndCurrencyAsync(string bankName, string currencyCode)
        {
            return await _context.ExchangeRates
                                 .Where(r => r.BankName == bankName && r.CurrencyCode == currencyCode)
                                 .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(ExchangeRate entity)
        {
            _context.ExchangeRates.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.ExchangeRates.RemoveRange(entity);
                 await _context.SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}