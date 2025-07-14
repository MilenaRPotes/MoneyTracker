using System.ComponentModel;
using MoneyTracker.Data;
using MoneyTracker.Models;
using MoneyTracker.Helpers;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MoneyTracker.ViewModels
{
    public class ExpenseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Add properties for binding here 
        private string? _description;
        public string? Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }

        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(nameof(Amount)); }
        }

        private string? _category;
        public string? Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        private DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(nameof(Date)); }
        }

        public string[] Categories => new[] { "Food", "Transport", "Housing", "Entertainment", "Other" };


        public ICommand SaveCommand { get; }

        public ExpenseViewModel()
        {
            SaveCommand = new RelayCommand(SaveExpense);
            DeleteExpenseCommand = new RelayCommand(DeleteExpense, CanDelete);
            Category = Categories.FirstOrDefault();

            LoadExpenses();
        }

        //FilterExpense By Month and Year
        private ObservableCollection<Expense> _allExpenses = new(); // Todos los gastos sin filtrar

        public class MonthNameItem
        {
            public int MonthNumber { get; set; }
            public string MonthName { get; set; }
        }

        public List<int> AvailableYears { get; set; } = new();

        public List<MonthNameItem> AvailableMonths { get; set; } = Enumerable.Range(1,12)
            .Select(m => new MonthNameItem 
            { 
                MonthNumber = m,
                MonthName = new DateTime(1, m, 1).ToString("MMMM", CultureInfo.InvariantCulture)
            }).ToList();

        private int _selectedYear;
        public int SelectedYear 
        { 
            get => _selectedYear;
            set 
            { 
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
                FilterExpensesByMonth();
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
                FilterExpensesByMonth();
            }
        }

        private void FillAvailableYears()
        {
            AvailableYears = _allExpenses
                .Select(e => e.Date.Year)
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

        private void FilterExpensesByMonth()
        {
            if (SelectedMonth == null || SelectedYear == 0)
                return;

            var filtered = _allExpenses
                .Where(e => e.Date.Year == SelectedYear && e.Date.Month == SelectedMonth.MonthNumber)
                .OrderByDescending(e => e.Date)
                .ToList();

            Expenses.Clear();
            foreach (var expense in filtered)
                Expenses.Add(expense);

            TotalExpenses = filtered.Sum(e => e.Amount);
        }



        private void SaveExpense()
        {
            //Validating fields before saving 
            if (string.IsNullOrWhiteSpace(Description))
            {
                DialogService.ShowMessage("Validation Error", "Please enter a description.");
                return;
            }

            if (Amount <= 0)
            {
                DialogService.ShowMessage("Validation Error", "Amount must be greater than zero.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Category))
            {
                DialogService.ShowMessage("Validation Error", "Please select a category");
                return;
            }


            try
            {
                using var db = new AppDbContext();

                var expense = new Expense
                {
                    Description = this.Description,
                    Amount = this.Amount,
                    Category = this.Category,
                    Date = this.Date
                };

                db.Expenses.Add(expense);
                db.SaveChanges();

                //Reload the table in the view opc1
                //LoadExpenses();

                //Add directly to the observable list and Insert at top to see most recent on top opc2
                _allExpenses.Insert(0, expense); // Agrega el nuevo gasto a la colección completa

                FillAvailableYears(); // Vuelve a llenar los años disponibles si hay uno nuevo

                SelectedYear = expense.Date.Year;
                SelectedMonth = AvailableMonths.FirstOrDefault(m => m.MonthNumber == expense.Date.Month);

                FilterExpensesByMonth(); // Filtra la lista y también actualiza el total mensual


                // Clear fields after saving
                Description = string.Empty;
                Amount = 0;
                Category = Categories.FirstOrDefault() ?? "Other";
                Date = DateTime.Now;
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error saving expense: {ex.Message}\n{ex.InnerException?.Message}", "Database Error");
            }
        }

        public ObservableCollection<Expense> Expenses { get; set; } = new();

        //private void LoadExpenses()
        //{
        //    using var db = new AppDbContext();
        //    var allExpenses = db.Expenses.OrderByDescending(e => e.Date).ToList();
        //    Expenses.Clear();
        //    foreach (var expense in allExpenses)
        //    {
        //        Expenses.Add(expense);
        //    }

        //    TotalExpenses = allExpenses.Sum(e => e.Amount); // Current total

        //}

        private void LoadExpenses()
        {
            using var db = new AppDbContext();
            var allExpensesFromDb = db.Expenses.OrderByDescending(e => e.Date).ToList();

            _allExpenses = new ObservableCollection<Expense>(allExpensesFromDb); // Guarda todo sin filtrar

            FillAvailableYears();       // Llena los años disponibles
            FilterExpensesByMonth();    // Aplica filtro por año y mes actual
        }


        private decimal _totalExpense;
        public decimal TotalExpenses
        {
            get => _totalExpense;
            set
            {
                _totalExpense = value;
                OnPropertyChanged(nameof(TotalExpenses));
            }

        }

        public ICommand DeleteExpenseCommand { get; }

        private Expense? _selectedExpense;
        public Expense? SelectedExpense
        {
            get => _selectedExpense;
            set
            {
                _selectedExpense = value;
                OnPropertyChanged(nameof(SelectedExpense));
                (DeleteExpenseCommand as RelayCommand)?.RaiseCanExecuteChanged(); // Activate the delete button
            }
        }

        private void DeleteExpense()
        {
            if (SelectedExpense == null) return;

            //Confirmation before deleting 
            bool confirm = DialogService.ShowConfirmation("Are you sure you want to delete this expense?", "Delete Confirmation");
            if (!confirm) return;

            {

            }

            try
            {
                using var db = new AppDbContext();
                var expenseToDelete = db.Expenses.Find(SelectedExpense.Id);
                if (expenseToDelete != null)
                {
                    db.Expenses.Remove(expenseToDelete);
                    db.SaveChanges();

                    //Remove from the visible list in the UI 
                    Expenses.Remove(SelectedExpense);

                    //Update the total 
                    TotalExpenses -= expenseToDelete.Amount;

                    //Clear the selection 
                    SelectedExpense = null;

                }
                else
                {
                    MessageBox.Show("Expense not found in the database.", "Delete Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting expense: {ex.Message}\n{ex.InnerException?.Message}", "Database Error");
            }

        }

        private bool CanDelete()
        {
            return SelectedExpense != null;
        }
    }
}
