using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace MoneyTracker.Models
{
    public class MonthItem
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public string DisplayName => $"{Year} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month)}";

        public override string ToString() => DisplayName;
    }
}
