using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Models
{
    public class MonthlySummary
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance => TotalIncome - TotalExpenses;
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        //public string Month {  get; set; } = string.Empty;  
        //public decimal Income { get; set; }
        //public decimal Expenses { get; set; }
        //public decimal Balance => Income - Expenses;
    }
}
