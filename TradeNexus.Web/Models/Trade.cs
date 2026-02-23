using System;

namespace TradeNexus.Web.Models
{
    public class Trade
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime TradeDate { get; set; }
    }
}