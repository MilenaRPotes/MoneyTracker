using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Models
{
    internal class WeeklyFinancialData
    {
        public string WeekLabel {  get; set; }
        public decimal Income {  get; set; }
        public decimal Expense { get; set; }
    }
}
