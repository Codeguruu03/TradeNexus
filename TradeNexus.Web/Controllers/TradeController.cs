using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using TradeNexus.Web.Data;
using TradeNexus.Web.Services;

namespace TradeNexus.Web.Controllers
{
    public class TradeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TradeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult CalculateRisk(int clientId)
        {
            var trades = _context.Trades
                .Where(t => t.ClientId == clientId)
                .Select(t => new { quantity = t.Quantity, price = t.Price })
                .ToList();

            var input = new
            {
                trades = trades,
                availableMargin = 500000
            };

            string jsonInput = JsonConvert.SerializeObject(input);

            var service = new PythonRiskService();
            var result = service.ExecuteRiskEngine(jsonInput);

            ViewBag.RiskResult = result;

            return View("RiskResult");
        }
    }
}