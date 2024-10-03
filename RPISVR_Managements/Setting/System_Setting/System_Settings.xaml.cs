using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Mysqlx;
using RPISVR_Managements.Student_Informations.Check_Student_Informations;
using RPISVR_Managements.Student_Informations.Insert_Student_Informations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Display.Core;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using System.Net.NetworkInformation;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using MySql.Data.MySqlClient;
using RPISVR_Managements.Model;
using System.Windows.Input;
using RPISVR_Managements.ViewModel;





namespace RPISVR_Managements.Setting.System_Setting
{
    public sealed partial class System_Settings : Page,INotifyPropertyChanged
    {
        //Network Icon
        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource)); // Notify the UI of changes
            }
        }

        //Database Icon
        private ImageSource _imageSource_Database;
        public ImageSource ImageSourceDatabase
        {
            get => _imageSource_Database;
            set
            {
                _imageSource_Database = value;
                OnPropertyChanged(nameof(ImageSourceDatabase));
            }
        }


        //Text Network Connect
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }


        //Text Database Connect
        private string _message_data;
        public string Message_data
        {
            get { return _message_data; }
            set
            {
                _message_data = value;
                OnPropertyChanged(nameof(Message_data));
            }
        }

        //Database Connection
        private DatabaseConnection _dbConnection;
        private string _connectionStatus;

        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged(nameof(ConnectionStatus));
            }
        }

        public ICommand TestConnectionCommand {  get; }

        public System_Settings()
        {
            this.InitializeComponent();
            this.DataContext = this;
           
            
            // Subscribe to the network status change event
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;

            //Check the network state when the app starts
            CheckNetworkConnectivity();


            //Database
            _dbConnection = new DatabaseConnection();
            TestConnectionCommand = new RelayCommand(TestConnection);
            //TestConnectionCommand = new RelayCommand(async () => await TestConnection());
            TestConnection();



        }
        // Event handler to detect when the network status changes
        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            // This event runs on a background thread, so use Dispatcher to update UI
            DispatcherQueue.TryEnqueue(() =>
            {
                // Check network connectivity when the status changes
                CheckNetworkConnectivity();
            });
        }

        // Method to check network connectivity status
        private void CheckNetworkConnectivity()
        {
            var profiles = NetworkInformation.GetInternetConnectionProfile();

            if (profiles != null && profiles.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
            {
                // Network is connected
                Debug.WriteLine("Network is connected.");
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-wifi-100.png"));               
                Message = "បណ្ដាញបានតភ្ជាប់";
                Message_data = "ទិន្នន័យបានតភ្ជាប់";
               
            }
            else
            {
                // Network is disconnected
                Debug.WriteLine("No network connection.");
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-wi-fi-disconnected-100.png"));
                Message = "បណ្ដាញបានកាត់ផ្ដាច់";
                Message_data = "ទិន្នន័យបានកាត់ផ្ដាច់";
               

            }
        }

        //Test Database
        // Method to test the database connection
        public async void TestConnection()
        {
            while(true)
            {
                try
                {
                    using (MySqlConnection connection = _dbConnection.OpenConnection())
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            ConnectionStatus = "ទិន្នន័យភ្ជាប់ដោយជោគជ័យ";
                            ImageSourceDatabase = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-cloud-sync-100.png"));
                        }
                        else
                        {
                            ConnectionStatus = "ទិន្នន័យភ្ជាប់បរាជ័យ";
                            ImageSourceDatabase = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-delete-database-48.png"));
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    ConnectionStatus = $"Error: {ex.Message}";
                }
                // Wait for 5 seconds before checking again
                await Task.Delay(5000); // Adjust the delay based on your needs
            }
            
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTheme = (sender as ComboBox)?.SelectedItem as ComboBoxItem;

            // Get the MainWindow instance using App.GetMainWindow()
            var mainWindow = App.GetMainWindow();

            if (selectedTheme != null)
            {
                string theme = selectedTheme.Content.ToString();

                if (theme == "ពណ៌ស")
                {
                    mainWindow.SetTheme(ElementTheme.Light);
                    //App.m_window.SetTheme(ElementTheme.Light);

                }
                else if (theme == "ពណ៌ខ្មៅ")
                {
                    mainWindow.SetTheme(ElementTheme.Dark);
                    //App.m_window.SetTheme(ElementTheme.Dark);
                }
            }
        }
       

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Btn_Click_Test_NetworkAsync(object sender, RoutedEventArgs e)
        {
            CheckNetworkConnectivity();
            
        }
        //private void Btn_Click_Test_DatabaseAsync(object sender, RoutedEventArgs e)
        //{
        //    CheckNetworkConnectivity();

        //}
    }
}
