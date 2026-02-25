using System;
using System.Linq;
using TradeNexus.Web.Models;

namespace TradeNexus.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            Console.WriteLine("Initializing SQLite database...");
            context.Database.EnsureCreated();

            if (context.Trades.Any())
            {
                Console.WriteLine("Database already seeded.");
                return;
            }

            Console.WriteLine("Seeding database with sample data...");
            var trades = new Trade[]
            {
                // Client 1 — Rahul Sharma (Alpha Broking)
                new Trade { ClientId=1, ClientCode="CL001", ClientName="Rahul Sharma", PAN="ABCDE1234F", City="Mumbai", State="Maharashtra", MarginAvailable=500000, SubBrokerId=1, SubBrokerCode="SB001", SubBrokerName="Alpha Broking", InstrumentId=1, Symbol="RELIANCE", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,10), BuySell="BUY", Quantity=100, Price=2500, Brokerage=500, ExchangeCharges=50, SEBIFee=5, StampDuty=25, STT=100, GST=90, VaRPercent=12, ExposurePercent=5 },
                new Trade { ClientId=1, ClientCode="CL001", ClientName="Rahul Sharma", PAN="ABCDE1234F", City="Mumbai", State="Maharashtra", MarginAvailable=500000, SubBrokerId=1, SubBrokerCode="SB001", SubBrokerName="Alpha Broking", InstrumentId=2, Symbol="TCS", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,12), BuySell="SELL", Quantity=50, Price=3600, Brokerage=300, ExchangeCharges=30, SEBIFee=3, StampDuty=10, STT=120, GST=60, VaRPercent=10, ExposurePercent=4 },
                new Trade { ClientId=1, ClientCode="CL001", ClientName="Rahul Sharma", PAN="ABCDE1234F", City="Mumbai", State="Maharashtra", MarginAvailable=500000, SubBrokerId=1, SubBrokerCode="SB001", SubBrokerName="Alpha Broking", InstrumentId=3, Symbol="HDFCBANK", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,15), BuySell="BUY", Quantity=200, Price=1700, Brokerage=400, ExchangeCharges=40, SEBIFee=4, StampDuty=20, STT=80, GST=70, VaRPercent=9, ExposurePercent=4 },

                // Client 2 — Priya Mehta (Beta Capital)
                new Trade { ClientId=2, ClientCode="CL002", ClientName="Priya Mehta", PAN="FGHIJ5678K", City="Delhi", State="Delhi", MarginAvailable=750000, SubBrokerId=2, SubBrokerCode="SB002", SubBrokerName="Beta Capital", InstrumentId=4, Symbol="NIFTY26MARFUT", Segment="FUTURES", Exchange="NSE", LotSize=50, TradeDate=new DateTime(2026,2,11), BuySell="BUY", Quantity=2, Price=22500, Brokerage=1000, ExchangeCharges=100, SEBIFee=10, StampDuty=50, STT=200, GST=180, VaRPercent=15, ExposurePercent=6 },
                new Trade { ClientId=2, ClientCode="CL002", ClientName="Priya Mehta", PAN="FGHIJ5678K", City="Delhi", State="Delhi", MarginAvailable=750000, SubBrokerId=2, SubBrokerCode="SB002", SubBrokerName="Beta Capital", InstrumentId=5, Symbol="INFY", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,14), BuySell="SELL", Quantity=150, Price=1800, Brokerage=350, ExchangeCharges=35, SEBIFee=4, StampDuty=15, STT=90, GST=65, VaRPercent=11, ExposurePercent=4 },

                // Client 3 — Amit Verma (Gamma Securities) — HIGH RISK
                new Trade { ClientId=3, ClientCode="CL003", ClientName="Amit Verma", PAN="KLMNO9012P", City="Bangalore", State="Karnataka", MarginAvailable=300000, SubBrokerId=3, SubBrokerCode="SB003", SubBrokerName="Gamma Securities", InstrumentId=6, Symbol="BANKNIFTY26MARFUT", Segment="FUTURES", Exchange="NSE", LotSize=15, TradeDate=new DateTime(2026,2,10), BuySell="SELL", Quantity=4, Price=48000, Brokerage=1500, ExchangeCharges=150, SEBIFee=15, StampDuty=75, STT=300, GST=270, VaRPercent=18, ExposurePercent=7 },
                new Trade { ClientId=3, ClientCode="CL003", ClientName="Amit Verma", PAN="KLMNO9012P", City="Bangalore", State="Karnataka", MarginAvailable=300000, SubBrokerId=3, SubBrokerCode="SB003", SubBrokerName="Gamma Securities", InstrumentId=7, Symbol="ADANIENT", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,16), BuySell="BUY", Quantity=300, Price=2400, Brokerage=700, ExchangeCharges=70, SEBIFee=7, StampDuty=35, STT=150, GST=130, VaRPercent=20, ExposurePercent=8 },

                // Client 4 — Sneha Joshi (Delta Investments) — WATCHLIST
                new Trade { ClientId=4, ClientCode="CL004", ClientName="Sneha Joshi", PAN="PQRST3456Q", City="Pune", State="Maharashtra", MarginAvailable=450000, SubBrokerId=4, SubBrokerCode="SB004", SubBrokerName="Delta Investments", InstrumentId=8, Symbol="BAJFINANCE", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,13), BuySell="BUY", Quantity=120, Price=6800, Brokerage=900, ExchangeCharges=90, SEBIFee=9, StampDuty=45, STT=180, GST=160, VaRPercent=16, ExposurePercent=6 },
                new Trade { ClientId=4, ClientCode="CL004", ClientName="Sneha Joshi", PAN="PQRST3456Q", City="Pune", State="Maharashtra", MarginAvailable=450000, SubBrokerId=4, SubBrokerCode="SB004", SubBrokerName="Delta Investments", InstrumentId=9, Symbol="WIPRO", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,17), BuySell="SELL", Quantity=200, Price=550, Brokerage=200, ExchangeCharges=20, SEBIFee=2, StampDuty=10, STT=45, GST=40, VaRPercent=12, ExposurePercent=5 },

                // Client 5 — Rohit Malhotra (Epsilon Broking) — SAFE
                new Trade { ClientId=5, ClientCode="CL005", ClientName="Rohit Malhotra", PAN="UVWXY7890R", City="Hyderabad", State="Telangana", MarginAvailable=900000, SubBrokerId=5, SubBrokerCode="SB005", SubBrokerName="Epsilon Broking", InstrumentId=10, Symbol="SUNPHARMA", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,12), BuySell="BUY", Quantity=80, Price=1650, Brokerage=250, ExchangeCharges=25, SEBIFee=3, StampDuty=12, STT=60, GST=55, VaRPercent=11, ExposurePercent=4 },
                new Trade { ClientId=5, ClientCode="CL005", ClientName="Rohit Malhotra", PAN="UVWXY7890R", City="Hyderabad", State="Telangana", MarginAvailable=900000, SubBrokerId=5, SubBrokerCode="SB005", SubBrokerName="Epsilon Broking", InstrumentId=11, Symbol="HCLTECH", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,18), BuySell="BUY", Quantity=100, Price=1400, Brokerage=300, ExchangeCharges=30, SEBIFee=3, StampDuty=15, STT=65, GST=58, VaRPercent=13, ExposurePercent=5 },

                // Client 6 — Kavita Rao (Zeta Securities) — HIGH RISK
                new Trade { ClientId=6, ClientCode="CL006", ClientName="Kavita Rao", PAN="ABCYZ1234S", City="Chennai", State="Tamil Nadu", MarginAvailable=350000, SubBrokerId=6, SubBrokerCode="SB006", SubBrokerName="Zeta Securities", InstrumentId=12, Symbol="SENSEX26MARFUT", Segment="FUTURES", Exchange="BSE", LotSize=10, TradeDate=new DateTime(2026,2,14), BuySell="BUY", Quantity=3, Price=75000, Brokerage=2000, ExchangeCharges=200, SEBIFee=20, StampDuty=100, STT=400, GST=360, VaRPercent=22, ExposurePercent=9 },

                // Client 7 — Arjun Reddy (Theta Capital) — SAFE
                new Trade { ClientId=7, ClientCode="CL007", ClientName="Arjun Reddy", PAN="DEFGH5678T", City="Ahmedabad", State="Gujarat", MarginAvailable=800000, SubBrokerId=1, SubBrokerCode="SB001", SubBrokerName="Alpha Broking", InstrumentId=13, Symbol="LTIM", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,15), BuySell="SELL", Quantity=60, Price=5200, Brokerage=450, ExchangeCharges=45, SEBIFee=5, StampDuty=22, STT=100, GST=90, VaRPercent=14, ExposurePercent=6 },
                new Trade { ClientId=7, ClientCode="CL007", ClientName="Arjun Reddy", PAN="DEFGH5678T", City="Ahmedabad", State="Gujarat", MarginAvailable=800000, SubBrokerId=1, SubBrokerCode="SB001", SubBrokerName="Alpha Broking", InstrumentId=14, Symbol="AXISBANK", Segment="CAPITAL", Exchange="NSE", LotSize=1, TradeDate=new DateTime(2026,2,19), BuySell="BUY", Quantity=130, Price=1150, Brokerage=280, ExchangeCharges=28, SEBIFee=3, StampDuty=14, STT=58, GST=52, VaRPercent=10, ExposurePercent=4 },
            };

            context.Trades.AddRange(trades);
            context.SaveChanges();
        }
    }
}
