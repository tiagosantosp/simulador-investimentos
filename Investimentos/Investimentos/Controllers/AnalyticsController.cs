using Microsoft.AspNetCore.Mvc;
using Investimentos.Application.Interfaces;

namespace Investimentos.API.Controllers
{
    [ApiController]
    [Route("api/portfolios/{id}")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IRebalancingService _rebalancingService;
        private readonly IRiskAnalysisService _riskService;
        private readonly IPortfolioService _portfolioService; 


        public AnalyticsController(
            IRebalancingService rebalancingService,
            IRiskAnalysisService riskService,
            IPortfolioService portfolioService)
        {
            _rebalancingService = rebalancingService;
            _riskService = riskService;
            _portfolioService = portfolioService;
        }

        [HttpGet("rebalancing")]
        public async Task<IActionResult> GetRebalancingSuggestion(int id)
        {
            try
            {
                var result = await _rebalancingService.GetRebalancingSuggestionsAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException) { return NotFound("Portfólio não encontrado."); }
        }

        [HttpGet("risk-analysis")]
        public async Task<IActionResult> GetRiskAnalysis(int id)
        {
            try
            {
                var result = await _riskService.AnalyzePortfolioRiskAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException) { return NotFound("Portfólio não encontrado."); }
        }

        [HttpGet("performance")]
        public async Task<IActionResult> GetPerformance(int id)
        {

            var result = await _portfolioService.GetPerformanceAsync(id);

            if (result == null) return NotFound("Portfólio não encontrado.");

            return Ok(result);
        }
    }
}