namespace Investimentos.Application.DTOs
{
    public class PositionDto
    {
        public string AssetSymbol { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentPrice { get; set; } 
        public decimal TotalValue { get; set; }   
        public decimal Performance { get; set; }  
    }
}