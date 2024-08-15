using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Mapping
{
    public static class ExchangeRateMapping
    {
        private static readonly Dictionary<string, string> CodeToName = new Dictionary<string, string>
        {
            {"840", "USD" },
            {"980", "UAH"},
            {"USD", "840" },
            {"UAH", "980"}
        };
        public static string GetCurrencyName(string code)
        {
            return CodeToName.TryGetValue(code, out var name) ? name : code;
        }
        public static string GetCurrencyCode(string name)
        {
            return CodeToName.TryGetValue(name, out var code) ? code : name;
        }

    }
}
