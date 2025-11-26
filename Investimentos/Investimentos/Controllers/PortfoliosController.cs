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

        [HttpGet]
        public async Task<IActionResult> GetByUser([FromQuery] string userId)
        {
            var portfolios = await _service.GetAllByUserIdAsync(userId);
            return Ok(portfolios);
        }

        [HttpPost("{id}/positions")]
        public async Task<IActionResult> AddPosition(int id, [FromBody] PositionInputModel input)
        {
            // Nota: PositionInputModel é uma classe simples criada aqui ou no DTO para receber os dados
            await _service.AddPositionAsync(id, input.Symbol, input.Quantity, input.Price);
            return NoContent();
        }
    }

    // Classe simples para receber o POST da posição
   
}