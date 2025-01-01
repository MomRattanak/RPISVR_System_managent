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
using System.Threading;
using Microsoft.UI.Dispatching;
using System.Data.Common;
using RPISVR_Managements.ViewModel;
using System.Data;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.Word;


namespace RPISVR_Managements.ViewModel
{
    public class StudentViewModel:INotifyPropertyChanged 
    {
        private ObservableCollection<Student_Info> _students;
        private ObservableCollection<Student_Info> _classes;
        private readonly DatabaseConnection _dbConnection;
        private DatabaseConnection _studentModel;
        private DatabaseConnection _classModel;

       
        //Student
        private int _currentPage = 1;
        private int _totalPages;
        private int _pageSize = 10;
        private int _totalStudents;
        public int TotalPages => (_totalStudents + _pageSize - 1) / _pageSize;

        //Class
        private int _currentPage_Class = 1;
        private int _totalPages_Class;
        private int _classSize = 10;
        private int _totalClasses;
        public int TotalPage_Class => (_totalClasses + _classSize - 1) / _classSize;

        
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand_Check { get; }
        public ICommand NextPageCommand_Check { get; }
        public ICommand PreviousPageCommand_Class { get; }
        public ICommand NextPageCommand_Class { get; }
        // Command to handle the form submission
        public ICommand SubmitCommand { get; }
        public ICommand ClearCommand { get; }

        //Command to Search_Stu_Info
        public ICommand Search_Stu_Info { get; }
        //GeneratePDFCommand Student Info
        public ICommand GeneratePdfCommand { get; }
        //GeneratePDFCommand Solarship Report
        public ICommand Search_Stu_Info_Report { get; }
        public ICommand GeneratePDFCommand_Solarship_Report { get; }
        //GenerateEXCELCommand Total Student Report
        public ICommand GenerateExcelCommand_Student_Report { get; }
        //GeneratePDF Command Student Card
        public ICommand GeneratePDFCommand_Student_Card { get; }
        public ICommand ShowStudentCardsCommand { get; }

       
        public  StudentViewModel()
        {
            //Classes
            SubmitCommand_Class = new RelayCommand(async () => await SubmitAsync_Class());
            ClearCommand_Class = new RelayCommand(async () => await ClearAsync_Class());
            Search_Class_Info = new RelayCommand(async () => await Search_Class_Information(Search_Class_In_Study_Year, Search_Class_In_Skill, Search_Class_In_Level, Search_Class_In_Student_Year, Search_Class_Semester, Search_Class_In_Study_Timeshift, Search_Class_In_Study_Type));
            Command_Edit_Class = new RelayCommand(async () => await Edit_Class());
            Command_Delete_Class = new RelayCommand(async () => await Delete_Class());
            Command_Edit_Class_Prepare = new RelayCommand(async () => await Edit_Class_Prepare());
            Clear_Class_Update = new RelayCommand(async () => await Clear_Class_UpdateAsync());
            Command_Add_Student_to_Class = new RelayCommand(async () => await Edit_Class_Prepare_Student());
            Clear_Class_In_Add_Student_Into_Class = new RelayCommand(async () => await Clear_Class_In_Add_Student());
            Command_Show_Student_In_Class = new RelayCommand(async () => await Get_Student_to_ListClassPrepare());
            Command_Insert_Students_to_Class = new RelayCommand(async () => await Insert_Students_to_Class());
            Command_ClearStudent_in_ClassList = new RelayCommand(async () => await ClearStudent_in_ClassList());
            Command_Delete_Student_in_Class = new RelayCommand(async () => await Delete_Student_in_Class());


            //Student
            SubmitCommand = new RelayCommand(async () => await SubmitAsync());
            ClearCommand = new RelayCommand(async () => await ClearAsync());
            Search_Stu_Info = new RelayCommand(async () => await Search_Student_Info(Search_Edu_Level, Search_Edu_Skill_Subject, Search_Edu_StudyTimeShift, Search_Edu_TypeStudy, Search_Edu_StudyYear));
            GeneratePdfCommand = new RelayCommand(async () => await GeneratePDF_Student_Information());
            //Report Student
            Search_Stu_Info_Report = new RelayCommand(async () => await Search_Education_Report_Solarship(SearchText_Education_Level, SearchText_Education_StudyYear, SearchText_Education_StudyType));
            GeneratePDFCommand_Solarship_Report = new RelayCommand(async () => await GeneratePDF_Solarship_Report());
            GenerateExcelCommand_Student_Report = new RelayCommand(async () => await GenerateExcel_Student_Report());
            ShowStudentCardsCommand = new RelayCommand(GenerateStudentCards);
            _dbConnection = new DatabaseConnection();

            //Student
            Students = new ObservableCollection<Student_Info>();
            Students_Check_Info = new ObservableCollection<Student_Info>();
            Student_Report_Solarship = new ObservableCollection<Student_Info>();
            Student_Report_Card = new ObservableCollection<Student_Info>();
            DisplayedStudentCards = new ObservableCollection<Student_Info>();
            //Classs
            Classes_Info = new ObservableCollection<Student_Info>();
            //Prepare Edit Class 
            Class_Info_Edit_Selected = new ObservableCollection<Student_Info>();
            //Add Student to Class 
            Class_Info_Add_Student_Selected = new ObservableCollection<Student_Info>();
            //Show Student for Class
            List_Students_Display = new ObservableCollection<Student_Info>();
            List_Student_In_Class_Display = new ObservableCollection<Student_Info>();

            //Command for Previouse,Back Button
            PreviousPageCommand = new RelayCommand(PreviousPage, CanGoPreviousPage);
            NextPageCommand = new RelayCommand(NextPage, CanGoNextPage);

            //Command for Previouse,Back Button
            PreviousPageCommand_Check = new RelayCommand(PreviousPage_Check, CanGoPreviousPage2);
            NextPageCommand_Check = new RelayCommand(NextPage_Check, CanGoNextPage2);

            //Class Back,Next
            NextPageCommand_Class = new RelayCommand(PreviousPage_Class, CanGoPreviousPage_Class);
            PreviousPageCommand_Class = new RelayCommand(NextPage_Class, CanGoNextPage_Class);
            //Card Report
            GeneratePDFCommand_Student_Card = new RelayCommand(async () => await GeneratePDF_Student_Card());
            //Edit
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

            //Get ID Class
            _classModel = new DatabaseConnection();
            _totalClasses = _classModel.GetTotalClassCount();

            //Data to Combobox Province
            Provinces_Combobox = new ObservableCollection<Student_Info>();
            Live_Provinces_Combobox = new ObservableCollection<Student_Info>();
            LoadData_to_Combobox_Province();
            LoadData_to_Combobox_LiveProvince();

            //Data to Combobox District
            Districts_Combobox = new ObservableCollection<Student_Info>();
            Live_Districts_Combobox = new ObservableCollection<Student_Info>();

            //Data to Combobox Commune
            Communes_Combobox = new ObservableCollection<Student_Info>();
            Live_Communes_Combobox =new ObservableCollection<Student_Info>();

            //Data to Combobox Village
            Villages_Combobox = new ObservableCollection<Student_Info>();
            Live_Villages_Combobox = new ObservableCollection<Student_Info>();

            //Data to Combobox EducationLevel
            EducationsLevel_Combobox = new ObservableCollection<Student_Info>();
            LoadData_to_Combobox_EducationLevel();

            //Data to Combobox EducationSkillSubject
            EducationSubjectSkill_Combobox = new ObservableCollection<Student_Info>();
            LoadData_to_Combobox_EducationSkillSubject();

            //Data to Combobox EducationStudyTimeShift
            EducationStudyTimeShift_Combobox = new ObservableCollection<Student_Info>();
            LoadData_to_Combobox_EducationStudyTimeShift();

            //Data to Combobox EducationStudyType
            EducationStudyType_Combobox = new ObservableCollection<Student_Info>();
            LoadData_to_Combobox_EducationStudyType();

            //Data to Combobox EducationStudyYear
            EducationStudyYear_Combobox = new ObservableCollection<Student_Info>();
            LoadData_to_Combobox_EducationStudyYear();

            //Student to ListView
            IsLoading = true;
            _ = LoadStudents(SearchText_ID_Name_Insert);

            //Classes to ListView
            _ = LoadClasstoListViews(Search_Class_Search_Name_Generation);

            
        }

        

        //Get_data_to_combobox Province
        private ObservableCollection<Student_Info> _provinces;
        public ObservableCollection<Student_Info> Provinces_Combobox
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                OnPropertyChanged(nameof(Provinces_Combobox));
            }
        }
        private ObservableCollection<Student_Info> _live_provinces;
        public ObservableCollection<Student_Info> Live_Provinces_Combobox
        {
            get { return _live_provinces; }
            set
            {
                _live_provinces = value;
                OnPropertyChanged(nameof(Live_Provinces_Combobox));
            }
        }
        //Get_data_to_combobox District
        private ObservableCollection<Student_Info> _districts;
        public ObservableCollection<Student_Info> Districts_Combobox
        {
            get { return _districts; }
            set
            {
                _districts = value;
                OnPropertyChanged(nameof(Districts_Combobox));
            }
        }
        private ObservableCollection<Student_Info> _live_districts;
        public ObservableCollection<Student_Info> Live_Districts_Combobox
        {
            get { return _live_districts; }
            set
            {
                _live_districts = value;
                OnPropertyChanged(nameof(Live_Districts_Combobox));
            }
        }
        //Get_data_to_combobox Commune
        private ObservableCollection<Student_Info> _communes;
        public ObservableCollection<Student_Info> Communes_Combobox
        {
            get { return _communes; }
            set
            {
                _communes = value;
                OnPropertyChanged(nameof(Communes_Combobox));
            }
        }
        private ObservableCollection<Student_Info> _live_communes;
        public ObservableCollection<Student_Info> Live_Communes_Combobox
        {
            get { return _live_communes; }
            set
            {
                _live_communes = value;
                OnPropertyChanged(nameof(Live_Communes_Combobox));
            }
        }
        //Get_data_to_combobox Village
        private ObservableCollection<Student_Info> _villages;
        public ObservableCollection<Student_Info> Villages_Combobox
        {
            get { return _villages; }
            set
            {
                _villages = value;
                OnPropertyChanged(nameof(Villages_Combobox));
            }
        }
        private ObservableCollection<Student_Info> _live_villages;
        public ObservableCollection<Student_Info> Live_Villages_Combobox
        {
            get { return _live_villages; }
            set
            {
                _live_villages = value;
                OnPropertyChanged(nameof(Live_Villages_Combobox));
            }
        }
        //Get data to Combobox Education Level
        private ObservableCollection<Student_Info> _education_level;
        public ObservableCollection<Student_Info> EducationsLevel_Combobox
        {
            get { return _education_level; }
            set
            {
                _education_level = value;
                OnPropertyChanged(nameof(EducationsLevel_Combobox));
            }
        }
        //Get data to Combobox Education Skill Subject
        private ObservableCollection<Student_Info> _education_subject_skill;
        public ObservableCollection<Student_Info> EducationSubjectSkill_Combobox
        {
            get { return _education_subject_skill; }
            set
            {
                _education_subject_skill = value;
                OnPropertyChanged(nameof(EducationSubjectSkill_Combobox));
            }
        }
        //Get data to Combobox Education StudyTimeShift
        private ObservableCollection<Student_Info> _education_studytimeshift;
        public ObservableCollection<Student_Info> EducationStudyTimeShift_Combobox
        {
            get { return _education_studytimeshift; }
            set
            {
                _education_studytimeshift = value;
                OnPropertyChanged(nameof(EducationStudyTimeShift_Combobox));
            }
        }
        //Get data to Combobox Education StudyType
        private ObservableCollection<Student_Info> _education_studytype;
        public ObservableCollection<Student_Info> EducationStudyType_Combobox
        {
            get { return _education_studytype; }
            set
            {
                _education_studytype = value;
                OnPropertyChanged(nameof(EducationStudyType_Combobox));
            }
        }
        //Get data to Combobox Education StudyYear
        private ObservableCollection<Student_Info> _education_studyyear;
        public ObservableCollection<Student_Info> EducationStudyYear_Combobox
        {
            get { return _education_studyyear; }
            set
            {
                _education_studyyear = value;
                OnPropertyChanged(nameof(EducationStudyYear_Combobox));
            }
        }
        //Load Data to Combobox EducationStudyYear
        private void LoadData_to_Combobox_EducationStudyYear()
        {
            var EducationStudyYearList = _dbConnection.GetEducationStudyYear_toCombobox_Student_Info();
            foreach (var education_studyyear in EducationStudyYearList)
            {
                EducationStudyYear_Combobox.Add(education_studyyear);
            }
        }
        //Load Data to Combobox EducationStudyType
        private void LoadData_to_Combobox_EducationStudyType()
        {
            var EducationStudyTypeList = _dbConnection.GetEducationStudyType_toCombobox_Student_Info();
            foreach (var education_studytype_skill in EducationStudyTypeList)
            {
                EducationStudyType_Combobox.Add(education_studytype_skill);
            }
        }
        //Load Data to Combobox EducationStudyTimeShift
        private void LoadData_to_Combobox_EducationStudyTimeShift()
        {
            var EducationStudyTimeShiftList = _dbConnection.GetEducationStudyTimeShift_toCombobox_Student_Info();
            foreach (var education_studytimeshift_skill in EducationStudyTimeShiftList)
            {
                EducationStudyTimeShift_Combobox.Add(education_studytimeshift_skill);
            }
        }
        //Load Data to Combobox EducationSkillSubject
        private void LoadData_to_Combobox_EducationSkillSubject()
        {
            var EducationSkillSubjectList = _dbConnection.GetEducationSkillSubject_toCombobox_Student_Info();
            foreach (var education_subject_skill in EducationSkillSubjectList)
            {
                EducationSubjectSkill_Combobox.Add(education_subject_skill);
            }
        }
        //Load Data to Combobox EducationLevel
        private void LoadData_to_Combobox_EducationLevel()
        {
            var EducatinLevelList = _dbConnection.GetEducationLevel_toCombobox_Student_info();
            foreach (var educationlevel in EducatinLevelList)
            {
                EducationsLevel_Combobox.Add(educationlevel);
            }
        }
        //Load Data to Combobox Province
        private void LoadData_to_Combobox_Province()
        {
            var ProvinceList = _dbConnection.GetProvince_toCombobox_Student_info();
            foreach (var province in ProvinceList)
            {
                Provinces_Combobox.Add(province);
            }
        }
        private void LoadData_to_Combobox_LiveProvince()
        {
            var Live_ProvinceList = _dbConnection.GetLive_Province_toCombobox_Student_info();
            foreach (var live_province in Live_ProvinceList)
            {
                Live_Provinces_Combobox.Add(live_province);
            }
        }
        //SelectedBirthProvince
        private Student_Info _selectedBirthProvince_Info;
        public Student_Info SelectedBirthProvince_Info
        {
            get { return _selectedBirthProvince_Info; }
            set
            {
                _selectedBirthProvince_Info = value;
                OnPropertyChanged(nameof(SelectedBirthProvince_Info)); 
                ValidateSelectProvince();
                if (_selectedBirthProvince_Info != null)
                {
                    Debug.WriteLine($"Selected Birth Province ID: {_selectedBirthProvince_Info.Stu_Birth_Province_ID}");
                    LoadData_to_Combobox_BirthDistrict(_selectedBirthProvince_Info.Stu_Birth_Province_ID);
                }

            }
        }
        //SelectedLiveProvince
        private Student_Info _selectedLiveProvince_Info;
        public Student_Info SelectedLiveProvince_Info
        {
            get { return _selectedLiveProvince_Info; }
            set
            {
                _selectedLiveProvince_Info = value;
                OnPropertyChanged(nameof(SelectedLiveProvince_Info));
                ValidateSelectLiveProvince();
                if (_selectedLiveProvince_Info != null)
                {
                    Debug.WriteLine($"Selected Live Province ID: {_selectedLiveProvince_Info.Stu_Live_Pro_ID}");
                    LoadData_to_Combobox_LiveDistrict(_selectedLiveProvince_Info.Stu_Live_Pro_ID);
                }

            }
        }
        //Real-time validation method Select Province
        public SolidColorBrush SelectProvinceBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectProvinceBorderBrush));
            }
        }
        public SolidColorBrush SelectLiveProvinceBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectLiveProvinceBorderBrush));
            }
        }
        //ValidateSelectProvince
        private void ValidateSelectProvince()
        {
            if (SelectedBirthProvince_Info == null)
            {
                SelectProvinceBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectProvinceBorderBrush = new SolidColorBrush(Colors.Green);               
            }
        }
        private void ValidateSelectLiveProvince()
        {
            if (SelectedLiveProvince_Info == null)
            {
                SelectLiveProvinceBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectLiveProvinceBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Select Province
        public void SelectedProvince_Combobox_Student_Info()
        {
            string selectedProvinceName = SelectedBirthProvince_Info?.Stu_Birth_Province;
            int selectedProvinceId = SelectedBirthProvince_Info?.Stu_Birth_Province_ID ?? -1; // Default value if null

            Debug.WriteLine($"Selected Province: {selectedProvinceName}, ID: {selectedProvinceId}");
        }
        public void SelectedLiveProvince_Combobox_Student_Info()
        {
            string selectedLiveProvinceName = SelectedLiveProvince_Info?.Stu_Live_Pro;
            int selectedLiveProvinceId = SelectedLiveProvince_Info?.Stu_Live_Pro_ID ?? -1; // Default value if null

            Debug.WriteLine($"Selected Province: {selectedLiveProvinceName}, ID: {selectedLiveProvinceId}");
        }
        //End_Province

        //Real-time validation method Select District
        public SolidColorBrush SelectDistrictBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectDistrictBorderBrush));
            }
        }
        public SolidColorBrush SelectLiveDistrictBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectLiveDistrictBorderBrush));
            }
        }
        //ValidateSelectDistrict
        private void ValidateSelectDistrict()
        {
            if (SelectedBirthDistrict_Info == null)
            {
                SelectDistrictBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectDistrictBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        private void ValidateSelectLiveDistrict()
        {
            if (SelectedLiveDistrict_Info == null)
            {
                SelectLiveDistrictBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectLiveDistrictBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Load Data to Combobox Birth District
        private void LoadData_to_Combobox_BirthDistrict(int Stu_Birth_Province_ID)
        {
            Districts_Combobox.Clear();
            var DistrictList = _dbConnection.GetBirthDistrict_toCombobox(Stu_Birth_Province_ID);
            if (DistrictList != null && DistrictList.Count > 0)
            {
                foreach (var district in DistrictList)
                {
                    Districts_Combobox.Add(district);
                }
            }
            else
            {
                Debug.WriteLine($"No District found for Province_ID: {Stu_Birth_Province_ID}");
            }

        }
        //Load Data to Combobox Live District
        private void LoadData_to_Combobox_LiveDistrict(int Stu_Live_Province_ID)
        {
            Live_Districts_Combobox.Clear();
            var Live_DistrictList = _dbConnection.GetLiveDistrict_toCombobox(Stu_Live_Province_ID);
            if (Live_DistrictList != null && Live_DistrictList.Count > 0)
            {
                foreach (var district in Live_DistrictList)
                {
                    Live_Districts_Combobox.Add(district);
                }
            }
            else
            {
                Debug.WriteLine($"No District found for Province_ID: {Stu_Birth_Province_ID}");
            }
        }

        //SelectedDistrict
        private Student_Info _selectedBirthDistrict_Info;
        public Student_Info SelectedBirthDistrict_Info
        {
            get { return _selectedBirthDistrict_Info; }
            set
            {
                _selectedBirthDistrict_Info = value;
                OnPropertyChanged(nameof(SelectedBirthDistrict_Info));
                ValidateSelectDistrict();
                if (_selectedBirthDistrict_Info != null)
                {
                    Debug.WriteLine($"Selected Birth District ID: {_selectedBirthDistrict_Info.Stu_Birth_District_ID}");
                    LoadData_to_Combobox_BirthCommune(_selectedBirthDistrict_Info.Stu_Birth_District_ID);
                }
            }
        }
        private Student_Info _selectedLiveDistrict_Info;
        public Student_Info SelectedLiveDistrict_Info
        {
            get { return _selectedLiveDistrict_Info; }
            set
            {
                _selectedLiveDistrict_Info = value;
                OnPropertyChanged(nameof(SelectedLiveDistrict_Info));
                ValidateSelectLiveDistrict();
                if (_selectedLiveDistrict_Info != null)
                {
                    Debug.WriteLine($"Selected Live District ID: {_selectedLiveDistrict_Info.Stu_Live_Dis_ID}");
                    LoadData_to_Combobox_LiveCommune(_selectedLiveDistrict_Info.Stu_Live_Dis_ID);
                }

            }
        }

        //SelectedDistrict_Combobox_Student_Info
        public void SelectedDistrit_Combobox_Student_Info()
        {
            string selectedDistrictName = SelectedBirthDistrict_Info?.Stu_Birth_Distric;
            int selectedDistrictId = SelectedBirthDistrict_Info?.Stu_Birth_District_ID ?? -1; // Default value if null

            Debug.WriteLine($"Selected District: {selectedDistrictName}, ID: {selectedDistrictId}");
        }
        public void SelectedLiveDistrit_Combobox_Student_Info()
        {
            string selectedLiveDistrictName = SelectedLiveDistrict_Info?.Stu_Live_Dis;
            int selectedLiveDistrictId = SelectedLiveDistrict_Info?.Stu_Live_Dis_ID ?? -1; // Default value if null

            Debug.WriteLine($"Selected District: {selectedLiveDistrictName}, ID: {selectedLiveDistrictId}");
        }
        //Real-time validation method Select Commune
        public SolidColorBrush SelectCommuneBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectCommuneBorderBrush));
            }
        }
        public SolidColorBrush SelectLiveCommuneBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectLiveCommuneBorderBrush));
            }
        }
        //ValidateSelectCommune
        private void ValidateSelectCommue()
        {
            if (_selectedBirthCommune_Info == null)
            {
                SelectCommuneBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectCommuneBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        private void ValidateSelectLiveCommue()
        {
            if (_selectedLiveCommune_Info == null)
            {
                SelectLiveCommuneBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectLiveCommuneBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Select Commune
        private Student_Info _selectedBirthCommune_Info;
        public Student_Info SelectedBirthCommune_Info
        {
            get { return _selectedBirthCommune_Info; }
            set
            {
                _selectedBirthCommune_Info = value;
                OnPropertyChanged(nameof(SelectedBirthCommune_Info));
                ValidateSelectCommue();
                if (_selectedBirthCommune_Info != null)
                {
                    Debug.WriteLine($"Selected Birth Commune ID: {_selectedBirthCommune_Info.Stu_Birth_Commune_ID}");
                    LoadData_to_Combobox_BirthVillage(_selectedBirthCommune_Info.Stu_Birth_Commune_ID);
                }
            }
        }
        //Live Commue
        private Student_Info _selectedLiveCommune_Info;
        public Student_Info SelectedLiveCommune_Info
        {
            get { return _selectedLiveCommune_Info; }
            set
            {
                _selectedLiveCommune_Info = value;
                OnPropertyChanged(nameof(SelectedLiveCommune_Info));
                ValidateSelectLiveCommue();
                if (_selectedLiveCommune_Info != null)
                {
                    Debug.WriteLine($"Selected Live Commune ID: {_selectedLiveCommune_Info.Stu_Live_Comm_ID}");
                    LoadData_to_Combobox_LiveVillage(_selectedLiveCommune_Info.Stu_Live_Comm_ID);
                }
            }
        }

        //Load Data to Combobox Birth Commune
        private void LoadData_to_Combobox_BirthCommune(int Stu_Birth_Commune_ID)
        {
            Communes_Combobox.Clear();
            var CommuneList = _dbConnection.GetBirthCommune_toCombobox(Stu_Birth_Commune_ID);
            if (CommuneList != null && CommuneList.Count > 0)
            {
                foreach (var commune in CommuneList)
                {
                    Communes_Combobox.Add(commune);
                }
            }
            else
            {
                Debug.WriteLine($"No Commune found for District ID: {Stu_Birth_District_ID}");
            }
        }
        //Load Data to Combobox Live Commune
        private void LoadData_to_Combobox_LiveCommune(int Stu_Live_Commune_ID)
        {
            Live_Communes_Combobox.Clear();
            var Live_CommuneList = _dbConnection.GetLiveCommune_toCombobox(Stu_Live_Commune_ID);
            if (Live_CommuneList != null && Live_CommuneList.Count > 0)
            {
                foreach (var live_commune in Live_CommuneList)
                {
                    Live_Communes_Combobox.Add(live_commune);
                }
            }
            else
            {
                Debug.WriteLine($"No Commune found for Commune ID: {Stu_Live_Comm_ID}");
            }
        }
        //SelectedCommune_Combobox_Student_Info
        public void SelectedCommune_Combobox_Student_Info()
        {
            string selectedCommuneName = SelectedBirthCommune_Info?.Stu_Birth_Commune;
            int selectedCommuneId = SelectedBirthCommune_Info?.Stu_Birth_Commune_ID ?? -1; // Default value if null

            Debug.WriteLine($"Selected Commune: {selectedCommuneName}, ID: {selectedCommuneId}");
        }
        public void SelectedLiveCommune_Combobox_Student_Info()
        {
            string selectedLiveCommuneName = SelectedLiveCommune_Info?.Stu_Live_Comm;
            int selectedLiveCommuneId = SelectedLiveCommune_Info?.Stu_Live_Comm_ID ?? -1; // Default value if null

            Debug.WriteLine($"Selected Commune: {selectedLiveCommuneName}, ID: {selectedLiveCommuneId}");
        }

        //Load Data to Combobox Birth Village
        private void LoadData_to_Combobox_BirthVillage(int Stu_Birth_Commune_ID)
        {
            Villages_Combobox.Clear();
            var VillageList = _dbConnection.GetBirthVillage_toCombobox(Stu_Birth_Commune_ID);
            if (VillageList != null && VillageList.Count > 0)
            {
                foreach (var village in VillageList)
                {
                    Villages_Combobox.Add(village);
                }
            }
            else
            {
                Debug.WriteLine($"No Village found for Commune ID: {Stu_Birth_Commune_ID}");
            }
        }
        private void LoadData_to_Combobox_LiveVillage(int Stu_Live_Comm_ID)
        {
            Live_Villages_Combobox.Clear();
            var Live_VillageList = _dbConnection.GetLiveVillage_toCombobox(Stu_Live_Comm_ID);
            if (Live_VillageList != null && Live_VillageList.Count > 0)
            {
                foreach (var live_village in Live_VillageList)
                {
                    Live_Villages_Combobox.Add(live_village);
                }
            }
            else
            {
                Debug.WriteLine($"No Village found for Commune ID: {Stu_Live_Comm_ID}");
            }
        }
        //Real-time validation method Select Village
        public SolidColorBrush SelectVillageBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectVillageBorderBrush));
            }
        }
        public SolidColorBrush SelectLive_VillageBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(SelectLive_VillageBorderBrush));
            }
        }
        //ValidateSelectVillage
        private void ValidateSelectVillage()
        {
            if (_selectedBirthVillage_Info == null)
            {
                SelectVillageBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectVillageBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        private void ValidateSelectLiveVillage()
        {
            if (_selectedLiveVillage_Info == null)
            {
                SelectLive_VillageBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SelectLive_VillageBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Select Village
        private Student_Info _selectedBirthVillage_Info;
        public Student_Info SelectedBirthVillage_Info
        {
            get { return _selectedBirthVillage_Info; }
            set
            {
                _selectedBirthVillage_Info = value;
                OnPropertyChanged(nameof(SelectedBirthVillage_Info));
                ValidateSelectVillage(); 
            }
        }
        private Student_Info _selectedLiveVillage_Info;
        public Student_Info SelectedLiveVillage_Info
        {
            get { return _selectedLiveVillage_Info; }
            set
            {
                _selectedLiveVillage_Info = value;
                OnPropertyChanged(nameof(SelectedLiveVillage_Info));
                ValidateSelectLiveVillage();
            }
        }
        //Selectedvillage_Combobox_Student_Info
        public void SelectedVillage_Combobox_Student_Info()
        {
            string selectedvillageName = SelectedBirthVillage_Info?.Stu_Birth_Village;
            int selectedVillageId = SelectedBirthVillage_Info?.Stu_Birth_Village_ID ?? -1; // Default value if null

            Debug.WriteLine($"Selected Village: {selectedvillageName}, ID: {selectedVillageId}");
        }
        public void SelectedLiveVillage_Combobox_Student_Info()
        {
            string selectedLivevillageName = SelectedLiveVillage_Info?.Stu_Live_Vill;
            int selectedLiveVillageId = SelectedLiveVillage_Info?.Stu_Live_Vill_ID ?? -1; // Default value if null

            Debug.WriteLine($"Selected Village: {selectedLivevillageName}, ID: {selectedLiveVillageId}");
        }

        //EducationLevel Selection
        private Student_Info _selectedEducationLevel_Info;
        public Student_Info SelectedEducationLevel_Info
        {
            get { return _selectedEducationLevel_Info; }
            set
            {
                _selectedEducationLevel_Info = value;
                OnPropertyChanged(nameof(SelectedEducationLevel_Info));
                ValidateStu_EducationLevels();
            }
        }
        //EducationSkillSubject
        private Student_Info _selectedEducationSkillSubject;
        public Student_Info SelectedEducationSubjects_Info
        {
            get { return _selectedEducationSkillSubject; }
            set
            {
                _selectedEducationSkillSubject = value;
                OnPropertyChanged(nameof(SelectedEducationSubjects_Info));
                ValidateStu_EducationSubjects();
            }
        }
        //EducationStudyTimeShift
        private Student_Info _selectedStudyTimeShift;
        public Student_Info SelectedStu_StudyTimeShift_Info
        {
            get { return _selectedStudyTimeShift; }
            set
            {
                _selectedStudyTimeShift = value;
                OnPropertyChanged(nameof(SelectedStu_StudyTimeShift_Info));
                ValidateStu_StudyTimeShift();
            }
        }
        //EducationStudyType
        private Student_Info _selectedEducationType;
        public Student_Info SelectedStu_EducationType_Info
        {
            get { return _selectedEducationType; }
            set
            {
                _selectedEducationType = value;
                OnPropertyChanged(nameof(SelectedStu_EducationType_Info));
               ValidateStu_Stu_TypeStudy();
            }
        }
        //EducationStudyYear
        private Student_Info _selectedEducationYear;
        public Student_Info SelectesStu_StudyYear_Info
        {
            get { return _selectedEducationYear; }
            set
            {
                _selectedEducationYear = value;
                OnPropertyChanged(nameof(SelectesStu_StudyYear_Info));
                ValidateStu_StudyYear();
            }
        }

        //Student_Info
        public ObservableCollection<Student_Info> Students
        {
            get => _students;
            set
            {
                _students = value;
                OnPropertyChanged(nameof(Students));  // Notify the UI when the Students collection changes
            }
        }
        
        //Students_Check_Info
        public ObservableCollection<Student_Info> Students_Check_Info
        {
            get => _students;
            set
            {
                _students = value;
                OnPropertyChanged(nameof(Students_Check_Info)); 
            }
        }
        
        //Student_Card_Report
        public ObservableCollection<Student_Info> Student_Report_Card
        {
            get => _students;
            set
            {
                _students = value;
                OnPropertyChanged(nameof(Student_Report_Card));
            }
        }
        
        //Class_Info
        public ObservableCollection<Student_Info> Classes_Info
        {
            get => _classes;
            set
            {
                _classes = value;
                OnPropertyChanged(nameof(Classes_Info));
            }
        }
        //Edit Prepare Class
        private ObservableCollection<Student_Info> _seleceted_class_edit;
        public ObservableCollection<Student_Info> Class_Info_Edit_Selected
        {
            get => _seleceted_class_edit;
            set
            {
                _seleceted_class_edit = value;
                OnPropertyChanged(nameof(Class_Info_Edit_Selected));
            }
        }
        //Add Student to Class
        private ObservableCollection<Student_Info> _seleceted_class_add_Student;
        public ObservableCollection<Student_Info> Class_Info_Add_Student_Selected
        {
            get => _seleceted_class_add_Student;
            set
            {
                _seleceted_class_add_Student = value;
                OnPropertyChanged(nameof(Class_Info_Add_Student_Selected));
            }
        }
        //List Student Display
        private ObservableCollection<Student_Info> _list_student_selected;
        public ObservableCollection<Student_Info> List_Students_Display
        {
            get => _list_student_selected;
            set
            {   
                _list_student_selected = value;
                OnPropertyChanged(nameof(List_Students_Display));
                
            }
        }
        //List Student In Class Display
        private ObservableCollection<Student_Info> _list_student_in_class_display;
        public ObservableCollection<Student_Info> List_Student_In_Class_Display
        {
            get => _list_student_in_class_display;
            set
            {
                _list_student_in_class_display = value;
                OnPropertyChanged(nameof(List_Student_In_Class_Display));
            }
        }
        //Search ID_Name in Insert_Student
        private string _searchText_ID_Name_Insert;
        public string SearchText_ID_Name_Insert
        {
            get => _searchText_ID_Name_Insert;
            set
            {
                if (_searchText_ID_Name_Insert != value)
                {
                    _searchText_ID_Name_Insert = value;
                    OnPropertyChanged(nameof(SearchText_ID_Name_Insert));
                    Debug.WriteLine("Property changed: SearchText_ID_Name_Insert = " + value);
                    OnSearchTextChanged_Insert(_searchText_ID_Name_Insert);
                }
            }
        }
        //Search By ID, Name
        private async void OnSearchTextChanged_Insert(string newText_ID_Name)
        {
            Debug.WriteLine($"Search ID or Name Insert Mode: {newText_ID_Name}");
            await LoadStudents(newText_ID_Name);
        }

        // Method to load students from the database, including images
        public async Task LoadStudents(string newText_ID_Name)
        {
           IsLoading = true;
            try
            {
                await Task.Delay(10);

                //
                var studentsList = _dbConnection.GetStudents_Info(CurrentPage, _pageSize, newText_ID_Name);
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
            finally
            {
                // Hide the loading indicator
                IsLoading = false;
            }
            await Task.CompletedTask;
            
        }

        //Student
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
       

        //Class
        public int CurrentPage_Class
        {
            get => _currentPage_Class;
            set
            {
                _currentPage_Class = value;
                OnPropertyChanged(nameof(CurrentPage_Class));
                OnPropertyChanged(nameof(PageInfo));
            }
        }
       

        private async void NextPage()
        {
            Debug.WriteLine("Next Page Command Executed");
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadStudents(SearchText_ID_Name_Insert);
                OnPageChanged();
                Debug.WriteLine($"Current Page: {CurrentPage}");
            }
        }
        private async void NextPage_Check()
        {
            Debug.WriteLine("Next Page Command Executed");
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                var searchTask = Search_Student_Info(Search_Edu_Level, Search_Edu_Skill_Subject, Search_Edu_StudyTimeShift, Search_Edu_TypeStudy,Search_Edu_StudyYear);
                var fetchTask = FetchStudentInfo(SearchText_ID_Name);

                await Task.WhenAll(searchTask, fetchTask);
                OnPageChanged();
                Debug.WriteLine($"Current Page Next Check: {CurrentPage}");
            }
            //OnPropertyChanged(nameof(CanGoNextPage2));
        }


        private async void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadStudents(SearchText_ID_Name_Insert);
                Debug.WriteLine($"Current Page: {CurrentPage}");
            }
            OnPropertyChanged(nameof(CanGoPreviousPage));  // Notify the UI to enable or disable the button
        }
        private async void PreviousPage_Check()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                var searchTask = Search_Student_Info(Search_Edu_Level, Search_Edu_Skill_Subject, Search_Edu_StudyTimeShift,Search_Edu_TypeStudy, Search_Edu_StudyYear);
                var fetchTask = FetchStudentInfo(SearchText_ID_Name);
                await Task.WhenAll(searchTask,fetchTask);
                Debug.WriteLine($"Current Page: {CurrentPage}");
            }
            OnPageChanged();
            
        }

        private bool CanGoPreviousPage()
        {
            return CurrentPage > 1;  // Enable only if not on the first page
        }

        private bool CanGoNextPage()
        {
            return CurrentPage < TotalPages;  // Enable only if not on the last page
        }

        //Check Student
        private bool CanGoPreviousPage2()
        {
            Debug.WriteLine($"CanGoPreviousPage2 Evaluated: {CurrentPage > 1}");
            return CurrentPage > 1;  // Enable only if not on the first page
        }

        private bool CanGoNextPage2()
        {
            Debug.WriteLine($"CanGoNextPage2 Evaluated: {CurrentPage < TotalPages}");
            return CurrentPage < TotalPages;  // Enable only if not on the last page
        }

        private void OnPageChanged()
        {
            //Insert Page
            (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            //Check Page
            (PreviousPageCommand_Check as RelayCommand)?.RaiseCanExecuteChanged();
            (NextPageCommand_Check as RelayCommand)?.RaiseCanExecuteChanged();
            //Class Page
            (PreviousPageCommand_Class as RelayCommand)?.RaiseCanExecuteChanged();
            (NextPageCommand_Class as RelayCommand)?.RaiseCanExecuteChanged();
        }


        //Color Border Error in Input Box.
        private SolidColorBrush _ErrorBorderBrush = new SolidColorBrush(Colors.Transparent); // Default transparent border

        //Color Stu_Generation
        public SolidColorBrush Stu_GenerationBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_GenerationBorderBrush));
            }
        }
        private void ValidateStu_Generation()
        {
            if (string.IsNullOrEmpty(Stu_Generation))
            {
                Stu_GenerationBorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_GenerationBorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }
        private string _Stu_Generation;
        public string Stu_Generation
        {
            get => _Stu_Generation;
            set
            {
                _Stu_Generation = value;
                OnPropertyChanged(nameof(Stu_Generation));
                ValidateStu_Generation();
            }
        }
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
            if (SelectedEducationLevel_Info == null)
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
            if (SelectedEducationSubjects_Info == null)
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
            if (SelectedStu_StudyTimeShift_Info == null)
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
            if (SelectedStu_EducationType_Info == null)
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
            if (SelectesStu_StudyYear_Info == null)
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
        //Validate Stu_StatePoor
        public SolidColorBrush Stu_StatePoor_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Stu_StatePoor_BorderBrush));
            }
        }
        private void ValidateStu_StatePoor()
        {
            if (string.IsNullOrEmpty(Stu_StatePoor))
            {
                Stu_StatePoor_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Stu_StatePoor_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
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

        //Stu_IDShow
        private string _Stu_IDShow;
        public string Stu_IDShow
        {
            get => _Stu_IDShow;
            set
            {
                _Stu_IDShow = value;
                OnPropertyChanged(nameof(Stu_IDShow));
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
        //Stu_BirthdayDateShow
        private string _Stu_BirthdayDateShow;
        public string Stu_BirthdayDateShow
        {
            get => _Stu_BirthdayDateShow;
            set
            {
                if(Stu_BirthdayDateShow != value)
                {
                    _Stu_BirthdayDateShow = value;
                    OnPropertyChanged(nameof(Stu_BirthdayDateShow));
                }
            }
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
                    OnPropertyChanged(nameof(Stu_Gender));
                }
            }
        }
        //String gender in Khmer
        public string Stu_Gender
        {
            get => _IsMale ? "ស្រី":"ប្រុស" ; // Return "ប្រុស" if IsMale is true, else "ស្រី"
        }

        //Stu_GenderShow
        private string _Stu_GenderShow;
        public string Stu_GenderShow
        {
            get => _Stu_GenderShow;
            set
            {
                if(_Stu_GenderShow != value)
                {
                    _Stu_GenderShow = value;
                    OnPropertyChanged(nameof(Stu_GenderShow));
                }
            }
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

        //Stu_Levels_ID
        private int _Stu_EducationLevels_ID;
        public int Stu_EducationLevels_ID
        {
            get => _Stu_EducationLevels_ID;
            set
            {
                if(_Stu_EducationLevels_ID  != value)
                {
                    _Stu_EducationLevels_ID = value;
                    OnPropertyChanged(nameof(Stu_EducationLevels_ID));
                    ValidateStu_EducationLevels();
                }
            }
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

        //Stu_EducationSubject_ID
        private int _Stu_EducationSubject_ID;
        public int Stu_EducationSubject_ID
        {
            get => _Stu_EducationSubject_ID;
            set
            {
                if(_Stu_EducationSubject_ID != value)
                {
                    _Stu_EducationSubject_ID = value;
                    OnPropertyChanged(nameof(Stu_EducationSubject_ID));
                    ValidateStu_EducationSubjects();
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

        //Stu_StudyTimeShift_ID
        private int _Stu_StudyTimeShift_ID;
        public int Stu_StudyTimeShift_ID
        {
            get => _Stu_StudyTimeShift_ID;
            set
            {
                if(_Stu_StudyTimeShift_ID != value)
                {
                    _Stu_StudyTimeShift_ID = value;
                    OnPropertyChanged(nameof(Stu_StudyTimeShift));
                    ValidateStu_StudyTimeShift();
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

        //Stu_TypeStudy_ID
        private int _Stu_EducationType_ID;
        public int Stu_EducationType_ID
        {
            get => _Stu_EducationType_ID;
            set
            {
                if(_Stu_EducationType_ID != value)
                {
                    _Stu_EducationType_ID = value;
                    OnPropertyChanged(nameof(Stu_EducationType_ID));
                    ValidateStu_Stu_TypeStudy();
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
        //Stu_Birth_Province_ID
        private int _Stu_Birth_Province_ID;
        public int Stu_Birth_Province_ID
        {
            get => _Stu_Birth_Province_ID;
            set
            {
                if(_Stu_Birth_Province_ID != value)
                {
                    _Stu_Birth_Province_ID = value;
                    OnPropertyChanged(nameof(Stu_Birth_Province_ID));
                    ValidateSelectProvince(); ;
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
                    ValidateSelectProvince();
                }
            }
        }
        //Stu_Birth_District_ID
        private int _Stu_Birth_District_ID;
        public int Stu_Birth_District_ID
        {
            get => _Stu_Birth_District_ID;
            set
            {
                if (Stu_Birth_District_ID != value)
                {
                    _Stu_Birth_District_ID = value;
                    OnPropertyChanged(nameof(Stu_Birth_District_ID));
                    ValidateSelectDistrict();
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
                    ValidateSelectDistrict();
                }
            }
        }
        //Stu_Birth_Commune_ID
        private int _Stu_Birth_Commune_ID;
        public int Stu_Birth_Commune_ID
        {
            get => _Stu_Birth_Commune_ID;
            set
            {
                if (Stu_Birth_Commune_ID != value)
                {
                    _Stu_Birth_Commune_ID = value;
                    OnPropertyChanged(nameof(Stu_Birth_Commune_ID));
                    ValidateSelectCommue();
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
                    ValidateSelectCommue();
                }
            }
        }
        //Stu_Birth_Village_ID
        private int _Stu_Birth_Village_ID;
        public int Stu_Birth_Village_ID
        {
            get => _Stu_Birth_Village_ID;
            set
            {
                if (Stu_Birth_Village_ID != value)
                {
                    _Stu_Birth_Village_ID = value;
                    OnPropertyChanged(nameof(Stu_Birth_Village_ID));
                    ValidateSelectVillage();
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

        //Stu_Live_Pro_ID
        private int _Stu_Live_Pro_ID;
        public int Stu_Live_Pro_ID
        {
            get => _Stu_Live_Pro_ID;
            set
            {
                if(_Stu_Live_Pro_ID != value)
                {
                    _Stu_Live_Pro_ID = value;
                    OnPropertyChanged(nameof(Stu_Live_Pro_ID));
                    ValidateSelectLiveProvince();
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
                    ValidateSelectLiveProvince();
                }
            }
        }

        //Stu_Live_Dis_ID
        private int _Stu_Live_Dis_ID;
        public int Stu_Live_Dis_ID
        {
            get => _Stu_Live_Dis_ID;
            set
            {
                if(_Stu_Live_Dis_ID  != value)
                {
                    _Stu_Live_Dis_ID = value;
                    OnPropertyChanged(nameof(Stu_Live_Dis_ID));
                    ValidateSelectLiveDistrict();
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
                    ValidateSelectLiveDistrict();
                }
            }
        }

        //Stu_Live_Comm_ID
        private int _Stu_Live_Comm_ID;
        public int Stu_Live_Comm_ID
        {
            get => _Stu_Live_Comm_ID;
            set
            {
                if(_Stu_Live_Comm_ID != value)
                {
                    _Stu_Live_Comm_ID = value;
                    OnPropertyChanged(nameof(Stu_Live_Comm_ID));
                    ValidateSelectLiveCommue();
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
                    ValidateSelectLiveCommue();
                }
            }
        }

        //Stu_Live_Vill_-ID
        private int _Stu_Live_Vill_ID;
        public int Stu_Live_Vill_ID
        {
            get => _Stu_Live_Vill_ID;
            set
            {
                if(_Stu_Live_Vill_ID  != value)
                {
                    _Stu_Live_Vill_ID = value;
                    OnPropertyChanged(nameof(Stu_Live_Vill_ID));
                    ValidateSelectLiveVillage();
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
                    ValidateSelectLiveVillage();
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

        //Stu_StudyYear_ID
        private int _Stu_StudyYear_ID;
        public int Stu_StudyYear_ID
        {
            get => _Stu_StudyYear_ID;
            set
            {
                if(_Stu_StudyYear_ID!= value)
                {
                    _Stu_StudyYear_ID=value;
                    OnPropertyChanged(nameof(Stu_StudyYear));
                    ValidateStu_StudyYear();
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
        //Stu_StatePoor
        private string _Stu_StatePoor;
        public string Stu_StatePoor
        {
            get => _Stu_StatePoor;
            set
            {
                if( _Stu_StatePoor != value)
                {
                    _Stu_StatePoor = value;
                    OnPropertyChanged(nameof(Stu_StatePoor));
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
                UpdateStudent.Stu_EducationLevels = SelectedEducationLevel_Info.Stu_EducationLevels;
                UpdateStudent.Stu_EducationSubjects = SelectedEducationSubjects_Info.Stu_EducationSubjects;
                UpdateStudent.Stu_StudyTimeShift = SelectedStu_StudyTimeShift_Info.Stu_StudyTimeShift;
                UpdateStudent.Stu_PhoneNumber = Stu_PhoneNumber;
                UpdateStudent.Stu_EducationType = SelectedStu_EducationType_Info.Stu_EducationType;
                UpdateStudent.Stu_NationalID = Stu_NationalID;
                UpdateStudent.Stu_StudyingTime = Stu_StudyingTime;
                UpdateStudent.Stu_Birth_Province = SelectedBirthProvince_Info.Stu_Birth_Province;
                UpdateStudent.Stu_Birth_Distric = SelectedBirthDistrict_Info.Stu_Birth_Distric;
                UpdateStudent.Stu_Birth_Commune = SelectedBirthCommune_Info.Stu_Birth_Commune;
                UpdateStudent.Stu_Birth_Village = SelectedBirthVillage_Info.Stu_Birth_Village;
                UpdateStudent.Stu_Live_Pro = SelectedLiveProvince_Info.Stu_Live_Pro;
                UpdateStudent.Stu_Live_Dis = SelectedLiveDistrict_Info.Stu_Live_Dis;
                UpdateStudent.Stu_Live_Comm = SelectedLiveCommune_Info.Stu_Live_Comm;
                UpdateStudent.Stu_Live_Vill = SelectedLiveVillage_Info.Stu_Live_Vill;
                UpdateStudent.Stu_Jobs = Stu_Jobs;
                UpdateStudent.Stu_School = Stu_School;
                UpdateStudent.Stu_StudyYear = SelectesStu_StudyYear_Info.Stu_StudyYear;
                UpdateStudent.Stu_Semester = Stu_Semester;
                UpdateStudent.Stu_StatePoor = Stu_StatePoor;
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
                UpdateStudent.Stu_Generation = Stu_Generation;

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
                    Stu_EducationLevels = this.SelectedEducationLevel_Info.Stu_EducationLevels,
                    Stu_EducationSubjects = this.SelectedEducationSubjects_Info.Stu_EducationSubjects,
                    Stu_StudyTimeShift = this.SelectedStu_StudyTimeShift_Info.Stu_StudyTimeShift,
                    Stu_PhoneNumber = this.Stu_PhoneNumber,
                    Stu_EducationType = this.SelectedStu_EducationType_Info.Stu_EducationType,
                    Stu_NationalID = this.Stu_NationalID,
                    Stu_StudyingTime = this.Stu_StudyingTime,
                    Stu_Birth_Province = this.SelectedBirthProvince_Info.Stu_Birth_Province,
                    Stu_Birth_Distric = this.SelectedBirthDistrict_Info.Stu_Birth_Distric,
                    Stu_Birth_Commune = this.SelectedBirthCommune_Info.Stu_Birth_Commune,
                    Stu_Birth_Village = this.SelectedBirthVillage_Info.Stu_Birth_Village,
                    Stu_Live_Pro = this.SelectedLiveProvince_Info.Stu_Live_Pro,
                    Stu_Live_Dis = this.SelectedLiveDistrict_Info.Stu_Live_Dis,
                    Stu_Live_Comm = this.SelectedLiveCommune_Info.Stu_Live_Comm,
                    Stu_Live_Vill = this.SelectedLiveVillage_Info.Stu_Live_Vill,
                    Stu_Jobs = this.Stu_Jobs,
                    Stu_School = this.Stu_School,
                    Stu_StudyYear = this.SelectesStu_StudyYear_Info.Stu_StudyYear,
                    Stu_Semester = this.Stu_Semester,
                    Stu_StatePoor = this.Stu_StatePoor,
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
                    Stu_Generation = this.Stu_Generation,


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
            Stu_Generation = string.Empty;
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
            //IsStuImage_Yes = _selectedStudent.Stu_Image_YesNo == "គ្មានរូបថត";
            //Stu_Image_YesNo = null;
            Stu_Image_Source =null;
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
            ValidateSelectProvince();
            ValidateSelectDistrict();
            ValidateSelectCommue();
            ValidateSelectVillage();
            ValidateSelectLiveProvince();
            ValidateSelectLiveDistrict();
            ValidateSelectLiveCommue();
            ValidateSelectLiveVillage();
            ValidateStu_Generation();
            ValidateStu_StudyYear();
            ValidateStu_Semester();
            ValidateStu_StatePoor();
            ValidateStu_Image_Total_Big();
            ValidateStu_Image_TotalSmall();
            SelectedProvince_Combobox_Student_Info();
            SelectedDistrit_Combobox_Student_Info();
            SelectedCommune_Combobox_Student_Info();
            SelectedVillage_Combobox_Student_Info();
            
            SelectedLiveDistrit_Combobox_Student_Info();
            SelectedLiveCommune_Combobox_Student_Info();
            SelectedLiveVillage_Combobox_Student_Info();
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
            if (SelectedEducationLevel_Info == null)
            {
                ErrorMessage = "កម្រិតសិក្សា ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Subject
            if (SelectedEducationSubjects_Info == null)
            {
                ErrorMessage = "ជំនាញ ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_TypeStudy
            if (SelectedStu_EducationType_Info == null)
            {
                ErrorMessage = "ប្រភេទសិក្សា ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            //Validate Stu_Generation
            if(string.IsNullOrEmpty(Stu_Generation))
            {
                ErrorMessage = "ជំនាន់ ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
           
            // Validate Stu_StudyTime
            if (SelectedStu_StudyTimeShift_Info == null)
            {
                ErrorMessage = "វេនសិក្សា ត្រូវតែជ្រើសរើស  !";
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
            if(SelectedBirthProvince_Info == null)
            {
                ErrorMessage = "ខេត្តកំណើត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
                      
            // Validate Stu_Birth_Distric
            if (SelectedBirthDistrict_Info == null)
            {
                ErrorMessage = "ស្រុក/ខណ្ឌកំណើត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Birth_Commune
            if (SelectedBirthCommune_Info == null)
            {
                ErrorMessage = "ឃុំ/សង្កាត់កំណើត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Birth_Village
            if (SelectedBirthVillage_Info == null)
            {
                ErrorMessage = "ភូមិកំណើត ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Live_Pro
            if (SelectedLiveProvince_Info == null)
            {
                ErrorMessage = "ខេត្តរស់នៅបច្ចុប្បន្ន ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Live_Dis
            if (SelectedLiveDistrict_Info == null)
            {
                ErrorMessage = "ស្រុក/ខណ្ឌរស់នៅបច្ចុប្បន្ន ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Live_Comm
            if (SelectedBirthCommune_Info == null)
            {
                ErrorMessage = "ឃុំ/សង្កាត់រស់នៅបច្ចុប្បន្ន ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_Live_Vill
            if (SelectedLiveVillage_Info == null)
            {
                ErrorMessage = "ភូមិរស់នៅបច្ចុប្បន្ន ត្រូវតែជ្រើសរើស !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            // Validate Stu_StudyYear
            if (SelectesStu_StudyYear_Info == null)
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
            //Validate Stu_StatePoor()
            if (string.IsNullOrEmpty(Stu_StatePoor))
            {
                ErrorMessage = "ប្រភេទសិស្សនិស្សិត ត្រូវតែជ្រើសរើស !";
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
            //Debug.WriteLine($"Stu_Birth_Pro:{Stu_Birth_Province}");
            
            Debug.WriteLine($"Stu_Birth_Distric: {Stu_Birth_Distric}");
            Debug.WriteLine($"Stu_Birth_Commune:{Stu_Birth_Commune}");
            Debug.WriteLine($"Stu_Birth_Village:{Stu_Birth_Village}");
            Debug.WriteLine($"Stu_Live_Pro:{Stu_Live_Pro}");
            Debug.WriteLine($"Stu_Live_Dis: {Stu_Live_Dis}");
            Debug.WriteLine($"Stu_Live_Comm:{Stu_Live_Comm}");
            Debug.WriteLine($"Stu_Live_Vill:{Stu_Live_Vill}");
            Debug.WriteLine($"Stu_Jobs:{Stu_Jobs}");
            Debug.WriteLine($"Stu_School:{Stu_School}");
            Debug.WriteLine($"Stu_Mother_Name:{Stu_Mother_Name}");
            Debug.WriteLine($"Stu_Mother_Phone:{Stu_Mother_Phone}");
            Debug.WriteLine($"Stu_Mother_Job:{Stu_Mother_Job}");
            Debug.WriteLine($"Stu_Father_Name:{Stu_Father_Name}");
            Debug.WriteLine($"Stu_Father_Phone:{Stu_Father_Phone}");
            Debug.WriteLine($"Stu_Father_Job:{Stu_Father_Job}");
            Debug.WriteLine($"Stu_StudyYear: {Stu_StudyYear}");
            Debug.WriteLine($"Stu_Semester: {Stu_Semester}");
            Debug.WriteLine($"Stu_StatePoor: {Stu_StatePoor}");
            Debug.WriteLine($"Stu_Image_YesNo:{Stu_Image_YesNo}");
            Debug.WriteLine($"Stu_ImageDegree_YesNo:{Stu_ImageDegree_YesNo}");
            Debug.WriteLine($"Stu_ImageBirth_Cert_YesNo:{Stu_ImageBirth_Cert_YesNo}");
            Debug.WriteLine($"Stu_ImageDNation_YesNo:{Stu_ImageIDNation_YesNo}");
            Debug.WriteLine($"Stu_ImagePoor_Card_YesNo:{Stu_ImagePoor_Card_YesNo}");
            Debug.WriteLine($"Stu_ImagePoor_Card_Source:{Stu_ImagePoor_Card_Source}");
            Debug.WriteLine($"Stu_Image_Poor_Card_Bytes:{Stu_Image_Poor_Card_Bytes}");
            Debug.WriteLine($"Stu_Image_Total_Big:{Stu_Image_Total_Big}");
            Debug.WriteLine($"Stu_Image_TotalSmall:{Stu_Image_TotalSmall}");
            Debug.WriteLine($"Stu_Generation: {Stu_Generation}");

            //Check Student Infomation Before Insert
            //var student_check_info = await _dbConnection.GetStudents_Check_Student_Info(Stu_FirstName_KH, Stu_LastName_KH, Stu_Gender, Stu_BirthdayDateOnly, Stu_EducationType = this.SelectedStu_EducationType_Info.Stu_EducationType, Stu_StudyYear = this.SelectesStu_StudyYear_Info.Stu_StudyYear);
            

            //if ((Stu_FirstName_KH == student_check_info.Stu_FirstName_KH1.Trim() &&
            //        Stu_LastName_KH == student_check_info.Stu_LastName_KH1.Trim() &&
            //        Stu_Gender.Trim() == student_check_info.Stu_Gender1.Trim() &&
            //        Stu_BirthdayDateOnly.Trim() == student_check_info.Stu_BirthdayDateOnly1.Trim() &&
            //        Stu_EducationType.Trim() == student_check_info.Stu_EducationType1.Trim() &&
            //        Stu_StudyYear.Trim() == student_check_info.Stu_StudyYear1.Trim()))
            //        {
            //            ErrorMessage = "និស្សិតឈ្មោះ " + Stu_FirstName_KH +Stu_LastName_KH+" "+ Stu_EducationType+" "+Stu_StudyYear+ " មានទិន្នន័យរួចរាល់ហើយ !";
            //            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
            //            MessageColor = new SolidColorBrush(Colors.Red);
            //            return;
            //        }
            //else
            //{
                // If everything is valid
                SaveStudentInformationToDatabase();
                ClearStudentInfo();
                await LoadStudents(SearchText_ID_Name_Insert);
            //}

            
            await Task.CompletedTask;
            
        }

        public async Task ClearAsync()
        {
            ClearStudentInfo();
            IsInsertEnabled = true;
            IsUpdateEnabled = false;

            await Task.CompletedTask;
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
                   //EducationLevel
                    SelectedEducationLevel_Info = EducationsLevel_Combobox
                        .FirstOrDefault(education_level =>  education_level.Stu_EducationLevels == _selectedStudent.Stu_EducationLevels);
                    OnPropertyChanged(nameof(SelectedEducationLevel_Info));
                    //EducationSkillsubject
                    SelectedEducationSubjects_Info = EducationSubjectSkill_Combobox
                        .FirstOrDefault(education_skillsubject => education_skillsubject.Stu_EducationSubjects == _selectedStudent.Stu_EducationSubjects);
                    OnPropertyChanged(nameof(SelectedEducationSubjects_Info));
                    //EducationStudyTimeShift
                    SelectedStu_StudyTimeShift_Info = EducationStudyTimeShift_Combobox
                        .FirstOrDefault(education_studytimeshift => education_studytimeshift.Stu_StudyTimeShift == _selectedStudent.Stu_StudyTimeShift);
                    OnPropertyChanged(nameof(SelectedStu_StudyTimeShift_Info));
                    //Stu_StudyTimeShift = _selectedStudent.Stu_StudyTimeShift;
                    Stu_PhoneNumber = _selectedStudent.Stu_PhoneNumber;
                    //EducationType
                    SelectedStu_EducationType_Info = EducationStudyType_Combobox
                        .FirstOrDefault(education_studytype => education_studytype.Stu_EducationType == _selectedStudent.Stu_EducationType);
                    OnPropertyChanged(nameof(SelectedStu_EducationType_Info));
                    //Stu_EducationType = _selectedStudent.Stu_EducationType;
                    Stu_NationalID = _selectedStudent.Stu_NationalID;
                    Stu_StudyingTime = _selectedStudent.Stu_StudyingTime;
                    //ListView Location to Combobox
                    //Province
                    SelectedBirthProvince_Info = Provinces_Combobox
                        .FirstOrDefault(province => province.Stu_Birth_Province == _selectedStudent.Stu_Birth_Province);
                    OnPropertyChanged(nameof(SelectedBirthProvince_Info));
                    //District
                    SelectedBirthDistrict_Info = Districts_Combobox
                        .FirstOrDefault(district => district.Stu_Birth_Distric == _selectedStudent.Stu_Birth_Distric);
                    OnPropertyChanged(nameof(SelectedBirthDistrict_Info));
                    //Commune
                    SelectedBirthCommune_Info = Communes_Combobox
                        .FirstOrDefault(commune => commune.Stu_Birth_Commune == _selectedStudent.Stu_Birth_Commune);
                    OnPropertyChanged(nameof(SelectedBirthCommune_Info));
                    //Village
                    SelectedBirthVillage_Info = Villages_Combobox
                        .FirstOrDefault(village => village.Stu_Birth_Village == _selectedStudent.Stu_Birth_Village);
                    OnPropertyChanged(nameof (SelectedBirthVillage_Info));
                    //Live Provice
                    SelectedLiveProvince_Info = Live_Provinces_Combobox
                        .FirstOrDefault(live_province => live_province.Stu_Live_Pro == _selectedStudent.Stu_Live_Pro);
                    OnPropertyChanged(nameof(SelectedLiveProvince_Info));
                    //Live District
                    SelectedLiveDistrict_Info = Live_Districts_Combobox
                        .FirstOrDefault(live_district => live_district.Stu_Live_Dis == _selectedStudent.Stu_Live_Dis);
                    OnPropertyChanged(nameof(SelectedLiveDistrict_Info));
                    //Live Commune
                    SelectedLiveCommune_Info = Live_Communes_Combobox
                        .FirstOrDefault(live_commune => live_commune.Stu_Live_Comm == _selectedStudent.Stu_Live_Comm);
                    OnPropertyChanged(nameof(SelectedLiveCommune_Info));
                    //Live Village
                    SelectedLiveVillage_Info = Live_Villages_Combobox
                        .FirstOrDefault(live_village => live_village.Stu_Live_Vill == _selectedStudent.Stu_Live_Vill);
                    OnPropertyChanged(nameof(SelectedLiveVillage_Info));

                    Stu_Jobs = _selectedStudent.Stu_Jobs;
                    Stu_School = _selectedStudent.Stu_School;
                    //Stu_StudyYear
                    SelectesStu_StudyYear_Info = EducationStudyYear_Combobox
                        .FirstOrDefault(study_year => study_year.Stu_StudyYear == _selectedStudent.Stu_StudyYear);
                    OnPropertyChanged(nameof(SelectesStu_StudyYear_Info));

                    Stu_Generation = _selectedStudent.Stu_Generation;
                    Stu_StatePoor = _selectedStudent.Stu_StatePoor;
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

        // The selected student in the ListView Check Student
        public Student_Info SelectedStudent_CheckStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged();               
                if (_selectedStudent != null)
                {
                    Stu_IDShow = _selectedStudent.Stu_ID;
                    Stu_FirstName_KH = _selectedStudent.Stu_FirstName_KH;
                    Stu_LastName_KH = _selectedStudent.Stu_LastName_KH;
                    Stu_FirstName_EN = _selectedStudent.Stu_FirstName_EN;
                    Stu_LastName_EN = _selectedStudent.Stu_LastName_EN;
                    Stu_GenderShow = _selectedStudent.Stu_Gender;
                    IsSingle = _selectedStudent.Stu_StateFamily == "មានគ្រួសារ";
                    Stu_Live_Vill = _selectedStudent.Stu_Live_Vill;
                    Stu_Live_Comm = _selectedStudent.Stu_Live_Comm;
                    Stu_Live_Dis = _selectedStudent.Stu_Live_Dis;
                    Stu_Live_Pro = _selectedStudent.Stu_Live_Pro;
                    Stu_BirthdayDateShow = _selectedStudent.Stu_BirthdayDateOnly;
                    //EducationLevel
                    Stu_EducationLevels = _selectedStudent.Stu_EducationLevels;              
                    //EducationSkillsubject
                    Stu_EducationSubjects = _selectedStudent.Stu_EducationSubjects;
                    //EducationStudyTimeShift
                    Stu_StudyTimeShift = _selectedStudent.Stu_StudyTimeShift; 
                    //Stu_StudyTimeShift = _selectedStudent.Stu_StudyTimeShift;
                    Stu_PhoneNumber = _selectedStudent.Stu_PhoneNumber;
                    //EducationType
                    Stu_EducationType = _selectedStudent.Stu_EducationType;
                    //Stu_EducationType = _selectedStudent.Stu_EducationType;
                    Stu_NationalID = _selectedStudent.Stu_NationalID;
                    Stu_StudyingTime = _selectedStudent.Stu_StudyingTime;                   
                    Stu_Jobs = _selectedStudent.Stu_Jobs;
                    Stu_School = _selectedStudent.Stu_School;
                    Stu_StudyYear = _selectedStudent.Stu_StudyYear;                  
                    Stu_Semester = _selectedStudent.Stu_Semester;
                    Stu_StatePoor = _selectedStudent.Stu_StatePoor;
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

                }
                OnPropertyChanged(nameof(SelectedStudent_CheckStudent));
                
            }
        }

        private string _searchText;
        public string SearchText_ID_Name
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText_ID_Name));                    
                    OnSearchTextChanged(_searchText);
                }
            }
        }
        //Search By ID, Name
        private async void OnSearchTextChanged(string newText)
        {
           Debug.WriteLine($"Search ID or Name Check Mode: {newText}");
            await FetchStudentInfo(newText);
        }
        
        public async Task FetchStudentInfo(string Search_ID_Name)
        {
            if (string.IsNullOrEmpty(Search_ID_Name))
            {
                var studentsList = _dbConnection.GetStudents_Check_Stu_Info(CurrentPage, _pageSize, Search_ID_Name);
                Students.Clear();
                foreach (var student in studentsList)
                {
                    Students.Add(student);
                }
                return;
            }
                
            IsLoading = true;
            try
            {
                await Task.Delay(10);

                //
                var studentsList = _dbConnection.GetStudents_Check_Stu_Info(CurrentPage, _pageSize, Search_ID_Name);
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
                (NextPageCommand_Check as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand_Check as RelayCommand)?.RaiseCanExecuteChanged();
            }
            finally
            {
                // Hide the loading indicator
                IsLoading = false;
            }

            await Task.CompletedTask;
        }

        //Choose Selection items for Search
        //Search_Edu_Level
        private string _search_Edu_Level;
        public string Search_Edu_Level
        {
            get => _search_Edu_Level;
            set
            {
                _search_Edu_Level = value;
                OnPropertyChanged(Search_Edu_Level);
            }
        }
        //Search_Edu_Skill_Subject
        private string _search_Edu_Skill_Subject;
        public string Search_Edu_Skill_Subject
        {
            get => _search_Edu_Skill_Subject;
            set
            {
                _search_Edu_Skill_Subject = value;
                OnPropertyChanged(nameof(Search_Edu_Skill_Subject));
            }
        }
        //Search_Edu_StudyTimeShift
        private string _search_Edu_StudyTimeShift;
        public string Search_Edu_StudyTimeShift
        {
            get => _search_Edu_StudyTimeShift;
            set
            {
                _search_Edu_StudyTimeShift = value;
                OnPropertyChanged(nameof(Search_Edu_StudyTimeShift));
            }
        }
        //Search_Edu_Type_Study
        private string _search_Edu_TypeStudy;
        public string Search_Edu_TypeStudy
        {
            get => _search_Edu_TypeStudy;
            set
            {
                _search_Edu_TypeStudy = value;
                OnPropertyChanged(nameof(Search_Edu_TypeStudy));
            }
        }
        //Search_StudyYear
        private string _search_Edu_StudyYear;
        public string Search_Edu_StudyYear
        {
            get => _search_Edu_StudyYear;
            set
            {
                _search_Edu_StudyYear = value;
                OnPropertyChanged(nameof(Search_Edu_StudyYear));
            }
        }
        //EducationLevel Selection
        private Student_Info _selectedEducationLevel_Stu_Info;
        public Student_Info SelectedEducationLevel_Stu_Info
        {
            get { return _selectedEducationLevel_Stu_Info; }
            set
            {
                _selectedEducationLevel_Stu_Info = value;
                OnPropertyChanged(nameof(SelectedEducationLevel_Stu_Info)); 

                if(_selectedEducationLevel_Stu_Info == null)
                {
                    Search_Edu_Level = null;
                }
                else
                {
                    Search_Edu_Level = _selectedEducationLevel_Stu_Info.Stu_EducationLevels;
                }
            }
        }
        //EducationSkillSubject
        private Student_Info _selectedEducationSkillSubject_Stu_Info;
        public Student_Info SelectedEducationSubjects_Stu_Info
        {
            get { return _selectedEducationSkillSubject_Stu_Info; }
            set
            {
                _selectedEducationSkillSubject_Stu_Info = value;
                OnPropertyChanged(nameof(SelectedEducationSubjects_Stu_Info));

                if (_selectedEducationSkillSubject_Stu_Info == null)
                {
                    Search_Edu_Skill_Subject = null;
                }
                else
                {
                    Search_Edu_Skill_Subject = _selectedEducationSkillSubject_Stu_Info.Stu_EducationSubjects;
                }
                
            }
        }
        //EducationStudyTimeShift
        private Student_Info _selectedStudyTimeShift_Stu_Info;
        public Student_Info SelectedStu_StudyTimeShift_Stu_Info
        {
            get { return _selectedStudyTimeShift_Stu_Info; }
            set
            {
                _selectedStudyTimeShift_Stu_Info = value;
                OnPropertyChanged(nameof(SelectedStu_StudyTimeShift_Stu_Info));
                if(_selectedStudyTimeShift_Stu_Info == null)
                {
                    Search_Edu_StudyTimeShift = null;
                }
                else
                {
                    Search_Edu_StudyTimeShift = this._selectedStudyTimeShift_Stu_Info.Stu_StudyTimeShift;
                }
            }
        }
        //EducationStudyType
        private Student_Info _selectedEducationType_Stu_Info;
        public Student_Info SelectedStu_EducationType_Stu_Info
        {
            get { return _selectedEducationType_Stu_Info; }
            set
            {
                _selectedEducationType_Stu_Info = value;
                OnPropertyChanged(nameof(SelectedStu_EducationType_Stu_Info));
                if(_selectedEducationType_Stu_Info == null)
                {
                    Search_Edu_TypeStudy = null;
                }
                else
                {
                    Search_Edu_TypeStudy = this._selectedEducationType_Stu_Info.Stu_EducationType;
                }
                
            }
        }
        //EducationStudyYear
        private Student_Info _selectedStu_StudyYear_Stu_Info;
        public Student_Info SelectedStu_StudyYear_Stu_Info
        {
            get => _selectedStu_StudyYear_Stu_Info;
            set
            {
                _selectedStu_StudyYear_Stu_Info = value;
                OnPropertyChanged(nameof(SelectedStu_StudyYear_Stu_Info));
                if(_selectedStu_StudyYear_Stu_Info == null)
                {
                    Search_Edu_StudyYear = null;
                }
                else
                {
                    Search_Edu_StudyYear = this._selectedStu_StudyYear_Stu_Info.Stu_StudyYear;
                }
            }
        }

        

        public async Task Search_Student_Info(string Search_Edu_Level,string Search_Edu_Skill_Subject,string Search_Edu_StudyTimeShift,string Search_Edu_TypeStudy,string Search_Edu_StudyYear)
        {       

            Debug.WriteLine("SelectedEducationLevel_Stu_Info = " + Search_Edu_Level);
            Debug.WriteLine("SelectedEducationSubjects_Stu_Info = " + Search_Edu_Skill_Subject);
            Debug.WriteLine("SelectedStu_StudyTimeShift_Stu_Info = " + Search_Edu_StudyTimeShift);
            Debug.WriteLine("SelectedStu_EducationType_Stu_Info = " + Search_Edu_TypeStudy);
            Debug.WriteLine("SelectedStu_StudyYear_Stu_Info = " + Search_Edu_StudyYear);

            if (string.IsNullOrEmpty(Search_Edu_Level) && string.IsNullOrEmpty(Search_Edu_Skill_Subject) && string.IsNullOrEmpty(Search_Edu_StudyTimeShift) && string.IsNullOrEmpty(Search_Edu_TypeStudy) && string.IsNullOrEmpty(Search_Edu_StudyYear))
            {
                var studentsList = _dbConnection.GetStudents_Check_Stu_Info_by_Combobox(CurrentPage, _pageSize, Search_Edu_Level, Search_Edu_Skill_Subject, Search_Edu_StudyTimeShift, Search_Edu_TypeStudy, Search_Edu_StudyYear);
                Students.Clear();
                foreach (var student in studentsList)
                {
                    Students.Add(student);
                }
                return;
            }


            IsLoading = true;
            try
            {
                await Task.Delay(10);

                //
                var studentsList = _dbConnection.GetStudents_Check_Stu_Info_by_Combobox(CurrentPage, _pageSize, Search_Edu_Level, Search_Edu_Skill_Subject, Search_Edu_StudyTimeShift, Search_Edu_TypeStudy, Search_Edu_StudyYear);
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
                (NextPageCommand_Check as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand_Check as RelayCommand)?.RaiseCanExecuteChanged();
            }
            finally
            {
                // Hide the loading indicator
                IsLoading = false;
            }

            await Task.CompletedTask;
        }

        //Search_Stu_ID_for_Edit
        private string _selectedStu_ID_Edit;
        public string SelectedStu_ID_Edit
        {
            get => _selectedStu_ID_Edit;
            set
            {
                _selectedStu_ID_Edit = value;
                OnPropertyChanged(nameof(SelectedStu_ID_Edit));
                if(SelectedStu_ID_Edit != value)
                {
                    SearchText_ID_Name = SelectedStu_ID_Edit;
                    OnPropertyChanged(nameof(SearchText_ID_Name));
                    Debug.WriteLine("Property changed: SearchText_ID_Name_Insert = " + value);
                }
            }
        }
        private bool _isEditModeforEdit;
        public bool IsEditModeforEdit
        {
            get => _isEditModeforEdit;
            set
            {
                _isEditModeforEdit = value;
                OnPropertyChanged(nameof(IsEditModeforEdit));
                Debug.WriteLine("IsEditModeforEdit set to: " + value);
            }
        }
        public void SetEditMode(bool isEditMode)
        {
            IsEditModeforEdit = isEditMode;
        }

        //GeneratePDF Student Information 
        public async Task GeneratePDF_Student_Information()
        {
            if(_selectedStudent != null) 
            {       
                // Convert date to Khmer format and assign it to a property in _selectedStudent
                _selectedStudent.Stu_BirthdayDateShow = ConvertToKhmerDate(_selectedStudent.Stu_BirthdayDateOnly);
                
                PdfReportService_Student_Info.CreateReport(_selectedStudent);
            }
            else
            {
                Debug.WriteLine("No student selected for PDF generation.");
            }
            await Task.CompletedTask;
        }

        // Dictionary to map month numbers to Khmer month names
        private static readonly Dictionary<int, string> KhmerMonths_Report = new Dictionary<int, string>
{
    { 1, "មករា" },
    { 2, "កុម្ភៈ" },
    { 3, "មីនា" },
    { 4, "មេសា" },
    { 5, "ឧសភា" },
    { 6, "មិថុនា" },
    { 7, "កក្កដា" },
    { 8, "សីហា" },
    { 9, "កញ្ញា" },
    { 10, "តុលា" },
    { 11, "វិច្ឆិកា" },
    { 12, "ធ្នូ" }
};
        public string ConvertToKhmerDate(string date)
        {
            try
            {
                // Parse the date string with exact format "dd/MM/yyyy"
                DateTime parsedDate = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                int day = parsedDate.Day;
                int month = parsedDate.Month;
                int year = parsedDate.Year;

                // Get Khmer month name
                string khmerMonth = KhmerMonths_Report.ContainsKey(month) ? KhmerMonths_Report[month] : month.ToString();

                // Format the date as "day KhmerMonth year"
                return $"{day} {khmerMonth} {year}";
            }
            catch (FormatException)
            {
                // If parsing fails, return the original date string or handle the error as needed
                return date;
            }
        }
        //Full Name KH
        private string _Full_Name_KH;
        public string Full_Name_KH
        {
            get => _Full_Name_KH;
            set
            {
                _Full_Name_KH = value;
                OnPropertyChanged(nameof(Full_Name_KH));
            }
        }
        //Full Name EN
        private string _Full_Name_EN;
        public string Full_Name_EN
        {
            get => _Full_Name_EN;
            set
            {
                _Full_Name_EN = value;
                OnPropertyChanged(nameof(Full_Name_EN));
            }
        }

        //Student_Report_Solarship
        private ObservableCollection<Student_Info> _students_solarship;
        public ObservableCollection<Student_Info> Student_Report_Solarship
        {
            get => _students_solarship;
            set
            {
                _students_solarship = value;
                OnPropertyChanged(nameof(Student_Report_Solarship));
            }
        }
        //Search Education Level by Comboobox report
        private string _searchText_Education_Level;
        public string SearchText_Education_Level
        {
            get => _searchText_Education_Level;
            set
            {
                if (_searchText_Education_Level != value)
                {
                    _searchText_Education_Level = value;
                    OnPropertyChanged(nameof(SearchText_Education_Level));
                }

            }
        }
        //Search Education StudyYear
        private string _SearchText_Education_StudyYear;
        public string SearchText_Education_StudyYear
        {
            get => _SearchText_Education_StudyYear;
            set
            {
                _SearchText_Education_StudyYear = value;
                OnPropertyChanged(nameof(SearchText_Education_StudyYear));
            }
        }
        //Search Education StudyType
        private string _SearchText_Education_StudyType;
        public string SearchText_Education_StudyType
        {
            get => _SearchText_Education_StudyType;
            set
            {
                _SearchText_Education_StudyType = value;
                OnPropertyChanged(nameof(SearchText_Education_StudyType));
            }
        }

        //SearchText_Education_StudyType_Text
        private Student_Info _SearchText_Education_StudyType_Text;
        public Student_Info SearchText_Education_StudyType_Text
        {
            get => _SearchText_Education_StudyType_Text;
            set
            {
                _SearchText_Education_StudyType_Text = value;
                OnPropertyChanged(nameof(SearchText_Education_StudyType_Text));
                if(_SearchText_Education_StudyType_Text == null)
                {
                    SearchText_Education_StudyType = null;
                }
                else
                {
                    SearchText_Education_StudyType = this._SearchText_Education_StudyType_Text.Stu_EducationType;
                }
                
            }
        }
        //SearchText_Education_StudyYear_Text
        private Student_Info _SearchText_Education_StudyYear_Text;
        public Student_Info SearchText_Education_StudyYear_Text
        {
            get => _SearchText_Education_StudyYear_Text;
            set
            {
                _SearchText_Education_StudyYear_Text = value;
                OnPropertyChanged(nameof(SearchText_Education_StudyYear_Text));
                if(_SearchText_Education_StudyYear_Text == null)
                {
                    SearchText_Education_StudyYear = null;                   
                }
                else
                {
                    SearchText_Education_StudyYear = this._SearchText_Education_StudyYear_Text.Stu_StudyYear;
                }
                
            }
        }
        //SearchText_Education_Level_Text
        private Student_Info _search_Education_Level_Text;
        public Student_Info SearchText_Education_Level_Text
        {
            get => _search_Education_Level_Text;
            set
            {
                _search_Education_Level_Text = value;
                OnPropertyChanged(nameof(SearchText_Education_Level_Text));
                if (_search_Education_Level_Text == null)
                {
                    SearchText_Education_Level = null;
                    Education_Level_Text = "បរិញ្ញាបត្របច្ចេកវិទ្យា សញ្ញាបត្រវិស្វករ បរិញ្ញាបត្រ បរិញ្ញាបត្ររង សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ១ សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ២ និងសញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ៣ ";     
                }
                else
                {
                    SearchText_Education_Level = _search_Education_Level_Text.Stu_EducationLevels;
                    if(SearchText_Education_Level == "បរិញ្ញាបត្របច្ចេកវិទ្យា"|| SearchText_Education_Level == "សញ្ញាបត្រវិស្វករ" || SearchText_Education_Level == "បរិញ្ញាបត្រ")
                    {
                        Education_Level_Text = "បរិញ្ញាបត្របច្ចេកវិទ្យា សញ្ញាបត្រវិស្វករ បរិញ្ញាបត្រ";
                    }
                    else if(SearchText_Education_Level == "បរិញ្ញាបត្ររង" || SearchText_Education_Level == "សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស")
                    {
                        Education_Level_Text = "បរិញ្ញាបត្ររង សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស";
                    }
                    else
                    {
                        Education_Level_Text = SearchText_Education_Level;
                    }
                    Debug.WriteLine($"SearchText_Education_Level: {SearchText_Education_Level}");
                }
            }
        }

        public async Task Search_Education_Report_Solarship(string SearchText_Education_Level,string SearchText_Education_StudyYear, string SearchText_Education_StudyType)
        {

            if (string.IsNullOrEmpty(SearchText_Education_Level) && string.IsNullOrEmpty(SearchText_Education_StudyYear) && string.IsNullOrEmpty(SearchText_Education_StudyType))
            {
                Student_Report_Solarship.Clear();
                ErrorMessage = "សូមជ្រើសរើស ជម្រើសទាំងបីជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Report_Student_Info_Icon/icons8-choose-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Debug.WriteLine($"Select {SearchText_Education_Level}");
            Debug.WriteLine($"Select {SearchText_Education_StudyYear}");
            Debug.WriteLine($"Select {SearchText_Education_StudyType}");
            IsLoading = true;
            try
            {
                await Task.Delay(10);

                //
                var studentsList = _dbConnection.GetStudents_Report_Stu_Info_by_Solarship(SearchText_Education_Level, SearchText_Education_StudyYear, SearchText_Education_StudyType);
                // Clear the existing list to prepare for the new page data
                Student_Report_Solarship.Clear();

                // Iterate over the studentsList returned by the database and add them to the ObservableCollection
                foreach (var student in studentsList)
                {
                    Student_Report_Solarship.Add(student);
                }

                Student_Report_Solarship = new ObservableCollection<Student_Info>(studentsList);
            }
            finally
            {
                // Hide the loading indicator
                IsLoading = false;
            }
            await Task.CompletedTask;
        }
        //Multi Selection Student Total
        private List<Student_Info> _selectedStudents_Report = new List<Student_Info>();
        public List<Student_Info> SelectedStudents_Report
        {
            get => _selectedStudents_Report;
            set
            {
                _selectedStudents_Report = value;
                OnPropertyChanged(nameof(SelectedStudents_Report));               
            }
        }

        //Education_Level_Text
        private string _Education_Level_Text;
        public string Education_Level_Text
        {
            get => _Education_Level_Text;
            set
            {
                _Education_Level_Text = value;
                OnPropertyChanged(nameof(Education_Level_Text));
            }
        }
        //Education_Start_Date
        private string _Education_Start_Date;
        public string Education_Start_Date
        {
            get => _Education_Start_Date;
            set
            {
                _Education_Start_Date = value;
                OnPropertyChanged(nameof(Education_Start_Date));
            }
        }

        //Solarship Report PDF
        public async Task GeneratePDF_Solarship_Report()
        {
            if(SelectedStudents_Report == null || !SelectedStudents_Report.Any() && Education_Level_Text == null && SearchText_Education_StudyType == null && SearchText_Education_StudyYear == null)
            {
                Student_Report_Solarship.Clear();
                ErrorMessage = "សូមជ្រើសរើស ជម្រើសទាំងបី និងសិស្សនិស្សិតជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Report_Student_Info_Icon/icons8-choose-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                Debug.WriteLine("No student selection.");
                return;   
            }
            else
            {
                
                PDFService_Report_Student_Solarship.CreateReport(SelectedStudents_Report, SearchText_Education_StudyType, Education_Level_Text, Education_Start_Date, SearchText_Education_StudyYear);
                Debug.WriteLine("PDF reports generated for all selected students.");
            }
            //foreach (var student in SelectedStudents_Report)
            //{
            //    // Convert the birthday date for each student
            //    student.Stu_BirthdayDateShow = ConvertToKhmerDate(student.Stu_BirthdayDateOnly);

            //    PDFService_Report_Student_Solarship.CreateReport(student);
            //    await Task.Delay(1000); // 1-second delay
            //}
            
            await Task.CompletedTask;
        }
        

        //Student Report Excel
        public async Task GenerateExcel_Student_Report()
        {
            if (SelectedStudents_Report == null || !SelectedStudents_Report.Any() && Education_Level_Text == null && SearchText_Education_StudyType == null && SearchText_Education_StudyYear == null)
            {
                Student_Report_Solarship.Clear();
                ErrorMessage = "សូមជ្រើសរើស ជម្រើសទាំងបី និងសិស្សនិស្សិតជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Report_Student_Info_Icon/icons8-choose-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                Debug.WriteLine("No student selection.");
                return;
            }
            else
            {
                ExportExcel_Student_Report.ExportToExcel(SelectedStudents_Report.ToList(),Education_Level_Text);
            }
                await Task.CompletedTask;
        }

        //Multi Selection Student Card
        private List<Student_Info> _selectedStudent_Card = new List<Student_Info>();
        public List<Student_Info> Selection_Student_Card
        {
            get => _selectedStudent_Card;
            set
            {
                _selectedStudent_Card = value;
                if (_selectedStudent_Card != null && _selectedStudent_Card.Count > 0)
                {
                    // Convert the birthdates of selected students
                    Stu_BirthdayDateShow = string.Join(", ",
                        _selectedStudent_Card.Select(student => ConvertToKhmerDate(student.Stu_BirthdayDateOnly)));
                }
                else
                {
                    return;
                }
                OnPropertyChanged(nameof(Selection_Student_Card));
            }
        }

        //Preview Student Card
        private ObservableCollection<Student_Info> _displayedStudentCards;
        public ObservableCollection<Student_Info> DisplayedStudentCards
        {
            get => _displayedStudentCards;
            set
            {
                _displayedStudentCards = value;
                OnPropertyChanged(nameof(DisplayedStudentCards));
            }
        }



        private void GenerateStudentCards()
        {
            
            DisplayedStudentCards.Clear();
            if(Selection_Student_Card == null)
            {
                Debug.WriteLine("No Selection");
                return;
            }
            else
            {
                foreach (var student in Selection_Student_Card)
                {
                    DisplayedStudentCards.Add(student);
                }
                Debug.WriteLine($"DisplayedStudentCards Count: {DisplayedStudentCards.Count}");
            }
            
        }

        
        //Student Card Command
        public async Task GeneratePDF_Student_Card()
        {
            if(Selection_Student_Card == null && Selection_Student_Card.Any())
            {         
                ErrorMessage = "សូមជ្រើសរើសសិស្សនិស្សិតជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Report_Student_Info_Icon/icons8-choose-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                OnPropertyChanged(nameof(Selection_Student_Card));
                Debug.WriteLine("No student selection.");
                return;
            }
            else
            {
                // Convert the birthdates of selected students
                foreach (var student in _selectedStudent_Card)
                {
                    string baseUrl = "http://localhost/Student_Card/index.php";
                    student.Stu_BirthdayDateShow = ConvertToKhmerDate(student.Stu_BirthdayDateOnly);
                    string studentID = student.Stu_ID;
                    string studentInfoUrl = $"{baseUrl}?stu_id={studentID}";
                    student.QRCodeBytes = QRCodeService.GenerateQRCode(studentInfoUrl);

                    Debug.WriteLine($"QR Code generated successfully for {student.Stu_ID}");
                }

                PDFService_Generate_Student_Card.CreateCard_Report(Selection_Student_Card);
                OnPropertyChanged(nameof(Selection_Student_Card));
                Debug.WriteLine("Report Card OK");

            }
            
            await Task.CompletedTask;
        }

        //Classes
        public ICommand SubmitCommand_Class { get; }
        public ICommand ClearCommand_Class { get; }
        public ICommand Search_Class_Info { get; }


        //No_Class
        private string _No_Class;
        public string No_Class
        {
            get => _No_Class;
            set
            {
                _No_Class = value;
                OnPropertyChanged(nameof(No_Class));
            }
        }
        //Class_ID
        private string _Class_ID;
        public string Class_ID
        {
            get => _Class_ID;
            set
            {
                if (_Class_ID != value)
                {
                    _Class_ID = value;
                    OnPropertyChanged(nameof(Class_ID));
                    ValidateClass_ID();
                }

            }
        }
        //ClassIDBorderBrush
        public SolidColorBrush ClassIDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(ClassIDBorderBrush));
            }
        }
        private void ValidateClass_ID()
        {
            if (string.IsNullOrEmpty(Class_ID))
            {
                ClassIDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ClassIDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Class_Name
        private string _Class_Name;
        public string Class_Name
        {
            get => _Class_Name;
            set
            {
                if (_Class_Name != value)
                {
                    _Class_Name = value;
                    OnPropertyChanged(nameof(Class_Name));
                    ValidateClass_Name();
                }

            }
        }
        //Class_NameBorderBrush
        public SolidColorBrush Class_NameBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Class_NameBorderBrush));
            }
        }
        private void ValidateClass_Name()
        {
            if (string.IsNullOrEmpty(Class_Name))
            {
                Class_NameBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Class_NameBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Class_In_Skill
        private int _Class_In_Skill_ID;
        public int Class_In_Skill_ID
        {
            get => _Class_In_Skill_ID;
            set
            {
                
                _Class_In_Skill_ID = value;
                OnPropertyChanged(nameof(Class_In_Skill_ID));

            }
        }
        //Class_In_Study_Year
        private string _Class_In_Study_Year;
        public string Class_In_Study_Year
        {
            get => _Class_In_Study_Year;
            set
            {
                _Class_In_Study_Year = value;
                OnPropertyChanged(nameof(Class_In_Study_Year));
            }
        }
        //Class_In_Skill
        private string _Class_In_Skill;
        public string Class_In_Skill
        {
            get => _Class_In_Skill;
            set
            {
                if (_Class_In_Skill != value)
                {
                    _Class_In_Skill = value;
                    OnPropertyChanged(nameof(Class_In_Skill));
                    ValidateClass_In_Skill_Select();
                }            
            }
        }
        //Class_In_Level
        private string _Class_In_Level;
        public string Class_In_Level
        {
            get => _Class_In_Level;
            set
            {
                _Class_In_Level = value;
                OnPropertyChanged(nameof(Class_In_Level));
                ValidateClass_In_Level_Select();
            }
        }
        //Class_In_Student_Year
        private string _Class_In_Student_Year;
        public string Class_In_Student_Year
        {
            get => _Class_In_Student_Year;
            set
            {
                _Class_In_Student_Year = value;
                OnPropertyChanged(nameof(Class_In_Student_Year));
                ValidateClass_In_Student_Year();
            }
        }
        //Class_In_Semester
        private string _Class_In_Semester;
        public string Class_In_Semester
        {
            get => _Class_In_Semester;
            set
            {
                _Class_In_Semester = value;
                OnPropertyChanged(nameof(Class_In_Semester));
                ValidateClass_In_Semester_Select();
            }
        }
        //Class_In_Generation
        private string _Class_In_Generation;
        public string Class_In_Generation
        {
            get => _Class_In_Generation;
            set
            {
                _Class_In_Generation = value;
                OnPropertyChanged(nameof(Class_In_Generation));
                ValidateClass_In_Generation();
            }
        }
        //Class_In_Study_Timeshift
        private string _Class_In_Study_Timeshift;
        public string Class_In_Study_Timeshift
        {
            get => _Class_In_Study_Timeshift;
            set
            {
                _Class_In_Study_Timeshift = value;
                OnPropertyChanged(nameof(Class_In_Study_Timeshift));
            }
        }
        //Class_In_Study_Type
        private string _Class_In_Study_Type;
        public string Class_In_Study_Type
        {
            get => _Class_In_Study_Type;
            set
            {
                _Class_In_Study_Type = value;
                OnPropertyChanged(nameof(Class_In_Study_Type));
            }
        }

        //Class_In_Skill_Select
        private Student_Info _Class_In_Skill_Select;
        public Student_Info Class_In_Skill_Select
        {
            get => _Class_In_Skill_Select;
            set
            {
                if (_Class_In_Skill_Select != value)
                {
                    _Class_In_Skill_Select = value;
                    OnPropertyChanged(nameof(Class_In_Skill_Select));
                    if(Class_In_Skill_Select == null)
                    {
                        Class_In_Skill = null;
                    }
                    else
                    {
                        Class_In_Skill = Class_In_Skill_Select.Stu_EducationSubjects;
                    }
                    ValidateClass_In_Skill_Select();
                }

            }
        }
        //Class_In_Level_Select
        private Student_Info _Class_In_Level_Select;
        public Student_Info Class_In_Level_Select
        {
            get { return _Class_In_Level_Select; }
            set
            {
                if (_Class_In_Level_Select != value)
                {
                    _Class_In_Level_Select = value;
                    OnPropertyChanged(nameof(Class_In_Level_Select));
                    if (_Class_In_Level_Select == null)
                    {
                        Class_In_Level = null;
                    }
                    else
                    {
                        Class_In_Level = _Class_In_Level_Select.Stu_EducationLevels;
                    }
                    ValidateClass_In_Level_Select();
                }

            }
        }
        //Class_In_Study_Year_Select
        private Student_Info _Class_In_Study_Year_Select;
        public Student_Info Class_In_Study_Year_Select
        {
            get { return _Class_In_Study_Year_Select; }
            set
            {
                _Class_In_Study_Year_Select = value;
                OnPropertyChanged(nameof(Class_In_Study_Year_Select));
                if(_Class_In_Study_Year_Select == null)
                {
                    Class_In_Study_Year = null;
                }
                else
                {
                    Class_In_Study_Year = _Class_In_Study_Year_Select.Stu_StudyYear;
                }
                ValidateClass_In_Study_Year_Select();
            }
        }
        //Class_In_Student_Year_Select
        private Student_Info _Class_In_Student_Year_Select;
        public Student_Info Class_In_Student_Year_Select
        {
            get { return _Class_In_Student_Year_Select; }
            set
            {
                _Class_In_Student_Year_Select = value;
                OnPropertyChanged(nameof(Class_In_Student_Year_Select));
                if(_Class_In_Student_Year_Select==null)
                {
                    Class_In_Student_Year= null;
                }
                else
                {
                    Class_In_Student_Year = Class_In_Student_Year_Select.ToString();
                }
                ValidateClass_In_Student_Year();
            }
        }

        //Class_In_Study_Timeshift_Select
        private Student_Info _Class_In_Study_Timeshift_Select;
        public Student_Info Class_In_Study_Timeshift_Select
        {
            get { return _Class_In_Study_Timeshift_Select; }
            set
            {
                _Class_In_Study_Timeshift_Select = value;
                OnPropertyChanged(nameof(Class_In_Study_Timeshift_Select));
                if(_Class_In_Study_Timeshift_Select==null)
                {
                    Class_In_Study_Timeshift = null;
                }
                else
                {
                    Class_In_Study_Timeshift = Class_In_Study_Timeshift_Select.Stu_StudyTimeShift;
                }
                ValidateClass_In_Study_Timeshift_Select();
            }
        }
        //Class_In_Study_Type_Select
        private Student_Info _Class_In_Study_Type_Select;
        public Student_Info Class_In_Study_Type_Select
        {
            get => _Class_In_Study_Type_Select;
            set
            {
                    _Class_In_Study_Type_Select = value;
                    OnPropertyChanged(nameof(Class_In_Study_Type_Select));

                    if(_Class_In_Study_Type_Select == null)
                    {
                        Class_In_Study_Type = null;
                    }
                    else
                    {
                        Class_In_Study_Type = Class_In_Study_Type_Select.Stu_EducationType;
                    }
                    ValidateClass_In_Study_Type_Select();
                
            }
        }

        //ValidateClass_In_Study_Type_Select
        public SolidColorBrush Class_In_Study_TypeBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Class_In_Study_TypeBorderBrush));
            }
        }
        private void ValidateClass_In_Study_Type_Select()
        {
            if (Class_In_Study_Type_Select == null)
            {
                Class_In_Study_TypeBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Class_In_Study_TypeBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateClass_In_Study_Timeshift_Select
        public SolidColorBrush Class_In_Study_TimeshiftBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Class_In_Study_TimeshiftBorderBrush));
            }
        }
        private void ValidateClass_In_Study_Timeshift_Select()
        {
            if (Class_In_Study_Timeshift_Select == null)
            {
                Class_In_Study_TimeshiftBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Class_In_Study_TimeshiftBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }

        //Class_In_Generation
        public SolidColorBrush Class_In_GenerationBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Class_In_GenerationBorderBrush));
            }
        }
        private void ValidateClass_In_Generation()
        {
            if (string.IsNullOrEmpty(Class_In_Generation))
            {
                Class_In_GenerationBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Class_In_GenerationBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Class_In_Study_Year_BorderBrush
        public SolidColorBrush Class_In_Study_Year_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Class_In_Study_Year_BorderBrush));
            }
        }
        private void ValidateClass_In_Study_Year_Select()
        {
            if (Class_In_Study_Year_Select == null)
            {
                Class_In_Study_Year_BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Class_In_Study_Year_BorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Class_In_Level_Select
        public SolidColorBrush Class_In_Level_Select_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Class_In_Level_Select_BorderBrush));
            }
        }
        private void ValidateClass_In_Level_Select()
        {
            if (Class_In_Level_Select == null)
            {
                Class_In_Level_Select_BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Class_In_Level_Select_BorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //Class_In_Skill_Select
        public SolidColorBrush Class_In_Skill_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Class_In_Skill_BorderBrush));
            }
        }
        private void ValidateClass_In_Skill_Select()
        {
            if (Class_In_Skill_Select == null)
            {
                Class_In_Skill_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Class_In_Skill_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }
        //ValidateClass_In_Student_Year_Select
        public SolidColorBrush Class_In_Student_Year_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Class_In_Student_Year_BorderBrush));
            }
        }
        private void ValidateClass_In_Student_Year()
        {
            if (string.IsNullOrEmpty(Class_In_Student_Year))
            {
                Class_In_Student_Year_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Class_In_Student_Year_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //VClass_Semester_BorderBrush
        public SolidColorBrush Class_Semester_BorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Class_Semester_BorderBrush));
            }
        }
        private void ValidateClass_In_Semester_Select()
        {
            if (string.IsNullOrEmpty(Class_In_Semester))
            {
                Class_Semester_BorderBrush = new SolidColorBrush(Colors.Red);  // Set red border on empty
            }
            else
            {
                Class_Semester_BorderBrush = new SolidColorBrush(Colors.Green); // Set green border on valid
            }
        }

        //Insert
        public void SaveClassInformationToDatabase()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            var UpdateClasses = Classes_Info.FirstOrDefault(s => s.Class_ID == Class_ID);
            if (UpdateClasses != null)
            {
                UpdateClasses.Class_ID = Class_ID;
                UpdateClasses.Class_Name = Class_Name;
                UpdateClasses.Class_In_Skill = Class_In_Skill;
                UpdateClasses.Class_In_Study_Year = Class_In_Study_Year;
                UpdateClasses.Class_In_Level = Class_In_Level;
                UpdateClasses.Class_In_Student_Year = Class_In_Student_Year;
                UpdateClasses.Class_In_Semester = Class_In_Semester;
                UpdateClasses.Class_In_Generation = Class_In_Generation;
                UpdateClasses.Class_In_Study_Timeshift = Class_In_Study_Timeshift;
                UpdateClasses.Class_In_Study_Type = Class_In_Study_Type;
                Debug.WriteLine("Update Mode.");

                bool success = dbConnection.Update_Classes_Information(UpdateClasses);

                if (success)
                {

                    ErrorMessage = "ថ្នាក់ឈ្មោះ " + Class_Name + " បានធ្វើបច្ចុប្បន្នភាពជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                }
                else
                {
                    ErrorMessage = "ថ្នាក់ឈ្មោះ " + Class_Name + " ធ្វើបច្ចុប្បន្នភាពបរាជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    
                }

            }
            else   
            {
                Student_Info classes_info = new Student_Info()
                {
                    No_Class = this.No_Class, //No Auto
                    Class_ID = this.Class_ID, //(Auto)
                    Class_Name = this.Class_Name,
                    Class_In_Skill = this.Class_In_Skill_Select.Stu_EducationSubjects,
                    Class_In_Study_Year = this.Class_In_Study_Year_Select.Stu_StudyYear,
                    Class_In_Level = this.Class_In_Level_Select.Stu_EducationLevels,
                    Class_In_Student_Year = this.Class_In_Student_Year,
                    Class_In_Semester = this.Class_In_Semester,
                    Class_In_Generation = this.Class_In_Generation,
                    Class_In_Study_Timeshift = this.Class_In_Study_Timeshift_Select.Stu_StudyTimeShift,
                    Class_In_Study_Type = this.Class_In_Study_Type_Select.Stu_EducationType
                };
                bool success = dbConnection.Insert_Class_Information(classes_info);

                if (success)
                {

                    ErrorMessage = "ថ្នាក់ឈ្មោះ " + Class_Name + " បានរក្សាទុកជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                }
                else
                {
                    ErrorMessage = "ថ្នាក់ឈ្មោះ " + Class_Name + " រក្សាទុកបរាជ៏យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
            }
            
        }

        public async Task SubmitAsync_Class()
        { 
           
            //ValidateClass_ID();
            ValidateClass_Name();
            ValidateClass_In_Skill_Select();
            ValidateClass_In_Study_Year_Select();
            ValidateClass_In_Student_Year();
            ValidateClass_In_Level_Select();
            ValidateClass_In_Semester_Select();
            ValidateClass_In_Generation();
            ValidateClass_In_Study_Timeshift_Select();
            ValidateClass_In_Study_Type_Select();

            //if (string.IsNullOrEmpty(Class_ID))
            //{
            //    ErrorMessage = "ID ថ្នាក់រៀន ត្រូវតែបំពេញ  !";
            //    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
            //    MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
            //    return;
            //}
            if (Class_In_Study_Year_Select == null)
            {
                ErrorMessage = "ឆ្នាំសិក្សា ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (Class_In_Skill_Select == null)
            {
                ErrorMessage = "ជំនាញសិក្សា ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            
            if(Class_In_Level_Select == null)
            {
                ErrorMessage = "កម្រិតសិក្សា ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (string.IsNullOrEmpty(Class_In_Student_Year))
            {
                ErrorMessage = "ឆ្នាំទី ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (string.IsNullOrEmpty(Class_In_Semester))
            {
                ErrorMessage = "ឆមាស ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (string.IsNullOrEmpty(Class_In_Generation))
            {
                ErrorMessage = "ជំនាន់ទី ត្រូវតែបំពេញ  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (Class_In_Study_Timeshift_Select == null)
            {
                ErrorMessage = "វេនសិក្សា ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (Class_In_Study_Type_Select == null)
            {
                ErrorMessage = "ប្រភេទសិក្សា ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (string.IsNullOrEmpty(Class_Name))
            {
                ErrorMessage = "ឈ្មោះថ្នាក់រៀន ត្រូវតែបំពេញ  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            // Clear any previous error message
            ErrorMessage = string.Empty;
            MessageColor = null;

            Debug.WriteLine($"No: {No_Class}");
            Debug.WriteLine($"ID: {Class_ID}");
            Debug.WriteLine($"Name: {Class_Name}");
            Debug.WriteLine($"Skill: {Class_In_Skill}");
            Debug.WriteLine($"Year: {Class_In_Study_Year}");
            Debug.WriteLine($"Level:{Class_In_Level}");
            Debug.WriteLine($"StudentYear: {Class_In_Student_Year}");
            Debug.WriteLine($"Semester: {Class_In_Semester}");
            Debug.WriteLine($"Generation: {Class_In_Generation}");
            Debug.WriteLine($"ShiftTime: {Class_In_Study_Timeshift}");
            Debug.WriteLine($"Study Type: {Class_In_Study_Type}");

            //Check Class Infomation Before Insert
            //var class_check_info = await _dbConnection.GetClasses_Check_Info(Class_Name, Class_In_Skill, Class_In_Study_Year, Class_In_Level, Class_In_Student_Year, Class_In_Semester, Class_In_Generation, Class_In_Study_Timeshift, Class_In_Study_Type);
            SaveClassInformationToDatabase();
            
            _ = LoadClasstoListViews(Search_Class_Search_Name_Generation);
            Clear_Class_Edit();

            // Remove the selected item from the collection
            if (Class_Info_Edit_Selected.Contains(SelectedClass_Edition))
            {
                Class_Info_Edit_Selected.Remove(SelectedClass_Edition);
            }
            OnPropertyChanged(nameof(Class_Info_Edit_Selected));
            // Clear the selection
            SelectedClass_Edition = null;
            Debug.WriteLine("Clear Class in ListView Success.");

            await Task.CompletedTask;
        }

        
        //Clear
        public async Task ClearAsync_Class()
        {
            //Clear Class Study Year
            Class_In_Study_Year_Select = EducationStudyYear_Combobox
                .FirstOrDefault(education_studyyear => education_studyyear.Stu_StudyYear == null);
            OnPropertyChanged(nameof(Class_In_Study_Year_Select));

            //Clear Class Subject
            Class_In_Skill_Select = EducationSubjectSkill_Combobox
                .FirstOrDefault(education_subject => education_subject.Stu_EducationSubjects == null);
            OnPropertyChanged(nameof(Class_In_Skill_Select));

            //Class Level
            Class_In_Level_Select = EducationsLevel_Combobox
                .FirstOrDefault(education_level => education_level.Stu_EducationLevels == null);
            OnPropertyChanged(nameof(Class_In_Level_Select));

            //Class Student Year
            Class_In_Student_Year = null;

            //Class Semester
            Class_In_Semester = null;

            //Class Generation
            Class_In_Generation = null;

            //Class TimeShift
            Class_In_Study_Timeshift_Select = EducationStudyTimeShift_Combobox
                .FirstOrDefault(education_timeshift => education_timeshift.Stu_StudyTimeShift == null);
            OnPropertyChanged(nameof(Class_In_Study_Timeshift_Select));

            //Class StudyType
            Class_In_Study_Type_Select = EducationStudyType_Combobox
                .FirstOrDefault(education_type => education_type.Stu_EducationType == null);
            OnPropertyChanged(nameof(Class_In_Study_Type_Select));

            //Class Name
            Class_Name = null;

            //Class ID
            Class_ID = null;

            ErrorMessage = "";
            ErrorImageSource = null;
            MessageColor = null;
            

            OnPropertyChanged(nameof(Class_ID));

            if(string.IsNullOrEmpty(Class_ID))
            {
                IsInsertEnabled = true;
                IsUpdateEnabled = false;
            }
            else
            {
                IsInsertEnabled = false;
                IsUpdateEnabled = true;
            } 

            await Task.CompletedTask;
        }

        //Load Class to ListView
        //Search_Class_Search_Name_Generation
        private string _Search_Class_Search_Name_Generation;
        public string Search_Class_Search_Name_Generation
        {
            get => _Search_Class_Search_Name_Generation;
            set
            {
                if (_Search_Class_Search_Name_Generation != value)
                {
                    _Search_Class_Search_Name_Generation = value;
                    OnPropertyChanged(nameof(Search_Class_Search_Name_Generation));
                    Debug.WriteLine("Property changed: Search_Class_Search_Name_Generation = " + value);
                    OnSearchTextChanged_ClassName_Generation(_Search_Class_Search_Name_Generation);
                }
            }
        }
        //Search By ID, Name
        private async void OnSearchTextChanged_ClassName_Generation(string newText_Name_Generation)
        {
            Debug.WriteLine($"Search Name_Generation Insert Mode: {newText_Name_Generation}");
            await LoadClasstoListViews(newText_Name_Generation);
        }

        //Load Class
        public async Task LoadClasstoListViews(string newText_Name_Generation)
        {
            IsLoading = true;
            try
            {
                await Task.Delay(10);

                //
                
                var classList = _dbConnection.GetClass_Info(CurrentPage, _classSize, newText_Name_Generation);
                // Clear the existing list to prepare for the new page data
                Classes_Info.Clear();
                Debug.WriteLine("Loading class for page: " + CurrentPage_Class);

                
                foreach (var class_info in classList)
                {
                    Classes_Info.Add(class_info);
                }

                Classes_Info = new ObservableCollection<Student_Info>(classList);

                // Raise CanExecuteChanged to update button states
                (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
            finally
            {
                // Hide the loading indicator
                IsLoading = false;
            }
            await Task.CompletedTask;

        }

        //Fetch Class Info
        public async Task FetchClassInfo(string newText_Name_Generation)
        {
            if (string.IsNullOrEmpty(newText_Name_Generation))
            {
                var classList = _dbConnection.GetClass_Info(CurrentPage_Class, _classSize, newText_Name_Generation);
                Classes_Info.Clear();
                foreach (var class_info in classList)
                {
                    Classes_Info.Add(class_info);
                }
                return;
            }

            IsLoading = true;
            try
            {
                await Task.Delay(10);

                //
                var classList = _dbConnection.GetClass_Info(CurrentPage_Class, _classSize, newText_Name_Generation);
                // Clear the existing list to prepare for the new page data
                Classes_Info.Clear();
                Debug.WriteLine("Loading classes for page: " + CurrentPage_Class);

                // Iterate over the studentsList returned by the database and add them to the ObservableCollection
                foreach (var class_info in classList)
                {
                    Classes_Info.Add(class_info);
                }

                Classes_Info = new ObservableCollection<Student_Info>(classList);

                // Raise CanExecuteChanged to update button states
                (NextPageCommand_Class as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand_Class as RelayCommand)?.RaiseCanExecuteChanged();
            }
            finally
            {
                // Hide the loading indicator
                IsLoading = false;
            }

            await Task.CompletedTask;
        }

        //Next Page Class
        private async void NextPage_Class()
        {
            Debug.WriteLine("Next Page Class Command Executed");
            if (CurrentPage_Class < TotalPage_Class)
            {
                CurrentPage_Class++;
                var searchTask = Search_Class_Information(Search_Class_In_Study_Year, Search_Class_In_Skill, Search_Class_In_Level, Search_Class_In_Student_Year, Search_Class_Semester, Search_Class_In_Study_Timeshift, Search_Class_In_Study_Type);
                var fetchTask = FetchClassInfo(Search_Class_Search_Name_Generation);
                
                await Task.WhenAll(fetchTask, searchTask);
                OnPageChanged();
                Debug.WriteLine($"Current Page Class Check: {CurrentPage_Class}");
            }
           
        }
        //Back Page Class
        private async void PreviousPage_Class()
        {
            if (CurrentPage_Class > 1)
            {
                CurrentPage_Class--;
                var fetchTask = FetchClassInfo(Search_Class_Search_Name_Generation);
                var searchTask = Search_Class_Information(Search_Class_In_Study_Year, Search_Class_In_Skill, Search_Class_In_Level, Search_Class_In_Student_Year, Search_Class_Semester, Search_Class_In_Study_Timeshift, Search_Class_In_Study_Type);

                await Task.WhenAll(fetchTask, searchTask);
                Debug.WriteLine($"Current Class Page: {CurrentPage_Class}");
            }
            OnPageChanged();

        }

        //Class NextPage and BackPage
        private bool CanGoPreviousPage_Class()
        {
            Debug.WriteLine($"CanGoPreviousPage_Class Evaluated: {CurrentPage_Class > 1}");
            return CurrentPage_Class > 1;
        }

        private bool CanGoNextPage_Class()
        {
            Debug.WriteLine($"CanGoNextPage_Class Evaluated: {CurrentPage_Class < TotalPage_Class}");
            return CurrentPage_Class < TotalPage_Class;
        }

        //Search by button click
        private string _search_class_in_skill;
        public string Search_Class_In_Skill
        {
            get => _search_class_in_skill;
            set
            {
                _search_class_in_skill = value;
                OnPropertyChanged(nameof(Search_Class_In_Skill));
            }
        }
        private Student_Info _Search_Class_In_Skill_Select;
        public Student_Info Search_Class_In_Skill_Select
        {
            get { return _Search_Class_In_Skill_Select; }
            set
            {
                if(_Search_Class_In_Skill_Select  != value)
                {
                    _Search_Class_In_Skill_Select = value;
                    OnPropertyChanged(nameof(Search_Class_In_Skill_Select));
                    if(Search_Class_In_Skill_Select == null)
                    {
                        Search_Class_In_Skill = null;
                    }
                    else
                    {
                        Search_Class_In_Skill = Search_Class_In_Skill_Select.Stu_EducationSubjects;
                    }
                }
                Debug.WriteLine($"Search_Class_In_Skill: {Search_Class_In_Skill}");
            }
        }
        private string _search_class_in_level;
        public string Search_Class_In_Level
        {
            get => _search_class_in_level;
            set
            {
                _search_class_in_level = value;
                OnPropertyChanged(nameof(Search_Class_In_Level));
            }
        }
        private Student_Info _Search_Class_In_Level_Select;
        public Student_Info Search_Class_In_Level_Select
        {
            get { return _Search_Class_In_Level_Select; }
            set
            {
                if(_Search_Class_In_Level_Select != value)
                {
                    _Search_Class_In_Level_Select = value;
                    OnPropertyChanged(nameof(Search_Class_In_Level_Select));
                    if(Search_Class_In_Level_Select==null)
                    {
                        Search_Class_In_Level = null;
                    }
                    else
                    {
                        Search_Class_In_Level = Search_Class_In_Level_Select.Stu_EducationLevels;
                    }
                }
                Debug.WriteLine($"Search_Class_In_Level: {Search_Class_In_Level}");
            }
        }
        private string _search_class_in_study_year;
        public string Search_Class_In_Study_Year
        {
            get => _search_class_in_study_year;
            set
            {
                _search_class_in_study_year = value;
                OnPropertyChanged(nameof(Search_Class_In_Study_Year));
            }
        }
        private Student_Info _Search_Class_In_Study_Year_Select;
        public Student_Info Search_Class_In_Study_Year_Select
        {
            get { return _Search_Class_In_Study_Year_Select; }
            set
            {
                if (_Search_Class_In_Study_Year_Select != value)
                {
                    _Search_Class_In_Study_Year_Select = value;
                    OnPropertyChanged(nameof(Search_Class_In_Study_Year_Select));

                    if (Search_Class_In_Study_Year_Select == null)
                    {
                        Search_Class_In_Study_Year = null;
                    }
                    else
                    {
                        Search_Class_In_Study_Year = Search_Class_In_Study_Year_Select.Stu_StudyYear;
                    }
                }
                Debug.WriteLine($"Search_Class_In_Study_Year: {Search_Class_In_Study_Year}");
            }
        }
        private string _search_class_in_student_year;
        public string Search_Class_In_Student_Year
        {
            get => _search_class_in_student_year;
            set
            {
                _search_class_in_student_year = value;
                OnPropertyChanged(nameof(Search_Class_In_Student_Year));
            }
        }
        private string _search_class_semester;
        public string Search_Class_Semester
        {
            get => _search_class_semester;
            set
            {
                _search_class_semester = value;
                OnPropertyChanged(nameof(Search_Class_Semester));
            }
        }
        private string _search_class_in_study_timeshift;
        public string Search_Class_In_Study_Timeshift
        {
            get => _search_class_in_study_timeshift;
            set
            {
                _search_class_in_study_timeshift = value;
                OnPropertyChanged(nameof(Search_Class_In_Study_Timeshift));
            }
        }
        private Student_Info _Search_Class_In_Study_Timeshift_Select;
        public Student_Info Search_Class_In_Study_Timeshift_Select
        {
            get { return _Search_Class_In_Study_Timeshift_Select; }
            set
            {
                if (_Search_Class_In_Study_Timeshift_Select != value)
                {
                    _Search_Class_In_Study_Timeshift_Select = value;
                    OnPropertyChanged(nameof(Search_Class_In_Study_Timeshift_Select));
                    if(Search_Class_In_Study_Timeshift_Select==null)
                    {
                        Search_Class_In_Study_Timeshift = null;
                    }
                    else
                    {
                        Search_Class_In_Study_Timeshift = Search_Class_In_Study_Timeshift_Select.Stu_StudyTimeShift;
                    }
                }
                Debug.WriteLine($"Search_Class_In_Study_Timeshift: {Search_Class_In_Study_Timeshift}");
            }
        }
        private string _search_class_in_study_type;
        public string Search_Class_In_Study_Type
        {
            get => _search_class_in_study_type;
            set
            {
                _search_class_in_study_type = value;
                OnPropertyChanged(nameof(Search_Class_In_Study_Type));
            }
        }
        private Student_Info _Search_Class_In_Study_Type_Select;
        public Student_Info Search_Class_In_Student_Type_Select
        {
            get { return _Search_Class_In_Study_Type_Select; }
            set
            {
                if(_Search_Class_In_Study_Type_Select != value)
                {
                    _Search_Class_In_Study_Type_Select = value;
                    OnPropertyChanged(nameof(Search_Class_In_Student_Type_Select));

                    if(Search_Class_In_Student_Type_Select == null)
                    {
                        Search_Class_In_Study_Type = null;
                    }
                    else
                    {
                        Search_Class_In_Study_Type = Search_Class_In_Student_Type_Select.Stu_EducationType;
                    }
                    
                }
                Debug.WriteLine($"Search_Class_In_Study_Type: {Search_Class_In_Study_Type}");
            }
        }

        //Search 
        public async Task Search_Class_Information(string Search_Class_In_Study_Year, string Search_Class_In_Skill, string Search_Class_In_Level, string Search_Class_In_Student_Year, string Search_Class_Semester, string Search_Class_In_Study_Timeshift, string Search_Class_In_Study_Type)
        {
            Debug.WriteLine($"Search_Class_In_Study_Year: {Search_Class_In_Study_Year}");
            Debug.WriteLine($"Search_Class_In_Skill: {Search_Class_In_Skill}");
            Debug.WriteLine($"Search_Class_In_Level: {Search_Class_In_Level}");
            Debug.WriteLine($"Search_Class_In_Student_Year: {Search_Class_In_Student_Year}");
            Debug.WriteLine($"Search_Class_Semester: {Search_Class_Semester}");
            Debug.WriteLine($"Search_Class_In_Study_Timeshift: {Search_Class_In_Study_Timeshift}");
            Debug.WriteLine($"Search_Class_In_Study_Type: {Search_Class_In_Study_Type}");

            if (string.IsNullOrEmpty(Search_Class_In_Study_Year) && string.IsNullOrEmpty(Search_Class_In_Skill) && string.IsNullOrEmpty(Search_Class_In_Level) && string.IsNullOrEmpty(Search_Class_In_Student_Year) && string.IsNullOrEmpty(Search_Class_Semester) && string.IsNullOrEmpty(Search_Class_In_Study_Timeshift) && string.IsNullOrEmpty(Search_Class_In_Study_Type))
            {
                var classesList = _dbConnection.GetClasses_Check_Info_by_Combobox(CurrentPage_Class, _classSize, Search_Class_In_Study_Year, Search_Class_In_Skill, Search_Class_In_Level, Search_Class_In_Student_Year, Search_Class_Semester, Search_Class_In_Study_Timeshift, Search_Class_In_Study_Type);
                Classes_Info.Clear();
                foreach (var class_info in classesList)
                {
                    Classes_Info.Add(class_info);
                }
                return;
            }

            IsLoading = true;
            try
            {
                await Task.Delay(10);

                var classesList = _dbConnection.GetClasses_Check_Info_by_Combobox(CurrentPage_Class, _classSize, Search_Class_In_Study_Year, Search_Class_In_Skill, Search_Class_In_Level, Search_Class_In_Student_Year, Search_Class_Semester, Search_Class_In_Study_Timeshift, Search_Class_In_Study_Type);
                Debug.WriteLine("Loading class for page: " + CurrentPage_Class);
                Classes_Info.Clear();
                foreach (var class_info in classesList)
                {
                    Classes_Info.Add(class_info);
                }

                Classes_Info = new ObservableCollection<Student_Info>(classesList);

                // Raise CanExecuteChanged to update button states
                (NextPageCommand_Check as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand_Check as RelayCommand)?.RaiseCanExecuteChanged();
            }
            finally
            {
                // Hide the loading indicator
                IsLoading = false;
            }

            await Task.CompletedTask;
        }

        //Multi Selection Class Total
        private List<Student_Info> _selectedClasses_Edit_Delete = new List<Student_Info>();
        public List<Student_Info> SelectedClasses_Edit_Delete
        {
            get => _selectedClasses_Edit_Delete;
            set
            {
                _selectedClasses_Edit_Delete = value;
                OnPropertyChanged(nameof(SelectedClasses_Edit_Delete));
            }
        }
        //Multi Select Class Prepare
        private List<Student_Info> _selectedClasses_Prepare_All = new List<Student_Info>();
        public List<Student_Info> SelectedClasses_Prepare_All
        {
            get => _selectedClasses_Prepare_All;
            set
            {
                _selectedClasses_Prepare_All = value;
                OnPropertyChanged(nameof(SelectedClasses_Prepare_All));
            }
        }
        //First Selection Insert Class
        private Student_Info _firstSelectedClass;
        public Student_Info FirstSelectedClass
        {
            get => _firstSelectedClass;
            set
            {
                _firstSelectedClass = value;
                OnPropertyChanged(nameof(FirstSelectedClass));
            }
        }
        //First Selection Prepare Class
        private Student_Info _firstSelectedClasss_Prepare;
        public Student_Info FirstSelectedClasss_Preparing
        {
            get => _firstSelectedClasss_Prepare;
            set
            {
                _firstSelectedClasss_Prepare = value;
                OnPropertyChanged(nameof(FirstSelectedClasss_Preparing));
            }
        }

        //Command Edit
        public ICommand Command_Edit_Class { get; set; }
        public ICommand Command_Delete_Class { get; set; }

        //Method Edit Class
        public async Task Edit_Class()
        {
            if(_firstSelectedClass == null)
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            Debug.WriteLine($"First Selection: {FirstSelectedClass.Class_Name}");

            //Class Study Year
            Class_In_Study_Year_Select = EducationStudyYear_Combobox
                .FirstOrDefault(education_studyyear => education_studyyear.Stu_StudyYear == FirstSelectedClass.Class_In_Study_Year);
            OnPropertyChanged(nameof(Class_In_Study_Year_Select));

            //Class Subject
            Class_In_Skill_Select = EducationSubjectSkill_Combobox
                .FirstOrDefault(education_subject => education_subject.Stu_EducationSubjects == FirstSelectedClass.Class_In_Skill);
            OnPropertyChanged(nameof(Class_In_Skill_Select));

            //Class Level
            Class_In_Level_Select = EducationsLevel_Combobox
                .FirstOrDefault(education_level => education_level.Stu_EducationLevels == FirstSelectedClass.Class_In_Level);
            OnPropertyChanged(nameof(Class_In_Level_Select));

            //Class Student Year
            Class_In_Student_Year = FirstSelectedClass.Class_In_Student_Year;

            //Class Semester
            Class_In_Semester = FirstSelectedClass.Class_In_Semester;

            //Class Generation
            Class_In_Generation = FirstSelectedClass.Class_In_Generation;

            //Class TimeShift
            Class_In_Study_Timeshift_Select = EducationStudyTimeShift_Combobox
                .FirstOrDefault(education_timeshift => education_timeshift.Stu_StudyTimeShift == FirstSelectedClass.Class_In_Study_Timeshift);
            OnPropertyChanged(nameof(Class_In_Study_Timeshift_Select));

            //Class StudyType
            Class_In_Study_Type_Select = EducationStudyType_Combobox
                .FirstOrDefault(education_type => education_type.Stu_EducationType == FirstSelectedClass.Class_In_Study_Type);
            OnPropertyChanged(nameof(Class_In_Study_Type_Select));

            //Class Name
            Class_Name = FirstSelectedClass.Class_Name;

            //Class ID
            Class_ID = FirstSelectedClass.Class_ID;

            ErrorMessage = "";
            ErrorImageSource = null;
            MessageColor = new SolidColorBrush(Colors.Transparent);

            OnPropertyChanged(nameof(FirstSelectedClass));
            if (_firstSelectedClass != null)
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

        //Method Delete Class
        public async Task Delete_Class()
        {
            if(SelectedClasses_Edit_Delete==null || !SelectedClasses_Edit_Delete.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            foreach (var classes in SelectedClasses_Edit_Delete)
            {
                var classIds = SelectedClasses_Edit_Delete.Select(c => c.Class_ID).ToList();
               _dbConnection.Delete_Class_Info(classIds);

                Debug.WriteLine($"Deleted Class Name: {classes.Class_Name}");
            }
            _ = LoadClasstoListViews(Search_Class_Search_Name_Generation);
            ErrorMessage = "ថ្នាក់ឈ្មោះ " + Class_Name + " បានលុបចេញជោគជ័យ !";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);

            await Task.CompletedTask;
        }

        //Command Prepare Class
        public ICommand Command_Edit_Class_Prepare { get; set; }
        public ICommand Clear_Class_Update { get; set; }
        public ICommand Command_Add_Student_to_Class { get; set; }

        //Add Student
        public async Task Edit_Class_Prepare_Student()
        {
            if (SelectedClasses_Prepare_All == null || !SelectedClasses_Prepare_All.Any())
            {
                Debug.WriteLine("No Selection");
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                Debug.WriteLine("Selected");
                Class_Info_Add_Student_Selected.Clear();
                foreach (var classes_edit in SelectedClasses_Prepare_All)
                {

                    Class_Info_Add_Student_Selected.Add(new Student_Info
                    {
                        No_Class = classes_edit.No_Class,
                        Class_ID = classes_edit.Class_ID,
                        Class_Name = classes_edit.Class_Name,
                        Class_In_Study_Year = classes_edit.Class_In_Study_Year,
                        Class_In_Skill = classes_edit.Class_In_Skill,
                        Class_In_Level = classes_edit.Class_In_Level,
                        Class_In_Student_Year = classes_edit.Class_In_Student_Year,
                        Class_In_Semester = classes_edit.Class_In_Semester,
                        Class_In_Generation = classes_edit.Class_In_Generation,
                        Class_In_Study_Timeshift = classes_edit.Class_In_Study_Timeshift,
                        Class_In_Study_Type = classes_edit.Class_In_Study_Type,
                        Max_Student_InClass = classes_edit.Max_Student_InClass,
                        Current_Student_InClass = classes_edit.Current_Student_InClass,
                        Current_Class_State = classes_edit.Current_Class_State
                    });
                }
            }

            await Task.CompletedTask;
        }
        //Update Add
        public async Task Edit_Class_Prepare()
        {
            if(SelectedClasses_Prepare_All==null || !SelectedClasses_Prepare_All.Any())
            {
                Debug.WriteLine("No Selection");
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                Debug.WriteLine("Selected");
                Class_Info_Edit_Selected.Clear();
                foreach (var classes_edit in SelectedClasses_Prepare_All)
                {
                   
                    Class_Info_Edit_Selected.Add(new Student_Info
                    {
                        No_Class = classes_edit.No_Class,
                        Class_ID = classes_edit.Class_ID,
                        Class_Name = classes_edit.Class_Name,
                        Class_In_Study_Year = classes_edit.Class_In_Study_Year,
                        Class_In_Skill = classes_edit.Class_In_Skill,
                        Class_In_Level = classes_edit.Class_In_Level,
                        Class_In_Student_Year = classes_edit.Class_In_Student_Year,
                        Class_In_Semester = classes_edit.Class_In_Semester,
                        Class_In_Generation = classes_edit.Class_In_Generation,
                        Class_In_Study_Timeshift = classes_edit.Class_In_Study_Timeshift,
                        Class_In_Study_Type = classes_edit.Class_In_Study_Type
                    });
                }
            }
            
            await Task.CompletedTask;
        }

        

        private Student_Info _selectedClass_Edit;
        public Student_Info SelectedClass_Edition
        {
            get => _selectedClass_Edit;
            set
            {
               _selectedClass_Edit = value;
                OnPropertyChanged();

                if (_selectedClass_Edit != null)
                {
                    //Update_Study_Year_Edit
                    Class_In_Study_Year_Select = EducationStudyYear_Combobox
                        .FirstOrDefault(educatioin_studyyear_edit => educatioin_studyyear_edit.Stu_StudyYear == _selectedClass_Edit.Class_In_Study_Year);
                    OnPropertyChanged(nameof(Class_In_Study_Year_Select));

                    //Update_Subject_Edit
                    Class_In_Skill_Select = EducationSubjectSkill_Combobox
                        .FirstOrDefault(education_subject_edit => education_subject_edit.Stu_EducationSubjects == _selectedClass_Edit.Class_In_Skill);
                    OnPropertyChanged(nameof(Class_In_Skill_Select));

                    //Update_Level_Edit
                    Class_In_Level_Select = EducationsLevel_Combobox
                        .FirstOrDefault(education_level_edit => education_level_edit.Stu_EducationLevels == _selectedClass_Edit.Class_In_Level);
                    OnPropertyChanged(nameof(Class_In_Level_Select));

                    //Update_Student_Year_Edit
                    Class_In_Student_Year = _selectedClass_Edit.Class_In_Student_Year;
                    OnPropertyChanged(Class_In_Student_Year);

                    //Updare_Semester_Edit
                    Class_In_Semester = _selectedClass_Edit.Class_In_Semester;
                    OnPropertyChanged(Class_In_Semester);

                    //Update_Generation_Edit
                    Class_In_Generation = _selectedClass_Edit.Class_In_Generation;
                    OnPropertyChanged(Class_In_Generation);

                    //Update_StudyTime_Edit
                    Class_In_Study_Timeshift_Select = EducationStudyTimeShift_Combobox
                        .FirstOrDefault(education_studyshift_edit => education_studyshift_edit.Stu_StudyTimeShift == _selectedClass_Edit.Class_In_Study_Timeshift);
                    OnPropertyChanged(nameof(Class_In_Study_Timeshift_Select));

                    //Update_TypeStudy_Edit
                    Class_In_Study_Type_Select = EducationStudyType_Combobox
                        .FirstOrDefault(education_study_type_edit => education_study_type_edit.Stu_EducationType == _selectedClass_Edit.Class_In_Study_Type);
                    OnPropertyChanged(nameof(Class_In_Study_Type_Select));

                    //Update_Class_Name_Edit
                    Class_Name = _selectedClass_Edit.Class_Name;

                    //Update_Class_ID_Edit
                    Class_ID = _selectedClass_Edit.Class_ID;

                    ErrorMessage = "";
                    ErrorImageSource = null;
                    MessageColor = null;
                }
                OnPropertyChanged(nameof(SelectedClass_Edition));
                if (_selectedClass_Edit != null)
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

        private void Clear_Class_Edit()
        {
            
            //Clear Class Study Year
            Class_In_Study_Year_Select = EducationStudyYear_Combobox
                .FirstOrDefault(education_studyyear => education_studyyear.Stu_StudyYear == null);
            OnPropertyChanged(nameof(Class_In_Study_Year_Select));

            //Clear Class Subject
            Class_In_Skill_Select = EducationSubjectSkill_Combobox
                .FirstOrDefault(education_subject => education_subject.Stu_EducationSubjects == null);
            OnPropertyChanged(nameof(Class_In_Skill_Select));

            //Class Level
            Class_In_Level_Select = EducationsLevel_Combobox
                .FirstOrDefault(education_level => education_level.Stu_EducationLevels == null);
            OnPropertyChanged(nameof(Class_In_Level_Select));

            //Class Student Year
            Class_In_Student_Year = null;

            //Class Semester
            Class_In_Semester = null;

            //Class Generation
            Class_In_Generation = null;

            //Class TimeShift
            Class_In_Study_Timeshift_Select = EducationStudyTimeShift_Combobox
                .FirstOrDefault(education_timeshift => education_timeshift.Stu_StudyTimeShift == null);
            OnPropertyChanged(nameof(Class_In_Study_Timeshift_Select));

            //Class StudyType
            Class_In_Study_Type_Select = EducationStudyType_Combobox
                .FirstOrDefault(education_type => education_type.Stu_EducationType == null);
            OnPropertyChanged(nameof(Class_In_Study_Type_Select));

            //Class Name
            Class_Name = null;

            //Class ID
            Class_ID = null;

            OnPropertyChanged(nameof(Class_ID));

            if (string.IsNullOrEmpty(Class_ID))
            {
                IsInsertEnabled = true;
                IsUpdateEnabled = false;
            }
            else
            {
                IsInsertEnabled = false;
                IsUpdateEnabled = true;
            }
        }


        //Clear Add Class
        public async Task Clear_Class_In_Add_Student()
        {
            if(SelectedClass_Add_Student == null)
            {
                Debug.WriteLine("No Selection");
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            // Remove the selected item from the collection
            if (Class_Info_Add_Student_Selected.Contains(SelectedClass_Add_Student))
            {
                Class_Info_Add_Student_Selected.Remove(SelectedClass_Add_Student);
                Class_ID = null;
                Class_In_Study_Year = null;
                Class_In_Skill = null;
                Class_In_Level = null;
                Class_In_Student_Year = null;
                Class_In_Study_Timeshift = null;
                Class_Name = null;
                Total_Count_Students_Class = null;
                Total_Count_Female_Class = null;
                Max_Student_InClass = 0;
                Current_Student_InClass = 0;
                Current_Class_State = null;
            }
            OnPropertyChanged(nameof(SelectedClass_Add_Student));
            // Clear the selection
            SelectedClass_Add_Student = null;
            List_Students_Display.Clear();
            List_Student_In_Class_Display.Clear();

            Debug.WriteLine("Clear Class in ListView Success.");

            // Provide feedback to the user
            ErrorMessage = "ថ្នាក់ជ្រើសរើសត្រូវបានដកចេញជោគជ័យ!";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);

            await Task.CompletedTask;
        }
        //Clear
        public async Task Clear_Class_UpdateAsync()
        {
            if(SelectedClass_Edition==null)
            {
                Debug.WriteLine("No Selection");
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            // Remove the selected item from the collection
            if (Class_Info_Edit_Selected.Contains(SelectedClass_Edition))
            {
               Class_Info_Edit_Selected.Remove(SelectedClass_Edition);
            }
            OnPropertyChanged(nameof(Class_Info_Edit_Selected));
            // Clear the selection
            SelectedClass_Edition = null;  
            Debug.WriteLine("Clear Class in ListView Success.");

            // Provide feedback to the user
            ErrorMessage = "ថ្នាក់ជ្រើសរើសត្រូវបានដកចេញជោគជ័យ!";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
            await Task.CompletedTask;
        }


        //Add Student into Class
        private Student_Info _selectedClass_Add_Student;
        public Student_Info SelectedClass_Add_Student
        {
            get => _selectedClass_Add_Student;
            set
            {
                _selectedClass_Add_Student = value;
                OnPropertyChanged();

                if (_selectedClass_Add_Student != null)
                {
                    Class_ID = _selectedClass_Add_Student.Class_ID;
                    Class_Name = _selectedClass_Add_Student.Class_Name;
                    Class_In_Study_Year = _selectedClass_Add_Student.Class_In_Study_Year;
                    Class_In_Skill = _selectedClass_Add_Student.Class_In_Skill;
                    Class_In_Level = _selectedClass_Add_Student.Class_In_Level;
                    Class_In_Student_Year = _selectedClass_Add_Student.Class_In_Student_Year;
                    Class_In_Semester = _selectedClass_Add_Student.Class_In_Semester;
                    Class_In_Generation = _selectedClass_Add_Student.Class_In_Generation;
                    Class_In_Study_Timeshift = _selectedClass_Add_Student.Class_In_Study_Timeshift;
                    Class_In_Study_Type = _selectedClass_Add_Student.Class_In_Study_Type;
                    Max_Student_InClass = _selectedClass_Add_Student.Max_Student_InClass;
                    Current_Class_State = _selectedClass_Add_Student.Current_Class_State;

                    Debug.WriteLine($"Class ID: {Class_ID}");
                    _=Count_Student_Selected_Class();
                }
            }
        }
        public ICommand Clear_Class_In_Add_Student_Into_Class { get; set; }

        //Total_Count_Students
        private string _Total_Count_Students_Class;
        public string Total_Count_Students_Class
        {
            get => _Total_Count_Students_Class;
            set
            {
                _Total_Count_Students_Class = value;
                OnPropertyChanged(nameof(Total_Count_Students_Class));
            }
        }
        //Total_Count_Female
        private string _Total_Count_Female_Class;
        public string Total_Count_Female_Class
        {
            get => _Total_Count_Female_Class;
            set
            {
                _Total_Count_Female_Class = value;
                OnPropertyChanged(nameof(Total_Count_Female_Class));
            }
        }

        //Select Student Count
        public async Task Count_Student_Selected_Class()
        {
            //Get count student when selected classes.
            _studentModel = new DatabaseConnection();
            var (student_total_count, student_female_count) = _studentModel.Get_Count_Total_and_Female_Students_Classes(Class_In_Study_Year, Class_In_Level, Class_In_Skill, Class_In_Student_Year, Class_In_Study_Timeshift);
            Total_Count_Students_Class = student_total_count;
            Total_Count_Female_Class = student_female_count;
            Debug.WriteLine($"Count Student: {Total_Count_Students_Class}.Female: {Total_Count_Female_Class}");
            await Task.CompletedTask;
        }

        //Command Show Students
        public ICommand Command_Show_Student_In_Class { get; set; }

        //Setup Total Student
        private int _Max_Student_In_Class;
        public int Max_Student_InClass
        {
            get => _Max_Student_In_Class;
            set
            {
                if(_Max_Student_In_Class != value)
                {
                    _Max_Student_In_Class = value;
                    OnPropertyChanged(nameof(Max_Student_InClass));
                }
            }
        }
        //Current Student in class
        private int _Current_Student_InClass;
        public int Current_Student_InClass
        {
            get => _Current_Student_InClass;
            set
            {
                if(_Current_Student_InClass != value)
                {
                    _Current_Student_InClass = value;
                    OnPropertyChanged(nameof(Current_Student_InClass));
                }               
            }
        }
        //State Class
        private string _Current_Class_State;
        public string Current_Class_State
        {
            get => _Current_Class_State;
            set
            {
                _Current_Class_State = value;
                OnPropertyChanged(nameof(Current_Class_State));

            }
        }

        //Methods to Select Students in class
        public async Task Get_Student_to_ListClassPrepare()
        {
            var viewModel = new StudentViewModel();

            if (string.IsNullOrEmpty(Class_Name) )
            {
                Debug.WriteLine("No Class Selection");
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if(Max_Student_InClass==0)
            {
                Debug.WriteLine("No Enter total Student.");
                ErrorMessage = "សូមបញ្ចូលចំនួននិស្សិតសរុបក្នុងថ្នាក់ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if(Max_Student_InClass < 1 || Max_Student_InClass >= 50)
            {
                Debug.WriteLine("Total Student should smaller than 50.");
                ErrorMessage = "ចំនួននិស្សិតសរុបក្នុងថ្នាក់ត្រូវតែតិចជាង 50 នាក់ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                IsLoading = true;
                try
                {
                    await Task.Delay(10);

                    //
                    var classList_Displays = _dbConnection.Display_Student_List_in_Class(Max_Student_InClass, Class_In_Study_Year, Class_In_Level, Class_In_Skill, Class_In_Student_Year, Class_In_Study_Timeshift);
                    var studentList_Displays = _dbConnection.Display_Student_List_in_Class2(Max_Student_InClass, Class_In_Study_Year, Class_In_Level, Class_In_Skill, Class_In_Student_Year, Class_In_Study_Timeshift);
                    // Clear the existing list to prepare for the new page data
                    List_Students_Display.Clear();
                    List_Student_In_Class_Display.Clear();
                    Debug.WriteLine("Loading student.");

                    // Iterate over the studentsList returned by the database and add them to the ObservableCollection                  
                    foreach (var student in classList_Displays)
                    {
                        List_Students_Display.Add(student);
                        student.Full_Name_KH = student.Stu_FirstName_KH +" "+ student.Stu_LastName_KH;                               
                       
                    }

                    List_Students_Display = new ObservableCollection<Student_Info>(classList_Displays);
                
                    foreach (var student_in_class in studentList_Displays)
                    {
                        List_Student_In_Class_Display.Add(student_in_class);
                        student_in_class.Full_Name_KH = student_in_class.Stu_FirstName_KH + " " + student_in_class.Stu_LastName_KH;
                    }

                    List_Student_In_Class_Display = new ObservableCollection<Student_Info>(studentList_Displays);

                    int total_stu = viewModel.GetTotalStudents(Class_ID);
                    Current_Student_InClass = total_stu;
                    
                   
                }
                finally
                {
                    // Hide the loading indicator
                    IsLoading = false;
                }

                Debug.WriteLine($"Total Students: {Max_Student_InClass}");
                Debug.WriteLine("Click Show Student in Class.");
                OnPropertyChanged(nameof(Max_Student_InClass));
            }
            
            await Task.CompletedTask;
        }

        //Add Students to class
        private List<Student_Info> _selected_students_Add_to_Class = new List<Student_Info>();
        public List<Student_Info> Selected_Students_to_Class
        {
            get => _selected_students_Add_to_Class;
            set
            {
                _selected_students_Add_to_Class = value;
                OnPropertyChanged(nameof(Selected_Students_to_Class));
            }
        }
        //Delete Student in class
        private List<Student_Info> _selected_students_in_Class = new List<Student_Info>();
        public List<Student_Info> Selected_Students_in_Class
        {
            get => _selected_students_in_Class;
            set
            {
                _selected_students_in_Class = value;
                OnPropertyChanged(nameof(Selected_Students_in_Class));
            }
        }

        //Command Insert Student to Class
        public ICommand Command_Insert_Students_to_Class {  get; set; }

        public int GetTotalStudents(string class_id)
        {
            // Call the method from the Database class
            int total_stu = _dbConnection.GetTotalStudentsInClass(class_id);

            if (total_stu >= 0)
            {
                Debug.WriteLine($"Total students in class {class_id}: {total_stu}");
            }
            else
            {
                Debug.WriteLine("An error occurred while retrieving the total student count.");
            }

            return total_stu; // Return the value for further use
        }

        

        //Method Insert students to class
        public async Task Insert_Students_to_Class()
        {
            var viewModel = new StudentViewModel();
            int Total_Current_Student = viewModel.GetTotalStudents(Class_ID);

            if (Selected_Students_to_Class == null || !Selected_Students_to_Class.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសសិស្សនិស្សិតជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(Class_ID))
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Max_Student_InClass < 0 || Max_Student_InClass >= 50)
            {
                Debug.WriteLine("Total Student should smaller than 50.");
                ErrorMessage = "ចំនួននិស្សិតសរុបក្នុងថ្នាក់ត្រូវតែតិចជាង 50 នាក់ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if(Max_Student_InClass < Total_Current_Student)
            {
                Debug.WriteLine("Total Student should bigger than Before value.");
                ErrorMessage = "ចំនួនកំណត់និស្សិតសរុបក្នុងថ្នាក់តិចជាងចំនួននិស្សិតជាក់ស្ដែង !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {               
                int total_stu = 0; 
                int student_id_count = 0;
                var class_id = Class_ID;
                foreach (var student_class in Selected_Students_to_Class)
                {
                    
                    //var class_id = Class_ID;
                    var max_student_class = Max_Student_InClass;
                    var student_id = new List<int> { student_class.ID };
                    
                    _dbConnection.Insert_Students_to_Class(student_id, class_id, max_student_class);
                    _dbConnection.GetTotalStudentsInClass(class_id);

                   
                    Debug.WriteLine($"Selected Student ID: {student_class.ID}");
                    Debug.WriteLine($"Selected Class ID: {class_id}");

                    student_id_count++;
                }

                
                total_stu = viewModel.GetTotalStudents(class_id);
                Current_Student_InClass = total_stu;
                
                int student_select_count = Current_Student_InClass;

                _dbConnection.UpdateStudentSelectCount(class_id, student_select_count);
                await Task.CompletedTask;
            }
            _ = LoadClasstoListViews(Search_Class_Search_Name_Generation);
            _ = Get_Student_to_ListClassPrepare();

            ErrorMessage = "និស្សិតត្រូវបានបញ្ចូលទៅក្នុងថ្នាក់បានជោគជ័យ !";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Command Delete Students in Class
        public ICommand Command_Delete_Student_in_Class { get; set; }

        //Method for delete student in class
        public async Task Delete_Student_in_Class()
        {
            var viewModel = new StudentViewModel();

            if (Selected_Students_in_Class == null || !Selected_Students_in_Class.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសសិស្សនិស្សិតជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(Class_ID))
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                int total_stu = 0;
                int student_select_count = 0;
                var class_id = Class_ID;

                foreach (var student_in_class in Selected_Students_in_Class)
                {
                    student_select_count++;
                                      
                    var student_id = new List<int> { student_in_class.ID };


                    Debug.WriteLine($"Selected Student in Class ID: {student_in_class.ID}");
                    Debug.WriteLine($"Selected Class ID: {class_id}");
                    

                    var result = _dbConnection.Delete_Students_in_Class(student_id, class_id);
                    

                    if (result)
                    {
                        total_stu = viewModel.GetTotalStudents(class_id);
                        Current_Student_InClass = total_stu;

                        _ = LoadClasstoListViews(Search_Class_Search_Name_Generation);
                        _ = Get_Student_to_ListClassPrepare();

                        Debug.WriteLine("All students were successfully removed from the class.");
                    }
                    else
                    {
                        Debug.WriteLine("Failed to remove one or more students from the class.");

                        //Error Message
                        ErrorMessage = "បរាជ័យ  !";
                        ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                        MessageColor = new SolidColorBrush(Colors.Red);
                        return;
                    }                 
                }

                total_stu = viewModel.GetTotalStudents(class_id);
                Current_Student_InClass = total_stu;

                int total_student_select_count = Current_Student_InClass;

                _dbConnection.UpdateStudentSelectCount(class_id, total_student_select_count);

                Debug.WriteLine($"Current Student Selected Count: {total_student_select_count}");

                //Success Message
                Debug.WriteLine("Success delete students in class.");
                ErrorMessage = "និស្សិតត្រូវបានលុបចេញជោគជ័យ!";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);
            }

            await Task.CompletedTask;
        }

        public ICommand Command_ClearStudent_in_ClassList { get; set; }
        //Clear Multi Student Class
        public async Task ClearStudent_in_ClassList()
        {
            if (Selected_Students_to_Class == null || !Selected_Students_to_Class.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសសិស្សនិស្សិតជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                foreach (var student_class in Selected_Students_to_Class)
                {
                    //var student_id = Selected_Students_to_Class.Select(c => c.ID).ToList();

                    if (List_Students_Display.Contains(student_class))
                    {
                        List_Students_Display.Remove(student_class);
                    }
                    Debug.WriteLine("Student Removed");
                    OnPropertyChanged(nameof(Selected_Students_to_Class));
                    Selected_Students_to_Class = null;

                }         

                //Success Message
                ErrorMessage = "និស្សិតត្រូវបានដកចេញជោគជ័យ!";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);
                
            }
            await Task.CompletedTask;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }   
}
