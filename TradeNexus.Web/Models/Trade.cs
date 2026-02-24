using System;

namespace TradeNexus.Web.Models
{
    public class Trade
    {
        public int TradeId { get; set; }
        
        // Client Info
        public int ClientId { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string PAN { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal MarginAvailable { get; set; }
        
        // SubBroker Info
        public int SubBrokerId { get; set; }
        public string SubBrokerCode { get; set; }
        public string SubBrokerName { get; set; }
        
        // Instrument Info
        public int InstrumentId { get; set; }
        public string Symbol { get; set; }
        public string Segment { get; set; }
        public string Exchange { get; set; }
        public int LotSize { get; set; }
        
        // Trade Details
        public DateTime TradeDate { get; set; }
        public string BuySell { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        
        // Charges
        public decimal Brokerage { get; set; }
        public decimal ExchangeCharges { get; set; }
        public decimal SEBIFee { get; set; }
        public decimal StampDuty { get; set; }
        public decimal STT { get; set; }
        public decimal GST { get; set; }
        
        // Risk Metrics
        public decimal VaRPercent { get; set; }
        public decimal ExposurePercent { get; set; }
    }
}