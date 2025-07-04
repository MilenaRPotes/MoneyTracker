using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MoneyTracker.Converters
{
    public class IncomeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isIncome)
            {
                return isIncome ? "💰" : "💸";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
