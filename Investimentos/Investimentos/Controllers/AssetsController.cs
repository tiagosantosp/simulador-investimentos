using Microsoft.AspNetCore.Mvc;
using Investimentos.Application.Interfaces;
using Investimentos.Application.DTOs;

namespace Investimentos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _service;

        public AssetsController(IAssetService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assets = await _service.GetAllAsync();
            return Ok(assets);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetBySymbol([FromQuery] string symbol)
        {
            var asset = await _service.GetBySymbolAsync(symbol);
            if (asset == null) return NotFound("Ativo não encontrado.");
            return Ok(asset);
        }

        [HttpPut("{symbol}/price")]
        public async Task<IActionResult> UpdatePrice(string symbol, [FromBody] decimal newPrice)
        {
            if (newPrice <= 0) return BadRequest("O preço deve ser maior que zero.");

            await _service.UpdatePriceAsync(symbol, newPrice);
            return NoContent();
        }
    }
}