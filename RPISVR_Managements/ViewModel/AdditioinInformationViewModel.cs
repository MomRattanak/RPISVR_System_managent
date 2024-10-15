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
using System.Diagnostics.Eventing.Reader;
using Windows.Graphics.Printing;
using System.Data.Common;
using System.Data;

namespace RPISVR_Managements.ViewModel
{
    public class AdditioinInformationViewModel : INotifyPropertyChanged
    {
        //Database
        private readonly DatabaseConnection _dbConnection;

        //Data in ListView
        private ObservableCollection<Education_Levels> _education_level;
        private ObservableCollection<Education_Skills> _education_skill;
        private ObservableCollection<Education_StudyTimeShift> _education_studytimeshift;
        private ObservableCollection<Education_TypeStudy> _education_typestudy;
        private ObservableCollection<Education_StudyYear> _education_studyyear;
        private ObservableCollection<Provinces_Info> _provinces_info;
        private ObservableCollection<Districts_Info> _district_info;
        private ObservableCollection<Communes_Info> _communes_info;
        private ObservableCollection<Village_Info> _village_info;

        //Get_Last_Edu_ID
        private DatabaseConnection _education_LevelModel;
        //Get_Last_Sk_ID
        private DatabaseConnection _education_SkillModel;
        //Get_Last_STS_ID
        private DatabaseConnection _education_StudyTimeShiftModel;
        //Get_Last_TS_ID
        private DatabaseConnection _education_TypeStudyModel;
        //Get_Last_SY_ID
        private DatabaseConnection _education_StudyYearModel;
        //Get_Last_PV_ID
        private DatabaseConnection _province_InfoModel;
        //Get_Last_DS_-ID
        private DatabaseConnection _district_InfoModel;
        //Get_Last_CD_ID
        private DatabaseConnection _commune_InfoModel;
        //Get_Last_VL_ID
        private DatabaseConnection _village_InfoModel;

        //Command Education_Level
        public ICommand SubmitCommand_Add_Information { get; }
        public ICommand ClearCommand_Education_Level { get; }
        public ICommand DeleteCommand_Education_Level { get; }
        //Command Educatioin_Skill
        public ICommand SubmitCommand_Add_Skill_Information { get; }
        public ICommand ClearCommand_Education_Skill { get; }
        public ICommand DeleteCommand_Education_Skill { get; }
        //Command Education_StudyTimeShift
        public ICommand SubmitCommand_Add_StudyTimeShift_Information { get; }
        public ICommand ClearCommand_Education_StudyTimeShift { get; }
        public ICommand DeleteCommand_Education_StudyTimeShift { get; }
        //Command Education_TypeStudy
        public ICommand SubmitCommand_Add_TypeStudy_Information { get; }
        public ICommand ClearCommand_Education_TypeStudy { get; }
        public ICommand DeleteCommand_Education_TypeStudy { get; }
        //Command Education_StudyYear
        public ICommand SubmitCommand_Add_StudyYear_Information { get; }
        public ICommand ClearCommand_Education_StudyYear { get; }
        public ICommand DeleteCommand_Education_StudyYear { get; }
        //Command Add_Province
        public ICommand SubmitCommand_Add_Province_Information { get; }
        public ICommand ClearCommand_Province { get; }
        public ICommand DeleteCommand_Province { get; }
        //Command Add_District
        public ICommand SubmitCommand_Add_District_Information { get; }
        public ICommand ClearCommand_District { get; }
        public ICommand DeleteCommand_District { get; }
        //Command Add_Commune
        public ICommand SubmitCommand_Add_Commune_Information { get; }
        public ICommand ClearCommand_Commune { get; }
        public ICommand DeleteCommand_Commune { get; }
        //Command Add_Village
        public ICommand SubmitCommand_Add_Village_Information { get; }
        public ICommand ClearCommand_Village { get; }
        public ICommand DeleteCommand_Village { get; }


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

            //Education_StudyTimeShift Mode
            //Submit Command
            SubmitCommand_Add_StudyTimeShift_Information = new RelayCommand(async () => await SubmitAsync_Education_StudyTimeShifts());
            //Clear Command
            ClearCommand_Education_StudyTimeShift = new RelayCommand(async () => await ClearAsync());
            //Delete Command
            DeleteCommand_Education_StudyTimeShift = new RelayCommand(async () => await Delete_Education_StudyTimeShift_Info());
            //Data to ListView
            Education_StudyTimeShift_ListView = new ObservableCollection<Education_StudyTimeShift>();
            //Load Data to ListView
            LoadEducation_StudyTimeShift();
            //Get STS_ID
            _education_StudyTimeShiftModel = new DatabaseConnection();
            var (sts_id, studytimeshift_id) = _education_StudyTimeShiftModel.Get_STS_ID_and_StudyTimeShift_ID();
            STS_ID = sts_id;
            StudyTimeShift_ID = studytimeshift_id;

            //Education_TypeStudy Mode
            //Submit Command
            SubmitCommand_Add_TypeStudy_Information = new RelayCommand(async () => await SubmitAsync_Education_TypeStudys());
            //Clear Command
            ClearCommand_Education_TypeStudy = new RelayCommand(async () => await ClearAsync());
            //Delete Command
            DeleteCommand_Education_TypeStudy = new RelayCommand(async () => await Delete_Education_TypeStudy_Info());
            //Data to ListView
            Education_TypeStudy_ListView = new ObservableCollection<Education_TypeStudy>();
            //Load Data to ListView
            LoadEducation_TypeStudy();
            //Get STS_ID
            _education_TypeStudyModel = new DatabaseConnection();
            var (ts_id, typestudy_id) = _education_TypeStudyModel.Get_TS_ID_and_TypeStudy_ID();
            TS_ID = ts_id;
            TypeStudy_ID = typestudy_id;

            //Education_StudyYear Mode
            //Submit Command
            SubmitCommand_Add_StudyYear_Information = new RelayCommand(async () => await SubmitAsync_Education_StudyYears());
            //Clear Command
            ClearCommand_Education_StudyYear = new RelayCommand(async () => await ClearAsync());
            //Delete Command
            DeleteCommand_Education_StudyYear = new RelayCommand(async () => await Delete_Education_StudyYear_Info());
            //Data to ListView
            Education_StudyYear_ListView = new ObservableCollection<Education_StudyYear>();
            //Load Data to ListView
            LoadEducation_StudyYear();
            //Get TS_ID
            _education_StudyYearModel = new DatabaseConnection();
            var (sy_id, studyyear_id) = _education_StudyYearModel.Get_SY_ID_and_Edu_StudyYear_ID();
            SY_ID = sy_id;
            Edu_StudyYear_ID = studyyear_id;

            //Add_Province Mode
            //Submit Command
            SubmitCommand_Add_Province_Information = new RelayCommand(async () => await SubmitAsync_Add_Provinces());
            //Clear Command
            ClearCommand_Province = new RelayCommand(async () => await ClearAsync());
            //Delete Command
            DeleteCommand_Province = new RelayCommand(async () => await Delete_Add_Province_Info());
            //Data to ListView
            Province_Info_ListView = new ObservableCollection<Provinces_Info>();
            //Load Data to ListView
            LoadProvince_Info();
            //Get PV_ID
            _province_InfoModel = new DatabaseConnection();
            var (p_id, pv_id) = _province_InfoModel.Get_P_ID_and_PV_ID();
            P_ID = p_id;
            PV_ID = pv_id;

            //Add_District Mode
            //Submit Command
            SubmitCommand_Add_District_Information = new RelayCommand(async () => await SubmitAsync_Add_Districts());
            //Clear Command
            ClearCommand_District = new RelayCommand(async () => await ClearAsync());
            //Delete Command
            DeleteCommand_District = new RelayCommand(async () => await Delete_District_Info());
            //Data to Combobox
            Provinces_Combobox = new ObservableCollection<Districts_Info>();
            //Data to Combobox
            LoadData_to_Combobox();
            District_Info_ListView = new ObservableCollection<Districts_Info>();
            //Load Data to ListView
            LoadDistrict_Info();
            //Get PV_ID_district_InforModel
            _district_InfoModel = new DatabaseConnection();
            var (d_id, ds_id) = _district_InfoModel.Get_D_ID_and_DS_ID();
            D_ID = d_id;
            DS_ID = ds_id;

            //Add Commune Mode
            //Submit Command
            SubmitCommand_Add_Commune_Information = new RelayCommand(async () => await SubmitAsync_Add_Communes());
            //Clear Command
            ClearCommand_Commune = new RelayCommand(async () => await ClearAsync());
            //Delete Command
            DeleteCommand_Commune = new RelayCommand(async () => await Delete_Commune_Info());
            //Data to Combobox
            Provinces_Combobox = new ObservableCollection<Districts_Info>();
            
            //Date to Combobox District
            Districts_Combobox = new ObservableCollection<Communes_Info>();
            //Data to Combobox
            LoadData_to_Combobox();
            //Load Data to ListView
            Commune_Info_ListView = new ObservableCollection<Communes_Info>();
            LoadCommunes_Info();
           
            //Get CM_ID_commune_InforModel
            _commune_InfoModel = new DatabaseConnection();
            var (c_id, cm_id) = _commune_InfoModel.Get_C_ID_and_CM_ID();
            C_ID = c_id;
            CM_ID = cm_id;

            //Add Village Mode
            //Submit Command
            SubmitCommand_Add_Village_Information = new RelayCommand(async () => await SubmitAsync_Add_Villages());
            //Clear Command
            ClearCommand_Village = new RelayCommand(async () => await ClearAsync());
            //Delete Command
            DeleteCommand_Village = new RelayCommand(async () => await Delete_Village_Info());
            //Data to Combobox
            //Provinces_Combobox = new ObservableCollection<Districts_Info>();
            //Data to Combobox
            //LoadData_to_Combobox();
            //Date to Combobox District
            //Districts_Combobox = new ObservableCollection<Communes_Info>();
            Commune_Combobox = new ObservableCollection<Village_Info>();
            //Load Data to ListView
            Village_Info_ListView = new ObservableCollection<Village_Info>();
            LoadVillages_Info();

            //Get VL_ID_Village_InforModel
            _village_InfoModel = new DatabaseConnection();
            var (v_id, vl_id) = _village_InfoModel.Get_V_ID_and_VL_ID();
            V_ID = v_id;
            VL_ID = vl_id;

        }
        //STS_ID
        private int _STS_ID;
        public int STS_ID
        {
            get => _STS_ID;
            set
            {
                if (_STS_ID != value)
                {
                    _STS_ID = value;
                    OnPropertyChanged(nameof(STS_ID));
                }
            }
        }
        //StudyTimeShift_ID
        private string _StudyTimeShift_ID;
        public string StudyTimeShift_ID
        {
            get => _StudyTimeShift_ID;
            set
            {
                if (_StudyTimeShift_ID != value)
                {
                    _StudyTimeShift_ID = value;
                    OnPropertyChanged(nameof(StudyTimeShift_ID));
                    ValidateStudyTimeShift_ID();
                }
            }
        }
        //StudyTimeShift_Name_KH
        private string _StudyTimeShift_Name_KH;
        public string StudyTimeShift_Name_KH
        {
            get => _StudyTimeShift_Name_KH;
            set
            {
                if (_StudyTimeShift_Name_KH != value)
                {
                    _StudyTimeShift_Name_KH = value;
                    OnPropertyChanged(nameof(StudyTimeShift_Name_KH));
                    ValidateStudyTimeShift_Name_KH();
                }
            }
        }
        //StudyTimeShift_Name_EN
        private string _StudyTimeShift_Name_EN;
        public string StudyTimeShift_Name_EN
        {
            get => _StudyTimeShift_Name_EN;
            set
            {
                if (_StudyTimeShift_Name_EN != value)
                {
                    _StudyTimeShift_Name_EN = value;
                    OnPropertyChanged(nameof(StudyTimeShift_Name_EN));
                }
            }
        }
        //StudyTimeShift_Name_Short
        private string _StudyTimeShift_Name_Short;
        public string StudyTimeShift_Name_Short
        {
            get => _StudyTimeShift_Name_Short;
            set
            {
                if (_StudyTimeShift_Name_Short != value)
                {
                    _StudyTimeShift_Name_Short = value;
                    OnPropertyChanged(nameof(StudyTimeShift_Name_Short));
                }
            }
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
                if (_skill_name_kh != value)
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
                if (_edu_id != value)
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
                if (_edu_level_name_en != value)
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
                if (_edu_level_name_short != value)
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
        //Real-time validation method StudyTimeShift_ID
        public SolidColorBrush StudyTimeShift_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(StudyTimeShift_IDBorderBrush));
            }
        }
        //ValidateStudyTimeShift_ID
        private void ValidateStudyTimeShift_ID()
        {
            if (string.IsNullOrEmpty(StudyTimeShift_ID))
            {
                StudyTimeShift_IDBorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                StudyTimeShift_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method StudyTimeShift_Name_KH
        public SolidColorBrush StudyTimeShift_Name_KHBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(StudyTimeShift_Name_KHBorderBrush));
            }
        }
        //ValidateStudyTimeShift_Name_KH
        private void ValidateStudyTimeShift_Name_KH()
        {
            if (string.IsNullOrEmpty(StudyTimeShift_Name_KH))
            {
                StudyTimeShift_Name_KHBorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                StudyTimeShift_Name_KHBorderBrush = new SolidColorBrush(Colors.Green);
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
        //DeleteEducation_StudyTimeShiftfromDatabase
        public void Delete_Education_StudyTimeShifts()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var DeleteEducation_StudyTimeShift = Education_StudyTimeShift_ListView.FirstOrDefault(s => s.StudyTimeShift_ID == StudyTimeShift_ID);
            if (DeleteEducation_StudyTimeShift != null)
            {
                DeleteEducation_StudyTimeShift.StudyTimeShift_ID = StudyTimeShift_ID;

                Debug.WriteLine("Delete Mode");
                bool sucess = dbConnection.Delete_Education_StudyTimeShift_Information(DeleteEducation_StudyTimeShift);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + StudyTimeShift_ID + " ទិន្នន័យលុបបានជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + StudyTimeShift_ID + " ទិន្នន័យលុបបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Edu_Error_Message = "លុបទិន្នន័យបរាជ័យ";
                MessageColor = new SolidColorBrush(Colors.Red);
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
            if (DeleteEducation_Level != null)
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
        //SaveEducation_StudyTimeShifttoDatabase
        public void SaveEducation_StudyTimeShifts()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            //Update Mode
            var UpdateEducation_StudyTimeShift = Education_StudyTimeShift_ListView.FirstOrDefault(s => s.StudyTimeShift_ID == StudyTimeShift_ID);
            //Education_StudyTimeShift_ListView Get from top (Selected ListView).
            if (UpdateEducation_StudyTimeShift != null)
            {
                Debug.WriteLine("Update Mode");
                UpdateEducation_StudyTimeShift.StudyTimeShift_ID = StudyTimeShift_ID;
                UpdateEducation_StudyTimeShift.StudyTimeShift_Name_KH = StudyTimeShift_Name_KH;
                UpdateEducation_StudyTimeShift.StudyTimeShift_Name_EN = StudyTimeShift_Name_EN;
                UpdateEducation_StudyTimeShift.StudyTimeShift_Name_Short = StudyTimeShift_Name_Short;

                bool sucess = dbConnection.Update_Education_StudyTimeShift_Information(UpdateEducation_StudyTimeShift);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + StudyTimeShift_ID + " បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + StudyTimeShift_ID + " បានធ្ចើបច្ចុប្បន្នភាពបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                //Insert Mode
                Education_StudyTimeShift education_studytimeshift_info = new Education_StudyTimeShift
                {
                    StudyTimeShift_ID = this.StudyTimeShift_ID,
                    StudyTimeShift_Name_KH = this.StudyTimeShift_Name_KH,
                    StudyTimeShift_Name_EN = this.StudyTimeShift_Name_EN,
                    StudyTimeShift_Name_Short = this.StudyTimeShift_Name_Short,
                };
                Debug.WriteLine("Insert Mode");
                bool success = dbConnection.Insert_Education_StudyTimeShifts(education_studytimeshift_info);

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

        //SubmitAsync_Education_StudyTimeShifts
        public async Task SubmitAsync_Education_StudyTimeShifts()
        {
            ValidateStudyTimeShift_ID();
            ValidateStudyTimeShift_Name_KH();

            //Validate StudyTimeShift_ID
            if (string.IsNullOrEmpty(StudyTimeShift_ID))
            {
                Edu_Error_Message = "សូមបញ្ចូល លេខសម្គាល់";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            //Validate StudyTimeShift_Name_KH
            if (string.IsNullOrEmpty(StudyTimeShift_Name_KH))
            {
                Edu_Error_Message = "សូមបញ្ចូល វេនសិក្សា";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Debug.WriteLine($"ID: " + StudyTimeShift_ID);
            Debug.WriteLine($"Name: " + StudyTimeShift_Name_KH);

            SaveEducation_StudyTimeShifts();
            LoadEducation_StudyTimeShift();
            Clear_Education_StudyTimeShift();
            await Task.CompletedTask;
        }

        //SubmitAsync_Education_Skills
        public async Task SubmitAsync_Education_Skills()
        {
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
            Debug.WriteLine($"Skill_ID: " + Skill_ID);
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
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_Level_ID + " បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
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
        public async Task Delete_Education_StudyTimeShift_Info()
        {
            Delete_Education_StudyTimeShifts();
            Clear_Education_StudyTimeShift();
            LoadEducation_StudyTimeShift();
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
            Clear_Education_StudyTimeShift();
            Clear_Education_TypeStudy_Text();
            Clear_Education_StudyYear_Text();
            Clear_Province_Info_Text();
            Clear_District_Info_Text();
            Clear_Commune_Info_Text();
            Clear_Village_Info_Text();
            await Task.CompletedTask;
        }
        //Method for Clear Text
        public void Clear_Village_Info_Text()
        { 
            _village_InfoModel = new DatabaseConnection();
            var (v_id, vl_id) = _village_InfoModel.Get_V_ID_and_VL_ID();
            V_ID = v_id;
            VL_ID = vl_id;

            Village_Name_KH = string.Empty;
            Village_Name_EN = string.Empty;
        }
        public void Clear_Commune_Info_Text()
        {
            _commune_InfoModel = new DatabaseConnection();
            var (c_id, cm_id) = _commune_InfoModel.Get_C_ID_and_CM_ID();
            C_ID = c_id;
            CM_ID = cm_id;

            Commune_Name_KH = String.Empty;
            Commune_Name_EN = String.Empty;
        }
        public void Clear_District_Info_Text()
        {
            _district_InfoModel = new DatabaseConnection();
            var (d_id, ds_id) = _district_InfoModel.Get_D_ID_and_DS_ID();
            D_ID = d_id;
            DS_ID = ds_id;

            District_Name_KH = string.Empty;
            District_Name_EN = string.Empty;
        }
        public void Clear_Province_Info_Text()
        {
            //Get PV_ID
            _province_InfoModel = new DatabaseConnection();
            var (p_id, pv_id) = _province_InfoModel.Get_P_ID_and_PV_ID();
            P_ID = p_id;
            PV_ID = pv_id;

            Province_Name_KH = string.Empty;
            Province_Name_EN = string.Empty;
        }
        public void Clear_Education_StudyYear_Text()
        {
            //Get TS_ID
            _education_StudyYearModel = new DatabaseConnection();
            var (sy_id, studyyear_id) = _education_StudyYearModel.Get_SY_ID_and_Edu_StudyYear_ID();
            SY_ID = sy_id;
            Edu_StudyYear_ID = studyyear_id;

            //Edu_StudyYear_ID = string.Empty;
            Edu_StudyYear_Name = string.Empty;
        }
        public void Clear_Education_TypeStudy_Text()
        {
            //Get TS_ID
            _education_TypeStudyModel = new DatabaseConnection();
            var (ts_id, typestudy_id) = _education_TypeStudyModel.Get_TS_ID_and_TypeStudy_ID();
            TS_ID = ts_id;
            TypeStudy_ID = typestudy_id;
            Debug.WriteLine("TS_ID: " + TS_ID);
            Debug.WriteLine("TypeStudy_ID: " + TypeStudy_ID);

            //Skill_ID = string.Empty;
            TypeStudy_Name_KH = string.Empty;
            TypeStudy_Name_EN = string.Empty;
            TypeStudy_Name_Short = string.Empty;
        }
        public void Clear_Education_StudyTimeShift()
        {
            //Get STS_ID
            _education_StudyTimeShiftModel = new DatabaseConnection();
            var (sts_id, studytimeshift_id) = _education_StudyTimeShiftModel.Get_STS_ID_and_StudyTimeShift_ID();
            STS_ID = sts_id;
            StudyTimeShift_ID = studytimeshift_id;

            StudyTimeShift_Name_KH = string.Empty;
            StudyTimeShift_Name_EN = string.Empty;
            StudyTimeShift_Name_Short = string.Empty;
        }
        public void Clear_Education_Skill_Text()
        {
            //Get Sk_ID
            _education_SkillModel = new DatabaseConnection();
            var (sk_id, edu_skill_id) = _education_SkillModel.Get_Sk_ID_and_Skill_ID();
            Sk_ID = sk_id;
            Skill_ID = edu_skill_id;

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

            //Edu_Level_ID = string.Empty;
            Edu_Level_Name_KH = string.Empty;
            Edu_Level_Name_EN = string.Empty;
            Edu_Level_Name_Short = string.Empty;
        }

        //Method to get data to ListView
        //Data to ListView
        public ObservableCollection<Education_StudyTimeShift> Education_StudyTimeShift_ListView
        {
            get => _education_studytimeshift;
            set
            {
                _education_studytimeshift = value;
                OnPropertyChanged(nameof(Education_StudyTimeShift_ListView));  // Notify the UI when the Students collection changes
            }
        }
        public ObservableCollection<Education_Skills> Education_Skill_ListView
        {
            get => _education_skill;
            set
            {
                _education_skill = value;
                OnPropertyChanged(nameof(Education_Skill_ListView));  // Notify the UI when the Students collection changes
            }
        }
        public ObservableCollection<Education_Levels> Education_Level_ListView
        {
            get => _education_level;
            set
            {
                _education_level = value;
                OnPropertyChanged(nameof(Education_Level_ListView));  // Notify the UI when the Students collection changes
            }
        }
        //Load Education_StutyTimeShift
        private void LoadEducation_StudyTimeShift()
        {
            if (_dbConnection == null)
            {
                Debug.WriteLine("_dbConnection is not initialized.");
                return;
            }
            var education_studytimeshift_list = _dbConnection.LoadEducation_StudyTimeShift();
            if (education_studytimeshift_list != null && education_studytimeshift_list.Count > 0)
            {
                // Clear the existing items in the ObservableCollection
                Education_StudyTimeShift_ListView.Clear();

                // Add new items from the database
                foreach (var education_studytimeshift in education_studytimeshift_list)
                {
                    Education_StudyTimeShift_ListView.Add(education_studytimeshift);
                }
                Education_StudyTimeShift_ListView = new ObservableCollection<Education_StudyTimeShift>(education_studytimeshift_list);
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
            if (education_skill_list != null && education_skill_list.Count > 0)
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

        //Select Education StudyTimeShift in to the ListView
        private Education_StudyTimeShift _selectedEducation_StudyTimeShift;
        public Education_StudyTimeShift SelectedEducation_StudyTimeShift
        {
            get => _selectedEducation_StudyTimeShift;
            set
            {
                _selectedEducation_StudyTimeShift = value;
                OnPropertyChanged();
                if (_selectedEducation_StudyTimeShift != null)
                {
                    StudyTimeShift_ID = _selectedEducation_StudyTimeShift.StudyTimeShift_ID;
                    StudyTimeShift_Name_KH = _selectedEducation_StudyTimeShift.StudyTimeShift_Name_KH;
                    StudyTimeShift_Name_EN = _selectedEducation_StudyTimeShift.StudyTimeShift_Name_EN;
                    StudyTimeShift_Name_Short = _selectedEducation_StudyTimeShift.StudyTimeShift_Name_Short;
                }
                OnPropertyChanged(nameof(SelectedEducation_StudyTimeShift));
            }
        }
        //Select Education Skill in to the ListView
        private Education_Skills _selectedEducation_Skill;
        public Education_Skills SelectedEducation_Skill
        {
            get => _selectedEducation_Skill;
            set
            {
                _selectedEducation_Skill = value;
                OnPropertyChanged();
                if (_selectedEducation_Skill != null)
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

                if (_selectedEducation_Level != null)
                {
                    Edu_Level_ID = _selectedEducation_Level.Edu_Level_ID;
                    Edu_Level_Name_KH = _selectedEducation_Level.Edu_Level_Name_KH;
                    Edu_Level_Name_EN = _selectedEducation_Level.Edu_Level_Name_EN;
                    Edu_Level_Name_Short = _selectedEducation_Level.Edu_Level_Name_Short;
                }
                OnPropertyChanged(nameof(SelectedEducation_Level));
            }
        }

        //Start Education_AddTypeStudy
        //ST_ID
        private int _ts_id;
        public int TS_ID
        {
            get => _ts_id;
            set
            {
                if (_ts_id != value)
                {
                    _ts_id = value;
                    OnPropertyChanged(nameof(TS_ID));
                }
            }
        }
        //TypeStudy_ID
        private string _TypeStudy_ID;
        public string TypeStudy_ID
        {
            get => _TypeStudy_ID;
            set
            {
                if (_TypeStudy_ID != value)
                {
                    _TypeStudy_ID = value;
                    OnPropertyChanged(nameof(TypeStudy_ID));
                    ValidateTypeStudy_ID();
                }
            }
        }
        //TypeStudy_Name_KH
        private string _TypeStudy_Name_KH;
        public string TypeStudy_Name_KH
        {
            get => _TypeStudy_Name_KH;
            set
            {
                if (_TypeStudy_Name_KH != value)
                {
                    _TypeStudy_Name_KH = value;
                    OnPropertyChanged(nameof(TypeStudy_Name_KH));
                    ValidateTypeStudy_Name_KH();
                }
            }
        }
        //TypeStudy_Name_EN
        private string _TypeStudy_Name_EN;
        public string TypeStudy_Name_EN
        {
            get => _TypeStudy_Name_EN;
            set
            {
                if (_TypeStudy_Name_EN != value)
                {
                    _TypeStudy_Name_EN = value;
                    OnPropertyChanged(nameof(TypeStudy_Name_EN));
                }
            }
        }
        //TypeStudy_Name_Short
        private string _TypeStudy_Name_Short;
        public string TypeStudy_Name_Short
        {
            get => _TypeStudy_Name_Short;
            set
            {
                _TypeStudy_Name_Short = value;
                OnPropertyChanged(nameof(TypeStudy_Name_Short));
            }
        }
        //Real-time validation method TypeStudy_ID
        public SolidColorBrush TypeStudy_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(TypeStudy_IDBorderBrush));
            }
        }
        //ValidateEdu_Level_Name_KH
        private void ValidateTypeStudy_ID()
        {
            if (string.IsNullOrEmpty(TypeStudy_ID))
            {
                TypeStudy_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                TypeStudy_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method TypeStudy_Name_KH
        public SolidColorBrush TypeStudy_Name_KHBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(TypeStudy_Name_KHBorderBrush));
            }
        }
        //ValidateTypeStudy_Name_KH
        private void ValidateTypeStudy_Name_KH()
        {
            if (string.IsNullOrEmpty(TypeStudy_Name_KH))
            {
                TypeStudy_Name_KHBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                TypeStudy_Name_KHBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //SubmitAsync_Education_TypeStudys
        public async Task SubmitAsync_Education_TypeStudys()
        {
            ValidateTypeStudy_ID();
            ValidateTypeStudy_Name_KH();
            // Clear any previous error message
            Edu_Error_Message = string.Empty;
            MessageColor = null;

            //Validate Edu_Level_ID
            if (string.IsNullOrEmpty(TypeStudy_ID))
            {
                Edu_Error_Message = "សូមបញ្ចូល លេខសម្គាល់";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            //Validate Edu_Level_Name_KH
            if (string.IsNullOrEmpty(TypeStudy_Name_KH))
            {
                Edu_Error_Message = "សូមបញ្ចូល ប្រភេទសិក្សា";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            SaveEducation_TypeStudys();
            LoadEducation_TypeStudy();
            Clear_Education_TypeStudy_Text();
            await Task.CompletedTask;
        }
        //SaveEducation_TypeStudytoDatabase
        public void SaveEducation_TypeStudys()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            //Update Mode
            var UpdateEducation_TypeStudy = Education_TypeStudy_ListView.FirstOrDefault(s => s.TypeStudy_ID == TypeStudy_ID);
            //Education_TypeStudy_ListView Get from top (Selected ListView).
            if (UpdateEducation_TypeStudy != null)
            {
                Debug.WriteLine("Update Mode");
                UpdateEducation_TypeStudy.TypeStudy_ID = TypeStudy_ID;
                UpdateEducation_TypeStudy.TypeStudy_Name_KH = TypeStudy_Name_KH;
                UpdateEducation_TypeStudy.TypeStudy_Name_EN = TypeStudy_Name_EN;
                UpdateEducation_TypeStudy.TypeStudy_Name_Short = TypeStudy_Name_Short;

                bool sucess = dbConnection.Update_Education_TypeStudy_Information(UpdateEducation_TypeStudy);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + TypeStudy_ID + " បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + TypeStudy_ID + " បានធ្ចើបច្ចុប្បន្នភាពបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                //Insert Mode
                Education_TypeStudy education_typestudy_info = new Education_TypeStudy
                {
                    TypeStudy_ID = this.TypeStudy_ID,
                    TypeStudy_Name_KH = this.TypeStudy_Name_KH,
                    TypeStudy_Name_EN = this.TypeStudy_Name_EN,
                    TypeStudy_Name_Short = this.TypeStudy_Name_Short,
                };
                Debug.WriteLine("Insert Mode");
                bool success = dbConnection.Insert_Education_TypeStudys(education_typestudy_info);

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
        //Data to ListView
        public ObservableCollection<Education_TypeStudy> Education_TypeStudy_ListView
        {
            get => _education_typestudy;
            set
            {
                _education_typestudy = value;
                OnPropertyChanged(nameof(Education_TypeStudy_ListView));  // Notify the UI when the Students collection changes
            }
        }
        //LoadEducation_TypeStudy
        public void LoadEducation_TypeStudy()
        {
            // Ensure _dbConnection is properly initialized
            if (_dbConnection == null)
            {
                Debug.WriteLine("_dbConnection is not initialized.");
                return;
            }

            var education_typestudy_list = _dbConnection.LoadEducation_TypeStudy();

            if (education_typestudy_list != null && education_typestudy_list.Count > 0)
            {
                // Clear the existing items in the ObservableCollection
                Education_TypeStudy_ListView.Clear();

                // Add new items from the database
                foreach (var education_typestudy in education_typestudy_list)
                {
                    Education_TypeStudy_ListView.Add(education_typestudy);
                }
                Education_TypeStudy_ListView = new ObservableCollection<Education_TypeStudy>(education_typestudy_list);
            }
            else
            {
                Debug.WriteLine("No education typestudy found.");
            }
        }
        //Select Education_TypeStudy in the ListView
        private Education_TypeStudy _selectedEducation_TypeStudy;
        public Education_TypeStudy SelectedEducation_TypeStudy
        {
            get => _selectedEducation_TypeStudy;
            set
            {
                _selectedEducation_TypeStudy = value;
                OnPropertyChanged();

                if (_selectedEducation_TypeStudy != null)
                {
                    TypeStudy_ID = _selectedEducation_TypeStudy.TypeStudy_ID;
                    TypeStudy_Name_KH = _selectedEducation_TypeStudy.TypeStudy_Name_KH;
                    TypeStudy_Name_EN = _selectedEducation_TypeStudy.TypeStudy_Name_EN;
                    TypeStudy_Name_Short = _selectedEducation_TypeStudy.TypeStudy_Name_Short;
                }
                OnPropertyChanged(nameof(SelectedEducation_TypeStudy));
            }
        }
        //DeleteEducation_TypeStudyfromDatabase
        public void Delete_Education_TypeStudys()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var DeleteEducation_TypeStudy = Education_TypeStudy_ListView.FirstOrDefault(s => s.TypeStudy_ID == TypeStudy_ID);
            if (DeleteEducation_TypeStudy != null)
            {
                DeleteEducation_TypeStudy.TypeStudy_ID = TypeStudy_ID;

                Debug.WriteLine("Delete Mode");
                bool sucess = dbConnection.Delete_Education_TypeStudy_Information(DeleteEducation_TypeStudy);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + TypeStudy_ID + " ទិន្នន័យលុបបានជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + TypeStudy_ID + " ទិន្នន័យលុបបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Edu_Error_Message = "លុបទិន្នន័យបរាជ័យ";
                MessageColor = new SolidColorBrush(Colors.Red);
            }
        }
        //For command Delete
        public async Task Delete_Education_TypeStudy_Info()
        {
            Delete_Education_TypeStudys();
            LoadEducation_TypeStudy();
            Clear_Education_TypeStudy_Text();

            await Task.CompletedTask;
        }
        //End Education_TypeStudy Mode

        //Start StudyYear Mode
        //SY_ID
        private int _SY_ID;
        public int SY_ID
        {
            get => _SY_ID;
            set
            {
                if (_SY_ID != value)
                {
                    _SY_ID = value;
                    OnPropertyChanged(nameof(SY_ID));
                }
            }
        }
        //Edu_StudyYear_ID
        private string _Edu_StudyYear_ID;
        public string Edu_StudyYear_ID
        {
            get => _Edu_StudyYear_ID;
            set
            {
                if (_Edu_StudyYear_ID != value)
                {
                    _Edu_StudyYear_ID = value;
                    OnPropertyChanged(nameof(Edu_StudyYear_ID));
                    ValidateEdu_StudyYear_ID();
                }
            }
        }
        //Edu_StudyYear_Name
        private string _Edu_StudyYear_Name;
        public string Edu_StudyYear_Name
        {
            get => _Edu_StudyYear_Name;
            set
            {
                if (_Edu_StudyYear_Name != value)
                {
                    _Edu_StudyYear_Name = value;
                    OnPropertyChanged(nameof(Edu_StudyYear_Name));
                    ValidateEdu_StudyYear_Name();
                }
            }
        }
        //Real-time validation method Edu_StudyYear_ID
        public SolidColorBrush Edu_StudyYear_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Edu_StudyYear_IDBorderBrush));
            }
        }
        //ValidateEdu_StudyYear_ID
        private void ValidateEdu_StudyYear_ID()
        {
            if (string.IsNullOrEmpty(Edu_StudyYear_ID))
            {
                Edu_StudyYear_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Edu_StudyYear_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method Edu_StudyYear_Name
        public SolidColorBrush Edu_StudyYear_NameBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Edu_StudyYear_NameBorderBrush));
            }
        }
        //ValidateEdu_StudyYear_Name
        private void ValidateEdu_StudyYear_Name()
        {
            if (string.IsNullOrEmpty(Edu_StudyYear_Name))
            {
                Edu_StudyYear_NameBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Edu_StudyYear_NameBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        public async Task SubmitAsync_Education_StudyYears()
        {
            ValidateEdu_StudyYear_ID();
            ValidateEdu_StudyYear_Name();

            if (string.IsNullOrEmpty(Edu_StudyYear_ID))
            {
                Edu_Error_Message = "សូមបញ្ចូល លេខសម្គាល់";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(Edu_StudyYear_Name))
            {
                Edu_Error_Message = "សូមបញ្ចូល ឆ្នាំសិក្សា";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            SaveEducation_StudyYears();
            LoadEducation_StudyYear();
            Clear_Education_StudyYear_Text();

            Debug.WriteLine(Edu_StudyYear_ID);
            Debug.WriteLine(Edu_StudyYear_Name);
            await Task.CompletedTask;
        }
        //SaveEducation_StudyYeartoDatabase
        public void SaveEducation_StudyYears()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            //Update Mode
            var UpdateEducation_StudyYear = Education_StudyYear_ListView.FirstOrDefault(s => s.Edu_StudyYear_ID == Edu_StudyYear_ID);
            //Education_TypeStudy_ListView Get from top (Selected ListView).
            if (UpdateEducation_StudyYear != null)
            {
                Debug.WriteLine("Update Mode");
                UpdateEducation_StudyYear.Edu_StudyYear_ID = Edu_StudyYear_ID;
                UpdateEducation_StudyYear.Edu_StudyYear_Name = Edu_StudyYear_Name;


                bool sucess = dbConnection.Update_Education_StudyYear_Information(UpdateEducation_StudyYear);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_StudyYear_ID + " បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_StudyYear_ID + " បានធ្ចើបច្ចុប្បន្នភាពបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                //Insert Mode
                Education_StudyYear education_studyyear_info = new Education_StudyYear
                {
                    Edu_StudyYear_ID = this.Edu_StudyYear_ID,
                    Edu_StudyYear_Name = this.Edu_StudyYear_Name,
                };
                Debug.WriteLine("Insert Mode");
                bool success = dbConnection.Insert_Education_StudyYears(education_studyyear_info);

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
        //Data to ListView
        public ObservableCollection<Education_StudyYear> Education_StudyYear_ListView
        {
            get => _education_studyyear;
            set
            {
                _education_studyyear = value;
                OnPropertyChanged(nameof(Education_StudyYear_ListView));  // Notify the UI when the Students collection changes
            }
        }
        //LoadEducation_StudyYear
        public void LoadEducation_StudyYear()
        {
            // Ensure _dbConnection is properly initialized
            if (_dbConnection == null)
            {
                Debug.WriteLine("_dbConnection is not initialized.");
                return;
            }

            var education_studyyear_list = _dbConnection.LoadEducation_StudyYear();

            if (education_studyyear_list != null && education_studyyear_list.Count > 0)
            {
                // Clear the existing items in the ObservableCollection
                Education_StudyYear_ListView.Clear();

                // Add new items from the database
                foreach (var education_studyyear in education_studyyear_list)
                {
                    Education_StudyYear_ListView.Add(education_studyyear);
                }
                Education_StudyYear_ListView = new ObservableCollection<Education_StudyYear>(education_studyyear_list);
            }
            else
            {
                Debug.WriteLine("No education StudyYear found.");
            }
        }
        //Select Education_StudyYear in the ListView
        private Education_StudyYear _selectedEducation_StudyYear;
        public Education_StudyYear SelectedEducation_StudyYear
        {
            get => _selectedEducation_StudyYear;
            set
            {
                _selectedEducation_StudyYear = value;
                OnPropertyChanged();

                if (_selectedEducation_StudyYear != null)
                {
                    Edu_StudyYear_ID = _selectedEducation_StudyYear.Edu_StudyYear_ID;
                    Edu_StudyYear_Name = _selectedEducation_StudyYear.Edu_StudyYear_Name;
                }
                OnPropertyChanged(nameof(SelectedEducation_StudyYear));
            }
        }
        //For Command Delete Education_StudyYear
        public async Task Delete_Education_StudyYear_Info()
        {
            Delete_Education_StudyYears();
            LoadEducation_StudyYear();
            Clear_Education_StudyYear_Text();

            await Task.CompletedTask;
        }
        //Method Delete
        public void Delete_Education_StudyYears()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var DeleteEducation_StudyYear = Education_StudyYear_ListView.FirstOrDefault(s => s.Edu_StudyYear_ID == Edu_StudyYear_ID);
            if (DeleteEducation_StudyYear != null)
            {
                DeleteEducation_StudyYear.Edu_StudyYear_ID = Edu_StudyYear_ID;

                Debug.WriteLine("Delete Mode");
                bool sucess = dbConnection.Delete_Education_StudyYear_Information(DeleteEducation_StudyYear);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_StudyYear_ID + " ទិន្នន័យលុបបានជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + Edu_StudyYear_ID + " ទិន្នន័យលុបបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Edu_Error_Message = "លុបទិន្នន័យបរាជ័យ";
                MessageColor = new SolidColorBrush(Colors.Red);
            }
        }
        //End Education_StudyYear

        //Start Add_Province
        //P_ID
        private int _P_ID;
        public int P_ID
        {
            get => _P_ID;
            set
            {
                if (_P_ID != value)
                {
                    _P_ID = value;
                    OnPropertyChanged(nameof(P_ID));
                }
            }
        }
        //PV_ID
        private string _PV_ID;
        public string PV_ID
        {
            get => _PV_ID;
            set
            {
                if (_PV_ID != value)
                {
                    _PV_ID = value;
                    OnPropertyChanged(nameof(PV_ID));
                    ValidatePV_ID();
                }
            }
        }
        //Province_Name_KH
        private string _Province_Name_KH;
        public string Province_Name_KH
        {
            get => _Province_Name_KH;
            set
            {
                if (_Province_Name_KH != value)
                {
                    _Province_Name_KH = value;
                    OnPropertyChanged(nameof(Province_Name_KH));
                    ValidateProvince_Name_KH();
                }
            }
        }
        //Province_Name_EN
        private string _Province_Name_EN;
        public string Province_Name_EN
        {
            get => _Province_Name_EN;
            set
            {
                _Province_Name_EN = value;
                OnPropertyChanged(nameof(Province_Name_EN));
            }
        }
        //Real-time validation method PV_ID
        public SolidColorBrush PV_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(PV_IDBorderBrush));
            }
        }
        //ValidatePV_ID
        private void ValidatePV_ID()
        {
            if (string.IsNullOrEmpty(PV_ID))
            {
                PV_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                PV_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method Province_Name_KH
        public SolidColorBrush Province_Name_KHBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Province_Name_KHBorderBrush));
            }
        }
        //ValidateProvince_Name_KH
        private void ValidateProvince_Name_KH()
        {
            if (string.IsNullOrEmpty(Province_Name_KH))
            {
                Province_Name_KHBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Province_Name_KHBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Command for add province
        public async Task SubmitAsync_Add_Provinces()
        {
            ValidatePV_ID();
            ValidateProvince_Name_KH();

            if (string.IsNullOrEmpty(PV_ID))
            {
                Edu_Error_Message = "សូមបញ្ចូល លេខសម្គាល់";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(Province_Name_KH))
            {
                Edu_Error_Message = "សូមបញ្ចូល ខេត្ត";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Save_Province();
            LoadProvince_Info();
            Clear_Province_Info_Text();


            await Task.CompletedTask;
        }
        //Save Province
        public void Save_Province()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var UpdateProvince = Province_Info_ListView.FirstOrDefault(s => s.PV_ID == PV_ID);
            //Province ListView Get from top (Selected ListView).
            if (UpdateProvince != null)
            {
                Debug.WriteLine("Update Mode");
                UpdateProvince.PV_ID = PV_ID;
                UpdateProvince.Province_Name_KH = Province_Name_KH;
                UpdateProvince.Province_Name_EN = Province_Name_EN;

                bool sucess = dbConnection.Update_Provinces_Information(UpdateProvince);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + PV_ID + " បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + PV_ID + " បានធ្ចើបច្ចុប្បន្នភាពបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Provinces_Info provinces_Info = new Provinces_Info()
                {
                    PV_ID = this.PV_ID,
                    Province_Name_KH = this.Province_Name_KH,
                    Province_Name_EN = this.Province_Name_EN,
                };
                Debug.WriteLine("Insert Mode");
                bool success = dbConnection.Insert_Provinces(provinces_Info);

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
        //Data to ListView
        public ObservableCollection<Provinces_Info> Province_Info_ListView
        {
            get => _provinces_info;
            set
            {
                _provinces_info = value;
                OnPropertyChanged(nameof(Province_Info_ListView));  // Notify the UI when the Students collection changes
            }
        }
        //LoadProvince_Info
        public void LoadProvince_Info()
        {
            // Ensure _dbConnection is properly initialized
            if (_dbConnection == null)
            {
                Debug.WriteLine("_dbConnection is not initialized.");
                return;
            }

            var province_list = _dbConnection.LoadProvinces_Info();

            if (province_list != null && province_list.Count > 0)
            {
                // Clear the existing items in the ObservableCollection
                Province_Info_ListView.Clear();

                // Add new items from the database
                foreach (var province_info in province_list)
                {
                    Province_Info_ListView.Add(province_info);
                }
                Province_Info_ListView = new ObservableCollection<Provinces_Info>(province_list);

            }
            else
            {
                Debug.WriteLine("No Province Info found.");
            }
        }
        //Select Province in the ListView
        private Provinces_Info _selectedProvinces;
        public Provinces_Info SelectedProvinces
        {
            get => _selectedProvinces;
            set
            {
                _selectedProvinces = value;
                OnPropertyChanged();

                if (_selectedProvinces != null)
                {
                    PV_ID = _selectedProvinces.PV_ID;
                    Province_Name_KH = _selectedProvinces.Province_Name_KH;
                    Province_Name_EN = _selectedProvinces.Province_Name_EN;
                }
                OnPropertyChanged(nameof(SelectedProvinces));
            }
        }
        //For Command Delete
        public async Task Delete_Add_Province_Info()
        {
            Delete_Provinces_Info();
            Clear_Province_Info_Text();
            LoadProvince_Info();

            await Task.CompletedTask;
        }
        //For delete province
        public void Delete_Provinces_Info()
        { 
            DatabaseConnection dbConnection = new DatabaseConnection();
                var DeleteProvince = Province_Info_ListView.FirstOrDefault(s => s.PV_ID == PV_ID);
                if (DeleteProvince != null)
                {
                    DeleteProvince.PV_ID = PV_ID;

                    Debug.WriteLine("Delete Mode");
                    bool sucess = dbConnection.Delete_Province_Information(DeleteProvince);
                    if (sucess)
                    {
                        Edu_Error_Message = "លេខសម្កាល់ " + PV_ID + " ទិន្នន័យលុបបានជោគជ័យ";
                        MessageColor = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        Edu_Error_Message = "លេខសម្កាល់ " + PV_ID + " ទិន្នន័យលុបបរាជ័យ";
                        MessageColor = new SolidColorBrush(Colors.Red);
                    }
                }
                else
                {
                    Edu_Error_Message = "លុបទិន្នន័យបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
        }
        //End Province

        //Start District
        //D_ID
        private int _D_ID;
        public int D_ID
        {
            get => _D_ID;
            set
            {
                if (_D_ID != value)
                {
                    _D_ID = value;
                    OnPropertyChanged(nameof(D_ID));                  
                }
            }
        }
        //DS_ID
        private string _DS_ID;
        public string DS_ID
        {
            get => _DS_ID;
            set
            {
                if (_DS_ID != value)
                {
                    _DS_ID = value;
                    OnPropertyChanged(nameof(DS_ID));
                    ValidateDS_ID();
                }
            }
        }
        //District_Name_KH
        private string _District_Name_KH;
        public string District_Name_KH
        {
            get => _District_Name_KH;
            set
            {
                if(_District_Name_KH  != value)
                {
                    _District_Name_KH = value;
                    OnPropertyChanged(nameof(District_Name_KH));
                    ValidateDistrict_Name_KH();
                }
            }
        }
        //District_Name_KH
        private string _District_Name_EN;
        public string District_Name_EN
        {
            get => _District_Name_EN;
            set
            {
                if (_District_Name_EN != value)
                {
                    _District_Name_EN = value;
                    OnPropertyChanged(nameof(District_Name_EN));
                }
            }
        }
        //Province_ID
        private int _Province_ID;
        public int Province_ID
        {
            get => _Province_ID;
            set
            {
                if(_Province_ID != value)
                {
                    _Province_ID = value;
                    OnPropertyChanged(nameof(Province_ID));
                    ValidateSelectProvince();
                }
            }
        }

        //SelectedProvince
        private Districts_Info _selectedProvince_Info;
        public Districts_Info SelectedProvince_Info
        {
            get { return _selectedProvince_Info; }
            set
            {
                _selectedProvince_Info = value;
                OnPropertyChanged(nameof(SelectedProvince_Info));  // Notify UI that the property has changed
                
            }
        }
        //Real-time validation method DS_ID
        public SolidColorBrush DS_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(DS_IDBorderBrush));
            }
        }
        //ValidateDS_ID
        private void ValidateDS_ID()
        {
            if (string.IsNullOrEmpty(DS_ID))
            {
                DS_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                DS_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method District_Name_KH
        public SolidColorBrush District_Name_KHBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(District_Name_KHBorderBrush));
            }
        }
        //ValidateDistrict_Name_KH
        private void ValidateDistrict_Name_KH()
        {
            if (string.IsNullOrEmpty(District_Name_KH))
            {
                District_Name_KHBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                District_Name_KHBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method Select Province
        public SolidColorBrush SelectProBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectProBorderBrush));
            }
        }
        //ValidateSelectProvince
        private void ValidateSelectProvince()
        {
            if (SelectedProvince_Info == null)
            {
                SelectProBorderBrush = new SolidColorBrush(Colors.Red);
                Edu_Error_Message = "សូមជ្រើសរើសខេត្ត"; // Set error message for UI feedback
            }
            else
            {
                SelectProBorderBrush = new SolidColorBrush(Colors.Green);
                Edu_Error_Message = ""; // Clear error message
            }
        }
        public void SelectedProvince_In_District()
        {
            if (SelectedProvince_Info == null)
            {
                Edu_Error_Message = "សូមជ្រើសរើសខេត្ត";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            string selectedProvinceName = SelectedProvince_Info?.District_In_Pro;
            int selectedProvinceId = SelectedProvince_Info?.Province_ID ?? -1; // Default value if null

            // Do something with the selected district or province
            Debug.WriteLine($"Selected Province: {selectedProvinceName}, ID: {selectedProvinceId}");
        }
        //Real-time validation method District_In_Pro
        public SolidColorBrush District_In_ProBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(District_In_ProBorderBrush));
            }
        }
        //Get_data_to_combobox Province
        private ObservableCollection<Districts_Info> _provinces;
        public ObservableCollection<Districts_Info> Provinces_Combobox
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                OnPropertyChanged(nameof(Provinces_Combobox));
            }
        }
        //Load Data to Combobox
        private void LoadData_to_Combobox()
        {
            var ProvinceList = _dbConnection.GetProvince_toCombobox();
            foreach (var province in ProvinceList)
            {
                Provinces_Combobox.Add(province);
            }
        }
        //Save Data District to database
        public void SaveData_District_toDatabase()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var UpdateDistrict = District_Info_ListView.FirstOrDefault(s => s.DS_ID == DS_ID);
            //Province ListView Get from top (Selected ListView).
            if (UpdateDistrict != null)
            {
                Debug.WriteLine("Update Mode");
                UpdateDistrict.DS_ID = DS_ID;
                UpdateDistrict.District_Name_KH = District_Name_KH;
                UpdateDistrict.District_Name_EN = District_Name_EN;
                // Assuming a ComboBox that selects a province and binds to SelectedProvince_Info
                if (SelectedProvince_Info != null)
                {
                    UpdateDistrict.Province_ID = SelectedProvince_Info.Province_ID;  // Ensure valid Province_ID
                }
                else
                {
                    Edu_Error_Message = "សូមជ្រើសរើសខេត្តដែលស្រុកស្ថិតនៅ!";
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }


                bool sucess = dbConnection.Update_Districts_Information(UpdateDistrict);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + DS_ID + " បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + DS_ID + " បានធ្ចើបច្ចុប្បន្នភាពបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Districts_Info districts_Info = new Districts_Info()
                {
                    DS_ID = this.DS_ID,
                    District_Name_KH = this.District_Name_KH,
                    District_Name_EN = this.District_Name_EN,
                    SelectedProvince = SelectedProvince_Info.Province_ID,
                };
                Debug.WriteLine("Insert Mode");
                bool success = dbConnection.Insert_Districts(districts_Info);

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
        //Data to ListView
        public ObservableCollection<Districts_Info> District_Info_ListView
        {
            get => _district_info;
            set
            {
                _district_info = value;
                OnPropertyChanged(nameof(District_Info_ListView));  // Notify the UI when the Students collection changes
            }
        }
        //LoadDistrict_Info
        public void LoadDistrict_Info()
        {
            // Ensure _dbConnection is properly initialized
            if (_dbConnection == null)
            {
                Debug.WriteLine("_dbConnection is not initialized.");
                return;
            }

            var district_list = _dbConnection.LoadDistricts_Info();

            if (district_list != null && district_list.Count > 0)
            {
                // Clear the existing items in the ObservableCollection
                District_Info_ListView.Clear();

                // Add new items from the database
                foreach (var district_info in district_list)
                {
                    District_Info_ListView.Add(district_info);
                }
                District_Info_ListView = new ObservableCollection<Districts_Info>(district_list);

            }
            else
            {
                Debug.WriteLine("No District Info are found.");
            }
        }
        //Select Data from ListView

        //Command Submit
        public async Task SubmitAsync_Add_Districts()
        {
            ValidateDS_ID();
            ValidateDistrict_Name_KH();
            ValidateSelectProvince();
            //SelectedProvince_In_District();
            if (string.IsNullOrEmpty(DS_ID))
            {
                Edu_Error_Message = "សូមបញ្ចូល លេខសម្គាល់";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(District_Name_KH))
            {
                Edu_Error_Message = "សូមបញ្ចូល ស្រុក";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedProvince_Info == null)
            {
                Edu_Error_Message = "សូមជ្រើសរើសខេត្ត"; 
                MessageColor = new SolidColorBrush(Colors.Red); 
                Debug.WriteLine("Submit failed: SelectedProvince_Info is null.");
                return;
            }
            SelectedProvince_In_District();
            SaveData_District_toDatabase();
            LoadDistrict_Info();
            Clear_District_Info_Text(); 
            Debug.WriteLine(DS_ID);
            Debug.WriteLine(District_Name_KH);
            

            await Task.CompletedTask;
        }
              
        //Select Data District from ListView to Combobox
        private Districts_Info _selectedDistricts;
        public Districts_Info SelectedDistricts
        {
            get => _selectedDistricts;
            set
            {
                _selectedDistricts = value;
                OnPropertyChanged();

                if (_selectedDistricts != null)
                {
                    DS_ID = _selectedDistricts.DS_ID;
                    District_Name_KH = _selectedDistricts.District_Name_KH;
                    District_Name_EN = _selectedDistricts.District_Name_EN;
                    // Find the matching province based on the selected district's province_id
                    SelectedProvince_Info = Provinces_Combobox
                        .FirstOrDefault(province => province.District_In_Pro == _selectedDistricts.District_In_Pro);
                    OnPropertyChanged(nameof(SelectedProvince_Info));
                }
                OnPropertyChanged(nameof(SelectedDistricts));
            }
        }
        //Delete District Info
        public void Delete_Districts_Info()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var DeleteDistrict = District_Info_ListView.FirstOrDefault(s => s.DS_ID == DS_ID);
            if (DeleteDistrict != null)
            {
                DeleteDistrict.DS_ID = DS_ID;

                Debug.WriteLine("Delete Mode");
                bool sucess = dbConnection.Delete_District_Information(DeleteDistrict);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + DS_ID + " ទិន្នន័យលុបបានជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + DS_ID + " ទិន្នន័យលុបបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Edu_Error_Message = "លុបទិន្នន័យបរាជ័យ";
                MessageColor = new SolidColorBrush(Colors.Red);
            }
        }
        //Command Delete
        public async Task Delete_District_Info()
        {
            Delete_Districts_Info();
            LoadDistrict_Info();
            Clear_District_Info_Text();

            await Task.CompletedTask;

        }
        //End District Info

        //Start Add Commune
        //C_ID
        private int _C_ID;
        public int C_ID
        {
            get => _C_ID;
            set
            {
                if (_C_ID != value)
                {
                    _C_ID = value;
                    OnPropertyChanged(nameof(C_ID));
                }
            }
        }
        //CM_ID
        private string _CM_ID;
        public string CM_ID
        {
            get => _CM_ID;
            set
            {
                if(_CM_ID != value)
                {
                    _CM_ID = value;
                    OnPropertyChanged(nameof(CM_ID));
                    ValidateCM_ID();
                }
            }
        }
        //Commune_Name_KH
        private string _Commune_Name_KH;
        public string Commune_Name_KH
        {
            get => _Commune_Name_KH;
            set
            {
                if(_Commune_Name_KH != value)
                {
                    _Commune_Name_KH = value;
                    OnPropertyChanged(nameof(Commune_Name_KH));
                    ValidateCommune_Name_KH();
                }
            }
        }
        //Commune_Name_EN
        private string _Commune_Name_EN;
        public string Commune_Name_EN
        {
            get => _Commune_Name_EN;
            set
            {
                if(_Commune_Name_EN != value)
                {
                    _Commune_Name_EN = value;
                    OnPropertyChanged(nameof(Commune_Name_EN));
                }
            }
        }
        //District_ID
        private int _District_ID;
        public int District_ID
        {
            get => _District_ID;
            set
            {
                if(_District_ID  != value)
                {
                    District_ID = value;
                    OnPropertyChanged(nameof(District_ID));
                }
            }
        }
        //Commune_In_Dis
        private string _Commune_In_Dis;
        public string Commune_In_Dis
        {
            get => _Commune_In_Dis;
            set
            {
                if(_Commune_In_Dis != value)
                {
                    _Commune_In_Dis = value;
                    OnPropertyChanged(nameof(Commune_In_Dis));
                }
            }
        }
        //Commune_In_Pro
        private string _Commune_In_Pro;
        public string Commune_In_Pro
        {
            get => _Commune_In_Pro;
            set
            {
                if(_Commune_In_Pro != value)
                {
                    _Commune_In_Pro = value;
                    OnPropertyChanged(nameof(Commune_In_Pro));
                    ValidateSelectProvince_InComm();
                }
            }
        }
        //SelectedProvince
        private Districts_Info _selectedProvince_Incomm;
        public Districts_Info SelectedProvince_Incomm
        {
            get { return _selectedProvince_Incomm; }
            set
            {
                _selectedProvince_Incomm = value;
                OnPropertyChanged(nameof(SelectedProvince_Incomm));  // Notify UI that the property has changed
                ValidateSelectProvince_InComm();
                if (_selectedProvince_Incomm != null)
                {
                    Debug.WriteLine($"Selected Province ID: {_selectedProvince_Incomm.Province_ID}");
                    LoadData_to_Combobox_District(_selectedProvince_Incomm.Province_ID);
                }

            }
        }
        //SelectedCommunce
        private Communes_Info _selectedDistrict_Incomm;
        public Communes_Info SelectedDistrict_Incomm
        {
            get { return _selectedDistrict_Incomm; }
            set
            {
                _selectedDistrict_Incomm = value;
                OnPropertyChanged(nameof(SelectedDistrict_Incomm));  // Notify UI that the property has changed
                ValidateSelectDistrict_InComm();
                if (_selectedDistrict_Incomm != null)
                {
                    Debug.WriteLine($"Selected District ID: {_selectedDistrict_Incomm.District_ID}");
                    LoadData_to_Combobox_Commune(_selectedDistrict_Incomm.District_ID);
                }
            }
        }      
        //Real-time validation method CM_ID
        public SolidColorBrush CM_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(CM_IDBorderBrush));
            }
        }
        //ValidateCM_ID
        private void ValidateCM_ID()
        {
            if (string.IsNullOrEmpty(CM_ID))
            {
                CM_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                CM_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method Commune_Name_KH
        public SolidColorBrush Commune_Name_KHBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Commune_Name_KHBorderBrush));
            }
        }
        //ValidateCommune_Name_KH
        private void ValidateCommune_Name_KH()
        {
            if (string.IsNullOrEmpty(Commune_Name_KH))
            {
                Commune_Name_KHBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Commune_Name_KHBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }

        //Real-time validation method SelectedProvince_Incomm
        public SolidColorBrush SelectPro_IncommBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectPro_IncommBorderBrush));
            }
        }
        
        //ValidateSelectProvince
        private void ValidateSelectProvince_InComm()
        {
            if (SelectedProvince_Incomm == null)
            {
                SelectPro_IncommBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectPro_IncommBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Select Province 
        public void SelectedProvince_In_Commune()
        {
            if (SelectedProvince_Incomm != null)
            {
                string selectedProvinceName = SelectedProvince_Incomm.District_In_Pro;
                int selectedProvinceId = SelectedProvince_Incomm.Province_ID;

                // Do something with the selected district or province
                Debug.WriteLine($"Selected Province {selectedProvinceName} ,ID: {selectedProvinceId}");
            }
            else
            {
                Edu_Error_Message = "សូមជ្រើសរើសខេត្ត";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
        }

        //Border Combobox District
        public SolidColorBrush SelectDis_IncommBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectDis_IncommBorderBrush));
            }
        }
        //ValidateSelectDistrict
        private void ValidateSelectDistrict_InComm()
        {
            if (SelectedDistrict_Incomm == null)
            {
                SelectDis_IncommBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectDis_IncommBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Select District
        public void SelectedDistrict_In_Commune()
        {
            if (SelectedDistrict_Incomm != null)
            {
                string selectedDistrictName = SelectedDistrict_Incomm.Commune_In_Dis;
                int selectedDistrictID = SelectedDistrict_Incomm.District_ID;

                // Do something with the selected district or province
                Debug.WriteLine($"Selected District {selectedDistrictName} ,ID: {selectedDistrictID}");
            }
            else
            {
                Edu_Error_Message = "សូមជ្រើសរើសស្រុក";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
        }
        public async Task SubmitAsync_Add_Communes()
        {
            ValidateCM_ID();
            ValidateCommune_Name_KH();
            ValidateSelectProvince_InComm();
            ValidateSelectDistrict_InComm();

            if (string.IsNullOrEmpty(CM_ID))
            {
                Edu_Error_Message = "សូមបញ្ចូល លេខសម្គាល់";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(Commune_Name_KH))
            {
                Edu_Error_Message = "សូមបញ្ចូល ឃុំ/ខណ្ឌ";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedProvince_Incomm == null)
            {
                Edu_Error_Message = "សូមជ្រើសរើសខេត្ត";
                MessageColor = new SolidColorBrush(Colors.Red);
                Debug.WriteLine("Submit failed: SelectedProvince_Info is null.");
                return;
            }
            if (SelectedDistrict_Incomm == null)
            {
                Edu_Error_Message = "សូមជ្រើសរើសស្រុក";
                MessageColor = new SolidColorBrush(Colors.Red);
                Debug.WriteLine("Submit failed: SelectedDistrict_Info is null.");
                return;
            }
            SelectedProvince_In_Commune();
            SelectedDistrict_In_Commune();
            SaveCommunetoDatabase();
            LoadCommunes_Info();
            Clear_Commune_Info_Text();

            await Task.CompletedTask;
        }
        //Get_data_to_combobox District
        private ObservableCollection<Communes_Info> _districts;
        public ObservableCollection<Communes_Info> Districts_Combobox
        {
            get { return _districts; }
            set
            {
                _districts = value;
                OnPropertyChanged(nameof(Districts_Combobox));
            }
        }
        //Get_data_to_Combobox Cummune
        private ObservableCollection<Village_Info> _villages;
        public ObservableCollection<Village_Info> Commune_Combobox
        {
            get { return _villages; }
            set
            {
                _villages = value;
                OnPropertyChanged(nameof(Commune_Combobox));
            }
        }
        //Load Data to Combobox Commune
        private void LoadData_to_Combobox_Commune(int districtID)
        {
            Commune_Combobox.Clear();
            var CommuneList = _dbConnection.GetCommune_toCombobox(districtID);
            if(CommuneList != null && CommuneList.Count>0)
            {
                foreach (var commune in CommuneList)
                {
                    Commune_Combobox.Add(commune);
                }
            }
            else
            {
                Debug.WriteLine($"No Communes found for District_ID: {districtID}");
            }
        }
        //Load Data to Combobox District
        private void LoadData_to_Combobox_District(int provinceID)
        {
            Districts_Combobox.Clear();
            var DistrictList = _dbConnection.GetDistrict_toCombobox(provinceID);
            if (DistrictList != null && DistrictList.Count > 0)
            {
               
                // Add the fetched districts to the Districts_Combobox collection
                foreach (var district in DistrictList)
                {
                    Districts_Combobox.Add(district);
                }
            }
            else
            {
                Debug.WriteLine($"No districts found for Province_ID: {provinceID}");
            }
        }
        //Save Data Commune to Database
        public void SaveCommunetoDatabase()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();

            var UpdateCommune = Commune_Info_ListView.FirstOrDefault(s => s.CM_ID == CM_ID);
            //Province ListView Get from top (Selected ListView).
            if (UpdateCommune != null)
            {
                Debug.WriteLine("Update Mode");
                UpdateCommune.CM_ID = CM_ID;
                UpdateCommune.Commune_Name_KH = Commune_Name_KH;
                UpdateCommune.Commune_Name_EN = Commune_Name_EN;
                // Assuming a ComboBox that selects a province and binds to SelectedProvince_Info
                if (SelectedDistrict_Incomm != null)
                {
                    UpdateCommune.District_ID = SelectedDistrict_Incomm.District_ID;  // Ensure valid Province_ID
                }
                else
                {
                    Edu_Error_Message = "សូមជ្រើសរើសស្រុកដែលឃុំស្ថិតនៅ!";
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }


                bool sucess = dbConnection.Update_Communes_Information(UpdateCommune);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + CM_ID + " បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + CM_ID + " បានធ្ចើបច្ចុប្បន្នភាពបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                //Insert Mode
                Communes_Info communes_Info = new Communes_Info()
                {
                    CM_ID = this.CM_ID,
                    Commune_Name_KH = this.Commune_Name_KH,
                    Commune_Name_EN = this.Commune_Name_EN,
                    //SelectedProvince_Incomm = SelectedProvince_Incomm.Province_ID,
                    SelectedDistrict_Incomm = SelectedDistrict_Incomm.District_ID,
                };
                Debug.WriteLine("Insert Mode");
                bool success = dbConnection.Insert_Communes(communes_Info);

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
        //Data to ListView
        public ObservableCollection<Communes_Info> Commune_Info_ListView
        {
            get => _communes_info;
            set
            {
                _communes_info = value;
                OnPropertyChanged(nameof(Commune_Info_ListView));  // Notify the UI when the Students collection changes
            }
        }
        //LoadDistrict_Info
        public void LoadCommunes_Info()
        {
            // Ensure _dbConnection is properly initialized
            if (_dbConnection == null)
            {
                Debug.WriteLine("_dbConnection is not initialized.");
                return;
            }

            var commune_list = _dbConnection.LoadCommunes_Info();

            if (commune_list != null && commune_list.Count > 0)
            {
                // Clear the existing items in the ObservableCollection
                Commune_Info_ListView.Clear();

                // Add new items from the database
                foreach (var commune_info in commune_list)
                {
                    Commune_Info_ListView.Add(commune_info);
                }
                Commune_Info_ListView = new ObservableCollection<Communes_Info>(commune_list);

            }
            else
            {
                Debug.WriteLine("No Commune Info are found.");
            }
        }

        //Select Data District from ListView to Combobox
        private Communes_Info _selectedCommunes;
        public Communes_Info SelectedCommunces
        {
            get => _selectedCommunes;
            set
            {
                _selectedCommunes = value;
                OnPropertyChanged();

                if (_selectedCommunes != null)
                {
                    CM_ID = _selectedCommunes.CM_ID;
                    Commune_Name_KH = _selectedCommunes.Commune_Name_KH;
                    Commune_Name_EN = _selectedCommunes.Commune_Name_EN;
                    // Find the matching province based on the selected district's province_id
                    SelectedProvince_Incomm = Provinces_Combobox
                        .FirstOrDefault(province => province.District_In_Pro == _selectedCommunes.Commune_In_Pro);
                    OnPropertyChanged(nameof(SelectedProvince_Incomm));

                    SelectedDistrict_Incomm = Districts_Combobox
                        .FirstOrDefault(district => district.Commune_In_Dis == _selectedCommunes.Commune_In_Dis);
                    OnPropertyChanged(nameof(SelectedDistrict_Incomm));
                }
                OnPropertyChanged(nameof(SelectedCommunces));
            }
        }
        //Delete Commune Command
        public async Task Delete_Commune_Info()
        {
            Delete_Communes_Info();
            LoadCommunes_Info() ;  
            Clear_Commune_Info_Text() ;

            await Task.CompletedTask;
        }
        //Method Delete Command
        public void Delete_Communes_Info()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var DeleteCommune = Commune_Info_ListView.FirstOrDefault(s => s.CM_ID == CM_ID);
            if (DeleteCommune != null)
            {
                DeleteCommune.CM_ID = CM_ID;

                Debug.WriteLine("Delete Mode");
                bool sucess = dbConnection.Delete_Commune_Information(DeleteCommune);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + CM_ID + " ទិន្នន័យលុបបានជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + CM_ID + " ទិន្នន័យលុបបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Edu_Error_Message = "លុបទិន្នន័យបរាជ័យ";
                MessageColor = new SolidColorBrush(Colors.Red);
            }
        }
        //End Commune Info

        //V_ID
        private int _V_ID;
        public int V_ID
        {
            get => _V_ID;
            set
            {
                if (_V_ID != value)
                {
                    _V_ID = value;
                    OnPropertyChanged(nameof(V_ID));
                }
            }
        }
        //VL_ID
        private string _VL_ID;
        public string VL_ID
        {
            get => _VL_ID;
            set
            {
                if (_VL_ID != value)
                {
                    _VL_ID = value;
                    OnPropertyChanged(nameof(VL_ID));
                    ValidateVL_ID();
                }
            }
        }
        //Village_Name_KH
        private string _Village_Name_KH;
        public string Village_Name_KH
        {
            get => _Village_Name_KH;
            set
            {
                if(_Village_Name_KH != value)
                {
                    _Village_Name_KH = value;
                    OnPropertyChanged(nameof(Village_Name_KH));
                    ValidateVillage_Name_KH();
                }
            }
        }
        //Village_Name_EN
        private string _Village_Name_EN;
        public string Village_Name_EN
        {
            get => _Village_Name_EN;
            set
            {
                if(_Village_Name_EN  != value)
                {
                    _Village_Name_EN = value;
                    OnPropertyChanged(nameof(Village_Name_EN));
                }
            }
        }
        //Commune_ID
        private int _Commune_ID;
        public int Commune_ID
        {
            get => _Commune_ID;
            set
            {
                _Commune_ID = value;
                OnPropertyChanged(nameof(Commune_ID));
            }
        }
        //Real-time validation method VL_ID
        public SolidColorBrush VL_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(VL_IDBorderBrush));
            }
        }
        //ValidateVL_ID
        private void ValidateVL_ID()
        {
            if (string.IsNullOrEmpty(VL_ID))
            {
                VL_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                VL_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Real-time validation method Village_Name_KH
        public SolidColorBrush Village_Name_KHBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Village_Name_KHBorderBrush));
            }
        }
        //ValidateVillage_Name_KH
        private void ValidateVillage_Name_KH()
        {
            if (string.IsNullOrEmpty(Village_Name_KH))
            {
                Village_Name_KHBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Village_Name_KHBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //SelectProvince
        private Village_Info _selectedProvince_InVill;
        public Village_Info SelectedProvince_InVill
        {
            get { return _selectedProvince_InVill; }
            set
            {
                _selectedProvince_InVill = value;
                OnPropertyChanged(nameof(SelectedProvince_InVill));
                ValidateSelectProvince_InComm();

            }
        }
        //SelectDistrict
        private Village_Info _selectedDistrict_InVill;
        public Village_Info SelectedDistrict_InVill
        {
            get { return _selectedDistrict_InVill; }
            set
            {
                _selectedDistrict_InVill = value;
                OnPropertyChanged(nameof(SelectedDistrict_InVill));
                ValidateSelectDistrict_InComm();

            }
        }
        //SelectedCommunce
        private Village_Info _selectedCommune_InVill;
        public Village_Info SelectedCommune_InVill
        {
            get { return _selectedCommune_InVill; }
            set
            {
                _selectedCommune_InVill = value;
                OnPropertyChanged(nameof(SelectedCommune_InVill));  // Notify UI that the property has changed
                ValidateSelectedCommune_InVill();

            }
        }
        //Border Combobox Commune
        public SolidColorBrush SelectedCommune_InVillBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectedCommune_InVillBorderBrush));
            }
        }
        //ValidateSelectCommune
        private void ValidateSelectedCommune_InVill()
        {
            if (SelectedCommune_InVill == null)
            {
                SelectedCommune_InVillBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectedCommune_InVillBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Select District
        public void SelectedComm_InVill()
        {
            if (SelectedCommune_InVill != null)
            {
                string selectedCommuneName = SelectedCommune_InVill.Village_In_Comm;
                int selectedCommuneID = SelectedCommune_InVill.Commune_ID;

                // Do something with the selected district or province
                Debug.WriteLine($"Selected Commune {selectedCommuneName} ,ID: {selectedCommuneID}");
            }   
        }

        public async Task SubmitAsync_Add_Villages()
        {
            ValidateVL_ID();
            ValidateVillage_Name_KH();
            ValidateSelectProvince_InComm();
            ValidateSelectDistrict_InComm();
            ValidateSelectedCommune_InVill();
            if (string.IsNullOrEmpty(VL_ID))
            {
                Edu_Error_Message = "សូមបញ្ចូល លេខសម្គាល់";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(Village_Name_KH))
            {
                Edu_Error_Message = "សូមបញ្ចូលឈ្មោះភូមិ";
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedProvince_Incomm == null)
            {
                Edu_Error_Message = "សូមជ្រើសរើសខេត្ត";
                MessageColor = new SolidColorBrush(Colors.Red);
                Debug.WriteLine("Submit failed: SelectedProvince_Info is null.");
                return;
            }
            if (SelectedDistrict_Incomm == null)
            {
                Edu_Error_Message = "សូមជ្រើសរើសស្រុក";
                MessageColor = new SolidColorBrush(Colors.Red);
                Debug.WriteLine("Submit failed: SelectedDistrict_Info is null.");
                return;
            }
            if (SelectedCommune_InVill == null)
            {
                Edu_Error_Message = "សូមជ្រើសរើសឃុំ";
                MessageColor = new SolidColorBrush(Colors.Red);
                Debug.WriteLine("Submit failed: SelectedCommune_Info is null.");
                return;
            }
            SelectedProvince_In_Commune();
            SelectedDistrict_In_Commune();
            SelectedComm_InVill();

            SaveVillagetoDatabase();
            LoadVillages_Info();
            Clear_Village_Info_Text();

            await Task.CompletedTask;
        }
        //Save and Update
        public void SaveVillagetoDatabase()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();

            var UpdateVillage = Village_Info_ListView.FirstOrDefault(s => s.VL_ID == VL_ID);
            //Province ListView Get from top (Selected ListView).
            if (UpdateVillage != null)
            {
                Debug.WriteLine("Update Mode");
                UpdateVillage.VL_ID = VL_ID;
                UpdateVillage.Village_Name_KH = Village_Name_KH;
                UpdateVillage.Village_Name_EN = Village_Name_EN;
                // Assuming a ComboBox that selects a province and binds to SelectedProvince_Info
                if (SelectedCommune_InVill != null)
                {
                    UpdateVillage.Commune_ID = SelectedCommune_InVill.Commune_ID; // Ensure valid Province_ID
                }
                else
                {
                    Edu_Error_Message = "សូមជ្រើសរើសស្រុកដែលឃុំស្ថិតនៅ!";
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }


                bool sucess = dbConnection.Update_Villages_Information(UpdateVillage);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + VL_ID + " បានធ្ចើបច្ចុប្បន្នភាពជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + VL_ID + " បានធ្ចើបច្ចុប្បន្នភាពបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                //Insert Mode
                Village_Info villages_Info = new Village_Info()
                {
                    VL_ID = this.VL_ID,
                    Village_Name_KH = this.Village_Name_KH,
                    Village_Name_EN = this.Village_Name_EN,
                    //SelectedProvince_Incomm = SelectedProvince_Incomm.Province_ID,
                    //SelectedDistrict_Incomm = SelectedDistrict_Incomm.District_ID,
                    SelectedCommune_InVill = SelectedCommune_InVill.Commune_ID
                };
                Debug.WriteLine("Insert Mode");
                bool success = dbConnection.Insert_Villages(villages_Info);

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
        //Data to ListView
        public ObservableCollection<Village_Info> Village_Info_ListView
        {
            get => _village_info;
            set
            {
                _village_info = value;
                OnPropertyChanged(nameof(Village_Info_ListView));
            }
        }
        //LoadVillage_Info
        public void LoadVillages_Info()
        {
            // Ensure _dbConnection is properly initialized
            if (_dbConnection == null)
            {
                Debug.WriteLine("_dbConnection is not initialized.");
                return;
            }

            var village_list = _dbConnection.LoadVillages_Info();

            if (village_list != null && village_list.Count > 0)
            {
                // Clear the existing items in the ObservableCollection
                Village_Info_ListView.Clear();

                // Add new items from the database
                foreach (var village_info in village_list)
                {
                    Village_Info_ListView.Add(village_info);
                }
                Village_Info_ListView = new ObservableCollection<Village_Info>(village_list);

            }
            else
            {
                Debug.WriteLine("No Village Info are found.");
            }
        }
        //Select Data Village from ListView to box
        private Village_Info _selectedVillages;
        public Village_Info SelectedVillages
        {
            get => _selectedVillages;
            set
            {
                _selectedVillages = value;
                OnPropertyChanged();

                if (_selectedVillages != null)
                {
                    VL_ID = _selectedVillages.VL_ID;
                    Village_Name_KH = _selectedVillages.Village_Name_KH;
                    Village_Name_EN = _selectedVillages.Village_Name_EN;
                    // Find the matching province based on the selected district's province_id
                    SelectedProvince_Incomm = Provinces_Combobox
                        .FirstOrDefault(province => province.District_In_Pro == _selectedVillages.Village_In_Pro);
                    OnPropertyChanged(nameof(SelectedProvince_Incomm));

                    SelectedDistrict_Incomm = Districts_Combobox
                        .FirstOrDefault(district => district.Commune_In_Dis == _selectedVillages.Village_In_Dis);
                    OnPropertyChanged(nameof(SelectedDistrict_Incomm));

                    SelectedCommune_InVill = Commune_Combobox
                        .FirstOrDefault(commune => commune.Village_In_Comm == _selectedVillages.Village_In_Comm);
                    OnPropertyChanged(nameof(SelectedCommune_InVill));
                }
                OnPropertyChanged(nameof(SelectedCommunces));
            }
        }
        //Delete Command
        public async Task Delete_Village_Info()
        {
            Delete_Villages_Info();
            LoadVillages_Info();
            Clear_Village_Info_Text();

            await Task.CompletedTask;
        }
        public void Delete_Villages_Info()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var DeleteVillage = Village_Info_ListView.FirstOrDefault(s => s.VL_ID == VL_ID);
            if (DeleteVillage != null)
            {
                DeleteVillage.VL_ID = VL_ID;

                Debug.WriteLine("Delete Mode");
                bool sucess = dbConnection.Delete_Village_Information(DeleteVillage);
                if (sucess)
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + VL_ID + " ទិន្នន័យលុបបានជោគជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    Edu_Error_Message = "លេខសម្កាល់ " + VL_ID + " ទិន្នន័យលុបបរាជ័យ";
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                Edu_Error_Message = "លុបទិន្នន័យបរាជ័យ";
                MessageColor = new SolidColorBrush(Colors.Red);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
