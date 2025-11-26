using Investimentos.Application.DTOs;
using Investimentos.Application.Interfaces;
using Investimentos.Domain.Interface;
using Investimentos.Domain.Interfaces;

namespace Investimentos.Application.Services
{
    public class RebalancingService : IRebalancingService
    {
        private readonly IPortfolioRepository _portfolioRepo;

        // Constantes de Regra de Negócio
        private const decimal MIN_TRANSACTION_VALUE = 100.00m; // Mínimo R$ 100 pra operar
        private const decimal TRANSACTION_COST_PERCENT = 0.003m; // 0.3% de taxa
        private const decimal DEVIATION_THRESHOLD = 0.05m; // 5% de desvio mínimo para agir

        public RebalancingService(IPortfolioRepository portfolioRepo)
        {
            _portfolioRepo = portfolioRepo;
        }

        public async Task<RebalancingResultDto> GetRebalancingSuggestionsAsync(int portfolioId)
        {
            // 1. Busca os dados brutos (incluindo Asset para ter o preço atual)
            var portfolio = await _portfolioRepo.GetByIdWithPositionsAsync(portfolioId);

            if (portfolio == null)
                throw new KeyNotFoundException("Portfólio não encontrado");

            // 2. Calcula valor total atual da carteira
            // Soma: (Qtd * Preço Atual do Ativo)
            var totalPortfolioValue = portfolio.Positions
                .Sum(p => p.Quantity * (p.Asset?.CurrentPrice ?? 0));

            var result = new RebalancingResultDto
            {
                PortfolioId = portfolio.Id,
                TotalValue = totalPortfolioValue
            };

            if (totalPortfolioValue == 0) return result; // Evita divisão por zero

            // 3. Analisa cada posição
            foreach (var position in portfolio.Positions)
            {
                if (position.Asset == null) continue;

                decimal currentPrice = position.Asset.CurrentPrice;
                decimal positionValue = position.Quantity * currentPrice;

                // Qual a fatia atual da pizza? (Ex: 0.25 = 25%)
                decimal currentAllocation = positionValue / totalPortfolioValue;

                // Qual a meta? (Ex: 0.20 = 20%)
                decimal targetAllocation = position.TargetAllocation;

                // Diferença (Ex: 0.20 - 0.25 = -0.05 ou -5%)
                decimal deviation = targetAllocation - currentAllocation;

                // Quanto dinheiro isso representa? (Ex: -5% de 100k = -5.000)
                decimal amountToMove = deviation * totalPortfolioValue;

                var suggestion = new RebalancingSuggestionDto
                {
                    AssetSymbol = position.AssetSymbol,
                    CurrentPercent = Math.Round(currentAllocation * 100, 2),
                    TargetPercent = Math.Round(targetAllocation * 100, 2),
                    EstimatedCost = 0
                };

                // 4. Lógica de Decisão (Comprar ou Vender?)

                // Se a diferença for muito pequena (ex: mudar R$ 50,00), ignora.
                // Math.Abs tira o sinal negativo para comparar tamanho absoluto.
                if (Math.Abs(amountToMove) < MIN_TRANSACTION_VALUE)
                {
                    suggestion.Action = "MANTER";
                    suggestion.AmountValue = 0;
                    suggestion.Quantity = 0;
                }
                else
                {
                    // Arredonda para baixo para não comprar fração de ação (int)
                    int quantityToMove = (int)(Math.Abs(amountToMove) / currentPrice);

                    if (amountToMove > 0) // Preciso comprar (Target > Atual)
                    {
                        suggestion.Action = "COMPRAR";
                        suggestion.AmountValue = Math.Abs(amountToMove);
                        suggestion.Quantity = quantityToMove;
                    }
                    else // Preciso vender (Target < Atual)
                    {
                        suggestion.Action = "VENDER";
                        suggestion.AmountValue = Math.Abs(amountToMove);
                        suggestion.Quantity = quantityToMove;
                    }

                    // Calcula o custo da transação (0.3%)
                    suggestion.EstimatedCost = suggestion.AmountValue * TRANSACTION_COST_PERCENT;
                }

                result.Suggestions.Add(suggestion);
            }

            return result;
        }
    }
}