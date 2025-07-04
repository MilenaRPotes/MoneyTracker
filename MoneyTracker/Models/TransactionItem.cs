using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Models
{
    public class TransactionItem
    {  
        public DateTime Date { get; set; }
        public string Description {  get; set; }
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; } //True for income, False for expenses 

    }
}
