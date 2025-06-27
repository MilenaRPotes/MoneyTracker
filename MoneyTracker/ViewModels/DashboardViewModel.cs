using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MoneyTracker.Data;
using System.Linq;

namespace MoneyTracker.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name) =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private decimal _totalExpenses;
        public decimal TotalExpenses 
        { 
            get => _totalExpenses; 
            set { _totalExpenses = value; OnPropertyChanged(nameof(TotalExpenses)); }
        }

        private decimal _totalIncome;
        public decimal TotalIncome 
        {
            get => _totalIncome;
            set { _totalIncome = value; OnPropertyChanged(nameof(TotalIncome)); }
        }

        //public decimal Balance => TotalIncome - TotalExpenses;
        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            set { _balance = value; OnPropertyChanged(nameof(Balance)); }
        }

        public string CurrentMonth => DateTime.Now.ToString("MMMM yyyy");

        private decimal _monthlyIncome;
        public decimal MonthlyIncome
        {
            get => _monthlyIncome;
            set { _monthlyIncome = value; OnPropertyChanged(nameof(MonthlyIncome)); }
        }

        private decimal _monthlyExpenses;
        public decimal MonthlyExpenses
        {
            get => _monthlyExpenses;
            set { _monthlyExpenses = value; OnPropertyChanged(nameof(MonthlyExpenses)); }
        }

        public DashboardViewModel() 
        {
            LoadData();
        }

        private void LoadData() 
        {
            //using var db = new AppDbContext();
            //TotalExpenses = db.Expenses.Sum(e => e.Amount);
            //TotalIncome = db.Incomes.Sum(i => i.Amount);
            //OnPropertyChanged(nameof(Balance));

            using var db = new AppDbContext();

            TotalIncome = db.Incomes.Sum(i => i.Amount);
            TotalExpenses = db.Expenses.Sum(e => e.Amount);
            Balance = TotalIncome - TotalExpenses;

            var now = DateTime.Now;
            MonthlyIncome = db.Incomes.Where(i => i.Date.Month == now.Month && i.Date.Year == now.Year).Sum(i => i.Amount);
            MonthlyExpenses = db.Expenses.Where(e => e.Date.Month == now.Month && e.Date.Year == now.Year).Sum(e => e.Amount);

        }
    }
}
