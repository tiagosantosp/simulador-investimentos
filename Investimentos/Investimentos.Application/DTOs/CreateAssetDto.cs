namespace Investimentos.Application.DTOs
{
    public class CreateAssetDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Stock";
        public string Sector { get; set; } = "General";
        public decimal CurrentPrice { get; set; }
    }
}
