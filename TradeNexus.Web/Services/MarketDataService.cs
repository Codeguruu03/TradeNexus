using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TradeNexus.Web.Services
{
    public class MarketDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string _apiKey;

        // Map unsupported futures/index symbols to proxy cash symbols (not present in your trade DB)
        private static readonly Dictionary<string, string> FuturesProxyMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "NIFTY24JANFUT", "SBIN" },
            { "BANKNIFTY24JANFUT", "ICICIBANK" }
        };

        // Futures/index derivatives not directly supported by quote endpoints
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
        /// Uses Yahoo quote batch endpoint for speed.
        /// For unsupported futures symbols, proxy cash symbols are used.
        /// Results are cached for 2 minutes.
        /// </summary>
        public async Task<Dictionary<string, decimal?>> GetLivePricesAsync(IEnumerable<string> symbols)
        {
            var result = new Dictionary<string, decimal?>(StringComparer.OrdinalIgnoreCase);
            var symbolList = (symbols ?? Array.Empty<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!symbolList.Any())
                return result;

            // Original symbol -> effective symbol used for market quote lookup
            var symbolMap = symbolList.ToDictionary(s => s, ResolveEffectiveSymbol, StringComparer.OrdinalIgnoreCase);

            // Collect cache misses for effective symbols
            var missingEffectiveSymbols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var symbol in symbolList)
            {
                var effective = symbolMap[symbol];
                var cacheKey = $"ltp_{effective.ToUpperInvariant()}";

                if (_cache.TryGetValue(cacheKey, out decimal cached))
                {
                    result[symbol] = cached;
                }
                else
                {
                    missingEffectiveSymbols.Add(effective);
                }
            }

            // Fast batch fetch (single HTTP call)
            var fetched = missingEffectiveSymbols.Any()
                ? await FetchBatchQuotesFromYahooAsync(missingEffectiveSymbols)
                : new Dictionary<string, decimal?>(StringComparer.OrdinalIgnoreCase);

            // Fallback to AlphaVantage for symbols still missing from Yahoo
            if (missingEffectiveSymbols.Any())
            {
                foreach (var effective in missingEffectiveSymbols)
                {
                    if (!fetched.TryGetValue(effective, out var val) || val == null)
                    {
                        var alpha = await FetchSingleQuoteFromAlphaAsync(effective);
                        if (alpha != null)
                            fetched[effective] = alpha;
                    }
                }
            }

            // Populate result + cache
            foreach (var symbol in symbolList)
            {
                if (result.ContainsKey(symbol))
                    continue;

                var effective = symbolMap[symbol];
                fetched.TryGetValue(effective, out var price);
                result[symbol] = price;

                if (price != null)
                {
                    var cacheKey = $"ltp_{effective.ToUpperInvariant()}";
                    _cache.Set(cacheKey, price.Value, TimeSpan.FromMinutes(2));
                }
            }

            return result;
        }

        private static string ResolveEffectiveSymbol(string symbol)
        {
            if (FuturesProxyMap.TryGetValue(symbol, out var proxy))
                return proxy;

            return symbol?.Trim()?.ToUpperInvariant() ?? string.Empty;
        }

        private async Task<Dictionary<string, decimal?>> FetchBatchQuotesFromYahooAsync(IEnumerable<string> effectiveSymbols)
        {
            var result = new Dictionary<string, decimal?>(StringComparer.OrdinalIgnoreCase);
            var list = effectiveSymbols
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var symbol in list)
                result[symbol] = null;

            if (!list.Any())
                return result;

            try
            {
                // Yahoo supports multi-symbol quotes in one call
                // Example: RELIANCE.NS,TCS.NS
                var yahooSymbols = list.Select(s => $"{s}.NS");
                var url = "https://query1.finance.yahoo.com/v7/finance/quote?symbols=" + string.Join(",", yahooSymbols);

                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);
                var quotes = json["quoteResponse"]?["result"] as JArray;

                if (quotes == null)
                    return result;

                foreach (var quote in quotes)
                {
                    var quoteSymbol = quote["symbol"]?.Value<string>(); // e.g., RELIANCE.NS
                    if (string.IsNullOrWhiteSpace(quoteSymbol))
                        continue;

                    var baseSymbol = quoteSymbol.Split('.')[0].ToUpperInvariant();
                    var token = quote["regularMarketPrice"];

                    if (token != null && decimal.TryParse(token.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var price) && price > 0)
                    {
                        result[baseSymbol] = price;
                    }
                }
            }
            catch
            {
                // swallow and return nulls; fallback handles remaining symbols
            }

            return result;
        }

        private async Task<decimal?> FetchSingleQuoteFromAlphaAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || IsFutures(symbol))
                return null;

            try
            {
                var bseSymbol = $"{symbol}.BSE";
                var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={bseSymbol}&apikey={_apiKey}";
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);
                var priceToken = json["Global Quote"]?["05. price"];

                if (priceToken != null && decimal.TryParse(priceToken.Value<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var price) && price > 0)
                    return price;
            }
            catch
            {
            }

            return null;
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
