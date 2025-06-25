using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MoneyTracker.Data;
using MoneyTracker.Helpers;
using MoneyTracker.Models;

namespace MoneyTracker.ViewModels
{
    public class IncomeViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Add properties for binding here 

        private string? _description;
        public string? Description 
        { 
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        private decimal _amount;
        public  decimal Amount 
        { 
            get => _amount;
            set { _amount = value; OnPropertyChanged(nameof(Amount));}
        }

        private DateTime _date = DateTime.Now;
        public DateTime Date 
        {
            get => _date;
            set { _date = value; OnPropertyChanged(nameof(Date));}
        }

        public ObservableCollection<Income> Incomes { get; set; } = new();

        private decimal _totalIncome;
        public decimal TotalIncome 
        {
            get => _totalIncome;
            set { _totalIncome = value; OnPropertyChanged(nameof(TotalIncome));}
        
        }

        public ICommand SaveIncomeCommand { get; }

        public IncomeViewModel() 
        {
            SaveIncomeCommand = new RelayCommand(SaveIncome);
            LoadIncomes();
        }

        private void SaveIncome() 
        {
            if (string.IsNullOrWhiteSpace(Description)) 
            {
                DialogService.ShowMessage("Description is required.", "Validation Error");
                return;
            }
            if (Amount <= 0) 
            {
                DialogService.ShowMessage("Amount must be greater than 0.", "Validation Error");
                return;
            }

            try 
            {
                using var db = new AppDbContext();

                var income = new Income
                {
                    Description = this.Description,
                    Amount = this.Amount,
                    Date = this.Date,
                };

                db.Incomes.Add(income);
                db.SaveChanges();

                Incomes.Insert(0, income);
                TotalIncome += income.Amount;

                Description = string.Empty;
                Amount = 0;
                Date = DateTime.Now;

            }
            catch (Exception ex) 
            {
                DialogService.ShowMessage($"Error saving income: {ex.Message}", "Database Error");
            }
        
        }

        private void LoadIncomes() 
        {
            using var db = new AppDbContext();
            var allIncomes = db.Incomes.OrderByDescending(i => i.Date).ToList();

            Incomes.Clear();
            foreach (var i in allIncomes)
                Incomes.Add(i);

            TotalIncome = allIncomes.Sum(i => i.Amount);
        }
    }
}
