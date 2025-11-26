using Microsoft.AspNetCore.Mvc;
using Investimentos.Application.Interfaces;

namespace Investimentos.API.Controllers
{
    [ApiController]
    [Route("api/portfolios/{id}")] 
    public class AnalyticsController : ControllerBase
    {
        private readonly IRebalancingService _rebalancingService;

        public AnalyticsController(IRebalancingService rebalancingService)
        {
            _rebalancingService = rebalancingService;
        }

        [HttpGet("rebalancing")]
        public async Task<IActionResult> GetRebalancingSuggestion(int id)
        {
            try
            {
                var result = await _rebalancingService.GetRebalancingSuggestionsAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Portfólio não encontrado.");
            }
        }
    }
}