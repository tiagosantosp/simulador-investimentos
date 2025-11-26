namespace Investimentos.Application.DTOs
{
    public class PortfolioPerformanceDto
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; }
        public decimal TotalInvested { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal TotalReturn { get; set; }
        public decimal ReturnPercentage { get; set; }
        public AssetPerformanceDto BestAsset { get; set; }
        public AssetPerformanceDto WorstAsset { get; set; }
    }
}
