using System;
using System.Linq;
using TradeNexus.Web.Models;

namespace TradeNexus.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Create database if it doesn't exist
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.SubBrokers.Any())
            {
                return;   // DB has been seeded
            }

            // Seed Sub-Brokers
            var subBrokers = new SubBroker[]
            {
                new SubBroker { BrokerName = "Zerodha", ExposureLimit = 10000000, IsActive = true },
                new SubBroker { BrokerName = "Upstox", ExposureLimit = 5000000, IsActive = true },
                new SubBroker { BrokerName = "AngelOne", ExposureLimit = 7500000, IsActive = true }
            };

            context.SubBrokers.AddRange(subBrokers);
            context.SaveChanges();

            // Seed Clients
            var clients = new Client[]
            {
                new Client { ClientName = "Raj Kumar", SubBrokerId = 1, TradingLimit = 500000 },
                new Client { ClientName = "Priya Sharma", SubBrokerId = 1, TradingLimit = 750000 },
                new Client { ClientName = "Amit Patel", SubBrokerId = 2, TradingLimit = 300000 },
                new Client { ClientName = "Sneha Gupta", SubBrokerId = 3, TradingLimit = 600000 }
            };

            context.Clients.AddRange(clients);
            context.SaveChanges();

            // Seed Trades
            var trades = new Trade[]
            {
                new Trade { ClientId = 1, Symbol = "RELIANCE", Quantity = 100, Price = 2450.50m, TradeDate = DateTime.Now.AddDays(-2) },
                new Trade { ClientId = 1, Symbol = "TCS", Quantity = 50, Price = 3500.75m, TradeDate = DateTime.Now.AddDays(-1) },
                new Trade { ClientId = 2, Symbol = "INFY", Quantity = 200, Price = 1450.25m, TradeDate = DateTime.Now.AddDays(-3) },
                new Trade { ClientId = 3, Symbol = "HDFC", Quantity = 75, Price = 1650.00m, TradeDate = DateTime.Now },
                new Trade { ClientId = 4, Symbol = "ICICIBANK", Quantity = 150, Price = 950.50m, TradeDate = DateTime.Now.AddDays(-1) }
            };

            context.Trades.AddRange(trades);
            context.SaveChanges();
        }
    }
}
