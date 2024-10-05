using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using RPISVR_Managements.Model;
using Google.Protobuf.WellKnownTypes;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using Microsoft.UI.Xaml.Controls;
using RPISVR_Managements.Dialog_Control;
using Microsoft.UI.Xaml;

namespace RPISVR_Managements.ViewModel
{
    public class AdditioinInformationViewModel: INotifyPropertyChanged
    {
        //Database
        private readonly DatabaseConnection _dbConnection;
        //Data in ListView
        private ObservableCollection<Education_Levels> _education_level;
        private ObservableCollection<Education_Skills> _education_skill;
        //Get_Last_Edu_ID
        private DatabaseConnection _education_LevelModel;
        //Get_Lasr_Sk_ID
        private DatabaseConnection _education_SkillModel;
        //Command Education_Level
        public ICommand SubmitCommand_Add_Information { get; }
        public ICommand ClearCommand_Education_Level { get; }
        public ICommand DeleteCommand_Education_Level { get; }
        //Command Educatioin_Skill
        public ICommand SubmitCommand_Add_Skill_Information { get; }
        public ICommand ClearCommand_Education_Skill { get; }
        public ICommand DeleteCommand_Education_Skill { get; }


        public AdditioinInformationViewModel()
        {
            //Database 
            _dbConnection = new DatabaseConnection();

            //Education_Level Mode
            //Submit Command
            SubmitCommand_Add_Information = new RelayCommand(async () => await SubmitAsync_Education_Levels());
            //Clear Command
            ClearCommand_Education_Level = new RelayCommand(async () => await ClearAsync());
            //Delete Command
            DeleteCommand_Education_Level = new RelayCommand(async () => await Delete_Education_Level_Info());
            //Data to ListView
            Education_Level_ListView = new ObservableCollection<Education_Levels>();
            //Load Data to ListView
            LoadEducation_Levels();
            //Get Edu_ID
            _education_LevelModel = new DatabaseConnection();
            var (edu_id, edu_level_id) = _education_LevelModel.Get_Edu_ID_and_Edu_Level_ID();
            Edu_ID = edu_id;
            Edu_Level_ID = edu_level_id;

            //Education_Skill Mode
            //Submit Command
            SubmitCommand_Add_Skill_Information = new RelayCommand(async () => await SubmitAsync_Education_Skills());
            //Clear Command
            ClearCommand_Education_Skill = new RelayCommand(async () => await ClearAsync());
            //Delete Command
            DeleteCommand_Education_Skill = new RelayCommand(async () => await Delete_Education_Skill_Info());
            //Data to ListView
            Education_Skill_ListView = new ObservableCollection<Education_Skills>();
            //Load Data to ListView
            LoadEducation_Skills();
            //Get Edu_ID
            _education_SkillModel = new DatabaseConnection();
            var (sk_id, edu_skill_id) = _education_SkillModel.Get_Sk_ID_and_Skill_ID();
            Sk_ID = sk_id;
            Skill_ID = edu_skill_id;
            Debug.WriteLine("Sk_ID: " + Sk_ID);
            Debug.WriteLine("Skill_ID: " + Skill_ID);


        }
        //Sk_ID
        private int _sk_id;
        public int Sk_ID
        {
            get => _sk_id;
            set
            {
                if (_sk_id != value)
                {
                    _sk_id = value;
                    OnPropertyChanged(nameof(Sk_ID));
                }
            }
        }
        //Skill_ID
        private string _skill_id;
        public string Skill_ID
        {
            get => _skill_id;
            set
            {
                if (_skill_id != value)
                {
                    _skill_id = value;
                    OnPropertyChanged(nameof(Skill_ID));
                    ValidateEdu_Skill_ID();
                }
            }
        }
        //Skill_Name_KH
        private string _skill_name_kh;
        public string Skill_Name_KH
        {
            get => _skill_name_kh;
            set
            {
                if( _skill_name_kh != value)
                {
                    _skill_name_kh = value;
                    OnPropertyChanged(nameof(Skill_Name_KH));
                    ValidateSkill_Name_KH();
                }
            }
        }
        //Skill_Name_EN
        private string _skill_name_en;
        public string Skill_Name_EN
        {
            get => _skill_name_en;
            set
            {
                if (_skill_name_en != value)
                {
                    _skill_name_en = value;
                    OnPropertyChanged(nameof(Skill_Name_EN));
                }
            }
        }
        //Skill_Name_EN
        private string _skill_name_short;
        public string Skill_Name_Short
        {
            get => _skill_name_short;
            set
            {
                if (_skill_name_short != value)
                {
                    _skill_name_short = value;
                    OnPropertyChanged(nameof(Skill_Name_Short));
                }
            }
        }

        //Edu_ID
        private int _edu_id;
        public int Edu_ID
        {
            get => _edu_id;
            set
            {
                if(_edu_id != value)
                {
                    _edu_id = value;
                    OnPropertyChanged(nameof(Edu_ID));
                }
            }
        }

        //Education_Level_ID
        private string _edu_level_id;
        public string Edu_Level_ID
        {
            get => _edu_level_id;
            set
            {
                if (_edu_level_id != value)
                {
                    _edu_level_id = value;
                    OnPropertyChanged(nameof(Edu_Level_ID));
                    ValidateEdu_Level_ID();
                }
            }
        }
        //Education_Level_Name_KH
        private string _edu_levevl_name_kh;
        public string Edu_Level_Name_KH
        {
            get => _edu_levevl_name_kh;
            set
            {
                if (_edu_levevl_name_kh != value)
                {
                    _edu_levevl_name_kh = value;
                    OnPropertyChanged(nameof(Edu_Level_Name_KH));
                    ValidateEdu_Level_Name_KH();
                }
            }
        }
        //Education_Level_Name_EN
        private string _edu_level_name_en;
        public string Edu_Level_Name_EN
        {
            get => _edu_level_name_en;
            set
            {
                if(_edu_level_name_en != value)
                {
                    _edu_level_name_en = value;
                    OnPropertyChanged(nameof(Edu_Level_Name_EN));
                }
            }
        }
        //Education_Level_Name_Short
        private string _edu_level_name_short;
        public string Edu_Level_Name_Short
        {
            get => _edu_level_name_short;
            set
            {
                if(_edu_level_name_short != value)
                {
                    _edu_level_name_short = value;
                    OnPropertyChanged(nameof(Edu_Level_Name_Short));
                }
            }
        }
        //Real-time validation method Skill_ID
        public SolidColorBrush SkIDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SkIDBorderBrush));
            }
        }
        //ValidateSkill_ID
        private void ValidateEdu_Skill_ID()
        {
            if (string.IsNullOrEmpty(Skill_ID))
            {
                SkIDBorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                SkIDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method Skill_Name_KH
        public SolidColorBrush Skill_Name_KHBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Skill_Name_KHBorderBrush));
            }
        }
        //ValidateSkill_Name_KH
        private void ValidateSkill_Name_KH()
        {
            if (string.IsNullOrEmpty(Skill_Name_KH))
            {
                Skill_Name_KHBorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Skill_Name_KHBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method Skill_Name_EN
        public SolidColorBrush Skill_Name_ENBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Skill_Name_ENBorderBrush));
            }
        }
        //ValidateSkill_Name_EN
        private void ValidateSkill_Name_EN()
        {
            if (string.IsNullOrEmpty(Skill_Name_EN))
            {
                Skill_Name_ENBorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Skill_Name_ENBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Color Border Error in Input Box.
        private SolidColorBrush _ErrorBorderBrush = new SolidColorBrush(Colors.Transparent); // Default transparent border
        //Update Color
        private void UpdateMessageColor()
        {
            //    // Change the message color depending on the message content
            if (Edu_Error_Message.Contains("ជោគជ័យ")) // Check for success keyword in Khmer
            {
                MessageColor = new SolidColorBrush(Colors.Green); // Success: Green color
            }
            else
            {
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
            }
        }
        //Edu_Error_Message
        private string _Edu_Error_Message;
        public string Edu_Error_Message
        {
            get => _Edu_Error_Message;
            set
            {
                _Edu_Error_Message = value;
                OnPropertyChanged(nameof(Edu_Error_Message));
                UpdateMessageColor();
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
        //Real-time validation method Edu_ID
        public SolidColorBrush EduIDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(EduIDBorderBrush));
            }
        }
        //ValidateEdu_Level_ID
        private void ValidateEdu_Level_ID()
        {
            if (string.IsNullOrEmpty(Edu_Level_ID))
            {
                EduIDBorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                EduIDBorderBrush = new SolidColorBrush(Colors.Green); 
            }
        }
        //Real-time validation method Edu_Name_KH
        public SolidColorBrush EduLevelNameKHBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(EduLevelNameKHBorderBrush));
            }
        }
        //ValidateEdu_Level_Name_KH
        private void ValidateEdu_Level_Name_KH()
        {
            if (string.IsNullOrEmpty(Edu_Level_Name_KH))
            {
                EduLevelNameKHBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                EduLevelNameKHBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //DeleteEducation_SkillfromDatabase
        public void Delete_Education_Skills()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var DeleteEducation_Skill = Education_Skill_ListView.FirstOrDefault(s => s.Skill_ID == Skill_ID);
            if (DeleteEducation_Skill != null)
            {
                DeleteEducation_Skill.Skill_ID = Skill_ID;

                Debug.WriteLine("Delete Mode");
                bool sucess = dbConnection.Delete_Education_Skill_Information(DeleteEducation_Skill);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_Level_ID + " ទិន្នន័យលុបបានជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_Level_ID + " ទិន្នន័យលុបបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Edu_Error_Message = "លុបទិន្នន័យបរាជ័យ";
                MessageColor = new SolidColorBrush(Colors.Red);
            }
        }
        //DeleteEducation_LevelsfromDatabase
        public void Delete_Education_Levels()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var DeleteEducation_Level = Education_Level_ListView.FirstOrDefault(s => s.Edu_Level_ID == Edu_Level_ID);
            if(DeleteEducation_Level != null)
            {
                DeleteEducation_Level.Edu_Level_ID = Edu_Level_ID;

                Debug.WriteLine("Delete Mode");
                bool sucess = dbConnection.Delete_Education_Level_Information(DeleteEducation_Level);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_Level_ID + " ទិន្នន័យលុបបានជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_Level_ID + " ទិន្នន័យលុបបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Edu_Error_Message = "លុបទិន្នន័យបរាជ័យ";
                MessageColor = new SolidColorBrush(Colors.Red);
            }
        }
        //SaveEducation_SkilltoDatabase
        public void Save_Education_Skills()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            //Update Mode
            var UpdateEducation_Skill = Education_Skill_ListView.FirstOrDefault(s => s.Skill_ID == Skill_ID);
            //Education_Level_ListView Get from top (Selected ListView).
            if (UpdateEducation_Skill != null)
            {
                Debug.WriteLine("Update Mode");
                UpdateEducation_Skill.Skill_ID = Skill_ID;
                UpdateEducation_Skill.Skill_Name_KH = Skill_Name_KH;
                UpdateEducation_Skill.Skill_Name_EN = Skill_Name_EN;
                UpdateEducation_Skill.Skill_Name_Short = Skill_Name_Short;

                bool sucess = dbConnection.Update_Education_Skill_Information(UpdateEducation_Skill);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Skill_ID + " បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Skill_ID + " បានធ្ចើបច្ចុប្បន្នភាពបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                //Insert Mode
                Education_Skills education_skill_info = new Education_Skills
                {
                    Skill_ID = this.Skill_ID,
                    Skill_Name_KH = this.Skill_Name_KH,
                    Skill_Name_EN = this.Skill_Name_EN,
                    Skill_Name_Short = this.Skill_Name_Short,
                };
                Debug.WriteLine("Insert Mode");
                bool success = dbConnection.Insert_Education_Skills(education_skill_info);

                if (success)
                {
                    Edu_Error_Message = "ទិន្នន័យបានរក្សាទុកជោគជ័យ";
                }
                else
                {
                    Edu_Error_Message = "ទិន្នន័យបានរក្សាទុកបរាជ័យ !";
                }
            }
        }
        //Method to get data to ListView
        //Data to ListView
        //private ObservableCollection<Education_Skills> _education_skill;
        public ObservableCollection<Education_Skills> Education_Skill_ListView
        {
            get => _education_skill;
            set
            {
                _education_skill = value;
                OnPropertyChanged(nameof(Education_Skill_ListView));  // Notify the UI when the Students collection changes
            }
        }
        //
        public async Task SubmitAsync_Education_Skills()
        {
            Debug.WriteLine("Test Command");
            ValidateEdu_Skill_ID();
            ValidateSkill_Name_KH();

            //Validate Skill_ID
            if (string.IsNullOrEmpty(Skill_ID))
            {
                Edu_Error_Message = "សូមបញ្ចូល លេខសម្គាល់";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            //Validate Skill_Name_KH
            if (string.IsNullOrEmpty(Skill_Name_KH))
            {
                Edu_Error_Message = "សូមបញ្ចូល ជំនាញ";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            Debug.WriteLine("Education_Skill Insert Mode");
            Debug.WriteLine($"Skill_ID: "+Skill_ID);
            Debug.WriteLine($"Skill_Name_KH: " + Skill_Name_KH);
            Debug.WriteLine($"Skill_Name_EN: " + Skill_Name_EN);
            Debug.WriteLine($"Skill_Name_Short: " + Skill_Name_Short);

            // Clear any previous error message
            Edu_Error_Message = string.Empty;
            MessageColor = null;

            Save_Education_Skills();
            LoadEducation_Skills();
            Clear_Education_Skill_Text();
            await Task.CompletedTask;
        }
        //SaveEducation_LevelstoDatabase
        public void Save_Education_Levels()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();

            //Update Mode
            var UpdateEducation_Level = Education_Level_ListView.FirstOrDefault(s => s.Edu_Level_ID == Edu_Level_ID);
            //Education_Level_ListView Get from top (Selected ListView).
            if (UpdateEducation_Level != null)
            {
                UpdateEducation_Level.Edu_Level_ID = Edu_Level_ID;
                UpdateEducation_Level.Edu_Level_Name_KH = Edu_Level_Name_KH;
                UpdateEducation_Level.Edu_Level_Name_EN = Edu_Level_Name_EN;
                UpdateEducation_Level.Edu_Level_Name_Short = Edu_Level_Name_Short;

                Debug.WriteLine("Update Mode");
                bool sucess = dbConnection.Update_Education_Level_Information(UpdateEducation_Level);
                if (sucess)
                {
                    Edu_Error_Message ="លេខសម្កាល់ "+Edu_Level_ID+" បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_Level_ID + " បានធ្ចើបច្ចុប្បន្នភាពបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                //Insert Mode
                Education_Levels education_info = new Education_Levels
                {
                    Edu_Level_ID = this.Edu_Level_ID,
                    Edu_Level_Name_KH = this.Edu_Level_Name_KH,
                    Edu_Level_Name_EN = this.Edu_Level_Name_EN,
                    Edu_Level_Name_Short = this.Edu_Level_Name_Short,
                };
                Debug.WriteLine("Insert Mode");
                bool success = dbConnection.Insert_Education_Levels(education_info);

                if (success)
                {
                    Edu_Error_Message = "ទិន្នន័យបានរក្សាទុកជោគជ័យ";
                }
                else
                {
                    Edu_Error_Message = "ទិន្នន័យបានរក្សាទុកបរាជ័យ !";
                }
            }     
        }
        public async Task Delete_Education_Level_Info()
        {
            Delete_Education_Levels();
            Clear_Education_Level_Text();
            LoadEducation_Levels();
            await Task.CompletedTask;
        }
        public async Task Delete_Education_Skill_Info()
        {
            Delete_Education_Skills();
            Clear_Education_Skill_Text();
            LoadEducation_Skills();
            await Task.CompletedTask;
        }

        public async Task SubmitAsync_Education_Levels()
        {
           ValidateEdu_Level_ID();
           ValidateEdu_Level_Name_KH();

            // Clear any previous error message
            Edu_Error_Message = string.Empty;
            MessageColor = null;

            //Validate Edu_Level_ID
            if (string.IsNullOrEmpty(Edu_Level_ID))
            {
                Edu_Error_Message = "សូមបញ្ចូល លេខសម្គាល់";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            //Validate Edu_Level_Name_KH
            if (string.IsNullOrEmpty(Edu_Level_Name_KH))
            {
                Edu_Error_Message = "សូមបញ្ចូល កម្រិតសិក្សា";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Debug.WriteLine("Insert Mode");
                Debug.WriteLine(Edu_Level_ID);
                Debug.WriteLine(Edu_Level_Name_KH);
                Debug.WriteLine(Edu_Level_Name_EN);
                Debug.WriteLine(Edu_Level_Name_Short);

            Save_Education_Levels();
            Clear_Education_Level_Text();
            LoadEducation_Levels();
            
            await Task.CompletedTask;
        }
        //
        public async Task ClearAsync()
        {
            Clear_Education_Level_Text();
            Clear_Education_Skill_Text();
            await Task.CompletedTask;
        }
        //Method for Clear Text
        public void Clear_Education_Skill_Text()
        {
            //Get Edu_ID
            _education_SkillModel = new DatabaseConnection();
            var (sk_id, edu_skill_id) = _education_SkillModel.Get_Sk_ID_and_Skill_ID();
            Sk_ID = sk_id;
            Skill_ID = edu_skill_id;
            Debug.WriteLine("Sk_ID: " + Sk_ID);
            Debug.WriteLine("Skill_ID: " + Skill_ID);
            //Skill_ID = string.Empty;
            Skill_Name_KH = string.Empty;
            Skill_Name_EN = string.Empty;
            Skill_Name_Short = string.Empty;
        }
        public void Clear_Education_Level_Text()
        {
            //Get Edu_ID
            _education_LevelModel = new DatabaseConnection();
            var (edu_id, edu_level_id) = _education_LevelModel.Get_Edu_ID_and_Edu_Level_ID();
            Edu_ID = edu_id;
            Edu_Level_ID = edu_level_id;

            Debug.WriteLine("Edu_ID: " + Edu_ID);
            Debug.WriteLine("Edu_Level_ID: " + Edu_Level_ID);
            //Edu_Level_ID = string.Empty;
            Edu_Level_Name_KH = string.Empty;
            Edu_Level_Name_EN = string.Empty;
            Edu_Level_Name_Short = string.Empty;
        }

        //Method to get data to ListView
        //Data to ListView
        //private ObservableCollection<Education_Levels> _education_level;
        public ObservableCollection<Education_Levels> Education_Level_ListView
        {
            get => _education_level;
            set
            {
                _education_level = value;
                OnPropertyChanged(nameof(Education_Level_ListView));  // Notify the UI when the Students collection changes
            }
        }
        //Load Education_Skill
        private void LoadEducation_Skills()
        {
            if (_dbConnection == null)
            {
                Debug.WriteLine("_dbConnection is not initialized.");
                return;
            }
            var education_skill_list = _dbConnection.LoadEducation_Skill();
            if(education_skill_list != null && education_skill_list.Count > 0)
            {
                // Clear the existing items in the ObservableCollection
                Education_Skill_ListView.Clear();

                // Add new items from the database
                foreach (var education_skill in education_skill_list)
                {
                    Education_Skill_ListView.Add(education_skill);
                }
                Education_Skill_ListView = new ObservableCollection<Education_Skills>(education_skill_list);
            }
        }
        //Load Education_Level
        private void LoadEducation_Levels()
        {
            // Ensure _dbConnection is properly initialized
            if (_dbConnection == null)
            {
                Debug.WriteLine("_dbConnection is not initialized.");
                return;
            }

            var education_level_list = _dbConnection.LoadEducation_Level();

            if (education_level_list != null && education_level_list.Count > 0)
            {
                // Clear the existing items in the ObservableCollection
                Education_Level_ListView.Clear();

                // Add new items from the database
                foreach (var education_level in education_level_list)
                {
                    Education_Level_ListView.Add(education_level);
                }
                Education_Level_ListView = new ObservableCollection<Education_Levels>(education_level_list);
            }
            else
            {
                Debug.WriteLine("No education levels found.");
            }
        }
        //Select Education Skill in the ListView
        private Education_Skills _selectedEducation_Skill;
        public Education_Skills SelectedEducation_Skill
        {
            get => _selectedEducation_Skill;
            set
            {
                _selectedEducation_Skill = value;
                OnPropertyChanged();
                if(_selectedEducation_Skill != null)
                {
                    Skill_ID = _selectedEducation_Skill.Skill_ID;
                    Skill_Name_KH = _selectedEducation_Skill.Skill_Name_KH;
                    Skill_Name_EN = _selectedEducation_Skill.Skill_Name_EN;
                    Skill_Name_Short = _selectedEducation_Skill.Skill_Name_Short;
                }
                OnPropertyChanged(nameof(SelectedEducation_Skill));
            }
        }
        //Select Education Level in the ListView
        private Education_Levels _selectedEducation_Level;
        public Education_Levels SelectedEducation_Level
        {
            get => _selectedEducation_Level;
            set
            {
                _selectedEducation_Level = value;
                OnPropertyChanged();

                if(_selectedEducation_Level != null)
                {
                    Edu_Level_ID = _selectedEducation_Level.Edu_Level_ID;
                    Edu_Level_Name_KH = _selectedEducation_Level.Edu_Level_Name_KH;
                    Edu_Level_Name_EN = _selectedEducation_Level.Edu_Level_Name_EN;
                    Edu_Level_Name_Short = _selectedEducation_Level.Edu_Level_Name_Short;
                }
                OnPropertyChanged(nameof(SelectedEducation_Level));
            }
        }
 

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
