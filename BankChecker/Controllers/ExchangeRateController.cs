using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace BankChecker.Controllers
{
    public class ExchangeRateController : Controller
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly AppDbContext _context;

        public ExchangeRateController(IExchangeRateService exchangeRateService, AppDbContext context)
        {
            _exchangeRateService = exchangeRateService;
            _context = context;

        }
        public async Task<IActionResult> BankList()
        {
            var exchangeRates = await _exchangeRateService.GetExchangeRatesAsync();
            return View(exchangeRates);
        }

        public async Task<IActionResult> ExchangeDetails(string bankName, string currencyCode)
        {
            var bank = await _context.FullExchangeRates
                            .Where(r => r.BankName == bankName && r.CurrencyCode == currencyCode)
                            .FirstOrDefaultAsync();

            if (bank != null)
            {
                return View(bank);
            }
            return NotFound();
        }
    }
}
