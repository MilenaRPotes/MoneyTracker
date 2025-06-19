using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
