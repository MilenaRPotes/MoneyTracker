using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MoneyTracker.Data;
using MoneyTracker.Models;
using SkiaSharp;

namespace MoneyTracker.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<TransactionItem> RecentTransactions { get; set; } = new ObservableCollection<TransactionItem>();

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

        private ObservableCollection<ISeries> _weeklySeries = new();
        public ObservableCollection<ISeries> WeeklySeries
        {
            get => _weeklySeries;
            set { _weeklySeries = value; OnPropertyChanged(nameof(WeeklySeries)); }
        }

        private string[] _weeklyLabels = Array.Empty<string>();
        public string[] WeeklyLabels
        {
            get => _weeklyLabels;
            set { _weeklyLabels = value; OnPropertyChanged(nameof(WeeklyLabels)); }
        }

        public DashboardViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            using var db = new AppDbContext();

            TotalIncome = db.Incomes.Sum(i => i.Amount);
            TotalExpenses = db.Expenses.Sum(e => e.Amount);
            Balance = TotalIncome - TotalExpenses;

            var now = DateTime.Now;
            MonthlyIncome = db.Incomes.Where(i => i.Date.Month == now.Month && i.Date.Year == now.Year).Sum(i => i.Amount);
            MonthlyExpenses = db.Expenses.Where(e => e.Date.Month == now.Month && e.Date.Year == now.Year).Sum(e => e.Amount);

            LoadWeeklyFinancialChart();
            LoadRecentTransactions();
        }
        
        public IEnumerable<Axis> XAxes {  get; set; }
        public IEnumerable<Axis> YAxes {  get; set; }

        private void LoadWeeklyFinancialChart()
        {
            using var db = new AppDbContext();

            var now = DateTime.Now;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var firstMonday = monthStart;
            while (firstMonday.DayOfWeek != DayOfWeek.Monday)
                firstMonday = firstMonday.AddDays(-1);

            var weeks = new List<(DateTime Start, DateTime End)>();
            for (var start = firstMonday; start <= monthEnd; start = start.AddDays(7))
            {
                var end = start.AddDays(6);
                if (end > monthEnd)
                    end = monthEnd;

                weeks.Add((start, end));
            }

            var weeklyData = new List<WeeklyFinancialData>();
            int weekNumber = 1;

            foreach (var week in weeks)
            {
                var income = db.Incomes.Where(i => i.Date >= week.Start && i.Date <= week.End).Sum(i => i.Amount);
                var expense = db.Expenses.Where(e => e.Date >= week.Start && e.Date <= week.End).Sum(e => e.Amount);

                weeklyData.Add(new WeeklyFinancialData
                {
                    WeekLabel = $"Week {weekNumber} ({week.Start:dd MMM} - {week.End:dd MMM})",
                    Income = income,
                    Expense = expense
                });

                weekNumber++;
            }

            WeeklyLabels = weeklyData.Select(w => w.WeekLabel).ToArray();

            WeeklySeries = new ObservableCollection<ISeries>
            {
                new LineSeries<decimal>
                {
                    Values = weeklyData.Select(w => w.Income).ToArray(),
                    Name = "Income",
                    Stroke = new SolidColorPaint(new SKColor(34, 197, 94)),
                    Fill = null
                },
                new LineSeries<decimal>
                {
                    Values = weeklyData.Select(w => w.Expense).ToArray(),
                    Name = "Expenses",
                    Stroke = new SolidColorPaint(new SKColor(239, 68, 68)),
                    Fill = null
                }
            };

            XAxes = new List<Axis>
                    {
                        new Axis
                        {
                            Labels = WeeklyLabels.ToList(),
                            LabelsRotation = 15
                        }
                    };

            YAxes = new List<Axis>
                    {
                        new Axis
                        {
                            Labeler = Labelers.Currency
                        }
                    };
        }

        private void LoadRecentTransactions() 
        { 
            using var db = new AppDbContext();

            var expenses = db.Expenses
                .OrderByDescending(e => e.Date)
                .Take(5)
                .Select( e => new TransactionItem
                { 
                    Date = e.Date,
                    Description = e.Description,
                    Amount = -e.Amount,
                    IsIncome = false,
             
                });

            var incomes = db.Incomes
                .OrderByDescending(i => i.Date)
                .Take(5)
                .Select(i => new TransactionItem 
                { 
                    Date = i.Date,
                    Description = i.Description,
                    Amount = i.Amount,
                    IsIncome = true
                });

            //We join both, order them by date and take the 5 most recent
            var recent = expenses.Concat(incomes)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            RecentTransactions.Clear();
            foreach(var item in recent)
                RecentTransactions.Add(item);
        }
    }
}

