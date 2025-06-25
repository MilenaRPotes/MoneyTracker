using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyTracker.Views.Dialogs;
using System.Windows;
using Microsoft.VisualBasic;

namespace MoneyTracker.Helpers
{
    public static class DialogService
    {
        //Confirmation with Yes / No button 
        public static bool ShowConfirmation(string message, string title = "Confirm", Window owner = null)
        {
            var dialog = new CustomDialog(message, title,true);
            dialog.Owner = owner ?? Application.Current.MainWindow;
            return dialog.ShowDialog() == true;
            
        }

        // Simple message with OK button only 

        public static void ShowMessage(string message, string title = "Information", Window? owner = null) 
        {
            var dialog = new CustomDialog(message, title, false);
            dialog.Owner = owner ?? Application.Current.MainWindow; 
            dialog.ShowDialog(); 
        }

    }
}
