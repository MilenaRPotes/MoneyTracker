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

        private ObservableCollection<Income> _allIncomes = new(); // All income (unfiltered)
        public ObservableCollection<Income> Incomes { get; set; } = new(); //Those displayed in the DataGrid

        public List<int> AvailableYears { get; set; } = new();
        
        public List<MonthNameItem> AvailableMonths { get; set; } = Enumerable.Range(1, 12)
                                .Select(m => new MonthNameItem
                                {
                                    MonthNumber = m,
                                    MonthName = new DateTime(1, m, 1).ToString("MMMM", System.Globalization.CultureInfo.InvariantCulture)                               
                                }).ToList();

        public class MonthNameItem
        {
            public int MonthNumber { get; set; }
            public string MonthName { get; set; }
        }


        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
                FilterIncomesByMonth();
            }
        }

        private MonthNameItem _selectedMonth;
        public MonthNameItem SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                _selectedMonth = value;
                OnPropertyChanged(nameof(SelectedMonth));
                FilterIncomesByMonth();
            }
        }

        private void FillAvailableYears()
        {
            AvailableYears = _allIncomes
                 .Select(i => i.Date.Year)
                 .Distinct()
                 .OrderByDescending(y => y)
                 .ToList();

            OnPropertyChanged(nameof(AvailableYears));

            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            SelectedYear = AvailableYears.Contains(currentYear) ? currentYear : AvailableYears.FirstOrDefault();

            SelectedMonth = AvailableMonths.FirstOrDefault(m => m.MonthNumber == currentMonth)
                            ?? AvailableMonths.FirstOrDefault();
        }

        private void FilterIncomesByMonth()
        {
            if (SelectedMonth == null || SelectedYear == 0)
                return;

            var filtered = _allIncomes
                    .Where(i => i.Date.Year == SelectedYear && i.Date.Month == SelectedMonth.MonthNumber)
                    .OrderByDescending(i => i.Date)
                    .ToList();

            Incomes.Clear();

            foreach (var income in filtered)
                Incomes.Add(income);

            TotalIncome = filtered.Sum(i => i.Amount);
        }






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
                    Source = this.Source,
                    Date = this.Date,
                };

                db.Incomes.Add(income);
                db.SaveChanges();

               //Update filtering by month
               _allIncomes.Insert(0, income); // Save to the entire collection 
                FillAvailableYears();
                SelectedYear = income.Date.Year;
                SelectedMonth = AvailableMonths.FirstOrDefault(m => m.MonthNumber == income.Date.Month);

                FilterIncomesByMonth(); // FIlter the DataGrid to show only the current months's revenue

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
            var allIncomesFromDb = db.Incomes.OrderByDescending(i => i.Date).ToList();

            _allIncomes = new ObservableCollection<Income>(allIncomesFromDb);

            FillAvailableYears();
            FilterIncomesByMonth();

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
