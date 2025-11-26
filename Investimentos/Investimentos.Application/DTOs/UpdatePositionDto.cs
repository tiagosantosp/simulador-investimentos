namespace Investimentos.Application.DTOs
{
    public class UpdatePositionDto
    {
        public int Quantity { get; set; }
        public decimal AveragePrice { get; set; } // Opcional: usuário pode querer corrigir o preço médio
    }
}
