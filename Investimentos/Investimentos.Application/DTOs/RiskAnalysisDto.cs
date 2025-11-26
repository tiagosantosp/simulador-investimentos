namespace Investimentos.Application.DTOs
{
    public class RiskAnalysisDto
    {
        public int PortfolioId { get; set; }
        public double Volatility { get; set; }
        public double SharpeRatio { get; set; }
        public double TotalReturn { get; set; }
        public Dictionary<string, decimal> SectorAllocation { get; set; } = new Dictionary<string, decimal>();
    }
}