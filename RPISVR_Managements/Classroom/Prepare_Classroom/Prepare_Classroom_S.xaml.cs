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
using MySqlX.XDevAPI.Common;
using RPISVR_Managements.Classroom.Add_Classroom;
using RPISVR_Managements.Classroom.Add_Curriculum;
using RPISVR_Managements.Model;
using RPISVR_Managements.Student_Informations.Check_Student_Informations;
using RPISVR_Managements.Student_Informations.Insert_Student_Informations;
using RPISVR_Managements.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;



namespace RPISVR_Managements.Classroom.Prepare_Classroom
{
    
    public sealed partial class Prepare_Classroom_S : Page
    {
        public StudentViewModel ViewModel { get; set; }
        public StudentViewModel viewModel_Tab { get; set; }

        private DatabaseConnection _ConnectionString;

        public Prepare_Classroom_S()
        {
            this.InitializeComponent();

            ViewModel = new StudentViewModel();
            viewModel_Tab = new StudentViewModel();

            this.DataContext = new StudentViewModel();

            //ErrorMessage
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
                    if (viewModel.CurrentOperation == "Export_Schedule_PDF")
                    {
                        viewModel.HandleYesResponseExport_Schedule_PDF();
                    }
                    else if (viewModel.CurrentOperation == "Delete_Schedule")
                    {
                        viewModel.HandleYes_DeleteSchedule();
                    }
                    else if (viewModel.CurrentOperation == "Export_Schedule_Excel")
                    {
                        viewModel.HandleYesResponseExport_Schedule_Excel();
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

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);

        }

        private void btn_add_student_to_classes(object sender, RoutedEventArgs e)
        {
            TabView_Add_Student_to_Class.Visibility = Visibility.Visible;

            //Add to TabView
            if(!TabView_Class_Info.TabItems.Contains(TabView_Add_Student_to_Class))
            {
                TabView_Class_Info.TabItems.Add(TabView_Add_Student_to_Class);
            }
            TabView_Class_Info.SelectedItem = TabView_Add_Student_to_Class;
        }

        private void PrepareClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var selectedItemsClass = listView.SelectedItems.Cast<Student_Info>().ToList();
            var FirstClassSelectedItem = listView.SelectedItems.Cast<Student_Info>().FirstOrDefault();

            if(DataContext is StudentViewModel viewModel)
            {
                viewModel.FirstSelectedClasss_Preparing = FirstClassSelectedItem;
                viewModel.SelectedClasses_Prepare_All = selectedItemsClass;
            }
        }

        private void btn_edit_class_prepare(object sender, RoutedEventArgs e)
        {
            TabView_Update_Class.Visibility = Visibility.Visible;

            //Add to TabView
            if (!TabView_Class_Info.TabItems.Contains(TabView_Update_Class))
            {
                TabView_Class_Info.TabItems.Add(TabView_Update_Class);
            }
            TabView_Class_Info.SelectedItem = TabView_Update_Class;
            
        }

        private void btn_add_new_class(object sender, RoutedEventArgs e)
        {
            Tabview_Add_Class.Visibility = Visibility.Visible;

            //Add to TabView
            if (!TabView_Class_Info.TabItems.Contains(Tabview_Add_Class))
            {
                TabView_Class_Info.TabItems.Add(Tabview_Add_Class);
            }
            TabView_Class_Info.SelectedItem = Tabview_Add_Class;
            //Frame.Navigate(typeof(Add_Classrooms));
        }

        private async void btn_delete_class_selection(object sender, RoutedEventArgs e)
        {
            if (DataContext is StudentViewModel viewModel)
            {
                if (viewModel.SelectedClasses_Prepare_All == null || !viewModel.SelectedClasses_Prepare_All.Any())
                {
                    Debug.WriteLine("No items selected to delete.");


                    // Perform additional delete actions if required
                    // Example: Clearing the list after deletion
                    // viewModel.SelectedClasses_Prepare_All.Clear();

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
                    foreach (var item in viewModel.SelectedClasses_Prepare_All)
                    {
                        Debug.WriteLine($"Selected ID: {item.Class_ID}");
                    }
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
                    }

                }
            }
            else
            {
                Debug.WriteLine("DataContext is not of type StudentViewModel.");
            }
        }

        private async void DeleteClassItem()
        {
            string connectionString = _ConnectionString._connectionString.ToString();

            if (DataContext is StudentViewModel viewModel)
            {
                if(viewModel.SelectedClasses_Prepare_All == null || !viewModel.SelectedClasses_Prepare_All.Any())
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
                        foreach (var selectedItems in viewModel.SelectedClasses_Prepare_All)
                        {
                            string deleteQuery = "DELETE FROM classes WHERE class_id = @class_id";

                            using(var command = new MySqlCommand(deleteQuery, connection))
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
                        foreach (var selectedItem in viewModel.SelectedClasses_Prepare_All.ToList())
                        {
                            viewModel.Classes_Info.Remove(selectedItem);
                        }

                        // Clear the selection
                        viewModel.SelectedClasses_Prepare_All.Clear();
                    }
                }catch (Exception ex)
                {
                    Debug.WriteLine($"Error while deleting classes: {ex.Message}"); 
                    return;
                }
            }
        }

        private void btn_open_full_screen_add_class(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Add_Classrooms));
        }

        private void Selected_Student_Add_to_Class(object sender, SelectionChangedEventArgs e)
        {
            var listView_student = sender as ListView;
            var selected_Students_to_Classes = listView_student.SelectedItems.Cast<Student_Info>().ToList();
            // Check if there are any selected items
            var first_Student_ClassSelectedItem = listView_student.SelectedItems.Cast<Student_Info>().FirstOrDefault();
            // Update the ViewModel with the selected items
            if (DataContext is StudentViewModel viewModel)
            {
                viewModel.Selected_Students_to_Class = selected_Students_to_Classes;
                //viewModel.FirstSelectedClass = first_Student_ClassSelectedItem;
            }
        }

        private void btn_select_all_Items(object sender, RoutedEventArgs e)
        {
            if(sender is ToggleButton toggleButton)
            {
                // Check if the ListView has items
                if (List_Students_Display.ItemsSource is IEnumerable<object> items)
                {
                    if (toggleButton.IsChecked == true) // If ToggleButton is checked
                    {
                        // Select all items
                        List_Students_Display.SelectedItems.Clear();
                        foreach (var item in items)
                        {
                            List_Students_Display.SelectedItems.Add(item);
                        }
                        FontIcon icon = new FontIcon();
                        icon.Glyph = "\uE73A"; // Update the button content
                    }
                    else // If ToggleButton is unchecked
                    {
                        // Deselect all items
                        List_Students_Display.SelectedItems.Clear();
                        FontIcon icon = new FontIcon();
                        icon.Glyph = "\uE73B";
                    }
                }
            }
        }

        private void Selected_Student_In_Class_Delete(object sender, SelectionChangedEventArgs e)
        {
            var listView_student_in_class = sender as ListView;
            var selected_Students_in_Classes = listView_student_in_class.SelectedItems.Cast<Student_Info>().ToList();
            // Check if there are any selected items
            var first_Student_ClassSelectedItem = listView_student_in_class.SelectedItems.Cast<Student_Info>().FirstOrDefault();
            // Update the ViewModel with the selected items
            if (DataContext is StudentViewModel viewModel)
            {
                viewModel.Selected_Students_in_Class = selected_Students_in_Classes;
                //viewModel.FirstSelectedClass = first_Student_ClassSelectedItem;
            }
        }

        private void btn_select_all_student_in_class(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                // Check if the ListView has items
                if (List_Students_In_Class_Display.ItemsSource is IEnumerable<object> items)
                {
                    if (toggleButton.IsChecked == true) // If ToggleButton is checked
                    {
                        // Select all items
                        List_Students_In_Class_Display.SelectedItems.Clear();
                        foreach (var item in items)
                        {
                            List_Students_In_Class_Display.SelectedItems.Add(item);
                        }
                        FontIcon icon = new FontIcon();
                        icon.Glyph = "\uE73A"; // Update the button content
                    }
                    else // If ToggleButton is unchecked
                    {
                        // Deselect all items
                        List_Students_In_Class_Display.SelectedItems.Clear();
                        FontIcon icon = new FontIcon();
                        icon.Glyph = "\uE73B";
                    }
                }
            }
        }

        private void Selected_Student_Export(object sender, SelectionChangedEventArgs e)
        {
            var listView_student_in_class = sender as ListView;
            var selected_Students_in_Classes = listView_student_in_class.SelectedItems.Cast<Student_Info>().ToList();
           
            // Check if there are any selected items
            var first_Student_ClassSelectedItem = listView_student_in_class.SelectedItems.Cast<Student_Info>().FirstOrDefault();
            // Update the ViewModel with the selected items
            if (DataContext is StudentViewModel viewModel)
            {
                viewModel.Selected_Students_in_Class = null;
                viewModel.Selected_Students_in_Class = selected_Students_in_Classes;
                //viewModel.FirstSelectedClass = first_Student_ClassSelectedItem;
            }
        }

        private void btn_all_curriculum(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Add_Curriculums));
        }

        private void btn_add_schedule(object sender, RoutedEventArgs e)
        {
            TabView_Create_Schedule.Visibility = Visibility.Visible;

            //Add to TabView
            if (!TabView_Class_Info.TabItems.Contains(TabView_Create_Schedule))
            {
                TabView_Class_Info.TabItems.Add(TabView_Create_Schedule);
            }
            TabView_Class_Info.SelectedItem = TabView_Create_Schedule;
        }

        
        

        private void TabViewChange(object sender, SelectionChangedEventArgs e)
        {
            var listView_class_in_schedule = sender as ListView;
            //var selected_class_in_schedule = listView_class_in_schedule.SelectedItems.Cast<Student_Info>().ToList();
            // Check if there are any selected items
            var first_selected_class_in_schedule = listView_class_in_schedule.SelectedItems.Cast<Student_Info>().FirstOrDefault();
            // Update the ViewModel with the selected items
            if (DataContext is StudentViewModel viewModel)
            {
                viewModel.Selected_class_in_Schedule_List = first_selected_class_in_schedule;
                //viewModel.FirstSelectedClass = first_Student_ClassSelectedItem;

                if(viewModel.Class_In_Study_Timeshift == "វេនសៅរ៍អាទិត្យ")
                {
                    Debug.WriteLine("Open Tab Sat Sun.");
                    Schedule_Mon_Fri.Visibility = Visibility.Collapsed;
                    Schedule_Sat_Sun.Visibility = Visibility.Visible;

                    //Add to TabView
                    if (!Table_Schedule.TabItems.Contains(Schedule_Sat_Sun))
                    {
                        Table_Schedule.TabItems.Add(Schedule_Sat_Sun);
                    }
                    Table_Schedule.SelectedItem = Schedule_Sat_Sun;
                }
                else
                {
                    Debug.WriteLine("Open Tab Mon Fri.");
                    Schedule_Mon_Fri.Visibility = Visibility.Visible;
                    Schedule_Sat_Sun.Visibility = Visibility.Collapsed;
                    //Add to TabView
                    if (!Table_Schedule.TabItems.Contains(Schedule_Mon_Fri))
                    {
                        Table_Schedule.TabItems.Add(Schedule_Mon_Fri);
                    }
                    Table_Schedule.SelectedItem = Schedule_Mon_Fri;
                }
            }
           
        }
   
    }
}
