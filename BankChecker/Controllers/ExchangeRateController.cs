using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Repository.Repositories;

namespace BankChecker.Controllers
{
    [Route("ExchangeRate")]
    public class ExchangeRateController : Controller
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly AppDbContext _context;
        public ExchangeRateController(IExchangeRateService exchangeRateService, AppDbContext context)
        {
            _exchangeRateService = exchangeRateService;
            _context = context;
        }

        [HttpGet("BankList")]
        public async Task<IActionResult> BankList()
        {
            var exchangeRates = await _exchangeRateService.GetExchangeRatesAsync();
            return View(exchangeRates);
        }

        [HttpGet("ExchangeDetails/{bankName}/{currencyCode}")]
        public async Task<IActionResult> ExchangeDetails(string bankName, string currencyCode)
        {
            var bank = await _exchangeRateService.GetRateByBankAndCurrencyAsync(bankName, currencyCode);

            if (bank != null)
            {
                return View(bank);
            }
            return NotFound();
        }
    }
}
