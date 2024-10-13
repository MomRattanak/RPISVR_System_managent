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



namespace RPISVR_Managements.Addition_Informations
{
    public sealed partial class Add_Villages : UserControl
    {
        public AdditioinInformationViewModel ViewModel { get; set; }
        public Add_Villages()
        {
            this.InitializeComponent();
            ViewModel = new AdditioinInformationViewModel();
            this.DataContext = ViewModel;
        }
    }
}
