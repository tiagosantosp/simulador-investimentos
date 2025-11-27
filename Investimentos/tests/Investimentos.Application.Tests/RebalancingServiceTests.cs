using Investimentos.Application.Services;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Investimentos.Application.Tests
{
    public class RebalancingServiceTests
    {
        private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock;
        private readonly RebalancingService _rebalancingService;

        public RebalancingServiceTests()
        {
            _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
            _rebalancingService = new RebalancingService(_portfolioRepositoryMock.Object);
        }

        [Fact]
        public async Task GetRebalancingSuggestionsAsync_ShouldSuggestBuyAndSellCorrectly()
        {
            // Arrange: Configura um portfólio com duas posições: uma acima do peso (overweight) e outra abaixo (underweight).
            // O objetivo é verificar se o serviço sugere corretamente a VENDA do ativo acima do peso e a COMPRA do ativo abaixo.
            var portfolioId = 1;

            var assetSell = new Asset { Symbol = "SELL", CurrentPrice = 100.0m };
            var assetBuy = new Asset { Symbol = "BUY", CurrentPrice = 100.0m };
            
            var portfolio = new Portfolio
            {
                Id = portfolioId,
                Name = "Test Portfolio",
                Positions = new List<Position>
                {
                    // Posição 1: 70% do portfólio (R$700 de R$1000), mas o alvo é 50%. Deve sugerir "VENDER".
                    new Position 
                    { 
                        AssetSymbol = "SELL", 
                        Quantity = 7, 
                        TargetAllocation = 0.50m, 
                        Asset = assetSell
                    },
                    // Posição 2: 30% do portfólio (R$300 de R$1000), mas o alvo é 50%. Deve sugerir "COMPRAR".
                    new Position 
                    { 
                        AssetSymbol = "BUY", 
                        Quantity = 3,
                        TargetAllocation = 0.50m,
                        Asset = assetBuy
                    }
                }
            };
            // Valor total do portfólio = 700 + 300 = 1000. Alvo para cada um é R$500.

            _portfolioRepositoryMock.Setup(repo => repo.GetByIdWithPositionsAsync(portfolioId)).ReturnsAsync(portfolio);

            // Act: Executa o serviço de rebalanceamento.
            var result = await _rebalancingService.GetRebalancingSuggestionsAsync(portfolioId);

            // Assert: Verifica se as sugestões de compra e venda estão corretas.
            Assert.NotNull(result);
            Assert.Equal(2, result.Suggestions.Count);

            // Verifica a sugestão de Venda
            var sellSuggestion = result.Suggestions.FirstOrDefault(s => s.AssetSymbol == "SELL");
            Assert.NotNull(sellSuggestion);
            Assert.Equal("VENDER", sellSuggestion.Action);
            // Valor atual é 700. Alvo é 1000 * 0.50 = 500. Valor a mover é 200.
            Assert.Equal(200, sellSuggestion.AmountValue); 
            Assert.Equal(2, sellSuggestion.Quantity); // 200 / 100 (preço) = 2 ações

            // Verifica a sugestão de Compra
            var buySuggestion = result.Suggestions.FirstOrDefault(s => s.AssetSymbol == "BUY");
            Assert.NotNull(buySuggestion);
            Assert.Equal("COMPRAR", buySuggestion.Action);
            // Valor atual é 300. Alvo é 1000 * 0.50 = 500. Valor a mover é 200.
            Assert.Equal(200, buySuggestion.AmountValue);
            Assert.Equal(2, buySuggestion.Quantity); // 200 / 100 (preço) = 2 ações
        }
    }
}
