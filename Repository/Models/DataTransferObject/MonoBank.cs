namespace Repository.Models.DataTransferObject
{
    public class MonoBank
    {
        public string CurrencyCodeA { get; set; }
        public string CurrencyCodeB { get; set; }
        public long Date { get; set; }
        public decimal RateBuy { get; set; }
        public decimal RateSell { get; set; }
        public decimal RateCross { get; set; }
    }
}
