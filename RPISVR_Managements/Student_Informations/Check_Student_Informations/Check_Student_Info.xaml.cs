using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RPISVR_Managements.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Dispatching;
using RPISVR_Managements.Model;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Windows.Devices.Enumeration;
using RPISVR_Managements.Student_Informations.Insert_Student_Informations;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media.Imaging;

namespace RPISVR_Managements.Student_Informations.Check_Student_Informations
{



    public sealed partial class Check_Student_Info : Page
    {
        public StudentViewModel ViewModel { get; set; }
        private DatabaseConnection _ConnectionString;

        //Refresh Timer
        private Timer _refreshTimer;
        private const int RefreshInterval = 10000;
        private DispatcherTimer _dispatcherTimer;


        public Check_Student_Info()
        {
            this.InitializeComponent();

            this.DataContext = App.SharedViewModel;

            ViewModel = new StudentViewModel();
            this.DataContext = new StudentViewModel();  // Bind ViewModel to View  

            // Subscribe to ErrorMessage changes from the ViewModel
            var viewModel = (StudentViewModel)this.DataContext;

            //Connect Database
            _ConnectionString = new DatabaseConnection();
            string connectionString = _ConnectionString._connectionString;

            //viewModel.SetEditMode(true);
            App.SharedViewModel.SetEditMode(true);


            StartAutoRefresh();



        }
        private void StartAutoRefresh()
        {
            // Initialize the timer to refresh data periodically
            _refreshTimer = new Timer(RefreshData, null, 0, RefreshInterval);
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(10); // Set to desired interval
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStu_DataFromDatabase();
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStu_byStudyTime();
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent();
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent_Bac_Count();
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent_Associate_Count();
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent_C3_Count();
            _dispatcherTimer.Tick += (senser, e) => LoadTotalStudent_C2_Count();
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent_C1_Count();
            _dispatcherTimer.Tick += (sender, e) => LoadTotalStudent_1_5M_Count();
            _dispatcherTimer.Start();
        }
        private void RefreshData(object state)
        {
            // Fetch and update data on the main thread
            DispatcherQueue.TryEnqueue(() =>
            {
                LoadTotalStu_DataFromDatabase(); // Fetch latest data and update chart
                LoadTotalStudent();
                LoadTotalStudent_Bac_Count();
                LoadTotalStudent_Associate_Count();
                LoadTotalStudent_C3_Count();
                LoadTotalStudent_C2_Count();
                LoadTotalStudent_C1_Count();
                LoadTotalStudent_1_5M_Count();
                LoadTotalStu_byStudyTime();
            });
        }
        //Graph 1.5M Student
        private void LoadTotalStudent_1_5M_Count()
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_1_5M_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_1_5M_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_1_5M_count FROM student_infomations WHERE stu_education_level IN ('ជំនាញបច្ចេកទេស និងវិជ្ជាជីវៈ (1.5M)');";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_1_5_M_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                catch (Exception ex)
                {
                    Console.WriteLine("1.5M Error: " + ex.Message);
                }
            }
        }
        //Graph C1 Student
        private void LoadTotalStudent_C1_Count()
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_C1_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_C1_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_C1_count FROM student_infomations WHERE stu_education_level IN ('សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ១');";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_C1_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                catch (Exception ex)
                {
                    Console.WriteLine("C1 Error: " + ex.Message);
                }
            }
        }
        //Graph C2 Student
        private void LoadTotalStudent_C2_Count()
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_C2_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_C2_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_C2_count FROM student_infomations WHERE stu_education_level IN ('សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ២');";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_C2_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                catch (Exception ex)
                {
                    Console.WriteLine("C2 Error: " + ex.Message);
                }
            }
        }
        //Graph C3 Student
        private void LoadTotalStudent_C3_Count()
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
                catch (Exception ex)
                {
                    Console.WriteLine("C3 Error: " + ex.Message);
                }
            }

        }
        //Graph Associate student
        private void LoadTotalStudent_Associate_Count()
        {

            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_associate_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_ass_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_ass_count FROM student_infomations WHERE stu_education_level IN ('បរិញ្ញាបត្ររង', 'សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស');";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_Associate_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
        //Graph Bachelor student
        private void LoadTotalStudent_Bac_Count()
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_education_level) AS Total_student_bac_count,COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_bac_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_bac_count FROM student_infomations WHERE stu_education_level IN ('បរិញ្ញាបត្រ', 'សញ្ញាបត្រវិស្វករ', 'បរិញ្ញាបត្របច្ចេកវិទ្យា');";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Refresh LoadTotalStudent_Bac_Count Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        //Graph total student
        private void LoadTotalStudent()
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT COUNT(stu_id) AS student_count, COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_count, COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_count FROM student_infomations; ";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Connection Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., show a message to the user)
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
        //Graph show Total Student by StudyTime
        private void LoadTotalStu_byStudyTime()
        {
            string connectionString = _ConnectionString._connectionString.ToString();
            string query = "SELECT stu_study_time_shift, COUNT(stu_id) AS student_count, COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_count,COUNT(CASE WHEN stu_gender = 'ប្រុស' THEN 1 END) AS male_student_count FROM student_infomations GROUP BY stu_study_time_shift;";

            var data = new List<(string studytime_shift, int total_count, int female_count, int male_count)>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Connection Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., show a message to the user)
                    Debug.WriteLine("Error: " + ex.Message);
                }
            }
        }
        private void UpdateChart_Count_by_StudyTime(List<(string studytime_shift, int total_count, int female_count, int male_count)> data)
        {
            ChartCanvas_Show_By_StudyTime.Children.Clear(); // Clear previous chart data
            CreateBarChart_Count_By_StudyTime(data); // Create chart with updated data
        }

        //Graph Show Count Total Stu by Skill Subject
        private void LoadTotalStu_DataFromDatabase()
        {

            string connectionString = _ConnectionString._connectionString.ToString();
            
            string query = "SELECT " +
                "stu_education_subject, " +
                "COUNT(stu_id) AS student_count, " +
                "COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_count " +
                "FROM student_infomations " +
                "GROUP BY stu_education_subject; ";

            var data = new List<(string skill, int total_count, int female_count)>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Debug.WriteLine(conn.ConnectionString + "Connection Ok");
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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

        private void Clear(object sender, RoutedEventArgs e)
        {
            Stu_EducationLevels_Search.SelectedItem = null;
            Stu_Subject_Search.SelectedItem = null;
            Stu_StudyTime_Search.SelectedItem = null;
            Stu_TypeStudy_Search.SelectedItem = null;
            Stu_StudyYear_Search.SelectedItem = null;

        }

        private void clear_edu_level(object sender, RoutedEventArgs e)
        {
            Stu_EducationLevels_Search.SelectedItem = null;
        }

        private void clear_edu_subject(object sender, RoutedEventArgs e)
        {
            Stu_Subject_Search.SelectedItem = null;
        }

        private void clear_edu_studytime(object sender, RoutedEventArgs e)
        {
            Stu_StudyTime_Search.SelectedItem = null;
        }

        private void clear_edu_typestudy(object sender, RoutedEventArgs e)
        {
            Stu_TypeStudy_Search.SelectedItem = null;
        }

        private void clear_edu_studyyear(object sender, RoutedEventArgs e)
        {
            Stu_StudyYear_Search.SelectedItem = null;
        }

        private async void btn_Edit_in_Check_Mode(object sender, RoutedEventArgs e)
        {
            string Stu_ID_Edit;
            Stu_ID_Edit = Stu_ID.Text;
            if (string.IsNullOrEmpty(Stu_ID_Edit))
            {
                var dialog = new ContentDialog
                {
                    Title = new TextBlock
                    {
                        Text = "សូមជ្រើសរើសសិស្សនិស្សិតជាមុនសិន",
                        FontSize = 18,
                        FontFamily = new FontFamily("Khmer OS Battambang"),
                        FontWeight = FontWeights.Bold,
                    },
                    CloseButtonText = "យល់ព្រម",
                    XamlRoot = this.XamlRoot,
                    RequestedTheme = ElementTheme.Default


                };
                await dialog.ShowAsync();
            }
            else
            {
                // Store the student ID in App.xaml.cs
                (Application.Current as App).StudentID = Stu_ID_Edit;

                // Navigate to Page1 (Insert_Student_Info)
                Frame.Navigate(typeof(Insert_Student_Info));


            }
        }

        private async void btn_Delete_Student_Info_in_Check_Mode(object sender, RoutedEventArgs e)
        {
            string Stu_ID_Edit;
            Stu_ID_Edit = Stu_ID.Text;
            if (string.IsNullOrEmpty(Stu_ID_Edit))
            {
                var dialog = new ContentDialog
                {
                    Title = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Children =
                                    {
                                        new Image
                                        {
                                            Source = new BitmapImage(new Uri("ms-appx:///Assets//Student_Info_Icon//icons8-information-96.png")), // Path to your image file
                                            Width = 24,
                                            Height = 24,
                                            Margin = new Thickness(0, 0, 8, 0) // Add some space between the icon and the text
                                        },
                                        new TextBlock
                                        {
                                            Text = " សូមជ្រើសរើសសិស្សនិស្សិតជាមុនសិន",
                                            TextAlignment = TextAlignment.Center,
                                            FontSize = 16,
                                            FontFamily = new FontFamily("Khmer OS Battambang"),
                                            //FontWeight = FontWeights.Bold
                                        }
                                     }
                    },
                    CloseButtonText = "យល់ព្រម",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot,
                    RequestedTheme = ElementTheme.Default
                };
                await dialog.ShowAsync();
            }
            else
            {
                var deleteDialog = new ContentDialog
                {
                    Title = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Children =
                                    {
                                        new Image
                                        {
                                            Source = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png")), // Path to your image file
                                            Width = 24,
                                            Height = 24,
                                            Margin = new Thickness(0, 0, 8, 0) // Add some space between the icon and the text
                                        },
                                        new TextBlock
                                        {
                                            Text = $"តើអ្នកចង់លុបទិន្នន័យនេះមែនទេ? {Stu_ID_Edit}",
                                            TextAlignment = TextAlignment.Center,
                                            FontSize = 16,
                                            FontFamily = new FontFamily("Khmer OS Battambang"),
                                            //FontWeight = FontWeights.Bold
                                        }
                                     }
                    },
                    PrimaryButtonText = "លុប",
                    CloseButtonText = "ទេ",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot,
                    RequestedTheme = ElementTheme.Default
                };


                // Show the dialog and capture the result
                var result = await deleteDialog.ShowAsync();

                // Check if the user clicked "Yes" (PrimaryButton)
                if (result == ContentDialogResult.Primary)
                {
                    // Perform the delete action here
                    DeleteItem();

                    // Show confirmation or feedback if needed
                    var confirmationDialog = new ContentDialog
                    {
                        Title = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Children =
                                    {
                                        new Image
                                        {
                                            Source = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png")),
                                            Width = 24,
                                            Height = 24,
                                            Margin = new Thickness(0, 0, 8, 0)
                                        },
                                        new TextBlock
                                        {
                                            Text = $"{Stu_ID_Edit} បានលុបដោយជោគជ័យ",
                                            TextAlignment = TextAlignment.Center,
                                            FontSize = 16,
                                            FontFamily = new FontFamily("Khmer OS Battambang"),
                                            //FontWeight = FontWeights.Bold
                                        }
                                     }
                        },
                        CloseButtonText = "បិទ",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = this.XamlRoot,
                        RequestedTheme = ElementTheme.Default
                    };
                    await confirmationDialog.ShowAsync();
                    Frame.Navigate(typeof(Check_Student_Info));
                }

            }

        }

        private bool DeleteItem()
        {
            string Stu_ID_Edit = Stu_ID.Text;

            try
            {
                string connectionString = _ConnectionString._connectionString.ToString();
                string query = "DELETE FROM student_infomations WHERE stu_id = @stu_id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    Debug.WriteLine("Connection OK: " + conn.ConnectionString);

                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add the parameter with the student ID to delete
                        cmd.Parameters.AddWithValue("@stu_id", Stu_ID_Edit);

                        // Execute the delete query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Check if any rows were deleted
                        if (rowsAffected > 0)
                        {
                            Debug.WriteLine("Delete OK");
                            return true; // Indicate that the delete was successful
                        }
                        else
                        {
                            Debug.WriteLine("No record found with the given ID.");
                            return false; // No record was deleted
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred: " + ex.Message);
                return false; // Indicate that an error occurred
            }
        }

    }
}


       
