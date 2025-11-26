using Microsoft.AspNetCore.Mvc;
using Investimentos.Application.Interfaces;
using Investimentos.Application.DTOs;
using Investimentos.API.Models;

namespace Investimentos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfolioService _service;

        public PortfoliosController(IPortfolioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUser([FromQuery] string userId)
        {
            var portfolios = await _service.GetAllByUserIdAsync(userId);
            return Ok(portfolios);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePortfolioDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var portfolio = await _service.GetByIdAsync(id);
            if (portfolio == null) return NotFound("Portfólio não encontrado.");
            return Ok(portfolio);
        }

        [HttpPost("{id}/positions")]
        public async Task<IActionResult> AddPosition(int id, [FromBody] PositionInputModel input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _service.AddPositionAsync(id, input.Symbol, input.Quantity, input.Price);
            return Ok("Posição adicionada com sucesso.");
        }

        [HttpPut("{id}/positions/{positionId}")]
        public async Task<IActionResult> UpdatePosition(int id, int positionId, [FromBody] UpdatePositionDto dto)
        {
            try
            {
                await _service.UpdatePositionAsync(id, positionId, dto.Quantity, dto.AveragePrice);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound("Posição não encontrada."); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id}/positions/{positionId}")]
        public async Task<IActionResult> DeletePosition(int id, int positionId)
        {
            try
            {
                await _service.RemovePositionAsync(id, positionId);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound("Posição não encontrada."); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }
    }
}