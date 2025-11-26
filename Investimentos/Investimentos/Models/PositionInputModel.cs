using System.ComponentModel.DataAnnotations; 
namespace Investimentos.API.Models
{
    public class PositionInputModel
    {
        [Required]
        public string Symbol { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Price { get; set; }
    }
}