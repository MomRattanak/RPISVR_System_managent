using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MySql.Data.MySqlClient;
using RPISVR_Managements.Model;
using RPISVR_Managements.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace RPISVR_Managements.Home
{
    
    public sealed partial class HomePage : Page
    {
        public StudentViewModel ViewModel { get; set; }
        private DatabaseConnection _ConnectionString;

        //Refresh Timer
        private Timer _refreshTimer;
        private const int RefreshInterval = 10000;
        private DispatcherTimer _dispatcherTimer;

        public HomePage()
        {
            this.InitializeComponent();

            ViewModel = new StudentViewModel();
            this.DataContext = new StudentViewModel();

            //ErrorMessage
            var viewModel = (StudentViewModel)this.DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;

            //Connect Database
            _ConnectionString = new DatabaseConnection();
            string connectionString = _ConnectionString._connectionString;

            StartAutoRefresh();
        }

        private void StartAutoRefresh()
        {
            // Initialize the timer to refresh data periodically
            _refreshTimer = new Timer(RefreshData, null, 0, RefreshInterval);
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(10); // Set to desired interval
            _dispatcherTimer.Tick += (sender, e) => Load_Total_Class_By_Year(selectedYear);
            _dispatcherTimer.Tick += (sender, e) => Load_Working_Class_By_Year(selectedYear, Class_State_Working);
            _dispatcherTimer.Tick += (sender, e) => Load_Complete_Class_By_Year(selectedYear, Class_State_Complete);
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent(selectedYear);
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent_Bac_Count(selectedYear);
            _dispatcherTimer.Tick += (sender,e) => LoadTotalStudent_Associate_Count(selectedYear);
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent_C3_Count(selectedYear);
            _dispatcherTimer.Tick += (senser, e) => LoadTotalStudent_C2_Count(selectedYear);
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent_C1_Count(selectedYear);
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent_1_5M_Count(selectedYear);
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStu_byStudyTime(selectedYear);
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStu_DataFromDatabase(selectedYear);
            _dispatcherTimer.Start();
        }
        private void RefreshData(object state)
        {
            // Fetch and update data on the main thread
            DispatcherQueue.TryEnqueue(() =>
            {
                //Class
                Load_Total_Class_By_Year(selectedYear);
                Load_Working_Class_By_Year(selectedYear, Class_State_Working);
                Load_Complete_Class_By_Year(selectedYear, Class_State_Complete);

                //Student
                LoadTotalStudent(selectedYear);
                LoadTotalStudent_Bac_Count(selectedYear);
                LoadTotalStudent_Associate_Count(selectedYear);
                LoadTotalStudent_C3_Count(selectedYear);
                LoadTotalStudent_C2_Count(selectedYear);
                LoadTotalStudent_C1_Count(selectedYear);
                LoadTotalStudent_1_5M_Count(selectedYear);

                //
                LoadTotalStu_byStudyTime(selectedYear);
                LoadTotalStu_DataFromDatabase(selectedYear);
            });
        }


        public string selectedYear;
        public string selectedSkill;
        public string selectedLevel;
        public string Class_State_Working = "ដំណើរការ";
        public string Class_State_Complete = "បញ្ចប់";

        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = (StudentViewModel)sender;

            // Check if the changed property is ErrorMessage
            if (e.PropertyName == nameof(viewModel.ErrorMessage) && !string.IsNullOrEmpty(viewModel.ErrorMessage))
            {

                var dialog = new ContentDialog
                {
                    Title = new TextBlock
                    {
                        Text = "ការជូនដំណឹង",
                        FontSize = 18,
                        FontFamily = new FontFamily("Khmer OS Battambang"),
                        FontWeight = FontWeights.Bold,
                    },
                    CloseButtonText = "យល់ព្រម",
                    XamlRoot = this.XamlRoot,
                    RequestedTheme = ElementTheme.Default
                };
                //
                // Create the Image control and bind its Source property
                Image errorImage = new Image
                {
                    Width = 30,
                    Height = 30
                };

                // Bind the Image.Source to the ErrorImageSource in the ViewModel
                Binding imageBinding = new Binding
                {
                    Path = new PropertyPath("ErrorImageSource"),
                    Source = viewModel, // The ViewModel is the source of the binding
                    Mode = BindingMode.TwoWay
                };
                errorImage.SetBinding(Image.SourceProperty, imageBinding);

                // Create the TextBlock control and bind its Text property
                TextBlock messageTextBlock = new TextBlock
                {
                    FontSize = 12,
                    FontFamily = new FontFamily("Khmer OS Battambang"),
                    TextWrapping = TextWrapping.Wrap
                };

                // Bind the TextBlock.Text to the ErrorMessage in the ViewModel
                Binding textBinding = new Binding
                {
                    Path = new PropertyPath("ErrorMessage"),
                    Source = viewModel, // The ViewModel is the source of the binding
                    Mode = BindingMode.TwoWay
                };
                messageTextBlock.SetBinding(TextBlock.TextProperty, textBinding);

                // Create the StackPanel to hold the Image and TextBlock
                StackPanel contentStackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10,
                    Children =
                    {
                        errorImage,   // Add the Image control to the StackPanel
                        messageTextBlock // Add the TextBlock control to the StackPanel
                    }
                };

                // Set the ContentDialog's content to the StackPanel
                dialog.Content = contentStackPanel;

                dialog.CloseButtonStyle = (Style)Application.Current.Resources["CustomDialogButtonStyle"];
                await dialog.ShowAsync();
                Debug.WriteLine(viewModel.ErrorImageSource);
            }
        }

        private void Study_Year_Selected_Class_State(object sender, SelectionChangedEventArgs e)
        {
            if (Class_StudyYear_Count_State.SelectedItem != null)
            {
                var selectedItem = Class_StudyYear_Count_State.SelectedItem;
                selectedYear = (selectedItem as Student_Info)?.Stu_StudyYear; // Replace "YourItemType" with the type of items in your ItemsSource

                Debug.WriteLine($"Selected Year: {selectedYear}");
                Load_Total_Class_By_Year(selectedYear);
                Load_Working_Class_By_Year(selectedYear, Class_State_Working);
                Load_Complete_Class_By_Year(selectedYear, Class_State_Complete);

                //
                LoadTotalStudent(selectedYear);
                LoadTotalStudent_Bac_Count(selectedYear);
                LoadTotalStudent_Associate_Count(selectedYear);
                LoadTotalStudent_C3_Count(selectedYear);
                LoadTotalStudent_C2_Count(selectedYear);
                LoadTotalStudent_C1_Count(selectedYear);
                LoadTotalStudent_1_5M_Count(selectedYear);

                //
                LoadTotalStu_byStudyTime(selectedYear);
                LoadTotalStu_DataFromDatabase(selectedYear);
            }
        }

        //Load Total Class
        private void Load_Total_Class_By_Year(string selectedYear)
        {
            Debug.WriteLine($"Loading total classes for the year: {selectedYear}");

            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(class_id) AS TotalClass FROM classes WHERE class_in_study_year = @class_in_study_year";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh Load_Total_Class_By_Year Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@class_in_study_year", selectedYear);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int total_class_count = reader.GetInt32("TotalClass");

                        Total_Class_Count_Select_Year.Text = total_class_count.ToString();
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}");
                }
            }
        }

        //Load Class Working
        private void Load_Working_Class_By_Year(string selectedYear, string Class_State_Working)
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(class_id) AS TotalClass_Working FROM classes WHERE class_in_study_year = @class_in_study_year && Class_State = @Class_State";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh Load_Working_Class_By_Year Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@class_in_study_year", selectedYear);
                    cmd.Parameters.AddWithValue("@Class_State", Class_State_Working);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int total_class_count_working = reader.GetInt32("TotalClass_Working");

                        Total_Class_Working.Text = total_class_count_working.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}");
                }
            }
        }

        //Load Class Complete
        private void Load_Complete_Class_By_Year(string selectedYear, string Class_State_Complete)
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(class_id) AS TotalClass_Complete FROM classes WHERE class_in_study_year = @class_in_study_year && Class_State = @Class_State";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh Load_Complete_Class_By_Year Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@class_in_study_year", selectedYear);
                    cmd.Parameters.AddWithValue("@Class_State", Class_State_Complete);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int total_class_count_complete = reader.GetInt32("TotalClass_Complete");

                        Total_Class_Complete.Text = total_class_count_complete.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}");
                }
            }
        }

        //Graph total student
        private void LoadTotalStudent(string selectedYear)
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_id) AS student_count, COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_count, COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_count FROM student_infomations WHERE stu_study_year = @stu_study_year; ";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Connection Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@stu_study_year", selectedYear);

                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int total_stu_count = reader.GetInt32("student_count");
                            int total_female_count = reader.GetInt32("female_student_count");
                            int total_male_count = reader.GetInt32("male_student_count");

                            Total_Student_Count.Text = "សិស្សនិស្សិតសរុប " + total_stu_count.ToString() + " នាក់";
                            Total_Student_Female_Count.Text = "ស្រី " + total_female_count.ToString() + " នាក់";
                            Total_Student_Male_Count.Text = "ប្រុស " + total_male_count.ToString() + " នាក់";
                        }
                    }               
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., show a message to the user)
                    Debug.WriteLine("Error LoadTotalStudent: " + ex.Message);
                }
            }
        }

        //Graph Bachelor student
        private void LoadTotalStudent_Bac_Count(string selectedYear)
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_bac_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_bac_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_bac_count FROM student_infomations WHERE stu_education_level IN ('បរិញ្ញាបត្រ', 'សញ្ញាបត្រវិស្វករ', 'បរិញ្ញាបត្របច្ចេកវិទ្យា') AND stu_study_year = @stu_study_year;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_Bac_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@stu_study_year", selectedYear);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            int total_stu_bac_count = reader.GetInt32("Total_student_bac_count");
                            int total_female_bac_count = reader.GetInt32("female_student_bac_count");
                            int total_male_bac_count = reader.GetInt32("male_student_bac_count");

                            Total_Stu_Count_Bachelor.Text = "កម្រិតបរិញ្ញាបត្រ " + total_stu_bac_count.ToString() + " នាក់";
                            Total_Stu_Female_Count_Bachelor.Text = "ស្រី " + total_female_bac_count.ToString() + " នាក់";
                            Total_Student_Male_Count_Bachelor.Text = "ប្រុស " + total_male_bac_count.ToString() + " នាក់";
                        }
                    }          
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        //Graph 1.5M Student
        private void LoadTotalStudent_1_5M_Count(string selectedYear)
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_1_5M_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_1_5M_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_1_5M_count FROM student_infomations WHERE stu_education_level IN ('ជំនាញបច្ចេកទេស និងវិជ្ជាជីវៈ (1.5M)') AND stu_study_year = @stu_study_year;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_1_5_M_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@stu_study_year", selectedYear);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            int total_stu_1_5M_count = reader.GetInt32("Total_student_1_5M_count");
                            int total_female_1_5M_count = reader.GetInt32("female_student_1_5M_count");
                            int total_male_1_5M_count = reader.GetInt32("male_student_1_5M_count");

                            Total_Stu_Count_1_5M.Text = "ជំនាញបច្ចេកទេស និងវិជ្ជាជីវៈ (1.5M)ចំនួន " + total_stu_1_5M_count.ToString() + " នាក់";
                            Total_Stu_Female_Count_1_5M.Text = "ស្រី " + total_female_1_5M_count.ToString() + " នាក់";
                            Total_Student_Male_Count_1_5M.Text = "ប្រុស " + total_male_1_5M_count.ToString() + " នាក់";
                        }
                    }                  
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("1.5M Error: " + ex.Message);
                }
            }
        }

        //Graph C1 Student
        private void LoadTotalStudent_C1_Count(string selectedYear)
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_C1_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_C1_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_C1_count FROM student_infomations WHERE stu_education_level IN ('សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ១') AND stu_study_year = @stu_study_year";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_C1_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@stu_study_year", selectedYear);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            int total_stu_c1_count = reader.GetInt32("Total_student_C1_count");
                            int total_female_c1_count = reader.GetInt32("female_student_C1_count");
                            int total_male_c1_count = reader.GetInt32("male_student_C1_count");

                            Total_Stu_Count_C1.Text = "សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ១ចំនួន " + total_stu_c1_count.ToString() + " នាក់";
                            Total_Stu_Female_Count_C1.Text = "ស្រី " + total_female_c1_count.ToString() + " នាក់";
                            Total_Student_Male_Count_C1.Text = "ប្រុស " + total_male_c1_count.ToString() + " នាក់";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("C1 Error: " + ex.Message);
                }
            }
        }

        //Graph C2 Student
        private void LoadTotalStudent_C2_Count(string selectedYear)
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_C2_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_C2_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_C2_count FROM student_infomations WHERE stu_education_level IN ('សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ២') AND stu_study_year = @stu_study_year";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_C2_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@stu_study_year", selectedYear);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            int total_stu_c2_count = reader.GetInt32("Total_student_C2_count");
                            int total_female_c2_count = reader.GetInt32("female_student_C2_count");
                            int total_male_c2_count = reader.GetInt32("male_student_C2_count");

                            Total_Stu_Count_C2.Text = "សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ២ចំនួន " + total_stu_c2_count.ToString() + " នាក់";
                            Total_Stu_Female_Count_C2.Text = "ស្រី " + total_female_c2_count.ToString() + " នាក់";
                            Total_Student_Male_Count_C2.Text = "ប្រុស " + total_male_c2_count.ToString() + " នាក់";
                        }
                    }                 
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("C2 Error: " + ex.Message);
                }
            }
        }

        //Graph C3 Student
        private void LoadTotalStudent_C3_Count(string selectedYear)
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_C3_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_C3_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_C3_count FROM student_infomations WHERE stu_education_level IN ('សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ៣');";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_C3_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@stu_study_year", selectedYear);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            int total_stu_c3_count = reader.GetInt32("Total_student_C3_count");
                            int total_female_c3_count = reader.GetInt32("female_student_C3_count");
                            int total_male_c3_count = reader.GetInt32("male_student_C3_count");

                            Total_Stu_Count_C3.Text = "សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ៣ចំនួន " + total_stu_c3_count.ToString() + " នាក់";
                            Total_Stu_Female_Count_C3.Text = "ស្រី " + total_female_c3_count.ToString() + " នាក់";
                            Total_Student_Male_Count_C3.Text = "ប្រុស " + total_male_c3_count.ToString() + " នាក់";
                        }
                    }            
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("C3 Error: " + ex.Message);
                }
            }

        }

        //Graph Associate student
        private void LoadTotalStudent_Associate_Count(string selectedYear)
        {

            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_associate_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_ass_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_ass_count FROM student_infomations WHERE stu_education_level IN ('បរិញ្ញាបត្ររង', 'សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស') AND stu_study_year = @stu_study_year;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_Associate_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@stu_study_year", selectedYear);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            int total_stu_ass_count = reader.GetInt32("Total_student_associate_count");
                            int total_female_ass_count = reader.GetInt32("female_student_ass_count");
                            int total_male_ass_count = reader.GetInt32("male_student_ass_count");

                            Total_Stu_Count_Associate.Text = "កម្រិតបរិញ្ញាបត្ររង " + total_stu_ass_count.ToString() + " នាក់";
                            Total_Stu_Female_Count_Associate.Text = "ស្រី " + total_female_ass_count.ToString() + " នាក់";
                            Total_Student_Male_Count_Associate.Text = "ប្រុស " + total_male_ass_count.ToString() + " នាក់";
                        }
                    }               
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error LoadTotalStudent_Associate_Count: " + ex.Message);
                }
            }
        }

        //Graph show Total Student by StudyTime
        private void LoadTotalStu_byStudyTime(string selectedYear)
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT stu_study_time_shift, COUNT(stu_id) AS student_count, COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_count FROM student_infomations WHERE stu_study_year = @stu_study_year GROUP BY stu_study_time_shift;";

            var data = new List<(string studytime_shift, int total_count, int female_count, int male_count)>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Connection Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@stu_study_year", selectedYear);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string studytime_shift = reader.GetString("stu_study_time_shift");
                            int total_count = reader.GetInt32("student_count");
                            int female_count = reader.GetInt32("female_student_count");
                            int male_count = reader.GetInt32("male_student_count");
                            data.Add((studytime_shift, total_count, female_count, male_count));
                        }

                        // After fetching data, pass it to the method to generate the graph.
                        UpdateChart_Count_by_StudyTime(data);
                        //CreateBarChart(data);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., show a message to the user)
                    Debug.WriteLine("Error: " + ex.Message);
                }
            }
        }

        //Update Chart Count
        private void UpdateChart_Count_by_StudyTime(List<(string studytime_shift, int total_count, int female_count, int male_count)> data)
        {
            ChartCanvas_Show_By_StudyTime.Children.Clear(); // Clear previous chart data
            CreateBarChart_Count_By_StudyTime(data); // Create chart with updated data
        }

        //Graph Show Count Total Stu by Skill Subject
        private void LoadTotalStu_DataFromDatabase(string selectedYear)
        {

            string connectionString = _ConnectionString._connectionString.ToString();

            string query = "SELECT " +
                "stu_education_subject, " +
                "COUNT(stu_id) AS student_count, " +
                "COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_count " +
                "FROM student_infomations WHERE stu_study_year = @stu_study_year " +
                "GROUP BY stu_education_subject; ";

            var data = new List<(string skill, int total_count, int female_count)>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Connection Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    {
                        cmd.Parameters.AddWithValue("@stu_study_year", selectedYear);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        //List<(string skill, int total_count,int female_count)> data = new List<(string, int,int)>();

                        while (reader.Read())
                        {
                            string skill = reader.GetString("stu_education_subject");
                            int total_count = reader.GetInt32("student_count");
                            int female_count = reader.GetInt32("female_student_count");
                            data.Add((skill, total_count, female_count));
                        }

                        // After fetching data, pass it to the method to generate the graph.
                        UpdateChart(data);
                        //CreateBarChart(data);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., show a message to the user)
                    Debug.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void UpdateChart(List<(string skill, int total_count, int female_count)> data)
        {
            ChartCanvas.Children.Clear(); // Clear previous chart data
            CreateBarChart(data); // Create chart with updated data
        }

        private void CreateBarChart(List<(string skill, int total_count, int female_count)> data)
        {
            double canvasLeft = 50;  // Starting position for the first bar
            double maxBarHeight = 200;  // Maximum height for the largest bar
            double barWidth = 40;  // Width of each bar
            double spacing = 0;   // Space between bars
            double groupSpacing = 40;   // Space between bar groups

            int maxCount = 0;
            foreach (var entry in data)
            {
                if (entry.total_count > maxCount)
                {
                    maxCount = entry.total_count;
                }
            }

            foreach (var entry in data)
            {
                // Calculate height of each bar relative to the max count.
                double totalHeight = (entry.total_count / (double)maxCount) * maxBarHeight;
                double femaleHeight = (entry.female_count / (double)maxCount) * maxBarHeight;

                // Total students bar
                var totalBar = new Microsoft.UI.Xaml.Shapes.Rectangle
                {
                    Width = barWidth,
                    Height = totalHeight,
                    Fill = new SolidColorBrush(Colors.Aqua)  // Light color for total students
                };
                Canvas.SetLeft(totalBar, canvasLeft);
                Canvas.SetTop(totalBar, maxBarHeight - totalHeight + 50);
                ChartCanvas.Children.Add(totalBar);

                // Female students bar
                var femaleBar = new Microsoft.UI.Xaml.Shapes.Rectangle
                {
                    Width = barWidth,
                    Height = femaleHeight,
                    Fill = new SolidColorBrush(Colors.DarkRed)  // Dark color for female students
                };
                Canvas.SetLeft(femaleBar, canvasLeft + barWidth + spacing);
                Canvas.SetTop(femaleBar, maxBarHeight - femaleHeight + 50);
                ChartCanvas.Children.Add(femaleBar);

                // Labels for total count
                var totalCountLabel = new TextBlock
                {
                    Text = entry.total_count.ToString(),
                    Width = barWidth,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 12,
                };
                Canvas.SetLeft(totalCountLabel, canvasLeft);
                Canvas.SetTop(totalCountLabel, maxBarHeight - totalHeight + 35);
                ChartCanvas.Children.Add(totalCountLabel);

                // Labels for female count
                var femaleCountLabel = new TextBlock
                {
                    Text = entry.female_count.ToString(),
                    Width = barWidth,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 12,
                };
                Canvas.SetLeft(femaleCountLabel, canvasLeft + barWidth + spacing);
                Canvas.SetTop(femaleCountLabel, maxBarHeight - femaleHeight + 35);
                ChartCanvas.Children.Add(femaleCountLabel);

                // Skill label (placed under the group of bars)
                TextBlock skillLabel = new TextBlock
                {
                    Text = entry.skill,
                    Width = barWidth * 2 + spacing,  // Adjust width to cover both bars
                    TextAlignment = TextAlignment.Center,
                    FontSize = 10,
                    FontFamily = new FontFamily("Khmer OS Battambang"),
                    TextWrapping = TextWrapping.Wrap
                };
                Canvas.SetLeft(skillLabel, canvasLeft - 5);  // Center the text under the group
                Canvas.SetTop(skillLabel, maxBarHeight + 60);
                ChartCanvas.Children.Add(skillLabel);

                // Move to the next position for the next group of bars
                canvasLeft += barWidth * 2 + spacing + groupSpacing;
            }
            // Add a legend
            AddLegend();
        }

        //
        private void AddLegend()
        {
            // Legend for Total Students
            var totalLegendColor = new Microsoft.UI.Xaml.Shapes.Rectangle
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.Aqua)
            };
            Canvas.SetLeft(totalLegendColor, 50);
            Canvas.SetTop(totalLegendColor, 300);
            ChartCanvas.Children.Add(totalLegendColor);

            var totalLegendLabel = new TextBlock
            {
                Text = "សិស្សនិស្សិតសរុប",
                FontSize = 10,
                FontFamily = new FontFamily("Khmer OS Battambang"),

            };
            Canvas.SetLeft(totalLegendLabel, 75);
            Canvas.SetTop(totalLegendLabel, 300);
            ChartCanvas.Children.Add(totalLegendLabel);

            // Legend for Female Students
            var femaleLegendColor = new Microsoft.UI.Xaml.Shapes.Rectangle
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.Red)
            };
            Canvas.SetLeft(femaleLegendColor, 50);
            Canvas.SetTop(femaleLegendColor, 330);
            ChartCanvas.Children.Add(femaleLegendColor);

            var femaleLegendLabel = new TextBlock
            {
                Text = "ស្រី",
                FontSize = 10,
                FontFamily = new FontFamily("Khmer OS Battambang")

            };
            Canvas.SetLeft(femaleLegendLabel, 75);
            Canvas.SetTop(femaleLegendLabel, 330);
            ChartCanvas.Children.Add(femaleLegendLabel);
        }

        //
        private void CreateBarChart_Count_By_StudyTime(List<(string studytime_shift, int total_count, int female_count, int male_count)> data)
        {
            double canvasLeft = 60;  // Starting position for the first bar
            double maxBarHeight = 200;  // Maximum height for the largest bar
            double barWidth = 40;  // Width of each bar
            double spacing = 0;   // Space between bars
            double groupSpacing = 40;   // Space between bar groups

            int maxCount = 0;
            foreach (var entry in data)
            {
                if (entry.total_count > maxCount)
                {
                    maxCount = entry.total_count;
                }
            }

            foreach (var entry in data)
            {
                // Calculate height of each bar relative to the max count.
                double totalHeight = (entry.total_count / (double)maxCount) * maxBarHeight;
                double femaleHeight = (entry.female_count / (double)maxCount) * maxBarHeight;


                // Total students bar
                var totalBar = new Microsoft.UI.Xaml.Shapes.Rectangle
                {
                    Width = barWidth,
                    Height = totalHeight,
                    Fill = new SolidColorBrush(Colors.Aqua)  // Light color for total students
                };
                Canvas.SetLeft(totalBar, canvasLeft);
                Canvas.SetTop(totalBar, maxBarHeight - totalHeight + 50);
                ChartCanvas_Show_By_StudyTime.Children.Add(totalBar);

                // Female students bar
                var femaleBar = new Microsoft.UI.Xaml.Shapes.Rectangle
                {
                    Width = barWidth,
                    Height = femaleHeight,
                    Fill = new SolidColorBrush(Colors.DarkRed)  // Red color for female students
                };
                Canvas.SetLeft(femaleBar, canvasLeft + barWidth + spacing);
                Canvas.SetTop(femaleBar, maxBarHeight - femaleHeight + 50);
                ChartCanvas_Show_By_StudyTime.Children.Add(femaleBar);

                // Labels for total count
                var totalCountLabel = new TextBlock
                {
                    Text = entry.total_count.ToString(),
                    Width = barWidth,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 12,
                };
                Canvas.SetLeft(totalCountLabel, canvasLeft);
                Canvas.SetTop(totalCountLabel, maxBarHeight - totalHeight + 35);
                ChartCanvas_Show_By_StudyTime.Children.Add(totalCountLabel);

                // Labels for female count
                var femaleCountLabel = new TextBlock
                {
                    Text = entry.female_count.ToString(),
                    Width = barWidth,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 12,
                };
                Canvas.SetLeft(femaleCountLabel, canvasLeft + barWidth + spacing);
                Canvas.SetTop(femaleCountLabel, maxBarHeight - femaleHeight + 35);
                ChartCanvas_Show_By_StudyTime.Children.Add(femaleCountLabel);

                // Skill label (placed under the group of bars)
                TextBlock skillLabel = new TextBlock
                {
                    Text = entry.studytime_shift,
                    Width = barWidth * 2 + spacing,  // Adjust width to cover both bars
                    TextAlignment = TextAlignment.Center,
                    FontSize = 10,
                    FontFamily = new FontFamily("Khmer OS Battambang"),
                    TextWrapping = TextWrapping.Wrap
                };
                Canvas.SetLeft(skillLabel, canvasLeft - 5);  // Center the text under the group
                Canvas.SetTop(skillLabel, maxBarHeight + 60);
                ChartCanvas_Show_By_StudyTime.Children.Add(skillLabel);

                // Move to the next position for the next group of bars
                canvasLeft += barWidth * 2 + spacing + groupSpacing;
            }
            // Add a legend
            AddLegend_For_Graph_StudyTime();
        }

        private void AddLegend_For_Graph_StudyTime()
        {
            // Legend for Total Students
            var totalLegendColor = new Microsoft.UI.Xaml.Shapes.Rectangle
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.Aqua)
            };
            Canvas.SetLeft(totalLegendColor, 50);
            Canvas.SetTop(totalLegendColor, 300);
            ChartCanvas_Show_By_StudyTime.Children.Add(totalLegendColor);

            var totalLegendLabel = new TextBlock
            {
                Text = "សិស្សនិស្សិតសរុប",
                FontSize = 10,
                FontFamily = new FontFamily("Khmer OS Battambang"),

            };
            Canvas.SetLeft(totalLegendLabel, 75);
            Canvas.SetTop(totalLegendLabel, 300);
            ChartCanvas_Show_By_StudyTime.Children.Add(totalLegendLabel);

            // Legend for Female Students
            var femaleLegendColor = new Microsoft.UI.Xaml.Shapes.Rectangle
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.Red)
            };
            Canvas.SetLeft(femaleLegendColor, 50);
            Canvas.SetTop(femaleLegendColor, 330);
            ChartCanvas_Show_By_StudyTime.Children.Add(femaleLegendColor);

            var femaleLegendLabel = new TextBlock
            {
                Text = "ស្រី",
                FontSize = 10,
                FontFamily = new FontFamily("Khmer OS Battambang")

            };
            Canvas.SetLeft(femaleLegendLabel, 75);
            Canvas.SetTop(femaleLegendLabel, 330);
            ChartCanvas_Show_By_StudyTime.Children.Add(femaleLegendLabel);
        }

    }
}
