using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RPISVR_Managements.Dialog_Control;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RPISVR_Managements.ViewModel
{
    public class DeleteCommand:ICommand
    {
        public event EventHandler CanExecuteChanged;

        private XamlRoot _xamlRoot;

        // Constructor to initialize with XamlRoot
        public DeleteCommand(XamlRoot xamlRoot)
        {
            _xamlRoot = xamlRoot;
        }
  
        public bool CanExecute(object parameter)
        {
            // You can add logic to enable or disable the delete button based on certain conditions
            return true;
        }

        public async void Execute(object parameter)
        {
            // Retrieve the button or UI element that triggered the command
            var button = parameter as FrameworkElement;

            if (button == null || button.XamlRoot == null)
            {
                throw new InvalidOperationException("XamlRoot is not available. Ensure that the UI element is in the visual tree and is passed as a command parameter.");
            }

            // Show the reusable delete confirmation dialog
            var deleteDialog = new DeleteConfirmationDialog
            {
                XamlRoot = button.XamlRoot  // Set the XamlRoot dynamically
            };

            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Execute delete logic here
                DeleteItem(parameter);
            }
        }



        private void DeleteItem(object parameter)
        {
            Debug.WriteLine("Click Delete.");
        }
    }
}
