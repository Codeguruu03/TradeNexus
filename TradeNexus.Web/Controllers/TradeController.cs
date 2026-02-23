using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using TradeNexus.Web.Data;
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
            var clients = await _context.Clients.Include(c => c.SubBroker).ToListAsync();
            return View(clients);
        }

        public async Task<IActionResult> CalculateRisk(int clientId)
        {
            var trades = await _context.Trades
                .Where(t => t.ClientId == clientId)
                .Select(t => new { quantity = t.Quantity, price = t.Price })
                .ToListAsync();

            var input = new
            {
                trades = trades,
                availableMargin = 500000 // In production, this would come from a ClientMargin table
            };

            string jsonInput = JsonConvert.SerializeObject(input);

            var result = _riskService.ExecuteRiskEngine(jsonInput);

            ViewBag.RiskResult = result;
            ViewBag.ClientId = clientId;

            return View("RiskResult");
        }
    }
}