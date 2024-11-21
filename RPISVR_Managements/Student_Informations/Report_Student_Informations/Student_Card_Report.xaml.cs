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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RPISVR_Managements.Student_Informations.Report_Student_Informations
{
    
    public sealed partial class Student_Card_Report : Page
    {
        public StudentViewModel ViewModel { get; set; }
        private DatabaseConnection _ConnectionString;

        public Student_Card_Report()
        {
            this.InitializeComponent();

            ViewModel = new StudentViewModel();
            this.DataContext = new StudentViewModel();  // Bind ViewModel to View  

            // Subscribe to ErrorMessage changes from the ViewModel
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

        private void clear_edu_level(object sender, RoutedEventArgs e)
        {
            Stu_EducationLevels_Search.SelectedItem = null;
        }

        private void clear_edu_subject(object sender, RoutedEventArgs e)
        {
            Stu_Subject_Search.SelectedItem=null;
        }

        private void clear_edu_studytime(object sender, RoutedEventArgs e)
        {
            Stu_StudyTime_Search.SelectedItem= null;
        }

        private void clear_edu_typestudy(object sender, RoutedEventArgs e)
        {
            Stu_TypeStudy_Search.SelectedItem = null;
        }

        private void clear_edu_studyyear(object sender, RoutedEventArgs e)
        {
            Stu_StudyYear_Search.SelectedItem= null;
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            Stu_StudyYear_Search.SelectedItem = null;
            Stu_TypeStudy_Search.SelectedItem = null;
            Stu_StudyTime_Search.SelectedItem = null;
            Stu_Subject_Search.SelectedItem = null;
            Stu_EducationLevels_Search.SelectedItem = null;
        }

        private void Selected_Student_for_Card_and_CV(object sender, SelectionChangedEventArgs e)
        {
            var listView_stu_card = sender as ListView;
            var selectedStudents_Card_CV = listView_stu_card.SelectedItems.Cast<Student_Info>().ToList();

            // Debug: Check selected items
            Debug.WriteLine($"Selected Items Count: {selectedStudents_Card_CV.Count}");
            // Update the ViewModel with the selected items
            if (DataContext is StudentViewModel viewModel)
            {
                viewModel.Selection_Student_Card = selectedStudents_Card_CV;
            }
        }
    }
}
