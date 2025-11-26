namespace Investimentos.Application.DTOs
{
    public class RebalancingResultDto
    {
        public int PortfolioId { get; set; }
        public decimal TotalValue { get; set; }
        public List<RebalancingSuggestionDto> Suggestions { get; set; } = new List<RebalancingSuggestionDto>();
    }

    public class RebalancingSuggestionDto
    {
        public string AssetSymbol { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public decimal CurrentPercent { get; set; }
        public decimal TargetPercent { get; set; }
        public decimal AmountValue { get; set; }
        public int Quantity { get; set; }
        public decimal EstimatedCost { get; set; }
    }
}