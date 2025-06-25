using System.Windows;

namespace MoneyTracker.Views.Dialogs
{

    public partial class CustomDialog : Window
    {
        public bool Result { get; private set; } = false; 
        public CustomDialog(string message, string title = "Confirm", bool isConfirmation = true)
        {
            InitializeComponent();

           
            this.Title = title;
            DialogTitleBlock.Text = title;
            MessageText.Text = message;

            //Show buttons according to dialog type
            ConfirmationButtons.Visibility = isConfirmation ? Visibility.Visible : Visibility.Collapsed;
            InfoButton.Visibility = isConfirmation ? Visibility.Collapsed : Visibility.Visible;

        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result=false;
            DialogResult = false;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
