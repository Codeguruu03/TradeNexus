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

        public async Task<IActionResult> ClientTrades(int clientId)
        {
            var clientTrades = await _context.Trades
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.TradeDate)
                .ToListAsync();

            if (clientTrades == null || clientTrades.Count == 0)
            {
                ViewBag.Message = "No trades found for this client.";
                return View(new List<Trade>());
            }

            ViewBag.ClientName = clientTrades.FirstOrDefault()?.ClientName;
            ViewBag.ClientCode = clientTrades.FirstOrDefault()?.ClientCode;
            ViewBag.TotalTrades = clientTrades.Count;

            return View(clientTrades);
        }

        public async Task<IActionResult> CalculateRisk(int clientId)
        {
            var trades = await _context.Trades
                .Where(t => t.ClientId == clientId)
                .ToListAsync();

            if (trades == null || trades.Count == 0)
            {
                ViewBag.Message = "No trades found for this client.";
                ViewBag.ClientId = clientId;
                return View("RiskResult");
            }

            var clientInfo = trades.FirstOrDefault();
            var availableMargin = clientInfo?.MarginAvailable ?? 500000;

            // Prepare trade data for Python risk engine
            var tradeList = trades.Select(t => new 
            { 
                quantity = t.Quantity, 
                price = t.Price,
                symbol = t.Symbol,
                buySell = t.BuySell
            }).ToList();

            var input = new
            {
                trades = tradeList,
                availableMargin = availableMargin
            };

            string jsonInput = JsonConvert.SerializeObject(input);
            var result = _riskService.ExecuteRiskEngine(jsonInput);

            ViewBag.RiskResult = result;
            ViewBag.ClientId = clientId;
            ViewBag.ClientName = clientInfo?.ClientName;
            ViewBag.TradeCount = trades.Count;
            ViewBag.TotalExposure = trades.Sum(t => t.Quantity * t.Price);

            return View("RiskResult");
        }
    }
}