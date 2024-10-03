using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using MySql.Data.MySqlClient;
using RPISVR_Managements.Model;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;
using Windows.System;
using RPISVR_Managements.ViewModel;
using Microsoft.UI.Dispatching;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;


namespace RPISVR_Managements.ViewModel
{
    public class SharedViewModel : INotifyPropertyChanged
    {
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

        public ICommand TestConnectionCommand { get; }
        public SharedViewModel()
        {
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
            // Get the DispatcherQueue instance for the current UI thread
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            // Use TryEnqueue to update the UI
            dispatcherQueue.TryEnqueue(() =>
            {
                // Call the method to check the network connection
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
                Message = "បណ្ដាញបានតភ្ជាប់";
                Message_data = "ទិន្នន័យបានតភ្ជាប់";

            }
            else
            {
                // Network is disconnected
                Debug.WriteLine("No network connection.");            
                Message = "បណ្ដាញបានកាត់ផ្ដាច់";
                Message_data = "ទិន្នន័យបានកាត់ផ្ដាច់";


            }
        }

        //Test Database
        // Method to test the database connection
        public async void TestConnection()
        {
            while (true)
            {
                try
                {
                    using (MySqlConnection connection = _dbConnection.OpenConnection())
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            ConnectionStatus = "ទិន្នន័យភ្ជាប់ដោយជោគជ័យ";
                            
                        }
                        else
                        {
                            ConnectionStatus = "ទិន្នន័យភ្ជាប់បរាជ័យ";
                           
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
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
