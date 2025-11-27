using Investimentos.Application.Services;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Investimentos.Application.Tests
{
    public class RiskAnalysisServiceTests
    {
        private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock;
        private readonly Mock<IPriceHistoryRepository> _priceHistoryRepositoryMock;
        private readonly RiskAnalysisService _riskAnalysisService;

        public RiskAnalysisServiceTests()
        {
            _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
            _priceHistoryRepositoryMock = new Mock<IPriceHistoryRepository>();
            _riskAnalysisService = new RiskAnalysisService(_portfolioRepositoryMock.Object, _priceHistoryRepositoryMock.Object);
        }

        [Fact]
        public async Task AnalyzePortfolioRiskAsync_ShouldCalculateVolatilityAndSharpeRatio()
        {
            // Arrange: Prepara os dados para o teste de análise de risco.
            // Um portfólio com uma única posição e um histórico de preços com variação são criados
            // para permitir o cálculo da volatilidade e do Sharpe Ratio.
            var portfolioId = 1;
            var assetSymbol = "RISKY";
            
            // Mock do Portfólio
            var portfolio = new Portfolio
            {
                Id = portfolioId,
                Positions = new List<Position>
                {
                    new Position 
                    { 
                        AssetSymbol = assetSymbol, 
                        Quantity = 10, 
                        AveragePrice = 100, // Custo total = 1000
                        Asset = new Asset { Symbol = assetSymbol, CurrentPrice = 102 } // Valor atual = 1020
                    }
                }
            };
            _portfolioRepositoryMock.Setup(repo => repo.GetByIdWithPositionsAsync(portfolioId)).ReturnsAsync(portfolio);

            // Mock do Histórico de Preços para o cálculo da volatilidade
            var priceHistory = new List<PriceHistory>
            {
                new PriceHistory { Price = 100.0m },
                new PriceHistory { Price = 102.0m },
                new PriceHistory { Price = 101.0m },
                new PriceHistory { Price = 103.0m },
                new PriceHistory { Price = 102.0m }
            };
            _priceHistoryRepositoryMock.Setup(repo => repo.GetBySymbolAsync(assetSymbol)).ReturnsAsync(priceHistory);

            // Cálculos manuais para as asserções
            // A lógica detalhada do cálculo do desvio padrão está no histórico do agente.
            var expectedVolatility = 0.01711;
            // Retorno Total = (1020 - 1000) / 1000 = 0.02
            // Taxa Livre de Risco (Selic) = 0.12 (constante no serviço)
            // Sharpe Ratio = (Retorno - Selic) / Volatilidade = (0.02 - 0.12) / 0.01711 = -5.844
            var expectedSharpeRatio = -5.84; 

            // Act: Executa o método de análise de risco.
            var result = await _riskAnalysisService.AnalyzePortfolioRiskAsync(portfolioId);

            // Assert: Verifica se a volatilidade e o Sharpe Ratio foram calculados corretamente.
            // Usamos uma precisão para lidar com pequenas diferenças em cálculos de ponto flutuante.
            Assert.NotNull(result);
            Assert.Equal(expectedVolatility, result.Volatility, 3); // Precisão de 3 casas decimais
            Assert.Equal(expectedSharpeRatio, result.SharpeRatio, 2); // Precisão de 2 casas decimais
        }
    }
}
