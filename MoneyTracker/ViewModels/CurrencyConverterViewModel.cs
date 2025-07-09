using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyTracker.Models;
using MoneyTracker.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MoneyTracker.Helpers;

namespace MoneyTracker.ViewModels
{
    public class CurrencyConverterViewModel : INotifyPropertyChanged
    {
        private readonly CurrencyConverter _currencyConverter;
        private CurrencyConversion _conversion = new CurrencyConversion();

        public CurrencyConverterViewModel() 
        { 
            _currencyConverter = new CurrencyConverter();
            ConvertCommand = new RelayCommand(async () => await ConvertCurrencyAsync());
        }

        public CurrencyConversion Conversion
        {
            get => _conversion;
            set { _conversion = value; OnPropertyChanged(); }
        }

        public ICommand ConvertCommand { get;  }
        public List<string> AvailableCurrencies { get; } = new List<string>
        {
            "USD", "EUR", "GBP", "AUD", "CAD", "JPY", "CHF", "CNY", "NZD", "MXN", "BRL", "COP"
        };


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async Task ConvertCurrencyAsync()
        {
            try
            {
                var result = await _currencyConverter.ConvertCurrencyAsync(Conversion.FromCurrency, Conversion.ToCurrency, Conversion.Amount);
                Conversion.ConvertedAmount = result;
                OnPropertyChanged(nameof(Conversion)); // Notify full Conversion changed
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar errores, por ejemplo mostrar un MessageBox (en el futuro podríamos hacer un servicio de manejo de errores)
                Console.WriteLine($"Error converting currency: {ex.Message}");
            }
        }
    }
}
