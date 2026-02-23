namespace TradeNexus.Web.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public int SubBrokerId { get; set; }
        public decimal TradingLimit { get; set; }

        public SubBroker SubBroker { get; set; }
    }
}