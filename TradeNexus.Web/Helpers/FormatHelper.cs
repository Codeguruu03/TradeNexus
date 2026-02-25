using System;
using System.Globalization;

namespace TradeNexus.Web.Helpers
{
    public static class FormatHelper
    {
        public static string ToIndianCurrency(this decimal amount)
        {
            var cultureInfo = new CultureInfo("en-IN");
            return "₹" + amount.ToString("N0", cultureInfo);
        }

        public static string ToIndianCurrencyWithDecimals(this decimal amount)
        {
            var cultureInfo = new CultureInfo("en-IN");
            return "₹" + amount.ToString("N2", cultureInfo);
        }
    }
}
