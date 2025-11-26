using Investimentos.Application.DTOs;
using Investimentos.Application.Interfaces;
using Investimentos.Domain.Interface;
using Investimentos.Domain.Interfaces;

namespace Investimentos.Application.Services
{
    public class RiskAnalysisService : IRiskAnalysisService
    {
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IPriceHistoryRepository _historyRepo;

        // Taxa Livre de Risco (Selic) - No mundo real viria do Banco, aqui fixamos conforme o SeedData (12% a.a)
        private const double RISK_FREE_RATE = 0.12;

        public RiskAnalysisService(IPortfolioRepository portfolioRepo, IPriceHistoryRepository historyRepo)
        {
            _portfolioRepo = portfolioRepo;
            _historyRepo = historyRepo;
        }

        public async Task<RiskAnalysisDto> AnalyzePortfolioRiskAsync(int portfolioId)
        {
            var portfolio = await _portfolioRepo.GetByIdWithPositionsAsync(portfolioId);
            if (portfolio == null) throw new KeyNotFoundException("Portfólio não encontrado.");

            // 1. Calcular alocação por setor (Diversificação)
            var sectorAllocation = new Dictionary<string, decimal>();
            decimal totalValue = 0;

            foreach (var pos in portfolio.Positions)
            {
                if (pos.Asset == null) continue;

                decimal val = pos.Quantity * pos.Asset.CurrentPrice;
                totalValue += val;

                if (!sectorAllocation.ContainsKey(pos.Asset.Sector))
                    sectorAllocation[pos.Asset.Sector] = 0;

                sectorAllocation[pos.Asset.Sector] += val;
            }

            // Converte valores absolutos em porcentagem
            if (totalValue > 0)
            {
                foreach (var key in sectorAllocation.Keys.ToList())
                {
                    sectorAllocation[key] = Math.Round(sectorAllocation[key] / totalValue * 100, 2);
                }
            }

            // 2. Calcular Retorno Total do Portfólio
            // (Valor Atual - Investido) / Investido
            decimal totalInvested = portfolio.Positions.Sum(p => p.Quantity * p.AveragePrice);
            double totalReturn = 0;

            if (totalInvested > 0)
            {
                totalReturn = (double)((totalValue - totalInvested) / totalInvested);
            }

            // 3. Calcular Volatilidade (Simplificada: Média ponderada da volatilidade dos ativos)
            // Nota: Uma implementação "Pro" usaria matriz de covariância, mas isso é complexo demais para 1 dia.
            double portfolioVolatility = 0;

            foreach (var pos in portfolio.Positions)
            {
                // Busca histórico de preços do ativo
                var history = await _historyRepo.GetBySymbolAsync(pos.AssetSymbol);

                if (history.Any())
                {
                    // Calcula desvio padrão dos retornos diários deste ativo
                    double assetVol = CalculateStandardDeviation(history.Select(h => (double)h.Price).ToList());

                    // Peso deste ativo na carteira
                    double weight = (double)((pos.Quantity * pos.Asset!.CurrentPrice) / totalValue);

                    // Soma ponderada
                    portfolioVolatility += assetVol * weight;
                }
            }

            // 4. Calcular Sharpe Ratio
            // Sharpe = (Retorno - Selic) / Volatilidade
            double sharpeRatio = 0;
            if (portfolioVolatility > 0)
            {
                sharpeRatio = (totalReturn - RISK_FREE_RATE) / portfolioVolatility;
            }

            return new RiskAnalysisDto
            {
                PortfolioId = portfolio.Id,
                SectorAllocation = sectorAllocation,
                TotalReturn = Math.Round(totalReturn * 100, 2), // Em %
                Volatility = Math.Round(portfolioVolatility, 4),
                SharpeRatio = Math.Round(sharpeRatio, 2)
            };
        }

        // Função auxiliar de estatística (Desvio Padrão)
        private double CalculateStandardDeviation(List<double> prices)
        {
            if (prices.Count < 2) return 0;

            // 1. Calcular retornos diários (variação de um dia para o outro)
            var returns = new List<double>();
            for (int i = 1; i < prices.Count; i++)
            {
                double previous = prices[i - 1];
                double current = prices[i];
                // Retorno logarítmico é mais preciso, mas o simples (p2-p1)/p1 serve aqui
                returns.Add((current - previous) / previous);
            }

            // 2. Média dos retornos
            double avgReturn = returns.Average();

            // 3. Soma dos quadrados das diferenças
            double sumSquares = returns.Sum(r => Math.Pow(r - avgReturn, 2));

            // 4. Variância e Desvio Padrão
            double variance = sumSquares / (returns.Count - 1);

            // Multiplicamos por Sqrt(252) para anualizar a volatilidade (dias úteis no ano)
            // Ou retornamos a diária. Para o teste, vamos retornar a simples do período.
            return Math.Sqrt(variance);
        }
    }
}