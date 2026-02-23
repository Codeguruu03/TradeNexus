namespace TradeNexus.Web.Models
{
    public class SubBroker
    {
        public int Id { get; set; }
        public string BrokerName { get; set; }
        public decimal ExposureLimit { get; set; }
        public bool IsActive { get; set; }
    }
}