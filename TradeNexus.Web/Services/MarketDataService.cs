using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TradeNexus.Web.Services
{
    public class MarketDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string _apiKey;

        // Futures/index derivatives not supported by AlphaVantage — skip them
        private static readonly HashSet<string> FuturesKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "FUT", "MARFUT", "JUNFUT", "SEPFUT", "DECFUT"
        };

        public MarketDataService(HttpClient httpClient, IMemoryCache cache, IConfiguration config)
        {
            _httpClient = httpClient;
            _cache = cache;
            _apiKey = config["AlphaVantage:ApiKey"] ?? "";
        }

        /// <summary>
        /// Returns a dictionary of { symbol -> live price }.
        /// Returns null for futures/unsupported or on API failure.
        /// Results are cached for 5 minutes to stay within free tier limits.
        /// </summary>
        public async Task<Dictionary<string, decimal?>> GetLivePricesAsync(IEnumerable<string> symbols)
        {
            var result = new Dictionary<string, decimal?>(StringComparer.OrdinalIgnoreCase);

            foreach (var symbol in symbols)
            {
                // Skip derivatives/futures
                if (IsFutures(symbol))
                {
                    result[symbol] = null;
                    continue;
                }

                var cacheKey = $"ltp_{symbol.ToUpper()}";

                // Return cached price if available
                if (_cache.TryGetValue(cacheKey, out decimal cached))
                {
                    result[symbol] = cached;
                    continue;
                }

                // Fetch from AlphaVantage
                try
                {
                    var bseSymbol = $"{symbol}.BSE";
                    var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={bseSymbol}&apikey={_apiKey}";
                    var response = await _httpClient.GetStringAsync(url);
                    var json = JObject.Parse(response);

                    var priceToken = json["Global Quote"]?["05. price"];
                    if (priceToken != null && decimal.TryParse(priceToken.Value<string>(), out decimal price) && price > 0)
                    {
                        // Cache for 5 minutes
                        _cache.Set(cacheKey, price, TimeSpan.FromMinutes(5));
                        result[symbol] = price;
                    }
                    else
                    {
                        result[symbol] = null;
                    }
                }
                catch
                {
                    result[symbol] = null;
                }

                // Small delay to respect rate limits (5 req/min on free tier)
                await Task.Delay(200);
            }

            return result;
        }

        private static bool IsFutures(string symbol)
        {
            foreach (var kw in FuturesKeywords)
                if (symbol.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            return false;
        }
    }
}
