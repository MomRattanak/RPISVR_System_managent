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
using RPISVR_Managements.ViewModel;
using RPISVR_Managements.Model;
using Microsoft.UI.Text;
using System.ComponentModel;
using System.Diagnostics;


namespace RPISVR_Managements.Teacher_Information.Insert_Teacher_Information
{
    
    public sealed partial class Insert_Teacher_Info : Page
    {
        public TeacherViewModel ViewModel { get; set; }
        private DatabaseConnection _ConnectionString;

        public Insert_Teacher_Info()
        {
            this.InitializeComponent();

            ViewModel = new TeacherViewModel();
            this.DataContext = new TeacherViewModel();

            //ErrorMessage
            var viewModel = (TeacherViewModel)this.DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            viewModel.PropertyChanged += ViewModel_Delete_PropertyChanged;

            //Connect Database
            _ConnectionString = new DatabaseConnection();
            string connectionString = _ConnectionString._connectionString;


        }

        private async void ViewModel_Delete_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = (TeacherViewModel)sender;

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
                    Debug.WriteLine("OK");
                    viewModel.HandleYesResponse(); // Call ViewModel's method to handle Yes
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
            var viewModel = (TeacherViewModel)sender;

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

        private void Selected_Teacher(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var selectedTeacher_List = listView.SelectedItems.Cast<Teacher_Informatioins>().ToList();
            var firstSelected_Teacher = listView.SelectedItems.Cast<Teacher_Informatioins>().FirstOrDefault();

            if(DataContext is TeacherViewModel viewModel_Teacher)
            {
                viewModel_Teacher.Multi_Selected_Teacher = selectedTeacher_List;
                viewModel_Teacher.First_Select_Teacher = firstSelected_Teacher;
            }

        }
    }


}
