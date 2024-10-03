using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Storage;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using ABI.Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using WinRT.Interop;
using RPISVR_Managements.Model;
using Org.BouncyCastle.Tls;
using System.Collections.ObjectModel;
using MySqlX.XDevAPI.Relational;
using System.ComponentModel.DataAnnotations;
using Windows.Storage.Streams;
using static System.Net.Mime.MediaTypeNames;


namespace RPISVR_Managements.ViewModel
{
    public class StudentViewModel:INotifyPropertyChanged 
    {
        private ObservableCollection<Student_Info> _students;
        private readonly DatabaseConnection _dbConnection;
        private DatabaseConnection _studentModel;


        private int _currentPage = 1;
        private int _totalPages;
        private int _pageSize = 10;
        private int _totalStudents;
        public int TotalPages => (_totalStudents + _pageSize - 1) / _pageSize;

        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        // Command to handle the form submission
        public ICommand SubmitCommand { get; }
        public ICommand ClearCommand { get; }

        public StudentViewModel()
        {
          
            SubmitCommand = new RelayCommand(async () => await SubmitAsync());
            ClearCommand = new RelayCommand(async () => await ClearAsync());
            _dbConnection = new DatabaseConnection();


            Students = new ObservableCollection<Student_Info>();
            

            //Command for Previouse,Back Button
            PreviousPageCommand = new RelayCommand(PreviousPage, CanGoPreviousPage);
            NextPageCommand = new RelayCommand(NextPage, CanGoNextPage);

            //
            IsUpdateEnabled = false;

            // Populate Days and Years
            for (int i = 1; i <= 31; i++) Days.Add(i); // Days 1-31
            for (int i = DateTime.Now.Year - 100; i <= DateTime.Now.Year + 50; i++) Years.Add(i); // Years from 100 years ago to 50 years in the future

            // Default to today's date
            SelectedDay = DateTime.Now.Day;
            SelectedYear = DateTime.Now.Year;
            SelectedKhmerMonth = KhmerMonths[DateTime.Now.Month - 1];

            Stu_Insert_by_ID = "ADMIN001";
            Stu_Insert_DateTime = DateTime.Now;
            Stu_Insert_Info = "1";
            Stu_Update_By_ID = "ADMIN001";
            Stu_Update_DateTime = DateTime.Now;
            Stu_Update_Info = "2";
            Stu_Delete_By_ID = "ADMIN001";
            Stu_Delete_DateTime= DateTime.Now;
            Stu_Delete_Info = "3";
            

            //Get ID and Stu_ID
            _studentModel = new DatabaseConnection();
            _totalStudents = _studentModel.GetTotalStudentsCount();
            var (id,stu_ID) = _studentModel.Get_ID_and_Stu_ID();   
            ID = id;
            Stu_ID = stu_ID;
            Debug.WriteLine("New ID: " + ID);
            Debug.WriteLine("New Stu_ID: " + stu_ID);

            LoadStudents();

        }
        public ObservableCollection<Student_Info> Students
        {
            get => _students;
            set
            {
                _students = value;
                OnPropertyChanged(nameof(Students));  // Notify the UI when the Students collection changes
            }
        }

        // Method to load students from the database, including images
        private void LoadStudents()
        {
            var studentsList = _dbConnection.GetStudents_Info(CurrentPage, _pageSize);
            // Clear the existing list to prepare for the new page data
            Students.Clear();
            Debug.WriteLine("Loading students for page: " + CurrentPage);
            
            // Iterate over the studentsList returned by the database and add them to the ObservableCollection
            foreach (var student in studentsList)
            {              
                Students.Add(student); 
            }
            Students = new ObservableCollection<Student_Info>(studentsList);
            
            // Raise CanExecuteChanged to update button states
            (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(PageInfo));
            }
        }

        public int TotalPageS
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(PageInfo));
            }
        }

        private void NextPage()
        {
            Debug.WriteLine("Next Page Command Executed");
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadStudents();
                OnPageChanged();
                Debug.WriteLine($"Current Page: {CurrentPage}");
            }
        }

  
        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadStudents();
                Debug.WriteLine($"Current Page: {CurrentPage}");
            }
            OnPropertyChanged(nameof(CanGoPreviousPage));  // Notify the UI to enable or disable the button
        }
        private bool CanGoPreviousPage()
        {
            return CurrentPage > 1;  // Enable only if not on the first page
        }

        private bool CanGoNextPage()
        {
            return CurrentPage < TotalPages;  // Enable only if not on the last page
        }

        private void OnPageChanged()
        {
            (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }


        //Color Border Error in Input Box.
        private SolidColorBrush _ErrorBorderBrush = new SolidColorBrush(Colors.Transparent); // Default transparent border

        //Real-time validation method Stu_ID
        public SolidColorBrush StuIDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(StuIDBorderBrush));
            }
        } 
        private void ValidateStuID()
        {
            if (string.IsNullOrEmpty(Stu_ID))
            {
                StuIDBorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                StuIDBorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        } 

        //Validation Stu_FirstName_KH
        public SolidColorBrush Stu_FirstName_KH_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_FirstName_KH_BorderBrush));
            }
        }
        private void ValidateStu_FirstName_KH()
        {
            if (string.IsNullOrEmpty(Stu_FirstName_KH))
            {
                Stu_FirstName_KH_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_FirstName_KH_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_LastName_KH
        public SolidColorBrush Stu_LastName_KH_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_LastName_KH_BorderBrush));
            }
        }
        private void ValidateStu_LastName_KH()
        {
            if (string.IsNullOrEmpty(Stu_LastName_KH))
            {
                Stu_LastName_KH_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_LastName_KH_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_FirstName_EN
        public SolidColorBrush Stu_FirstName_EN_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_FirstName_EN_BorderBrush));
            }
        }
        private void ValidateStu_FirstName_EN()
        {
            if (string.IsNullOrEmpty(Stu_FirstName_EN))
            {
                Stu_FirstName_EN_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_FirstName_EN_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_FirstName_EN
        public SolidColorBrush Stu_LastName_EN_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_LastName_EN_BorderBrush));
            }
        }
        private void ValidateStu_LastName_EN()
        {
            if (string.IsNullOrEmpty(Stu_LastName_EN))
            {
                Stu_LastName_EN_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_LastName_EN_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Levels
        public SolidColorBrush Stu_EducationLevels_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_EducationLevels_BorderBrush));
            }
        }
        private void ValidateStu_EducationLevels()
        {
            if (string.IsNullOrEmpty(Stu_EducationLevels))
            {
                Stu_EducationLevels_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_EducationLevels_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Subject
        public SolidColorBrush Stu_EducationSubjects_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_EducationSubjects_BorderBrush));
            }
        }
        private void ValidateStu_EducationSubjects()
        {
            if (string.IsNullOrEmpty(Stu_EducationSubjects))
            {
                Stu_EducationSubjects_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_EducationSubjects_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_StudyTime
        public SolidColorBrush Stu_StudyTimeShift_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_StudyTimeShift_BorderBrush));
            }
        }
        private void ValidateStu_StudyTimeShift()
        {
            if (string.IsNullOrEmpty(Stu_StudyTimeShift))
            {
                Stu_StudyTimeShift_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_StudyTimeShift_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_TypeStudy
        public SolidColorBrush Stu_TypeStudy_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_TypeStudy_BorderBrush));
            }
        }
        private void ValidateStu_Stu_TypeStudy()
        {
            if (string.IsNullOrEmpty(Stu_EducationType))
            {
                Stu_TypeStudy_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_TypeStudy_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_PhoneNumber
        public SolidColorBrush Stu_PhoneNumber_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_PhoneNumber_BorderBrush));
            }
        }
        private void ValidateStu_PhoneNumber()
        {
            if (string.IsNullOrEmpty(Stu_PhoneNumber))
            {
                Stu_PhoneNumber_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_PhoneNumber_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_NationalID
        public SolidColorBrush Stu_NationalID_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_NationalID_BorderBrush));
            }
        }
        private void ValidateStu_NationalID()
        {
            if (string.IsNullOrEmpty(Stu_NationalID))
            {
                Stu_NationalID_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_NationalID_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_StudyingTime
        public SolidColorBrush Stu_StudyingTime_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_StudyingTime_BorderBrush));
            }
        }
        private void ValidateStu_StudyingTime()
        {
            if (string.IsNullOrEmpty(Stu_StudyingTime))
            {
                Stu_StudyingTime_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_StudyingTime_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Birth_Province
        public SolidColorBrush Stu_Birth_Province_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Birth_Province_BorderBrush));
            }
        }
        private void ValidateStu_Birth_Province()
        {
            if (string.IsNullOrEmpty(Stu_Birth_Province))
            {
                Stu_Birth_Province_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Birth_Province_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Birth_Distric
        public SolidColorBrush Stu_Birth_Distric_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Birth_Distric_BorderBrush));
            }
        }
        private void ValidateStu_Birth_Distric()
        {
            if (string.IsNullOrEmpty(Stu_Birth_Distric))
            {
                Stu_Birth_Distric_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Birth_Distric_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Birth_Commune
        public SolidColorBrush Stu_Birth_Commune_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Birth_Commune_BorderBrush));
            }
        }
        private void ValidateStu_Birth_Commune()
        {
            if (string.IsNullOrEmpty(Stu_Birth_Commune))
            {
                Stu_Birth_Commune_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Birth_Commune_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Birth_Village
        public SolidColorBrush Stu_Birth_Village_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Birth_Village_BorderBrush));
            }
        }
        private void ValidateStu_Birth_Village()
        {
            if (string.IsNullOrEmpty(Stu_Birth_Village))
            {
                Stu_Birth_Village_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Birth_Village_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Live_Pro
        public SolidColorBrush Stu_Live_Pro_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Live_Pro_BorderBrush));
            }
        }
        private void ValidateStu_Live_Pro()
        {
            if (string.IsNullOrEmpty(Stu_Live_Pro))
            {
                Stu_Live_Pro_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Live_Pro_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Live_Dis
        public SolidColorBrush Stu_Live_Dis_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Live_Dis_BorderBrush));
            }
        }
        private void ValidateStu_Live_Dis()
        {
            if (string.IsNullOrEmpty(Stu_Live_Dis))
            {
                Stu_Live_Dis_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Live_Dis_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Live_Comm
        public SolidColorBrush Stu_Live_Comm_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Live_Comm_BorderBrush));
            }
        }
        private void ValidateStu_Live_Comm()
        {
            if (string.IsNullOrEmpty(Stu_Live_Comm))
            {
                Stu_Live_Comm_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Live_Comm_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Live_Vill
        public SolidColorBrush Stu_Live_Vill_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Live_Vill_BorderBrush));
            }
        }
        private void ValidateStu_Live_Vill()
        {
            if (string.IsNullOrEmpty(Stu_Live_Vill))
            {
                Stu_Live_Vill_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Live_Vill_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_StudyYear
        public SolidColorBrush Stu_StudyYear_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_StudyYear_BorderBrush));
            }
        }
        private void ValidateStu_StudyYear()
        {
            if (string.IsNullOrEmpty(Stu_StudyYear))
            {
                Stu_StudyYear_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_StudyYear_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Semester
        public SolidColorBrush Stu_Semester_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Semester_BorderBrush));
            }
        }
        private void ValidateStu_Semester()
        {
            if (string.IsNullOrEmpty(Stu_Semester))
            {
                Stu_Semester_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Semester_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Image_Total_Big
        public SolidColorBrush Stu_Image_Total_Big_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Image_Total_Big_BorderBrush));
            }
        }
        private void ValidateStu_Image_Total_Big()
        {
            if (string.IsNullOrEmpty(Stu_Image_Total_Big))
            {
                Stu_Image_Total_Big_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Image_Total_Big_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Validation Stu_Image_TotalSmall
        public SolidColorBrush Stu_Image_TotalSmall_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_Image_TotalSmall_BorderBrush));
            }
        }
        private void ValidateStu_Image_TotalSmall()
        {
            if (string.IsNullOrEmpty(Stu_Image_TotalSmall))
            {
                Stu_Image_TotalSmall_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_Image_TotalSmall_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //ID
        private int _ID;
        public int ID
        {
            get => _ID;
            set
            {
                _ID = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        //Stu_ID
        private string _Stu_ID;
        public string Stu_ID
        {
            get => _Stu_ID;
            set 
            {
                if (_Stu_ID != value)
                {
                    {
                        _Stu_ID = value;
                        OnPropertyChanged(nameof(Stu_ID));
                        ValidateStuID(); 
                    }
                }
            }
            

        }

        //Stu_FirstName_KH
        private string _Stu_FirstName_KH;
        public string Stu_FirstName_KH
        {
            get => _Stu_FirstName_KH;
            set
            {
                _Stu_FirstName_KH = value;
                OnPropertyChanged();
                ValidateStu_FirstName_KH();
            }

        }


        //Stu_LastName_KH
        private string _Stu_LastName_KH;
        public string Stu_LastName_KH
        {
            get => _Stu_LastName_KH;
            set
            {
                _Stu_LastName_KH = value;
                OnPropertyChanged();
                ValidateStu_LastName_KH();
            }

        }

        //Stu_FirstName_EN
        private string _Stu_FirstName_EN;
        public string Stu_FirstName_EN
        {
            get => _Stu_FirstName_EN;
            set 
            { 
                _Stu_FirstName_EN = value;
                OnPropertyChanged();
                ValidateStu_FirstName_EN();
            }
        }

        //Stu_LastName_EN
        private string _Stu_LastName_EN;
        public string Stu_LastName_EN
        {
            get => _Stu_LastName_EN;
            set 
            { 
                _Stu_LastName_EN = value; 
                OnPropertyChanged();
                ValidateStu_LastName_EN();
            }
        }

        //Stu_Birthday
        private DateTimeOffset? _Stu_Birthday;
        // DatePicker binding property
        public DateTimeOffset? Stu_Birthday
        {
            get => _Stu_Birthday;
            set
            {
                if (_Stu_Birthday != value)
                {
                    _Stu_Birthday = value;
                    OnPropertyChanged(nameof(Stu_Birthday)); // Notify binding update
                    OnPropertyChanged(nameof(Stu_BirthdayInKhmer)); // Update the Khmer date
                    OnPropertyChanged(nameof(Stu_BirthdayDateOnly)); // Notify that the date-only property has changed
                }
            }
        }
        // Property that returns the date in Khmer format
        public string Stu_BirthdayInKhmer
        {
            get
            {
                if (_Stu_Birthday.HasValue)
                {
                    var date = _Stu_Birthday.Value;
                    var khmerMonth = KhmerCalendarHelper.GetKhmerMonthName(date.Month);
                    return $"{date.Day} {khmerMonth} {date.Year}";
                }
                return "មិនជ្រើសរើសថ្ងៃ"; // "No date selected" in Khmer
            }
        }
        // Property that returns only the date part (DateTime) without the time
        public string Stu_BirthdayDateOnly
        {
            get => SelectedDate?.ToString("dd/MM/yyyy") ?? "No Date Selected"; 
        }

        //Stu_Gender
        private bool _IsMale;
        public bool IsMale
        {
            get => _IsMale;
            set 
            {
                if (_IsMale != value)
                {
                    _IsMale = value;
                    OnPropertyChanged(nameof(IsMale));
                }
            }
        }
        //String gender in Khmer
        public string Stu_Gender
        {
            get => _IsMale ? "ស្រី":"ប្រុស" ; // Return "ប្រុស" if IsMale is true, else "ស្រី"
        }

        //Stu_StateFamily
        private bool _IsSingle;
        public bool IsSingle
        {
            get => _IsSingle;
            set 
            { 
                if(_IsSingle !=value)
                {
                    _IsSingle = value;
                    OnPropertyChanged(nameof(IsSingle));
                }
            }
        }
        //String Stu_StateFamily in Khmer
        public string Stu_StateFamily
        {
            get => _IsSingle ? "មានគ្រួសារ":"នៅលីវ" ; //Return "នៅលីវ" if IsSingle is true, else "មានគ្រួសារ"
        }

        //Stu_Levels
        private string _Stu_EducationLevels;
        public string Stu_EducationLevels
        {
            get => _Stu_EducationLevels;
            set
            {
                if(_Stu_EducationLevels != value)
                {
                    _Stu_EducationLevels = value;
                    OnPropertyChanged(nameof(Stu_EducationLevels));
                    ValidateStu_EducationLevels();
                }
            }
        }

        //Stu_Subject
        private string _Stu_EducationSubjects;
        public string Stu_EducationSubjects
        {
            get => _Stu_EducationSubjects;
            set
            {
                if(_Stu_EducationSubjects!=value)
                {
                    _Stu_EducationSubjects = value;
                    OnPropertyChanged(nameof(Stu_EducationSubjects));
                    ValidateStu_EducationSubjects();
                }
            }
        }

        //Stu_StudyTimeShift
        private string _Stu_StudyTimeShift;
        public string Stu_StudyTimeShift
        {
            get => _Stu_StudyTimeShift;
            set
            {
                if(_Stu_StudyTimeShift != value)
                {
                    _Stu_StudyTimeShift = value;
                    OnPropertyChanged(nameof(Stu_StudyTimeShift));
                    ValidateStu_StudyTimeShift();
                }
            }
        }

        //Stu_TypeStudy
        private string _Stu_EducationType;
        public string Stu_EducationType
        {
            get => _Stu_EducationType;
            set
            {
                if(_Stu_EducationType != value)
                {
                    _Stu_EducationType = value;
                    OnPropertyChanged(nameof(Stu_EducationType));
                    ValidateStu_Stu_TypeStudy();
                }
            }
        }

        //Stu_PhoneNumber
        private string _Stu_PhoneNumber;
        public string Stu_PhoneNumber
        {
            get => _Stu_PhoneNumber;
            set
            {
                if (_Stu_PhoneNumber != value)
                {
                    _Stu_PhoneNumber = value;
                    OnPropertyChanged(nameof(Stu_PhoneNumber));
                    ValidateStu_PhoneNumber();
                }
            }
        }

        //Stu_NationalID
        private string _Stu_NationalID;
        public string Stu_NationalID
        {
            get => _Stu_NationalID;
            set
            {
                if (_Stu_NationalID != value)
                {
                    _Stu_NationalID = value;
                    OnPropertyChanged(nameof(Stu_NationalID));
                    ValidateStu_NationalID();
                }
            }
        }

        //Stu_StudyingTime
        private string _Stu_StudyingTime;
        public string Stu_StudyingTime
        {
            get => _Stu_StudyingTime;
            set
            {
                if( _Stu_StudyingTime != value)
                {
                    _Stu_StudyingTime=value;
                    OnPropertyChanged(nameof(Stu_StudyingTime));
                    ValidateStu_StudyingTime();
                }
            }
        }

        //Stu_Birth_Province
        private string _Stu_Birth_Province;
        public string Stu_Birth_Province
        {
            get => _Stu_Birth_Province;
            set
            {
                if(_Stu_Birth_Province!= value)
                {
                    _Stu_Birth_Province=value;
                    OnPropertyChanged(nameof(Stu_Birth_Province));
                    ValidateStu_Birth_Province();
                }
            }
        }

        //Stu_Birth_Distric
        private string _Stu_Birth_Distric;
        public string Stu_Birth_Distric
        {
            get => _Stu_Birth_Distric;
            set
            {
                if(_Stu_Birth_Distric!= value)
                {
                    _Stu_Birth_Distric=value;
                    OnPropertyChanged(nameof(Stu_Birth_Distric));
                    ValidateStu_Birth_Distric();
                }
            }
        }

        //Stu_Birth_Commune
        private string _Stu_Birth_Commune;
        public string Stu_Birth_Commune
        {
            get => _Stu_Birth_Commune;
            set
            {
                if(_Stu_Birth_Commune!= value)
                {
                    _Stu_Birth_Commune=value;
                    OnPropertyChanged(nameof(Stu_Birth_Commune));
                    ValidateStu_Birth_Commune();
                }
            }
        }

        //Stu_Birth_Village
        private string _Stu_Birth_Village;
        public string Stu_Birth_Village
        {
            get => _Stu_Birth_Village;
            set
            {
                if(_Stu_Birth_Village!= value)
                {
                    _Stu_Birth_Village=value;
                    OnPropertyChanged(nameof(Stu_Birth_Village));
                    ValidateStu_Birth_Village();
                }
            }
        }

        //Stu_Live_Pro
        private string _Stu_Live_Pro;
        public string Stu_Live_Pro
        {
            get => _Stu_Live_Pro;
            set
            {
                if(_Stu_Live_Pro!= value)
                {
                    _Stu_Live_Pro=value;
                    OnPropertyChanged(nameof(Stu_Live_Pro));
                    ValidateStu_Live_Pro();
                }
            }
        }

        //Stu_Live_Dis
        private string _Stu_Live_Dis;
        public string Stu_Live_Dis
        {
            get => _Stu_Live_Dis;
            set
            {
                if(_Stu_Live_Dis!= value)
                {
                    _Stu_Live_Dis=value;
                    OnPropertyChanged(nameof(Stu_Live_Dis));
                    ValidateStu_Live_Dis();
                }
            }
        }

        //Stu_Live_Comm
        private string _Stu_Live_Comm;
        public string Stu_Live_Comm
        {
            get => _Stu_Live_Comm;
            set
            {
                if(_Stu_Live_Comm!= value)
                {
                    _Stu_Live_Comm=value;
                    OnPropertyChanged(nameof(Stu_Live_Comm));
                    ValidateStu_Live_Comm();
                }
            }
        }

        //Stu_Live_Vill
        private string _Stu_Live_Vill;
        public string Stu_Live_Vill
        {
            get => _Stu_Live_Vill;
            set
            {
                if(_Stu_Live_Vill!= value)
                {
                    _Stu_Live_Vill=value;
                    OnPropertyChanged(nameof(Stu_Live_Vill));
                    ValidateStu_Live_Vill();
                }
            }
        }

        //Stu_Jobs
        private string _Stu_Jobs;
        public string Stu_Jobs
        {
            get => _Stu_Jobs;
            set
            {
                if(_Stu_Jobs!= value)
                {
                    _Stu_Jobs=value;
                    OnPropertyChanged(nameof(Stu_Jobs));
                }
            }
        }

        //Stu_School
        private string _Stu_School;
        public string Stu_School
        {
            get => _Stu_School;
            set
            {
                if (_Stu_School != value)
                {
                    _Stu_School = value;
                    OnPropertyChanged(nameof(Stu_School));
                }
            }
        }

        //Stu_StudyYear
        private string _Stu_StudyYear;
        public string Stu_StudyYear
        {
            get => _Stu_StudyYear;
            set
            {
                if (_Stu_StudyYear != value)
                {
                    _Stu_StudyYear = value;
                    OnPropertyChanged(nameof(Stu_StudyYear));
                    ValidateStu_StudyYear();
                }
            }
        }

        //Stu_Semester
        private string _Stu_Semester;
        public string Stu_Semester
        {
            get => _Stu_Semester;
            set
            {
                if (_Stu_Semester != value)
                {
                    _Stu_Semester = value;
                    OnPropertyChanged(nameof(Stu_Semester));
                    ValidateStu_Semester();
                }
            }
        }

        //Stu_Mother_Name
        private string _Stu_Mother_Name;
        public string Stu_Mother_Name
        {
            get => _Stu_Mother_Name;
            set
            {
                if (_Stu_Mother_Name != value)
                {
                    _Stu_Mother_Name = value;
                    OnPropertyChanged(nameof(Stu_Mother_Name));
                }
            }
        }

        //Stu_Mother_Phone
        private string _Stu_Mother_Phone;
        public string Stu_Mother_Phone
        {
            get => _Stu_Mother_Phone;
            set
            {
                if (_Stu_Mother_Phone != value)
                {
                    _Stu_Mother_Phone = value;
                    OnPropertyChanged(nameof(Stu_Mother_Phone));
                }
            }
        }

        //Stu_Mother_Job
        private string _Stu_Mother_Job;
        public string Stu_Mother_Job
        {
            get => _Stu_Mother_Job;
            set
            {
                if (_Stu_Mother_Job != value)
                {
                    _Stu_Mother_Job = value;
                    OnPropertyChanged(nameof(Stu_Mother_Job));
                }
            }
        }

        //Stu_Father_Name
        private string _Stu_Father_Name;
        public string Stu_Father_Name
        {
            get => _Stu_Father_Name;
            set
            {
                if (_Stu_Father_Name != value)
                {
                    _Stu_Father_Name = value;
                    OnPropertyChanged(nameof(Stu_Father_Name));
                }
            }
        }

        //Stu_Father_Phone
        private string _Stu_Father_Phone;
        public string Stu_Father_Phone
        {
            get => _Stu_Father_Phone;
            set
            {
                if (_Stu_Father_Phone != value)
                {
                    _Stu_Father_Phone = value;
                    OnPropertyChanged(nameof(Stu_Father_Phone));
                }
            }
        }

        //Stu_Father_Job
        private string _Stu_Father_Job;
        public string Stu_Father_Job
        {
            get => _Stu_Father_Job;
            set
            {
                if (_Stu_Father_Job != value)
                {
                    _Stu_Father_Job = value;
                    OnPropertyChanged(nameof(Stu_Father_Job));
                }
            }
        }

        //Stu_Image_YesNo
        private bool _IsStuImage_Yes;
        public bool IsStuImage_Yes
        {
            get => _IsStuImage_Yes;
            set
            {
                    if (_IsStuImage_Yes != value)
                    {
                        _IsStuImage_Yes = value;

                        // Clear the image source when the toggle is turned off
                        if (!_IsStuImage_Yes)
                        {
                            Stu_Image_Source = null;  // Clear the image when the toggle is off
                            ProfileImageBytes = null; // Optionally clear the byte array as well
                            Stu_Image_Total_Big = "0";
                            Stu_Image_TotalSmall = "0";
                        }

                        // Notify that IsStuImage_Yes changed and that Stu_Image_YesNo needs to update
                        OnPropertyChanged(nameof(IsStuImage_Yes));
                        OnPropertyChanged(nameof(Stu_Image_YesNo));  // Notify text update
                    }
                   
            }
        }

        //String IsImage_Yes_No in Khmer
        public string Stu_Image_YesNo
        {
            get => _IsStuImage_Yes ? "មានរូបថត" : "គ្មានរូបថត"; 
        }

        private BitmapImage _profileImageSource;  // For displaying image in the UI
        private byte[] _profileImageBytes;  // For storing image as byte array

        // Property for storing image in UI
        public BitmapImage Stu_Image_Source
        {
            get => _profileImageSource;
            set
            {
                _profileImageSource = value;
                OnPropertyChanged(nameof(Stu_Image_Source));        
            }
        }

        // Property for storing image as byte array
        public byte[] ProfileImageBytes
        {
            get => _profileImageBytes;
            set
            {
                _profileImageBytes = value;
                OnPropertyChanged(nameof(ProfileImageBytes));

                // Debug: Print the length of the byte array to confirm it has valid data
                if (_profileImageBytes != null)
                {
                    Debug.WriteLine($"ProfileImageBytes length: {_profileImageBytes.Length}");
                }
                else
                {
                    Debug.WriteLine("ProfileImageBytes is null");
                }

            }
        }

        //Stu_ImageDegree_YesNo
        private bool _Is_ImageDegree_YesNo;
        public bool Is_ImageDegree_YesNo
        {
            get => _Is_ImageDegree_YesNo;
            set
            {
                if (_Is_ImageDegree_YesNo != value)
                {

                    _Is_ImageDegree_YesNo = value;

                        // Clear the image source when the toggle is turned off
                        if (!_Is_ImageDegree_YesNo)
                        {
                            Stu_Image_Degree_Source = null;  // Clear the image when the toggle is off
                            Stu_Image_Degree_Bytes = null; // Optionally clear the byte array as well
                           
                        }
                        OnPropertyChanged(nameof(Is_ImageDegree_YesNo));
                        OnPropertyChanged(nameof(Stu_ImageDegree_YesNo));  // Notify text update  
                }
            }
        }

        

        //String Stu_ImageDegree_Yes_No in Khmer
        public string Stu_ImageDegree_YesNo
        {
            get => _Is_ImageDegree_YesNo ? "មាន" : "គ្មាន"; 
        }

        private BitmapImage _Stu_Image_Degree_Source;  // For displaying image in the UI
        public BitmapImage Stu_Image_Degree_Source
        {
            get => _Stu_Image_Degree_Source;
            set
            {
                _Stu_Image_Degree_Source = value;
                OnPropertyChanged(nameof(Stu_Image_Degree_Source));

            }
            
        }

        private byte[] _Stu_Image_Degree_Bytes;  // For storing image as byte array
        public byte[] Stu_Image_Degree_Bytes
        {
            get => _Stu_Image_Degree_Bytes;
            set
            {

                _Stu_Image_Degree_Bytes = value;
                OnPropertyChanged(nameof(Stu_Image_Degree_Bytes));

                // Debug: Print the length of the byte array to confirm it has valid data
                if (_Stu_Image_Degree_Bytes != null)
                {
                    Debug.WriteLine($"Stu_Image_Degree_Bytes length: {_Stu_Image_Degree_Bytes.Length}");
                }
                else
                {
                    Debug.WriteLine("Stu_Image_Degree_Bytes is null");
                }
            }
        }

        //Stu_ImageIDegree_YesNo
        private bool _Is_ImageBirth_Cert_YesNo;
        public bool Is_ImageBirth_Cert_YesNo
        {
            get => _Is_ImageBirth_Cert_YesNo;
            set
            {
                if (_Is_ImageBirth_Cert_YesNo != value)
                {
                    _Is_ImageBirth_Cert_YesNo = value;

                    // Clear the image source when the toggle is turned off
                    if (!_Is_ImageBirth_Cert_YesNo)
                    {
                        Stu_ImageBirth_Cert_Source = null;  // Clear the image when the toggle is off
                        Stu_ImageBirth_Cert_Bytes = null; // Optionally clear the byte array as well

                    }
                    OnPropertyChanged(nameof(Is_ImageBirth_Cert_YesNo));
                    OnPropertyChanged(nameof(Stu_ImageBirth_Cert_YesNo));  // Notify text update 
                    
                }
            }
        }
        //String Stu_ImageDegree_Yes_No in Khmer
        public string Stu_ImageBirth_Cert_YesNo
        {
            get => _Is_ImageBirth_Cert_YesNo ? "មាន" : "គ្មាន";
        }

        private BitmapImage _Stu_ImageBirth_Cert_Source;  // For displaying image in the UI
        public BitmapImage Stu_ImageBirth_Cert_Source
        {
            get => _Stu_ImageBirth_Cert_Source;
            set
            {
                _Stu_ImageBirth_Cert_Source = value;
                OnPropertyChanged();
            }
        }

        private byte[] _Stu_ImageBirth_Cert_Bytes;  // For storing image as byte array
        public byte[] Stu_ImageBirth_Cert_Bytes
        {
            get => _Stu_ImageBirth_Cert_Bytes;
            set
            {
                _Stu_ImageBirth_Cert_Bytes = value;
                OnPropertyChanged(nameof(Stu_ImageBirth_Cert_Bytes));

                // Debug: Print the length of the byte array to confirm it has valid data
                if (_Stu_ImageBirth_Cert_Bytes != null)
                {
                    Debug.WriteLine($"Stu_ImageBirth_Cert_Bytes length: {_Stu_ImageBirth_Cert_Bytes.Length}");
                }
                else
                {
                    Debug.WriteLine("Stu_ImageBirth_Cert_Bytes is null");
                }
            }
        }

        //Stu_ImageIDNation_YesNo
        private bool _Is_Stu_ImageIDNation_YesNo;
        public bool Is_Stu_ImageIDNation_YesNo
        {
            get => _Is_Stu_ImageIDNation_YesNo;
            set
            {
                if (_Is_Stu_ImageIDNation_YesNo != value)
                {
                    _Is_Stu_ImageIDNation_YesNo = value;

                    // Clear the image source when the toggle is turned off
                    if (!_Is_Stu_ImageIDNation_YesNo)
                    {
                        Stu_ImageIDNation_Source = null;  // Clear the image when the toggle is off
                        Stu_ImageIDNation_Bytes = null; // Optionally clear the byte array as well

                    }
                    OnPropertyChanged(nameof(Is_Stu_ImageIDNation_YesNo));
                    OnPropertyChanged(nameof(Stu_ImageIDNation_Source));  // Notify text update 
                    
                }
            }
        }

        //String Stu_ImageIDNation_Yes_No in Khmer
        public string Stu_ImageIDNation_YesNo
        {
            get => _Is_Stu_ImageIDNation_YesNo ? "មាន" : "គ្មាន";
        }

        private BitmapImage _Stu_ImageIDNation_Source;  // For displaying image in the UI
        public BitmapImage Stu_ImageIDNation_Source
        {
            get => _Stu_ImageIDNation_Source;
            set
            {
                _Stu_ImageIDNation_Source = value;
                OnPropertyChanged();
            }
        }

        private byte[] _Stu_ImageIDNation_Bytes;  // For storing image as byte array
        public byte[] Stu_ImageIDNation_Bytes
        {
            get => _Stu_ImageIDNation_Bytes;
            set
            {
                _Stu_ImageIDNation_Bytes = value;
                OnPropertyChanged(nameof(Stu_ImageIDNation_Bytes));

                // Debug: Print the length of the byte array to confirm it has valid data
                if (_Stu_ImageIDNation_Bytes != null)
                {
                    Debug.WriteLine($"Stu_ImageIDNation_Bytes length: {_Stu_ImageIDNation_Bytes.Length}");
                }
                else
                {
                    Debug.WriteLine("Stu_ImageIDNation_Bytes is null");
                }
            }
        }

        //Stu_Image_Poor_Card_YesNo
        private bool _Is_ImagePoor_Card_YesNo;
        public bool Is_ImagePoor_Card_YesNo
        {
            get => _Is_ImagePoor_Card_YesNo;
            set
            {
                if (_Is_ImagePoor_Card_YesNo != value)
                {
                    _Is_ImagePoor_Card_YesNo = value;

                    // Clear the image source when the toggle is turned off
                    if (!_Is_ImagePoor_Card_YesNo)
                    {
                        Stu_ImagePoor_Card_Source = null;  // Clear the image when the toggle is off
                        Stu_Image_Poor_Card_Bytes = null; // Optionally clear the byte array as well

                    }
                    OnPropertyChanged(nameof(Is_ImagePoor_Card_YesNo));
                    OnPropertyChanged(nameof(Stu_ImagePoor_Card_Source));  // Notify text update 
                    
                }
            }
        }
        //String Stu_Image_Poor_Yes_No in Khmer
        public string Stu_ImagePoor_Card_YesNo
        {
            get => _Is_ImagePoor_Card_YesNo ? "មាន" : "គ្មាន";
        }

        private BitmapImage _Stu_ImagePoor_Card_Source;  // For displaying image in the UI
        public BitmapImage Stu_ImagePoor_Card_Source
        {
            get => _Stu_ImagePoor_Card_Source;
            set
            {
                _Stu_ImagePoor_Card_Source = value;
                OnPropertyChanged();
            }
        }

        private byte[] _Stu_Image_Poor_Card_Bytes;  // For storing image as byte array
        public byte[] Stu_Image_Poor_Card_Bytes
        {
            get => _Stu_Image_Poor_Card_Bytes;
            set
            {
                _Stu_Image_Poor_Card_Bytes = value;
                OnPropertyChanged(nameof(Stu_Image_Poor_Card_Bytes));

                // Debug: Print the length of the byte array to confirm it has valid data
                if (_Stu_Image_Poor_Card_Bytes != null)
                {
                    Debug.WriteLine($"Stu_Image_Poor_Card_Bytes length: {_Stu_Image_Poor_Card_Bytes.Length}");
                }
                else
                {
                    Debug.WriteLine("Stu_Image_Poor_Card_Bytes is null");
                }
            }
        }

        //Stu_Image_Total_Big
        private string _Stu_Image_Total_Big;
        public string Stu_Image_Total_Big
        {
            get => _Stu_Image_Total_Big;
            set
            {
                if (_Stu_Image_Total_Big != value)
                {
                    _Stu_Image_Total_Big = value;
                    OnPropertyChanged(nameof(Stu_Image_Total_Big));
                    ValidateStu_Image_Total_Big();
                }
            }
        }

        //Stu_Image_TotalSmall
        private string _Stu_Image_TotalSmall;
        public string Stu_Image_TotalSmall
        {
            get => _Stu_Image_TotalSmall;
            set
            {
                if (_Stu_Image_TotalSmall != value)
                {
                    _Stu_Image_TotalSmall = value;
                    OnPropertyChanged(nameof(Stu_Image_TotalSmall));
                    ValidateStu_Image_TotalSmall();
                }
            }
        }

        //Stu_Insert_by_ID
        private string _Stu_Insert_by_ID;
        public string Stu_Insert_by_ID
        {
            get => _Stu_Insert_by_ID;
            set
            {
                _Stu_Insert_by_ID = value;
                OnPropertyChanged();
            }
                
        }

        //Stu_Insert_DateTime
        private DateTime _Stu_Insert_DateTime;
        public DateTime Stu_Insert_DateTime
        {
            get => _Stu_Insert_DateTime;
            set
            {
                _Stu_Insert_DateTime = value;
                OnPropertyChanged();
            }
        }

        //Stu_Insert_Info
        private string _Stu_Insert_Info;
        public string Stu_Insert_Info
        {
            get => _Stu_Insert_Info;
            set
            {
                _Stu_Insert_Info = value;
                OnPropertyChanged();
            }
        }

        //Stu_Update_By_ID
        private string _Stu_Update_By_ID;
        public string Stu_Update_By_ID
        {
            get => _Stu_Update_By_ID;
            set
            {
                _Stu_Update_By_ID = value;
                OnPropertyChanged();
            }
        }

        //Stu_Update_DateTime
        private DateTime _Stu_Update_DateTime;
        public DateTime Stu_Update_DateTime
        {
            get => _Stu_Update_DateTime;
            set
            {
                _Stu_Update_DateTime = value;
                OnPropertyChanged();
            }
        }

        //Stu_Update_Info
        private string _Stu_Update_Info;
        public string Stu_Update_Info
        {
            get => _Stu_Update_Info;
            set
            {
                _Stu_Update_Info = value;
                OnPropertyChanged();
            }
        }

        //Stu_Delete_By_ID
        private string _Stu_Delete_By_ID;
        public string Stu_Delete_By_ID
        {
            get => _Stu_Delete_By_ID;
            set
            {
                _Stu_Delete_By_ID = value;
                OnPropertyChanged();
            }
        }

        //Stu_Delete_DateTime
        private DateTime _Stu_Delete_DateTime;
        public DateTime Stu_Delete_DateTime
        {
            get => _Stu_Delete_DateTime;
            set
            {
                _Stu_Delete_DateTime = value;
                OnPropertyChanged();
            }
        }

        //Stu_Delete_Info
        private string _Stu_Delete_Info;
        public string Stu_Delete_Info
        {
            get => _Stu_Delete_By_ID;
            set
            {
                _Stu_Delete_By_ID = value;
                OnPropertyChanged();
            }
        }

        //Get Error Message
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
               UpdateMessageColor(); // Update the color based on the message content
            }
        }

        //Get Dynamic color to textblock
        private SolidColorBrush _messageColor;
        public SolidColorBrush MessageColor
        {
            get => _messageColor;
            set
            {
                _messageColor = value;
                OnPropertyChanged(nameof(MessageColor));
            }
        }

        //Database Icon
        private ImageSource _ErrorImageSource;
        public ImageSource ErrorImageSource
        {
            get => _ErrorImageSource;
            set
            {
                _ErrorImageSource = value;
                OnPropertyChanged(nameof(ErrorImageSource));
            }
        }

        //Update Color
        private void UpdateMessageColor()
        {
            //    // Change the message color depending on the message content
            if (ErrorMessage.Contains("ជោគជ័យ")) // Check for success keyword in Khmer
            {
                MessageColor = new SolidColorBrush(Colors.Green); // Success: Green color
            }
            else
            {
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
            }
        }

        //SaveStudentInformationtoDatabase
        public void SaveStudentInformationToDatabase()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var UpdateStudent = Students.FirstOrDefault(s => s.Stu_ID == Stu_ID);
            if (UpdateStudent != null)
            {
                UpdateStudent.ID = ID;
                UpdateStudent.Stu_ID = Stu_ID;
                UpdateStudent.Stu_FirstName_KH = Stu_FirstName_KH;
                UpdateStudent.Stu_LastName_KH = Stu_LastName_KH;
                UpdateStudent.Stu_FirstName_EN = Stu_FirstName_EN;
                UpdateStudent.Stu_LastName_EN = Stu_LastName_EN;
                UpdateStudent.Stu_Gender = Stu_Gender;
                UpdateStudent.Stu_StateFamily = Stu_StateFamily;
                UpdateStudent.Stu_BirthdayDateOnly = Stu_BirthdayDateOnly;
                UpdateStudent.Stu_EducationLevels = Stu_EducationLevels;
                UpdateStudent.Stu_EducationSubjects = Stu_EducationSubjects;
                UpdateStudent.Stu_StudyTimeShift = Stu_StudyTimeShift;
                UpdateStudent.Stu_PhoneNumber = Stu_PhoneNumber;
                UpdateStudent.Stu_EducationType = Stu_EducationType;
                UpdateStudent.Stu_NationalID = Stu_NationalID;
                UpdateStudent.Stu_StudyingTime = Stu_StudyingTime;
                UpdateStudent.Stu_Birth_Province = Stu_Birth_Province;
                UpdateStudent.Stu_Birth_Distric = Stu_Birth_Distric;
                UpdateStudent.Stu_Birth_Commune = Stu_Birth_Commune;
                UpdateStudent.Stu_Birth_Village = Stu_Birth_Village;
                UpdateStudent.Stu_Live_Pro = Stu_Live_Pro;
                UpdateStudent.Stu_Live_Dis = Stu_Live_Dis;
                UpdateStudent.Stu_Live_Comm = Stu_Live_Comm;
                UpdateStudent.Stu_Live_Vill = Stu_Live_Vill;
                UpdateStudent.Stu_Jobs = Stu_Jobs;
                UpdateStudent.Stu_School = Stu_School;
                UpdateStudent.Stu_StudyYear = Stu_StudyYear;
                UpdateStudent.Stu_Semester = Stu_Semester;
                UpdateStudent.Stu_Mother_Name = Stu_Mother_Name;
                UpdateStudent.Stu_Mother_Phone = Stu_Mother_Phone;
                UpdateStudent.Stu_Mother_Job = Stu_Mother_Job;
                UpdateStudent.Stu_Father_Name = Stu_Father_Name;
                UpdateStudent.Stu_Father_Phone = Stu_Father_Phone;
                UpdateStudent.Stu_Father_Job = Stu_Father_Job;
                UpdateStudent.Stu_Image_YesNo = Stu_Image_YesNo;
                UpdateStudent.ProfileImageBytes = ProfileImageBytes;
                UpdateStudent.Stu_Image_Total_Big = Stu_Image_Total_Big;
                UpdateStudent.Stu_Image_TotalSmall = Stu_Image_TotalSmall;
                UpdateStudent.Stu_Images_Degree_Yes_No = Stu_ImageDegree_YesNo;
                UpdateStudent.Stu_Image_Degree_Bytes = Stu_Image_Degree_Bytes;
                UpdateStudent.Stu_ImageBirth_Cert_YesNo = Stu_ImageBirth_Cert_YesNo;
                UpdateStudent.Stu_ImageBirth_Cert_Bytes = Stu_ImageBirth_Cert_Bytes;
                UpdateStudent.Stu_ImageIDNation_YesNo = Stu_ImageIDNation_YesNo;
                UpdateStudent.Stu_ImageIDNation_Bytes = Stu_ImageIDNation_Bytes;
                UpdateStudent.Stu_ImagePoor_Card_YesNo = Stu_ImagePoor_Card_YesNo;
                UpdateStudent.Stu_Image_Poor_Card_Bytes = Stu_Image_Poor_Card_Bytes;
                UpdateStudent.Stu_Insert_by_ID = Stu_Insert_by_ID;
                //UpdateStudent.Stu_Insert_DateTime = Stu_Insert_DateTime;
                //UpdateStudent.Stu_Insert_Info = Stu_Insert_Info;
                UpdateStudent.Stu_Update_By_ID = Stu_Update_By_ID;
                UpdateStudent.Stu_Update_DateTime = Stu_Update_DateTime;
                UpdateStudent.Stu_Update_Info = Stu_Update_Info;
                UpdateStudent.Stu_Delete_By_ID = Stu_Delete_By_ID;
                UpdateStudent.Stu_Delete_DateTime = Stu_Delete_DateTime;
                UpdateStudent.Stu_Delete_Info = Stu_Delete_Info;

                Debug.WriteLine("Update Mode");

                bool success = dbConnection.Update_Student_Information(UpdateStudent);

                if (success)
                {

                    ErrorMessage = "លេខសម្ភាល់ " + Stu_ID + " បានធ្វើបច្ចុប្បន្នភាពជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                }
                else
                {
                    ErrorMessage = "លេខសម្ភាល់ " + Stu_ID + " ធ្វើបច្ចុប្បន្នភាពបរាជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Debug.WriteLine("Insert Mode");
                // Debug: Check if ProfileImageBytes has valid data
                if (ProfileImageBytes != null && ProfileImageBytes.Length > 0)
                {
                    Debug.WriteLine($"Inserting student image with byte array length: {ProfileImageBytes.Length}");

                };
                Student_Info student_Info = new Student_Info
                {
                    ID = this.ID,
                    Stu_ID = this.Stu_ID,
                    Stu_FirstName_KH = this.Stu_FirstName_KH,
                    Stu_LastName_KH = this.Stu_LastName_KH,
                    Stu_FirstName_EN = this.Stu_FirstName_EN,
                    Stu_LastName_EN = this.Stu_LastName_EN,
                    Stu_Gender = this.Stu_Gender,
                    Stu_StateFamily = this.Stu_StateFamily,
                    Stu_BirthdayDateOnly = this.Stu_BirthdayDateOnly,
                    Stu_EducationLevels = this.Stu_EducationLevels,
                    Stu_EducationSubjects = this.Stu_EducationSubjects,
                    Stu_StudyTimeShift = this.Stu_StudyTimeShift,
                    Stu_PhoneNumber = this.Stu_PhoneNumber,
                    Stu_EducationType = this.Stu_EducationType,
                    Stu_NationalID = this.Stu_NationalID,
                    Stu_StudyingTime = this.Stu_StudyingTime,
                    Stu_Birth_Province = this.Stu_Birth_Province,
                    Stu_Birth_Distric = this.Stu_Birth_Distric,
                    Stu_Birth_Commune = this.Stu_Birth_Commune,
                    Stu_Birth_Village = this.Stu_Birth_Village,
                    Stu_Live_Pro = this.Stu_Live_Pro,
                    Stu_Live_Dis = this.Stu_Live_Dis,
                    Stu_Live_Comm = this.Stu_Live_Comm,
                    Stu_Live_Vill = this.Stu_Live_Vill,
                    Stu_Jobs = this.Stu_Jobs,
                    Stu_School = this.Stu_School,
                    Stu_StudyYear = this.Stu_StudyYear,
                    Stu_Semester = this.Stu_Semester,
                    Stu_Mother_Name = this.Stu_Mother_Name,
                    Stu_Mother_Phone = this.Stu_Mother_Phone,
                    Stu_Mother_Job = this.Stu_Mother_Job,
                    Stu_Father_Name = this.Stu_Father_Name,
                    Stu_Father_Phone = this.Stu_Father_Phone,
                    Stu_Father_Job = this.Stu_Father_Job,
                    Stu_Image_YesNo = this.Stu_Image_YesNo,
                    ProfileImageBytes = this.ProfileImageBytes,
                    Stu_Image_Total_Big = this.Stu_Image_Total_Big,
                    Stu_Image_TotalSmall = this.Stu_Image_TotalSmall,
                    Stu_Images_Degree_Yes_No = this.Stu_ImageDegree_YesNo,
                    Stu_Image_Degree_Bytes = this.Stu_Image_Degree_Bytes,
                    Stu_ImageBirth_Cert_YesNo = this.Stu_ImageBirth_Cert_YesNo,
                    Stu_ImageBirth_Cert_Bytes = this.Stu_ImageBirth_Cert_Bytes,
                    Stu_ImageIDNation_YesNo = this.Stu_ImageIDNation_YesNo,
                    Stu_ImageIDNation_Bytes = this.Stu_ImageIDNation_Bytes,
                    Stu_ImagePoor_Card_YesNo = this.Stu_ImagePoor_Card_YesNo,
                    Stu_Image_Poor_Card_Bytes = this.Stu_Image_Poor_Card_Bytes,
                    Stu_Insert_by_ID = this.Stu_Insert_by_ID,
                    Stu_Insert_DateTime = this.Stu_Insert_DateTime,
                    Stu_Insert_Info = this.Stu_Insert_Info,
                    Stu_Update_By_ID = this.Stu_Update_By_ID,
                    Stu_Update_DateTime = this.Stu_Update_DateTime,
                    Stu_Update_Info = this.Stu_Update_Info,
                    Stu_Delete_By_ID = this.Stu_Delete_By_ID,
                    Stu_Delete_DateTime = this.Stu_Delete_DateTime,
                    Stu_Delete_Info = this.Stu_Delete_Info,


                };
                bool success = dbConnection.Insert_Student_Information(student_Info);

                if (success)
                {

                    ErrorMessage = "លេខសម្ភាល់ " + Stu_ID + " បានរក្សាទុកជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                }
                else
                {
                    ErrorMessage = "លេខសម្ភាល់ " + Stu_ID + " រក្សាទុកបរាជ៏យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
        }

        //Method for Clear Text
        public void ClearStudentInfo()
        {
            _studentModel = new DatabaseConnection();
            var (id, stu_ID) = _studentModel.Get_ID_and_Stu_ID();
            Debug.WriteLine("New ID: " + ID);
            Debug.WriteLine("New Stu_ID: " + stu_ID);
            ID = id;
            Stu_ID = stu_ID;
            Stu_FirstName_KH =string.Empty;
            Stu_LastName_KH = string.Empty;
            Stu_FirstName_EN = string.Empty;
            Stu_LastName_EN = string.Empty ;
            //Stu_Gender = string.Empty;
            //Stu_StateFamily = string.Empty;
            //Stu_BirthdayDateOnly = null;
            //Stu_EducationLevels = string.Empty;
            //Stu_EducationSubjects = string.Empty;
            //Stu_StudyTimeShift = string.Empty;
            Stu_PhoneNumber = string.Empty;
            //Stu_EducationType = string.Empty;
            Stu_NationalID = string.Empty;
            //Stu_StudyingTime = string.Empty;
            //Stu_Birth_Province = string.Empty;
            //Stu_Birth_Distric = string.Empty;
            //Stu_Birth_Commune = string.Empty;
            //Stu_Birth_Village = string.Empty;
            //Stu_Live_Pro = string.Empty;
            //Stu_Live_Dis = string.Empty;
            //Stu_Live_Comm = string.Empty;
            //Stu_Live_Vill = string.Empty;
            Stu_Jobs = string.Empty;
            Stu_School = string.Empty;
            //Stu_StudyYear = string.Empty;
            //Stu_Semester = string.Empty;
            Stu_Mother_Name = string.Empty;
            Stu_Mother_Phone = string.Empty;
            Stu_Mother_Job = string.Empty;
            Stu_Father_Name = string.Empty;
            Stu_Father_Phone = string.Empty;
            Stu_Father_Job = string.Empty;
            //Stu_Image_YesNo = null;
            Stu_Image_Source=null;
            ProfileImageBytes = null;
            Stu_Image_Total_Big = string.Empty;
            Stu_Image_TotalSmall = string.Empty;
            //Stu_Images_Degree_Yes_No = null;
            Stu_Image_Degree_Source = null;
            Stu_Image_Degree_Bytes = null;
            //Stu_ImageBirth_Cert_YesNo = null;
            Stu_ImageBirth_Cert_Bytes = null;
            Stu_ImageBirth_Cert_Source=null;
            //Stu_ImageIDNation_YesNo = null;
            Stu_ImageIDNation_Bytes = null;
            Stu_ImageIDNation_Source=null;
            //Stu_ImagePoor_Card_YesNo = null;
            Stu_ImagePoor_Card_Source = null;   
            Stu_Image_Poor_Card_Bytes = null;
            Stu_Insert_by_ID = string.Empty;
            Stu_Insert_DateTime = this.Stu_Insert_DateTime;
            Stu_Insert_Info = this.Stu_Insert_Info;
            //Stu_Update_By_ID = string.Empty ;
            Stu_Update_DateTime = this.Stu_Update_DateTime;
            Stu_Update_Info = this.Stu_Update_Info;
            Stu_Delete_By_ID = string.Empty;
            Stu_Delete_DateTime = this.Stu_Delete_DateTime;
            Stu_Delete_Info = this.Stu_Delete_Info;

        }
        // Define an event to notify the View
        public event EventHandler RequestStuIDFocus;
        //Validation Check TextBox
        public async Task SubmitAsync()
        {
            ValidateStuID(); // Call Validate again on Submit
            ValidateStu_FirstName_KH();
            ValidateStu_LastName_KH();
            ValidateStu_FirstName_EN();
            ValidateStu_LastName_EN();
            ValidateStu_EducationLevels();
            ValidateStu_EducationSubjects();
            ValidateStu_StudyTimeShift();
            ValidateStu_PhoneNumber();
            ValidateStu_Stu_TypeStudy();
            ValidateStu_StudyingTime();
            ValidateStu_NationalID();
            ValidateStu_Birth_Province();
            ValidateStu_Birth_Distric();
            ValidateStu_Birth_Commune();
            ValidateStu_Birth_Village();
            ValidateStu_Live_Pro();
            ValidateStu_Live_Dis();
            ValidateStu_Live_Comm();
            ValidateStu_Live_Vill();
            ValidateStu_StudyYear();
            ValidateStu_Semester();
            ValidateStu_Image_Total_Big();
            ValidateStu_Image_TotalSmall();


            // Clear any previous error message
            ErrorMessage = string.Empty;
            MessageColor = null;
            // Validate Stu_ID
            if (string.IsNullOrEmpty(Stu_ID))
            {
                ErrorMessage = "ID សិស្សនិស្សិត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }


            // Validate Stu_FirstName_KH
            if (string.IsNullOrEmpty(Stu_FirstName_KH))
            {
                ErrorMessage = "គោត្តនាម ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            // Validate Stu_LastName_KH
            if (string.IsNullOrEmpty(Stu_LastName_KH))
            {
                ErrorMessage = "នាម ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_FirstName_EN
            if (string.IsNullOrEmpty(Stu_FirstName_EN))
            {
                ErrorMessage = "ត្រកូល (ឡាតាំង) ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_LastName_EN
            if (string.IsNullOrEmpty(Stu_LastName_EN))
            {
                ErrorMessage = "ឈ្មោះ (ឡាតាំង) ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Levels
            if (string.IsNullOrEmpty(Stu_EducationLevels))
            {
                ErrorMessage = "កម្រិតសិក្សា ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Subject
            if (string.IsNullOrEmpty(Stu_EducationSubjects))
            {
                ErrorMessage = "ជំនាញ ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_StudyTime
            if (string.IsNullOrEmpty(Stu_EducationType))
            {
                ErrorMessage = "វេនសិក្សា ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_TypeStudy
            if (string.IsNullOrEmpty(Stu_StudyTimeShift))
            {
                ErrorMessage = "ប្រភេទសិក្សា ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_PhoneNumber
            if (string.IsNullOrEmpty(Stu_PhoneNumber))
            {
                ErrorMessage = "លេខទូរស័ព្ទផ្ទាល់ខ្លួនសិស្សនិស្សិត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_NationalID
            if (string.IsNullOrEmpty(Stu_NationalID))
            {
                ErrorMessage = "លេខអត្តសញ្ញាណប័ណ្ណសិស្សនិស្សិត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_StudyingTime
            if (string.IsNullOrEmpty(Stu_StudyingTime))
            {
                ErrorMessage = "ឆ្នាំទី ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Birth_Province
            if (string.IsNullOrEmpty(Stu_Birth_Province))
            {
                ErrorMessage = "ខេត្តកំណើត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Birth_Distric
            if (string.IsNullOrEmpty(Stu_Birth_Distric))
            {
                ErrorMessage = "ស្រុក/ខណ្ឌកំណើត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Birth_Commune
            if (string.IsNullOrEmpty(Stu_Birth_Commune))
            {
                ErrorMessage = "ឃុំ/សង្កាត់កំណើត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Birth_Village
            if (string.IsNullOrEmpty(Stu_Birth_Village))
            {
                ErrorMessage = "ភូមិកំណើត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Live_Pro
            if (string.IsNullOrEmpty(Stu_Live_Pro))
            {
                ErrorMessage = "ខេត្តរស់នៅបច្ចុប្បន្ន ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Live_Dis
            if (string.IsNullOrEmpty(Stu_Live_Dis))
            {
                ErrorMessage = "ស្រុក/ខណ្ឌរស់នៅបច្ចុប្បន្ន ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Live_Comm
            if (string.IsNullOrEmpty(Stu_Live_Comm))
            {
                ErrorMessage = "ឃុំ/សង្កាត់រស់នៅបច្ចុប្បន្ន ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Live_Vill
            if (string.IsNullOrEmpty(Stu_Live_Vill))
            {
                ErrorMessage = "ភូមិរស់នៅបច្ចុប្បន្ន ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_StudyYear
            if (string.IsNullOrEmpty(Stu_StudyYear))
            {
                ErrorMessage = "ឆ្នាំសិក្សា ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }


            // Validate Stu_Semester
            if (string.IsNullOrEmpty(Stu_Semester))
            {
                ErrorMessage = "ឆមាស ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }


            Debug.WriteLine($"Stu_ID:{Stu_ID}");
            Debug.WriteLine($"Stu_FirstName_KH:{Stu_FirstName_KH}");
            Debug.WriteLine($"Stu_LastName_KH:{Stu_LastName_KH}");
            Debug.WriteLine($"First Name: {Stu_FirstName_EN}");
            Debug.WriteLine($"Last Name: {Stu_LastName_EN}");          
            Debug.WriteLine($"Stu_Date_Only: {Stu_BirthdayDateOnly}");
            Debug.WriteLine($"Gender: {Stu_Gender}");
            Debug.WriteLine($"FamelyS:{Stu_StateFamily}");
            Debug.WriteLine($"EducationLevel:{Stu_EducationLevels}");
            Debug.WriteLine($"EducationSubject:{Stu_EducationSubjects}");
            Debug.WriteLine($"StudyTimeShift:{Stu_StudyTimeShift}");
            Debug.WriteLine($"EducationType:{Stu_EducationType}");
            Debug.WriteLine($"Stu_PhoneNumber: {Stu_PhoneNumber}");
            Debug.WriteLine($"Stu_NationID:{Stu_NationalID}");
            Debug.WriteLine($"Stu_Year:{Stu_StudyingTime}");
            Debug.WriteLine($"Stu_Birth_Pro:{Stu_Birth_Province}");
            Debug.WriteLine($"Stu_Birth_Distric: {Stu_Birth_Distric}");
            Debug.WriteLine($"Stu_Birth_Commune:{Stu_Birth_Commune}");
            Debug.WriteLine($"Stu_Birth_Village:{Stu_Birth_Village}");
            Debug.WriteLine($"Stu_Live_Pro:{Stu_Live_Pro}");
            Debug.WriteLine($"Stu_Live_Dis: {Stu_Live_Dis}");
            Debug.WriteLine($"Stu_Live_Comm:{Stu_Live_Comm}");
            Debug.WriteLine($"Stu_Live_Vill:{Stu_Live_Vill}");
            Debug.WriteLine($"Stu_Jobs:{Stu_Jobs}");
            Debug.WriteLine($"Stu_School:{Stu_School}");
            Debug.WriteLine($"Stu_StudyYear:{Stu_StudyYear}");
            Debug.WriteLine($"Stu_Semester:{Stu_Semester}");
            Debug.WriteLine($"Stu_Mother_Name:{Stu_Mother_Name}");
            Debug.WriteLine($"Stu_Mother_Phone:{Stu_Mother_Phone}");
            Debug.WriteLine($"Stu_Mother_Job:{Stu_Mother_Job}");
            Debug.WriteLine($"Stu_Father_Name:{Stu_Father_Name}");
            Debug.WriteLine($"Stu_Father_Phone:{Stu_Father_Phone}");
            Debug.WriteLine($"Stu_Father_Job:{Stu_Father_Job}");
            Debug.WriteLine($"Stu_StudyYear: {Stu_StudyYear}");
            Debug.WriteLine($"Stu_Semester: {Stu_Semester}");
            Debug.WriteLine($"Stu_Image_YesNo:{Stu_Image_YesNo}");
            Debug.WriteLine($"Stu_ImageDegree_YesNo:{Stu_ImageDegree_YesNo}");
            Debug.WriteLine($"Stu_ImageBirth_Cert_YesNo:{Stu_ImageBirth_Cert_YesNo}");
            Debug.WriteLine($"Stu_ImageDNation_YesNo:{Stu_ImageIDNation_YesNo}");
            Debug.WriteLine($"Stu_ImagePoor_Card_YesNo:{Stu_ImagePoor_Card_YesNo}");
            Debug.WriteLine($"Stu_ImagePoor_Card_Source:{Stu_ImagePoor_Card_Source}");
            Debug.WriteLine($"Stu_Image_Poor_Card_Bytes:{Stu_Image_Poor_Card_Bytes}");
            Debug.WriteLine($"Stu_Image_Total_Big:{Stu_Image_Total_Big}");
            Debug.WriteLine($"Stu_Image_TotalSmall:{Stu_Image_TotalSmall}");


            // If everything is valid
            SaveStudentInformationToDatabase();
            ClearStudentInfo();
            LoadStudents();
            await Task.CompletedTask;
            
        }

        public async Task ClearAsync()
        {
            ClearStudentInfo();
            IsInsertEnabled = true;
            IsUpdateEnabled = false;
        }
        // Example method to display error messages
        private Task ShowErrorMessageAsync(string message)
        {
            // Show an error dialog or message to the user (this is just an example)
            System.Diagnostics.Debug.WriteLine(message);
            return Task.CompletedTask;
        }

        //KH_Date
        // Khmer month names
        public List<string> KhmerMonths { get; } = new List<string>
    {
        "មករា", "កុម្ភៈ", "មីនា", "មេសា", "ឧសភា", "មិថុនា",
        "កក្កដា", "សីហា", "កញ្ញា", "តុលា", "វិច្ឆិកា", "ធ្នូ"
    };

        // Days (1-31)
        public List<int> Days { get; } = new List<int>();
        public List<int> Years { get; } = new List<int>();

        private string _selectedKhmerMonth;
        public string SelectedKhmerMonth
        {
            get => _selectedKhmerMonth;
            set
            {
                if (_selectedKhmerMonth != value)
                {
                    _selectedKhmerMonth = value;
                    OnPropertyChanged(nameof(SelectedKhmerMonth));
                    UpdateSelectedDate();
                }
            }
        }

        private int _selectedDay;
        public int SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (_selectedDay != value)
                {
                    _selectedDay = value;
                    OnPropertyChanged(nameof(SelectedDay));
                    UpdateSelectedDate();
                }
            }
        }

        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (_selectedYear != value)
                {
                    _selectedYear = value;
                    OnPropertyChanged(nameof(SelectedYear));
                    UpdateSelectedDate();
                }
            }
        }

        // Full selected date property
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            private set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }
        // Updates the SelectedDate property based on the selected day, month, and year
        private void UpdateSelectedDate()
        {
            // Convert the selected Khmer month to a month number
            int month = KhmerCalendarHelper.GetMonthNumberByKhmerName(SelectedKhmerMonth);

            if (SelectedDay > 0 && month > 0 && SelectedYear > 0)
            {
                try
                {
                    // Set the SelectedDate property
                    SelectedDate = new DateTime(SelectedYear, month, SelectedDay);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Handle invalid date (like February 30th)
                    SelectedDate = null;
                }
            }
        }

        private BitmapImage ConvertBytesToImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                Console.WriteLine("Image bytes are null or empty");
                return null;
            }

            BitmapImage image = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(imageBytes);
                    writer.StoreAsync().GetResults();
                }

                stream.Seek(0);

                try
                {
                    image.SetSource(stream); // Load the image from the stream
                    Console.WriteLine("Image successfully loaded");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error setting image source: " + ex.Message);
                }
            }

            return image;
        }



        //End

        private Student_Info _newStudent = new Student_Info();
        private bool _isEditing = false;
        private Student_Info _selectedStudent;


        //Update
        private bool _isInsertEnabled = true;
        public bool IsInsertEnabled
        {
            get => _isInsertEnabled;
            set
            {
                _isInsertEnabled = value;
                OnPropertyChanged(nameof(IsInsertEnabled));
            }
        }

        private bool _isUpdateEnabled = false;
        public bool IsUpdateEnabled
        {
            get => _isUpdateEnabled;
            set
            {
                _isUpdateEnabled = value;
                OnPropertyChanged(nameof(IsUpdateEnabled));
            }
        }

        // The selected student in the ListView
        public Student_Info SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged();

                
                // When a student is selected, populate the existing properties
                if (_selectedStudent != null)
                {
                    Stu_ID = _selectedStudent.Stu_ID;
                    Stu_FirstName_KH = _selectedStudent.Stu_FirstName_KH;
                    Stu_LastName_KH = _selectedStudent.Stu_LastName_KH;
                    Stu_FirstName_EN = _selectedStudent.Stu_FirstName_EN;
                    Stu_LastName_EN = _selectedStudent.Stu_LastName_EN;
                    IsMale = _selectedStudent.Stu_Gender == "ស្រី";
                    IsSingle = _selectedStudent.Stu_StateFamily == "មានគ្រួសារ";
                    //Stu_BirthdayDateOnly = _selectedStudent.Stu_BirthdayDateOnly;
                    Stu_EducationLevels = _selectedStudent.Stu_EducationLevels;
                    Stu_EducationSubjects = _selectedStudent.Stu_EducationSubjects;
                    Stu_StudyTimeShift = _selectedStudent.Stu_StudyTimeShift;
                    Stu_PhoneNumber = _selectedStudent.Stu_PhoneNumber;
                    Stu_EducationType = _selectedStudent.Stu_EducationType;
                    Stu_NationalID = _selectedStudent.Stu_NationalID;
                    Stu_StudyingTime = _selectedStudent.Stu_StudyingTime;
                    Stu_Birth_Province = _selectedStudent.Stu_Birth_Province;
                    Stu_Birth_Distric = _selectedStudent.Stu_Birth_Distric;
                    Stu_Birth_Commune = _selectedStudent.Stu_Birth_Commune;
                    Stu_Birth_Village = _selectedStudent.Stu_Birth_Village;
                    Stu_Live_Pro = _selectedStudent.Stu_Live_Pro;
                    Stu_Live_Dis = _selectedStudent.Stu_Live_Dis;
                    Stu_Live_Comm = _selectedStudent.Stu_Live_Comm;
                    Stu_Live_Vill = _selectedStudent.Stu_Live_Vill;
                    Stu_Jobs = _selectedStudent.Stu_Jobs;
                    Stu_School = _selectedStudent.Stu_School;
                    Stu_StudyYear = _selectedStudent.Stu_StudyYear;
                    Stu_Semester = _selectedStudent.Stu_Semester;
                    Stu_Mother_Name = _selectedStudent.Stu_Mother_Name;
                    Stu_Mother_Phone = _selectedStudent.Stu_Mother_Phone;
                    Stu_Mother_Job = _selectedStudent.Stu_Mother_Job;
                    Stu_Father_Name = _selectedStudent.Stu_Father_Name;
                    Stu_Father_Phone = _selectedStudent.Stu_Father_Phone;
                    Stu_Father_Job = _selectedStudent.Stu_Father_Job;
                    IsStuImage_Yes = _selectedStudent.Stu_Image_YesNo == "មានរូបថត";
                    Stu_Image_Source = _selectedStudent.Stu_Image_Source;
                    ProfileImageBytes = _selectedStudent.ProfileImageBytes;
                    Stu_Image_Total_Big = _selectedStudent.Stu_Image_Total_Big;
                    Stu_Image_TotalSmall = _selectedStudent.Stu_Image_TotalSmall;
                    Is_ImageDegree_YesNo = _selectedStudent.Stu_Images_Degree_Yes_No == "មាន";
                    Stu_Image_Degree_Source = _selectedStudent.Stu_Image_Degree_Source;
                    Stu_Image_Degree_Bytes = _selectedStudent.Stu_Image_Degree_Bytes;
                    Is_ImageBirth_Cert_YesNo = _selectedStudent.Stu_ImageBirth_Cert_YesNo == "មាន";
                    Stu_ImageBirth_Cert_Source = _selectedStudent.Stu_ImageBirth_Cert_Source;
                    Stu_ImageBirth_Cert_Bytes = _selectedStudent.Stu_ImageBirth_Cert_Bytes;
                    Is_Stu_ImageIDNation_YesNo = _selectedStudent.Stu_ImageIDNation_YesNo == "មាន";
                    Stu_ImageIDNation_Source = _selectedStudent.Stu_ImageIDNation_Source;
                    Stu_ImageIDNation_Bytes = _selectedStudent.Stu_ImageIDNation_Bytes;
                    Is_ImagePoor_Card_YesNo = _selectedStudent.Stu_ImagePoor_Card_YesNo == "មាន";
                    Stu_ImagePoor_Card_Source = _selectedStudent.Stu_ImagePoor_Card_Source;
                    Stu_Image_Poor_Card_Bytes = _selectedStudent.Stu_Image_Poor_Card_Bytes;
                    Stu_Update_By_ID = _selectedStudent.Stu_Update_By_ID;
                    Stu_Update_DateTime = _selectedStudent.Stu_Update_DateTime;
                    Stu_Update_Info = _selectedStudent.Stu_Update_Info;
 

                    if (DateTime.TryParseExact(_selectedStudent.Stu_BirthdayDateOnly, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime birthday))
                    {
                        // Set individual day, month, and year values
                        SelectedDay = birthday.Day;
                        SelectedKhmerMonth = KhmerCalendarHelper.GetKhmerMonthName(birthday.Month);
                        SelectedYear = birthday.Year;
                    }
                    else
                    {
                        // Handle parsing error if the string does not match the expected format
                        Debug.WriteLine("Invalid date format for Stu_BirthdayDateOnly.");
                    }
                }
                OnPropertyChanged(nameof(SelectedStudent));
                if (_selectedStudent != null)
                {
                    // Disable Insert and Enable Update
                    IsInsertEnabled = false;
                    IsUpdateEnabled = true;
                }
                else
                {
                    // Enable Insert and Disable Update
                    IsInsertEnabled = true;
                    IsUpdateEnabled = false;
                }
            }
        }

        // Method to set the Khmer date picker values from DateTime
        public void SetKhmerDateFromDateTime(DateTime date)
        {
            SelectedDay = date.Day; // Set the day

            // Set the Khmer month (convert month number to Khmer month name)
            SelectedKhmerMonth = KhmerCalendarHelper.GetKhmerMonthName(date.Month);

            // Set the year
            SelectedYear = date.Year;
        }

        
        // This is used to determine if the student can be edited (after clicking "Edit")
        public bool CanEditStudent => SelectedStudent != null;
        private bool CanUpdateStudent()
        {
            return SelectedStudent != null;
        }


        
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    
}
