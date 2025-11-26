namespace Investimentos.Domain.Entities
{
    public class PriceHistory
    {
        public int Id { get; set; }
        public string AssetSymbol { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }
}