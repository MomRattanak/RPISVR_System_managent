using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RPISVR_Managements.Student_Informations.Check_Student_Informations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace RPISVR_Managements.Student_Informations.Report_Student_Informations
{
    public sealed partial class Report_Student_Info : Page
    {
        public Report_Student_Info()
        {
            this.InitializeComponent();
        }

        private void btn_click_stu_solarship_report(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(stu_solarship_report));
        }
    }
}
