namespace Investimentos.Domain.Entities
{
    public class Position
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public string AssetSymbol { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal AveragePrice { get; set; } 
        public decimal TargetAllocation { get; set; } 
        public DateTime LastTransaction { get; set; }
        public Asset? Asset { get; set; }
    }
}