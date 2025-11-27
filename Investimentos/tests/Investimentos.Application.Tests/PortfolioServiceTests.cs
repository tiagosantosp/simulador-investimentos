using AutoMapper;
using Investimentos.Application.DTOs;
using Investimentos.Application.Interfaces;
using Investimentos.Application.Services;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Investimentos.Application.Tests
{
    public class PortfolioServiceTests
    {
        private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock;
        private readonly Mock<IAssetRepository> _assetRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PortfolioService _portfolioService;

        public PortfolioServiceTests()
        {
            _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
            _assetRepositoryMock = new Mock<IAssetRepository>();
            _mapperMock = new Mock<IMapper>();
            _portfolioService = new PortfolioService(_portfolioRepositoryMock.Object, _assetRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldCalculateTotalReturnCorrectly()
        {
            // Arrange: Configura o cenário de teste.
            // Cria um portfólio com uma posição e um ativo correspondente com preços definidos
            // para que possamos verificar se o cálculo de retorno total está correto.
            var portfolioId = 1;
            var assetSymbol = "TEST1";
            var quantity = 10;
            var averagePrice = 50.0m;
            var currentPrice = 60.0m;
            var totalCost = quantity * averagePrice; // Custo total = 500
            var currentValue = quantity * currentPrice; // Valor atual = 600
            var expectedReturn = (currentValue - totalCost) / totalCost; // Retorno esperado = (600 - 500) / 500 = 0.20

            var portfolio = new Portfolio
            {
                Id = portfolioId,
                Name = "Test Portfolio",
                Positions = new List<Position>
                {
                    new Position { AssetSymbol = assetSymbol, Quantity = quantity, AveragePrice = averagePrice }
                }
            };

            var asset = new Asset { Symbol = assetSymbol, CurrentPrice = currentPrice };

            var portfolioDto = new PortfolioDto
            {
                Id = portfolioId,
                Name = "Test Portfolio",
                Positions = new List<PositionDto>
                {
                    new PositionDto { AssetSymbol = assetSymbol, Quantity = quantity, AveragePrice = averagePrice, TotalValue = currentValue }
                }
            };

            // Configura os mocks para retornar os dados de teste quando os métodos do serviço forem chamados.
            _portfolioRepositoryMock.Setup(repo => repo.GetByIdWithPositionsAsync(portfolioId)).ReturnsAsync(portfolio);
            _assetRepositoryMock.Setup(repo => repo.GetBySymbolAsync(assetSymbol)).ReturnsAsync(asset);
            _mapperMock.Setup(m => m.Map<PortfolioDto>(It.IsAny<Portfolio>())).Returns(portfolioDto);


            // Act: Executa o método que está sendo testado.
            var result = await _portfolioService.GetByIdAsync(portfolioId);

            // Assert: Verifica se o resultado é o esperado.
            // Confirma que o DTO não é nulo e que o retorno total e o valor atual foram calculados corretamente.
            Assert.NotNull(result);
            Assert.Equal(expectedReturn, result.TotalReturn);
            Assert.Equal(currentValue, result.CurrentValue);
        }
    }
}
