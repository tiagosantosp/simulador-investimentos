using Investimentos.Application.Interfaces;
using Investimentos.Application.Mappings;
using Investimentos.Application.Services;
using Investimentos.Domain.Interfaces;
using Investimentos.Infrastructure.Data;
using Investimentos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Banco em Memória
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InvestimentosDb"));

// 2. Injetar Repositórios (Infrastructure)
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();

// 3. Injetar Services (Application)
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IRebalancingService, RebalancingService>();
builder.Services.AddScoped<IRiskAnalysisService, RiskAnalysisService>();

// 4. Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 5. Configurar Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. INICIALIZAR O SEED DATA (Carregar o JSON)
// Criamos um escopo temporário para pegar o contexto e rodar o seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    // Garante que o banco foi criado
    context.Database.EnsureCreated();

    // Chama a classe estática que criamos na Fase 3
    await DbInitializer.SeedAsync(context);
}

// Configurar Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();