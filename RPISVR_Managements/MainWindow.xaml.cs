using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RPISVR_Managements.Home;
using RPISVR_Managements.Setting.System_Setting;
using RPISVR_Managements.Student_Informations.Insert_Student_Informations;
using RPISVR_Managements.List_and_Reports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.AccessControl;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using RPISVR_Managements.Student_Informations.Check_Student_Informations;
using RPISVR_Managements.List_and_Reports.Students_Report;
using RPISVR_Managements.List_and_Reports.Students_Name_Table;
using RPISVR_Managements.List_and_Reports.Student_Score;
using RPISVR_Managements.List_and_Reports.Student_Exam_Result;
using RPISVR_Managements.Student_Informations.Report_Student_Informations;
using RPISVR_Managements.Classroom.Add_Classroom;
using RPISVR_Managements.Classroom.Prepare_Classroom;
using RPISVR_Managements.Classroom.Check_Classroom;
using RPISVR_Managements.Student_Score.Insert_Student_Score;
using RPISVR_Managements.Student_Score.Check_Student_Score;
using RPISVR_Managements.Student_Score.Report_Student_Score;
using RPISVR_Managements.Attendance.Attendance_Student;
using RPISVR_Managements.Attendance.Attendance_Teacher;
using RPISVR_Managements.Attendance.Attendance_Report;
using RPISVR_Managements.Request_Attendance.Request_Attendance_Teacher;
using RPISVR_Managements.Request_Attendance.Request_Attendance_Student;
using RPISVR_Managements.Request_Attendance.Report_Request_Attendance;
using RPISVR_Managements.Teacher_Information.Insert_Teacher_Information;
using RPISVR_Managements.Teacher_Information.Check_Teacher_Information;
using RPISVR_Managements.Administrative_letter;
using RPISVR_Managements.List_and_Reports.Teacher_Time_Teach;
using RPISVR_Managements.List_and_Reports.Time_Table_Information;
using RPISVR_Managements.List_and_Reports.List_of_skill_levels_and_shifts;
using RPISVR_Managements.List_and_Reports.Schedule;
using RPISVR_Managements.List_and_Reports.Curriculum;
using RPISVR_Managements.List_and_Reports.Monthly_Report;
using RPISVR_Managements.List_and_Reports.Semi_Annual_Report;
using RPISVR_Managements.List_and_Reports.Yearly_Report;
using RPISVR_Managements.System_Helps;
using MySql.Data.MySqlClient;
using RPISVR_Managements.ViewModel;



namespace RPISVR_Managements
{
    
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;

        public MainWindow()
        {
            this.InitializeComponent();
           

            m_AppWindow = this.AppWindow;
            m_AppWindow.Changed += AppWindow_Changed;
            Activated += MainWindow_Activated;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            AppTitleBar.Loaded += AppTitleBar_Loaded;


            ExtendsContentIntoTitleBar = true;
            if (ExtendsContentIntoTitleBar == true)
            {
                m_AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            }

            //Call Tab Close Event
            TabView.TabCloseRequested += TabView_TabClosing;

            
        }
        private void TabView_TabClosing(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            var tabViewItem = args.Tab as TabViewItem;
            if (tabViewItem != null && tabViewItem.TabIndex == 1)
            {
                // Prevent closing by not removing the tab
            }
            else
            {
                // Close the tab by removing it from the TabItems collection
                TabView.TabItems.Remove(tabViewItem);
            }
        }
        // Method to change the theme for the entire MainWindow
        public void SetTheme(ElementTheme theme)
        {
            Application_Controls.RequestedTheme = theme;
            Main_Navigation.RequestedTheme = theme;
            AppTitleBar.RequestedTheme = theme;
            Application_Controls.RequestedTheme= theme;
        }

        

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // Set the initial interactive regions.
                SetRegionsForCustomTitleBar();
            }
        }

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // Update interactive regions if the size of the window changes.
                SetRegionsForCustomTitleBar();
            }
        }
        private void SetRegionsForCustomTitleBar()
        {
            // Specify the interactive regions of the title bar.

            double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

            RightPaddingColumn.Width = new GridLength(m_AppWindow.TitleBar.RightInset / scaleAdjustment);
            LeftPaddingColumn.Width = new GridLength(m_AppWindow.TitleBar.LeftInset / scaleAdjustment);

            // Get the rectangle around the AutoSuggestBox control.
            GeneralTransform transform = TitleBarSearchBox.TransformToVisual(null);
            Rect bounds = transform.TransformBounds(new Rect(0, 0,
                                                             TitleBarSearchBox.ActualWidth,
                                                             TitleBarSearchBox.ActualHeight));
            Windows.Graphics.RectInt32 SearchBoxRect = GetRect(bounds, scaleAdjustment);

            // Get the rectangle around the PersonPicture control.
            transform = PersonAccountPic.TransformToVisual(null);
            bounds = transform.TransformBounds(new Rect(0, 0,
                                                        PersonAccountPic.ActualWidth,
                                                        PersonAccountPic.ActualHeight));
            Windows.Graphics.RectInt32 PersonPicRect = GetRect(bounds, scaleAdjustment);

            var rectArray = new Windows.Graphics.RectInt32[] { SearchBoxRect, PersonPicRect };

            Microsoft.UI.Input.InputNonClientPointerSource nonClientInputSrc =
                InputNonClientPointerSource.GetForWindowId(this.AppWindow.Id);
            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
        }
        private Windows.Graphics.RectInt32 GetRect(Rect bounds, double scale)
        {
            return new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
        }


        //Foreground Title
        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                //TitleBarTextBlock.Foreground =
                //    (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
                TitleBarIcon.Visibility = Visibility.Visible;

            }
            else
            {
            //    TitleBarTextBlock.Foreground =
            //        (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
                TitleBarIcon.Visibility = Visibility.Visible;
            }
        }
        private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidPresenterChange)
            {
                switch (sender.Presenter.Kind)
                {
                    case AppWindowPresenterKind.CompactOverlay:
                        // Compact overlay - hide custom title bar
                        // and use the default system title bar instead.
                        AppTitleBar.Visibility = Visibility.Collapsed;
                        sender.TitleBar.ResetToDefault();
                        break;

                    case AppWindowPresenterKind.FullScreen:
                        // Full screen - hide the custom title bar
                        // and the default system title bar.
                        AppTitleBar.Visibility = Visibility.Collapsed;
                        sender.TitleBar.ExtendsContentIntoTitleBar = true;
                        break;

                    case AppWindowPresenterKind.Overlapped:
                        // Normal - hide the system title bar
                        // and use the custom title bar instead.
                        AppTitleBar.Visibility = Visibility.Visible;
                        sender.TitleBar.ExtendsContentIntoTitleBar = true;
                        break;

                    default:
                        // Use the default system title bar.
                        sender.TitleBar.ResetToDefault();
                        break;
                }
            }
        }
        private void SwitchPresenter(object sender, RoutedEventArgs e)
        {
            if (AppWindow != null)
            {
                AppWindowPresenterKind newPresenterKind;
                switch ((sender as Button).Name)
                {
                    case "CompactoverlaytBtn":
                        newPresenterKind = AppWindowPresenterKind.CompactOverlay;
                        break;

                    case "FullscreenBtn":
                        newPresenterKind = AppWindowPresenterKind.FullScreen;
                        break;

                    case "OverlappedBtn":
                        newPresenterKind = AppWindowPresenterKind.Overlapped;
                        break;

                    default:
                        newPresenterKind = AppWindowPresenterKind.Default;
                        break;
                }

                // If the same presenter button was pressed as the
                // mode we're in, toggle the window back to Default.
                if (newPresenterKind == AppWindow.Presenter.Kind)
                {
                    AppWindow.SetPresenter(AppWindowPresenterKind.Default);
                }
                else
                {
                    // Else request a presenter of the selected kind
                    // to be created and applied to the window.
                    AppWindow.SetPresenter(newPresenterKind);
                }
            }
        }



        private void MainNV_Load(object sender, RoutedEventArgs e)
        {
            //var tabViewItem = new TabViewItem();
            //tabViewItem.Header = "បញ្ចូលទិន្នន័យសិស្សនិស្សិត";
            //tabViewItem.TabIndex = 1;
            //tabViewItem.IconSource = new SymbolIconSource { Symbol = Symbol.Add };
            //var frame = new Frame();
            //frame.Navigate(typeof(Insert_Student_Info));
            //tabViewItem.Content = frame;

            //TabView.TabItems.Add(tabViewItem);
            //TabView.SelectedIndex = 1;
        }

        //Convert Icon
        private IconSource ConvertIconElementToIconSource(IconElement iconElement)
        {
            if (iconElement is SymbolIcon symbolIcon)
            {
                return new SymbolIconSource() { Symbol = symbolIcon.Symbol };
            }
            else if (iconElement is FontIcon fontIcon)
            {
                return new FontIconSource()
                {
                    Glyph = fontIcon.Glyph,
                    FontFamily = fontIcon.FontFamily,
                    FontSize = fontIcon.FontSize,
                    FontWeight = fontIcon.FontWeight,
                    FontStyle = fontIcon.FontStyle
                };
            }
            else if (iconElement is PathIcon pathIcon)
            {
                return new PathIconSource() { Data = pathIcon.Data };
            }
            else if (iconElement is BitmapIcon bitmapIcon)
            {
                return new BitmapIconSource() { UriSource = bitmapIcon.UriSource };
            }
            else if (iconElement is ImageIcon imageIcon)
            {
                return new ImageIconSource() { ImageSource = imageIcon.Source };
            }
            // Handle other icon types here if necessary
            return null;
        }

        //Select Item in Navigate
        private void Main_NV_Items_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = args.SelectedItem as NavigationViewItem;

            if (selectedItem != null)
            {
                string navItemTag = selectedItem.Tag?.ToString();
                //IconElement icon = selectedItem.Icon as IconElement;
                IconSource iconSource = ConvertIconElementToIconSource(selectedItem.Icon as IconElement);

                // Check if the selected item is from FooterMenuItems
                if (sender.FooterMenuItems.Contains(selectedItem))
                {
                    // Handle footer menu item selection
                    NavigateToPage(navItemTag, iconSource);
                }
                else if (navItemTag != null)
                {
                    // Handle normal and sub-menu item selection
                    NavigateToPage(navItemTag, iconSource);
                }
                else
                {
                    // Handle main menu item without a tag
                    Debug.WriteLine("Main Menu Item Selected");
                }
            }
        }
        

        //NavigateToPage
        private void NavigateToPage(string navItemTag, IconSource iconSource)
        {
            switch (navItemTag)
            {
                case "HomePage":
                    Debug.WriteLine("Open HomePage");
                    AddTabIfNotExists("ទំព័រដើម", typeof(HomePage), iconSource);
                    break;

                case "System_Settings":
                    Debug.WriteLine("Open Settings");
                    AddTabIfNotExists("ការកំណត់", typeof(System_Settings), iconSource);
                    break;

                case "Insert_Student_Info":
                    AddTabIfNotExists("បញ្ចូលទិន្ន័យសិស្សនិស្សិត", typeof(Insert_Student_Info), iconSource);
                    Debug.WriteLine("Open Insert_Student_Info");
                    break;

                case "Check_Student_Info":
                    AddTabIfNotExists("ពិនិត្យទិន្នន័យសិស្សនិស្សិត", typeof(Check_Student_Info), iconSource);
                    Debug.WriteLine("Check_Student_Info");
                    break;

                case "Report_Student_Info":
                    AddTabIfNotExists("របាយការណ៍សិស្សិនិស្សិត", typeof(Report_Student_Info), iconSource);
                    Debug.WriteLine("Open Report_Students_Info");
                    break;
              
                case "Add_Classrooms":
                    Debug.WriteLine("Open Page Add_Classrooms");
                    AddTabIfNotExists("បង្កើតថ្នាក់រៀន", typeof(Add_Classrooms), iconSource);
                    break;

                case "Prepare_Classroom_S":
                    AddTabIfNotExists("រៀបចំសិស្សនិស្សិតតាមថ្នាក់", typeof(Prepare_Classroom_S), iconSource);
                    Debug.WriteLine("Open Page Prepare_Classrooms");
                    break;
            
                case "CheckClassInfo_Classroom":
                    AddTabIfNotExists("ត្រួតពិនិត្យថ្នាក់រៀន",typeof(CheckClassInfo_Classroom), iconSource);
                    Debug.WriteLine("Open Page Check Classroom");
                    break;

                //For Student Score
                case "Insert_Student_Score_Info":
                    AddTabIfNotExists("បញ្ចូលពិន្ទុសិស្សនិស្សិត", typeof(Insert_Student_Score_Info), iconSource);
                    Debug.WriteLine("Open Page Insert Student Score");
                    break;

                case "Check_Student_Score_Info":
                    AddTabIfNotExists("ពិនិត្យទិន្នន័យពិន្ទុ", typeof(Check_Student_Score_Info),iconSource);
                    Debug.WriteLine("Open Page Check Student Score.");
                    break;

                case "Report_Student_Score_Info":
                    AddTabIfNotExists("របាយការណ៍ពិន្ទុ", typeof(Report_Student_Score_Info), iconSource);
                    Debug.WriteLine("Open Page Report Student Score.");
                    break;

                //Attendence

                case "Insert_Attendance_Students":
                    AddTabIfNotExists("បញ្ចូលវត្តមានសិស្សនិស្សិត",typeof(Insert_Attendance_Students), iconSource);
                    Debug.WriteLine("Open Page Insert_Attendance_Students.");
                    break ;

                case "Insert_Attendance_Teacher":
                    AddTabIfNotExists("បញ្ចូលវត្តមានគ្រូបច្ចេកទេស", typeof(Insert_Attendance_Teacher), iconSource);
                    Debug.WriteLine("Open Page Insert_Attendance_Teacher.");
                    break;

                case "Check_Attendance_Students":
                    AddTabIfNotExists("ពិនិត្យវត្តមានសិស្សនិស្សិត",typeof(Check_Attendance_Students),iconSource);
                    Debug.WriteLine("Open Page Check_Attendance_Students");
                    break;

                case "Check_Attendance_Teacher":
                    AddTabIfNotExists("ពិនិត្យវត្តមានគ្រូបច្ចេកទេស",typeof(Check_Attendance_Teacher),iconSource);
                    Debug.WriteLine("Open Page Check_Attendance_Teacher");
                    break;

                case "Report_Attendance_Teacher_Late":
                    AddTabIfNotExists("បញ្ចូលចំនួនម៉ោងចូលយឺត (គ្រូ)",typeof(Report_Attendance_Teacher_Late),iconSource);
                    Debug.WriteLine("Open Page Report_Attendance_Teacher_Late");
                    break;

                case "Report_Attendance_All":
                    AddTabIfNotExists("របាយការណ៍វត្តមាន និងអវត្តមាន",typeof(Report_Attendance_All),iconSource);
                    Debug.WriteLine("Open Page Report_Attendance_All");
                    break;

                //Request_Attendance

                case "Request_Attendance_Teacher_Info":
                    AddTabIfNotExists("សំណើសុំច្បាប់របស់គ្រូបច្ចេកទេស",typeof(Request_Attendance_Teacher_Info),iconSource);
                    Debug.WriteLine("Open Page Request_Attendance_Teacher");
                    break;

                case "Check_Request_Attendance_Info":
                    AddTabIfNotExists("ទិន្នន័យសុំច្បាប់គ្រូបច្ចេកទេស",typeof(Check_Request_Attendance_Info),iconSource);
                    Debug.WriteLine("Open Page Check_Request_Attendance");
                    break;

                case "Request_Attendance_Student_Info":
                    AddTabIfNotExists("សំណើសុំច្បាប់សិស្សនិស្សិត",typeof(Request_Attendance_Student_Info),iconSource);
                    Debug.WriteLine("Open Page Request_Attendance_Student_Info");
                    break;

                case "Check_Request_Attendance_Student_Info":
                    AddTabIfNotExists("ទិន្នន័យសុំច្បាប់សិស្សនិស្សិត",typeof(Check_Request_Attendance_Student_Info), iconSource);
                    Debug.WriteLine("Open Page Check_Request_Attendance_Student_Info");
                    break;

                case "Report_Request_Attendance_Info":
                    AddTabIfNotExists("របាយការណ៍នៃការសុំច្បាប់",typeof(Report_Request_Attendance_Info), iconSource);
                    Debug.WriteLine("Open Page Report_Request_Attendance_Info");
                    break;

                case "Insert_Teacher_Info":
                    AddTabIfNotExists("បញ្ចូលទិន្នន័យគ្រូបច្ចេកទេស",typeof(Insert_Teacher_Info), iconSource);
                    Debug.WriteLine("Open Page Insert_Teacher_Info");
                    break;

                case "Check_Teacher_Info":
                    AddTabIfNotExists("ពិនិត្យទិន្នន័យគ្រូបច្ចេកទេស", typeof(Check_Teacher_Info), iconSource);
                    Debug.WriteLine("Open Page Check_Teacher_Info");
                    break;

                //Administrative_letter

                case "Certificate_of_Education_Info":
                    AddTabIfNotExists("លិខិតបញ្ជាក់ការសិក្សា",typeof(Certificate_of_Education_Info),iconSource);
                    break;

                case "Transcript_of_Study_Records_Info":
                    AddTabIfNotExists("ព្រឹត្តិបត្រពិន្ទុ",typeof(Transcript_of_Study__Records_Info),iconSource);
                    break;

                case "Provision_Certificate_Info":
                    AddTabIfNotExists("សញ្ញាបត្របណ្ដោះអាសន្ន",typeof(Provision_Certificate_Info),iconSource);
                    break;

                //List_and_Report

                case "Student_Name_Table_Info":
                    AddTabIfNotExists("តារាងបញ្ជីឈ្មោះសិស្សនិស្សិត",typeof(Student_Name_Table_Info),iconSource);
                    break;

                case "Student_Score_Table_Info":
                    AddTabIfNotExists("តារាងពិន្ទុសិស្សនិស្សិត", typeof(Student_Score_Table_Info), iconSource);
                    break;

                case "Student_Exam_Result_Info":
                    AddTabIfNotExists("លទ្ធផលប្រឡងសិស្សនិស្សិត",typeof(Student_Exam_Result_Info),iconSource);
                    break;

                case "Teacher_Time_Teach_Info":
                    AddTabIfNotExists("ចំនួនម៉ោងបង្រៀនរបស់គ្រូបច្ចេកទេស", typeof(Teacher_Time_Teach_Info),iconSource);
                    break;

                case "Time_Table_Info":
                    AddTabIfNotExists("តារាងចំនួនម៉ោង", typeof(Time_Table_Info), iconSource);
                    break;

                case "List_of_skill_levels_and_shifts_Info":
                    AddTabIfNotExists("តារាងកម្រិត ជំនាញ និងវេនសិក្សា",typeof(List_of_skill_levels_and_shifts_Info),iconSource);
                    break;

                case "Schedule_Info":
                    AddTabIfNotExists("កាលវិភាគ",typeof(Schedule_Info),iconSource);
                    break;

                case "Curriculum_Info":
                    AddTabIfNotExists("កម្មវិធីសិក្សា", typeof(Curriculum_Info), iconSource);
                    break;

                case "Report_Attendance_All_Info":
                    AddTabIfNotExists("របាយការណ៍វត្តមាន និងអវត្តមាន",typeof(Report_Attendance_All),iconSource);
                    break;

                case "Report_Attendance_Teacher_Late_Info":
                    AddTabIfNotExists("របាយការណ៍ចូលយឺតចេញមុន", typeof(Report_Attendance_Teacher_Late), iconSource);
                    break;

                case "Monthly_Report_Info":
                    AddTabIfNotExists("របាយការណ៍ប្រចាំខែ",typeof(Monthly_Report_Info),iconSource);
                    break;

                case "Semi_Annual_Report_Info":
                    AddTabIfNotExists("របាយការណ៍ប្រចាំឆមាស", typeof(Semi_Annual_Report_Info), iconSource);
                    break;

                case "Annual_Report_Info":
                    AddTabIfNotExists("របាយការណ៍ប្រចាំឆ្នាំ", typeof(Annual_Report_Info), iconSource);
                    break;

                case "System_Helps_Info":
                    AddTabIfNotExists("ជំនួយ",typeof(System_Helps_Info),iconSource);
                    break;


                default:
                    Debug.WriteLine("Unrecognized Navigation Tag");
                    break;
            }
        }
        //Add new Tabview and Check if it duplicate
        private void AddTabIfNotExists(string header, Type pageType, IconSource iconSource)
        {
            // Check if the tab with the same header already exists
            foreach (TabViewItem tab in TabView.TabItems)
            {
                if (tab.Header.ToString() == header)
                {
                    // If it exists, select the tab and return
                    TabView.SelectedItem = tab;
                    return;
                }              
                
            }

            // If no duplicate tab is found, create a new one
            var tabViewItem = new TabViewItem
            {
                Header = header,
                IconSource = iconSource
            };


            var frame = new Frame();
            frame.Navigate(pageType);
            tabViewItem.Content = frame;

            TabView.TabItems.Add(tabViewItem);
            TabView.SelectedItem = tabViewItem; // Select the newly added tab
        }

        private void TabView_TabItemsChanged(TabView sender, IVectorChangedEventArgs args)
        {

        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        private void CloseAllTabs_Click(object sender, RoutedEventArgs e)
        {
            var tabsToClose = TabView.TabItems.ToList();
            foreach (var tab in tabsToClose)
            {
                if (tab is TabViewItem tabViewItem && tabViewItem.TabIndex != 1) // Skip the tab with TabIndex 1
                {
                    TabView.TabItems.Remove(tabViewItem);
                }
            }
        }

        private void TabView_Load(object sender, RoutedEventArgs e)
        {
            var tabViewItem = new TabViewItem();
            tabViewItem.Header = "ទំព័រដើម";
            tabViewItem.TabIndex = 1;
            tabViewItem.IconSource = new SymbolIconSource { Symbol = Symbol.Home };
            tabViewItem.IsClosable = false; // Disable the close button
            var frame = new Frame();
            frame.Navigate(typeof(HomePage));
            tabViewItem.Content = frame;

            TabView.TabItems.Add(tabViewItem);
            
            TabView.SelectedIndex = 1;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
