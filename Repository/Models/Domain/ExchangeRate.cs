using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Models.Domain
{
    public class ExchangeRate
    {
        public Guid Id { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyUahCode { get; set; }
        public string CurrencyUahName { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal BuyRate { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal SellRate { get; set; }
        public DateTime? Date { get; set; }
        public string BankName { get; set; }
    }
}
