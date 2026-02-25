using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TradeNexus.Web.Data;
using TradeNexus.Web.Services;

namespace TradeNexus.Web.Controllers.Api
{
    [ApiController]
    [Route("api/risk")]
    public class RiskApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PythonRiskService _riskService;

        public RiskApiController(ApplicationDbContext context, PythonRiskService riskService)
        {
            _context = context;
            _riskService = riskService;
        }

        [HttpPost("{clientId:int}")]
        [Authorize(Roles = "Admin,Risk")]
        public async Task<IActionResult> CalculateRisk(int clientId)
        {
            var trades = await _context.Trades
                .Where(t => t.ClientId == clientId)
                .ToListAsync();

            if (trades == null || trades.Count == 0)
            {
                return NotFound(new { message = "No trades found for this client." });
            }

            var clientInfo = trades.FirstOrDefault();
            var availableMargin = clientInfo?.MarginAvailable ?? 500000;

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

            var response = new
            {
                clientId,
                clientName = clientInfo?.ClientName,
                tradeCount = trades.Count,
                totalExposure = trades.Sum(t => t.Quantity * t.Price),
                riskResult = result
            };

            return Ok(response);
        }
    }
}
