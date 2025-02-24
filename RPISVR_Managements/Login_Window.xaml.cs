using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Windowing;
using Microsoft.UI.Input;
using RPISVR_Managements.Model;
using RPISVR_Managements.ViewModel;
using Microsoft.UI.Text;
using System.ComponentModel;
using System.Diagnostics;
using DocumentFormat.OpenXml.Drawing;
using MySql.Data.MySqlClient;
using Microsoft.UI;

namespace RPISVR_Managements
{
    
    public sealed partial class Login_Window : Window
    {
        private AppWindow m_AppWindow;

        public StudentViewModel ViewModel { get; set; }
        private DatabaseConnection _ConnectionString;

        public Login_Window()
        {
            this.InitializeComponent();

            //Connect Database
            _ConnectionString = new DatabaseConnection();
            string connectionString = _ConnectionString._connectionString;

            m_AppWindow = this.AppWindow;
            m_AppWindow.Changed += AppWindow_Changed;
            Activated += MainWindow_Activated;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            AppTitleBar.Loaded += AppTitleBar_Loaded;

            ExtendsContentIntoTitleBar = true;
            if (ExtendsContentIntoTitleBar == true)
            {
                m_AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            }

            LoadUser_Data_Combobox();

        }

        //// Method to change the theme for the entire MainWindow
        public void SetTheme(ElementTheme theme)
        {
            Application_Controls.RequestedTheme = theme;
            AppTitleBar.RequestedTheme = theme;
            Application_Controls.RequestedTheme = theme;
        }



        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // Set the initial interactive regions.
                SetRegionsForCustomTitleBar();
            }
        }

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // Update interactive regions if the size of the window changes.
                SetRegionsForCustomTitleBar();
            }
        }
        private void SetRegionsForCustomTitleBar()
        {
            // Specify the interactive regions of the title bar.

            double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

        }
        private Windows.Graphics.RectInt32 GetRect(Rect bounds, double scale)
        {
            return new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
        }


        //Foreground Title
        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                //TitleBarTextBlock.Foreground =
                //    (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
                TitleBarIcon.Visibility = Visibility.Visible;

            }
            else
            {
                //    TitleBarTextBlock.Foreground =
                //        (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
                TitleBarIcon.Visibility = Visibility.Visible;
            }
        }
        private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidPresenterChange)
            {
                switch (sender.Presenter.Kind)
                {
                    case AppWindowPresenterKind.CompactOverlay:
                        // Compact overlay - hide custom title bar
                        // and use the default system title bar instead.
                        AppTitleBar.Visibility = Visibility.Collapsed;
                        sender.TitleBar.ResetToDefault();
                        break;

                    case AppWindowPresenterKind.FullScreen:
                        // Full screen - hide the custom title bar
                        // and the default system title bar.
                        AppTitleBar.Visibility = Visibility.Collapsed;
                        sender.TitleBar.ExtendsContentIntoTitleBar = true;
                        break;

                    case AppWindowPresenterKind.Overlapped:
                        // Normal - hide the system title bar
                        // and use the custom title bar instead.
                        AppTitleBar.Visibility = Visibility.Visible;
                        sender.TitleBar.ExtendsContentIntoTitleBar = true;
                        break;

                    default:
                        // Use the default system title bar.
                        sender.TitleBar.ResetToDefault();
                        break;
                }
            }
        }
        private void SwitchPresenter(object sender, RoutedEventArgs e)
        {
            if (AppWindow != null)
            {
                AppWindowPresenterKind newPresenterKind;
                switch ((sender as Button).Name)
                {
                    case "CompactoverlaytBtn":
                        newPresenterKind = AppWindowPresenterKind.CompactOverlay;
                        break;

                    case "FullscreenBtn":
                        newPresenterKind = AppWindowPresenterKind.FullScreen;
                        break;

                    case "OverlappedBtn":
                        newPresenterKind = AppWindowPresenterKind.Overlapped;
                        break;

                    default:
                        newPresenterKind = AppWindowPresenterKind.Default;
                        break;
                }

                // If the same presenter button was pressed as the
                // mode we're in, toggle the window back to Default.
                if (newPresenterKind == AppWindow.Presenter.Kind)
                {
                    AppWindow.SetPresenter(AppWindowPresenterKind.Default);
                }
                else
                {
                    // Else request a presenter of the selected kind
                    // to be created and applied to the window.
                    AppWindow.SetPresenter(newPresenterKind);
                }
            }
        }

        private void btn_click_login(object sender, RoutedEventArgs e)
        {
            // Validate that a user is selected and a password is entered.
            if (user_login_combobox.SelectedItem == null || string.IsNullOrEmpty(password_box.Password))
            {
                if (user_login_combobox.SelectedItem == null)
                {
                    text_error_message.Text = "សូមជ្រើសរើសអ្នកប្រើប្រាស់ជាមុនសិន !";
                }
                else if (string.IsNullOrEmpty(password_box.Password))
                {
                    text_error_message.Text = "សូមបញ្ចូលលេខសម្ងាត់ជាមុនសិន !";
                }
                return;
            }

            // Get selected user and password from UI.
            string selectedUser = user_login_combobox.SelectedItem.ToString();
            string enteredPassword = password_box.Password;

            // Define your connection string and query.
            string connectionString = _ConnectionString._connectionString.ToString();
            // Parameterized query to avoid SQL injection.
            string query = "SELECT * FROM user_login WHERE user_name = @username AND user_password = @password";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Pass the parameters for username and password.
                        cmd.Parameters.AddWithValue("@username", selectedUser);
                        cmd.Parameters.AddWithValue("@password", enteredPassword);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // Successful login.
                                text_error_message.Text = "Login ជោគជ័យ";
                                text_error_message.Foreground = new SolidColorBrush(Colors.Green);
                                Debug.WriteLine("Login success for user: " + selectedUser);

                                // Proceed with login success, such as opening the main application window.

                                // After successful login:
                                string userRole = selectedUser; // This value should come from your database

                                // Create and show the main window, passing in the role
                                //MainWindow mainWindow = new MainWindow(userRole);
                                //mainWindow.Activate();
                                // Store the user details globally
                                App.LoggedInUser = selectedUser;
                                if(selectedUser == "Developer")
                                {
                                    App.UserRole = "សម្រាប់អ្នកអភិវឌ្ឈកម្មវិធី,អាចប្រើប្រាស់លើមុខងារថ្មីៗគ្រប់បែបយ៉ាង";
                                }
                                else if(selectedUser == "គណៈគ្រប់គ្រងវិទ្យាស្ថាន")
                                {
                                    App.UserRole = "សម្រាប់គណៈគ្រប់គ្រងវិទ្យាស្ថានពិនិត្យទិន្នន័យនិស្សិត, គ្រូបច្ចេកទេស, ទិន្នន័យសិក្សា គ្រប់បែបយ៉ាង";
                                }
                                else if (selectedUser == "គ្រូបច្ចេកទេស")
                                {
                                    App.UserRole = "សម្រាប់គ្រូបច្ចេកទេសធ្វើការគ្រប់គ្រងលើទិន្នន័យនិស្សិត,ការបញ្ចូលពិន្ទុ,ថ្នាក់រៀន ជាដើម";
                                }

                                // Create MainWindow instance and update App's reference
                                MainWindow mainWin = new MainWindow(userRole);
                                App.MainAppWindow = mainWin;  // 🔹 Update reference

                                mainWin.Activate();  // Show MainWindow
                                this.Close();
                            }
                            else
                            {
                                // No matching record found.
                                text_error_message.Text = "ឈ្មោះអ្នកប្រើប្រាស់ ឬលេខសម្ងាត់មិនត្រឹមត្រូវ!";
                                text_error_message.Foreground = new SolidColorBrush(Colors.Red);
                                Debug.WriteLine("Invalid login attempt for user: " + selectedUser);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Database error: " + ex.Message);
                text_error_message.Text = "មានកំហុសនៅក្នុងប្រព័ន្ធ";
                text_error_message.Foreground = new SolidColorBrush(Colors.Red);
            }
        } 
        public class User
        {
            public int ID { get; set; }
            public string Name { get; set; }

            // Override ToString to display the user's name in the combobox.
            public override string ToString()
            {
                return Name;
            }
        }

        private void LoadUser_Data_Combobox()
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT * FROM user_login";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string user_name = reader.GetString("user_name");
                        int ID = reader.GetInt32("ID");

                        User user = new User { ID = ID, Name = user_name };
                        user_login_combobox.Items.Add(user);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
}
