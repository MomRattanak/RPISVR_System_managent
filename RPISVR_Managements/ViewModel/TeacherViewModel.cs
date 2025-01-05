using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using RPISVR_Managements.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
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
        private readonly DatabaseConnection dbConnection;

        public ICommand CommandInsert_Teacher_Info { get; set; }

        public TeacherViewModel()
        {
            dbConnection = new DatabaseConnection();

            //Command Teacher
            CommandInsert_Teacher_Info = new RelayCommand(async () => await SubmitTeacher_Info());
            Command_Edit_Teacher = new RelayCommand(async () => await Edit_Teacher_Info());
            Command_Delete_Teacher = new RelayCommand(async () => await Delete_Teacher_Info());
            Command_Clear_Teacher = new RelayCommand(async () => await Clear_Teacher_Data());

            //List Teacher
            Teacher_Info = new ObservableCollection<Teacher_Informatioins>();

            //Get Teacher ID
            Get_TeacherID();
           
            //Load
            _ = LoadTeacherListView(SearchTeacherInfo);

        }
        //Method Get TeacherID
        public void Get_TeacherID()
        {
            //Get Teacher_ID
            var (id, teacher_id) = dbConnection.GetTeacher_ID();
            T_ID = id;
            Teacher_ID = teacher_id;
            OnPropertyChanged(nameof(Teacher_ID));
        }
        //Enable Edit
        private bool _isEditing = false;
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

        //Teacher Informations

        //Teacher_Info
        private ObservableCollection<Teacher_Informatioins> _teacher_info;
        public ObservableCollection<Teacher_Informatioins> Teacher_Info
        {
            get => _teacher_info;
            set
            {
                if(_teacher_info != value)
                {
                    _teacher_info = value;
                    OnPropertyChanged(nameof(Teacher_Info));
                }
                
            }
        }
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
        private string _errorMessage_Delete;
        public string ErrorMessage_Delete
        {
            get => _errorMessage_Delete;
            set
            {
                _errorMessage_Delete = value;
                OnPropertyChanged( nameof(ErrorMessage_Delete));
                UpdateMessage_DeleteColor();
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
        private SolidColorBrush _messageColor_Delete;
        public SolidColorBrush MessageColor_Delete
        {
            get => _messageColor_Delete;
            set
            {
                _messageColor_Delete = value;
                OnPropertyChanged(nameof(MessageColor_Delete));
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
        private ImageSource _ErrorImageSource_Delete;
        public ImageSource ErrorImageSource_Delete
        {
            get => _ErrorImageSource_Delete;
            set
            {
                _ErrorImageSource_Delete = value;
                OnPropertyChanged(nameof(ErrorImageSource_Delete));
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
        private void UpdateMessage_DeleteColor()
        {
            //    // Change the message color depending on the message content
            if (ErrorMessage_Delete.Contains("ជោគជ័យ")) // Check for success keyword in Khmer
            {
                MessageColor_Delete = new SolidColorBrush(Colors.Green); // Success: Green color
            }
            else
            {
                MessageColor_Delete = new SolidColorBrush(Colors.Red); // Error: Red color
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

            
            var UpdateTeacher = Teacher_Info.FirstOrDefault(s => s.Teacher_ID == Teacher_ID);
            if (UpdateTeacher != null)
            {
                //Update Mode
                UpdateTeacher.Teacher_ID = Teacher_ID;
                UpdateTeacher.Teacher_Name_KH = Teacher_Name_KH;
                UpdateTeacher.Teacher_Name_EN = Teacher_Name_EN;
                UpdateTeacher.Teacher_Phone = Teacher_Phone;

                Debug.WriteLine("Teacher Update Mode.");

                bool success = dbConnection.Update_Teacher_Info(UpdateTeacher);

                if(success)
                {
                    Debug.WriteLine("Update success.");     

                    //Enable Button
                    IsInsertEnabled = true;
                    IsUpdateEnabled = false;
                    
                    _ = LoadTeacherListView(SearchTeacherInfo);

                    ErrorMessage = "លេខសម្ភាល់ " + Teacher_ID + " បានធ្វើបច្ចុប្បន្នភាពជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);
                    Get_TeacherID(); 
                    ClearTextBox();
                }
                else
                {
                    Debug.WriteLine("Update faild.");
                    ErrorMessage = "លេខសម្ភាល់ " + Teacher_ID + " ធ្វើបច្ចុប្បន្នភាពបរាជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                }             
            }
            else
            {
                //Insert Mode
                Debug.WriteLine("Teacher Insert Mode.");
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
                    _ = LoadTeacherListView(SearchTeacherInfo);
                    ErrorMessage = "គ្រូបច្ចេកទេសឈ្មោះ៖ " + Teacher_Name_KH + " បានរក្សាទុកជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                    Get_TeacherID();
                    ClearTextBox();
                }
                else
                {
                    ErrorMessage = "គ្រូបច្ចេកទេសឈ្មោះ៖ " + Teacher_Name_KH + " រក្សាទុកបរាជ៏យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                } 
            }
            
        }

        // This is used to determine if the teacher can be edited (after clicking "Edit")
        public bool CanEditTeacher => First_Select_Teacher != null;
        private bool CanUpdateTeacher()
        {
            return First_Select_Teacher != null;
        }

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

        //Search by textbox.
        private string _search_teacher_info;
        public string SearchTeacherInfo
        {
            get => _search_teacher_info;
            set
            {
                if( _search_teacher_info != value)
                {
                    _search_teacher_info = value;
                    OnPropertyChanged(nameof(SearchTeacherInfo));
                    Debug.WriteLine($"Text Change: {value}");
                    OnSearchTextChanged_Teacher_Info(_search_teacher_info);
                }
            }
        }
        private async void OnSearchTextChanged_Teacher_Info(string newText_SearchTeacher)
        {
            Debug.WriteLine($"Search Text Teacher Info: {newText_SearchTeacher}");
            await LoadTeacherListView(newText_SearchTeacher);
        }

        //Load Teacher List
        public async Task LoadTeacherListView(string newText_SearchTeacher)
        {

            if (string.IsNullOrEmpty(newText_SearchTeacher))
            {
                Debug.WriteLine("Search Text Null.");

                var teacher_list = dbConnection.GetFetchTeacher_Info(newText_SearchTeacher);
                Teacher_Info.Clear();
                foreach (var teacher_info in teacher_list)
                {
                    Teacher_Info.Add(teacher_info);
                }
                return;
            }
            IsLoading = true;
            Debug.WriteLine($"Loading start: {IsLoading}");
            try
            {

                await Task.Delay(10);

                var teacher_list = dbConnection.GetFetchTeacher_Info(newText_SearchTeacher);

                //Clear
                Teacher_Info.Clear();
                Debug.WriteLine("Loading teacher infomation.");

                foreach (var teacher_info in teacher_list)
                {
                    Teacher_Info.Add(teacher_info);
                }

                Teacher_Info = new ObservableCollection<Teacher_Informatioins>(teacher_list);
            }
            finally
            {
                IsLoading = false;
                Debug.WriteLine($"Loading ends: {IsLoading}");
            }
            await Task.CompletedTask;
        }

        //Selected Teacher List
        private List<Teacher_Informatioins> _selected_Teacher;
        public List<Teacher_Informatioins>Multi_Selected_Teacher
        {
            get => _selected_Teacher;
            set
            {
                _selected_Teacher = value;
                OnPropertyChanged(nameof(Multi_Selected_Teacher));
            }
        }
        //First Selected Teacher List
        private Teacher_Informatioins _first_select_Teacher;
        public Teacher_Informatioins First_Select_Teacher
        {
            get => _first_select_Teacher;
            set
            {
                _first_select_Teacher= value;
                OnPropertyChanged(nameof(First_Select_Teacher));
            }
        }

        //Command Edit and Delete and Clear
        public ICommand Command_Edit_Teacher { get; set; }
        public ICommand Command_Delete_Teacher { get;set; }
        public ICommand Command_Clear_Teacher { get; set; }

        //Method Clear Data
        public async Task Clear_Teacher_Data()
        {
            Get_TeacherID();

            Teacher_Name_KH = null;
            OnPropertyChanged(nameof(Teacher_Name_KH));
            Teacher_Name_EN = null;
            OnPropertyChanged(nameof(Teacher_Name_EN));
            Teacher_Phone = null;
            OnPropertyChanged(nameof(Teacher_Phone));

            ErrorMessage = string.Empty;
            ErrorMessage_Delete = string.Empty;
            ErrorImageSource = null;
            ErrorImageSource_Delete = null;
            MessageColor = new SolidColorBrush(Colors.Transparent);
            MessageColor_Delete = new SolidColorBrush(Colors.Transparent);

            //Enable Button
            IsInsertEnabled = true;
            IsUpdateEnabled = false;

            await Task.CompletedTask;
        }

        private void ClearTextBox()
        {
            Teacher_Name_KH = null;
            OnPropertyChanged(nameof(Teacher_Name_KH));
            Teacher_Name_EN = null;
            OnPropertyChanged(nameof(Teacher_Name_EN));
            Teacher_Phone = null;
            OnPropertyChanged(nameof(Teacher_Phone));
        }
        //Method Edit Teacher Info
        public async Task Edit_Teacher_Info()
        {
            if(First_Select_Teacher == null)
            {
                ErrorMessage = "សូមជ្រើសរើសទិន្នន័យគ្រូបច្ចេកទេសជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            Debug.WriteLine($"First Selected Teacher Info: {First_Select_Teacher.Teacher_Name_KH}");

            //Load to TextBox
            Teacher_ID = First_Select_Teacher.Teacher_ID;
            Teacher_Name_KH = First_Select_Teacher.Teacher_Name_KH;
            Teacher_Name_EN = First_Select_Teacher.Teacher_Name_EN;
            Teacher_Phone = First_Select_Teacher.Teacher_Phone;

            //Clear Error Message
            ErrorImageSource = null;
            MessageColor = new SolidColorBrush(Colors.Transparent);

            OnPropertyChanged(nameof(First_Select_Teacher));

            if(_first_select_Teacher != null)
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
            await Task.CompletedTask;
        }

        //Method Delete Teacher
        public async Task Delete_Teacher_Info()
        {
            if(Multi_Selected_Teacher == null || !Multi_Selected_Teacher.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសទិន្នន័យគ្រូបច្ចេកទេសជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            { 
                ErrorMessage_Delete = "តើអ្នកពិតជាចង់លុបទិន្នន័យទាំងនេះមែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Yellow);
            }
            await Task.CompletedTask;
        }
        public void HandleYesResponse()
        {
            // Perform actions for "Yes"
            Debug.WriteLine("Yes response handled in ViewModel");
            Debug.WriteLine("Delete Mode.");

            foreach (var teacher_id in Multi_Selected_Teacher)
            {

                bool success = dbConnection.Delete_Teacher_Info(teacher_id.Teacher_ID);
                if (success)
                {
                    Get_TeacherID();
                    Debug.WriteLine($"Delete success ID: {teacher_id.Teacher_ID}");
                }
                else
                {
                    Debug.WriteLine("Delete Teacher Faild.");
                    break;
                }
            }


            _ = Clear_Teacher_Data();
            _ = LoadTeacherListView(SearchTeacherInfo);

            ErrorMessage = "ទិន្នន័យបានលុបដោយជោគជ័យ !";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);

        }

        public void HandleNoResponse()
        {
            // Perform actions for "No"
            Debug.WriteLine("No response handled in ViewModel");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
