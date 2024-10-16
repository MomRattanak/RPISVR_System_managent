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
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RPISVR_Managements.Addition_Informations
{
    public sealed partial class Add_Communces : UserControl
    {
        public AdditioinInformationViewModel ViewModel { get; set; }
        public Add_Communces()
        {  
            this.InitializeComponent();
            ViewModel = new AdditioinInformationViewModel();
            this.DataContext = ViewModel;
        }
    }
}
