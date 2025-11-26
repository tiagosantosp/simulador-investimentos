namespace Investimentos.Application.DTOs
{
    public class PortfolioDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }    
        public decimal CurrentValue { get; set; } 
        public decimal TotalReturn { get; set; }  
        public List<PositionDto> Positions { get; set; } = new List<PositionDto>();
    }
}