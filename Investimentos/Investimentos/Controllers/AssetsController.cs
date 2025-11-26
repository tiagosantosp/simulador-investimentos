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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var asset = await _service.GetByIdAsync(id);
            if (asset == null) return NotFound("Ativo não encontrado.");
            return Ok(asset);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetBySymbol([FromQuery] string symbol)
        {
            var asset = await _service.GetBySymbolAsync(symbol);
            if (asset == null) return NotFound("Ativo não encontrado.");
            return Ok(asset);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssetDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetBySymbol), new { symbol = created.Symbol }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/price")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] decimal newPrice)
        {
            if (newPrice <= 0) return BadRequest("O preço deve ser maior que zero.");

            // Nota: Se seu service usa Symbol, precisamos buscar o ativo primeiro
            var asset = await _service.GetByIdAsync(id);
            if (asset == null) return NotFound();

            await _service.UpdatePriceAsync(asset.Symbol, newPrice);
            return NoContent();
        }
    }
}