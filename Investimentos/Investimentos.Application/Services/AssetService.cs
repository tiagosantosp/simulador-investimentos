using AutoMapper;
using Investimentos.Application.DTOs;
using Investimentos.Application.Interfaces;
using Investimentos.Domain.Interfaces;

namespace Investimentos.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _repository;
        private readonly IMapper _mapper;

        public AssetService(IAssetRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<AssetDto>> GetAllAsync()
        {
            var assets = await _repository.GetAllAsync();
            return _mapper.Map<List<AssetDto>>(assets);
        }

        public async Task<AssetDto?> GetBySymbolAsync(string symbol)
        {
            var asset = await _repository.GetBySymbolAsync(symbol);
            return _mapper.Map<AssetDto>(asset);
        }

        public async Task UpdatePriceAsync(string symbol, decimal newPrice)
        {
            var asset = await _repository.GetBySymbolAsync(symbol);
            if (asset != null)
            {
                asset.CurrentPrice = newPrice;
                asset.LastUpdated = DateTime.UtcNow;
                await _repository.UpdateAsync(asset);
            }
        }
    }
}