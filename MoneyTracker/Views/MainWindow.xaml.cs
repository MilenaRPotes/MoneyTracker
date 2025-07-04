using System.Windows;
using MoneyTracker.Views.UserControls;

namespace MoneyTracker.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new DashboardControl();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DashboardControl();
        }

        private void Income_Click(object sender, RoutedEventArgs e)

        {
            MainContent.Content = new IncomeControl(); 
        }

        private void Expenses_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ExpenseControl();
        }

        private void Summary_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new MonthlySummaryControl();
        }
    }
}
