using AutoMapper;
using Investimentos.Application.DTOs;
using Investimentos.Application.Interfaces;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interface;
using Investimentos.Domain.Interfaces;

namespace Investimentos.Application.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IAssetRepository _assetRepo; // Precisa validar se o ativo existe
        private readonly IMapper _mapper;

        public PortfolioService(IPortfolioRepository portfolioRepo, IAssetRepository assetRepo, IMapper mapper)
        {
            _portfolioRepo = portfolioRepo;
            _assetRepo = assetRepo;
            _mapper = mapper;
        }

        public async Task<PortfolioDto> CreateAsync(CreatePortfolioDto dto)
        {
            var portfolio = _mapper.Map<Portfolio>(dto);
            portfolio.CreatedAt = DateTime.UtcNow;
            portfolio.TotalInvestment = 0; 

            await _portfolioRepo.AddAsync(portfolio);
            return _mapper.Map<PortfolioDto>(portfolio);
        }

        public async Task<List<PortfolioDto>> GetAllByUserIdAsync(string userId)
        {
            var portfolios = await _portfolioRepo.GetAllByUserIdAsync(userId);
            // O AutoMapper já vai usar a configuração do MappingProfile para calcular os totais das posições
            return _mapper.Map<List<PortfolioDto>>(portfolios);
        }

        public async Task<PortfolioDto?> GetByIdAsync(int id)
        {
            var portfolio = await _portfolioRepo.GetByIdWithPositionsAsync(id);
            if (portfolio == null) return null;

            var dto = _mapper.Map<PortfolioDto>(portfolio);

            // CÁLCULO DE VALOR ATUAL DO PORTFÓLIO
            // Soma o valor atual de todas as posições
            dto.CurrentValue = dto.Positions.Sum(p => p.TotalValue);

            // CÁLCULO DE CUSTO TOTAL (Para saber quanto investiu)
            dto.TotalCost = portfolio.Positions.Sum(p => p.Quantity * p.AveragePrice);

            // CÁLCULO DE RENTABILIDADE TOTAL
            if (dto.TotalCost > 0)
            {
                dto.TotalReturn = ((dto.CurrentValue - dto.TotalCost) / dto.TotalCost); // Retorna decimal (ex: 0.15 para 15%)
            }

            return dto;
        }

        public async Task AddPositionAsync(int portfolioId, string symbol, int quantity, decimal price)
        {
            var portfolio = await _portfolioRepo.GetByIdWithPositionsAsync(portfolioId);
            var asset = await _assetRepo.GetBySymbolAsync(symbol);

            if (portfolio == null || asset == null) return; // Poderia lançar exceção customizada aqui

            var position = new Position
            {
                PortfolioId = portfolioId,
                AssetSymbol = symbol,
                Quantity = quantity,
                AveragePrice = price,
                LastTransaction = DateTime.UtcNow
            };

            await _portfolioRepo.AddPositionAsync(position);
        }
    }
}