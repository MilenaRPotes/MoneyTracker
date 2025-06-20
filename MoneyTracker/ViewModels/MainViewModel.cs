using System.ComponentModel;
using MoneyTracker.Data;
using MoneyTracker.Models;
using MoneyTracker.Helpers;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace MoneyTracker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
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
            set { _description = value; OnPropertyChanged(nameof(Description));}
        
        }

        private decimal _amount;
        public decimal Amount 
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(nameof(Amount));}
        }

        private string? _category;
        public string? Category 
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category));}
        }

        private DateTime _date = DateTime.Now;
        public DateTime Date 
        { 
            get => _date;
            set { _date = value; OnPropertyChanged(nameof(Date));}
        }

        public string[] Categories => new[] { "Food", "Transport", "Housing", "Entertainment", "Other" };
    
    
        public ICommand SaveCommand {  get; }

        public MainViewModel() 
        {
            SaveCommand = new RelayCommand(SaveExpense);
            LoadExpenses();
        }

        private void SaveExpense() 
        {
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
                Expenses.Insert(0, expense); 

                // Clear fields after saving
                Description = string.Empty;
                Amount = 0;
                Category = Categories.FirstOrDefault();
                Date = DateTime.Now;
            }
            catch (Exception ex) 
            {

                MessageBox.Show($"Error saving expense: {ex.Message}\n{ex.InnerException?.Message}", "Database Error");
            }
        }

        public ObservableCollection<Expense> Expenses { get; set; } = new();

        private void LoadExpenses() 
        {
            using var db = new AppDbContext();
            var allExpenses = db.Expenses.OrderByDescending(e => e.Date).ToList();
            Expenses.Clear();
            foreach (var expense in allExpenses) 
            {
                Expenses.Add(expense);
            }

        }

    }
}
