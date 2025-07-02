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

        public decimal SavingRate => TotalIncome > 0 ? (TotalIncome - TotalExpenses) / TotalIncome : 0;

        public decimal ExpenseChangeFromLastMonth { get; set; } 

    }
}
