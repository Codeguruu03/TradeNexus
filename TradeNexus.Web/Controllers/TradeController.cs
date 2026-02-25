using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeNexus.Web.Data;
using TradeNexus.Web.Models;
using TradeNexus.Web.Services;

namespace TradeNexus.Web.Controllers
{
    public class TradeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PythonRiskService _riskService;

        public TradeController(ApplicationDbContext context, PythonRiskService riskService)
        {
            _context = context;
            _riskService = riskService;
        }

        public async Task<IActionResult> Index()
        {
            // Get distinct clients from trades
            var trades = await _context.Trades.ToListAsync();
            var uniqueClients = trades
                .GroupBy(t => t.ClientId)
                .Select(g => g.FirstOrDefault())
                .ToList();
            
            return View(uniqueClients);
        }

        public async Task<IActionResult> GetClientTrades(int clientId)
        {
            var trades = await _context.Trades
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.TradeDate)
                .ToListAsync();
            return PartialView("_ClientTradesPartial", trades);
        }

        public async Task<IActionResult> GetRiskAnalysis(int clientId)
        {
            var trades = await _context.Trades
                .Where(t => t.ClientId == clientId)
                .ToListAsync();

            if (trades == null || trades.Count == 0)
                return Content("<div class='p-3 text-center'>No trades found for risk analysis.</div>");

            var clientInfo = trades.FirstOrDefault();
            var tradeList = trades.Select(t => new { quantity = t.Quantity, price = (double)t.Price, symbol = t.Symbol, buySell = t.BuySell }).ToList();
            
            var input = new { trades = tradeList, availableMargin = (double)(clientInfo?.MarginAvailable ?? 500000) };
            string jsonInput = JsonConvert.SerializeObject(input);
            var riskData = _riskService.ExecuteRiskEngine(jsonInput);

            ViewBag.ClientName = clientInfo?.ClientName;
            ViewBag.TotalExposure = trades.Sum(t => t.Quantity * t.Price);
            
            return PartialView("_RiskAnalysisPartial", riskData);
        }

        public async Task<IActionResult> GetSubBrokerList()
        {
            var trades = await _context.Trades.ToListAsync();
            var subBrokers = trades
                .GroupBy(t => t.SubBrokerId)
                .Select(g => new { 
                    Id = g.Key, 
                    Name = g.First().SubBrokerName, 
                    Code = g.First().SubBrokerCode,
                    ClientCount = g.Select(x => x.ClientId).Distinct().Count(),
                    TotalExposure = g.Sum(x => x.Quantity * x.Price)
                }).ToList();

            return PartialView("_SubBrokersPartial", subBrokers);
        }

        public async Task<IActionResult> GetClientDetails(int clientId)
        {
            var clientTrade = await _context.Trades.FirstOrDefaultAsync(t => t.ClientId == clientId);
            return PartialView("_ClientDetailsPartial", clientTrade);
        }

        public async Task<IActionResult> GetClientsByType(string type)
        {
            var trades = await _context.Trades.ToListAsync();
            var clients = trades.GroupBy(t => t.ClientId).Select(g => g.First());

            if (type == "high")
            {
                clients = clients.Where(c => c.MarginAvailable > 0 && ((decimal)(c.Quantity * c.Price) / c.MarginAvailable * 100) > 80);
            }

            ViewBag.ListTitle = type == "high" ? "High Risk Clients" : "All Registered Clients";
            return PartialView("_ClientsListPartial", clients.ToList());
        }
    }
}
