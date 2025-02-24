using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using RPISVR_Managements.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace RPISVR_Managements
{
    
    public partial class App : Application
    {
        public static string LoggedInUser { get; set; } = "Guest";  // Default value
        public static string UserRole { get; set; } = "Unknown";    // Default value

        public App()
        {
            this.InitializeComponent();
        }
        public static Window MainAppWindow { get; set; }

        public static StudentViewModel SharedViewModel { get; } = new StudentViewModel();
        public string StudentID { get; set; }
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Use only one windows
            if (MainAppWindow == null)
            {
                MainAppWindow = new Login_Window();  // Create and assign the window
                MainAppWindow.Activate();  // Activate the window
            }
        }
        // Helper method to access MainWindow-specific methods
        public static MainWindow GetMainWindow()
        {
            return MainAppWindow as MainWindow;  // Safe cast to MainWindow
        }
        //public static MainWindow m_window { get; private set; }
  
    }
}
