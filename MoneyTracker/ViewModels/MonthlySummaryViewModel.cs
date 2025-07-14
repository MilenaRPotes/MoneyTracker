using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MoneyTracker.Data;
using MoneyTracker.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Globalization;

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
                OnPropertyChanged(nameof(SavingRateMessage));
                OnPropertyChanged(nameof(SavingRateColor));
                UpdateChart();
            
            }
        }

            public MonthlySummaryViewModel() 
            {
                LoadMonthlySummaries();
                SelectedMonth = MonthlySummaries.FirstOrDefault();
                UpdateChart();
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

                // Merge income into existing summaries or add new months with income only
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

                // Ordenar por año y mes ascendente para comparar con mes anterior
                var orderedSummaries = summaries.OrderByDescending(s => s.Year).ThenByDescending(s => s.Month).ToList();

                for (int i = 0; i < orderedSummaries.Count; i++)
                {
                    if (i > 0)
                    {
                        var current = orderedSummaries[i];
                        var previous = orderedSummaries[i - 1];

                        if (previous.TotalExpenses > 0)
                        {
                            current.ExpenseChangeFromLastMonth =
                                (current.TotalExpenses - previous.TotalExpenses) / previous.TotalExpenses;
                        }
                        else
                        {
                            current.ExpenseChangeFromLastMonth = 0;
                        }
                    }
                    else
                    {
                        orderedSummaries[i].ExpenseChangeFromLastMonth = 0;
                    }

                    MonthlySummaries.Add(orderedSummaries[i]);
                }



            }

            //Graphis
            public IEnumerable<ISeries> ColumnSeries { get; set; }
            public Axis[] XAxes { get; set; }
            public Axis[] YAxes { get; set;}

            private void UpdateChart()
            {
                if (SelectedMonth == null)
                {
                    // Mostrar solo líneas guía visuales (sin barras reales)
                    ColumnSeries = Array.Empty<ISeries>();

                    XAxes = new[]
                    {
                        new Axis
                        {
                            MinLimit = 0,
                            MaxLimit = 1,
                            Labels = Array.Empty<string>(), // Sin etiquetas
                            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 2 },
                            TicksPaint = null,
                            TextSize = 0
                        }
                    };

                    YAxes = new[]
                    {
                        new Axis
                        {
                            MinLimit = 0,
                            MaxLimit = 1,
                            Labels = Array.Empty<string>(),
                            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 2 },
                            TicksPaint = null,
                                    TextSize = 0
                        }
                    };

                    OnPropertyChanged(nameof(ColumnSeries));
                    OnPropertyChanged(nameof(XAxes));
                    OnPropertyChanged(nameof(YAxes));
                    return;
                }

                    // Gráfico con datos reales del mes seleccionado
                    ColumnSeries = new ISeries[]
                    {
                         new ColumnSeries<decimal>
                         {
                            Name = "Income",
                            Values = new decimal[] { SelectedMonth.TotalIncome },
                            Fill = new SolidColorPaint(SKColors.LightGreen)
                         },
                        new ColumnSeries<decimal>
                        {
                            Name = "Expenses",
                            Values = new decimal[] { SelectedMonth.TotalExpenses },
                            Fill = new SolidColorPaint(SKColors.IndianRed)
                        }
                    };

                    XAxes = new[]
                    {
                        new Axis
                        {
                            Labels = new[] { "This Month", "Expenses" },
                             Labeler = value => $"${value:N0}",
                            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                            TicksPaint = new SolidColorPaint(SKColors.Gray)
                        }
                    };

                    YAxes = new[]
                    {
                        new Axis
                        {
                            MinLimit = 0,
                            Labeler = value => $"${value:N0}",
                            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                            TicksPaint = new SolidColorPaint(SKColors.Gray)
                        }
                    };

                    OnPropertyChanged(nameof(ColumnSeries));
                    OnPropertyChanged(nameof(XAxes));
                    OnPropertyChanged(nameof(YAxes));


            UpdatePieCharts();
            }

        //PieChart
        public IEnumerable<ISeries> ExpensePieSeries { get; set; } = new List<ISeries>();
        public IEnumerable<ISeries> IncomePieSeries { get; set; } = new List<ISeries>();

        private void UpdatePieCharts() 
        {
            if (SelectedMonth == null) 
            {
                ExpensePieSeries = new List<ISeries>();
                IncomePieSeries = new List<ISeries>();
                OnPropertyChanged(nameof(ExpensePieSeries));
                OnPropertyChanged(nameof (IncomePieSeries));
                return;
            }

            using var db = new AppDbContext();

            var expenseGroups = db.Expenses
                .Where(e => e.Date.Year == SelectedMonth.Year && e.Date.Month == SelectedMonth.Month)
                .GroupBy(e => e.Category)
                .Select(g => new CategorySummary
                {
                     Category = g.Key,
                     Amount = g.Sum(e => e.Amount),
                     Percentage = SelectedMonth.TotalExpenses > 0 ? g.Sum(e => e.Amount) / SelectedMonth.TotalExpenses : 0
                })
                .ToList();

            var incomeGroups = db.Incomes
                .Where(i => i.Date.Year == SelectedMonth.Year && i.Date.Month == SelectedMonth.Month)
                .GroupBy(i => i.Source)
                .Select(g => new CategorySummary
                {
                    Category = g.Key,
                    Amount = g.Sum(i => i.Amount),
                    Percentage = SelectedMonth.TotalIncome > 0 ? g.Sum(i => i.Amount) / SelectedMonth.TotalIncome : 0
                })
                .ToList();

            ExpensePieSeries = expenseGroups.Select(e => new PieSeries<decimal>
                {
                    Name = $"{e.Category} ({e.Percentage:P0})",
                    Values = new[] { e.Amount },
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 0,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => $"{point.Model:N0}",
                    ToolTipLabelFormatter = point => $"{point.Model:C2}"


            })
                .ToList();
            IncomePieSeries = incomeGroups.Select(i => new PieSeries<decimal>
            {
                Name = $"{i.Category} ({i.Percentage:P0})",
                Values = new[] { i.Amount },
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 0,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => $"{point.Model:N0}",
                ToolTipLabelFormatter = point => $"{point.Model:C2}"
            })
            .ToList();

            OnPropertyChanged(nameof(ExpensePieSeries));
            OnPropertyChanged(nameof(IncomePieSeries));
        }



        //SavingRate
        public string SavingRateMessage 
        {
            get 
            {
                if (SelectedMonth == null) return string.Empty;

                if (SelectedMonth.TotalExpenses > SelectedMonth.TotalIncome) 
                {
                    return "You spent more than you earned this month.";
                }

                return $"{SelectedMonth.SavingRate:P2} of your income was saved this month";
            }
        
        }

        public string SavingRateColor
        {
            get 
            { 
                if(SelectedMonth == null) return " #000000 ";

                return SelectedMonth.TotalExpenses > SelectedMonth.TotalIncome ? "#E53935" : "#4CAF50";
            }
        }

    }
}
