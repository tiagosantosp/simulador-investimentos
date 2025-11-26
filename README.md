# 📈 Investment Portfolio - Backend API

Sistema de gestão de carteiras de investimentos, focado em **Valuation em tempo real**, **algoritmos de rebalanceamento inteligente** e **análise de risco** (Sharpe Ratio/Volatilidade).

---

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** com separação em camadas, garantindo desacoplamento e testabilidade:

```
Investimentos/
├── Investimentos.API/             # Camada de apresentação (Controllers, Configs)
├── Investimentos.Application/     # Camada de aplicação (Services, DTOs, Mappers)
├── Investimentos.Domain/          # Camada de domínio (Entities, Interfaces)
├── Investimentos.Infrastructure/  # Camada de infraestrutura (EF Core, Repositories, Seed)
└── Investimentos.Tests/           # Testes unitários (xUnit)
```

### Padrões Utilizados

- **Repository Pattern** - Abstração do acesso a dados
- **Dependency Injection** - Injeção de dependências nativa do .NET
- **DTO Pattern** - Transferência de dados sem expor Entidades
- **Service Layer** - Lógica de negócios complexa (Financeira)
- **Data Seeding** - Carga inicial automática de dados

---

## 🛠️ Tecnologias

### Stack Principal

- **.NET 8** - Framework
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core 8** - ORM
- **In-Memory Database** - Banco de dados (para alta performance em testes)
- **AutoMapper** - Mapeamento Objeto-Objeto
- **Swagger/OpenAPI** - Documentação da API

### Pacotes NuGet

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.x.x" />
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

---

## 📊 Modelagem do Banco de Dados

### Entidades

#### **Asset (Ativo)**
Representa um ativo financeiro (Ação, FII, etc).

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | int | Identificador único |
| Symbol | varchar(10) | Código do ativo (ex: PETR4) |
| Name | varchar(100) | Nome da empresa |
| Type | varchar(20) | Tipo (Stock, REIT, etc) |
| Sector | varchar(50) | Setor de atuação (Energy, Financial) |
| CurrentPrice | decimal | Preço de mercado atual |
| LastUpdated | datetime | Última atualização de preço |

#### **Portfolio (Carteira)**
Agrupador de posições de um usuário.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | int | Identificador único |
| Name | varchar(100) | Nome da carteira |
| UserId | varchar(50) | ID do usuário dono |
| TotalInvestment | decimal | Custo total de aquisição |
| CreatedAt | datetime | Data de criação |

#### **Position (Posição)**
Vínculo entre Carteira e Ativo.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | int | Identificador único |
| PortfolioId | int | FK para Portfolio |
| AssetSymbol | varchar(10) | Código do ativo |
| Quantity | int | Quantidade de cotas |
| AveragePrice | decimal | Preço médio de compra |
| TargetAllocation | decimal | Meta de alocação (ex: 0.20 para 20%) |
| LastTransaction | datetime | Data da última movimentação |

### Relacionamentos

- **Portfolio** 1:N **Position**
- **Position** N:1 **Asset** (via Symbol)

---

## 🧠 Algoritmos Financeiros

### ⚖️ Rebalanceamento de Carteira

O sistema analisa o desvio entre a **Alocação Atual** e a **Alocação Meta** (Target).

**Regras de Negócio:**
1. Calcula a % atual de cada ativo na carteira.
2. Compara com a `TargetAllocation`.
3. Gera ordem de **COMPRA** se Atual < Meta.
4. Gera ordem de **VENDA** se Atual > Meta.
5. **Threshold:** Ignora movimentações financeiras menores que R$ 100,00.
6. **Custos:** Considera taxa estimada de 0.3% por transação.

### 📉 Análise de Risco

Métricas estatísticas baseadas no histórico de preços.

- **Volatilidade:** Desvio padrão dos retornos diários do ativo.
- **Sharpe Ratio:** `(Retorno Carteira - Selic) / Volatilidade`.

---

## 🔌 API Endpoints

**Base URL:** `https://localhost:7153/api`

### 📂 Assets (Ativos)

#### `GET /api/assets`
Lista todos os ativos disponíveis no mercado.

**Response 200:**
```json
[
  {
    "symbol": "PETR4",
    "name": "Petrobras PN",
    "sector": "Energy",
    "currentPrice": 35.50
  },
  {
    "symbol": "VALE3",
    "name": "Vale ON",
    "sector": "Mining",
    "currentPrice": 65.20
  }
]
```

#### `PUT /api/assets/{symbol}/price`
Atualiza o preço de mercado de um ativo (simula volatilidade).

**Request:**
```json
38.90
```

**Response 204:** No Content

---

### 💼 Portfolios

#### `GET /api/portfolios/{id}`
Obtém detalhes da carteira com cálculo de rentabilidade em tempo real.

**Response 200:**
```json
{
  "id": 1,
  "name": "Portfólio Conservador",
  "totalCost": 98000.00,
  "currentValue": 105400.00,
  "totalReturn": 0.0755,
  "positions": [
    {
      "assetSymbol": "PETR4",
      "quantity": 500,
      "averagePrice": 30.00,
      "currentPrice": 35.50,
      "totalValue": 17750.00,
      "performance": 0.1833
    }
  ]
}
```

#### `POST /api/portfolios`
Cria uma nova carteira vazia.

**Request:**
```json
{
  "name": "Carteira Aposentadoria",
  "userId": "user-999"
}
```

---

### 📊 Analytics (Inteligência)

#### `GET /api/portfolios/{id}/rebalancing`
Executa o algoritmo de sugestão de rebalanceamento.

**Response 200:**
```json
{
  "portfolioId": 1,
  "totalValue": 105400.00,
  "suggestions": [
    {
      "assetSymbol": "PETR4",
      "action": "VENDER",
      "currentPercent": 25.5,
      "targetPercent": 20.0,
      "amountValue": 5500.00,
      "quantity": 154,
      "estimatedCost": 16.50
    },
    {
      "assetSymbol": "MGLU3",
      "action": "COMPRAR",
      "currentPercent": 5.0,
      "targetPercent": 10.0,
      "amountValue": 5200.00,
      "quantity": 594,
      "estimatedCost": 15.60
    }
  ]
}
```

#### `GET /api/portfolios/{id}/risk-analysis`
Retorna métricas de risco e diversificação.

**Response 200:**
```json
{
  "portfolioId": 1,
  "volatility": 0.0245,
  "sharpeRatio": 1.45,
  "totalReturn": 7.55,
  "sectorAllocation": {
    "Energy": 25.5,
    "Mining": 15.0,
    "Financial": 30.0
  }
}
```

---

## 🚀 Como Executar

### Pré-requisitos

- **.NET 8 SDK**
- **Visual Studio 2022** ou **VS Code**

### Passo 1: Clonar o Repositório

```bash
git clone <repository-url>
cd Investimentos
```

### Passo 2: Executar a API

O projeto utiliza **Banco em Memória**, então não é necessário configurar Connection Strings ou Docker. Basta rodar:

```bash
dotnet run --project Investimentos.API
```

### Passo 3: Acessar Swagger

O navegador abrirá automaticamente. Caso contrário, acesse:

```
https://localhost:7153/swagger
```

---

## 💾 Seed Data (Dados Iniciais)

Ao iniciar a aplicação, o sistema carrega automaticamente o arquivo `SeedData.json` contendo:

- **15 Ativos** reais do Ibovespa.
- **3 Portfólios** de exemplo (Conservador, Crescimento, Dividendos).
- **Histórico de Preços** para cálculo de volatilidade.

Isso permite testar endpoints de Analytics imediatamente sem necessidade de cadastros manuais.

---

## 🛡️ Tratamento de Erros

### Validações

✅ Preços e Quantidades não podem ser negativos.  
✅ Símbolos de ativos são verificados antes da compra.  
✅ ID de Portfólio inexistente retorna **404**.

### Exceções

Middleware global ou Try/Catch nos Controllers garantem retornos HTTP adequados (400, 404, 500) com mensagens amigáveis.

---

## 📚 Estrutura de Pastas Completa

```
Investimentos/
│
├── Investimentos.API/
│   ├── Controllers/
│   │   ├── AnalyticsController.cs
│   │   ├── AssetsController.cs
│   │   └── PortfoliosController.cs
│   ├── Models/
│   │   └── PositionInputModel.cs
│   └── Program.cs
│
├── Investimentos.Application/
│   ├── DTOs/
│   │   ├── AssetDto.cs
│   │   ├── PortfolioDto.cs
│   │   ├── RebalancingResultDto.cs
│   │   └── RiskAnalysisDto.cs
│   ├── Interfaces/
│   │   ├── IAssetService.cs
│   │   ├── IPortfolioService.cs
│   │   ├── IRebalancingService.cs
│   │   └── IRiskAnalysisService.cs
│   ├── Services/
│   │   ├── AssetService.cs
│   │   ├── PortfolioService.cs
│   │   ├── RebalancingService.cs
│   │   └── RiskAnalysisService.cs
│   └── Mappings/
│       └── MappingProfile.cs
│
├── Investimentos.Domain/
│   ├── Entities/
│   │   ├── Asset.cs
│   │   ├── Portfolio.cs
│   │   ├── Position.cs
│   │   └── PriceHistory.cs
│   └── Interfaces/
│       └── (Interfaces de Repositórios)
│
├── Investimentos.Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── DbInitializer.cs
│   ├── Repositories/
│   │   ├── AssetRepository.cs
│   │   ├── PortfolioRepository.cs
│   │   └── PriceHistoryRepository.cs
│   └── Seed/
│       └── SeedData.json
│
└── README.md
```

---

## 👨‍💻 Autor

Desenvolvido como **case técnico** para Vaga de Desenvolvedor Backend .NET.

**Foco:** Clean Architecture, Algoritmos e Boas Práticas.

---

## 📄 Licença

Este projeto foi desenvolvido para fins educacionais e de avaliação técnica.