using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investimentos.Application.DTOs
{
    public class AssetPerformanceDto
    {
        public string Symbol { get; set; }
        public decimal ReturnPercentage { get; set; }
    }
}
