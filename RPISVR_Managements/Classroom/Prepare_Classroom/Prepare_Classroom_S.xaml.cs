using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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



namespace RPISVR_Managements.Classroom.Prepare_Classroom
{
    
    public sealed partial class Prepare_Classroom_S : Page
    {
        public StudentViewModel ViewModel { get; set; }

        public Prepare_Classroom_S()
        {
            this.InitializeComponent();

            ViewModel = new StudentViewModel();
            this.DataContext = new StudentViewModel();

            //ErrorMessage
            var viewModel = (StudentViewModel)this.DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;

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

        
    }
}
