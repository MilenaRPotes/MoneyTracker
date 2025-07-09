using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Models
{
    public class CurrencyConversion
    {
        public string FromCurrency {  get; set; }
        public string ToCurrency { get; set; }
        public decimal Amount { get; set; } 
        public decimal ConvertedAmount { get; set; }

    }
}
