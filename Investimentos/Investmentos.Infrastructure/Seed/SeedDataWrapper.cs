using System.Text.Json.Serialization;

namespace Investimentos.Infrastructure.Seed
{
    public class SeedDataWrapper
    {
        public List<Domain.Entities.Asset> Assets { get; set; }
        public List<Domain.Entities.Portfolio> Portfolios { get; set; }

        public Dictionary<string, List<PricePointDto>> PriceHistory { get; set; }
    }

    public class PricePointDto
    {
        [JsonPropertyName("date")]
        public string DateString { get; set; } 
        public decimal Price { get; set; }
    }
}
