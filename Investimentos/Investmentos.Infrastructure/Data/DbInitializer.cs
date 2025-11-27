using Investimentos.Domain.Entities;
using Investimentos.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Investimentos.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Se já tiver dados, não faz nada
            if (await context.Assets.AnyAsync()) return;

            var seedFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Seed", "SeedData.json");

            if (!File.Exists(seedFilePath)) return;

            var jsonContent = await File.ReadAllTextAsync(seedFilePath);

            // Configuração para ler Case Insensitive 
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var seedData = JsonSerializer.Deserialize<SeedDataWrapper>(jsonContent, options);

            if (seedData == null) return;

            // 1. Salvar Assets
            if (seedData.Assets != null)
            {
                await context.Assets.AddRangeAsync(seedData.Assets);
                await context.SaveChangesAsync(); 
            }

            // 2. Salvar Portfolios 
            if (seedData.Portfolios != null)
            {
                await context.Portfolios.AddRangeAsync(seedData.Portfolios);
            }

            // 3. Salvar Price History (Convertendo do Dicionário para Entidade )
            if (seedData.PriceHistory != null)
            {
                var historyEntities = new List<PriceHistory>();

                foreach (var entry in seedData.PriceHistory)
                {
                    var symbol = entry.Key; // Ex: PETR4
                    foreach (var point in entry.Value)
                    {
                        if (DateTime.TryParse(point.DateString, out var date))
                        {
                            historyEntities.Add(new PriceHistory
                            {
                                AssetSymbol = symbol,
                                Date = date,
                                Price = point.Price
                            });
                        }
                    }
                }
                await context.PriceHistories.AddRangeAsync(historyEntities);
            }

            await context.SaveChangesAsync();
        }
    }
}
