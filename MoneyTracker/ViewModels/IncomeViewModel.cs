using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private string? _source;
        public string? Source 
        { 
            get => _source;
            set { _source = value; OnPropertyChanged(nameof(Source)); }
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

        private Income? _selectedIncome;
        public Income? SelectedIncome
        {
            get => _selectedIncome;
            set 
            { 
                _selectedIncome = value; 
                OnPropertyChanged(nameof(SelectedIncome));
                (DeleteIncomeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }


        public string[] Sources => new[] { "Job", "Freelance", "Gift", "Sale", "Rental", "Other" };

        public ICommand SaveIncomeCommand { get; }
        public ICommand DeleteIncomeCommand { get; }
        public IncomeViewModel() 
        {
            SaveIncomeCommand = new RelayCommand(SaveIncome);
            DeleteIncomeCommand = new RelayCommand(DeleteIncome, CanDelete);
            Source = Sources.FirstOrDefault();
            LoadIncomes();
        }

        private void SaveIncome() 
        {
            if (string.IsNullOrWhiteSpace(Description)) 
            {
                DialogService.ShowMessage("Description is required.", "Validation Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(Source))
            {
                DialogService.ShowMessage("Source is required.", "Validation Error");
                return ;
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
                    Source = this.Source,
                    Date = this.Date,
                };

                db.Incomes.Add(income);
                db.SaveChanges();

                Incomes.Insert(0, income);
                TotalIncome += income.Amount;

                //Clear fields
                Description = string.Empty;
                Source = Sources.FirstOrDefault();
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

        private void DeleteIncome() 
        {
            bool confirmed = DialogService.ShowConfirmation("Are you sure you want to delete this income?", "Delete confirmation");
            if (!confirmed) return;

            try 
            {
                using var db = new AppDbContext();
                var incomeToDelete = db.Incomes.FirstOrDefault(i => i.Id == SelectedIncome.Id);

                if(incomeToDelete != null) 
                {
                    db.Incomes.Remove(incomeToDelete);
                    db.SaveChanges();

                    Incomes.Remove(SelectedIncome);
                    TotalIncome -= incomeToDelete.Amount;
                }

                SelectedIncome = null;  
            } 
            catch (Exception ex) 
            {
                DialogService.ShowMessage($"Error deleting income: {ex.Message}", "Database Error");
            }
        }

        private bool CanDelete()
        {
            return SelectedIncome != null;
        }

    }
}
