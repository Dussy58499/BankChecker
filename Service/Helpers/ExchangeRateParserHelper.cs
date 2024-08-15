﻿using Newtonsoft.Json;
using Repository.Models.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Service.Mapping;
using System.Runtime.CompilerServices;

namespace Service.Helpers
{
    public static class ExchangeRateParserHelper
    {
        public static List<FullExchangeRate> ParseExchangeRates(string json, string bankName)
        {
            var rates = new List<FullExchangeRate>();
            try
            {
                switch (bankName.ToLower())
                {
                    case "privatbank":
                        var privatData = JsonConvert.DeserializeObject<List<PrivatBank>>(json);
                        rates = privatData.Where(item => item.Ccy == "USD" && item.Base_Ccy == "UAH")
                                          .Select(item => new FullExchangeRate
                                          {
                                              CurrencyCode = ExchangeRateMapping.GetCurrencyCode(item.Ccy),
                                              CurrencyName = item.Ccy,
                                              CurrencyUahCode = ExchangeRateMapping.GetCurrencyName(item.Base_Ccy),
                                              CurrencyUahName = item.Base_Ccy,
                                              BuyRate = item.Buy,
                                              SellRate = item.Sale,
                                              BankName = bankName
                                          }).ToList();
                        break;

                    case "monobank":
                        var monoData = JsonConvert.DeserializeObject<List<MonoBank>>(json);
                        rates = monoData.Where(item => item.CurrencyCodeA == "840" && item.CurrencyCodeB == "980")
                                        .Select(item => new FullExchangeRate
                                        {
                                            CurrencyCode = item.CurrencyCodeA,
                                            CurrencyName = ExchangeRateMapping.GetCurrencyName(item.CurrencyCodeA),
                                            CurrencyUahCode = item.CurrencyCodeB,
                                            CurrencyUahName = ExchangeRateMapping.GetCurrencyName(item.CurrencyCodeB),
                                            BuyRate = item.RateBuy,
                                            SellRate = item.RateSell,
                                            Date = TimeConverterHelper.GetDatetimeFromUnixTime(item.Date),
                                            BankName = bankName
                                        }).ToList();
                        break;

                    case "nbu":
                        var nbuData = JsonConvert.DeserializeObject<List<NbuBank>>(json);
                        rates = nbuData.Where(item => item.R030 == "840")
                                       .Select(item => new FullExchangeRate
                                       {
                                           CurrencyCode = item.R030,
                                           CurrencyName = item.Cc,
                                           CurrencyUahCode = "980",
                                           CurrencyUahName = "UAH",
                                           BuyRate=item.Rate,
                                           SellRate = item.Rate,
                                           Date = DateTime.Parse(item.ExchangeDate),
                                           BankName = bankName
                                       }).ToList();
                        break;

                    default:
                        throw new ArgumentException("Unsupported bank", nameof(bankName));
                }
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Error desearializing json for {bankName}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing exchange rates for {bankName}:{ex.Message}");
            }
            return rates;
        }
    }
}
