using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Repository.Models.Domain;

namespace Service.Interfaces
{
    public interface IExchangeRateService
    {
        Task<List<FullExchangeRate>> GetExchangeRatesAsync();
    }
}
