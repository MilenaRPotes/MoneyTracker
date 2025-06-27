using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MoneyTracker.Data;
using MoneyTracker.Models;

namespace MoneyTracker.ViewModels
{
    public class MonthlySummaryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ObservableCollection<MonthlySummary> MonthlySummaries { get; set; } = new();

        private MonthlySummary? _selectedMonth;
        public MonthlySummary? SelectedMonth 
        { 
            get => _selectedMonth;
            set 
            { 
                _selectedMonth = value;
                OnPropertyChanged(nameof(SelectedMonth));
            
            }
        }

        public MonthlySummaryViewModel() 
        {
            LoadMonthlySummaries();
        }

        private void LoadMonthlySummaries() 
        { 
            using var db = new AppDbContext();

            var summaries = db.Expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                 .Select(g => new MonthlySummary 
                 {
                     Year = g.Key.Year,
                     Month = g.Key.Month,
                     TotalExpenses = g.Sum(e => e.Amount)

                 })
                 .ToList();

            var incomeSummaries = db.Incomes
                .GroupBy(i => new { i.Date.Year, i.Date.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalIncome = g.Sum(i => i.Amount)
                })
                .ToList();

            foreach (var income in incomeSummaries) 
            {
                var summary = summaries.FirstOrDefault(s => s.Year == income.Year && s.Month == income.Month);
                if (summary != null)
                {
                    summary.TotalIncome = income.TotalIncome;
                }
                else 
                {
                    summaries.Add(new MonthlySummary 
                    { 
                        Year = income.Year,
                        Month = income.Month,
                        TotalIncome = income.TotalIncome,
                        TotalExpenses = 0
                    });
                }

            }

            foreach(var s in summaries.OrderByDescending(s => s.Year).ThenByDescending(s => s.Month)) 
            { 
                MonthlySummaries.Add(s);
            }
        }


    }
}
