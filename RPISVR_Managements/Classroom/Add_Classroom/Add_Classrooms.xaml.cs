using DocumentFormat.OpenXml.Bibliography;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using MySql.Data.MySqlClient;
using RPISVR_Managements.Classroom.Prepare_Classroom;
using RPISVR_Managements.Model;
using RPISVR_Managements.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace RPISVR_Managements.Classroom.Add_Classroom
{
        
    public sealed partial class Add_Classrooms : Page
    {
        public StudentViewModel ViewModel { get; set; }
        private DatabaseConnection _ConnectionString;

        public Add_Classrooms()
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

        private void Clear_Class_Study_Year_Searchr(object sender, RoutedEventArgs e)
        {
            Class_Study_Year_Search.SelectedItem = null;
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

        private void Class_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var selectedClasses = listView.SelectedItems.Cast<Student_Info>().ToList();
            // Check if there are any selected items
            var firstClassSelectedItem = listView.SelectedItems.Cast<Student_Info>().FirstOrDefault();
            // Update the ViewModel with the selected items
            if (DataContext is StudentViewModel viewModel)
            {
                viewModel.SelectedClasses_Edit_Delete = selectedClasses;
                viewModel.FirstSelectedClass = firstClassSelectedItem;
            }
        }

        private void btn_open_prepare_class(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Prepare_Classroom_S));
        }

        private async void btn_delete_class_selection(object sender, RoutedEventArgs e)
        {
            if(DataContext is StudentViewModel viewModel)
            {
                if(viewModel.SelectedClasses_Edit_Delete == null || !viewModel.SelectedClasses_Edit_Delete.Any())
                {
                    Debug.WriteLine("No items selected to delete.");

                    //Dailog Message
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
                                            Text = " សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន!",
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
                    foreach(var items in viewModel.SelectedClasses_Edit_Delete)
                    {
                        Debug.WriteLine($"Selected ID: {items.Class_ID}");
                    }

                    //Dailog Message YesNo for delete.
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
                                            Text = $"តើអ្នកចង់លុបទិន្នន័យថ្នាក់រៀនទាំងនេះមែនទេ?",
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


                    //Show the dialog and capture the result
                    var result = await deleteDialog.ShowAsync();

                    //Check if the user clicked "Yes"(PrimaryButton)
                    if (result == ContentDialogResult.Primary)
                    {
                        // Perform the delete action here
                        DeleteClassItem();

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
                                            Text = $"ថ្នាក់រៀនត្រូវបានលុបដោយជោគជ័យ",
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
                        //Frame.Navigate(typeof(Prepare_Classroom_S));
                    }
                    
                }
            }
            else
            {
                Debug.WriteLine("DataContext is not of type StudentViewModel.");
            }
        }

        //Method Delete
        private async void DeleteClassItem()
        {
            string connectionString = _ConnectionString._connectionString.ToString();

            if(DataContext is StudentViewModel viewModel)
            {
                if(viewModel.SelectedClasses_Edit_Delete == null || !viewModel.SelectedClasses_Edit_Delete.Any())
                {
                    Debug.WriteLine("No items selected to delete.");
                    return;
                }

                try
                {
                    using(var connection = new MySqlConnection(connectionString))
                    {
                        //Open connection
                        await connection.OpenAsync();

                        //Delete
                        foreach(var selectedItems in viewModel.SelectedClasses_Edit_Delete)
                        {
                            string deleteQuery = "DELETE FROM classes WHERE class_id = @class_id";

                            using (var command = new MySqlCommand(deleteQuery, connection))
                            {
                                command.Parameters.AddWithValue("@class_id", selectedItems.Class_ID);

                                int rowAffected = await command.ExecuteNonQueryAsync();
                                if (rowAffected > 0)
                                {
                                    Debug.WriteLine($"Deleted ID: {selectedItems.Class_ID}");
                                }
                                else
                                {
                                    Debug.WriteLine($"Fail to deleted class ID: {selectedItems.Class_ID}");
                                }
                            }
                        }
                        //Refresh Class List
                        foreach (var selectedItem in viewModel.SelectedClasses_Edit_Delete)
                        {
                            viewModel.Classes_Info.Remove(selectedItem);
                        }
                        //Clear
                        viewModel.SelectedClasses_Edit_Delete.Clear();
                    }
                    
                }catch(Exception ex)
                {
                    Debug.WriteLine($"Error while deleting classes: {ex.Message}");
                    return;
                }
            }
        }
    }
}
