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

        public decimal Balance => TotalIncome - TotalExpenses;

        public DashboardViewModel() 
        {
            LoadData();
        }

        private void LoadData() 
        {
            using var db = new AppDbContext();
            TotalExpenses = db.Expenses.Sum(e => e.Amount);
            TotalIncome = db.Incomes.Sum(i => i.Amount);
            OnPropertyChanged(nameof(Balance));

        }
    }
}
