using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RPISVR_Managements.Dialog_Control;
using RPISVR_Managements.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RPISVR_Managements.Addition_Informations
{
    public sealed partial class Add_Education_Levels : UserControl
    {
       
        public AdditioinInformationViewModel ViewModel { get; set; }
        public Add_Education_Levels()
        {
            this.InitializeComponent();
            //this.DataContext = new AdditioinInformationViewModel();


            ViewModel = new AdditioinInformationViewModel();
            this.DataContext = ViewModel;
            // Subscribe to ErrorMessage changes from the ViewModel
            //var viewModel = (AdditioinInformationViewModel)this.DataContext;

        }
    }
}
