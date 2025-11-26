namespace Investimentos.Domain.Entities
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty; 
        public decimal TotalInvestment { get; set; } 
        public DateTime CreatedAt { get; set; }

        public List<Position> Positions { get; set; } = new List<Position>();
    }
}