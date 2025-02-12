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
using RPISVR_Managements.Model;
using RPISVR_Managements.ViewModel;
using Microsoft.UI.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace RPISVR_Managements.Student_Score.Check_Student_Score
{   
    public sealed partial class Check_Student_Score_Info : Page
    {
        public StudentViewModel ViewModel { get; set; }
        private DatabaseConnection _ConnectionString;

        public Check_Student_Score_Info()
        {
            this.InitializeComponent();

            this.DataContext = App.SharedViewModel;

            ViewModel = new StudentViewModel();
            this.DataContext = new StudentViewModel();  // Bind ViewModel to View  

            // Subscribe to ErrorMessage changes from the ViewModel
            var viewModel = (StudentViewModel)this.DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            viewModel.PropertyChanged += ViewModel_Delete_PropertyChanged;

            //Connect Database
            _ConnectionString = new DatabaseConnection();
            string connectionString = _ConnectionString._connectionString;

        }

        private async void ViewModel_Delete_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = (StudentViewModel)sender;

            // Check if the changed property is ErrorMessage_Delete
            if (e.PropertyName == nameof(viewModel.ErrorMessage_Delete) && !string.IsNullOrEmpty(viewModel.ErrorMessage_Delete))
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
                    //CloseButtonText = "យល់ព្រម",
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
                    Path = new PropertyPath("ErrorImageSource_Delete"),
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
                    Path = new PropertyPath("ErrorMessage_Delete"),
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

                // Add "Yes" and "No" buttons to the ContentDialog
                dialog.PrimaryButtonText = "យល់ព្រម";
                dialog.CloseButtonText = "ទេ";

                // Set custom styles if needed
                dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["CustomDialogButtonStyle"];
                //dialog.SecondaryButtonStyle = (Style)Application.Current.Resources["CustomDialogButtonStyle"];

                // Handle the result when the dialog is closed
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    // Yes button clicked
                    if (viewModel.CurrentOperation == "Delete_Student_Score_Info_MonFri")
                    {
                        viewModel.HandleYes_Delete_Student_Info_MonFri();
                    }
                    else if (viewModel.CurrentOperation == "Delete_Student_Score_Info_SatSun")
                    {
                        viewModel.HandleYes_Delete_Student_Info_SatSun();
                    }
                    else if (viewModel.CurrentOperation == "Export_Student_Score_Info_SatSun_PDF")
                    {
                        viewModel.HandleYes_Export_Student_Info_SatSun_PDF();
                    }
                    else if (viewModel.CurrentOperation == "Export_Student_Score_Info_MonFri_PDF")
                    {
                        viewModel.HandleYes_Export_Student_Info_MonFri_PDF();
                    }
                    else if(viewModel.CurrentOperation == "Delete_Student_Score")
                    {
                        viewModel.HandleYesDelete_Setting_Score();
                    }
                    else if( viewModel.CurrentOperation == "Export_Students_Rank_PDF")
                    {
                        viewModel.HandleYes_Export_Student_Rank_PDF();
                    }
                    else if(viewModel.CurrentOperation == "Export_Students_Rank_Excel")
                    {
                        viewModel.HandleYes_Export_Students_Rank_Excel();
                    }
                    else if(viewModel.CurrentOperation == "Send_Students_Class_Up")
                    {
                        viewModel.Handle_Yes_Send_Student_Up_Class();
                    }


                }
                //else if (result == ContentDialogResult.Secondary)
                //{
                //    // No button clicked
                //    Debug.WriteLine("No");
                //    viewModel.HandleNoResponse(); // Call ViewModel's method to handle No
                //}

                dialog.CloseButtonStyle = (Style)Application.Current.Resources["CustomDialogButtonStyle"];
                //await dialog.ShowAsync();
                Debug.WriteLine(viewModel.ErrorImageSource);
            }
        }

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

        private void Clear_Class_Skill(object sender, RoutedEventArgs e)
        {
            Class_Subject_Search.SelectedItem = null;
        }

        private void Clear_Class_Level(object sender, RoutedEventArgs e)
        {
            Class_Edu_Level_Class.SelectedItem = null;
        }

        private void Clear_Class_Study_Year(object sender, RoutedEventArgs e)
        {
            Class_Studying_Class.SelectedItem = null;
        }

        private void Clear_Class_Semester(object sender, RoutedEventArgs e)
        {
            Search_Class_Semester.SelectedItem = null;
        }

        private void Clear_Class_StudyTime(object sender, RoutedEventArgs e)
        {
            Search_Class_StudyTime.SelectedItem = null;
        }

        private void Clear_Class_StudyType(object sender, RoutedEventArgs e)
        {
            Search_Class_TypeStudy.SelectedItem = null;
        }

        private void Clear_Class_Study_Year_Searchr(object sender, RoutedEventArgs e)
        {
            Class_Study_Year_Search.SelectedItem = null;
        }

        private void Clear_Class_All(object sender, RoutedEventArgs e)
        {
            Search_Class_TypeStudy.SelectedItem = null;
            Search_Class_StudyTime.SelectedItem = null;
            Search_Class_Semester.SelectedItem = null;
            Class_Studying_Class.SelectedItem = null;
            Class_Subject_Search.SelectedItem = null;
            Class_Edu_Level_Class.SelectedItem = null;
        }

        private void PrepareClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            //var selectedItemsClass = listView.SelectedItems.Cast<Student_Info>().ToList();
            var FirstClassSelectedItem = listView.SelectedItems.Cast<Student_Info>().FirstOrDefault();

            if (DataContext is StudentViewModel viewModel)
            {
                viewModel.Selected_Class_in_Student_Score = FirstClassSelectedItem;
                //viewModel.SelectedClasses_Prepare_All = selectedItemsClass;
            }
        }

        private void select_all_stu_rank(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                // Check if the ListView has items
                if (Table_Students_Rank_List.ItemsSource is IEnumerable<object> items)
                {
                    if (toggleButton.IsChecked == true) // If ToggleButton is checked
                    {
                        // Select all items
                        Table_Students_Rank_List.SelectedItems.Clear();
                        foreach (var item in items)
                        {
                            Table_Students_Rank_List.SelectedItems.Add(item);
                        }
                    }
                    else // If ToggleButton is unchecked
                    {
                        // Deselect all items
                        Table_Students_Rank_List.SelectedItems.Clear();
                    }
                }
            }
        }

        private void Selected_Students_Rank(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var selectedItems_StudentRank = listView.SelectedItems.Cast<Class_Score>().ToList();
            //var FirstClassSelectedItem = listView.SelectedItems.Cast<Student_Info>().FirstOrDefault();

            if (DataContext is StudentViewModel viewModel)
            {
                //viewModel.Selected_Class_in_Student_Score = FirstClassSelectedItem;
                viewModel.MultiSelectAllStudent_Rank = selectedItems_StudentRank;
            }
            
        }
    }
}
