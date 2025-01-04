using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using RPISVR_Managements.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RPISVR_Managements.ViewModel
{
    public class TeacherViewModel:INotifyPropertyChanged
    {
        public ICommand CommandInsert_Teacher_Info { get; set; }

        public TeacherViewModel()
        {
            CommandInsert_Teacher_Info = new RelayCommand(async () => await SubmitTeacher_Info());
        }

        //Teacher Informations

        //Teacher ID
        private int _T_ID;
        public int T_ID
        {
            get => _T_ID;
            set
            {
                _T_ID = value;
                OnPropertyChanged(nameof(T_ID));
            }
        }
        private string _Teacher_ID;
        public string Teacher_ID
        {
            get => _Teacher_ID;
            set
            {
                _Teacher_ID = value;
                OnPropertyChanged(nameof(Teacher_ID));
                ValidateTeacher_ID();
            }
        }

        //Teacher Name KH
        private string _Teacher_Name_KH;
        public string Teacher_Name_KH
        {
            get => _Teacher_Name_KH;
            set
            {
                _Teacher_Name_KH = value;
                OnPropertyChanged(nameof(Teacher_Name_KH));
                ValidateTeacher_Name_KH();
            }
        }
        //Teacher Name EN
        private string _Teacher_Name_EN;
        public string Teacher_Name_EN
        {
            get => _Teacher_Name_EN;
            set
            {
                _Teacher_Name_EN = value;
                OnPropertyChanged(nameof(Teacher_Name_EN));
                ValidateTeacher_Name_EN();
            }
        }
        //Teacher Phone
        private string _Teacher_Phone;
        public string Teacher_Phone
        {
            get => _Teacher_Phone;
            set
            {
                _Teacher_Phone = value;
                OnPropertyChanged(nameof(Teacher_Phone));
                ValidateTeacher_Phone();
            }
        }

        //Color Border Error in Input Box.
        private SolidColorBrush _ErrorBorderBrush = new SolidColorBrush(Colors.Transparent);

        //Validation Teacher_ID
        public SolidColorBrush Teacher_ID_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Teacher_ID_BorderBrush));
            }
        }
        //Validation Teacher_Name_KH
        public SolidColorBrush Teacher_Name_KH_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Teacher_Name_KH_BorderBrush));
            }
        }
        //Validation Teacher_Name_EN
        public SolidColorBrush Teacher_Name_EN_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Teacher_Name_EN_BorderBrush));
            }
        }
        //Validation Teacher_Phone
        public SolidColorBrush Teacher_Phone_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Teacher_Phone_BorderBrush));
            }
        }
        private void ValidateTeacher_Name_KH()
        {
            if (string.IsNullOrEmpty(Teacher_Name_KH))
            {
                Teacher_Name_KH_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Teacher_Name_KH_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }
        private void ValidateTeacher_ID()
        {
            if (string.IsNullOrEmpty(Teacher_ID))
            {
                Teacher_ID_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Teacher_ID_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }
        private void ValidateTeacher_Name_EN()
        {
            if (string.IsNullOrEmpty(Teacher_Name_EN))
            {
                Teacher_Name_EN_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Teacher_Name_EN_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }
        private void ValidateTeacher_Phone()
        {
            if (string.IsNullOrEmpty(Teacher_Phone))
            {
                Teacher_Phone_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Teacher_Phone_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
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

        public async Task SubmitTeacher_Info()
        {
            ValidateTeacher_ID();
            ValidateTeacher_Name_KH();
            ValidateTeacher_Name_EN();
            ValidateTeacher_Phone();

            // Clear any previous error message
            ErrorMessage = string.Empty;
            MessageColor = null;
            ErrorImageSource = null;

            if (string.IsNullOrEmpty(Teacher_ID))
            {
                ErrorMessage = "លេខសម្គាល់ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            // Validate Teacher_Name_KH
            if (string.IsNullOrEmpty(Teacher_Name_KH))
            {
                ErrorMessage = "ឈ្មោះជាភាសាខ្មែរ ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            // Validate Teacher_Name_EN
            if (string.IsNullOrEmpty(Teacher_Name_EN))
            {
                ErrorMessage = "ឈ្មោះ English ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            // Validate Teacher_Phone
            if (string.IsNullOrEmpty(Teacher_Phone))
            {
                ErrorMessage = "លេខទូរស័ព្ទ ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            SaveTeacherInfomation();

            await Task.CompletedTask;
        }

        //Method SaveTeacher Info
        public async void SaveTeacherInfomation()
        {
            Debug.WriteLine("Click Save Teacher Infomation.");
            Debug.WriteLine($"ID: {Teacher_ID}");
            Debug.WriteLine($"Name KH: {Teacher_Name_KH}");
            Debug.WriteLine($"Name EN: {Teacher_Name_EN}");
            Debug.WriteLine($"Phone: {Teacher_Phone}");

            DatabaseConnection dbConnection = new DatabaseConnection();

            //Check Teacher Before Insert or Update
            var teacher_check_first =await dbConnection.GetTeacher_Info_Check( Teacher_Name_KH, Teacher_Name_EN, Teacher_Phone);

            if(teacher_check_first.Teacher_Name_KH1 == Teacher_Name_KH &&
                teacher_check_first.Teacher_Name_EN1 == Teacher_Name_EN &&
                teacher_check_first.Teacher_Phone1 == Teacher_Phone)
            {
                    ErrorMessage = "គ្រូបច្ចេកទេសឈ្មោះ៖ " + Teacher_Name_KH + " មានទិន្នន័យរួចស្រេចហើយ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
            }

            //Update
            //var UpdateTeacher = Teacher_Informatioins.Fir
            Teacher_Informatioins teacher_info = new Teacher_Informatioins()
            {
                Teacher_ID = this.Teacher_ID,
                Teacher_Name_KH = this.Teacher_Name_KH,
                Teacher_Name_EN = this.Teacher_Name_EN,
                Teacher_Phone = this.Teacher_Phone
            };

            bool success = dbConnection.Insert_TeacherInfomations(teacher_info);

            if (success)
            {

                ErrorMessage = "គ្រូបច្ចេកទេសឈ្មោះ៖ " + Teacher_Name_KH + " បានរក្សាទុកជោគជ័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);

            }
            else
            {
                ErrorMessage = "គ្រូបច្ចេកទេសឈ្មោះ៖ " + Teacher_Name_KH + " រក្សាទុកបរាជ៏យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
            }
        }

        // This is used to determine if the student can be edited (after clicking "Edit")
        //public bool CanEditStudent => SelectedStudent != null;
        //private bool CanUpdateStudent()
        //{
        //    return SelectedStudent != null;
        //}

        //Loading
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
