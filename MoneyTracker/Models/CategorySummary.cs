using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Models
{
    public class CategorySummary
    {
        public string Category {  get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public decimal Percentage { get; set; } // Calculado con respecto al total de ingresos o gastos

    }
}
