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
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Tsp;
using Microsoft.UI.Xaml.Data;
using RPISVR_Managements.List_and_Reports.Schedule;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using System.Globalization;
using Microsoft.UI.Xaml.Controls.Primitives;


namespace RPISVR_Managements.ViewModel
{
    public class StudentViewModel : INotifyPropertyChanged
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


        public StudentViewModel()
        {
            //Curriculum
            Command_InsertCurriculum = new RelayCommand(async () => await Insert_Curriculum());
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
            Command_Export_Student_in_class_to_PDF = new RelayCommand(async () => await Export_Student_in_class_to_PDF());
            Command_Export_Student_Inclass_ToExcel = new RelayCommand(async () => await GenerateExcel_Student_In_Class_Report());

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
            Classes_Info_Attendence = new ObservableCollection<Class_Schedule>();
            Classes_Info_Attendence_S = new ObservableCollection<Class_Schedule>();
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
            Stu_Delete_DateTime = DateTime.Now;
            Stu_Delete_Info = "3";


            //Get ID and Stu_ID
            _studentModel = new DatabaseConnection();
            _totalStudents = _studentModel.GetTotalStudentsCount();
            var (id, stu_ID) = _studentModel.Get_ID_and_Stu_ID();
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
            Live_Communes_Combobox = new ObservableCollection<Student_Info>();

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

            //Curriculum
            CurriculumTeacher_Combobox = new ObservableCollection<Curriculum_Info>();
            LoadData_to_Combobox_Teacher_Curriculum();

            CurriculumSkill_Combobox = new ObservableCollection<Curriculum_Info>();
            LoadData_to_Combobox_Skill_InCurriculum();

            CurriculumLevel_Combobox = new ObservableCollection<Curriculum_Info>();
            LoadData_to_Combobox_Level_InCurriculum();

            //Load Curriculum to list view
            Curriculum_Info_List = new ObservableCollection<Curriculum_Info>();
            _ = LoadCurriculum_ListView(SearchCurriculumInfo);
            Curriculum_Info_List_Table = new ObservableCollection<Curriculum_Info>();
            Curriculum_TotalTime_List_Table = new ObservableCollection<Curriculum_Info>();

            //Command Edit Curriculum 
            Command_Edit_Curriculum = new RelayCommand(async () => await Edit_Curriculum_Info());

            //Command Clear Curriculum
            Command_Clear_Curriculum = new RelayCommand(async () => await Clear_Curriculum_Info());
            Command_Clear_Search_Year = new RelayCommand(async () => await Clear_Search_Year());

            //Get CurriculumID
            Get_CurriculumID();

            //Command Delete Curriculum
            Command_Delete_Curriculum = new RelayCommand(async () => await Delete_Curriculum_Info());

            //Command Search Curriculum
            Command_Search_Currculum = new RelayCommand(async () => await FetchCurriculum_Info_Table_TotalTime());
            //Defaule Select
            Selected_Search_Curriculum_Skill_ID = CurriculumSkill_Combobox
                .FirstOrDefault(skill_curriculum => skill_curriculum.Curriculum_Skill_Name == "វិទ្យាសាស្ត្រកុំព្យូទ័រ");
            OnPropertyChanged(nameof(Selected_Search_Curriculum_Skill_ID));
            Selected_Search_Curriculum_Level_ID = CurriculumLevel_Combobox
                .FirstOrDefault(level_curriculum => level_curriculum.Curriculum_Level_Name == "បរិញ្ញាបត្រ");
            OnPropertyChanged(nameof(Selected_Search_Curriculum_Level_ID));
            Text_Year = "1,2,3,4";
            _ = OnSearchTextChanged_Curriculum_Info_Table(Curriculum_Skill_Name, Curriculum_Level_Name, Curriculum_Search_Study_Year);

            //Curriculum to PDF and Excel
            Command_Export_Curricum_To_Excel = new RelayCommand(async () => await Export_Curriculum_Info_to_Excel());
            Command_Export_Curriclum_To_PDF = new RelayCommand(async () => await Export_Curriculum_Info_to_PDF());

            //Schedule
            Class_Info_List_Selected_In_Schedule = new ObservableCollection<Student_Info>();
            Command_Add_Class_to_List_in_Schedule = new RelayCommand(async () => await Load_Class_ToList_in_Schedule());
            CommandClear_Class_in_Schedule = new RelayCommand(async () => await Clear_Class_in_Schedule_List());
            Command_SaveSchedule = new RelayCommand(async () => await SaveSchedule());

            //SetTime Mon-Fri
            SD_Start_DateTime_MF1 = TimeSpan.Parse("07:30:00");
            SD_End_DateTime_MF1 = TimeSpan.Parse("09:30:00");
            SD_Start_DateTime_MF2 = TimeSpan.Parse("09:45:00");
            SD_End_DateTime_MF2 = TimeSpan.Parse("11:45:00");

            //SetTime Sat-Sun
            SD_Start_DateTime_SS1 = TimeSpan.Parse("07:30:00");
            SD_End_DateTime_SS1 = TimeSpan.Parse("09:30:00");
            SD_Start_DateTime_SS2 = TimeSpan.Parse("09:45:00");
            SD_End_DateTime_SS2 = TimeSpan.Parse("11:45:00");

            Class_ID_Schedule = 0;
            //Load Combobox
            Schedule_Skill_Name_Combobox = new ObservableCollection<Class_Schedule>();
            Schedule_Teacher_Name_Combobox = new ObservableCollection<Class_Schedule>();
            LoadData_to_Combobox_Schedule_Teacher();

            Command_Load_Schedule = new RelayCommand(async () => await LoadSchedule(Class_ID_Schedule));
            Command_ClearSchedule = new RelayCommand(async () => await ClearSchedule());
            Command_DeleteSchedule = new RelayCommand(async () => await DeleteSchedule());
            Command_Export_Schedule_PDF = new RelayCommand(async () => await Export_Schedule_PDF());

            //Sat_Sun
            Command_SaveSchedule_SatSun = new RelayCommand(async () => await SaveSchedule_SatSun());
            Command_ClearSchedule_SatSun = new RelayCommand(async () => await Clear_Schedule_Sat_Sun());
            Schedule_List_Sat_Sun = new ObservableCollection<Class_Schedule>();
            Command_EditSchedule_SatSun = new RelayCommand(async () => await Click_Edit_Schedule_SatSun());
            Command_DeleteSchedule_SatSun = new RelayCommand(async () => await Delete_Schedule_SatSun());
            Command_ExportSchedule_SatSun_PDF = new RelayCommand(async () => await Export_Schedule_SatSun_PDF());
            Command_Update_Class_State = new RelayCommand(async () => await Update_Class_State());
            Class_Info_State_Schedule = new ObservableCollection<Student_Info>();

            Class_StudyYear_Count_State = EducationStudyYear_Combobox.LastOrDefault();
            OnPropertyChanged(nameof(Class_StudyYear_Count_State));

            Search_Class_In_Skill_Select_State = EducationSubjectSkill_Combobox
                .FirstOrDefault(skill => skill.Stu_EducationSubjects == "វិទ្យាសាស្ត្រកុំព្យូទ័រ");
            OnPropertyChanged(nameof(Search_Class_In_Skill_Select_State));

            Search_Class_In_Level_Select_State = EducationsLevel_Combobox
                .FirstOrDefault(level => level.Stu_EducationLevels == "បរិញ្ញាបត្រ");
            OnPropertyChanged(nameof(Search_Class_In_Level_Select_State));

            //Student Score
            Class_Score = new ObservableCollection<Class_Score>();
            Class_Skill_Score_Info = new ObservableCollection<Class_Score>();
            Class_Student_Score_Info = new ObservableCollection<Class_Score>();
            Class_Skill_State_Info = new ObservableCollection<Class_Score>();
            Student_Score_Show_Skill = new RelayCommand(async () => await Show_Skill_For_Insert_Student_Score());
            Command_Save_Score = new RelayCommand(async () => await SaveStudentScore_Info());
            Command_Edit_Student_Score = new RelayCommand(async () => await Edit_Student_Score());
            Command_Unselect_and_Add = new RelayCommand(async () => await Unselect_and_Add());
            Command_Clear_Student_Score = new RelayCommand(async () => await Clear_Student_Score_Info());
            Command_Delete_Student_Score = new RelayCommand(async () => await Delete_Student_Score_Info());
            Command_Export_Student_Score_PDF = new RelayCommand(async () => await Export_Student_Score_PDF());

            //Student Score
            Student_InClass_Score = new ObservableCollection<Class_Score>();
            Student_Score_Type_Total = new ObservableCollection<Class_Score>();
            Command_Show_Student_In_Student_Score = new RelayCommand(async () => await Show_Student_in_Student_Score());
            Student_Total_Score_By_Subject = new ObservableCollection<Class_Score>();
            //Setting Score
            Command_Save_Setting_Score = new RelayCommand(async () => await Save_Setting_Score());
            Setting_Score_List = new ObservableCollection<Class_Score>();
            _ = Load_Setting_Score();
            Command_Clear_Setting_Score = new RelayCommand(async () => await Clear_Setting_Score_Box());
            Command_Edit_Setting_Score = new RelayCommand(async () => await Select_Setting_Score_Edit());
            Command_Delete_Setting_Score = new RelayCommand(async () => await Delete_Setting_Score());
            Students_Rank_List = new ObservableCollection<Class_Score>();
            Command_Show_Students_Rank = new RelayCommand(async () => await Show_Students_Rank());
            Command_Export_Student_Rank_PDF = new RelayCommand(async () => await Export_Students_Rank_PDF());
            Command_Export_Student_Rank_Excel = new RelayCommand(async () => await Export_Students_Rank_Excel());
            Command_Send_Student_Up_Class = new RelayCommand(async () => await Send_Student_Class_Up());

            //
            Command_Export_PDF_Certificate_of_Education = new RelayCommand(async () => await Export_Certificate_of_Education_PDF());
            Command_Show_Student_Class = new RelayCommand(async () => await ShowStudents_Class_Transcript());
            Transcript_Class_Info = new ObservableCollection<Student_Info>();
            Transcript_Score_Info = new ObservableCollection<Class_Score>();
            Command_Show_Score_andSubject = new RelayCommand(async () => await Show_Subject_and_Score_Transcript());
            Command_Export_Transcript_PDF = new RelayCommand(async () => await Export_Transcript_PDF());

            //Teacher Attendance
            Command_Show_Class_For_Attendene = new RelayCommand(async () => await ShowClass_For_Teacher_Attendence());
            Command_Save_Teacher_Attendent = new RelayCommand(async () => await SaveTeacher_Attendent());
            IsAttendent = true;
            SelectedDate_Attendent = DateTimeOffset.Now;
            Command_Search_Date_Teacher_Info = new RelayCommand(async () => await Search_Date_Teacher_Attendents_Info());
            Date_Teacher_Info_Attendence = new ObservableCollection<Class_Schedule>();
            _ = Load_Date_Teacher_Attendents_Info();
            Command_ShowData_Click_Date = new RelayCommand(async () => await ShowData_Click_Date());
            Command_Delete_Date_Click_Date = new RelayCommand(async () => await Delete_Date_Click_Date());
            Command_Clear_Data_Teacher_Info = new RelayCommand(async () => await Clear_Search_Date());
            Command_Edit_Teacher_Attendent_Info = new RelayCommand(async () => await Edit_Teacher_Attendent_Info());
            Command_Update_Teacher_Attendent = new RelayCommand(async () => await Update_Teacher_Attendent_Info());
            Command_Clear_Teacher_Attendent_Info = new RelayCommand(async () => await Clear_List_Teacher_Attendent());
        }


        //For click Yes in Delete
        private string _CurrentOperation;
        public string CurrentOperation
        {
            get => _CurrentOperation;
            set
            {
                _CurrentOperation = value;
                OnPropertyChanged(nameof(CurrentOperation));
            }
        }
        private string _Status;
        public string Status
        {
            get => _Status;
            set
            {
                _Status = value;
                OnPropertyChanged(nameof(Status));
            }

        }
        //List Score Transcript
        private ObservableCollection<Class_Score> _Transcript_Score_Info;
        public ObservableCollection<Class_Score> Transcript_Score_Info
        {
            get { return _Transcript_Score_Info; }
            set
            {
                _Transcript_Score_Info = value;
                OnPropertyChanged(nameof(Transcript_Score_Info));
            }
        }
        //List Class Transcript
        private ObservableCollection<Student_Info> _Transcript_Class_Info;
        public ObservableCollection<Student_Info> Transcript_Class_Info
        {
            get { return _Transcript_Class_Info; }
            set
            {
                _Transcript_Class_Info = value;
                OnPropertyChanged(nameof(Transcript_Class_Info));
            }
        }

        //List Students Rank
        private ObservableCollection<Class_Score> _Students_Rank_List;
        public ObservableCollection<Class_Score> Students_Rank_List
        {
            get { return _Students_Rank_List; }
            set
            {
                _Students_Rank_List = value;
                OnPropertyChanged(nameof(Students_Rank_List));
            }
        }
        //List Setting Score
        private ObservableCollection<Class_Score> _Setting_Score_List;
        public ObservableCollection<Class_Score> Setting_Score_List
        {
            get { return _Setting_Score_List; }
            set
            {
                _Setting_Score_List = value;
                OnPropertyChanged(nameof(Setting_Score_List));
            }
        }

        //List Student Total Score By Subject
        private ObservableCollection<Class_Score> _Student_Total_Score_By_Subject;
        public ObservableCollection<Class_Score> Student_Total_Score_By_Subject
        {
            get { return _Student_Total_Score_By_Subject; }
            set
            {
                _Student_Total_Score_By_Subject = value;
                OnPropertyChanged(nameof(Student_Total_Score_By_Subject));
            }
        }
        //List Student Score Type
        private ObservableCollection<Class_Score> _Student_Score_Type_Total;
        public ObservableCollection<Class_Score> Student_Score_Type_Total
        {
            get { return _Student_Score_Type_Total; }
            set
            {
                _Student_Score_Type_Total = value;
                OnPropertyChanged(nameof(Student_Score_Type_Total));
            }
        }
        //List Student For Score
        private ObservableCollection<Class_Score> _Student_InClass_Score;
        public ObservableCollection<Class_Score> Student_InClass_Score
        {
            get { return _Student_InClass_Score; }
            set
            {
                _Student_InClass_Score = value;
                OnPropertyChanged(nameof(Student_InClass_Score));
            }
        }

        //Combobox Score Type
        private ObservableCollection<Class_Score> _class_score;
        public ObservableCollection<Class_Score> Class_Score
        {
            get { return _class_score; }
            set
            {
                _class_score = value;
                OnPropertyChanged(nameof(Class_Score));
            }
        }
        //List Skill Score
        private ObservableCollection<Class_Score> _Class_Skill_Score_Info;
        public ObservableCollection<Class_Score> Class_Skill_Score_Info
        {
            get { return _Class_Skill_Score_Info; }
            set
            {
                _Class_Skill_Score_Info = value;
                OnPropertyChanged(nameof(Class_Skill_Score_Info));
            }
        }
        //List Skill Score State
        private ObservableCollection<Class_Score> _Class_Skill_State_Info;
        public ObservableCollection<Class_Score> Class_Skill_State_Info
        {
            get { return _Class_Skill_State_Info; }
            set
            {
                _Class_Skill_State_Info = value;
                OnPropertyChanged(nameof(Class_Skill_State_Info));
            }
        }
        //List Student, Score
        private ObservableCollection<Class_Score> _Class_Student_Score_Info;
        public ObservableCollection<Class_Score> Class_Student_Score_Info
        {
            get { return _Class_Student_Score_Info; }
            set
            {
                _Class_Student_Score_Info = value;
                OnPropertyChanged(nameof(Class_Student_Score_Info));
            }
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

        //Get data to Combobox EducationSubjectSkill_Combobox
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
        //Attendent
        private ObservableCollection<Class_Schedule> _Teacher_Attedent_Combobox;
        public ObservableCollection<Class_Schedule> Teacher_Attedent_Combobox
        {
            get { return _Teacher_Attedent_Combobox; }
            set
            {
                _Teacher_Attedent_Combobox = value;
                OnPropertyChanged(nameof(Teacher_Attedent_Combobox));
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
        public void LoadData_to_Combobox_EducationStudyYear()
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
        private ObservableCollection<Class_Schedule> _Classes_Info_Attendence;
        public ObservableCollection<Class_Schedule> Classes_Info_Attendence
        {
            get { return _Classes_Info_Attendence; }
            set
            {
                _Classes_Info_Attendence = value;
                OnPropertyChanged(nameof(Classes_Info_Attendence));
            }
        }
        private ObservableCollection<Class_Schedule> _Classes_Info_Attendence_S;
        public ObservableCollection<Class_Schedule> Classes_Info_Attendence_S
        {
            get { return _Classes_Info_Attendence_S; }
            set
            {
                _Classes_Info_Attendence_S = value;
                OnPropertyChanged(nameof(Classes_Info_Attendence_S));
            }
        }
        private ObservableCollection<Class_Schedule> _Date_Teacher_Info_Attendence;
        public ObservableCollection<Class_Schedule> Date_Teacher_Info_Attendence
        {
            get { return _Date_Teacher_Info_Attendence; }
            set
            {
                _Date_Teacher_Info_Attendence = value;
                OnPropertyChanged(nameof(Date_Teacher_Info_Attendence));
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
        //Class In Add Schedule
        private ObservableCollection<Student_Info> _selected_class_In_Add_Schedule;
        public ObservableCollection<Student_Info> Class_Info_List_Selected_In_Schedule
        {
            get => _selected_class_In_Add_Schedule;
            set
            {
                _selected_class_In_Add_Schedule = value;
                OnPropertyChanged(nameof(Class_Info_List_Selected_In_Schedule));
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
                var searchTask = Search_Student_Info(Search_Edu_Level, Search_Edu_Skill_Subject, Search_Edu_StudyTimeShift, Search_Edu_TypeStudy, Search_Edu_StudyYear);
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
                var searchTask = Search_Student_Info(Search_Edu_Level, Search_Edu_Skill_Subject, Search_Edu_StudyTimeShift, Search_Edu_TypeStudy, Search_Edu_StudyYear);
                var fetchTask = FetchStudentInfo(SearchText_ID_Name);
                await Task.WhenAll(searchTask, fetchTask);
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
                    OnPropertyChanged(nameof(DateTime_Attendent_Value));
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
                if (Stu_BirthdayDateShow != value)
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
            get => _IsMale ? "ស្រី" : "ប្រុស"; // Return "ប្រុស" if IsMale is true, else "ស្រី"
        }

        //Stu_GenderShow
        private string _Stu_GenderShow;
        public string Stu_GenderShow
        {
            get => _Stu_GenderShow;
            set
            {
                if (_Stu_GenderShow != value)
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
                if (_IsSingle != value)
                {
                    _IsSingle = value;
                    OnPropertyChanged(nameof(IsSingle));
                }
            }
        }
        //String Stu_StateFamily in Khmer
        public string Stu_StateFamily
        {
            get => _IsSingle ? "មានគ្រួសារ" : "នៅលីវ"; //Return "នៅលីវ" if IsSingle is true, else "មានគ្រួសារ"
        }

        //Stu_Levels_ID
        private int _Stu_EducationLevels_ID;
        public int Stu_EducationLevels_ID
        {
            get => _Stu_EducationLevels_ID;
            set
            {
                if (_Stu_EducationLevels_ID != value)
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
                if (_Stu_EducationLevels != value)
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
                if (_Stu_EducationSubject_ID != value)
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
                if (_Stu_EducationSubjects != value)
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
                if (_Stu_StudyTimeShift_ID != value)
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
                if (_Stu_StudyTimeShift != value)
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
                if (_Stu_EducationType_ID != value)
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
                if (_Stu_EducationType != value)
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
                if (_Stu_StudyingTime != value)
                {
                    _Stu_StudyingTime = value;
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
                if (_Stu_Birth_Province_ID != value)
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
                if (_Stu_Birth_Province != value)
                {
                    _Stu_Birth_Province = value;
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
                if (_Stu_Birth_Distric != value)
                {
                    _Stu_Birth_Distric = value;
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
                if (_Stu_Birth_Commune != value)
                {
                    _Stu_Birth_Commune = value;
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
                if (_Stu_Birth_Village != value)
                {
                    _Stu_Birth_Village = value;
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
                if (_Stu_Live_Pro_ID != value)
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
                if (_Stu_Live_Pro != value)
                {
                    _Stu_Live_Pro = value;
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
                if (_Stu_Live_Dis_ID != value)
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
                if (_Stu_Live_Dis != value)
                {
                    _Stu_Live_Dis = value;
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
                if (_Stu_Live_Comm_ID != value)
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
                if (_Stu_Live_Comm != value)
                {
                    _Stu_Live_Comm = value;
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
                if (_Stu_Live_Vill_ID != value)
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
                if (_Stu_Live_Vill != value)
                {
                    _Stu_Live_Vill = value;
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
                if (_Stu_Jobs != value)
                {
                    _Stu_Jobs = value;
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
                if (_Stu_StudyYear_ID != value)
                {
                    _Stu_StudyYear_ID = value;
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
                if (_Stu_StatePoor != value)
                {
                    _Stu_StatePoor = value;
                    OnPropertyChanged(nameof(Stu_StatePoor));
                }
            }
        }
        //Stu_Skill_English
        private string _Stu_Skill_English;
        public string Stu_Skill_English
        {
            get => _Stu_Skill_English;
            set
            {
                _Stu_Skill_English = value;
                OnPropertyChanged(nameof(Stu_Skill_English));
            }
        }
        //Stu_Gender_English
        private string _Stu_Gender_English;
        public string Stu_Gender_English
        {
            get => _Stu_Gender_English;
            set
            {
                _Stu_Gender_English = value;
                OnPropertyChanged(nameof(Stu_Gender_English));
            }
        }
        //Stu_Place_Birth_English
        private string _Stu_Place_Birth_English;
        public string Stu_Place_Birth_English
        {
            get => _Stu_Place_Birth_English;
            set
            {
                _Stu_Place_Birth_English = value;
                OnPropertyChanged(nameof(Stu_Place_Birth_English));
            }
        }
        //Stu_Degree_English
        private string _Stu_Degree_English;
        public string Stu_Degree_English
        {
            get => _Stu_Degree_English;
            set
            {
                _Stu_Degree_English = value;
                OnPropertyChanged(nameof(Stu_Degree_English));
            }
        }
        //Stu_Date_Graduation
        private string _Stu_Date_Graduation;
        public string Stu_Date_Graduation
        {
            get => _Stu_Date_Graduation;
            set
            {
                _Stu_Date_Graduation = value;
                OnPropertyChanged(nameof(Stu_Date_Graduation));
            }

        }
        //Stu_Internship_Text
        private string _Stu_Internship_Text;
        public string Stu_Internship_Text
        {
            get => _Stu_Internship_Text;
            set
            {
                _Stu_Internship_Text = value;
                OnPropertyChanged(nameof(Stu_Internship_Text));
            }
        }
        //Stu_Internship_Credit
        private int _Stu_Internship_Credit;
        public int Stu_Internship_Credit
        {
            get => _Stu_Internship_Credit;
            set
            {
                _Stu_Internship_Credit = value;
                OnPropertyChanged(nameof(Stu_Internship_Credit));
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
            Stu_FirstName_KH = string.Empty;
            Stu_LastName_KH = string.Empty;
            Stu_FirstName_EN = string.Empty;
            Stu_LastName_EN = string.Empty;
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
            Stu_Image_Source = null;
            ProfileImageBytes = null;
            Stu_Image_Total_Big = string.Empty;
            Stu_Image_TotalSmall = string.Empty;
            //Stu_Images_Degree_Yes_No = null;
            Stu_Image_Degree_Source = null;
            Stu_Image_Degree_Bytes = null;
            //Stu_ImageBirth_Cert_YesNo = null;
            Stu_ImageBirth_Cert_Bytes = null;
            Stu_ImageBirth_Cert_Source = null;
            //Stu_ImageIDNation_YesNo = null;
            Stu_ImageIDNation_Bytes = null;
            Stu_ImageIDNation_Source = null;
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
            if (string.IsNullOrEmpty(Stu_Generation))
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
            if (SelectedBirthProvince_Info == null)
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
            var student_check_info = await _dbConnection.GetStudents_Check_Student_Info(Stu_FirstName_KH, Stu_LastName_KH, Stu_Gender, Stu_BirthdayDateOnly, Stu_EducationType = this.SelectedStu_EducationType_Info.Stu_EducationType, Stu_StudyYear = this.SelectesStu_StudyYear_Info.Stu_StudyYear);

            if (Stu_FirstName_KH == student_check_info.Stu_FirstName_KH1 &&
                    Stu_LastName_KH == student_check_info.Stu_LastName_KH1 &&
                    Stu_Gender == student_check_info.Stu_Gender1 &&
                    Stu_BirthdayDateOnly == student_check_info.Stu_BirthdayDateOnly1 &&
                    Stu_EducationType == student_check_info.Stu_EducationType1 &&
                    Stu_StudyYear == student_check_info.Stu_StudyYear1)
            {
                ErrorMessage = "និស្សិតឈ្មោះ " + Stu_FirstName_KH + Stu_LastName_KH + " " + Stu_EducationType + " " + Stu_StudyYear + " មានទិន្នន័យរួចរាល់ហើយ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            // If everything is valid
            SaveStudentInformationToDatabase();
            ClearStudentInfo();
            await LoadStudents(SearchText_ID_Name_Insert);



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
                    //1
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
                        .FirstOrDefault(education_level => education_level.Stu_EducationLevels == _selectedStudent.Stu_EducationLevels);
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
                    OnPropertyChanged(nameof(SelectedBirthVillage_Info));
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

                if (_selectedEducationLevel_Stu_Info == null)
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
                if (_selectedStudyTimeShift_Stu_Info == null)
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
                if (_selectedEducationType_Stu_Info == null)
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
                if (_selectedStu_StudyYear_Stu_Info == null)
                {
                    Search_Edu_StudyYear = null;
                }
                else
                {
                    Search_Edu_StudyYear = this._selectedStu_StudyYear_Stu_Info.Stu_StudyYear;
                }
            }
        }



        public async Task Search_Student_Info(string Search_Edu_Level, string Search_Edu_Skill_Subject, string Search_Edu_StudyTimeShift, string Search_Edu_TypeStudy, string Search_Edu_StudyYear)
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
                if (SelectedStu_ID_Edit != value)
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
            if (_selectedStudent != null)
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
                if (_SearchText_Education_StudyType_Text == null)
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
                if (_SearchText_Education_StudyYear_Text == null)
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
                    if (SearchText_Education_Level == "បរិញ្ញាបត្របច្ចេកវិទ្យា" || SearchText_Education_Level == "សញ្ញាបត្រវិស្វករ" || SearchText_Education_Level == "បរិញ្ញាបត្រ")
                    {
                        Education_Level_Text = "បរិញ្ញាបត្របច្ចេកវិទ្យា សញ្ញាបត្រវិស្វករ បរិញ្ញាបត្រ";
                    }
                    else if (SearchText_Education_Level == "បរិញ្ញាបត្ររង" || SearchText_Education_Level == "សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស")
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

        public async Task Search_Education_Report_Solarship(string SearchText_Education_Level, string SearchText_Education_StudyYear, string SearchText_Education_StudyType)
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

                PDFService_Report_Student_Solarship.CreateReport(SelectedStudents_Report, SearchText_Education_StudyType, Education_Level_Text, Education_Start_Date, SearchText_Education_StudyYear);
                Debug.WriteLine("PDF reports generated for all selected students.");
            }
            

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
                ExportExcel_Student_Report.ExportToExcel(SelectedStudents_Report.ToList(), Education_Level_Text);
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
            if (Selection_Student_Card == null)
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
            if (Selection_Student_Card == null && Selection_Student_Card.Any())
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
                    if (Class_In_Skill_Select == null)
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
                if (_Class_In_Study_Year_Select == null)
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
                if (_Class_In_Student_Year_Select == null)
                {
                    Class_In_Student_Year = null;
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
                if (_Class_In_Study_Timeshift_Select == null)
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

                if (_Class_In_Study_Type_Select == null)
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

            if (Class_In_Level_Select == null)
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
            //Check Class Infomation Before Insert
            var class_check_info = await _dbConnection.GetClasses_Check_Info(Class_In_Skill, Class_In_Study_Year, Class_In_Level, Class_In_Student_Year, Class_In_Semester, Class_In_Generation, Class_In_Study_Timeshift, Class_In_Study_Type);

            if (Class_In_Skill == class_check_info.Class_In_Skill1 &&
                Class_In_Study_Year == class_check_info.Class_In_Study_Year1 &&
                Class_In_Level == class_check_info.Class_In_Level1 &&
                Class_In_Student_Year == class_check_info.Class_In_Student_Year1 &&
                Class_In_Semester == class_check_info.Class_In_Semester1 &&
                Class_In_Generation == class_check_info.Class_In_Generation1 &&
                Class_In_Study_Timeshift == class_check_info.Class_In_Study_Timeshift1 &&
                Class_In_Study_Type == class_check_info.Class_In_Study_Type1)
            {
                ErrorMessage = "ថ្នាក់រៀន៖ " + Class_Name + " មានទិន្នន័យដូចគ្នារួចស្រេចហើយ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
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
                if (_Search_Class_In_Skill_Select != value)
                {
                    _Search_Class_In_Skill_Select = value;
                    OnPropertyChanged(nameof(Search_Class_In_Skill_Select));
                    if (Search_Class_In_Skill_Select == null)
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
                if (_Search_Class_In_Level_Select != value)
                {
                    _Search_Class_In_Level_Select = value;
                    OnPropertyChanged(nameof(Search_Class_In_Level_Select));
                    if (Search_Class_In_Level_Select == null)
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
                    if (Search_Class_In_Study_Timeshift_Select == null)
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
                if (_Search_Class_In_Study_Type_Select != value)
                {
                    _Search_Class_In_Study_Type_Select = value;
                    OnPropertyChanged(nameof(Search_Class_In_Student_Type_Select));

                    if (Search_Class_In_Student_Type_Select == null)
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
        //Multi Select All Student Rank
        private List<Class_Score> _MultiSelectAllStudent_Rank;
        public List<Class_Score> MultiSelectAllStudent_Rank
        {
            get => _MultiSelectAllStudent_Rank;
            set
            {
                _MultiSelectAllStudent_Rank = value;
                OnPropertyChanged(nameof(MultiSelectAllStudent_Rank));
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
            if (_firstSelectedClass == null)
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
            if (SelectedClasses_Edit_Delete == null || !SelectedClasses_Edit_Delete.Any())
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
            if (SelectedClass_Add_Student == null)
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
            if (SelectedClass_Edition == null)
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

                    Debug.WriteLine($"Class dd ID: {Class_ID}");
                    _ = Count_Student_Selected_Class();
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
                if (_Max_Student_In_Class != value)
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
                if (_Current_Student_InClass != value)
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


            if (string.IsNullOrEmpty(Class_Name))
            {
                Debug.WriteLine("No Class Selection");
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Max_Student_InClass == 0)
            {
                Debug.WriteLine("No Enter total Student.");
                ErrorMessage = "សូមបញ្ចូលចំនួននិស្សិតសរុបក្នុងថ្នាក់ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Max_Student_InClass < 1 || Max_Student_InClass >= 50)
            {
                Debug.WriteLine("Total Student should smaller than 50.");
                ErrorMessage = "ចំនួននិស្សិតសរុបក្នុងថ្នាក់ត្រូវតែតិចជាង 50 នាក់ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(Class_ID))
            {
                Debug.WriteLine("No Class Selection");
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
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
                    Class_ID = SelectedClass_Add_Student.Class_ID;
                    //
                    var classList_Displays = _dbConnection.Display_Student_List_in_Class(Max_Student_InClass, Class_In_Study_Year, Class_In_Level, Class_In_Skill, Class_In_Student_Year, Class_In_Study_Timeshift);
                    var studentList_Displays = _dbConnection.Display_Student_List_in_Class2(Class_ID);
                    // Clear the existing list to prepare for the new page data
                    List_Students_Display.Clear();
                    List_Student_In_Class_Display.Clear();
                    Debug.WriteLine("Loading student.");

                    // Iterate over the studentsList returned by the database and add them to the ObservableCollection                  
                    foreach (var student in classList_Displays)
                    {
                        student.Full_Name_KH = student.Stu_FirstName_KH + " " + student.Stu_LastName_KH;
                        List_Students_Display.Add(student);
                    }

                    List_Students_Display = new ObservableCollection<Student_Info>(classList_Displays);

                    foreach (var student_in_class in studentList_Displays)
                    {

                        student_in_class.Full_Name_KH = student_in_class.Stu_FirstName_KH + " " + student_in_class.Stu_LastName_KH;
                        student_in_class.Full_Name_EN = student_in_class.Stu_FirstName_EN + " " + student_in_class.Stu_LastName_EN;
                        student_in_class.Stu_BirthdayDateShow = ConvertToKhmerDate(student_in_class.Stu_BirthdayDateOnly);
                        List_Student_In_Class_Display.Add(student_in_class);
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
        public ICommand Command_Insert_Students_to_Class { get; set; }

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
            if (Max_Student_InClass < 0 || Max_Student_InClass >= 50)
            {
                Debug.WriteLine("Total Student should smaller than 50.");
                ErrorMessage = "ចំនួននិស្សិតសរុបក្នុងថ្នាក់ត្រូវតែតិចជាង 50 នាក់ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Total_Current_Student >= Max_Student_InClass)
            {
                Debug.WriteLine("Total Student should bigger than Before value.");
                ErrorMessage = "និស្សិតសរុបក្នុងថ្នាក់ គ្រប់ចំនួនហើយ !";
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
                    total_stu = viewModel.GetTotalStudents(class_id);
                    Current_Student_InClass = total_stu;



                    if (Current_Student_InClass >= Max_Student_InClass)
                    {
                        Debug.WriteLine("The class is already full. No more students can be added.");
                        //return;
                        break;
                    }

                    //var class_id = Class_ID;
                    var max_student_class = Max_Student_InClass;
                    var student_id = new List<int> { student_class.ID };

                    _dbConnection.Insert_Students_to_Class(student_id, class_id, max_student_class);

                    Debug.WriteLine($"Selected Student ID: {student_class.ID}");
                    Debug.WriteLine($"Selected Class ID: {class_id}");

                    total_stu = viewModel.GetTotalStudents(class_id);
                    _dbConnection.UpdateStudentSelectCount(class_id, total_stu);
                    Current_Student_InClass = total_stu;
                    student_id_count++;

                }


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
                        ErrorMessage = "បរាជ័យក្នុងការលុបទិន្នន័យ !";
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

        //Command Export Student in class to PDF
        public ICommand Command_Export_Student_in_class_to_PDF { get; set; }

        public async Task Export_Student_in_class_to_PDF()
        {
            if (Selected_Students_in_Class == null || !Selected_Students_in_Class.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសសិស្សនិស្សិតក្នុងថ្នាក់ជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(Class_Name))
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                Debug.WriteLine("Export Student to PDF Success.");

                //int student_select_count = 0;
                var class_id = Class_ID;
                string class_name = Class_Name;
                string class_in_skill = Class_In_Skill;
                string class_in_level = Class_In_Level;
                string class_in_study_year = Class_In_Study_Year;
                string class_in_student_year = Class_In_Student_Year;
                string class_in_semester = Class_In_Semester;
                string class_in_generation = Class_In_Generation;
                string class_study_time_shift = Class_In_Study_Timeshift;
                string class_in_study_type = Class_In_Study_Type;

                //File Class_toPDF.
                PDFService_Generate_Student_In_Class.CreateReport(Selected_Students_in_Class, class_id, class_name, class_in_skill, class_in_level, class_in_study_year, class_in_student_year, class_in_semester, class_in_generation, class_study_time_shift, class_in_study_type);

            }

            await Task.CompletedTask;
        }

        //Command Export Student in Class to Excel
        public ICommand Command_Export_Student_Inclass_ToExcel { get; set; }

        //Student in Class Report Excel
        public async Task GenerateExcel_Student_In_Class_Report()
        {
            if (Selected_Students_in_Class == null || !Selected_Students_in_Class.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសសិស្សនិស្សិតក្នុងថ្នាក់ជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (string.IsNullOrEmpty(Class_Name))
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                var class_id = Class_ID;
                string class_name = Class_Name;
                string class_in_skill = Class_In_Skill;
                string class_in_level = Class_In_Level;
                string class_in_study_year = Class_In_Study_Year;
                string class_in_student_year = Class_In_Student_Year;
                string class_in_semester = Class_In_Semester;
                string class_in_generation = Class_In_Generation;
                string class_study_time_shift = Class_In_Study_Timeshift;
                string class_in_study_type = Class_In_Study_Type;

                //ExportExcel_Student_Report.ExportToExcel(SelectedStudents_Report.ToList(), Education_Level_Text);
                Export_Excel_Students_In_Class.ExportToExcel(Selected_Students_in_Class, class_id, class_name, class_in_skill, class_in_level, class_in_study_year, class_in_student_year, class_in_semester, class_in_generation, class_study_time_shift, class_in_study_type);
            }
            await Task.CompletedTask;
        }

        //Curriculum Model
        private int _C_ID;
        public int C_ID
        {
            get => _C_ID;
            set
            {
                _C_ID = value;
                OnPropertyChanged(nameof(C_ID));
            }
        }
        private string _Curriculum_ID;
        public string Curriculum_ID
        {
            get => _Curriculum_ID;
            set
            {
                _Curriculum_ID = value;
                OnPropertyChanged(nameof(Curriculum_ID));
                ValidateCurriculum_ID();
            }
        }
        private string _Curriculum_Name_KH;
        public string Curriculum_Name_KH
        {
            get => _Curriculum_Name_KH;
            set
            {
                _Curriculum_Name_KH = value;
                OnPropertyChanged(nameof(Curriculum_Name_KH));
                ValidateCurriculum_Name_KH();
            }
        }
        private string _Curriculum_Name_EN;
        public string Curriculum_Name_EN
        {
            get => _Curriculum_Name_EN;
            set
            {
                _Curriculum_Name_EN = value;
                OnPropertyChanged(nameof(Curriculum_Name_EN));
                ValidateCurriculum_Name_EN();
            }
        }
        private int _Curriculum_Total_Time;
        public int Curriculum_Total_Time
        {
            get => _Curriculum_Total_Time;
            set
            {
                _Curriculum_Total_Time = value;
                OnPropertyChanged(nameof(Curriculum_Total_Time));
                ValidateCurriculum_Total_Time();
            }
        }
        private int _Curriculum_Skill_ID;
        public int Curriculum_Skill_ID
        {
            get => _Curriculum_Skill_ID;
            set
            {
                _Curriculum_Skill_ID = value;
                OnPropertyChanged(nameof(Curriculum_Skill_ID));
                ValidateCurriculum_Skill_ID();
            }
        }
        private Curriculum_Info _SelectedCurriculum_Skill_ID;
        public Curriculum_Info SelectedCurriculum_Skill_ID
        {
            get => _SelectedCurriculum_Skill_ID;
            set
            {
                if (_SelectedCurriculum_Skill_ID != value)
                {
                    _SelectedCurriculum_Skill_ID = value;
                    OnPropertyChanged(nameof(SelectedCurriculum_Skill_ID));

                    if (SelectedCurriculum_Skill_ID == null)
                    {
                        Curriculum_Skill_ID = 0;
                        ValidateCurriculum_Skill_ID();
                    }
                    else
                    {
                        Curriculum_Skill_ID = SelectedCurriculum_Skill_ID.Curriculum_Skill_ID;
                        ValidateCurriculum_Skill_ID();
                    }
                }
            }
        }
        private int _Curriculum_Teacher_ID;
        public int Curriculum_Teacher_ID
        {
            get => _Curriculum_Teacher_ID;
            set
            {
                _Curriculum_Teacher_ID = value;
                OnPropertyChanged(nameof(Curriculum_Teacher_ID));
                ValidateCurriculum_Teacher_ID();
            }
        }
        private Curriculum_Info _SelectedCurriculum_Teacher_ID;
        public Curriculum_Info SelectedCurriculum_Teacher_ID
        {
            get => _SelectedCurriculum_Teacher_ID;
            set
            {
                if (_SelectedCurriculum_Teacher_ID != value)
                {
                    _SelectedCurriculum_Teacher_ID = value;
                    OnPropertyChanged(nameof(SelectedCurriculum_Teacher_ID));

                    if (SelectedCurriculum_Teacher_ID == null)
                    {
                        Curriculum_Teacher_ID = 0;
                        ValidateCurriculum_Teacher_ID();
                    }
                    else
                    {
                        Curriculum_Teacher_ID = SelectedCurriculum_Teacher_ID.Curriculum_Teacher_ID;
                        ValidateCurriculum_Teacher_ID();
                    }
                }
            }
        }
        private string _Curriculum_Study_Year;
        public string Curriculum_Study_Year
        {
            get => _Curriculum_Study_Year;
            set
            {
                _Curriculum_Study_Year = value;
                OnPropertyChanged(nameof(Curriculum_Study_Year));
                ValidateCurriculum_Study_Year();
            }
        }

        private string _Curriculum_Semester;
        public string Curriculum_Semester
        {
            get => _Curriculum_Semester;
            set
            {
                _Curriculum_Semester = value;
                OnPropertyChanged(nameof(Curriculum_Semester));
                ValidateCurriculum_Semester();
            }
        }

        private int _Curriculum_Total_Score;
        public int Curriculum_Total_Score
        {
            get => _Curriculum_Total_Score;
            set
            {
                _Curriculum_Total_Score = value;
                OnPropertyChanged(nameof(Curriculum_Total_Score));
                ValidateCurriculum_Total_Score();
            }
        }
        private int _Curriculum_Level_ID;
        public int Curriculum_Level_ID
        {
            get => _Curriculum_Level_ID;
            set
            {
                _Curriculum_Level_ID = value;
                OnPropertyChanged(nameof(Curriculum_Level_ID));
            }
        }
        private Curriculum_Info _SelectedCurriculum_Level_ID;
        public Curriculum_Info SelectedCurriculum_Level_ID
        {
            get => _SelectedCurriculum_Level_ID;
            set
            {
                _SelectedCurriculum_Level_ID = value;
                OnPropertyChanged(nameof(SelectedCurriculum_Level_ID));

                if (SelectedCurriculum_Level_ID == null)
                {
                    Curriculum_Level_ID = 0;
                    ValidateCurriculum_Level_ID();
                }
                else
                {
                    Curriculum_Level_ID = SelectedCurriculum_Level_ID.Curriculum_Level_ID;
                    ValidateCurriculum_Level_ID();
                }
            }
        }
        //ValidateCurriculum_Level_ID
        public SolidColorBrush Curriculum_Level_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_Level_IDBorderBrush));
            }
        }
        private void ValidateCurriculum_Level_ID()
        {
            if (SelectedCurriculum_Level_ID == null)
            {
                Curriculum_Level_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_Level_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateCurriculum_ID
        public SolidColorBrush Curriculum_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_IDBorderBrush));
            }
        }
        private void ValidateCurriculum_ID()
        {
            if (string.IsNullOrEmpty(Curriculum_ID))
            {
                Curriculum_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateCurriculum_Name_KH
        public SolidColorBrush Curriculum_Name_KHBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_Name_KHBorderBrush));
            }
        }
        private void ValidateCurriculum_Name_KH()
        {
            if (string.IsNullOrEmpty(Curriculum_Name_KH))
            {
                Curriculum_Name_KHBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_Name_KHBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateCurriculum_Name_EN
        public SolidColorBrush Curriculum_Name_ENBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_Name_ENBorderBrush));
            }
        }
        private void ValidateCurriculum_Name_EN()
        {
            if (string.IsNullOrEmpty(Curriculum_Name_EN))
            {
                Curriculum_Name_ENBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_Name_ENBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateCurriculum_Skill_ID
        public SolidColorBrush Curriculum_Skill_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_Skill_IDBorderBrush));
            }
        }
        private void ValidateCurriculum_Skill_ID()
        {
            if (Curriculum_Skill_ID == 0)
            {
                Curriculum_Skill_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_Skill_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateCurriculum_Teacher_ID
        public SolidColorBrush Curriculum_Teacher_IDBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_Teacher_IDBorderBrush));
            }
        }
        private void ValidateCurriculum_Teacher_ID()
        {
            if (Curriculum_Teacher_ID == 0)
            {
                Curriculum_Teacher_IDBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_Teacher_IDBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateCurriculum_Study_Year
        public SolidColorBrush Curriculum_Study_YearBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_Study_YearBorderBrush));
            }
        }
        private void ValidateCurriculum_Study_Year()
        {
            if (string.IsNullOrEmpty(Curriculum_Study_Year))
            {
                Curriculum_Study_YearBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_Study_YearBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateCurriculum_Semester
        public SolidColorBrush Curriculum_SemesterBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_SemesterBorderBrush));
            }
        }
        private void ValidateCurriculum_Semester()
        {
            if (string.IsNullOrEmpty(Curriculum_Semester))
            {
                Curriculum_SemesterBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_SemesterBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateCurriculum_Total_Time
        public SolidColorBrush Curriculum_Total_TimeBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_Total_TimeBorderBrush));
            }
        }
        private void ValidateCurriculum_Total_Time()
        {
            if (Curriculum_Total_Time <= 0)
            {
                Curriculum_Total_TimeBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_Total_TimeBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        //ValidateCurriculum_Total_Score
        public SolidColorBrush Curriculum_Total_ScoreBorderBrush
        {
            get => _ErrorBorderBrush;
            set
            {
                _ErrorBorderBrush = value;
                OnPropertyChanged(nameof(Curriculum_Total_ScoreBorderBrush));
            }
        }
        private void ValidateCurriculum_Total_Score()
        {
            if (Curriculum_Total_Score <= 0)
            {
                Curriculum_Total_ScoreBorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Curriculum_Total_ScoreBorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
        private string _errorMessage_Delete;
        public string ErrorMessage_Delete
        {
            get => _errorMessage_Delete;
            set
            {
                _errorMessage_Delete = value;
                OnPropertyChanged(nameof(ErrorMessage_Delete));
                UpdateMessage_DeleteColor();
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


        //Command InsertCurriculum
        public ICommand Command_InsertCurriculum { get; set; }

        public async Task Insert_Curriculum()
        {
            ValidateCurriculum_ID();
            ValidateCurriculum_Name_KH();
            ValidateCurriculum_Name_EN();
            ValidateCurriculum_Skill_ID();
            ValidateCurriculum_Level_ID();
            ValidateCurriculum_Teacher_ID();
            ValidateCurriculum_Study_Year();
            ValidateCurriculum_Semester();
            ValidateCurriculum_Total_Time();
            ValidateCurriculum_Total_Score();

            if (string.IsNullOrEmpty(Curriculum_ID))
            {
                ErrorMessage = "ID ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (string.IsNullOrEmpty(Curriculum_Name_KH))
            {
                ErrorMessage = "ឈ្មោះមុខវិជ្ជា (ខ្មែរ) ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (string.IsNullOrEmpty(Curriculum_Name_EN))
            {
                ErrorMessage = "ឈ្មោះមុខវិជ្ជា (English) ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (SelectedCurriculum_Skill_ID == null)
            {
                ErrorMessage = "ជំនាញ ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (SelectedCurriculum_Level_ID == null)
            {
                ErrorMessage = "កម្រិតសិក្សា ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (SelectedCurriculum_Teacher_ID == null)
            {
                ErrorMessage = "គ្រូបច្ចេកទេស ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (string.IsNullOrEmpty(Curriculum_Study_Year))
            {
                ErrorMessage = "ឆ្នាំ ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (string.IsNullOrEmpty(Curriculum_Semester))
            {
                ErrorMessage = "ឆមាស ត្រូវតែជ្រើសរើស  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (Curriculum_Total_Time <= 0)
            {
                ErrorMessage = "ចំនួនម៉ោងសរុប ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }
            if (Curriculum_Total_Score <= 0)
            {
                ErrorMessage = "ពិន្ទុសរុប ត្រូវតែបំពេញ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red); // Error: Red color
                return;
            }

            ErrorMessage = string.Empty;
            ErrorImageSource = null;
            MessageColor = new SolidColorBrush(Colors.Transparent);

            Debug.WriteLine($"Cur_ID: {Curriculum_ID}");
            Debug.WriteLine($"Name_KH: {Curriculum_Name_KH}");
            Debug.WriteLine($"Name_EN: {Curriculum_Name_EN}");
            Debug.WriteLine($"Level: {Curriculum_Level_ID}");
            Debug.WriteLine($"Year: {Curriculum_Study_Year}");
            Debug.WriteLine($"Skill_ID: {Curriculum_Skill_ID}");
            Debug.WriteLine($"Teacher_ID: {Curriculum_Teacher_ID}");
            Debug.WriteLine($"Semester: {Curriculum_Semester}");
            Debug.WriteLine($"Score: {Curriculum_Total_Score}");

            SaveCurriculum_Infomation();

            await Task.CompletedTask;
        }

        //Get data to Combobox Teacher Curriculum
        private ObservableCollection<Curriculum_Info> _teacher_info_curriculum;
        public ObservableCollection<Curriculum_Info> CurriculumTeacher_Combobox
        {
            get { return _teacher_info_curriculum; }
            set
            {
                _teacher_info_curriculum = value;
                OnPropertyChanged(nameof(CurriculumTeacher_Combobox));
            }
        }

        //Load Data to Combobox Teacher Curriculum
        private void LoadData_to_Combobox_Teacher_Curriculum()
        {
            var TeacherList = _dbConnection.GetTeacherInfo_List_Curriculum();
            foreach (var teacher_list in TeacherList)
            {
                CurriculumTeacher_Combobox.Add(teacher_list);
            }
        }
        //Get data to Combobox Skill Curriculum
        private ObservableCollection<Curriculum_Info> _skill_info_curriculum;
        public ObservableCollection<Curriculum_Info> CurriculumSkill_Combobox
        {
            get { return _skill_info_curriculum; }
            set
            {
                _skill_info_curriculum = value;
                OnPropertyChanged(nameof(CurriculumSkill_Combobox));
            }
        }
        //Get data to Combobox Level Curriculum
        private ObservableCollection<Curriculum_Info> _level_info_curriculum;
        public ObservableCollection<Curriculum_Info> CurriculumLevel_Combobox
        {
            get { return _level_info_curriculum; }
            set
            {
                _level_info_curriculum = value;
                OnPropertyChanged(nameof(CurriculumLevel_Combobox));
            }
        }
        //Get data to ListView
        private ObservableCollection<Curriculum_Info> _Curriculum_Info_List;
        public ObservableCollection<Curriculum_Info> Curriculum_Info_List
        {
            get { return _Curriculum_Info_List; }
            set
            {
                _Curriculum_Info_List = value;
                OnPropertyChanged(nameof(Curriculum_Info_List));
            }
        }
        private ObservableCollection<Curriculum_Info> _Curriculum_Info_List_Table;
        public ObservableCollection<Curriculum_Info> Curriculum_Info_List_Table
        {
            get { return _Curriculum_Info_List_Table; }
            set
            {
                _Curriculum_Info_List_Table = value;
                OnPropertyChanged(nameof(Curriculum_Info_List_Table));
            }
        }
        private ObservableCollection<Curriculum_Info> _Curriculum_TotalTime_List_Table;
        public ObservableCollection<Curriculum_Info> Curriculum_TotalTime_List_Table
        {
            get { return _Curriculum_TotalTime_List_Table; }
            set
            {
                _Curriculum_TotalTime_List_Table = value;
                OnPropertyChanged(nameof(Curriculum_TotalTime_List_Table));
            }
        }

        //Load data to Combobox Skill 
        private void LoadData_to_Combobox_Skill_InCurriculum()
        {
            var skillList = _dbConnection.GetSkillInfo_List_Curriculum();
            foreach (var skill_list in skillList)
            {
                CurriculumSkill_Combobox.Add(skill_list);
            }
        }
        //Load data to Combobox Level
        private void LoadData_to_Combobox_Level_InCurriculum()
        {
            var levelList = _dbConnection.GetLevelInfo_List_Curriculum();
            foreach (var level_list in levelList)
            {
                CurriculumLevel_Combobox.Add(level_list);
            }
        }

        //Method to Save Curriculum_Info
        public async void SaveCurriculum_Infomation()
        {
            //Check Curriculum Info Before
            var curriculum_check_first = await _dbConnection.GetCurriculum_Info_Check(Curriculum_Name_KH, Curriculum_Name_EN, Curriculum_Skill_ID, Curriculum_Teacher_ID, Curriculum_Study_Year, Curriculum_Semester, Curriculum_Total_Time, Curriculum_Total_Score, Curriculum_Level_ID);

            if (curriculum_check_first.Curriculum_Name_KH1 == Curriculum_Name_KH &&
                curriculum_check_first.Curriculum_Name_EN1 == Curriculum_Name_EN &&
                curriculum_check_first.Curriculum_Skill_ID1 == Curriculum_Skill_ID &&
                curriculum_check_first.Curriculum_Level_ID1 == Curriculum_Level_ID)
            {
                ErrorMessage = "កម្មវិធីសិក្សាមុខវិជ្ជា៖ " + Curriculum_Name_KH + " មានទិន្នន័យរួចស្រេចហើយ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            //Update Method
            var UpdateCurriculum = Curriculum_Info_List.FirstOrDefault(s => s.Curriculum_ID == Curriculum_ID);
            if (UpdateCurriculum != null)
            {
                UpdateCurriculum.Curriculum_ID = Curriculum_ID;
                UpdateCurriculum.Curriculum_Name_KH = Curriculum_Name_KH;
                UpdateCurriculum.Curriculum_Name_EN = Curriculum_Name_EN;
                UpdateCurriculum.Curriculum_Skill_ID = SelectedCurriculum_Skill_ID.Curriculum_Skill_ID;
                UpdateCurriculum.Curriculum_Level_ID = SelectedCurriculum_Level_ID.Curriculum_Level_ID;
                UpdateCurriculum.Curriculum_Teacher_ID = SelectedCurriculum_Teacher_ID.Curriculum_Teacher_ID;
                UpdateCurriculum.Curriculum_Study_Year = Curriculum_Study_Year;
                UpdateCurriculum.Curriculum_Semester = Curriculum_Semester;
                UpdateCurriculum.Curriculum_Total_Score = Curriculum_Total_Score;
                UpdateCurriculum.Curriculum_Total_Time = Curriculum_Total_Time;

                Debug.WriteLine("Curriculum Update Mode.");

                bool success = _dbConnection.Update_Curriculum_Info(UpdateCurriculum);

                if (success)
                {
                    Debug.WriteLine("Update success.");

                    //Enable Button
                    IsInsertEnabled = true;
                    IsUpdateEnabled = false;

                    ErrorMessage = "កម្មវិធីសិក្សាមុខវិជ្ជា៖ " + Curriculum_Name_KH + " បានធ្វើបច្ចុប្បន្នភាពជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                }
                else
                {
                    Debug.WriteLine("Update faild.");
                    ErrorMessage = "កម្មវិធីសិក្សាមុខវិជ្ជា៖ " + Curriculum_Name_KH + " ធ្វើបច្ចុប្បន្នភាពបរាជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                }
                ClearTextBox_Curriculum();
                Get_CurriculumID();
                _ = LoadCurriculum_ListView(SearchCurriculumInfo);
            }
            else
            {
                //Insert Method
                Debug.WriteLine("Insert Mode. <<Curriculum>>");
                Curriculum_Info curriculum_Info = new Curriculum_Info()
                {
                    Curriculum_ID = this.Curriculum_ID,
                    Curriculum_Name_KH = this.Curriculum_Name_KH,
                    Curriculum_Name_EN = this.Curriculum_Name_EN,
                    Curriculum_Skill_ID = SelectedCurriculum_Skill_ID.Curriculum_Skill_ID,
                    Curriculum_Level_ID = SelectedCurriculum_Level_ID.Curriculum_Level_ID,
                    Curriculum_Teacher_ID = SelectedCurriculum_Teacher_ID.Curriculum_Teacher_ID,
                    Curriculum_Study_Year = this.Curriculum_Study_Year,
                    Curriculum_Semester = this.Curriculum_Semester,
                    Curriculum_Total_Time = this.Curriculum_Total_Time,
                    Curriculum_Total_Score = this.Curriculum_Total_Score
                };

                bool success = _dbConnection.Insert_CurriculumInfomation(curriculum_Info);

                if (success)
                {
                    ErrorMessage = "កម្មវិធីសិក្សាមុខវិជ្ជា៖ " + Curriculum_Name_KH + " បានរក្សាទុកជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    ErrorMessage = "កម្មវិធីសិក្សាមុខវិជ្ជា៖ " + Curriculum_Name_KH + " រក្សាទុកបរាជ៏យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                ClearTextBox_Curriculum();
                Get_CurriculumID();
                _ = LoadCurriculum_ListView(SearchCurriculumInfo);
            }

            await Task.CompletedTask;
        }

        //Search Curriculum Textbox.
        private string _search_curriculum_info;
        public string SearchCurriculumInfo
        {
            get => _search_curriculum_info;
            set
            {
                if (_search_curriculum_info != value)
                {
                    _search_curriculum_info = value;
                    OnPropertyChanged(nameof(SearchCurriculumInfo));
                    Debug.WriteLine($" Text Search: {value}");
                    OnSearchTextChanged_Curriculum_Info(_search_curriculum_info);
                }
            }
        }
        private async void OnSearchTextChanged_Curriculum_Info(string newText_SearchCurriculum)
        {
            Debug.WriteLine($"Search Text Curriculum Info: {newText_SearchCurriculum}");
            await LoadCurriculum_ListView(newText_SearchCurriculum);
        }

        //Load Curriculum List
        public async Task LoadCurriculum_ListView(string newText_SearchCurriculum)
        {
            if (string.IsNullOrEmpty(newText_SearchCurriculum))
            {
                Debug.WriteLine("Search Text Null.");

                var curriculum_list = _dbConnection.GetFetchCurriculum_Info(newText_SearchCurriculum);
                Curriculum_Info_List.Clear();
                foreach (var curriculum_info in curriculum_list)
                {
                    Curriculum_Info_List.Add(curriculum_info);
                }
                return;
            }
            IsLoading = true;
            Debug.WriteLine($"Start Loading curriculum: {IsLoading}");

            try
            {
                await Task.Delay(10);
                var curriculum_list = _dbConnection.GetFetchCurriculum_Info(newText_SearchCurriculum);
                Curriculum_Info_List.Clear();
                foreach (var curriculum_info in curriculum_list)
                {
                    Curriculum_Info_List.Add(curriculum_info);
                }
                Curriculum_Info_List = new ObservableCollection<Curriculum_Info>(curriculum_list);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error curriculum loading {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                Debug.WriteLine($"Loading curriculum ends: {IsLoading}");
            }
            await Task.CompletedTask;
        }
        //Multi Selection Export
        private List<Curriculum_Info> _selected_multi_export;
        public List<Curriculum_Info> Multi_Selected_Curriculum_Export
        {
            get => _selected_multi_export;
            set
            {
                _selected_multi_export = value;
                OnPropertyChanged(nameof(Multi_Selected_Curriculum_Export));
            }
        }

        //Selection Curriculum List
        private List<Curriculum_Info> _selected_Curriculum;
        public List<Curriculum_Info> Multi_Selected_Curriculum
        {
            get => _selected_Curriculum;
            set
            {
                _selected_Curriculum = value;
                OnPropertyChanged(nameof(Multi_Selected_Curriculum));
            }
        }
        //First Selected Curriculum
        private Curriculum_Info _first_select_Curriculum;
        public Curriculum_Info First_Select_Curriculum
        {
            get => _first_select_Curriculum;
            set
            {
                _first_select_Curriculum = value;
                OnPropertyChanged(nameof(First_Select_Curriculum));
            }
        }
        private string _Text_Year;
        public string Text_Year
        {
            get => _Text_Year;
            set
            {
                _Text_Year = value;
                OnPropertyChanged(nameof(Text_Year));
            }
        }


        //Command Edit, Delete, Clear
        public ICommand Command_Edit_Curriculum { get; set; }
        public ICommand Command_Delete_Curriculum { get; set; }
        public ICommand Command_Clear_Curriculum { get; set; }
        public ICommand Command_Clear_Search_Year { get; set; }

        //Method Clear Search Year
        public async Task Clear_Search_Year()
        {
            Curriculum_Search_Study_Year = null;
            Text_Year = "1,2,3,4";

            await Task.CompletedTask;
        }
        //Method Clear Curriculum
        public async Task Clear_Curriculum_Info()
        {
            Curriculum_Name_KH = null;
            Curriculum_Name_EN = null;

            SelectedCurriculum_Skill_ID = CurriculumSkill_Combobox
                .FirstOrDefault(skill_curriculum => skill_curriculum.Curriculum_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedCurriculum_Skill_ID));
            SelectedCurriculum_Teacher_ID = CurriculumTeacher_Combobox
                .FirstOrDefault(teacher_curriculum => teacher_curriculum.Curriculum_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedCurriculum_Teacher_ID));
            SelectedCurriculum_Level_ID = CurriculumLevel_Combobox
                .FirstOrDefault(level_curriculum => level_curriculum.Curriculum_Level_Name == null);
            OnPropertyChanged(nameof(SelectedCurriculum_Level_ID));

            Curriculum_Study_Year = null;
            Curriculum_Semester = null;
            Curriculum_Total_Time = 0;
            Curriculum_Total_Score = 0;

            ErrorMessage = string.Empty;
            ErrorMessage_Delete = string.Empty;
            ErrorImageSource = null;
            ErrorImageSource_Delete = null;
            MessageColor = new SolidColorBrush(Colors.Transparent);
            MessageColor_Delete = new SolidColorBrush(Colors.Transparent);

            //Enable Button
            IsInsertEnabled = true;
            IsUpdateEnabled = false;

            Get_CurriculumID();

            await Task.CompletedTask;
        }

        //Clear TextBox
        private void ClearTextBox_Curriculum()
        {
            Curriculum_ID = null;
            Curriculum_Name_KH = null;
            Curriculum_Name_EN = null;

            SelectedCurriculum_Skill_ID = CurriculumSkill_Combobox
                .FirstOrDefault(skill_curriculum => skill_curriculum.Curriculum_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedCurriculum_Skill_ID));
            SelectedCurriculum_Teacher_ID = CurriculumTeacher_Combobox
                .FirstOrDefault(teacher_curriculum => teacher_curriculum.Curriculum_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedCurriculum_Teacher_ID));
            SelectedCurriculum_Level_ID = CurriculumLevel_Combobox
                .FirstOrDefault(level_curriculum => level_curriculum.Curriculum_Level_Name == null);
            OnPropertyChanged(nameof(SelectedCurriculum_Level_ID));

            Curriculum_Study_Year = null;
            Curriculum_Semester = null;
            Curriculum_Total_Time = 0;
            Curriculum_Total_Score = 0;
        }

        //Method Edit Curriculum
        public async Task Edit_Curriculum_Info()
        {
            if (First_Select_Curriculum == null)
            {
                ErrorMessage = "សូមជ្រើសរើសទិន្នន័យមុខវិជ្ជាក្នុងកម្មវិធីសិក្សាជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            Debug.WriteLine($"First Select Curriculum: {First_Select_Curriculum.Curriculum_Name_KH}");

            //Load to Box
            Curriculum_ID = First_Select_Curriculum.Curriculum_ID;
            Curriculum_Name_KH = First_Select_Curriculum.Curriculum_Name_KH;
            Curriculum_Name_EN = First_Select_Curriculum.Curriculum_Name_EN;
            SelectedCurriculum_Skill_ID = CurriculumSkill_Combobox
                .FirstOrDefault(skill_curriculum => skill_curriculum.Curriculum_Skill_Name == First_Select_Curriculum.Curriculum_Skill_Name);
            OnPropertyChanged(nameof(SelectedCurriculum_Skill_ID));
            SelectedCurriculum_Teacher_ID = CurriculumTeacher_Combobox
                .FirstOrDefault(teacher_curriculum => teacher_curriculum.Curriculum_Teacher_Name == First_Select_Curriculum.Curriculum_Teacher_Name);
            OnPropertyChanged(nameof(SelectedCurriculum_Teacher_ID));
            SelectedCurriculum_Level_ID = CurriculumLevel_Combobox
                .FirstOrDefault(level_curriculum => level_curriculum.Curriculum_Level_Name == First_Select_Curriculum.Curriculum_Level_Name);
            OnPropertyChanged(nameof(SelectedCurriculum_Level_ID));

            Curriculum_Study_Year = First_Select_Curriculum.Curriculum_Study_Year;
            Curriculum_Semester = First_Select_Curriculum.Curriculum_Semester;
            Curriculum_Total_Time = First_Select_Curriculum.Curriculum_Total_Time;
            Curriculum_Total_Score = First_Select_Curriculum.Curriculum_Total_Score;

            //Enable Button
            IsInsertEnabled = false;
            IsUpdateEnabled = true;

            await Task.CompletedTask;
        }

        //Method Get_CurriculumID
        private void Get_CurriculumID()
        {
            var (id, curr_id) = _dbConnection.Get_CurriculumID();
            C_ID = id;
            Curriculum_ID = curr_id;
            OnPropertyChanged(nameof(Curriculum_ID));
        }

        //Method Delete Curriculum_Info
        public async Task Delete_Curriculum_Info()
        {
            if (Multi_Selected_Curriculum == null || !Multi_Selected_Curriculum.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសទិន្នន័យមុខវិជ្ជាក្នុងកម្មវិធីសិក្សាជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                ErrorMessage_Delete = "តើអ្នកពិតជាចង់លុបទិន្នន័យទាំងនេះមែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Yellow);
                CurrentOperation = "Delete_Curriculum";
            }

            await Task.CompletedTask;
        }

        //Click Yes , No
        public void HandleYesResponse()
        {

            Debug.WriteLine("Yes response handled in ViewModel");
            Debug.WriteLine("Delete Mode.");

            foreach (var curriculum_id in Multi_Selected_Curriculum)
            {
                bool success = _dbConnection.Delete_Curriculum_Info(curriculum_id.Curriculum_ID);
                if (success)
                {
                    Debug.WriteLine($"Delete success ID: {curriculum_id.Curriculum_ID}");
                    Get_CurriculumID();
                }
                else
                {
                    Debug.WriteLine("Delete curriculum failed.");
                    break;
                }
                _ = Clear_Curriculum_Info();
                _ = LoadCurriculum_ListView(SearchCurriculumInfo);

                ErrorMessage = "ទិន្នន័យបានលុបដោយជោគជ័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);
            }
        }

        //Search Table Curriculum TextBox
        private string _Curriculum_Skill_Name;
        public string Curriculum_Skill_Name
        {
            get => _Curriculum_Skill_Name;
            set
            {
                _Curriculum_Skill_Name = value;
                OnPropertyChanged(nameof(Curriculum_Skill_Name));
            }
        }
        private Curriculum_Info _Selected_Search_Curriculum_Skill_ID;
        public Curriculum_Info Selected_Search_Curriculum_Skill_ID
        {
            get => _Selected_Search_Curriculum_Skill_ID;
            set
            {
                _Selected_Search_Curriculum_Skill_ID = value;
                OnPropertyChanged(nameof(Selected_Search_Curriculum_Skill_ID));

                if (Selected_Search_Curriculum_Skill_ID == null)
                {
                    Curriculum_Skill_Name = null;
                }
                else
                {
                    Curriculum_Skill_Name = Selected_Search_Curriculum_Skill_ID.Curriculum_Skill_Name;
                }
                OnPropertyChanged(nameof(Curriculum_Skill_Name));
                _ = OnSearchTextChanged_Curriculum_Info_Table(Curriculum_Skill_Name, Curriculum_Level_Name, Curriculum_Search_Study_Year);
            }
        }
        private string _Curriculum_Level_Name;
        public string Curriculum_Level_Name
        {
            get => _Curriculum_Level_Name;
            set
            {
                _Curriculum_Level_Name = value;
                OnPropertyChanged(nameof(Curriculum_Level_Name));
            }
        }
        private Curriculum_Info _Selected_Search_Curriculum_Level_ID;
        public Curriculum_Info Selected_Search_Curriculum_Level_ID
        {
            get => _Selected_Search_Curriculum_Level_ID;
            set
            {
                _Selected_Search_Curriculum_Level_ID = value;
                OnPropertyChanged(nameof(Selected_Search_Curriculum_Level_ID));
                if (Selected_Search_Curriculum_Level_ID == null)
                {
                    Curriculum_Level_Name = null;
                }
                else
                {
                    Curriculum_Level_Name = Selected_Search_Curriculum_Level_ID.Curriculum_Level_Name;
                }
                OnPropertyChanged(nameof(Curriculum_Level_Name));
                _ = OnSearchTextChanged_Curriculum_Info_Table(Curriculum_Skill_Name, Curriculum_Level_Name, Curriculum_Search_Study_Year);
            }
        }
        private string _Curriculum_Search_Study_Year;
        public string Curriculum_Search_Study_Year
        {
            get => _Curriculum_Search_Study_Year;
            set
            {
                _Curriculum_Search_Study_Year = value;
                OnPropertyChanged(nameof(Curriculum_Search_Study_Year));

                if (Curriculum_Search_Study_Year == null)
                {
                    Search_Study_Year_Curr = null;
                    Text_Year = "1,2,3,4";
                }
                else
                {
                    Search_Study_Year_Curr = Curriculum_Search_Study_Year;
                    Text_Year = "";
                }
                OnPropertyChanged(nameof(Text_Year));
                OnPropertyChanged(nameof(Search_Study_Year_Curr));
                _ = OnSearchTextChanged_Curriculum_Info_Table(Curriculum_Skill_Name, Curriculum_Level_Name, Curriculum_Search_Study_Year);
            }
        }
        private string _Search_Study_Year_Curr;
        public string Search_Study_Year_Curr
        {
            get => _Search_Study_Year_Curr;
            set
            {
                _Search_Study_Year_Curr = value;
                OnPropertyChanged(nameof(Search_Study_Year_Curr));
            }
        }
        private int _Class_In_Level_ID;
        public int Class_In_Level_ID
        {
            get => _Class_In_Level_ID;
            set
            {
                _Class_In_Level_ID = value;
                OnPropertyChanged(nameof(Class_In_Level_ID));
            }
        }

        private async Task OnSearchTextChanged_Curriculum_Info_Table(string Curriculum_Skill_Name, string Curriculum_Level_Name, string Curriculum_Search_Study_Year)
        {
            try
            {
                Debug.WriteLine($"Level Name: {Curriculum_Level_Name}");
                Debug.WriteLine($"Skill Name: {Curriculum_Skill_Name}");
                Debug.WriteLine($"Study Year: {Curriculum_Search_Study_Year}");
                await LoadCurriculum_ListView_Table(Curriculum_Level_Name, Curriculum_Skill_Name, Curriculum_Search_Study_Year);
                await LoadCurriculum_TotalTime_ListView(Curriculum_Level_Name, Curriculum_Skill_Name, Curriculum_Search_Study_Year);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }

        }

        //Load Curriculum List Table
        public async Task LoadCurriculum_ListView_Table(string Curriculum_Level_Name, string Curriculum_Skill_Name, string Search_Study_Year_Curr)
        {
            if (string.IsNullOrEmpty(Search_Study_Year_Curr))
            {
                Debug.WriteLine("You Select all Study Year 1,2,3,4.");

                var curriculum_table = _dbConnection.GetFetchCurriculum_Table_Info(Curriculum_Level_Name, Curriculum_Skill_Name, Search_Study_Year_Curr);

                if (curriculum_table == null)
                {
                    Debug.WriteLine("No data returned from the database.");
                }

                Curriculum_Info_List_Table.Clear();
                foreach (var curriculum_info in curriculum_table)
                {
                    Curriculum_Info_List_Table.Add(curriculum_info);
                }
                return;
            }


            try
            {
                await Task.Delay(10);
                var curriculum_table = _dbConnection.GetFetchCurriculum_Table_Info(Curriculum_Level_Name, Curriculum_Skill_Name, Search_Study_Year_Curr);
                Curriculum_Info_List_Table.Clear();
                foreach (var curriculum_info in curriculum_table)
                {
                    Curriculum_Info_List_Table.Add(curriculum_info);
                }
                Curriculum_Info_List_Table = new ObservableCollection<Curriculum_Info>(curriculum_table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            await Task.CompletedTask;
        }

        //Load Curriculum TotalTime Table
        public async Task LoadCurriculum_TotalTime_ListView(string Curriculum_Level_Name, string Curriculum_Skill_Name, string Search_Study_Year_Curr)
        {
            if (string.IsNullOrEmpty(Search_Study_Year_Curr))
            {
                Debug.WriteLine("You Select all Study Year 1,2,3,4.");

                var curriculum_totaltime = _dbConnection.GetFetchCurriculum_TotalTime_Info(Curriculum_Level_Name, Curriculum_Skill_Name, Search_Study_Year_Curr);

                if (curriculum_totaltime == null)
                {
                    Debug.WriteLine("No data returned from the database.");
                }

                Curriculum_TotalTime_List_Table.Clear();
                foreach (var curriculum_info in curriculum_totaltime)
                {
                    Curriculum_TotalTime_List_Table.Add(curriculum_info);
                }
                return;
            }


            try
            {
                await Task.Delay(10);
                var curriculum_totaltime = _dbConnection.GetFetchCurriculum_TotalTime_Info(Curriculum_Level_Name, Curriculum_Skill_Name, Search_Study_Year_Curr);
                Curriculum_TotalTime_List_Table.Clear();
                foreach (var curriculum_info in curriculum_totaltime)
                {
                    Curriculum_TotalTime_List_Table.Add(curriculum_info);
                }
                Curriculum_TotalTime_List_Table = new ObservableCollection<Curriculum_Info>(curriculum_totaltime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            await Task.CompletedTask;
        }

        //Command Search Curriculum Info
        public ICommand Command_Search_Currculum { get; set; }

        //Method to Select Curriculum Table
        public async Task FetchCurriculum_Info_Table_TotalTime()
        {
            if (string.IsNullOrEmpty(Search_Study_Year_Curr))
            {
                var curriculum_table = _dbConnection.GetFetchCurriculum_Table_Info(Curriculum_Level_Name, Curriculum_Skill_Name, Search_Study_Year_Curr);
                var curriculum_totaltime = _dbConnection.GetFetchCurriculum_TotalTime_Info(Curriculum_Level_Name, Curriculum_Skill_Name, Search_Study_Year_Curr);

                Curriculum_Info_List_Table.Clear();
                foreach (var curriculum_info in curriculum_table)
                {
                    Curriculum_Info_List_Table.Add(curriculum_info);
                }

                Curriculum_TotalTime_List_Table.Clear();
                foreach (var curriculum_info in curriculum_totaltime)
                {
                    Curriculum_TotalTime_List_Table.Add(curriculum_info);
                }
                return;
            }

            try
            {
                await Task.Delay(10);
                var curriculum_totaltime = _dbConnection.GetFetchCurriculum_TotalTime_Info(Curriculum_Level_Name, Curriculum_Skill_Name, Search_Study_Year_Curr);
                var curriculum_table = _dbConnection.GetFetchCurriculum_Table_Info(Curriculum_Level_Name, Curriculum_Skill_Name, Search_Study_Year_Curr);

                Curriculum_TotalTime_List_Table.Clear();
                foreach (var curriculum_info1 in curriculum_totaltime)
                {
                    Curriculum_TotalTime_List_Table.Add(curriculum_info1);
                }
                Curriculum_TotalTime_List_Table = new ObservableCollection<Curriculum_Info>(curriculum_totaltime);

                Curriculum_Info_List_Table.Clear();
                foreach (var curriculum_info in curriculum_table)
                {
                    Curriculum_Info_List_Table.Add(curriculum_info);
                }
                Curriculum_Info_List_Table = new ObservableCollection<Curriculum_Info>(curriculum_table);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            await Task.CompletedTask;
        }

        //Command Export Currriculum to PDf
        public ICommand Command_Export_Curriclum_To_PDF { get; set; }
        //Command Export Curriculum to Excel
        public ICommand Command_Export_Curricum_To_Excel { get; set; }

        //Method Export to Excel
        public async Task Export_Curriculum_Info_to_Excel()
        {
            if (Multi_Selected_Curriculum_Export == null || !Multi_Selected_Curriculum_Export.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសទិន្នន័យមុខវិជ្ជាក្នុងកម្មវិធីសិក្សាជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                ErrorMessage_Delete = "តើអ្នកពិតជាចង់យកទិន្នន័យទាំងនេះ ចេញជាឯកសារប្រភេទ Excel មែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Yellow);
                CurrentOperation = "Export_Curriculum_Excel";
                OnPropertyChanged(nameof(CurrentOperation));
            }

            await Task.CompletedTask;
        }

        //Method Export to PDF
        public async Task Export_Curriculum_Info_to_PDF()
        {
            if (Multi_Selected_Curriculum_Export == null || !Multi_Selected_Curriculum_Export.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសទិន្នន័យមុខវិជ្ជាក្នុងកម្មវិធីសិក្សាជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                ErrorMessage_Delete = "តើអ្នកពិតជាចង់យកទិន្នន័យទាំងនេះ ចេញជាឯកសារប្រភេទ PDF មែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Yellow);
                CurrentOperation = "Export_Curriculum_PDF";
                OnPropertyChanged(nameof(CurrentOperation));
            }

            await Task.CompletedTask;
        }

        //Method Export Curr.. PDF Click Yes   
        public void HandleYesResponseExport_Curriculum_PDF()
        {

            Debug.WriteLine("Export Yes.");

            string curriculum_skill_select = Selected_Search_Curriculum_Skill_ID.Curriculum_Skill_Name;
            string curriculum_level_select = Selected_Search_Curriculum_Level_ID.Curriculum_Level_Name;
            string curriculum_study_year_select;
            if (Search_Study_Year_Curr == null)
            {
                curriculum_study_year_select = "1,2,3,4";
            }
            else
            {
                curriculum_study_year_select = Search_Study_Year_Curr;
            }

            //File Curriculum_toPDF.
            PDFService_Generate_Curriculum_Info.CreateReport(Multi_Selected_Curriculum_Export, curriculum_skill_select, curriculum_level_select, curriculum_study_year_select);
            ErrorMessage = "ឯកសារ PDF ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Method Export Curr.. Excel Click Yes
        public void HandleYesResponseExport_Curriculum_Excel()
        {
            Debug.WriteLine("Export to Excel.");
            string curriculum_skill_select = Selected_Search_Curriculum_Skill_ID.Curriculum_Skill_Name;
            string curriculum_level_select = Selected_Search_Curriculum_Level_ID.Curriculum_Level_Name;
            string curriculum_study_year_select;
            if (Search_Study_Year_Curr == null)
            {
                curriculum_study_year_select = "1,2,3,4";
            }
            else
            {
                curriculum_study_year_select = Search_Study_Year_Curr;
            }
            Export_Excel_Curriculum_Info.ExportToExcel(Multi_Selected_Curriculum_Export, curriculum_skill_select, curriculum_level_select, curriculum_study_year_select);
            ErrorMessage = "ឯកសារ Excel ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Command add schedule
        public ICommand Command_Add_Class_to_List_in_Schedule { get; set; }

        //Method add class to List in Schedule
        public async Task Load_Class_ToList_in_Schedule()
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
                Class_Info_List_Selected_In_Schedule.Clear();
                foreach (var classes_edit in SelectedClasses_Prepare_All)
                {

                    Class_Info_List_Selected_In_Schedule.Add(new Student_Info
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

        private Student_Info _selected_class_in_schedule_list;
        public Student_Info Selected_class_in_Schedule_List
        {
            get => _selected_class_in_schedule_list;
            set
            {
                _selected_class_in_schedule_list = value;
                OnPropertyChanged(nameof(Selected_class_in_Schedule_List));
                //Class_In_Study_Timeshift = Selected_class_in_Schedule_List.Class_In_Study_Timeshift;
                if (_selected_class_in_schedule_list != null)
                {
                    Class_ID = Selected_class_in_Schedule_List.Class_ID;
                    Class_Name = Selected_class_in_Schedule_List.Class_Name;
                    Class_In_Study_Year = Selected_class_in_Schedule_List.Class_In_Study_Year;
                    Class_In_Skill = Selected_class_in_Schedule_List.Class_In_Skill;
                    Class_In_Level = Selected_class_in_Schedule_List.Class_In_Level;
                    Class_In_Student_Year = Selected_class_in_Schedule_List.Class_In_Student_Year;
                    Class_In_Semester = Selected_class_in_Schedule_List.Class_In_Semester;
                    Class_In_Generation = Selected_class_in_Schedule_List.Class_In_Generation;
                    Class_In_Study_Timeshift = Selected_class_in_Schedule_List.Class_In_Study_Timeshift;
                    Class_In_Study_Type = Selected_class_in_Schedule_List.Class_In_Study_Type;
                    Max_Student_InClass = Selected_class_in_Schedule_List.Max_Student_InClass;
                    Current_Class_State = Selected_class_in_Schedule_List.Current_Class_State;
                    Class_ID_Schedule = int.Parse(Class_ID);
                    SD_Class_Name = Class_Name;
                    Debug.WriteLine($"Class Schedule ID: {Class_ID}");
                    Debug.WriteLine($"Class Schedule Name: {Class_Name}");
                    _ = Count_Student_Selected_Class();
                    _ = LoadSchedule(Class_ID_Schedule);
                    _ = Load_Schedule_Sat_Sun_ToList(Class_ID);


                }
                OnPropertyChanged(nameof(Class_In_Study_Timeshift));
                if (Class_In_Study_Timeshift == "វេនចន្ទសុក្រ (ព្រឹក)")
                {
                    //SetTime
                    SD_Start_DateTime_MF1 = TimeSpan.Parse("07:30:00");
                    SD_End_DateTime_MF1 = TimeSpan.Parse("09:30:00");
                    SD_Start_DateTime_MF2 = TimeSpan.Parse("09:45:00");
                    SD_End_DateTime_MF2 = TimeSpan.Parse("11:45:00");
                }
                if (Class_In_Study_Timeshift == "វេនចន្ទសុក្រ (រសៀល)")
                {
                    //SetTime
                    SD_Start_DateTime_MF1 = TimeSpan.Parse("13:00:00");
                    SD_End_DateTime_MF1 = TimeSpan.Parse("14:50:00");
                    SD_Start_DateTime_MF2 = TimeSpan.Parse("15:05:00");
                    SD_End_DateTime_MF2 = TimeSpan.Parse("17:00:00");
                }
                if (Class_In_Study_Timeshift == "វេនសៅរ៍អាទិត្យ")
                {
                    //SetTime
                    SD_Start_DateTime_SS1 = TimeSpan.Parse("07:30:00");
                    SD_End_DateTime_SS1 = TimeSpan.Parse("11:30:00");
                    SD_Start_DateTime_SS2 = TimeSpan.Parse("13:00:00");
                    SD_End_DateTime_SS2 = TimeSpan.Parse("17:00:00");
                }
                LoadData_to_Combobox_Schedule_Skill_Name_Combobox(Class_In_Skill, Class_In_Level, Class_In_Student_Year, Class_In_Semester);

            }
        }

        //Command Clear Class in Schedule
        public ICommand CommandClear_Class_in_Schedule { get; set; }

        //Method
        public async Task Clear_Class_in_Schedule_List()
        {
            if (Selected_class_in_Schedule_List == null)
            {
                Debug.WriteLine("No Selection");
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            // Remove the selected item from the collection
            if (Class_Info_List_Selected_In_Schedule.Contains(Selected_class_in_Schedule_List))
            {
                Class_Info_List_Selected_In_Schedule.Remove(Selected_class_in_Schedule_List);
                Class_ID = null;
                Class_In_Study_Year = null;
                Class_In_Skill = null;
                Class_In_Level = null;
                Class_In_Semester = null;
                Class_In_Generation = null;
                Class_In_Study_Type = null;
                Class_In_Student_Year = null;
                Class_In_Study_Timeshift = null;
                Class_Name = null;
                Total_Count_Students_Class = null;
                Total_Count_Female_Class = null;
                Max_Student_InClass = 0;
                Current_Student_InClass = 0;
                Current_Class_State = null;
                Schedule_State = null;
                Schedule_ID = 0;
            }
            OnPropertyChanged(nameof(Selected_class_in_Schedule_List));
            // Clear the selection
            Selected_class_in_Schedule_List = null;
            List_Students_Display.Clear();
            List_Student_In_Class_Display.Clear();
            _ = ClearSchedule();
            Debug.WriteLine("Clear Class in Schedule Success.");

            // Provide feedback to the user
            ErrorMessage = "ថ្នាក់ជ្រើសរើសត្រូវបានដកចេញជោគជ័យ!";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);

            await Task.CompletedTask;
        }

        //Schedule

        private int _Class_ID_Schedule;
        public int Class_ID_Schedule
        {
            get => _Class_ID_Schedule;
            set
            {
                _Class_ID_Schedule = value;
                OnPropertyChanged(nameof(Class_ID_Schedule));
            }
        }
        private TimeSpan? _SD_Start_DateTime_MF1;
        public TimeSpan? SD_Start_DateTime_MF1
        {
            get => _SD_Start_DateTime_MF1;
            set
            {
                if (_SD_Start_DateTime_MF1 != value)
                {
                    _SD_Start_DateTime_MF1 = value;
                    Debug.WriteLine($"SD_Start_DateTime_MF1 updated to: {_SD_Start_DateTime_MF1}");
                    OnPropertyChanged(nameof(SD_Start_DateTime_MF1));
                }
            }
        }
        private TimeSpan? _SD_Start_DateTime_SS1;
        public TimeSpan? SD_Start_DateTime_SS1
        {
            get => _SD_Start_DateTime_SS1;
            set
            {
                if (_SD_Start_DateTime_SS1 != value)
                {
                    _SD_Start_DateTime_SS1 = value;
                    Debug.WriteLine($"SD_Start_DateTime_SS1 updated to: {_SD_Start_DateTime_MF1}");
                    OnPropertyChanged(nameof(SD_Start_DateTime_SS1));
                }
            }
        }

        private TimeSpan? _SD_End_DateTime_MF1;
        public TimeSpan? SD_End_DateTime_MF1
        {
            get => _SD_End_DateTime_MF1;
            set
            {
                if (_SD_End_DateTime_MF1 != value)
                {
                    _SD_End_DateTime_MF1 = value;
                    OnPropertyChanged(nameof(SD_End_DateTime_MF1));
                }
            }
        }
        private TimeSpan? _SD_End_DateTime_SS1;
        public TimeSpan? SD_End_DateTime_SS1
        {
            get => _SD_End_DateTime_SS1;
            set
            {
                if (_SD_End_DateTime_SS1 != value)
                {
                    _SD_End_DateTime_SS1 = value;
                    OnPropertyChanged(nameof(SD_End_DateTime_SS1));
                }
            }
        }
        private TimeSpan? _SD_Start_DateTime_MF2;
        public TimeSpan? SD_Start_DateTime_MF2
        {
            get => _SD_Start_DateTime_MF2;
            set
            {
                _SD_Start_DateTime_MF2 = value;
                OnPropertyChanged(nameof(SD_Start_DateTime_MF2));
            }
        }
        private TimeSpan? _SD_Start_DateTime_SS2;
        public TimeSpan? SD_Start_DateTime_SS2
        {
            get => _SD_Start_DateTime_SS2;
            set
            {
                _SD_Start_DateTime_SS2 = value;
                OnPropertyChanged(nameof(SD_Start_DateTime_SS2));
            }
        }

        private TimeSpan? _SD_End_DateTime_MF2;
        public TimeSpan? SD_End_DateTime_MF2
        {
            get => _SD_End_DateTime_MF2;
            set
            {
                _SD_End_DateTime_MF2 = value;
                OnPropertyChanged(nameof(SD_End_DateTime_MF2));
            }
        }
        private TimeSpan? _SD_End_DateTime_SS2;
        public TimeSpan? SD_End_DateTime_SS2
        {
            get => _SD_End_DateTime_SS2;
            set
            {
                _SD_End_DateTime_SS2 = value;
                OnPropertyChanged(nameof(SD_End_DateTime_SS2));
            }
        }

        public string DateTime_Start_Schedule_Strating
        {
            get => SelectedDate?.ToString("dd/MM/yyyy") ?? "No Date Selected";
        }
        private int _SD_Skill_ID;
        public int SD_Skill_ID
        {
            get => _SD_Skill_ID;
            set
            {
                _SD_Skill_ID = value;
                OnPropertyChanged(nameof(SD_Skill_ID));
            }
        }
        private string _SD_Skill_Name;
        public string SD_Skill_Name
        {
            get => _SD_Skill_Name;
            set
            {
                _SD_Skill_Name = value;
                OnPropertyChanged(nameof(_SD_Skill_Name));
            }
        }
        private int _SD_Teacher_ID;
        public int SD_Teacher_ID
        {
            get => _SD_Teacher_ID;
            set
            {
                _SD_Teacher_ID = value;
                OnPropertyChanged(nameof(SD_Teacher_ID));
            }
        }
        private string _SD_Teacher_Name;
        public string SD_Teacher_Name
        {
            get => _SD_Teacher_Name;
            set
            {
                _SD_Teacher_Name = value;
                OnPropertyChanged(nameof(SD_Teacher_Name));
            }
        }
        private bool _Load_State;
        public bool Load_State
        {
            get => _Load_State;
            set
            {
                _Load_State = value;
                OnPropertyChanged(nameof(Load_State));
            }
        }
        private Class_Schedule _SelectedSkill_SD_Mon1;
        public Class_Schedule SelectedSkill_SD_Mon1
        {
            get => _SelectedSkill_SD_Mon1;
            set
            {
                _SelectedSkill_SD_Mon1 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Mon1));

                if (_SelectedSkill_SD_Mon1 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Mon1 = SelectedSkill_SD_Mon1.SD_Skill_Name;
                        Debug.WriteLine($"Skill Mon Select: {SD_Skill_Name_Mon1}");
                        LoadTeacher_andTime_BySelectSkill_Mon1(SD_Skill_Name_Mon1);
                    }
                }
            }
        }
        private string _SD_Skill_Name_Mon1;
        public string SD_Skill_Name_Mon1
        {
            get => _SD_Skill_Name_Mon1;
            set
            {
                _SD_Skill_Name_Mon1 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Mon1));
            }
        }
        private Class_Schedule _SelectedSkill_SD_Mon2;
        public Class_Schedule SelectedSkill_SD_Mon2
        {
            get => _SelectedSkill_SD_Mon2;
            set
            {
                _SelectedSkill_SD_Mon2 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Mon2));
                if (_SelectedSkill_SD_Mon2 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Mon2 = SelectedSkill_SD_Mon2.SD_Skill_Name;
                        Debug.WriteLine($"Skill Mon Select2: {SD_Skill_Name_Mon2}");
                        LoadTeacher_andTime_BySelectSkill_Mon2(SD_Skill_Name_Mon2);
                    }

                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Tues1;
        public Class_Schedule SelectedSkill_SD_Tues1
        {
            get => _SelectedSkill_SD_Tues1;
            set
            {
                _SelectedSkill_SD_Tues1 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Tues1));
                if (_SelectedSkill_SD_Tues1 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Tues1 = SelectedSkill_SD_Tues1.SD_Skill_Name;
                        Debug.WriteLine($"Skill Tues Select1: {SD_Skill_Name_Tues1}");
                        LoadTeacher_andTime_BySelectSkill_Tues1(SD_Skill_Name_Tues1);
                    }

                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Tues2;
        public Class_Schedule SelectedSkill_SD_Tues2
        {
            get => _SelectedSkill_SD_Tues2;
            set
            {
                _SelectedSkill_SD_Tues2 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Tues2));
                if (_SelectedSkill_SD_Tues2 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Tues2 = SelectedSkill_SD_Tues2.SD_Skill_Name;
                        Debug.WriteLine($"Skill Tues Select2: {SD_Skill_Name_Tues2}");
                        LoadTeacher_andTime_BySelectSkill_Tues2(SD_Skill_Name_Tues2);
                    }
                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Wed1;
        public Class_Schedule SelectedSkill_SD_Wed1
        {
            get => _SelectedSkill_SD_Wed1;
            set
            {
                _SelectedSkill_SD_Wed1 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Wed1));
                if (_SelectedSkill_SD_Wed1 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Wed1 = SelectedSkill_SD_Wed1.SD_Skill_Name;
                        Debug.WriteLine($"Skill Wed Select2: {SD_Skill_Name_Wed1}");
                        LoadTeacher_andTime_BySelectSkill_Wed1(SD_Skill_Name_Wed1);
                    }
                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Wed2;
        public Class_Schedule SelectedSkill_SD_Wed2
        {
            get => _SelectedSkill_SD_Wed2;
            set
            {
                _SelectedSkill_SD_Wed2 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Wed2));
                if (_SelectedSkill_SD_Wed2 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Wed2 = SelectedSkill_SD_Wed2.SD_Skill_Name;
                        Debug.WriteLine($"Skill Wed Select2: {SD_Skill_Name_Wed2}");
                        LoadTeacher_andTime_BySelectSkill_Wed2(SD_Skill_Name_Wed2);
                    }
                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Thur1;
        public Class_Schedule SelectedSkill_SD_Thur1
        {
            get => _SelectedSkill_SD_Thur1;
            set
            {
                _SelectedSkill_SD_Thur1 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Thur1));
                if (_SelectedSkill_SD_Thur1 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Thur1 = SelectedSkill_SD_Thur1.SD_Skill_Name;
                        Debug.WriteLine($"Skill Thurs Select1: {SD_Skill_Name_Thur1}");
                        LoadTeacher_andTime_BySelectSkill_Thur1(SD_Skill_Name_Thur1);
                    }
                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Thur2;
        public Class_Schedule SelectedSkill_SD_Thur2
        {
            get => _SelectedSkill_SD_Thur2;
            set
            {
                _SelectedSkill_SD_Thur2 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Thur2));
                if (_SelectedSkill_SD_Thur2 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Thur2 = SelectedSkill_SD_Thur2.SD_Skill_Name;
                        Debug.WriteLine($"Skill Thurs Select2: {SD_Skill_Name_Thur2}");
                        LoadTeacher_andTime_BySelectSkill_Thur2(SD_Skill_Name_Thur2);
                    }
                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Fri1;
        public Class_Schedule SelectedSkill_SD_Fri1
        {
            get => _SelectedSkill_SD_Fri1;
            set
            {
                _SelectedSkill_SD_Fri1 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Fri1));
                if (_SelectedSkill_SD_Fri1 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Fri1 = SelectedSkill_SD_Fri1.SD_Skill_Name;
                        Debug.WriteLine($"Skill Fri Select1: {SD_Skill_Name_Fri1}");
                        LoadTeacher_andTime_BySelectSkill_Fri1(SD_Skill_Name_Fri1);
                    }
                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Fri2;
        public Class_Schedule SelectedSkill_SD_Fri2
        {
            get => _SelectedSkill_SD_Fri2;
            set
            {
                _SelectedSkill_SD_Fri2 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Fri2));
                if (_SelectedSkill_SD_Fri2 != null)
                {
                    if (Schedule_State == "មិនមាន")
                    {
                        SD_Skill_Name_Fri2 = SelectedSkill_SD_Fri2.SD_Skill_Name;
                        Debug.WriteLine($"Skill Fri Select2: {SD_Skill_Name_Fri2}");
                        LoadTeacher_andTime_BySelectSkill_Fri2(SD_Skill_Name_Fri2);
                    }
                }
            }
        }
        private string _SD_Skill_Name_Fri2;
        public string SD_Skill_Name_Fri2
        {
            get => _SD_Skill_Name_Fri2;
            set
            {
                _SD_Skill_Name_Fri2 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Fri2));
            }
        }
        private string _SD_Skill_Name_Fri1;
        public string SD_Skill_Name_Fri1
        {
            get => _SD_Skill_Name_Fri1;
            set
            {
                _SD_Skill_Name_Fri1 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Fri1));
            }
        }
        private string _SD_Skill_Name_Thur2;
        public string SD_Skill_Name_Thur2
        {
            get => _SD_Skill_Name_Thur2;
            set
            {
                _SD_Skill_Name_Thur2 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Thur2));
            }
        }
        private string _SD_Skill_Name_Thur1;
        public string SD_Skill_Name_Thur1
        {
            get => _SD_Skill_Name_Thur1;
            set
            {
                _SD_Skill_Name_Thur1 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Thur1));
            }
        }
        private string _SD_Skill_Name_Wed2;
        public string SD_Skill_Name_Wed2
        {
            get => _SD_Skill_Name_Wed2;
            set
            {
                _SD_Skill_Name_Wed2 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Wed2));
            }
        }
        private string _SD_Skill_Name_Wed1;
        public string SD_Skill_Name_Wed1
        {
            get => _SD_Skill_Name_Wed1;
            set
            {
                _SD_Skill_Name_Wed1 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Wed1));
            }
        }
        private string _SD_Skill_Name_Tues1;
        public string SD_Skill_Name_Tues1
        {
            get => _SD_Skill_Name_Tues1;
            set
            {
                _SD_Skill_Name_Tues1 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Tues1));
            }
        }
        private string _SD_Skill_Name_Tues2;
        public string SD_Skill_Name_Tues2
        {
            get => _SD_Skill_Name_Tues2;
            set
            {
                _SD_Skill_Name_Tues2 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Tues2));
            }
        }
        private string _SD_Skill_Name_Mon2;
        public string SD_Skill_Name_Mon2
        {
            get => _SD_Skill_Name_Mon2;
            set
            {
                _SD_Skill_Name_Mon2 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Mon2));
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Mon1;
        public Class_Schedule SelectedTeacher_SD_Mon1
        {
            get => _SelectedTeacher_SD_Mon1;
            set
            {
                _SelectedTeacher_SD_Mon1 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Mon1));
                if (_SelectedTeacher_SD_Mon1 != value)
                {
                    SD_Teacher_Mon01 = SelectedTeacher_SD_Mon1.SD_Teacher_Name;
                    Debug.WriteLine($"Teacher name Mon select: {SD_Teacher_Mon01}");
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Mon2;
        public Class_Schedule SelectedTeacher_SD_Mon2
        {
            get => _SelectedTeacher_SD_Mon2;
            set
            {
                _SelectedTeacher_SD_Mon2 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Mon2));
                if (_SelectedTeacher_SD_Mon2 != value)
                {
                    SD_Teacher_Mon02 = SelectedTeacher_SD_Mon2.SD_Teacher_Name;
                    Debug.WriteLine($"Teacher name Mon select: {SD_Teacher_Mon02}");
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Tues1;
        public Class_Schedule SelectedTeacher_SD_Tues1
        {
            get => _SelectedTeacher_SD_Tues1;
            set
            {
                _SelectedTeacher_SD_Tues1 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Tues1));
                if (_SelectedTeacher_SD_Tues1 != value)
                {
                    SD_Skill_Name_Tues1 = SelectedTeacher_SD_Tues1.SD_Skill_Name;
                    Debug.WriteLine($"Teacher name Tues select: {SD_Skill_Name_Tues1}");
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Tues2;
        public Class_Schedule SelectedTeacher_SD_Tues2
        {
            get => _SelectedTeacher_SD_Tues2;
            set
            {
                _SelectedTeacher_SD_Tues2 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Tues2));
                if (_SelectedTeacher_SD_Tues2 != value)
                {
                    SD_Teacher_Tues02 = SelectedTeacher_SD_Tues2.SD_Teacher_Name;
                    Debug.WriteLine($"Teacher name Tues select: {SD_Teacher_Tues02}");
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Wed1;
        public Class_Schedule SelectedTeacher_SD_Wed1
        {
            get => _SelectedTeacher_SD_Wed1;
            set
            {
                _SelectedTeacher_SD_Wed1 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Wed1));
                if (_SelectedTeacher_SD_Wed1 != value)
                {
                    SD_Teacher_Wed1 = SelectedTeacher_SD_Wed1.SD_Teacher_Name;
                    Debug.WriteLine($"Teacher name Tues select: {SD_Teacher_Wed1}");
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Wed2;
        public Class_Schedule SelectedTeacher_SD_Wed2
        {
            get => _SelectedTeacher_SD_Wed2;
            set
            {
                _SelectedTeacher_SD_Wed2 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Wed2));
                if (_SelectedTeacher_SD_Wed2 != value)
                {
                    SD_Teacher_Wed2 = SelectedTeacher_SD_Wed2.SD_Teacher_Name;
                    Debug.WriteLine($"Teacher name Wed select: {SD_Teacher_Wed2}");
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Thur1;
        public Class_Schedule SelectedTeacher_SD_Thur1
        {
            get => _SelectedTeacher_SD_Thur1;
            set
            {
                _SelectedTeacher_SD_Thur1 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Thur1));
                if (_SelectedTeacher_SD_Thur1 != value)
                {
                    SD_Teacher_Thur1 = SelectedTeacher_SD_Thur1.SD_Teacher_Name;
                    Debug.WriteLine($"Teacher name Wed select: {SD_Teacher_Thur1}");
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Thur2;
        public Class_Schedule SelectedTeacher_SD_Thur2
        {
            get => _SelectedTeacher_SD_Thur2;
            set
            {
                _SelectedTeacher_SD_Thur2 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Thur2));
                if (_SelectedTeacher_SD_Thur2 != value)
                {
                    SD_Teacher_Thur2 = SelectedTeacher_SD_Thur2.SD_Teacher_Name;
                    Debug.WriteLine($"Teacher name Thur select: {SD_Teacher_Thur2}");
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Fri1;
        public Class_Schedule SelectedTeacher_SD_Fri1
        {
            get => _SelectedTeacher_SD_Fri1;
            set
            {
                _SelectedTeacher_SD_Fri1 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Fri1));
                if (_SelectedTeacher_SD_Fri1 != value)
                {
                    SD_Teacher_Fri1 = SelectedTeacher_SD_Fri1.SD_Teacher_Name;
                    Debug.WriteLine($"Teacher name Fri select: {SD_Teacher_Fri1}");
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Fri2;
        public Class_Schedule SelectedTeacher_SD_Fri2
        {
            get => _SelectedTeacher_SD_Fri2;
            set
            {
                _SelectedTeacher_SD_Fri2 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Fri2));
                if (_SelectedTeacher_SD_Fri2 != value)
                {
                    SD_Teacher_Fri2 = SelectedTeacher_SD_Fri2.SD_Teacher_Name;
                    Debug.WriteLine($"Teacher name Fri select2: {SD_Teacher_Fri2}");
                }
            }
        }
        private string _SD_Teacher_Mon01;
        public string SD_Teacher_Mon01
        {
            get => _SD_Teacher_Mon01;
            set
            {
                _SD_Teacher_Mon01 = value;
                OnPropertyChanged(nameof(SD_Teacher_Mon01));
            }
        }
        private string _SD_Teacher_Mon02;
        public string SD_Teacher_Mon02
        {
            get => _SD_Teacher_Mon02;
            set
            {
                _SD_Teacher_Mon02 = value;
                OnPropertyChanged(nameof(SD_Teacher_Mon02));
            }
        }
        private string _SD_Teacher_Tues01;
        public string SD_Teacher_Tues01
        {
            get => _SD_Teacher_Tues01;
            set
            {
                _SD_Teacher_Tues01 = value;
                OnPropertyChanged(nameof(SD_Teacher_Tues01));
            }
        }
        private string _SD_Teacher_Tues02;
        public string SD_Teacher_Tues02
        {
            get => _SD_Teacher_Tues02;
            set
            {
                _SD_Teacher_Tues02 = value;
                OnPropertyChanged(nameof(SD_Teacher_Tues02));
            }
        }
        private string _SD_Teacher_Wed1;
        public string SD_Teacher_Wed1
        {
            get => _SD_Teacher_Wed1;
            set
            {
                _SD_Teacher_Wed1 = value;
                OnPropertyChanged(nameof(SD_Teacher_Wed1));
            }
        }
        private string _SD_Teacher_Wed2;
        public string SD_Teacher_Wed2
        {
            get => _SD_Teacher_Wed2;
            set
            {
                _SD_Teacher_Wed2 = value;
                OnPropertyChanged(nameof(SD_Teacher_Wed2));
            }
        }
        private string _SD_Teacher_Thur1;
        public string SD_Teacher_Thur1
        {
            get => _SD_Teacher_Thur1;
            set
            {
                _SD_Teacher_Thur1 = value;
                OnPropertyChanged(nameof(SD_Teacher_Thur1));
            }
        }
        private string _SD_Teacher_Thur2;
        public string SD_Teacher_Thur2
        {
            get => _SD_Teacher_Thur2;
            set
            {
                _SD_Teacher_Thur2 = value;
                OnPropertyChanged(nameof(SD_Teacher_Thur2));
            }
        }
        private string _SD_Teacher_Fri1;
        public string SD_Teacher_Fri1
        {
            get => _SD_Teacher_Fri1;
            set
            {
                _SD_Teacher_Fri1 = value;
                OnPropertyChanged(nameof(SD_Teacher_Fri1));
            }
        }
        private string _SD_Teacher_Fri2;
        public string SD_Teacher_Fri2
        {
            get => _SD_Teacher_Fri2;
            set
            {
                _SD_Teacher_Fri2 = value;
                OnPropertyChanged(nameof(SD_Teacher_Fri2));
            }
        }
        private int _SD_TotalTime_Mon1;
        public int SD_TotalTime_Mon1
        {
            get => _SD_TotalTime_Mon1;
            set
            {
                _SD_TotalTime_Mon1 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Mon1));
            }
        }
        private int _SD_TotalTime_Mon2;
        public int SD_TotalTime_Mon2
        {
            get => _SD_TotalTime_Mon2;
            set
            {
                _SD_TotalTime_Mon2 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Mon2));
            }
        }
        private int _SD_TotalTime_Tues1;
        public int SD_TotalTime_Tues1
        {
            get => _SD_TotalTime_Tues1;
            set
            {
                _SD_TotalTime_Tues1 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Tues1));
            }
        }
        private int _SD_TotalTime_Tues2;
        public int SD_TotalTime_Tues2
        {
            get => _SD_TotalTime_Tues2;
            set
            {
                _SD_TotalTime_Tues2 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Tues2));
            }
        }
        private int _SD_TotalTime_Wed1;
        public int SD_TotalTime_Wed1
        {
            get => _SD_TotalTime_Wed1;
            set
            {
                _SD_TotalTime_Wed1 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Wed1));
            }
        }
        private int _SD_TotalTime_Wed2;
        public int SD_TotalTime_Wed2
        {
            get => _SD_TotalTime_Wed2;
            set
            {
                _SD_TotalTime_Wed2 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Wed2));
            }
        }
        private int _SD_TotalTime_Thur1;
        public int SD_TotalTime_Thur1
        {
            get => _SD_TotalTime_Thur1;
            set
            {
                _SD_TotalTime_Thur1 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Thur1));
            }
        }

        private int _SD_TotalTime_Thur2;
        public int SD_TotalTime_Thur2
        {
            get => _SD_TotalTime_Thur2;
            set
            {
                _SD_TotalTime_Thur2 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Thur2));
            }
        }
        private int _SD_TotalTime_Fri1;
        public int SD_TotalTime_Fri1
        {
            get => _SD_TotalTime_Fri1;
            set
            {
                _SD_TotalTime_Fri1 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Fri1));
            }
        }
        private int _SD_TotalTime_Fri2;
        public int SD_TotalTime_Fri2
        {
            get => _SD_TotalTime_Fri2;
            set
            {
                _SD_TotalTime_Fri2 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Fri2));
            }
        }

        //Load Data Teacher and Time
        private async void LoadTeacher_andTime_BySelectSkill_Mon1(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Mon1 != null)
            {
                //Monday1
                SelectedTeacher_SD_Mon1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_mon1 => teacher_mon1.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Mon1));

                SD_TotalTime_Mon1 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Mon1));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Mon2(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Mon2 != null)
            {
                //Monday2
                SelectedTeacher_SD_Mon2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_mon2 => teacher_mon2.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Mon2));

                SD_TotalTime_Mon2 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Mon2));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Tues1(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Tues1 != null)
            {
                //Tuesday01
                SelectedTeacher_SD_Tues1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_tues1 => teacher_tues1.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Tues1));

                SD_TotalTime_Tues1 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Tues1));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Tues2(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Tues2 != null)
            {
                //Tuesday01
                SelectedTeacher_SD_Tues2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_tues2 => teacher_tues2.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Tues2));

                SD_TotalTime_Tues2 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Tues2));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Wed1(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Wed1 != null)
            {
                //Wednesday01
                SelectedTeacher_SD_Wed1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_wed1 => teacher_wed1.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Wed1));

                SD_TotalTime_Wed1 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Wed1));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Wed2(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Wed2 != null)
            {
                //Wednesday02
                SelectedTeacher_SD_Wed2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_wed2 => teacher_wed2.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Wed2));

                SD_TotalTime_Wed2 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Wed2));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Thur1(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Thur1 != null)
            {
                //Thursday01
                SelectedTeacher_SD_Thur1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_thurs1 => teacher_thurs1.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Thur1));

                SD_TotalTime_Thur1 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Thur1));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Thur2(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Thur2 != null)
            {
                //Thursday01
                SelectedTeacher_SD_Thur2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_thurs2 => teacher_thurs2.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Thur2));

                SD_TotalTime_Thur2 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Thur2));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Fri1(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Fri1 != null)
            {
                //Friday01
                SelectedTeacher_SD_Fri1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_fri1 => teacher_fri1.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Fri1));

                SD_TotalTime_Fri1 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Fri1));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Fri2(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Fri2 != null)
            {
                //Friday01
                SelectedTeacher_SD_Fri2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher_fri2 => teacher_fri2.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Fri2));

                SD_TotalTime_Fri2 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Fri2));
                return;
            }
        }

        private string _SD_Building_Name;
        public string SD_Building_Name
        {
            get => _SD_Building_Name;
            set
            {
                _SD_Building_Name = value;
                OnPropertyChanged(nameof(SD_Building_Name));
            }
        }
        private string _SD_Building_Room;
        public string SD_Building_Room
        {
            get => _SD_Building_Room;
            set
            {
                _SD_Building_Room = value;
                OnPropertyChanged(nameof(SD_Building_Room));
            }
        }
        private string _SD_Class_Name;
        public string SD_Class_Name
        {
            get => _SD_Class_Name;
            set
            {
                _SD_Class_Name = value;
                OnPropertyChanged(nameof(SD_Class_Name));
            }
        }
        private string _SD_Class_TimeShift;
        public string SD_Class_TimeShift
        {
            get => _SD_Class_TimeShift;
            set
            {
                _SD_Class_TimeShift = value;
                OnPropertyChanged(nameof(SD_Class_TimeShift));
            }
        }

        //Data to Combobox
        private ObservableCollection<Class_Schedule> _skill_class_schedule;
        public ObservableCollection<Class_Schedule> Schedule_Skill_Name_Combobox
        {
            get { return _skill_class_schedule; }
            set
            {
                _skill_class_schedule = value;
                OnPropertyChanged(nameof(Schedule_Skill_Name_Combobox));
            }
        }
        private ObservableCollection<Class_Schedule> _teacher_class_schedule;
        public ObservableCollection<Class_Schedule> Schedule_Teacher_Name_Combobox
        {
            get { return _teacher_class_schedule; }
            set
            {
                _teacher_class_schedule = value;
                OnPropertyChanged(nameof(Schedule_Teacher_Name_Combobox));
            }
        }
        private int _Schedule_ID;
        public int Schedule_ID
        {
            get => _Schedule_ID;
            set
            {
                _Schedule_ID = value;
                OnPropertyChanged(nameof(Schedule_ID));
            }
        }
        private string _Schedule_State;
        public string Schedule_State
        {
            get => _Schedule_State;
            set
            {
                _Schedule_State = value;
                OnPropertyChanged(nameof(Schedule_State));
            }
        }
        private string _Schedule_Name;
        public string Schedule_Name
        {
            get => _Schedule_Name;
            set
            {
                _Schedule_Name = value;
                OnPropertyChanged(nameof(Schedule_Name));
            }
        }
        private string _SD_Skill_Name_Sat1;
        public string SD_Skill_Name_Sat1
        {
            get => _SD_Skill_Name_Sat1;
            set
            {
                _SD_Skill_Name_Sat1 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Sat1));
            }
        }
        private string _SD_Skill_Name_Sat2;
        public string SD_Skill_Name_Sat2
        {
            get => _SD_Skill_Name_Sat2;
            set
            {
                _SD_Skill_Name_Sat2 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Sat2));
            }
        }
        private string _SD_Teacher_Sat1;
        public string SD_Teacher_Sat1
        {
            get => _SD_Teacher_Sat1;
            set
            {
                _SD_Teacher_Sat1 = value;
                OnPropertyChanged(nameof(SD_Teacher_Sat1));
            }
        }
        private string _SD_Teacher_Sat2;
        public string SD_Teacher_Sat2
        {
            get => _SD_Teacher_Sat2;
            set
            {
                _SD_Teacher_Sat2 = value;
                OnPropertyChanged(nameof(SD_Teacher_Sat2));
            }
        }
        private int _SD_TotalTime_Sat1;
        public int SD_TotalTime_Sat1
        {
            get => _SD_TotalTime_Sat1;
            set
            {
                _SD_TotalTime_Sat1 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Sat1));
            }
        }
        private int _SD_TotalTime_Sat2;
        public int SD_TotalTime_Sat2
        {
            get => _SD_TotalTime_Sat2;
            set
            {
                _SD_TotalTime_Sat2 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Sat2));
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Sat1;
        public Class_Schedule SelectedTeacher_SD_Sat1
        {
            get => _SelectedTeacher_SD_Sat1;
            set
            {
                _SelectedTeacher_SD_Sat1 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Sat1));
                if (_SelectedTeacher_SD_Sat1 != null)
                {
                    SD_Teacher_Sat1 = _SelectedTeacher_SD_Sat1.SD_Teacher_Name;
                    OnPropertyChanged(nameof(SD_Teacher_Sat1));
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Sat2;
        public Class_Schedule SelectedTeacher_SD_Sat2
        {
            get => _SelectedTeacher_SD_Sat2;
            set
            {
                _SelectedTeacher_SD_Sat2 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Sat2));
                if (_SelectedTeacher_SD_Sat2 != null)
                {
                    SD_Teacher_Sat2 = _SelectedTeacher_SD_Sat2.SD_Teacher_Name;
                    OnPropertyChanged(nameof(SD_Teacher_Sat2));
                }
            }
        }
        private string _SD_Skill_Name_Sun1;
        public string SD_Skill_Name_Sun1
        {
            get => _SD_Skill_Name_Sun1;
            set
            {
                _SD_Skill_Name_Sun1 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Sun1));
            }
        }
        private string _SD_Skill_Name_Sun2;
        public string SD_Skill_Name_Sun2
        {
            get => _SD_Skill_Name_Sun2;
            set
            {
                _SD_Skill_Name_Sun2 = value;
                OnPropertyChanged(nameof(SD_Skill_Name_Sun2));
            }
        }
        private string _SD_Teacher_Sun1;
        public string SD_Teacher_Sun1
        {
            get => _SD_Teacher_Sun1;
            set
            {
                _SD_Teacher_Sun1 = value;
                OnPropertyChanged(nameof(SD_Teacher_Sun1));
            }
        }
        private string _SD_Teacher_Sun2;
        public string SD_Teacher_Sun2
        {
            get => _SD_Teacher_Sun2;
            set
            {
                _SD_Teacher_Sun2 = value;
                OnPropertyChanged(nameof(SD_Teacher_Sun2));
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Sun1;
        public Class_Schedule SelectedTeacher_SD_Sun1
        {
            get => _SelectedTeacher_SD_Sun1;
            set
            {
                _SelectedTeacher_SD_Sun1 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Sun1));
                if (_SelectedTeacher_SD_Sun1 != null)
                {
                    SD_Teacher_Sun1 = _SelectedTeacher_SD_Sun1.SD_Teacher_Name;
                    OnPropertyChanged(nameof(SD_Teacher_Name));
                }
            }
        }
        private Class_Schedule _SelectedTeacher_SD_Sun2;
        public Class_Schedule SelectedTeacher_SD_Sun2
        {
            get => _SelectedTeacher_SD_Sun2;
            set
            {
                _SelectedTeacher_SD_Sun2 = value;
                OnPropertyChanged(nameof(SelectedTeacher_SD_Sun2));
                if (_SelectedTeacher_SD_Sun2 != null)
                {
                    SD_Teacher_Sun2 = _SelectedTeacher_SD_Sun2.SD_Teacher_Name;
                    OnPropertyChanged(nameof(SD_Teacher_Sun2));
                }
            }
        }
        private int _SD_TotalTime_Sun1;
        public int SD_TotalTime_Sun1
        {
            get => _SD_TotalTime_Sun1;
            set
            {
                _SD_TotalTime_Sun1 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Sun1));
            }
        }
        private int _SD_TotalTime_Sun2;
        public int SD_TotalTime_Sun2
        {
            get => _SD_TotalTime_Sun2;
            set
            {
                _SD_TotalTime_Sun2 = value;
                OnPropertyChanged(nameof(SD_TotalTime_Sun2));
            }
        }

        private void LoadData_to_Combobox_Schedule_Skill_Name_Combobox(string Class_In_Skill, string Class_In_Level, string Class_In_Student_Year, string Class_In_Semester)
        {
            var skill_id = Class_In_Skill;
            var class_level = Class_In_Level;
            var class_year = Class_In_Student_Year;
            var class_semester = Class_In_Semester;

            Debug.WriteLine($"Select Class Items in Schedule: {skill_id},{class_level},{class_year},{class_semester}");
            var SkillList = _dbConnection.GetSkill_toCombobox_Class_Schedule(skill_id, class_level, class_year, class_semester);
            Schedule_Skill_Name_Combobox.Clear();
            foreach (var skill_SD in SkillList)
            {
                Schedule_Skill_Name_Combobox.Add(skill_SD);
            }
        }
        private void LoadData_to_Combobox_Schedule_Teacher()
        {
            var TeacherList = _dbConnection.GetTeacher_toCombobox_Class_Schedule();
            Schedule_Teacher_Name_Combobox.Clear();
            foreach (var teacher_SD in TeacherList)
            {
                Schedule_Teacher_Name_Combobox.Add(teacher_SD);
            }
        }

        //Command Insert Schedule
        public ICommand Command_SaveSchedule { get; set; }
        public ICommand Command_ClearSchedule { get; set; }
        public ICommand Command_DeleteSchedule { get; set; }
        public ICommand Command_Export_Schedule_PDF { get; set; }
        public ICommand Command_Export_Schedule_Excel { get; set; }

        //Method Save Schedule
        public async Task SaveSchedule()
        {

            if (Class_ID_Schedule == 0)
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SD_Start_DateTime_MF1 == null)
            {
                ErrorMessage = "សូមជ្រើសរើសម៉ោងចាប់ផ្ដើមមុខវិជ្ជាជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SD_End_DateTime_MF1 == null)
            {
                ErrorMessage = "សូមជ្រើសរើសម៉ោងចាប់ផ្ដើមមុខវិជ្ជាជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SD_Start_DateTime_MF2 == null)
            {
                ErrorMessage = "សូមជ្រើសរើសម៉ោងចាប់ផ្ដើមមុខវិជ្ជាជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SD_End_DateTime_MF2 == null)
            {
                ErrorMessage = "សូមជ្រើសរើសម៉ោងចាប់ផ្ដើមមុខវិជ្ជាជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SD_Building_Name == null)
            {
                ErrorMessage = "សូមបំពេញឈ្មោះអគារជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SD_Building_Room == null)
            {
                ErrorMessage = "សូមបំពេញឈ្មោះថ្នាក់រៀនជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Mon1 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃចន្ទជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Mon2 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃចន្ទជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Tues1 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃអង្គារជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Tues2 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃអង្គារជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Wed1 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃពុធជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Wed2 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃពុធជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Thur1 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃពុធជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Thur2 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃពុធជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Fri1 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃពុធជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Fri2 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃពុធជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            ConfirmValue();
            SaveScheduleToDatabase();

            await Task.CompletedTask;
        }

        DateTime Create_Datetime = DateTime.Now;
        public void ConfirmValue()
        {

            Debug.WriteLine($"Class_ID_Schedule: {Class_ID_Schedule}");
            Debug.WriteLine($"Schedule Time Start1: {SD_Start_DateTime_MF1}");
            Debug.WriteLine($"Schedule Time End1: {SD_End_DateTime_MF1}");
            Debug.WriteLine($"Schedule Time Start2: {SD_Start_DateTime_MF2}");
            Debug.WriteLine($"Schedule Time End2: {SD_End_DateTime_MF2}");
            Debug.WriteLine($"Schedule Start Date: {DateTime_Start_Schedule_Strating}");
            Debug.WriteLine($"Schedule Create Time: {Create_Datetime}");
        }

        public void SaveScheduleToDatabase()
        {
            if (Schedule_State == "មាន" && Schedule_ID != 0)
            {
                //Update Mode
                Class_Schedule class_Schedule_update = new Class_Schedule()
                {
                    Schedule_ID = this.Schedule_ID,
                    Class_ID_Schedule = this.Class_ID_Schedule,
                    SD_Class_Name = this.SD_Class_Name,
                    SD_Class_TimeShift = this.Class_In_Study_Timeshift,
                    SD_Start_DateTime_MF1 = this.SD_Start_DateTime_MF1.Value,
                    SD_End_DateTime_MF1 = this.SD_End_DateTime_MF1.Value,
                    SD_Start_DateTime_MF2 = this.SD_Start_DateTime_MF2.Value,
                    SD_End_DateTime_MF2 = this.SD_End_DateTime_MF2.Value,
                    SD_Skill_Name_Mon1 = SelectedSkill_SD_Mon1.SD_Skill_Name,
                    SD_Teacher_Mon01 = SelectedTeacher_SD_Mon1.SD_Teacher_Name,
                    SD_TotalTime_Mon1 = this.SD_TotalTime_Mon1,
                    SD_Skill_Name_Mon2 = SelectedSkill_SD_Mon2.SD_Skill_Name,
                    SD_Teacher_Mon02 = SelectedTeacher_SD_Mon2.SD_Teacher_Name,
                    SD_TotalTime_Mon2 = this.SD_TotalTime_Mon2,
                    SD_Skill_Name_Tues1 = SelectedSkill_SD_Tues1.SD_Skill_Name,
                    SD_Teacher_Tues01 = SelectedTeacher_SD_Mon2.SD_Teacher_Name,
                    SD_TotalTime_Tues1 = this.SD_TotalTime_Tues1,
                    SD_Skill_Name_Tues2 = SelectedSkill_SD_Tues2.SD_Skill_Name,
                    SD_Teacher_Tues02 = SelectedTeacher_SD_Tues2.SD_Teacher_Name,
                    SD_TotalTime_Tues2 = this.SD_TotalTime_Tues2,
                    SD_Skill_Name_Wed1 = SelectedSkill_SD_Wed1.SD_Skill_Name,
                    SD_Teacher_Wed1 = SelectedTeacher_SD_Wed1.SD_Teacher_Name,
                    SD_TotalTime_Wed1 = this.SD_TotalTime_Wed1,
                    SD_Skill_Name_Wed2 = SelectedSkill_SD_Wed2.SD_Skill_Name,
                    SD_Teacher_Wed2 = SelectedTeacher_SD_Wed2.SD_Teacher_Name,
                    SD_TotalTime_Wed2 = this.SD_TotalTime_Wed2,
                    SD_Skill_Name_Thur1 = SelectedSkill_SD_Thur1.SD_Skill_Name,
                    SD_Teacher_Thur1 = SelectedTeacher_SD_Thur1.SD_Teacher_Name,
                    SD_TotalTime_Thur1 = this.SD_TotalTime_Thur1,
                    SD_Skill_Name_Thur2 = SelectedSkill_SD_Thur2.SD_Skill_Name,
                    SD_Teacher_Thur2 = SelectedTeacher_SD_Thur2.SD_Teacher_Name,
                    SD_TotalTime_Thur2 = this.SD_TotalTime_Thur2,
                    SD_Skill_Name_Fri1 = SelectedSkill_SD_Fri1.SD_Skill_Name,
                    SD_Teacher_Fri1 = SelectedTeacher_SD_Fri1.SD_Teacher_Name,
                    SD_TotalTime_Fri1 = this.SD_TotalTime_Fri1,
                    SD_Skill_Name_Fri2 = SelectedSkill_SD_Fri2.SD_Skill_Name,
                    SD_Teacher_Fri2 = SelectedTeacher_SD_Fri2.SD_Teacher_Name,
                    SD_TotalTime_Fri2 = this.SD_TotalTime_Fri2,
                    DateTime_Start_Schedule_Strating = this.DateTime_Start_Schedule_Strating,
                    SD_Building_Name = this.SD_Building_Name,
                    SD_Building_Room = this.SD_Building_Room

                };

                bool success = _dbConnection.UpdateSchedule(class_Schedule_update);

                if (success)
                {
                    ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " បានធ្វើបច្ចុប្បន្នភាពដោយជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);
                    _ = LoadSchedule(Class_ID_Schedule);
                }
                else
                {
                    ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " ធ្វើបច្ចុប្បន្នភាពបរាជ៏យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                Debug.WriteLine($"You can update schedule. {Schedule_ID}");

            }
            else
            {
                //Insert Mode
                Class_Schedule class_Schedule_Items = new Class_Schedule()
                {
                    Class_ID_Schedule = this.Class_ID_Schedule,
                    SD_Class_Name = this.SD_Class_Name,
                    SD_Class_TimeShift = this.Class_In_Study_Timeshift,
                    SD_Start_DateTime_MF1 = this.SD_Start_DateTime_MF1.Value,
                    SD_End_DateTime_MF1 = this.SD_End_DateTime_MF1.Value,
                    SD_Start_DateTime_MF2 = this.SD_Start_DateTime_MF2.Value,
                    SD_End_DateTime_MF2 = this.SD_End_DateTime_MF2.Value,
                    SD_Skill_Name_Mon1 = SelectedSkill_SD_Mon1.SD_Skill_Name,
                    SD_Teacher_Mon01 = SelectedTeacher_SD_Mon1.SD_Teacher_Name,
                    SD_TotalTime_Mon1 = this.SD_TotalTime_Mon1,
                    SD_Skill_Name_Mon2 = SelectedSkill_SD_Mon2.SD_Skill_Name,
                    SD_Teacher_Mon02 = SelectedTeacher_SD_Mon2.SD_Teacher_Name,
                    SD_TotalTime_Mon2 = this.SD_TotalTime_Mon2,
                    SD_Skill_Name_Tues1 = SelectedSkill_SD_Tues1.SD_Skill_Name,
                    SD_Teacher_Tues01 = SelectedTeacher_SD_Mon2.SD_Teacher_Name,
                    SD_TotalTime_Tues1 = this.SD_TotalTime_Tues1,
                    SD_Skill_Name_Tues2 = SelectedSkill_SD_Tues2.SD_Skill_Name,
                    SD_Teacher_Tues02 = SelectedTeacher_SD_Tues2.SD_Teacher_Name,
                    SD_TotalTime_Tues2 = this.SD_TotalTime_Tues2,
                    SD_Skill_Name_Wed1 = SelectedSkill_SD_Wed1.SD_Skill_Name,
                    SD_Teacher_Wed1 = SelectedTeacher_SD_Wed1.SD_Teacher_Name,
                    SD_TotalTime_Wed1 = this.SD_TotalTime_Wed1,
                    SD_Skill_Name_Wed2 = SelectedSkill_SD_Wed2.SD_Skill_Name,
                    SD_Teacher_Wed2 = SelectedTeacher_SD_Wed2.SD_Teacher_Name,
                    SD_TotalTime_Wed2 = this.SD_TotalTime_Wed2,
                    SD_Skill_Name_Thur1 = SelectedSkill_SD_Thur1.SD_Skill_Name,
                    SD_Teacher_Thur1 = SelectedTeacher_SD_Thur1.SD_Teacher_Name,
                    SD_TotalTime_Thur1 = this.SD_TotalTime_Thur1,
                    SD_Skill_Name_Thur2 = SelectedSkill_SD_Thur2.SD_Skill_Name,
                    SD_Teacher_Thur2 = SelectedTeacher_SD_Thur2.SD_Teacher_Name,
                    SD_TotalTime_Thur2 = this.SD_TotalTime_Thur2,
                    SD_Skill_Name_Fri1 = SelectedSkill_SD_Fri1.SD_Skill_Name,
                    SD_Teacher_Fri1 = SelectedTeacher_SD_Fri1.SD_Teacher_Name,
                    SD_TotalTime_Fri1 = this.SD_TotalTime_Fri1,
                    SD_Skill_Name_Fri2 = SelectedSkill_SD_Fri2.SD_Skill_Name,
                    SD_Teacher_Fri2 = SelectedTeacher_SD_Fri2.SD_Teacher_Name,
                    SD_TotalTime_Fri2 = this.SD_TotalTime_Fri2,
                    DateTime_Start_Schedule_Strating = this.DateTime_Start_Schedule_Strating,
                    SD_Building_Name = this.SD_Building_Name,
                    SD_Building_Room = this.SD_Building_Room

                };
                bool success = _dbConnection.Save_ScheduleInfo(class_Schedule_Items);

                if (success)
                {
                    ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " បានរក្សាទុកជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);
                    _ = LoadSchedule(Class_ID_Schedule);
                }
                else
                {
                    ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " រក្សាទុកបរាជ៏យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }

            }


        }
        //Command Load
        public ICommand Command_Load_Schedule { get; set; }

        //Method to Load Class Schedule
        public async Task LoadSchedule(int Class_ID_Schedule)
        {
            Debug.WriteLine($"Selected Load Schedule By class ID: {Class_ID_Schedule}");

            await Task.Delay(10);
            int class_id = Class_ID_Schedule;
            Load_State = true;

            var schedule_table = _dbConnection.Load_Schedule_Table_Info(class_id);

            if (schedule_table == null)
            {
                Debug.WriteLine("No data returned from the database.");
                return;
            }
            Schedule_State = "មិនមាន";
            Schedule_ID = 0;
            _ = ClearSchedule();
            foreach (var schedule_info in schedule_table)
            {
                if (schedule_info.Schedule_ID != 0)
                {
                    Schedule_State = "មាន";
                    Schedule_ID = schedule_info.Schedule_ID;
                    OnPropertyChanged(nameof(Schedule_State));
                    OnPropertyChanged(nameof(Schedule_ID));
                }
                SelectedSkill_SD_Mon1 = null;
                SelectedTeacher_SD_Mon1 = null;
                SD_TotalTime_Mon1 = 0;

                SelectedSkill_SD_Mon1 = Schedule_Skill_Name_Combobox
                   .FirstOrDefault(skill_mon1 => skill_mon1.SD_Skill_Name == schedule_info.SD_Skill_Name_Mon1);
                OnPropertyChanged(nameof(SelectedSkill_SD_Mon1));
                SelectedTeacher_SD_Mon1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Mon01);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Mon1));
                SD_TotalTime_Mon1 = schedule_info.SD_TotalTime_Mon1;
                OnPropertyChanged(nameof(SD_TotalTime_Mon1));

                SelectedSkill_SD_Mon2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == schedule_info.SD_Skill_Name_Mon2);
                OnPropertyChanged(nameof(SelectedSkill_SD_Mon2));
                SelectedTeacher_SD_Mon2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Mon01);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Mon2));
                SD_TotalTime_Mon2 = schedule_info.SD_TotalTime_Mon2;
                OnPropertyChanged(nameof(SD_TotalTime_Mon2));

                SelectedSkill_SD_Tues1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == schedule_info.SD_Skill_Name_Tues1);
                OnPropertyChanged(nameof(SelectedSkill_SD_Tues1));
                SelectedTeacher_SD_Tues1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Tues01);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Tues1));
                SD_TotalTime_Tues1 = schedule_info.SD_TotalTime_Tues1;
                OnPropertyChanged(nameof(SD_TotalTime_Tues1));

                SelectedSkill_SD_Tues2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == schedule_info.SD_Skill_Name_Tues1);
                OnPropertyChanged(nameof(SelectedSkill_SD_Tues2));
                SelectedTeacher_SD_Tues2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Tues01);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Tues2));
                SD_TotalTime_Tues2 = schedule_info.SD_TotalTime_Tues2;
                OnPropertyChanged(nameof(SD_TotalTime_Tues2));

                SelectedSkill_SD_Wed1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == schedule_info.SD_Skill_Name_Wed1);
                OnPropertyChanged(nameof(SelectedSkill_SD_Wed1));
                SelectedTeacher_SD_Wed1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Wed1);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Wed1));
                SD_TotalTime_Wed1 = schedule_info.SD_TotalTime_Wed1;
                OnPropertyChanged(nameof(SD_TotalTime_Wed1));

                SelectedSkill_SD_Wed2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == schedule_info.SD_Skill_Name_Wed2);
                OnPropertyChanged(nameof(SelectedSkill_SD_Wed2));
                SelectedTeacher_SD_Wed2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Wed2);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Wed2));
                SD_TotalTime_Wed2 = schedule_info.SD_TotalTime_Wed2;
                OnPropertyChanged(nameof(SD_TotalTime_Wed2));

                SelectedSkill_SD_Thur1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == schedule_info.SD_Skill_Name_Thur1);
                OnPropertyChanged(nameof(SelectedSkill_SD_Thur1));
                SelectedTeacher_SD_Thur1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Thur1);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Thur1));
                SD_TotalTime_Thur1 = schedule_info.SD_TotalTime_Thur1;
                OnPropertyChanged(nameof(SD_TotalTime_Thur1));

                SelectedSkill_SD_Thur2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == schedule_info.SD_Skill_Name_Thur2);
                OnPropertyChanged(nameof(SelectedSkill_SD_Thur2));
                SelectedTeacher_SD_Thur2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Thur2);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Thur2));
                SD_TotalTime_Thur2 = schedule_info.SD_TotalTime_Thur2;
                OnPropertyChanged(nameof(SD_TotalTime_Thur2));

                SelectedSkill_SD_Fri1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == schedule_info.SD_Skill_Name_Fri1);
                OnPropertyChanged(nameof(SelectedSkill_SD_Fri1));
                SelectedTeacher_SD_Fri1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Fri1);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Fri1));
                SD_TotalTime_Fri1 = schedule_info.SD_TotalTime_Fri1;
                OnPropertyChanged(nameof(SD_TotalTime_Fri1));

                SelectedSkill_SD_Fri2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == schedule_info.SD_Skill_Name_Fri2);
                OnPropertyChanged(nameof(SelectedSkill_SD_Fri2));
                SelectedTeacher_SD_Fri2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == schedule_info.SD_Teacher_Fri2);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Fri2));
                SD_TotalTime_Fri2 = schedule_info.SD_TotalTime_Fri2;
                OnPropertyChanged(nameof(SD_TotalTime_Fri2));

                if (DateTime.TryParseExact(schedule_info.DateTime_Start_Schedule_Strating, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime birthday))
                {
                    // Set individual day, month, and year values
                    SelectedDay = birthday.Day;
                    SelectedKhmerMonth = KhmerCalendarHelper.GetKhmerMonthName(birthday.Month);
                    SelectedYear = birthday.Year;
                }
                else
                {
                    Debug.WriteLine($"Get Date Error: {schedule_info.DateTime_Start_Schedule_Strating}");
                    // Handle parsing error if the string does not match the expected format
                    Debug.WriteLine("Invalid date format for DateTime_Start_Schedule_Strating.");
                }

                SD_Building_Name = schedule_info.SD_Building_Name;
                SD_Building_Room = schedule_info.SD_Building_Room;
            }

            Debug.WriteLine($"Teacher show: {SD_Skill_Name_Mon1}");
            Debug.WriteLine($"Total Time Mon Show: {SD_TotalTime_Mon1}");
            await Task.CompletedTask;
        }

        //Method Clear Schedule
        public async Task ClearSchedule()
        {
            SelectedSkill_SD_Mon1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill_mon1 => skill_mon1.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Mon1));
            SelectedTeacher_SD_Mon1 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Mon1));
            SD_TotalTime_Mon1 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Mon1));

            SelectedSkill_SD_Mon2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Mon2));
            SelectedTeacher_SD_Mon2 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Mon2));
            SD_TotalTime_Mon2 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Mon2));

            SelectedSkill_SD_Tues1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Tues1));
            SelectedTeacher_SD_Tues1 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Tues1));
            SD_TotalTime_Tues1 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Tues1));

            SelectedSkill_SD_Tues2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Tues2));
            SelectedTeacher_SD_Tues2 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Tues2));
            SD_TotalTime_Tues2 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Tues2));

            SelectedSkill_SD_Wed1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Wed1));
            SelectedTeacher_SD_Wed1 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Wed1));
            SD_TotalTime_Wed1 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Wed1));

            SelectedSkill_SD_Wed2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Wed2));
            SelectedTeacher_SD_Wed2 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Wed2));
            SD_TotalTime_Wed2 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Wed2));

            SelectedSkill_SD_Thur1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Thur1));
            SelectedTeacher_SD_Thur1 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Thur1));
            SD_TotalTime_Thur1 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Thur1));

            SelectedSkill_SD_Thur2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Thur2));
            SelectedTeacher_SD_Thur2 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Thur2));
            SD_TotalTime_Thur2 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Thur2));

            SelectedSkill_SD_Fri1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Fri1));
            SelectedTeacher_SD_Fri1 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Fri1));
            SD_TotalTime_Fri1 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Fri1));

            SelectedSkill_SD_Fri2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Fri2));
            SelectedTeacher_SD_Fri2 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Fri2));
            SD_TotalTime_Fri2 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Fri2));

            SD_Building_Name = null;
            SD_Building_Room = null;

            await Task.CompletedTask;
        }

        //Method Delete Schedule
        public async Task DeleteSchedule()
        {
            var schedule_id = Schedule_ID;

            if (schedule_id == 0)
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (schedule_id != 0)
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់លុបទិន្នន័យកាលវិភាគថ្នាក់ {SD_Class_Name} នេះមែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Delete_Schedule";
                OnPropertyChanged(nameof(CurrentOperation));
            }

            await Task.CompletedTask;
        }

        //Yes Delete Schedule
        public void HandleYes_DeleteSchedule()
        {
            var schedule_id = Schedule_ID;
            bool success = _dbConnection.DeleteSchedule(schedule_id);

            if (success)
            {
                ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " បានលុបដោយជោគជ័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);
                _ = LoadSchedule(Class_ID_Schedule);
                Schedule_State = "មិនមាន";
                Schedule_ID = 0;
            }
            else
            {
                ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " លុបបរាជ៏យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Yellow);
                return;
            }
        }

        //Method Export Schedule PDF
        public async Task Export_Schedule_PDF()
        {
            var schedule_id = Schedule_ID;

            if (schedule_id == 0)
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនដែលមានកាលវិភាគជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (schedule_id != 0)
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់ទាញយកឯកសារកាលវិភាគ ជាប្រភេទ PDF មែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Yellow);
                CurrentOperation = "Export_Schedule_PDF";
                OnPropertyChanged(nameof(CurrentOperation));
            }

            await Task.CompletedTask;
        }

        //Method Export Schedule PDF
        public void HandleYesResponseExport_Schedule_PDF()
        {
            Class_Schedule class_Schedule_Items = new Class_Schedule()
            {
                Class_ID_Schedule = this.Class_ID_Schedule,
                SD_Class_Name = this.SD_Class_Name,
                SD_Class_TimeShift = this.Class_In_Study_Timeshift,
                SD_Start_DateTime_MF1 = this.SD_Start_DateTime_MF1.Value,
                SD_End_DateTime_MF1 = this.SD_End_DateTime_MF1.Value,
                SD_Start_DateTime_MF2 = this.SD_Start_DateTime_MF2.Value,
                SD_End_DateTime_MF2 = this.SD_End_DateTime_MF2.Value,
                SD_Skill_Name_Mon1 = SelectedSkill_SD_Mon1.SD_Skill_Name,
                SD_Teacher_Mon01 = SelectedTeacher_SD_Mon1.SD_Teacher_Name,
                SD_TotalTime_Mon1 = this.SD_TotalTime_Mon1,
                SD_Skill_Name_Mon2 = SelectedSkill_SD_Mon2.SD_Skill_Name,
                SD_Teacher_Mon02 = SelectedTeacher_SD_Mon2.SD_Teacher_Name,
                SD_TotalTime_Mon2 = this.SD_TotalTime_Mon2,
                SD_Skill_Name_Tues1 = SelectedSkill_SD_Tues1.SD_Skill_Name,
                SD_Teacher_Tues01 = SelectedTeacher_SD_Mon2.SD_Teacher_Name,
                SD_TotalTime_Tues1 = this.SD_TotalTime_Tues1,
                SD_Skill_Name_Tues2 = SelectedSkill_SD_Tues2.SD_Skill_Name,
                SD_Teacher_Tues02 = SelectedTeacher_SD_Tues2.SD_Teacher_Name,
                SD_TotalTime_Tues2 = this.SD_TotalTime_Tues2,
                SD_Skill_Name_Wed1 = SelectedSkill_SD_Wed1.SD_Skill_Name,
                SD_Teacher_Wed1 = SelectedTeacher_SD_Wed1.SD_Teacher_Name,
                SD_TotalTime_Wed1 = this.SD_TotalTime_Wed1,
                SD_Skill_Name_Wed2 = SelectedSkill_SD_Wed2.SD_Skill_Name,
                SD_Teacher_Wed2 = SelectedTeacher_SD_Wed2.SD_Teacher_Name,
                SD_TotalTime_Wed2 = this.SD_TotalTime_Wed2,
                SD_Skill_Name_Thur1 = SelectedSkill_SD_Thur1.SD_Skill_Name,
                SD_Teacher_Thur1 = SelectedTeacher_SD_Thur1.SD_Teacher_Name,
                SD_TotalTime_Thur1 = this.SD_TotalTime_Thur1,
                SD_Skill_Name_Thur2 = SelectedSkill_SD_Thur2.SD_Skill_Name,
                SD_Teacher_Thur2 = SelectedTeacher_SD_Thur2.SD_Teacher_Name,
                SD_TotalTime_Thur2 = this.SD_TotalTime_Thur2,
                SD_Skill_Name_Fri1 = SelectedSkill_SD_Fri1.SD_Skill_Name,
                SD_Teacher_Fri1 = SelectedTeacher_SD_Fri1.SD_Teacher_Name,
                SD_TotalTime_Fri1 = this.SD_TotalTime_Fri1,
                SD_Skill_Name_Fri2 = SelectedSkill_SD_Fri2.SD_Skill_Name,
                SD_Teacher_Fri2 = SelectedTeacher_SD_Fri2.SD_Teacher_Name,
                SD_TotalTime_Fri2 = this.SD_TotalTime_Fri2,
                DateTime_Start_Schedule_Strating = this.DateTime_Start_Schedule_Strating,
                SD_Building_Name = this.SD_Building_Name,
                SD_Building_Room = this.SD_Building_Room

            };

            //File Schedule toPDF.
            PDFService_Generate_Schedule_Info.CreateReport(class_Schedule_Items, Class_In_Skill, Class_In_Level, Class_In_Study_Year, Class_In_Student_Year, Class_In_Semester, Class_In_Generation);
            ErrorMessage = "ឯកសារ PDF ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Method Export Schedule Excel
        public void HandleYesResponseExport_Schedule_Excel()
        {
            Debug.WriteLine("Export to Excel.");
            string curriculum_skill_select = Selected_Search_Curriculum_Skill_ID.Curriculum_Skill_Name;
            string curriculum_level_select = Selected_Search_Curriculum_Level_ID.Curriculum_Level_Name;
            string curriculum_study_year_select;
            if (Search_Study_Year_Curr == null)
            {
                curriculum_study_year_select = "1,2,3,4";
            }
            else
            {
                curriculum_study_year_select = Search_Study_Year_Curr;
            }
            Export_Excel_Curriculum_Info.ExportToExcel(Multi_Selected_Curriculum_Export, curriculum_skill_select, curriculum_level_select, curriculum_study_year_select);
            ErrorMessage = "ឯកសារ Excel ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        private Class_Schedule _SelectedSkill_SD_Sat1;
        public Class_Schedule SelectedSkill_SD_Sat1
        {
            get => _SelectedSkill_SD_Sat1;
            set
            {
                _SelectedSkill_SD_Sat1 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Sat1));
                if (_SelectedSkill_SD_Sat1 != null)
                {
                    if (seleceted_Schedule_CanUpdate == false)
                    {
                        SD_Skill_Name_Sat1 = SelectedSkill_SD_Sat1.SD_Skill_Name;
                        Debug.WriteLine($"Skill Sat Select1: {SD_Skill_Name_Sat1}");
                        LoadTeacher_andTime_BySelectSkill_Sat1(SD_Skill_Name_Sat1);
                    }
                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Sat2;
        public Class_Schedule SelectedSkill_SD_Sat2
        {
            get => _SelectedSkill_SD_Sat2;
            set
            {
                _SelectedSkill_SD_Sat2 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Sat2));
                if (_SelectedSkill_SD_Sat2 != null)
                {
                    if (seleceted_Schedule_CanUpdate == false)
                    {
                        SD_Skill_Name_Sat2 = SelectedSkill_SD_Sat2.SD_Skill_Name;
                        Debug.WriteLine($"Skill Sat Select2: {SD_Skill_Name_Sat2}");
                        LoadTeacher_andTime_BySelectSkill_Sat2(SD_Skill_Name_Sat2);
                    }
                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Sun1;
        public Class_Schedule SelectedSkill_SD_Sun1
        {
            get => _SelectedSkill_SD_Sun1;
            set
            {
                _SelectedSkill_SD_Sun1 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Sun1));
                if (_SelectedSkill_SD_Sun1 != null)
                {
                    if (seleceted_Schedule_CanUpdate == false)
                    {
                        SD_Skill_Name_Sun1 = SelectedSkill_SD_Sun1.SD_Skill_Name;
                        Debug.WriteLine($"Skill Sun Select1: {SD_Skill_Name_Sun1}");
                        LoadTeacher_andTime_BySelectSkill_Sun1(SD_Skill_Name_Sun1);
                    }
                }
            }
        }
        private Class_Schedule _SelectedSkill_SD_Sun2;
        public Class_Schedule SelectedSkill_SD_Sun2
        {
            get => _SelectedSkill_SD_Sun2;
            set
            {
                _SelectedSkill_SD_Sun2 = value;
                OnPropertyChanged(nameof(SelectedSkill_SD_Sun2));
                if (_SelectedSkill_SD_Sun2 != null)
                {
                    if (seleceted_Schedule_CanUpdate == false)
                    {
                        SD_Skill_Name_Sun2 = SelectedSkill_SD_Sun2.SD_Skill_Name;
                        Debug.WriteLine($"Skill Sun Select2: {SD_Skill_Name_Sun2}");
                        LoadTeacher_andTime_BySelectSkill_Sun2(SD_Skill_Name_Sun2);
                    }
                }
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Sun2(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Sun2 != null)
            {
                //Sunday01
                SelectedTeacher_SD_Sun2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Sun2));

                SD_TotalTime_Sun2 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Sun2));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Sun1(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Sun1 != null)
            {
                //Sunday01
                SelectedTeacher_SD_Sun1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Sun1));

                SD_TotalTime_Sun1 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Sun1));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Sat2(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Sat2 != null)
            {
                //Satureday01
                SelectedTeacher_SD_Sat2 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Sat2));

                SD_TotalTime_Sat2 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Sat2));
                return;
            }
        }
        private async void LoadTeacher_andTime_BySelectSkill_Sat1(string Skill_Name_Get)
        {
            var Skill_Name = Skill_Name_Get;
            var get_value = await _dbConnection.GetTeacher_Time_SelecetedSkill(Skill_Name);

            if (SelectedSkill_SD_Sat1 != null)
            {
                //Satureday01
                SelectedTeacher_SD_Sat1 = Schedule_Teacher_Name_Combobox
                    .FirstOrDefault(teacher => teacher.SD_Teacher_Name == get_value.teacher_name);
                OnPropertyChanged(nameof(SelectedTeacher_SD_Sat1));

                SD_TotalTime_Sat1 = get_value.total_Time1;
                OnPropertyChanged(nameof(SD_TotalTime_Sat1));
                return;
            }
        }

        public ICommand Command_SaveSchedule_SatSun { get; set; }
        public ICommand Command_ClearSchedule_SatSun { get; set; }
        public ICommand Command_EditSchedule_SatSun { get; set; }
        public ICommand Command_DeleteSchedule_SatSun { get; set; }
        public ICommand Command_ExportSchedule_SatSun_PDF { get; set; }

        private Class_Schedule _seleceted_Schedule_SatSun;
        public Class_Schedule Seleceted_Schedule_SatSun
        {
            get => _seleceted_Schedule_SatSun;
            set
            {
                _seleceted_Schedule_SatSun = value;
                OnPropertyChanged(nameof(Seleceted_Schedule_SatSun));
                IsItemSelected = _seleceted_Schedule_SatSun != null;
            }
        }
        //Edit able
        private bool _seleceted_Schedule_CanUpdate;
        public bool seleceted_Schedule_CanUpdate
        {
            get => _seleceted_Schedule_CanUpdate;
            set
            {
                _seleceted_Schedule_CanUpdate = value;
                OnPropertyChanged(nameof(seleceted_Schedule_CanUpdate));
            }
        }

        //Method Save
        public async Task SaveSchedule_SatSun()
        {
            if (SelectedSkill_SD_Sat1 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃសៅរ៍ ពេលព្រឹកជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Sat2 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃសៅរ៍ ពេលរសៀលជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Sun1 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃអាទិត្យ ពេលព្រឹកជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SelectedSkill_SD_Sun2 == null)
            {
                ErrorMessage = "សូមបំពេញកាលវិភាគថ្ងៃអាទិត្យ ពេលរសៀលជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SD_Building_Name == null)
            {
                ErrorMessage = "សូមបំពេញឈ្មោះអគារ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (SD_Building_Room == null)
            {
                ErrorMessage = "សូមបំពេញឈ្មោះបន្ទប់ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Schedule_Name == null)
            {
                ErrorMessage = "សូមបំពេញឈ្មោះកាលវិភាគ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            ConfirmValueSS();
            _ = SaveSchedule_Sat_Sun();
            _ = Load_Schedule_Sat_Sun_ToList(Class_ID);

            await Task.CompletedTask;
        }

        private void ConfirmValueSS()
        {
            Debug.WriteLine($"SS Skill Sat1: {SD_Skill_Name_Sat1}");
            Debug.WriteLine($"SS Skill Sat2: {SD_Skill_Name_Sat2}");
            Debug.WriteLine($"SS Skill Sun1: {SD_Skill_Name_Sun1}");
            Debug.WriteLine($"SS Skill Sun2: {SD_Skill_Name_Sun2}");
            Debug.WriteLine($"S1S TeacherName: {SD_Teacher_Sat1}");
            Debug.WriteLine($"S2S TeacherName: {SD_Teacher_Sat1}");
            Debug.WriteLine($"SS1 TeacherName: {SD_Teacher_Sun1}");
            Debug.WriteLine($"SS2 TeacherName: {SD_Teacher_Sun2}");
            Debug.WriteLine($"S1S TotalTime: {SD_TotalTime_Sat1}");
            Debug.WriteLine($"S2S TotalTime: {SD_TotalTime_Sat2}");
            Debug.WriteLine($"SS1 TotalTime: {SD_TotalTime_Sun1}");
            Debug.WriteLine($"SS2 TotalTime: {SD_TotalTime_Sun2}");
        }

        //Method Save Schedule Sat-Sun
        public async Task SaveSchedule_Sat_Sun()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            if (seleceted_Schedule_CanUpdate == true && Schedule_ID != 0)
            {

                Class_Schedule schedule_ss_items_update = new Class_Schedule()
                {
                    Schedule_ID = this.Schedule_ID,
                    Schedule_Name = this.Schedule_Name,
                    SD_Start_DateTime_SS1 = this.SD_Start_DateTime_SS1.Value,
                    SD_End_DateTime_SS1 = this.SD_End_DateTime_SS1.Value,
                    SD_Start_DateTime_SS2 = this.SD_Start_DateTime_SS2.Value,
                    SD_End_DateTime_SS2 = this.SD_End_DateTime_SS2.Value,
                    SD_Skill_Name_Sat1 = SelectedSkill_SD_Sat1.SD_Skill_Name,
                    SD_Teacher_Sat1 = SelectedTeacher_SD_Sat1.SD_Teacher_Name,
                    SD_TotalTime_Sat1 = this.SD_TotalTime_Sat1,
                    SD_Skill_Name_Sat2 = SelectedSkill_SD_Sat2.SD_Skill_Name,
                    SD_Teacher_Sat2 = SelectedTeacher_SD_Sat2.SD_Teacher_Name,
                    SD_TotalTime_Sat2 = this.SD_TotalTime_Sat2,
                    SD_Skill_Name_Sun1 = SelectedSkill_SD_Sun1.SD_Skill_Name,
                    SD_Teacher_Sun1 = SelectedTeacher_SD_Sun1.SD_Teacher_Name,
                    SD_TotalTime_Sun1 = this.SD_TotalTime_Sun1,
                    SD_Skill_Name_Sun2 = SelectedSkill_SD_Sun2.SD_Skill_Name,
                    SD_Teacher_Sun2 = SelectedTeacher_SD_Sun2.SD_Teacher_Name,
                    SD_TotalTime_Sun2 = this.SD_TotalTime_Sun2,
                    DateTime_Start_Schedule_Strating = this.DateTime_Start_Schedule_Strating,
                    SD_Building_Name = this.SD_Building_Name,
                    SD_Building_Room = this.SD_Building_Room

                };

                bool success = _dbConnection.UpdateSchedule_SatSun(schedule_ss_items_update);

                if (success)
                {
                    ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " បានធ្វើបច្ចុប្បន្ន ជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                    _ = Clear_Schedule_Sat_Sun();
                    _ = Load_Schedule_Sat_Sun_ToList(Class_ID);
                }
                else
                {
                    ErrorMessage = $"ថ្នាក់រៀន ({SD_Class_Name}) បានធ្វើបច្ចុប្បន្នភាពបរាជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
            }
            else
            {

                //Check before Insert
                var skill_schedule_check_first = await dbConnection.GetSkill_Schedule_Info_Check(SD_Class_Name);

                if (skill_schedule_check_first.SD_Skill_Name_Sat1_C == SD_Skill_Name_Sat1 || skill_schedule_check_first.SD_Skill_Name_Sat2_C == SD_Skill_Name_Sat1 || skill_schedule_check_first.SD_Skill_Name_Sun1_C == SD_Skill_Name_Sat1 || skill_schedule_check_first.SD_Skill_Name_Sun1_C == SD_Skill_Name_Sat1)
                {
                    ErrorMessage = $"មុខវិជ្ជា ({SD_Skill_Name_Sat1}) មានរួចហើយនៅក្នុងកាលវិភាគផ្សេងរបស់ថ្នាក់នេះ សូមផ្លាស់ប្ដូរមុខវិជ្ជាថ្មី !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                if (skill_schedule_check_first.SD_Skill_Name_Sat1_C == SD_Skill_Name_Sat2 || skill_schedule_check_first.SD_Skill_Name_Sat2_C == SD_Skill_Name_Sat2 || skill_schedule_check_first.SD_Skill_Name_Sun2_C == SD_Skill_Name_Sat2 || skill_schedule_check_first.SD_Skill_Name_Sun1_C == SD_Skill_Name_Sat2)
                {
                    ErrorMessage = $"មុខវិជ្ជា ({SD_Skill_Name_Sat2}) មានរួចហើយនៅក្នុងកាលវិភាគផ្សេងរបស់ថ្នាក់នេះ សូមផ្លាស់ប្ដូរមុខវិជ្ជាថ្មី !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                if (skill_schedule_check_first.SD_Skill_Name_Sat1_C == SD_Skill_Name_Sun1 || skill_schedule_check_first.SD_Skill_Name_Sat2_C == SD_Skill_Name_Sun1 || skill_schedule_check_first.SD_Skill_Name_Sun1_C == SD_Skill_Name_Sun1 || skill_schedule_check_first.SD_Skill_Name_Sun1_C == SD_Skill_Name_Sun1)
                {
                    ErrorMessage = $"មុខវិជ្ជា ({SD_Skill_Name_Sun1}) មានរួចហើយនៅក្នុងកាលវិភាគផ្សេងរបស់ថ្នាក់នេះ សូមផ្លាស់ប្ដូរមុខវិជ្ជាថ្មី !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                if (skill_schedule_check_first.SD_Skill_Name_Sat1_C == SD_Skill_Name_Sun2 || skill_schedule_check_first.SD_Skill_Name_Sat2_C == SD_Skill_Name_Sun2 || skill_schedule_check_first.SD_Skill_Name_Sun1_C == SD_Skill_Name_Sun2 || skill_schedule_check_first.SD_Skill_Name_Sun1_C == SD_Skill_Name_Sun2)
                {
                    ErrorMessage = $"មុខវិជ្ជា ({SD_Skill_Name_Sun2}) មានរួចហើយនៅក្នុងកាលវិភាគផ្សេងរបស់ថ្នាក់នេះ សូមផ្លាស់ប្ដូរមុខវិជ្ជាថ្មី !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }

                Class_Schedule schedule_ss_items = new Class_Schedule()
                {
                    Schedule_Name = this.Schedule_Name,
                    Class_ID_Schedule = this.Class_ID_Schedule,
                    SD_Class_Name = this.SD_Class_Name,
                    SD_Class_TimeShift = this.Class_In_Study_Timeshift,
                    SD_Start_DateTime_SS1 = this.SD_Start_DateTime_SS1.Value,
                    SD_End_DateTime_SS1 = this.SD_End_DateTime_SS1.Value,
                    SD_Start_DateTime_SS2 = this.SD_Start_DateTime_SS2.Value,
                    SD_End_DateTime_SS2 = this.SD_End_DateTime_SS2.Value,
                    SD_Skill_Name_Sat1 = SelectedSkill_SD_Sat1.SD_Skill_Name,
                    SD_Teacher_Sat1 = SelectedTeacher_SD_Sat1.SD_Teacher_Name,
                    SD_TotalTime_Sat1 = this.SD_TotalTime_Sat1,
                    SD_Skill_Name_Sat2 = SelectedSkill_SD_Sat2.SD_Skill_Name,
                    SD_Teacher_Sat2 = SelectedTeacher_SD_Sat2.SD_Teacher_Name,
                    SD_TotalTime_Sat2 = this.SD_TotalTime_Sat2,
                    SD_Skill_Name_Sun1 = SelectedSkill_SD_Sun1.SD_Skill_Name,
                    SD_Teacher_Sun1 = SelectedTeacher_SD_Sun1.SD_Teacher_Name,
                    SD_TotalTime_Sun1 = this.SD_TotalTime_Sun1,
                    SD_Skill_Name_Sun2 = SelectedSkill_SD_Sun2.SD_Skill_Name,
                    SD_Teacher_Sun2 = SelectedTeacher_SD_Sun2.SD_Teacher_Name,
                    SD_TotalTime_Sun2 = this.SD_TotalTime_Sun2,
                    DateTime_Start_Schedule_Strating = this.DateTime_Start_Schedule_Strating,
                    SD_Building_Name = this.SD_Building_Name,
                    SD_Building_Room = this.SD_Building_Room
                };

                bool success = _dbConnection.Save_Schedule_SatSun_Info(schedule_ss_items);

                if (success)
                {
                    ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " បានរក្សាទុកដោយជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                    _ = Clear_Schedule_Sat_Sun();
                    _ = Load_Schedule_Sat_Sun_ToList(Class_ID);
                }
                else
                {
                    ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " រក្សាទុកបរាជ៏យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
            }
            await Task.CompletedTask;
        }

        //Method Clear Schedule
        public async Task Clear_Schedule_Sat_Sun()
        {
            SelectedSkill_SD_Sat1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Sat1));
            SelectedTeacher_SD_Sat1 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Sat1));
            SD_TotalTime_Sat1 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Sat1));

            SelectedSkill_SD_Sat2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Sat2));
            SelectedTeacher_SD_Sat2 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Sat2));
            SD_TotalTime_Sat2 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Sat2));

            SelectedSkill_SD_Sun1 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Sun1));
            SelectedTeacher_SD_Sun1 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Sun1));
            SD_TotalTime_Sun1 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Sun1));

            SelectedSkill_SD_Sun2 = Schedule_Skill_Name_Combobox
                    .FirstOrDefault(skill => skill.SD_Skill_Name == null);
            OnPropertyChanged(nameof(SelectedSkill_SD_Sun2));
            SelectedTeacher_SD_Sun2 = Schedule_Teacher_Name_Combobox
                .FirstOrDefault(teacher => teacher.SD_Teacher_Name == null);
            OnPropertyChanged(nameof(SelectedTeacher_SD_Sun2));
            SD_TotalTime_Sun2 = 0;
            OnPropertyChanged(nameof(SD_TotalTime_Sun2));

            Schedule_Name = null;
            SD_Building_Name = null;
            SD_Building_Room = null;
            Schedule_ID = 0;
            seleceted_Schedule_CanUpdate = false;
            Can_Export_PDF = false;

            await Task.CompletedTask;
        }

        private ObservableCollection<Class_Schedule> _Schedule_List_Sat_Sun;
        public ObservableCollection<Class_Schedule> Schedule_List_Sat_Sun
        {
            get { return _Schedule_List_Sat_Sun; }
            set
            {
                _Schedule_List_Sat_Sun = value;
                OnPropertyChanged(nameof(Schedule_List_Sat_Sun));
            }
        }

        //Method Load Schedule Sat-Sun
        public async Task Load_Schedule_Sat_Sun_ToList(string class_id)
        {
            string SD_Class_ID_Select = class_id;
            try
            {
                Debug.WriteLine($"Selected Load Schedule Info Class ID: {SD_Class_ID_Select}");

                var schedule_satsun_table = _dbConnection.GetFetchSchedule_Table_Info(SD_Class_ID_Select);

                Schedule_List_Sat_Sun.Clear();

                foreach (var schedule_ss_info in schedule_satsun_table)
                {
                    Schedule_List_Sat_Sun.Add(schedule_ss_info);
                }
                Schedule_List_Sat_Sun = new ObservableCollection<Class_Schedule>(schedule_satsun_table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
            await Task.CompletedTask;
        }

        //Method Click Edit Schedule
        public async Task Click_Edit_Schedule_SatSun()
        {
            if (Seleceted_Schedule_SatSun == null)
            {
                Schedule_ID = 0;
                seleceted_Schedule_CanUpdate = false;
                ErrorMessage = "សូមជ្រើសរើសកាលវិភាគក្នុងតារាង ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                Schedule_ID = Seleceted_Schedule_SatSun.Schedule_ID;
                seleceted_Schedule_CanUpdate = true;
                Can_Export_PDF = true;
                Load_Schedule_Info_Selected_List(Schedule_ID);
            }
            await Task.CompletedTask;
        }

        private bool _isItemSelected;
        public bool IsItemSelected
        {
            get => _isItemSelected;
            set
            {
                if (_isItemSelected != value)
                {
                    _isItemSelected = value;
                    OnPropertyChanged(nameof(IsItemSelected));

                    Debug.WriteLine(IsItemSelected);
                    Debug.WriteLine(Seleceted_Schedule_SatSun);

                    // Unselect item when IsItemSelected is false
                    if (!_isItemSelected)
                    {
                        Seleceted_Schedule_SatSun = null;
                        Debug.WriteLine(IsItemSelected);

                        Debug.WriteLine(Seleceted_Schedule_SatSun);
                    }
                    _ = Clear_Schedule_Sat_Sun();
                }
            }
        }
        private bool _Can_Export_PDF;
        public bool Can_Export_PDF
        {
            get => _Can_Export_PDF;
            set
            {
                _Can_Export_PDF = value;
                OnPropertyChanged(nameof(Can_Export_PDF));
            }
        }

        //Method Load Schedule Info to Combobox
        private async void Load_Schedule_Info_Selected_List(int Schedule_ID)
        {

            var schedule_id = Schedule_ID;
            var get_info = _dbConnection.GetSchedule_Info_BySelectedTable(schedule_id);
            Debug.WriteLine($"Schedule_ID: {schedule_id}");
            foreach (var item in get_info)
            {
                Debug.WriteLine($"Sat1 : {item.SD_Skill_Name_Sat1}");
                Debug.WriteLine($"Sat2 : {item.SD_Skill_Name_Sat2}");
                Debug.WriteLine($"Sun1 : {item.SD_Skill_Name_Sun1}");
                Debug.WriteLine($"Sun2 : {item.SD_Skill_Name_Sun2}");
                if (item != null)
                {
                    //Satureday01
                    SelectedSkill_SD_Sat1 = Schedule_Skill_Name_Combobox
                        .FirstOrDefault(skill => skill.SD_Skill_Name == item.SD_Skill_Name_Sat1);
                    OnPropertyChanged(nameof(SelectedSkill_SD_Sat1));
                    SelectedTeacher_SD_Sat1 = Schedule_Teacher_Name_Combobox
                        .FirstOrDefault(teacher => teacher.SD_Teacher_Name == item.SD_Teacher_Sat1);
                    OnPropertyChanged(nameof(SelectedTeacher_SD_Sat1));
                    SD_TotalTime_Sat1 = item.SD_TotalTime_Sat1;
                    OnPropertyChanged(nameof(SD_TotalTime_Sat1));

                    //Satureday02
                    SelectedSkill_SD_Sat2 = Schedule_Skill_Name_Combobox
                        .FirstOrDefault(skill => skill.SD_Skill_Name == item.SD_Skill_Name_Sat2);
                    OnPropertyChanged(nameof(SelectedSkill_SD_Sat2));
                    SelectedTeacher_SD_Sat2 = Schedule_Teacher_Name_Combobox
                        .FirstOrDefault(teacher => teacher.SD_Teacher_Name == item.SD_Teacher_Sat2);
                    OnPropertyChanged(nameof(SelectedTeacher_SD_Sat2));
                    SD_TotalTime_Sat2 = item.SD_TotalTime_Sat2;
                    OnPropertyChanged(nameof(SD_TotalTime_Sat2));

                    //Sunday01
                    SelectedSkill_SD_Sun1 = Schedule_Skill_Name_Combobox
                        .FirstOrDefault(skill => skill.SD_Skill_Name == item.SD_Skill_Name_Sun1);
                    OnPropertyChanged(nameof(SelectedSkill_SD_Sun1));
                    SelectedTeacher_SD_Sun1 = Schedule_Teacher_Name_Combobox
                        .FirstOrDefault(teacher => teacher.SD_Teacher_Name == item.SD_Teacher_Sun1);
                    OnPropertyChanged(nameof(SelectedTeacher_SD_Sun1));
                    SD_TotalTime_Sun1 = item.SD_TotalTime_Sun1;
                    OnPropertyChanged(nameof(SD_TotalTime_Sun1));

                    //Sunday02
                    SelectedSkill_SD_Sun2 = Schedule_Skill_Name_Combobox
                        .FirstOrDefault(skill => skill.SD_Skill_Name == item.SD_Skill_Name_Sun2);
                    OnPropertyChanged(nameof(SelectedSkill_SD_Sun2));
                    SelectedTeacher_SD_Sun2 = Schedule_Teacher_Name_Combobox
                        .FirstOrDefault(teacher => teacher.SD_Teacher_Name == item.SD_Teacher_Sun2);
                    OnPropertyChanged(nameof(SelectedTeacher_SD_Sun2));
                    SD_TotalTime_Sun2 = item.SD_TotalTime_Sun2;
                    OnPropertyChanged(nameof(SD_TotalTime_Sun2));

                    //Schedule_ID = item.Schedule_ID;
                    Schedule_Name = item.Schedule_Name;
                    SD_Building_Name = item.SD_Building_Name;
                    SD_Building_Room = item.SD_Building_Room;

                    if (DateTime.TryParseExact(item.DateTime_Start_Schedule_Strating, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime birthday))
                    {
                        // Set individual day, month, and year values
                        SelectedDay = birthday.Day;
                        SelectedKhmerMonth = KhmerCalendarHelper.GetKhmerMonthName(birthday.Month);
                        SelectedYear = birthday.Year;
                    }
                    else
                    {
                        Debug.WriteLine($"Get Date Error: {item.DateTime_Start_Schedule_Strating}");
                        // Handle parsing error if the string does not match the expected format
                        Debug.WriteLine("Invalid date format for DateTime_Start_Schedule_Strating.");
                    }

                }
                else
                {
                    ErrorMessage = "មិនមានទិន្នន័យមកពី Database.";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
            }


            await Task.CompletedTask;
        }

        //Method Delete Schedule SatSun
        public void HandleYes_DeleteSchedule_SatSun()
        {
            Debug.WriteLine($"Delete Schedule SatSun {Schedule_ID}");

            var schedule_id = Schedule_ID;
            bool success = _dbConnection.DeleteSchedule_SatSun(schedule_id);

            if (success)
            {
                Schedule_Name = Seleceted_Schedule_SatSun.Schedule_Name;
                ErrorMessage = "កាលវិភាគ" + Schedule_Name + " ថ្នាក់រៀន៖ " + SD_Class_Name + " បានលុបដោយជោគជ័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);
                seleceted_Schedule_CanUpdate = false;

                _ = Load_Schedule_Sat_Sun_ToList(Class_ID);

                Seleceted_Schedule_SatSun = null;
                Schedule_ID = 0;
            }
            else
            {
                ErrorMessage = "កាលវិភាគថ្នាក់រៀន៖ " + SD_Class_Name + " លុបបរាជ៏យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Yellow);
                return;
            }

        }

        //Method Delete Schedule SS
        public async Task Delete_Schedule_SatSun()
        {
            if (Seleceted_Schedule_SatSun == null)
            {
                Schedule_ID = 0;
                seleceted_Schedule_CanUpdate = false;
                ErrorMessage = "សូមជ្រើសរើសកាលវិភាគក្នុងតារាង ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់លុបទិន្នន័យកាលវិភាគថ្នាក់ {SD_Class_Name} នេះមែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Delete_Schedule_SatSun";
                OnPropertyChanged(nameof(CurrentOperation));

                Schedule_ID = Seleceted_Schedule_SatSun.Schedule_ID;

            }

            await Task.CompletedTask;
        }

        //Method Export Schedule PDF
        public async Task Export_Schedule_SatSun_PDF()
        {
            if (Seleceted_Schedule_SatSun == null || Can_Export_PDF == false)
            {
                Schedule_ID = 0;
                seleceted_Schedule_CanUpdate = false;
                ErrorMessage = "សូមជ្រើសរើសកាលវិភាគក្នុងតារាង ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់ទាញយកទិន្នន័យកាលវិភាគថ្នាក់ {SD_Class_Name} ជាប្រភេទ PDF មែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Export_Schedule_SatSun_PDF";
                OnPropertyChanged(nameof(CurrentOperation));
                Schedule_ID = Seleceted_Schedule_SatSun.Schedule_ID;

            }

            await Task.CompletedTask;
        }

        //Method YesExport PDF
        public void HandleYes_Export_Schedule_SatSun_PDF()
        {
            Class_Schedule class_Schedule_Items = new Class_Schedule()
            {
                Class_ID_Schedule = this.Class_ID_Schedule,
                SD_Class_Name = this.SD_Class_Name,
                SD_Class_TimeShift = this.Class_In_Study_Timeshift,

                SD_Start_DateTime_SS1 = this.SD_Start_DateTime_SS1.Value,
                SD_End_DateTime_SS1 = this.SD_End_DateTime_SS1.Value,
                SD_Start_DateTime_SS2 = this.SD_Start_DateTime_SS2.Value,
                SD_End_DateTime_SS2 = this.SD_End_DateTime_SS2.Value,

                SD_Skill_Name_Sat1 = SelectedSkill_SD_Sat1.SD_Skill_Name,
                SD_Teacher_Sat1 = SelectedTeacher_SD_Sat1.SD_Teacher_Name,
                SD_TotalTime_Sat1 = this.SD_TotalTime_Sat1,
                SD_Skill_Name_Sat2 = SelectedSkill_SD_Sat2.SD_Skill_Name,
                SD_Teacher_Sat2 = SelectedTeacher_SD_Sat2.SD_Teacher_Name,
                SD_TotalTime_Sat2 = this.SD_TotalTime_Sat2,

                SD_Skill_Name_Sun1 = SelectedSkill_SD_Sun1.SD_Skill_Name,
                SD_Teacher_Sun1 = SelectedTeacher_SD_Sun1.SD_Teacher_Name,
                SD_TotalTime_Sun1 = this.SD_TotalTime_Sun1,
                SD_Skill_Name_Sun2 = SelectedSkill_SD_Sun2.SD_Skill_Name,
                SD_Teacher_Sun2 = SelectedTeacher_SD_Sun2.SD_Teacher_Name,
                SD_TotalTime_Sun2 = this.SD_TotalTime_Sun2,

                DateTime_Start_Schedule_Strating = this.DateTime_Start_Schedule_Strating,
                SD_Building_Name = this.SD_Building_Name,
                SD_Building_Room = this.SD_Building_Room,
                Schedule_Name = this.Schedule_Name
            };

            //File Schedule SS toPDF.
            PDFService_Generate_Schedule_SatSun_Info.CreateReport(class_Schedule_Items, Class_In_Skill, Class_In_Level, Class_In_Study_Year, Class_In_Student_Year, Class_In_Semester, Class_In_Generation);
            ErrorMessage = "ឯកសារ PDF ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Schedule State
        private string _Class_State;
        public string Class_State
        {
            get => _Class_State;
            set
            {
                _Class_State = value;
                OnPropertyChanged(nameof(Class_State));
            }
        }

        public ICommand Command_Update_Class_State { get; set; }

        //Method Update Class State
        public async Task Update_Class_State()
        {
            if (Current_Class_State == null)
            {
                Debug.WriteLine("No class state Selection");
                ErrorMessage = "សូមជ្រើសរើសស្ថានភាពថ្នាក់រៀន ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            Update_Class_State_Method();
            ConfirmValue_Class_State();
            _ = Search_Class_Information(Search_Class_In_Study_Year, Search_Class_In_Skill, Search_Class_In_Level, Search_Class_In_Student_Year, Search_Class_Semester, Search_Class_In_Study_Timeshift, Search_Class_In_Study_Type);
            _ = LoadClass_State_toListViews(Search_Class_State_Search);
            await Task.CompletedTask;
        }
        private void ConfirmValue_Class_State()
        {
            Debug.WriteLine($"Selected Class State: {Class_State}");
        }
        public void Update_Class_State_Method()
        {
            if (SelectedClasses_Prepare_All == null || !SelectedClasses_Prepare_All.Any())
            {
                Debug.WriteLine("No class Selection");
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀនក្នុងតារាងខាងលើ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {

                //Class_Info_Add_Student_Selected.Clear();
                foreach (var classes_edit in SelectedClasses_Prepare_All)
                {
                    Debug.WriteLine($"Selected :{classes_edit.Class_ID}");

                    bool success = _dbConnection.Update_Class_State(classes_edit.Class_ID, Current_Class_State);

                    if (success)
                    {
                        Debug.WriteLine($"Update class ID {classes_edit.Class_ID}, Current State: {Current_Class_State}");
                    }
                    else
                    {
                        Debug.WriteLine($"Update error class ID {classes_edit.Class_ID}, Current State: {Current_Class_State}");
                        return;
                    }
                }
                ErrorMessage = $"ថ្នាក់រៀនបានផ្លាស់ប្ដូរស្ថានភាពទៅជា {Current_Class_State}";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);
            }
        }

        //Class Schedule State
        private ObservableCollection<Student_Info> _Class_Info_State_Schedule;
        public ObservableCollection<Student_Info> Class_Info_State_Schedule
        {
            get => _Class_Info_State_Schedule;
            set
            {
                _Class_Info_State_Schedule = value;
                OnPropertyChanged(nameof(Class_Info_State_Schedule));
            }
        }

        //Search Text
        //
        private string _Search_Class_State_Search;
        public string Search_Class_State_Search
        {
            get => _Search_Class_State_Search;
            set
            {
                if (_Search_Class_State_Search != value)
                {
                    _Search_Class_State_Search = value;
                    OnPropertyChanged(nameof(Search_Class_State_Search));
                    Debug.WriteLine($"Search class state : {_Search_Class_State_Search}");
                    OnSearchTextChanged_Class_State(_Search_Class_State_Search);
                }
            }
        }
        private async void OnSearchTextChanged_Class_State(string newText_Class_State)
        {
            Debug.WriteLine($"Search Class State And Class Name: {newText_Class_State}");
            await LoadClass_State_toListViews(newText_Class_State);
            await Task.CompletedTask;
        }

        //Method Load Class State
        public async Task LoadClass_State_toListViews(string newText_Class_State)
        {
            IsLoading = true;
            try
            {
                await Task.Delay(10);

                var class_state_List = _dbConnection.GetClass_State_Info(newText_Class_State);

                Class_Info_State_Schedule.Clear();
                foreach (var class_info in class_state_List)
                {
                    Class_Info_State_Schedule.Add(class_info);
                }

                Class_Info_State_Schedule = new ObservableCollection<Student_Info>(class_state_List);

            }
            finally
            {
                // Hide the loading indicator
                IsLoading = false;
            }
            await Task.CompletedTask;
        }

        //
        private object _Class_StudyYear_Count_State;
        public object Class_StudyYear_Count_State
        {
            get => _Class_StudyYear_Count_State;
            set
            {
                _Class_StudyYear_Count_State = value;
                OnPropertyChanged(nameof(Class_StudyYear_Count_State));
            }
        }
        private Student_Info _Search_Class_In_Skill_Select_State;
        public Student_Info Search_Class_In_Skill_Select_State
        {
            get => _Search_Class_In_Skill_Select_State;
            set
            {
                _Search_Class_In_Skill_Select_State = value;
                OnPropertyChanged(nameof(Search_Class_In_Skill_Select_State));
            }
        }
        private Student_Info _Search_Class_In_Level_Select_State;
        public Student_Info Search_Class_In_Level_Select_State
        {
            get => _Search_Class_In_Level_Select_State;
            set
            {
                _Search_Class_In_Level_Select_State = value;
                OnPropertyChanged(nameof(Search_Class_In_Level_Select_State));
            }
        }

        //Student Score

        private int _Score_Type_ID;
        public int Score_Type_ID
        {
            get => _Score_Type_ID;
            set
            {
                _Score_Type_ID = value;
                OnPropertyChanged(nameof(Score_Type_ID));
            }
        }
        private string _Score_Type_Name;
        public string Score_Type_Name
        {
            get => _Score_Type_Name;
            set
            {
                _Score_Type_Name = value;
                OnPropertyChanged(nameof(Score_Type_Name));
            }
        }
        private Class_Score _selectedScoreType;
        public Class_Score Selected_Score_Type
        {
            get { return _selectedScoreType; }
            set
            {
                if (_selectedScoreType != value)
                {
                    _selectedScoreType = value;
                    OnPropertyChanged(nameof(Selected_Score_Type));


                    if (_selectedScoreType == null)
                    {
                        Score_Type_Name = null;

                    }
                    else
                    {
                        Score_Type_Name = _selectedScoreType.Score_Type_Name;
                    }
                }
            }
        }
        private int _Score_Skill_ID;
        public int Score_Skill_ID
        {
            get => _Score_Skill_ID;
            set
            {
                _Score_Skill_ID = value;
                OnPropertyChanged(nameof(Score_Skill_ID));
            }
        }
        private int _Score_Schedule_ID;
        public int Score_Schedule_ID
        {
            get => _Score_Schedule_ID;
            set
            {
                _Score_Schedule_ID = value;
                OnPropertyChanged(nameof(Score_Schedule_ID));
            }
        }
        private int _Score_Skill_TotalTime;
        public int Score_Skill_TotalTime
        {
            get => _Score_Skill_TotalTime;
            set
            {
                _Score_Skill_TotalTime = value;
                OnPropertyChanged(nameof(Score_Skill_TotalTime));
            }
        }
        private string _Score_Skill_Name;
        public string Score_Skill_Name
        {
            get => _Score_Skill_Name;
            set
            {
                _Score_Skill_Name = value;
                OnPropertyChanged(nameof(Score_Skill_Name));
            }
        }
        private string _Score_Skill_TeacherName;
        public string Score_Skill_TeacherName
        {
            get => _Score_Skill_TeacherName;
            set
            {
                _Score_Skill_TeacherName = value;
                OnPropertyChanged(nameof(Score_Skill_TeacherName));
            }
        }
        private string _Score_Skill_Day;
        public string Score_Skill_Day
        {
            get => _Score_Skill_Day;
            set
            {
                _Score_Skill_Day = value;
                OnPropertyChanged(nameof(Score_Skill_Day));
            }
        }
        private int _Score_Stu_ID;
        public int Score_Stu_ID
        {
            get => _Score_Stu_ID;
            set
            {
                _Score_Stu_ID = value;
                OnPropertyChanged(nameof(Score_Stu_ID));
            }
        }
        private string _Score_Student_ID;
        public string Score_Student_ID
        {
            get => _Score_Student_ID;
            set
            {
                _Score_Student_ID = value;
                OnPropertyChanged(nameof(Score_Student_ID));
            }
        }
        private string _Score_Student_Name;
        public string Score_Student_Name
        {
            get => _Score_Student_Name;
            set
            {
                _Score_Student_Name = value;
                OnPropertyChanged(nameof(Score_Student_Name));
            }
        }
        private string _Score_Student_Gender;
        public string Score_Student_Gender
        {
            get => _Score_Student_Gender;
            set
            {
                _Score_Student_Gender = value;
                OnPropertyChanged(nameof(Score_Student_Gender));
            }
        }
        private int _Student_Score;
        public int Student_Score
        {
            get => _Student_Score;
            set
            {
                _Student_Score = value;
                OnPropertyChanged(nameof(Student_Score));
            }
        }
        private string _Score_TimeShift;
        public string Score_TimeShift
        {
            get => _Score_TimeShift;
            set
            {
                _Score_TimeShift = value;
                OnPropertyChanged(nameof(Score_TimeShift));
            }
        }
        private float _Total_Score_Show;
        public float Total_Score_Show
        {
            get => _Total_Score_Show;
            set
            {
                _Total_Score_Show = value;
                OnPropertyChanged(nameof(Total_Score_Show));
            }
        }
        private float _Total_Score_Average_Show;
        public float Total_Score_Average_Show
        {
            get => _Total_Score_Average_Show;
            set
            {
                _Total_Score_Average_Show = value;
                OnPropertyChanged(nameof(Total_Score_Average_Show));
            }
        }
        private Student_Info _Selected_Class_in_Student_Score;
        public Student_Info Selected_Class_in_Student_Score
        {
            get => _Selected_Class_in_Student_Score;
            set
            {

                _Selected_Class_in_Student_Score = value;
                OnPropertyChanged(nameof(Selected_Class_in_Student_Score));

                if (_Selected_Class_in_Student_Score == null)
                {
                    Class_Name = null;
                    Class_ID = null;
                    Current_Class_State = null;
                    Class_In_Study_Timeshift = null;
                    Class_In_Study_Type = null;
                    Class_In_Generation = null;
                    Class_In_Semester = null;
                    Class_In_Student_Year = null;
                    Class_In_Study_Year = null;
                    Class_In_Level = null;
                    Class_In_Skill = null;
                }
                else
                {
                    Class_Name = _Selected_Class_in_Student_Score.Class_Name;
                    Class_ID = _Selected_Class_in_Student_Score.Class_ID;
                    Current_Class_State = _Selected_Class_in_Student_Score.Current_Class_State;
                    Class_In_Study_Timeshift = _Selected_Class_in_Student_Score.Class_In_Study_Timeshift;

                    Class_In_Study_Type = _Selected_Class_in_Student_Score.Class_In_Study_Type;
                    Class_In_Generation = _Selected_Class_in_Student_Score.Class_In_Generation;
                    Class_In_Semester = _Selected_Class_in_Student_Score.Class_In_Semester;
                    Class_In_Student_Year = _Selected_Class_in_Student_Score.Class_In_Student_Year;
                    Class_In_Study_Year = _Selected_Class_in_Student_Score.Class_In_Study_Year;
                    Class_In_Level = _Selected_Class_in_Student_Score.Class_In_Level;
                    Class_In_Skill = _Selected_Class_in_Student_Score.Class_In_Skill;

                }

            }
        }

        private void Clear_Class_Score_Selected()
        {
            Class_ID = null;
            Class_Name = null;
            Current_Class_State = null;
            Class_In_Study_Timeshift = null;
        }
        private void Load_Data_ScoreType()
        {
            var Score_Type = _dbConnection.GetScoreType_toCombobox_info();
            Class_Score.Clear();
            foreach (var score_type in Score_Type)
            {
                Class_Score.Add(score_type);
            }
        }
        //Command For Student Score
        public ICommand Student_Score_Show_Skill { get; set; }

        public async Task Show_Skill_For_Insert_Student_Score()
        {
            Load_Data_ScoreType();
            string class_id = Class_ID;
            string class_timeshift = Class_In_Study_Timeshift;
            Class_Skill_State_Info.Clear();

            _ = Show_StudentName_And_Score_Info();
            if (class_timeshift == null)
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀន ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            try
            {
                Debug.WriteLine($"Selected Load Schedule Skill Info Class ID: {class_id}");

                if (class_timeshift == "វេនសៅរ៍អាទិត្យ")
                {
                    Debug.WriteLine("Load Skill From Schedule Sat Sun.");

                    var skill_satsun_table = _dbConnection.GetFetchSchedule_Skill_SatSunTable_Info(class_id);

                    Class_Skill_Score_Info.Clear();

                    foreach (var schedule_skill_ss_info in skill_satsun_table)
                    {
                        Class_Skill_Score_Info.Add(schedule_skill_ss_info);
                    }
                    Class_Skill_Score_Info = new ObservableCollection<Class_Score>(skill_satsun_table);
                }
                else
                {
                    Debug.WriteLine("Load Skill From Schedule Mon Fri");

                    var skill_monfri_table = _dbConnection.GetFetchSchedule_Skill_MonFriTable_Info(class_id);

                    Class_Skill_Score_Info.Clear();

                    foreach (var schedule_skill_mn_info in skill_monfri_table)
                    {
                        Class_Skill_Score_Info.Add(schedule_skill_mn_info);
                    }
                    Class_Skill_Score_Info = new ObservableCollection<Class_Score>(skill_monfri_table);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }

            await Task.CompletedTask;
        }

        public async Task Show_StudentName_And_Score_Info()
        {
            string class_id = Class_ID;
            var student_score_info = _dbConnection.GetFetch_Student_Info_For_Score(class_id);

            Class_Student_Score_Info.Clear();

            foreach (var student_info in student_score_info)
            {
                Class_Student_Score_Info.Add(student_info);
            }
            Class_Student_Score_Info = new ObservableCollection<Class_Score>(student_score_info);
            await Task.CompletedTask;
        }
        private Class_Score _Selected_Skill_Name;
        public Class_Score Selected_Skill_Name
        {
            get { return _Selected_Skill_Name; }
            set
            {
                if (_Selected_Skill_Name != value)
                {
                    _Selected_Skill_Name = value;
                    OnPropertyChanged(nameof(Selected_Skill_Name));
                    if (_Selected_Skill_Name == null)
                    {
                        Score_Skill_Name = null;
                        Score_Schedule_ID = 0;
                    }
                    else
                    {
                        Score_Skill_Name = _Selected_Skill_Name.Score_Skill_Name;
                        Score_Schedule_ID = _Selected_Skill_Name.Score_Schedule_ID;
                        Score_TimeShift = Class_In_Study_Timeshift;
                        Debug.WriteLine($"Score_Skill_Name: {Score_Skill_Name},Score_Schedule_ID: {Score_Schedule_ID}, Score_TimeShift: {Score_TimeShift}");
                        Load_State_Score_Type(Score_Schedule_ID, Score_Skill_Name, Score_TimeShift);
                    }
                }
            }
        }
        //Load State Score Type
        private void Load_State_Score_Type(int Score_Schedule_ID, string Score_Skill_Name, string Score_TimeShift)
        {
            if (Score_TimeShift == "វេនសៅរ៍អាទិត្យ")
            {
                var skill_satsun_state_table = _dbConnection.GetFetchSchedule_Skill_SatSunTable_State_Info(Score_Schedule_ID, Score_Skill_Name);
                Class_Skill_State_Info.Clear();
                State_Score_Type = string.Empty;
                foreach (var schedule_skill_ss_info in skill_satsun_state_table)
                {
                    Class_Skill_State_Info.Add(schedule_skill_ss_info);
                }
                Class_Skill_State_Info = new ObservableCollection<Class_Score>(skill_satsun_state_table);
            }
            else
            {
                var skill_monfri_state_table = _dbConnection.GetFetchSchedule_Skill_MonFriTable_State_Info(Score_Schedule_ID, Score_Skill_Name);
                Class_Skill_State_Info.Clear();
                State_Score_Type = string.Empty;
                foreach (var schedule_skill_mn_info in skill_monfri_state_table)
                {
                    Class_Skill_State_Info.Add(schedule_skill_mn_info);
                }
                Class_Skill_State_Info = new ObservableCollection<Class_Score>(skill_monfri_state_table);
            }
        }
        //Selection Student Score List
        private List<Class_Score> _selected_Student_Score;
        public List<Class_Score> Multi_Selected_Student_Score
        {
            get => _selected_Student_Score;
            set
            {
                _selected_Student_Score = value;
                OnPropertyChanged(nameof(Multi_Selected_Student_Score));
            }
        }
        //First Selected Student Score
        private Class_Score _first_selected_Student_Score;
        public Class_Score First_Selected_Student_Score
        {
            get => _first_selected_Student_Score;
            set
            {
                _first_selected_Student_Score = value;
                OnPropertyChanged(nameof(First_Selected_Student_Score));
            }
        }
        //Command Save Score
        public ICommand Command_Save_Score { get; set; }

        //Method Save Student Score Info
        public async Task SaveStudentScore_Info()
        {
            if (Score_Type_Name == null)
            {
                ErrorMessage = "សូមជ្រើសរើសប្រភេទពិន្ទុ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Selected_Skill_Name == null)
            {
                ErrorMessage = "សូមជ្រើសរើសមុខវិជ្ជាត្រឹមត្រូវ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Multi_Selected_Student_Score == null || !Multi_Selected_Student_Score.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសនិស្សិត ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Confirm_Score_Info();
            SaveScore_Information();

            await Task.CompletedTask;

        }
        private bool _Can_Edit_Score_State;
        public bool Can_Edit_Score_State
        {
            get => _Can_Edit_Score_State;
            set
            {
                _Can_Edit_Score_State = value;
                OnPropertyChanged(nameof(Can_Edit_Score_State));
            }
        }
        private async void SaveScore_Information()
        {
            Class_ID = this._Selected_Class_in_Student_Score.Class_ID;
            Class_In_Study_Timeshift = this._Selected_Class_in_Student_Score.Class_In_Study_Timeshift;

            Student_Score = 0;
            Score_Type_Name = Selected_Score_Type.Score_Type_Name;

            Score_Schedule_ID = this.Selected_Skill_Name.Score_Schedule_ID;
            Score_Skill_Name = this.Selected_Skill_Name.Score_Skill_Name;
            Score_Skill_TotalTime = this.Selected_Skill_Name.Score_Skill_TotalTime;
            Score_Skill_TeacherName = this.Selected_Skill_Name.Score_Skill_TeacherName;

            if (Can_Edit_Score_State == true)
            {
                Debug.WriteLine("Update Mode.");

                if (Class_In_Study_Timeshift == "វេនសៅរ៍អាទិត្យ")
                {
                    foreach (var student_score in Multi_Selected_Student_Score)
                    {
                        Score_Stu_ID = student_score.Score_Stu_ID;
                        Score_Student_ID = student_score.Score_Student_ID;
                        Score_Student_Name = student_score.Score_Student_Name;
                        Score_Student_Gender = student_score.Score_Student_Gender;
                        Score_Student_BirthDay = student_score.Score_Student_BirthDay;
                        Student_Score = student_score.Student_Score;
                        Confirm_Score_Info();

                        bool success = _dbConnection.Update_Student_Score_SatSun_Info(student_score, Class_ID, Score_Schedule_ID, Score_Skill_Name, Score_Type_Name);

                        if (success)
                        {
                            Debug.WriteLine($"Success Update Student Score SatSun on Student ID {student_score.Score_Schedule_ID}");
                        }
                        else
                        {
                            Debug.WriteLine($"Error Update Student Score SatSun on Student ID {student_score.Score_Schedule_ID}");
                            return;
                        }

                    }
                    Load_State_Score_Type(Score_Schedule_ID, Score_Skill_Name, Score_TimeShift);
                    _ = Show_StudentName_And_Score_Info();

                    ErrorMessage = "ពិន្ទុនិស្សិត បានធ្វើបច្ចុប្បន្នភាព ជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                }
                else
                {
                    foreach (var student_score in Multi_Selected_Student_Score)
                    {
                        Score_Stu_ID = student_score.Score_Stu_ID;
                        Score_Student_ID = student_score.Score_Student_ID;
                        Score_Student_Name = student_score.Score_Student_Name;
                        Score_Student_Gender = student_score.Score_Student_Gender;
                        Score_Student_BirthDay = student_score.Score_Student_BirthDay;
                        Student_Score = student_score.Student_Score;
                        Confirm_Score_Info();

                        bool success = _dbConnection.Update_Student_Score_MonFri_Info(student_score, Class_ID, Score_Schedule_ID, Score_Skill_Name, Score_Type_Name);

                        if (success)
                        {
                            Debug.WriteLine($"Success Update Student Score MonFri on Student ID {student_score.Score_Schedule_ID}");
                        }
                        else
                        {
                            Debug.WriteLine($"Error Update Student Score MonFri on Student ID {student_score.Score_Schedule_ID}");
                            return;
                        }

                    }
                    Load_State_Score_Type(Score_Schedule_ID, Score_Skill_Name, Score_TimeShift);
                    _ = Show_StudentName_And_Score_Info();

                    ErrorMessage = "ពិន្ទុនិស្សិត បានធ្វើបច្ចុប្បន្នភាព ជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
            }
            else
            {
                Debug.WriteLine("Insert Mode.");

                if (Class_In_Study_Timeshift == "វេនសៅរ៍អាទិត្យ")
                {
                    foreach (var student_score in Multi_Selected_Student_Score)
                    {
                        Score_Stu_ID = student_score.Score_Stu_ID;
                        Score_Student_ID = student_score.Score_Student_ID;
                        Score_Student_Name = student_score.Score_Student_Name;
                        Score_Student_Gender = student_score.Score_Student_Gender;
                        Score_Student_BirthDay = student_score.Score_Student_BirthDay;
                        Student_Score = student_score.Student_Score;
                        Confirm_Score_Info();

                        //Check Before Insert
                        var check_student_score_info = await _dbConnection.Check_Student_Score_SatSun_Info(student_score.Score_Student_ID, Score_Skill_Name, Score_Schedule_ID, Score_Type_Name);

                        if (check_student_score_info.Score_Schedule_ID1 == Score_Schedule_ID &&
                           check_student_score_info.Score_Student_ID1 == student_score.Score_Student_ID &&
                           check_student_score_info.Score_Skill_Name1 == Score_Skill_Name &&
                           check_student_score_info.Score_Type_Name1 == Score_Type_Name)
                        {
                            ErrorMessage = "មុខវិជ្ជា៖ " + Score_Skill_Name + " ប្រភេទពិន្ទុ៖ " + Score_Type_Name + "មានទិន្នន័យដូចគ្នារួចស្រេចហើយ !";
                            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                            MessageColor = new SolidColorBrush(Colors.Red);
                            return;
                        }

                        bool success = _dbConnection.Save_Student_Score_SatSun_Info(student_score, Class_ID, Score_Schedule_ID, Score_Skill_Name, Score_Skill_TotalTime, Score_Skill_TeacherName, Score_Type_Name);

                        if (success)
                        {
                            Debug.WriteLine($"Success Save Student Score SatSun on Student ID {student_score.Score_Schedule_ID}");
                        }
                        else
                        {
                            Debug.WriteLine($"Error Save Student Score SatSun on Student ID {student_score.Score_Schedule_ID}");
                            return;
                        }

                    }
                    Load_State_Score_Type(Score_Schedule_ID, Score_Skill_Name, Score_TimeShift);
                    _ = Show_StudentName_And_Score_Info();

                    Selected_Score_Type = Class_Score
                       .FirstOrDefault(score_state => score_state.Score_Type_Name == null);
                    OnPropertyChanged(nameof(Selected_Score_Type));

                    ErrorMessage = "ពិន្ទុនិស្សិត បានរក្សាទុកដោយជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);

                }
                else
                {
                    foreach (var student_score in Multi_Selected_Student_Score)
                    {
                        Score_Stu_ID = student_score.Score_Stu_ID;
                        Score_Student_ID = student_score.Score_Student_ID;
                        Score_Student_Name = student_score.Score_Student_Name;
                        Score_Student_Gender = student_score.Score_Student_Gender;
                        Score_Student_BirthDay = student_score.Score_Student_BirthDay;
                        Student_Score = student_score.Student_Score;
                        Confirm_Score_Info();

                        //Check Before Insert
                        var check_student_score_info = await _dbConnection.Check_Student_Score_MonFri_Info(student_score.Score_Student_ID, Score_Skill_Name, Score_Schedule_ID, Score_Type_Name);

                        if (check_student_score_info.Score_Schedule_ID1 == Score_Schedule_ID &&
                           check_student_score_info.Score_Student_ID1 == student_score.Score_Student_ID &&
                           check_student_score_info.Score_Skill_Name1 == Score_Skill_Name &&
                           check_student_score_info.Score_Type_Name1 == Score_Type_Name)
                        {
                            ErrorMessage = "មុខវិជ្ជា៖ " + Score_Skill_Name + "ប្រភេទពិន្ទុ៖ " + Score_Type_Name + "មានទិន្នន័យដូចគ្នារួចស្រេចហើយ !";
                            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                            MessageColor = new SolidColorBrush(Colors.Red);
                            return;
                        }

                        bool success = _dbConnection.Save_Student_Score_MonFri_Info(student_score, Class_ID, Score_Schedule_ID, Score_Skill_Name, Score_Skill_TotalTime, Score_Skill_TeacherName, Score_Type_Name);

                        if (success)
                        {
                            Debug.WriteLine($"Success Save Student Score MonFri on Student ID {student_score.Score_Schedule_ID}");
                        }
                        else
                        {
                            Debug.WriteLine($"Error Save Student Score MonFri on Student ID {student_score.Score_Schedule_ID}");
                            return;
                        }

                    }
                    Load_State_Score_Type(Score_Schedule_ID, Score_Skill_Name, Score_TimeShift);
                    _ = Show_StudentName_And_Score_Info();

                    Selected_Score_Type = Class_Score
                        .FirstOrDefault(score_state => score_state.Score_Type_Name == null);
                    OnPropertyChanged(nameof(Selected_Score_Type));

                    ErrorMessage = "ពិន្ទុនិស្សិត បានរក្សាទុកដោយជោគជ័យ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
            }
        }
        private void Confirm_Score_Info()
        {
            Debug.WriteLine($"Score_Type_Name: {Score_Type_Name}");
            Debug.WriteLine($"Score_Skill_Name: {Score_Skill_Name}");
            Debug.WriteLine($"Score_Stu_ID: {Score_Stu_ID}");
            Debug.WriteLine($"Score_Student_ID: {Score_Student_ID}");
            Debug.WriteLine($"Score_Student_Name: {Score_Student_Name}");
            Debug.WriteLine($"Student_Score: {Student_Score}");
        }

        private string _Show_Score_Type;
        public string Show_Score_Type
        {
            get => _Show_Score_Type;
            set
            {
                _Show_Score_Type = value;
                OnPropertyChanged(nameof(Show_Score_Type));
            }
        }
        private string _State_Score_Type;
        public string State_Score_Type
        {
            get => _State_Score_Type;
            set
            {
                _State_Score_Type = value;
                OnPropertyChanged(nameof(State_Score_Type));
            }
        }
        private string _Score_Student_BirthDay;
        public string Score_Student_BirthDay
        {
            get => _Score_Student_BirthDay;
            set
            {
                _Score_Student_BirthDay = value;
                OnPropertyChanged(nameof(Score_Student_BirthDay));
            }
        }
        private int _Total_Score;
        public int Total_Score
        {
            get => _Total_Score;
            set
            {
                _Total_Score = value;
                OnPropertyChanged(nameof(Total_Score));
            }
        }
        private float _Total_Score_Average;
        public float Total_Score_Average
        {
            get => _Total_Score_Average;
            set
            {
                _Total_Score_Average = value;
                OnPropertyChanged(nameof(Total_Score_Average));
            }
        }
        private float _Average_Student;
        public float Average_Student
        {
            get => _Average_Student;
            set
            {
                _Average_Student = value;
                OnPropertyChanged(nameof(Average_Student));
            }
        }
        private int _Rank_Student;
        public int Rank_Student
        {
            get => _Rank_Student;
            set
            {
                _Rank_Student = value;
                OnPropertyChanged(nameof(Rank_Student));

            }
        }
        private string _Grade_Letter;
        public string Grade_Letter
        {
            get => _Grade_Letter;
            set
            {
                _Grade_Letter = value;
                OnPropertyChanged(nameof(Grade_Letter));
            }
        }
        private int _GPA_Value;
        public int GPA_Value
        {
            get => _GPA_Value;
            set
            {
                _GPA_Value = value;
                OnPropertyChanged(nameof(GPA_Value));
            }
        }
        private string _Grade_System;
        public string Grade_System
        {
            get => _Grade_System;
            set
            {
                _Grade_System = value;
                OnPropertyChanged(nameof(Grade_System));
            }
        }
        private int _Total_Students;
        public int Total_Students
        {
            get => _Total_Students;
            set
            {
                _Total_Students = value;
                OnPropertyChanged(nameof(Total_Students));
            }
        }
        private Class_Score _Selected_State_Skill_Score_Type;
        public Class_Score Selected_State_Skill_Score_Type
        {
            get => _Selected_State_Skill_Score_Type;
            set
            {
                if (_Selected_State_Skill_Score_Type != value)
                {
                    _Selected_State_Skill_Score_Type = value;
                    OnPropertyChanged(nameof(Selected_State_Skill_Score_Type));

                    if (_Selected_State_Skill_Score_Type == null)
                    {
                        Show_Score_Type = string.Empty;
                        State_Score_Type = string.Empty;
                    }
                    else
                    {
                        State_Score_Type = _Selected_State_Skill_Score_Type.State_Score_Type;
                        Show_Score_Type = _Selected_State_Skill_Score_Type.Show_Score_Type;
                        Debug.WriteLine($"Selected Score State: {Show_Score_Type}, State: {State_Score_Type}");
                    }
                }
            }
        }
        //Command Edit,Delete,Clear Student Score
        public ICommand Command_Edit_Student_Score { get; set; }
        public ICommand Command_Delete_Student_Score { get; set; }
        public ICommand Command_Clear_Student_Score { get; set; }

        //Method Edit Student Score
        public async Task Edit_Student_Score()
        {
            if (Selected_State_Skill_Score_Type == null)
            {
                ErrorMessage = "សូមជ្រើសរើសប្រភេទពិន្ទុក្នុងតារាង ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            Class_ID = Selected_Class_in_Student_Score.Class_ID;
            Class_In_Study_Timeshift = Selected_Class_in_Student_Score.Class_In_Study_Timeshift;
            Score_Schedule_ID = Selected_Skill_Name.Score_Schedule_ID;
            Score_Skill_Name = Selected_Skill_Name.Score_Skill_Name;
            Score_Skill_TotalTime = Selected_Skill_Name.Score_Skill_TotalTime;
            Score_Skill_TeacherName = Selected_Skill_Name.Score_Skill_TeacherName;
            State_Score_Type = _Selected_State_Skill_Score_Type.State_Score_Type;
            Show_Score_Type = _Selected_State_Skill_Score_Type.Show_Score_Type;

            Load_Student_Score(Class_ID, Score_Schedule_ID, Score_Skill_Name, Show_Score_Type);
            //Load_Student_Info_For_Check_Score(Class_ID, Score_Schedule_ID, Score_Skill_Name, Show_Score_Type);
            Can_Edit_Score_State = true;

            await Task.CompletedTask;
        }

        //Method Load Student Score
        private void Load_Student_Score(string Class_ID, int Score_Schedule_ID, string Score_Skill_Name, string Show_Score_Type)
        {
            var student_score_info = _dbConnection.SelectFetch_Student_Info_For_Score(Class_ID, Score_Schedule_ID, Score_Skill_Name, Show_Score_Type);

            Class_Student_Score_Info.Clear();

            foreach (var student_info in student_score_info)
            {
                Class_Student_Score_Info.Add(student_info);
                Score_Type_Name = student_info.Score_Type_Name;
            }

            Selected_Score_Type = Class_Score
                .FirstOrDefault(score_state => score_state.Score_Type_Name == Score_Type_Name);
            OnPropertyChanged(nameof(Selected_Score_Type));

            Class_Student_Score_Info = new ObservableCollection<Class_Score>(student_score_info);
        }

        public ICommand Command_Unselect_and_Add { get; set; }

        //Method Unselect and Add
        public async Task Unselect_and_Add()
        {
            Selected_State_Skill_Score_Type = null;
            Can_Edit_Score_State = false;
            _ = Show_StudentName_And_Score_Info();
            Selected_Score_Type = Class_Score
                .FirstOrDefault(score_state => score_state.Score_Type_Name == null);
            OnPropertyChanged(nameof(Selected_Score_Type));

            await Task.CompletedTask;
        }

        //Method Clear Student Score Info
        public async Task Clear_Student_Score_Info()
        {
            Class_Student_Score_Info.Clear();
            Selected_Score_Type = Class_Score
                .FirstOrDefault(score_state => score_state.Score_Type_Name == null);
            OnPropertyChanged(nameof(Selected_Score_Type));
            await Task.CompletedTask;
        }

        //Method Delete Student Score Info
        public async Task Delete_Student_Score_Info()
        {
            if (Selected_State_Skill_Score_Type == null)
            {
                ErrorMessage = "សូមជ្រើសរើសប្រភេទពិន្ទុក្នុងតារាង ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                Show_Score_Type = _Selected_State_Skill_Score_Type.Show_Score_Type;

                if (Class_In_Study_Timeshift == "វេនសៅរ៍អាទិត្យ")
                {
                    ErrorMessage_Delete = $"តើអ្នកពិតជាចង់លុបទិន្នន័យ {Show_Score_Type} នេះមែនទេ?";
                    ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                    MessageColor_Delete = new SolidColorBrush(Colors.Red);
                    CurrentOperation = "Delete_Student_Score_Info_SatSun";
                    OnPropertyChanged(nameof(CurrentOperation));
                }
                else
                {
                    ErrorMessage_Delete = $"តើអ្នកពិតជាចង់លុបទិន្នន័យ {Show_Score_Type} នេះមែនទេ?";
                    ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                    MessageColor_Delete = new SolidColorBrush(Colors.Red);
                    CurrentOperation = "Delete_Student_Score_Info_MonFri";
                    OnPropertyChanged(nameof(CurrentOperation));
                }
            }
            await Task.CompletedTask;
        }

        //Yes Delete MonFri
        public void HandleYes_Delete_Student_Info_MonFri()
        {
            Class_ID = Selected_Class_in_Student_Score.Class_ID;
            Class_In_Study_Timeshift = Selected_Class_in_Student_Score.Class_In_Study_Timeshift;
            Score_Schedule_ID = Selected_Skill_Name.Score_Schedule_ID;
            Score_Skill_Name = Selected_Skill_Name.Score_Skill_Name;
            Score_Skill_TotalTime = Selected_Skill_Name.Score_Skill_TotalTime;
            Score_Skill_TeacherName = Selected_Skill_Name.Score_Skill_TeacherName;
            State_Score_Type = _Selected_State_Skill_Score_Type.State_Score_Type;
            Show_Score_Type = _Selected_State_Skill_Score_Type.Show_Score_Type;

            bool success = _dbConnection.Delete_Student_Score_Info_MonFri(Class_ID, Score_Schedule_ID, Score_Skill_Name, Show_Score_Type);

            if (success)
            {
                ErrorMessage = "ទិន្នន័យពិន្ទុ៖ " + Show_Score_Type + " បានលុបដោយជោគជ័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);

                Load_State_Score_Type(Score_Schedule_ID, Score_Skill_Name, Score_TimeShift);
                _ = Show_StudentName_And_Score_Info();

                Selected_Score_Type = Class_Score
                    .FirstOrDefault(score_state => score_state.Score_Type_Name == null);
                OnPropertyChanged(nameof(Selected_Score_Type));

            }
            else
            {
                ErrorMessage = "ទិន្នន័យពិន្ទុ៖ " + Show_Score_Type + " លុបបរាជ៏យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Yellow);
                return;
            }
        }

        //Yes Delete SatSun
        public void HandleYes_Delete_Student_Info_SatSun()
        {
            Class_ID = Selected_Class_in_Student_Score.Class_ID;
            Class_In_Study_Timeshift = Selected_Class_in_Student_Score.Class_In_Study_Timeshift;
            Score_Schedule_ID = Selected_Skill_Name.Score_Schedule_ID;
            Score_Skill_Name = Selected_Skill_Name.Score_Skill_Name;
            Score_Skill_TotalTime = Selected_Skill_Name.Score_Skill_TotalTime;
            Score_Skill_TeacherName = Selected_Skill_Name.Score_Skill_TeacherName;
            State_Score_Type = _Selected_State_Skill_Score_Type.State_Score_Type;
            Show_Score_Type = _Selected_State_Skill_Score_Type.Show_Score_Type;

            bool success = _dbConnection.Delete_Student_Score_Info_SatSun(Class_ID, Score_Schedule_ID, Score_Skill_Name, Show_Score_Type);

            if (success)
            {
                ErrorMessage = "ទិន្នន័យពិន្ទុ៖ " + Show_Score_Type + " បានលុបដោយជោគជ័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);

                Load_State_Score_Type(Score_Schedule_ID, Score_Skill_Name, Score_TimeShift);
                _ = Show_StudentName_And_Score_Info();

                Selected_Score_Type = Class_Score
                    .FirstOrDefault(score_state => score_state.Score_Type_Name == null);
                OnPropertyChanged(nameof(Selected_Score_Type));

            }
            else
            {
                ErrorMessage = "ទិន្នន័យពិន្ទុ៖ " + Show_Score_Type + " លុបបរាជ៏យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                MessageColor = new SolidColorBrush(Colors.Yellow);
                return;
            }
        }

        //Command Export PDF
        public ICommand Command_Export_Student_Score_PDF { get; set; }

        //Method Export Student Score PDF
        public async Task Export_Student_Score_PDF()
        {
            if (Score_Type_Name == null)
            {
                ErrorMessage = "សូមជ្រើសរើសប្រភេទពិន្ទុ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Selected_Skill_Name == null)
            {
                ErrorMessage = "សូមជ្រើសរើសមុខវិជ្ជាត្រឹមត្រូវ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Multi_Selected_Student_Score == null || !Multi_Selected_Student_Score.Any())
            {
                ErrorMessage = "សូមជ្រើសរើសនិស្សិត ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Selected_State_Skill_Score_Type == null)
            {
                ErrorMessage = "សូមជ្រើសរើសប្រភេទពិន្ទុ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Show_Score_Type = _Selected_State_Skill_Score_Type.Show_Score_Type;

            if (Class_In_Study_Timeshift == "វេនសៅរ៍អាទិត្យ")
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់ទាញទិន្នន័យ {Show_Score_Type} នេះជាPDFមែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Export_Student_Score_Info_SatSun_PDF";
                OnPropertyChanged(nameof(CurrentOperation));
            }
            else
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់ទាញទិន្នន័យ {Show_Score_Type} នេះជាPDFមែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Export_Student_Score_Info_MonFri_PDF";
                OnPropertyChanged(nameof(CurrentOperation));
            }

            await Task.CompletedTask;
        }

        //Yes Export PDF SatSun
        public void HandleYes_Export_Student_Info_SatSun_PDF()
        {
            Class_ID = this._Selected_Class_in_Student_Score.Class_ID;
            Class_Name = this._Selected_Class_in_Student_Score.Class_Name;
            Class_In_Skill = this._Selected_Class_in_Student_Score.Class_In_Skill;
            Class_In_Study_Timeshift = this._Selected_Class_in_Student_Score.Class_In_Study_Timeshift;
            Class_In_Level = this._Selected_Class_in_Student_Score.Class_In_Level;
            Class_In_Student_Year = this._Selected_Class_in_Student_Score.Class_In_Student_Year;
            Class_In_Study_Year = this._Selected_Class_in_Student_Score.Class_In_Study_Year;
            Class_In_Semester = this._Selected_Class_in_Student_Score.Class_In_Semester;
            Class_In_Generation = this._Selected_Class_in_Student_Score.Class_In_Generation;
            Class_In_Study_Type = this._Selected_Class_in_Student_Score.Class_In_Study_Type;

            Student_Score = 0;
            Score_Type_Name = Selected_Score_Type.Score_Type_Name;

            Score_Schedule_ID = this.Selected_Skill_Name.Score_Schedule_ID;
            Score_Skill_Name = this.Selected_Skill_Name.Score_Skill_Name;
            Score_Skill_TotalTime = this.Selected_Skill_Name.Score_Skill_TotalTime;
            Score_Skill_TeacherName = this.Selected_Skill_Name.Score_Skill_TeacherName;

            foreach (var student_score in Multi_Selected_Student_Score)
            {
                Score_Stu_ID = student_score.Score_Stu_ID;
                Score_Student_ID = student_score.Score_Student_ID;
                Score_Student_Name = student_score.Score_Student_Name;
                Score_Student_Gender = student_score.Score_Student_Gender;
                Score_Student_BirthDay = student_score.Score_Student_BirthDay;
                Student_Score = student_score.Student_Score;
            }
            //File Student_Score_toPDF.
            PDFService_Generate_Student_Score_PDF.CreateReport(Multi_Selected_Student_Score, Class_Name, Class_In_Skill, Class_In_Study_Timeshift, Class_In_Level, Class_In_Study_Year, Class_In_Student_Year, Class_In_Semester, Class_In_Generation, Class_In_Study_Type, Score_Skill_Name, Score_Skill_TotalTime, Score_Skill_TeacherName, Score_Type_Name);
            ErrorMessage = "ឯកសារ PDF ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Yes Export PDF MonFri
        public void HandleYes_Export_Student_Info_MonFri_PDF()
        {
            Class_ID = this._Selected_Class_in_Student_Score.Class_ID;
            Class_Name = this._Selected_Class_in_Student_Score.Class_Name;
            Class_In_Skill = this._Selected_Class_in_Student_Score.Class_In_Skill;
            Class_In_Study_Timeshift = this._Selected_Class_in_Student_Score.Class_In_Study_Timeshift;
            Class_In_Level = this._Selected_Class_in_Student_Score.Class_In_Level;
            Class_In_Student_Year = this._Selected_Class_in_Student_Score.Class_In_Student_Year;
            Class_In_Study_Year = this._Selected_Class_in_Student_Score.Class_In_Study_Year;
            Class_In_Semester = this._Selected_Class_in_Student_Score.Class_In_Semester;
            Class_In_Generation = this._Selected_Class_in_Student_Score.Class_In_Generation;
            Class_In_Study_Type = this._Selected_Class_in_Student_Score.Class_In_Study_Type;

            Student_Score = 0;
            Score_Type_Name = Selected_Score_Type.Score_Type_Name;

            Score_Schedule_ID = this.Selected_Skill_Name.Score_Schedule_ID;
            Score_Skill_Name = this.Selected_Skill_Name.Score_Skill_Name;
            Score_Skill_TotalTime = this.Selected_Skill_Name.Score_Skill_TotalTime;
            Score_Skill_TeacherName = this.Selected_Skill_Name.Score_Skill_TeacherName;

            foreach (var student_score in Multi_Selected_Student_Score)
            {
                Score_Stu_ID = student_score.Score_Stu_ID;
                Score_Student_ID = student_score.Score_Student_ID;
                Score_Student_Name = student_score.Score_Student_Name;
                Score_Student_Gender = student_score.Score_Student_Gender;
                Score_Student_BirthDay = student_score.Score_Student_BirthDay;
                Student_Score = student_score.Student_Score;
            }
            //File Student_Score_toPDF.
            PDFService_Generate_Student_Score_PDF.CreateReport(Multi_Selected_Student_Score, Class_Name, Class_In_Skill, Class_In_Study_Timeshift, Class_In_Level, Class_In_Study_Year, Class_In_Student_Year, Class_In_Semester, Class_In_Generation, Class_In_Study_Type, Score_Skill_Name, Score_Skill_TotalTime, Score_Skill_TeacherName, Score_Type_Name);
            ErrorMessage = "ឯកសារ PDF ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Select Student For Show Score
        private Class_Score _Selected_Student_For_Show_Score;
        public Class_Score Selected_Student_For_Show_Score
        {
            get { return _Selected_Student_For_Show_Score; }
            set
            {
                if (_Selected_Student_For_Show_Score != value)
                {
                    _Selected_Student_For_Show_Score = value;
                    OnPropertyChanged(nameof(Selected_Student_For_Show_Score));

                    if (_Selected_Student_For_Show_Score == null)
                    {
                        ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀន ជាមុនសិន  !";
                        ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                        MessageColor = new SolidColorBrush(Colors.Red);
                        return;
                    }
                    else
                    {
                        Load_Student_Score(_Selected_Student_For_Show_Score.Score_Student_ID);
                    }

                }
            }
        }


        //Command Show Student
        public ICommand Command_Show_Student_In_Student_Score { get; set; }

        //Show Student in Student Score
        public async Task Show_Student_in_Student_Score()
        {
            if (Selected_Class_in_Student_Score == null)
            {
                ErrorMessage = "សូមជ្រើសរើសថ្នាក់រៀន ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                Class_ID = Selected_Class_in_Student_Score.Class_ID;

                Student_InClass_Score.Clear();
                try
                {
                    Load_Student_Info_For_Check_Score(Class_ID);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                }
            }


            await Task.CompletedTask;
        }

        private void Load_Student_Info_For_Check_Score(string Class_ID)
        {
            var student_check_score_info = _dbConnection.SelectFetch_Students_Info_Score(Class_ID);

            Student_InClass_Score.Clear();

            foreach (var student_info in student_check_score_info)
            {
                Student_InClass_Score.Add(student_info);
            }

            Student_InClass_Score = new ObservableCollection<Class_Score>(student_check_score_info);
        }

        //Load Student Score
        private void Load_Student_Score(string Score_Student_ID)
        {
            Class_ID = Selected_Class_in_Student_Score.Class_ID;
            Class_In_Study_Timeshift = Selected_Class_in_Student_Score.Class_In_Study_Timeshift;
            Debug.WriteLine($"Your Selected Student: {Score_Student_ID},{Class_ID},{Class_In_Study_Timeshift}");
            Setting_Letter_Grade = null;
            Setting_Grade_System = null;
            Total_Score_Average = 0;
            int Total_Score_Cont = 0;

            var select_setting_score = _dbConnection.GetSetting_Score_Info();

            if (Class_In_Study_Timeshift == "វេនសៅរ៍អាទិត្យ")
            {
                //Total Score By Subject
                Student_Total_Score_By_Subject.Clear();

                var get_total_score_bysubject = _dbConnection.GetTotalStudents_Score_BySubject1(Score_Student_ID, Class_ID);

                if (get_total_score_bysubject == null)
                {
                    Debug.WriteLine("No score return.");
                    return;
                }

                foreach (var total_score_info in get_total_score_bysubject)
                {
                    string gradeLetter = "N/A";  // Default to "N/A"
                    string gradeSystem = "N/A";  // Default in case no match is found

                    Debug.WriteLine($"🔍 Checking for Score_Skill_Name: {total_score_info.Score_Skill_Name}, Total_Score_Average: {total_score_info.Total_Score_Average}");

                    foreach (var setting_score in select_setting_score)
                    {
                        int Score1 = setting_score.Setting_Score1;
                        int Score2 = setting_score.Setting_Score2;
                        string Grade = setting_score.Setting_Letter_Grade;
                        string System = setting_score.Setting_Grade_System;

                        Debug.WriteLine($"📌 Checking range: {Score1} - {Score2} for Grade: {Grade}");

                        // Ensure the grade is correctly assigned
                        if (total_score_info.Total_Score_Average >= Score1 && total_score_info.Total_Score_Average <= Score2)
                        {
                            gradeLetter = Grade;  // Assign the correct grade
                            gradeSystem = System;  // Assign grade system if needed

                            Debug.WriteLine($"✅ Match Found! Grade Assigned: {gradeLetter}");
                            break;  // Stop searching once a match is found
                        }
                    }

                    // Assign the grade to the total_score_info object
                    total_score_info.Grade_Letter = gradeLetter;
                    total_score_info.Grade_System = gradeSystem;

                    Debug.WriteLine($"🎯 Final Grade for {total_score_info.Score_Skill_Name}: {total_score_info.Grade_Letter}");

                    // Add to observable collection
                    Student_Total_Score_By_Subject.Add(total_score_info);
                }

                Student_Total_Score_By_Subject = new ObservableCollection<Class_Score>(get_total_score_bysubject);

                //Score Type
                Student_Score_Type_Total.Clear();
                var score_type = _dbConnection.GetFetch_Student_Score_SatSun_Types_Total(Score_Student_ID, Class_ID);
                if (score_type == null)
                {
                    ErrorMessage = "និស្សិតមិនទាន់មានពិន្ទុទេ​ សូមបញ្ចូលពិន្ទុជាមុនសិន  !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                foreach (var score_info in score_type)
                {
                    Student_Score_Type_Total.Add(score_info);
                }
                Student_Score_Type_Total = new ObservableCollection<Class_Score>(score_type);

                //Total Score
                Total_Score = 0;
                Total_Students = 0;
                Total_Score_Average = 0;

                foreach (var score_info in score_type)
                {
                    Total_Score = Total_Score + score_info.Student_Score;
                }

                Total_Score_Cont = score_type.Count;

                if (Total_Score_Cont == 0)
                {
                    ErrorMessage = "និស្សិតមិនទាន់មានពិន្ទុទេ​ សូមបញ្ចូលពិន្ទុជាមុនសិន  !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                Total_Score_Average = (float)Total_Score / Total_Score_Cont;

                //Grade
                foreach (var compare_info in select_setting_score)
                {
                    if (Total_Score_Average >= compare_info.Setting_Score1 && Total_Score_Average <= compare_info.Setting_Score2)
                    {
                        Setting_Letter_Grade = compare_info.Setting_Letter_Grade;
                        Setting_Grade_System = compare_info.Setting_Grade_System;
                        Debug.WriteLine(compare_info.Setting_Letter_Grade);
                        return;
                    }
                }
            }
            else
            {
                //Total Score By Subject
                Student_Total_Score_By_Subject.Clear();

                var get_total_score_bysubject = _dbConnection.GetTotalStudents_Score_BySubject(Score_Student_ID, Class_ID);

                if (get_total_score_bysubject == null)
                {
                    Debug.WriteLine("No score return.");
                    return;
                }
                foreach (var total_score_info in get_total_score_bysubject)
                {
                    string gradeLetter = "N/A";  // Default to "N/A"
                    string gradeSystem = "N/A";  // Default in case no match is found

                    Debug.WriteLine($"🔍 Checking for Score_Skill_Name: {total_score_info.Score_Skill_Name}, Total_Score_Average: {total_score_info.Total_Score_Average}");

                    foreach (var setting_score in select_setting_score)
                    {
                        int Score1 = setting_score.Setting_Score1;
                        int Score2 = setting_score.Setting_Score2;
                        string Grade = setting_score.Setting_Letter_Grade;
                        string System = setting_score.Setting_Grade_System;

                        Debug.WriteLine($"📌 Checking range: {Score1} - {Score2} for Grade: {Grade}");

                        // Ensure the grade is correctly assigned
                        if (total_score_info.Total_Score_Average >= Score1 && total_score_info.Total_Score_Average <= Score2)
                        {
                            gradeLetter = Grade;  // Assign the correct grade
                            gradeSystem = System;  // Assign grade system if needed

                            Debug.WriteLine($"✅ Match Found! Grade Assigned: {gradeLetter}");
                            break;  // Stop searching once a match is found
                        }
                    }

                    // Assign the grade to the total_score_info object
                    total_score_info.Grade_Letter = gradeLetter;
                    total_score_info.Grade_System = gradeSystem;

                    Debug.WriteLine($"🎯 Final Grade for {total_score_info.Score_Skill_Name}: {total_score_info.Grade_Letter}");

                    // Add to observable collection
                    Student_Total_Score_By_Subject.Add(total_score_info);
                }

                Student_Total_Score_By_Subject = new ObservableCollection<Class_Score>(get_total_score_bysubject);

                //Score Type
                Student_Score_Type_Total.Clear();
                var score_type = _dbConnection.GetFetch_Student_Score_MonFri_Types_Total(Score_Student_ID, Class_ID);
                if (score_type == null)
                {
                    ErrorMessage = "និស្សិតមិនទាន់មានពិន្ទុទេ​ សូមបញ្ចូលពិន្ទុជាមុនសិន  !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                foreach (var score_info in score_type)
                {
                    Student_Score_Type_Total.Add(score_info);
                }
                Student_Score_Type_Total = new ObservableCollection<Class_Score>(score_type);

                //Total Score
                Total_Score_Show = 0;
                Total_Students = 0;
                Total_Score_Average = 0;

                foreach (var score_info in get_total_score_bysubject)
                {
                    Total_Score_Show = Total_Score_Show + score_info.Total_Score_Average;
                }

                Total_Score_Cont = get_total_score_bysubject.Count;

                if (Total_Score_Cont == 0)
                {
                    ErrorMessage = "និស្សិតមិនទាន់មានពិន្ទុទេ​ សូមបញ្ចូលពិន្ទុជាមុនសិន  !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                Total_Score_Average_Show = (float)Total_Score_Show / Total_Score_Cont;

                //Grade
                foreach (var compare_info in select_setting_score)
                {
                    if (Total_Score_Average_Show >= compare_info.Setting_Score1 && Total_Score_Average_Show <= compare_info.Setting_Score2)
                    {
                        Setting_Letter_Grade = compare_info.Setting_Letter_Grade;
                        Setting_Grade_System = compare_info.Setting_Grade_System;
                        Debug.WriteLine(compare_info.Setting_Letter_Grade);
                        return;
                    }
                }
            }

        }

        //Setting_Score
        private int _Setting_Score_ID;
        public int Setting_Score_ID
        {
            get => _Setting_Score_ID;
            set
            {
                _Setting_Score_ID = value;
                OnPropertyChanged(nameof(Setting_Score_ID));
            }
        }
        private int _Setting_Score1;
        public int Setting_Score1
        {
            get => _Setting_Score1;
            set
            {
                _Setting_Score1 = value;
                OnPropertyChanged(nameof(Setting_Score1));
            }
        }
        private int _Setting_Score2;
        public int Setting_Score2
        {
            get => _Setting_Score2;
            set
            {
                _Setting_Score2 = value;
                OnPropertyChanged(nameof(Setting_Score2));
            }
        }
        private string _Setting_Letter_Grade;
        public string Setting_Letter_Grade
        {
            get => _Setting_Letter_Grade;
            set
            {
                _Setting_Letter_Grade = value;
                OnPropertyChanged(nameof(Setting_Letter_Grade));
            }
        }
        private int _Setting_GPA_Value;
        public int Setting_GPA_Value
        {
            get => _Setting_GPA_Value;
            set
            {
                _Setting_GPA_Value = value;
                OnPropertyChanged(nameof(Setting_GPA_Value));
            }
        }
        private string _Setting_Grade_System;
        public string Setting_Grade_System
        {
            get => _Setting_Grade_System;
            set
            {
                _Setting_Grade_System = value;
                OnPropertyChanged(nameof(Setting_Grade_System));
            }
        }
        private bool _Can_Edit_Setting_Score;
        public bool Can_Edit_Setting_Score
        {
            get => _Can_Edit_Setting_Score;
            set
            {
                _Can_Edit_Setting_Score = value;
                OnPropertyChanged(nameof(Can_Edit_Setting_Score));
            }
        }

        public ICommand Command_Save_Setting_Score { get; set; }
        public ICommand Command_Clear_Setting_Score { get; set; }

        public async Task Clear_Setting_Score_Box()
        {
            Setting_Score1 = 0;
            Setting_Score2 = 0;
            Setting_Letter_Grade = null;
            Setting_GPA_Value = 0;
            Setting_Grade_System = null;
            Setting_Score_ID = 0;

            Can_Edit_Setting_Score = false;
            Seletecd_Setting_Score = null;
            await Task.CompletedTask;
        }
        public async Task Save_Setting_Score()
        {
            Setting_Score1 = this.Setting_Score1;
            Setting_Score2 = this.Setting_Score2;
            Setting_Letter_Grade = this.Setting_Letter_Grade;
            Setting_GPA_Value = this.Setting_GPA_Value;
            Setting_Grade_System = this.Setting_Grade_System;

            if (Setting_Score2 == 0)
            {
                ErrorMessage = "សូមពិនិត្យបញ្ចូលពិន្ទុ ជាមុនសិន  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Setting_Score1 < 0 || Setting_Score2 < 0)
            {
                ErrorMessage = "ពិន្ទុចាំបាច់ត្រូវតែធំជាង 0  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Setting_Score1 >= 100 || Setting_Score2 >= 101)
            {
                ErrorMessage = "ពិន្ទុចាំបាច់ត្រូវតែតូចជាង ឬស្នើ 100  !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Setting_Letter_Grade == null)
            {
                ErrorMessage = "សូមបំពេញតួអក្សរនិទ្ទេស ជាមុនសិន";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Setting_GPA_Value < 0 || Setting_GPA_Value > 20)
            {
                ErrorMessage = "សូមបំពេញតម្លៃ GPA អោយបានត្រឹមត្រូវ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (Setting_Grade_System == null)
            {
                ErrorMessage = "សូមបំពេញកម្រិតនិទ្ទេស ជាមុនសិន";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            Confirm_Value_Setting_Score();
            SaveSetting_Score();
            _ = Load_Setting_Score();
            _ = Clear_Setting_Score_Box();

            await Task.CompletedTask;
        }

        //Connfirm Value
        private void Confirm_Value_Setting_Score()
        {

            Debug.WriteLine(Setting_Score1);
            Debug.WriteLine(Setting_Score2);
            Debug.WriteLine(Setting_Letter_Grade);
            Debug.WriteLine(Setting_GPA_Value);
            Debug.WriteLine(Setting_Grade_System);

        }

        //Method Load Setting Score
        public async Task Load_Setting_Score()
        {
            Setting_Score_List.Clear();

            var setting_score = _dbConnection.Load_Setting_Score_ToTable();

            foreach (var item in setting_score)
            {
                Setting_Score_List.Add(item);

            }
            Setting_Score_List = new ObservableCollection<Class_Score>(setting_score);

            await Task.CompletedTask;
        }

        //Method Save Settting Score
        public void SaveSetting_Score()
        {
            if (Can_Edit_Setting_Score == true)
            {
                //Update Mode
                Class_Score update_score_setting = new Class_Score()
                {
                    Setting_Score_ID = Seletecd_Setting_Score.Setting_Score_ID,
                    Setting_Score1 = this.Setting_Score1,
                    Setting_Score2 = this.Setting_Score2,
                    Setting_Letter_Grade = this.Setting_Letter_Grade,
                    Setting_GPA_Value = this.Setting_GPA_Value,
                    Setting_Grade_System = this.Setting_Grade_System
                };

                bool success = _dbConnection.Update_Setting_Score(update_score_setting);

                if (success)
                {
                    Can_Edit_Setting_Score = false;
                    ErrorMessage = $" ការកំណត់លើប្រភេទពិន្ទុបានធ្វើបច្ចុប្បន្នភាព ជោគជ័យ {Setting_Grade_System}";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    ErrorMessage = $" ការកំណត់លើប្រភេទពិន្ទុបានធ្វើបច្ចុប្បន្នភាព បរាជ័យ {Setting_Grade_System}";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
            }
            else
            {
                //Insert Mode
                Class_Score setting_score = new Class_Score()
                {
                    Setting_Score1 = this.Setting_Score1,
                    Setting_Score2 = this.Setting_Score2,
                    Setting_Letter_Grade = this.Setting_Letter_Grade,
                    Setting_GPA_Value = this.Setting_GPA_Value,
                    Setting_Grade_System = this.Setting_Grade_System

                };
                bool success = _dbConnection.Save_Setting_Score(setting_score);

                if (success)
                {
                    Can_Edit_Setting_Score = false;
                    ErrorMessage = $" ការកំណត់លើប្រភេទពិន្ទុបានរក្សាទុក ជោគជ័យ {Setting_Grade_System}";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    ErrorMessage = $" ការកំណត់លើប្រភេទពិន្ទុបានរក្សាទុក បរាជ័យ {Setting_Grade_System}";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
            }

        }

        //Update Delete
        public ICommand Command_Edit_Setting_Score { get; set; }
        public ICommand Command_Delete_Setting_Score { get; set; }

        private Class_Score _Seletecd_Setting_Score;
        public Class_Score Seletecd_Setting_Score
        {
            get => _Seletecd_Setting_Score;
            set
            {
                if (_Seletecd_Setting_Score != value)
                {
                    _Seletecd_Setting_Score = value;
                    OnPropertyChanged(nameof(Seletecd_Setting_Score));
                }
            }
        }
        public async Task Select_Setting_Score_Edit()
        {
            if (Seletecd_Setting_Score != null)
            {
                Can_Edit_Setting_Score = true;
                Setting_Score_ID = Seletecd_Setting_Score.Setting_Score_ID;
                Setting_Score1 = Seletecd_Setting_Score.Setting_Score1;
                Setting_Score2 = Seletecd_Setting_Score.Setting_Score2;
                Setting_Letter_Grade = Seletecd_Setting_Score.Setting_Letter_Grade;
                Setting_GPA_Value = Seletecd_Setting_Score.Setting_GPA_Value;
                Setting_Grade_System = Seletecd_Setting_Score.Setting_Grade_System;

                Debug.WriteLine(Setting_Score_ID);
            }
            else
            {
                Can_Edit_Setting_Score = false;
                Setting_Score_ID = 0;

                ErrorMessage = "សូមបំពេញកម្រិតនិទ្ទេសក្នុងតារាង ជាមុនសិន";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            await Task.CompletedTask;
        }

        //Method Delete Setting Score
        public async Task Delete_Setting_Score()
        {
            if (Seletecd_Setting_Score != null)
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់លុបទិន្នន័យនេះមែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Delete_Student_Score";
                OnPropertyChanged(nameof(CurrentOperation));
                Can_Edit_Setting_Score = true;

            }
            else
            {
                ErrorMessage = "សូមបំពេញកម្រិតនិទ្ទេសក្នុងតារាង ជាមុនសិន";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            await Task.CompletedTask;
        }

        //Method Click Yes to Delete
        public void HandleYesDelete_Setting_Score()
        {
            Setting_Score_ID = Seletecd_Setting_Score.Setting_Score_ID;
            if (Can_Edit_Setting_Score == true)
            {
                bool success = _dbConnection.Delete_Setting_Score(Setting_Score_ID);

                if (success)
                {
                    _ = Load_Setting_Score();
                    _ = Clear_Setting_Score_Box();
                    Can_Edit_Setting_Score = false;
                    ErrorMessage = $" ការកំណត់លើប្រភេទពិន្ទុបានលុប ជោគជ័យ {Setting_Grade_System}";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    ErrorMessage = $" ការកំណត់លើប្រភេទពិន្ទុបានលុប បរាជ័យ {Setting_Grade_System}";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
            }
            else
            {
                ErrorMessage = "សូមបំពេញកម្រិតនិទ្ទេសក្នុងតារាង ជាមុនសិន";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
        }

        private int _Total_Count_Score_Type;
        public int Total_Count_Score_Type
        {
            get => _Total_Count_Score_Type;
            set
            {
                _Total_Count_Score_Type = value;
                OnPropertyChanged(nameof(Total_Count_Score_Type));
            }
        }
        private string _Student_Pass_State;
        public string Student_Pass_State
        {
            get => _Student_Pass_State;
            set
            {
                _Student_Pass_State = value;
                OnPropertyChanged(nameof(Student_Pass_State));
            }
        }
        public ICommand Command_Show_Students_Rank { get; set; }

        //Method Show Student Rank
        public async Task Show_Students_Rank()
        {
            Class_Name = Selected_Class_in_Student_Score.Class_Name;
            Class_ID = Selected_Class_in_Student_Score.Class_ID;
            Current_Class_State = Selected_Class_in_Student_Score.Current_Class_State;
            Class_In_Study_Timeshift = Selected_Class_in_Student_Score.Class_In_Study_Timeshift;

            Class_In_Study_Type = Selected_Class_in_Student_Score.Class_In_Study_Type;
            Class_In_Generation = Selected_Class_in_Student_Score.Class_In_Generation;
            Class_In_Semester = Selected_Class_in_Student_Score.Class_In_Semester;
            Class_In_Student_Year = Selected_Class_in_Student_Score.Class_In_Student_Year;
            Class_In_Study_Year = Selected_Class_in_Student_Score.Class_In_Study_Year;
            Class_In_Level = Selected_Class_in_Student_Score.Class_In_Level;
            Class_In_Skill = Selected_Class_in_Student_Score.Class_In_Skill;

            Students_Rank_List.Clear();
            //int Total_Score_Cont = 0;

            var select_setting_score = _dbConnection.GetSetting_Score_Info();
            var show_student_rank = _dbConnection.GetStudents_Rank_Info(Class_ID, Class_In_Study_Timeshift);

            foreach (var student_rank in show_student_rank)
            {
                string gradeLetter = "N/A";
                string gradeSystem = "N/A";

                //Debug.WriteLine($"🔍 Checking for Score_Skill_Name: {total_score_info.Score_Skill_Name}, Total_Score_Average: {total_score_info.Total_Score_Average}");

                foreach (var setting_score in select_setting_score)
                {
                    int Score1 = setting_score.Setting_Score1;
                    int Score2 = setting_score.Setting_Score2;
                    string Grade = setting_score.Setting_Letter_Grade;
                    string System = setting_score.Setting_Grade_System;

                    Debug.WriteLine($"📌 Checking range: {Score1} - {Score2} for Grade: {Grade}");

                    if (student_rank.Average_Student >= 50.00)
                    {
                        Student_Pass_State = "ជាប់";
                    }
                    if (student_rank.Average_Student < 50.00)
                    {
                        Student_Pass_State = "ធ្លាក់";
                    }

                    // Ensure the grade is correctly assigned
                    if (student_rank.Average_Student >= Score1 && student_rank.Average_Student <= Score2)
                    {
                        gradeLetter = Grade;  // Assign the correct grade
                        gradeSystem = System;  // Assign grade system if needed

                        Debug.WriteLine($"✅ Match Found! Grade Assigned: {gradeLetter}");
                        break;  // Stop searching once a match is found
                    }
                }

                // Assign the grade to the total_score_info object
                student_rank.Grade_Letter = gradeLetter;
                student_rank.Grade_System = gradeSystem;
                student_rank.Student_Pass_State = Student_Pass_State;
                Students_Rank_List.Add(student_rank);
            }
            Students_Rank_List = new ObservableCollection<Class_Score>(show_student_rank);
            await Task.CompletedTask;
        }

        //Method Export Student Eank
        public ICommand Command_Export_Student_Rank_PDF { get; set; }
        public ICommand Command_Export_Student_Rank_Excel { get; set; }

        //Student Rank PDF
        public async Task Export_Students_Rank_PDF()
        {
            if (MultiSelectAllStudent_Rank != null)
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់ទាញយកទិន្នន័យចំណាត់ថ្នាក់នេះជាប្រភេទ PDF មែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Export_Students_Rank_PDF";
                OnPropertyChanged(nameof(CurrentOperation));
                //Can_Edit_Setting_Score = true;    
            }
            else
            {
                ErrorMessage = $" សូមជ្រើសរើសនិស្សិតជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }


            await Task.CompletedTask;
        }

        public void HandleYes_Export_Student_Rank_PDF()
        {
            Class_Name = Selected_Class_in_Student_Score.Class_Name;
            Class_ID = Selected_Class_in_Student_Score.Class_ID;
            Current_Class_State = Selected_Class_in_Student_Score.Current_Class_State;
            Class_In_Study_Timeshift = Selected_Class_in_Student_Score.Class_In_Study_Timeshift;

            Class_In_Study_Type = Selected_Class_in_Student_Score.Class_In_Study_Type;
            Class_In_Generation = Selected_Class_in_Student_Score.Class_In_Generation;
            Class_In_Semester = Selected_Class_in_Student_Score.Class_In_Semester;
            Class_In_Student_Year = Selected_Class_in_Student_Score.Class_In_Student_Year;
            Class_In_Study_Year = Selected_Class_in_Student_Score.Class_In_Study_Year;
            Class_In_Level = Selected_Class_in_Student_Score.Class_In_Level;
            Class_In_Skill = Selected_Class_in_Student_Score.Class_In_Skill;

            //File Student_Rank_toPDF.
            PDFService_Generate_Students_Rank_PDF.CreateReport(MultiSelectAllStudent_Rank, Class_Name, Class_In_Skill, Class_In_Study_Timeshift, Class_In_Level, Class_In_Study_Year, Class_In_Student_Year, Class_In_Semester, Class_In_Generation, Class_In_Study_Type);
            ErrorMessage = "ឯកសារ PDF ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Student Rank Excel
        public async Task Export_Students_Rank_Excel()
        {
            if (MultiSelectAllStudent_Rank != null)
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់ទាញយកទិន្នន័យចំណាត់ថ្នាក់នេះជាប្រភេទ Excel មែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Export_Students_Rank_Excel";
                OnPropertyChanged(nameof(CurrentOperation));
                //Can_Edit_Setting_Score = true;    
            }
            else
            {
                ErrorMessage = $" សូមជ្រើសរើសនិស្សិតជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            await Task.CompletedTask;
        }

        public void HandleYes_Export_Students_Rank_Excel()
        {
            Export_Excel_Students_Rank.ExportToExcel(MultiSelectAllStudent_Rank, Class_Name, Class_In_Skill, Class_In_Study_Timeshift, Class_In_Level, Class_In_Study_Year, Class_In_Student_Year, Class_In_Semester, Class_In_Generation, Class_In_Study_Type);

            ErrorMessage = "ឯកសារ Excel ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Send Student Up Class
        public ICommand Command_Send_Student_Up_Class { get; set; }

        public async Task Send_Student_Class_Up()
        {
            if (MultiSelectAllStudent_Rank == null || !MultiSelectAllStudent_Rank.Any())
            {
                ErrorMessage = $" សូមជ្រើសរើសនិស្សិតជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;

            }
            else
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់ដំឡើងកម្រិតថ្នាក់ចំពោះនិស្សិតដែលមានលទ្ធផល(ជាប់) មែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Send_Students_Class_Up";
                OnPropertyChanged(nameof(CurrentOperation));
                //Can_Edit_Setting_Score = true;   
            }
            await Task.CompletedTask;
        }
        public void Handle_Yes_Send_Student_Up_Class()
        {

            Class_In_Study_Timeshift = Selected_Class_in_Student_Score.Class_In_Study_Timeshift;
            Class_In_Semester = Selected_Class_in_Student_Score.Class_In_Semester;
            Class_In_Student_Year = Selected_Class_in_Student_Score.Class_In_Student_Year;
            Class_In_Study_Year = Selected_Class_in_Student_Score.Class_In_Study_Year;
            //Null In Class

            foreach (var student_up_info in MultiSelectAllStudent_Rank)
            {
                if (student_up_info.Student_Pass_State == "ជាប់")
                {
                    if (Class_In_Semester == "1")
                    {
                        int Up_Semester = int.Parse(Class_In_Semester) + 1;
                        string Semester = Up_Semester.ToString();
                        string Up_Study_Year = Class_In_Study_Year;
                        int Up_Student_Year = int.Parse(Class_In_Student_Year);
                        string Student_Year = Up_Student_Year.ToString();
                        string Class_State = null;

                        bool success = _dbConnection.Up_Students_Class(Semester, Student_Year, Up_Study_Year, Class_State, student_up_info.Score_Student_ID);
                        if (success)
                        {
                            Debug.WriteLine($"Success Student ID: {student_up_info.Score_Student_ID} Passed.Semester = {Up_Semester}, Study Year: {Up_Study_Year}");
                        }
                        else
                        {
                            Debug.WriteLine($"Error Student Up Class: {student_up_info.Score_Student_ID}");
                            break;
                        }
                    }
                    else if (Class_In_Semester == "2")
                    {
                        int Up_Semester = int.Parse(Class_In_Semester) - 1;
                        string Semester = Up_Semester.ToString();
                        int Up_Student_Year = int.Parse(Class_In_Student_Year) + 1;
                        string Student_Year = Up_Student_Year.ToString();
                        string Class_State = null;
                        if (Up_Student_Year > 5)
                        {
                            Debug.WriteLine($"Student Year Can't up to {Up_Student_Year}");
                            break;
                        }
                        string Up_Study = Class_In_Study_Year;
                        // Split the string, increment the first year, and update the second year accordingly
                        string[] years = Up_Study.Split('-');
                        int firstYear = int.Parse(years[0]) + 1;
                        int secondYear = int.Parse(years[1]) + 1;

                        string Up_Study_Year = $"{firstYear}-{secondYear}";

                        bool success = _dbConnection.Up_Students_Class(Semester, Student_Year, Up_Study_Year, Class_State, student_up_info.Score_Student_ID);
                        if (success)
                        {
                            Debug.WriteLine($"Success Student ID: {student_up_info.Score_Student_ID} Passed.Semester = {Up_Semester}, Study Year: {Up_Study_Year}");
                        }
                        else
                        {
                            Debug.WriteLine($"Error Student Up Class: {student_up_info.Score_Student_ID}");
                            break;
                        }
                    }

                }
                if (student_up_info.Student_Pass_State == "ធ្លាក់")
                {
                    int Up_Semester = int.Parse(Class_In_Semester);
                    string Semester = Up_Semester.ToString();
                    string Up_Study_Year = Class_In_Study_Year;
                    int Up_Student_Year = int.Parse(Class_In_Student_Year);
                    string Student_Year = Up_Student_Year.ToString();
                    string Class_State = null;

                    bool success = _dbConnection.Up_Students_Class(Semester, Student_Year, Up_Study_Year, Class_State, student_up_info.Score_Student_ID);
                    if (success)
                    {
                        Debug.WriteLine($"Success Student ID: {student_up_info.Score_Student_ID} Passed.Semester = {Up_Semester}, Study Year: {Up_Study_Year}");
                    }
                    else
                    {
                        Debug.WriteLine($"Error Student Up Class: {student_up_info.Score_Student_ID}");
                        break;
                    }
                    Debug.WriteLine($"Student ID: {student_up_info.Score_Student_ID} Failed.");
                }
            }

            ErrorMessage = "ឯកសារ PDF ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
        }

        //Command Certificate_of_Education_Info
        public ICommand Command_Export_PDF_Certificate_of_Education { get; set; }
        public ICommand Command_Export_Word_Certificate_of_Education { get; set; }

        //Method Export Certificate_of_Education PDF
        public async Task Export_Certificate_of_Education_PDF()
        {
            if (SelectedStudent_CheckStudent == null)
            {
                ErrorMessage = $" សូមជ្រើសរើសនិស្សិតជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់បង្កើតលិខិតបញ្ជាក់ការសិក្សារបស់និស្សិតនេះ មែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Export_Certificate_of_Education_PDF";
                OnPropertyChanged(nameof(CurrentOperation));
            }

            await Task.CompletedTask;
        }
        public void HandleYes_Yes_Export_Student_Certificate_of_Education()
        {
            // Convert date to Khmer format and assign it to a property in _selectedStudent
            _selectedStudent.Stu_BirthdayDateShow = ConvertToKhmerDate(_selectedStudent.Stu_BirthdayDateOnly);
            PDFService_Generate_Certificate_Student_Info.CreateReport(SelectedStudent_CheckStudent);
            ErrorMessage = "PDF លិខិតបញ្ជាក់ការសិក្សា ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
            Debug.WriteLine("Export_Certificate_of_Education_PDF OK");
        }
        //Internship
        private string _Stu_Internship_Grade;
        public string Stu_Internship_Grade
        {
            get => _Stu_Internship_Grade;
            set
            {
                _Stu_Internship_Grade = value;
                OnPropertyChanged(nameof(Stu_Internship_Grade));
            }
        }
        //Command Transcript List
        public ICommand Command_Show_Student_Class { get; set; }

        private int _Class_ID_Show;
        public int Class_ID_Show
        {
            get => _Class_ID_Show;
            set
            {
                _Class_ID_Show = value;
                OnPropertyChanged(nameof(Class_ID_Show));
            }
        }
        //Method show class
        public async Task ShowStudents_Class_Transcript()
        {
            if (SelectedStudent_CheckStudent == null)
            {
                ErrorMessage = $" សូមជ្រើសរើសនិស្សិតជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                Debug.WriteLine(SelectedStudent_CheckStudent.ID);
                Debug.WriteLine($"{SelectedStudent_CheckStudent.Stu_EducationSubjects},{SelectedStudent_CheckStudent.Stu_EducationLevels},{SelectedStudent_CheckStudent.Stu_Gender},{SelectedStudent_CheckStudent.Stu_Birth_Province}");

                Full_Name_EN = SelectedStudent_CheckStudent.Stu_FirstName_EN + " " + SelectedStudent_CheckStudent.Stu_LastName_EN;
                Stu_Internship_Text = "Internship";
                //Student Info in English
                var get_stu_skill_english = _dbConnection.GetStu_SkillInfo_English(SelectedStudent_CheckStudent.Stu_EducationSubjects);
                if (!string.IsNullOrEmpty(get_stu_skill_english))
                {
                    Stu_Skill_English = get_stu_skill_english;
                }

                //Student Gender in English
                var get_stu_gender_eng = _dbConnection.GetStu_Gender_English(SelectedStudent_CheckStudent.Stu_Gender);
                if (!string.IsNullOrEmpty(get_stu_gender_eng))
                {
                    Stu_Gender_English = get_stu_gender_eng;
                }

                //Student Birth Place
                var get_stu_birth_place = _dbConnection.GetStu_BirthPlace_English(SelectedStudent_CheckStudent.Stu_Birth_Province);
                if (!string.IsNullOrEmpty(get_stu_birth_place))
                {
                    Stu_Place_Birth_English = get_stu_birth_place;
                }

                //Student Degree
                var get_stu_degree_eng = _dbConnection.GetStu_Degree_English(SelectedStudent_CheckStudent.Stu_EducationLevels);
                if (!string.IsNullOrEmpty(get_stu_degree_eng))
                {
                    Stu_Degree_English = get_stu_degree_eng;
                }

                var get_stu_class = _dbConnection.Get_Class_Student_Transcript(SelectedStudent_CheckStudent.ID);
                if (get_stu_class != null)
                {
                    Transcript_Class_Info.Clear();
                    foreach (var class_info in get_stu_class)
                    {
                        Transcript_Class_Info.Add(class_info);
                    }
                    Transcript_Class_Info = new ObservableCollection<Student_Info>(get_stu_class);
                }
                else
                {
                    ErrorMessage = $" Error ទិន្នន័យមកពី Database !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
            }



            await Task.CompletedTask;
        }
        //Multi Select Class Transcript
        private List<Student_Info> _Selected_All_Class_Transcript = new List<Student_Info>();
        public List<Student_Info> Selected_All_Class_Transcript
        {
            get => _Selected_All_Class_Transcript;
            set
            {
                _Selected_All_Class_Transcript = value;
                OnPropertyChanged(nameof(Selected_All_Class_Transcript));
            }
        }
        //Select all Attendent Teacher
        private List<Class_Schedule> _Selected_Teacher_Attendent;
        public List<Class_Schedule> Selected_Teacher_Attendent
        {
            get => _Selected_Teacher_Attendent;
            set
            {
                _Selected_Teacher_Attendent = value;
                OnPropertyChanged(nameof(Selected_Teacher_Attendent));
            }
        }
        //Select All Teacher Info Attendent
        private List<Class_Schedule> _Selected_Teacher_Info_Attendent;
        public List<Class_Schedule> Selected_Teacher_Info_Attendent
        {
            get => _Selected_Teacher_Info_Attendent;
            set
            {
                _Selected_Teacher_Info_Attendent = value;
                OnPropertyChanged(nameof(Selected_Teacher_Info_Attendent));
            }
        }
        //Command Show Score Info
        public ICommand Command_Show_Score_andSubject { get; set; }

        public async Task Show_Subject_and_Score_Transcript()
        {
            if (Selected_All_Class_Transcript == null || !Selected_All_Class_Transcript.Any())
            {
                ErrorMessage = $" សូមជ្រើសរើសថ្នាក់រៀនជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Transcript_Score_Info.Clear(); // Clear existing data
            int value_credit = 0;
            foreach (var classinfo in Selected_All_Class_Transcript)
            {


                Debug.WriteLine($"Select Class ID: {classinfo.Class_ID_Show}, Student ID: {SelectedStudent_CheckStudent.Stu_ID}");

                var list_subject_score = _dbConnection.GetSubject_Score_Transcript(classinfo.Class_ID_Show, SelectedStudent_CheckStudent.Stu_ID);
                var select_setting_score = _dbConnection.GetSetting_Score_Info();

                if (list_subject_score == null || !list_subject_score.Any())
                {
                    Debug.WriteLine("None Data Return.");
                    continue; // Continue instead of breaking the loop
                }

                foreach (var subject_score in list_subject_score)
                {
                    string gradeLetter = "N/A";
                    string gradeSystem = "N/A";

                    foreach (var setting_score in select_setting_score)
                    {
                        int Score1 = setting_score.Setting_Score1;
                        int Score2 = setting_score.Setting_Score2;
                        string Grade = setting_score.Setting_Letter_Grade;
                        string System = setting_score.Setting_Grade_System;

                        // Ensure the grade is correctly assigned
                        if (subject_score.Total_Score_Average >= Score1 && subject_score.Total_Score_Average <= Score2)
                        {
                            gradeLetter = Grade;  // Assign the correct grade
                            gradeSystem = System;  // Assign grade system if needed

                            Debug.WriteLine($"✅ Match Found! Grade Assigned: {gradeLetter}");
                            break;  // Stop searching once a match is found
                        }


                    }
                    if (subject_score.Score_Skill_TotalTime <= 30)
                    {
                        value_credit = 2;
                    }
                    else if (subject_score.Score_Skill_TotalTime >= 45 || subject_score.Score_Skill_TotalTime < 89)
                    {
                        value_credit = 3;
                    }
                    else if (subject_score.Score_Skill_TotalTime >= 90)
                    {
                        value_credit = 4;
                    }

                    // Assign the grade to the total_score_info object
                    subject_score.Report_Study_Credit = value_credit;
                    subject_score.Grade_Letter = gradeLetter;
                    subject_score.Grade_System = gradeSystem;
                    Transcript_Score_Info.Add(subject_score);
                }
            }

            await Task.CompletedTask;
        }

        //Export Transcript PDF
        public ICommand Command_Export_Transcript_PDF { get; set; }

        //Method 
        public async Task Export_Transcript_PDF()
        {
            if (Selected_Transcript_Items == null || !Selected_Transcript_Items.Any())
            {
                ErrorMessage = $" សូមជ្រើសរើសទិន្នន័យមុខវិជ្ជាជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់បង្កើតលិខិតបញ្ជាក់ការសិក្សារបស់និស្សិតនេះ មែនទេ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Red);
                CurrentOperation = "Export_Transcript_of_Education_PDF";
                OnPropertyChanged(nameof(CurrentOperation));
            }
            await Task.CompletedTask;
        }

        //HandleYes_Export_Transcript_PDF
        public void HandleYesExport_PDF_Transcript()
        {

            // Convert date to Khmer format and assign it to a property in _selectedStudent
            _selectedStudent.Stu_BirthdayDateShow = ConvertToKhmerDate(_selectedStudent.Stu_BirthdayDateOnly);
            PDFService_Generation_Student_Transcript.CreateReport(Selected_Transcript_Items, SelectedStudent_CheckStudent, Full_Name_EN, Stu_Skill_English, Stu_Gender_English, Stu_Place_Birth_English, Stu_Degree_English, Stu_Date_Graduation, Stu_Internship_Text, Stu_Internship_Credit, Stu_Internship_Grade);
            ErrorMessage = "PDF លិខិតព្រឹត្តិបត្រពិន្ទុ ត្រូវបានទាញចេញដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);
            Debug.WriteLine("Export_Transcript_of_Education_PDF OK");
        }


        //Report Transcript
        private int _Report_Study_Credit;
        public int Report_Study_Credit
        {
            get => _Report_Study_Credit;
            set
            {
                _Report_Study_Credit = value;
                OnPropertyChanged(nameof(Report_Study_Credit));
            }
        }
        private string _Report_StudyYear;
        public string Report_StudyYear
        {
            get => _Report_StudyYear;
            set
            {
                _Report_StudyYear = value;
                OnPropertyChanged(nameof(Report_StudyYear));
            }
        }
        private string _Report_Student_Year;
        public string Report_Student_Year
        {
            get => _Report_Student_Year;
            set
            {
                _Report_Student_Year = value;
                OnPropertyChanged(nameof(Report_Student_Year));
            }
        }
        private string _Report_Study_Semester;
        public string Report_Study_Semester
        {
            get => _Report_Study_Semester;
            set
            {
                _Report_Study_Semester = value;
                OnPropertyChanged(nameof(Report_Study_Semester));
            }
        }
        private string _Report_Study_Subject;
        public string Report_Study_Subject
        {
            get => _Report_Study_Subject;
            set
            {
                _Report_Study_Subject = value;
                OnPropertyChanged(nameof(Report_Study_Subject));
            }
        }

        //Multi Select Transcript Items
        private List<Class_Score> _Selected_Transcript_Items;
        public List<Class_Score> Selected_Transcript_Items
        {
            get { return _Selected_Transcript_Items; }
            set
            {
                _Selected_Transcript_Items = value;
                OnPropertyChanged(nameof(Selected_Transcript_Items));
            }
        }

        //Teacher Attendance
        private string _Class_Seletecd_Date;
        public string Class_Seletecd_Date
        {
            get => _Class_Seletecd_Date;
            set
            {
                _Class_Seletecd_Date = value;
                OnPropertyChanged(nameof(Class_Seletecd_Date));
            }
        }
        private int _TotalTime_Calculate;
        public int TotalTime_Calculate
        {
            get => _TotalTime_Calculate;
            set
            {
                _TotalTime_Calculate = value;
                OnPropertyChanged(nameof(TotalTime_Calculate));
            }
        }
        private int _Index;
        public int Index
        {
            get => _Index;
            set
            {
                _Index = value;
                OnPropertyChanged(nameof(Index));
            }
        }
        private string _Text_Reason_Attendent;
        public string Text_Reason_Attendent
        {
            get => _Text_Reason_Attendent;
            set
            {
                _Text_Reason_Attendent = value;
                OnPropertyChanged(nameof(Text_Reason_Attendent));
            }
        }
        private string _Teacher_Attendent_Show;
        public string Teacher_Attendent_Show
        {
            get => _Teacher_Attendent_Show;
            set
            {
                _Teacher_Attendent_Show = value;
                OnPropertyChanged(nameof(Teacher_Attendent_Show));
            }
        }
        private bool _IsAttendent;
        public bool IsAttendent
        {
            get => _IsAttendent;
            set
            {
                if (_IsAttendent != value)
                {
                    _IsAttendent = value;
                    Debug.WriteLine($"IsAttendent changed to: {_IsAttendent}");
                    OnPropertyChanged(nameof(IsAttendent));
                }
                OnPropertyChanged(nameof(IsAttendent));
            }
        }
        private DateTimeOffset? _SelectedDate_Attendent;
        public DateTimeOffset? SelectedDate_Attendent
        {
            get => _SelectedDate_Attendent;
            set
            {
                if (_SelectedDate_Attendent != value)
                {
                    _SelectedDate_Attendent = value;
                    OnPropertyChanged(nameof(SelectedDate_Attendent));

                    // Optional: Update other properties if needed
                    OnPropertyChanged(nameof(DateTime_Attendent_Value));
                }
            }
        }


        public string DateTime_Attendent_Value
        {
            
            get => SelectedDate_Attendent?.ToString("dd/MM/yyyy") ?? "No Date Selected";
        }



        //Command Show Class
        public ICommand Command_Show_Class_For_Attendene { get; set; }

        public async Task ShowClass_For_Teacher_Attendence()
        {
            if(Class_In_Study_Year_Select == null)
            {
                ErrorMessage = $" សូមជ្រើសរើសឆ្នាំសិក្សា ជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if(Class_In_Study_Timeshift_Select == null)
            {
                ErrorMessage = $" សូមជ្រើសរើសវេនសិក្សា ជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if(string.IsNullOrEmpty(Current_Class_State))
            {
                ErrorMessage = $" សូមជ្រើសរើសស្ថានភាពថ្នាក់រៀន ជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Comfirm_Value_Attendence();      
            Classes_Info_Attendence.Clear();

            var get_class_progress = _dbConnection.GetClasses_Progress(Class_In_Study_Year, Class_Seletecd_Date, Class_In_Study_Timeshift, Current_Class_State);

            foreach(var class_progrss in  get_class_progress)
            {
                Classes_Info_Attendence.Add(class_progrss);
            }
            Classes_Info_Attendence = new ObservableCollection<Class_Schedule>(get_class_progress);
            await Task.CompletedTask;
        }
        public void Comfirm_Value_Attendence()
        {
            Class_In_Study_Year =this. Class_In_Study_Year_Select.Stu_StudyYear;
            Class_Seletecd_Date = this.Class_Seletecd_Date;
            Class_In_Study_Timeshift =this. Class_In_Study_Timeshift_Select.Stu_StudyTimeShift;
            Current_Class_State = this.Current_Class_State;
            

            Debug.WriteLine($"Current_Class_State: {Current_Class_State}");
            Debug.WriteLine($"Class_In_Study_Timeshift: {Class_In_Study_Timeshift}");
            Debug.WriteLine($"Class_Seletecd_Date: {Class_Seletecd_Date}");
            Debug.WriteLine($"Class_In_Study_Year: {Class_In_Study_Year}");
        }
        
        //Save Teacher Attendent
        public ICommand Command_Save_Teacher_Attendent { get; set; }

        //Method Save
        public async Task SaveTeacher_Attendent()
        {
            UpdateSelectedDate();
            if (Selected_Teacher_Attendent == null || !Selected_Teacher_Attendent.Any())
            {
                ErrorMessage = $" សូមជ្រើសរើសទិន្នន័យគ្រូបច្ចេកទេស ជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            if (DateTime_Attendent_Value == null || DateTime_Attendent_Value == "No Date Selected")
            {
                ErrorMessage = $" សូមជ្រើសរើសពេលវេលាជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                Debug.WriteLine($"DateTime_Attendent_Value: {DateTime_Attendent_Value}");
                _ = SaveTeacher_Time_Attendent();
            }
            await Task.CompletedTask;
        }
        
        public async Task SaveTeacher_Time_Attendent()
        {
            Class_In_Study_Year = this.Class_In_Study_Year_Select.Stu_StudyYear;
            string DateTime_Attendent = this.DateTime_Attendent_Value;
            Class_Seletecd_Date = this.Class_Seletecd_Date;
            Class_In_Study_Timeshift = this.Class_In_Study_Timeshift_Select.Stu_StudyTimeShift;
            Current_Class_State = this.Current_Class_State;

            

            Debug.WriteLine($"DateTime_Start_Schedule_Strating: {DateTime_Start_Schedule_Strating}");
            Debug.WriteLine($"Test Date: {DateTime_Attendent}");

            foreach(var save_info in Selected_Teacher_Attendent)
            {
                //Check Before Insert
                var check_teacher_attendent_info = await _dbConnection.Check_Teacher_Attendent_Info(DateTime_Attendent, Class_In_Study_Year, Class_Seletecd_Date, Class_In_Study_Timeshift, save_info.Class_ID_Schedule, Current_Class_State);

                if (check_teacher_attendent_info.DateTime_Attendent1 == DateTime_Attendent &&
                   check_teacher_attendent_info.Class_In_Study_Year1 == Class_In_Study_Year &&
                   check_teacher_attendent_info.Class_Seletecd_Date1 == Class_Seletecd_Date &&
                   check_teacher_attendent_info.Class_In_Study_Timeshift1 == Class_In_Study_Timeshift &&
                   check_teacher_attendent_info.Class_ID_Schedule1 == save_info.Class_ID_Schedule &&
                   check_teacher_attendent_info.Current_Class_State1 == Current_Class_State)
                {
                    ErrorMessage = "កាលបរិច្ឆេទ៖ " + DateTime_Attendent + "មានទិន្នន័យដូចគ្នារួចស្រេចហើយ !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-fail-96.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }

                bool success = _dbConnection.Save_TeacherAttendent(save_info, Class_In_Study_Year, DateTime_Attendent, Class_Seletecd_Date, Class_In_Study_Timeshift, Current_Class_State);
                if(success )
                {
                    Debug.WriteLine($"Save_TeacherAttendent Success. {save_info.SD_Skill_Name}");          
                }
                else
                {
                    ErrorMessage = $" ទិន្នន័យវត្តមានគ្រូបច្ចេកទេសបានរក្សាទុក បរាជ័យ!";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }
                
            }
            ErrorMessage = " ទិន្នន័យវត្តមានគ្រូបច្ចេកទេសបានរក្សាទុក ដោយជោគជ័យ";
            ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
            MessageColor = new SolidColorBrush(Colors.Green);

            _ = Load_Date_Teacher_Attendents_Info();
            Classes_Info_Attendence.Clear();
        }

        private DateTimeOffset? _SelectedDate_Search;
        public DateTimeOffset? SelectedDate_Search
        {
            get => _SelectedDate_Search;
            set
            {
                if (_SelectedDate_Search != value)
                {
                    _SelectedDate_Search = value;
                    OnPropertyChanged(nameof(SelectedDate_Search));
                    OnPropertyChanged(nameof(FormattedDate_SelectedDate_Search));
                }
            }
        }
        public string FormattedDate_SelectedDate_Search
        {
            get => SelectedDate_Search?.ToString("MM/yyyy") ?? "No Date Selected";
        }

        //Command Search
        public ICommand Command_Search_Date_Teacher_Info { get; set; }
        public ICommand Command_Clear_Data_Teacher_Info { get; set; }

        //Method Search
        public async Task Search_Date_Teacher_Attendents_Info()
        {
            if(FormattedDate_SelectedDate_Search == null || FormattedDate_SelectedDate_Search == "No Date Selected")
            {
                ErrorMessage = $" សូមជ្រើសរើសពេលវេលាជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            var search_date = _dbConnection.GetDate_Search_Teacher_Attendent(FormattedDate_SelectedDate_Search);
            if (search_date == null)
            {
                ErrorMessage = $" Error ក្នុងការទាញយកទិន្នន័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Date_Teacher_Info_Attendence.Clear();

                foreach(var date in search_date)
                {
                    Date_Teacher_Info_Attendence.Add(date);
                }
                Date_Teacher_Info_Attendence = new ObservableCollection<Class_Schedule>(search_date);

            
            Debug.WriteLine($"FormattedDate_SelectedDate_Search: {FormattedDate_SelectedDate_Search}");
            await Task.CompletedTask;
        }

        //Method Clear Attendent
        public async Task Clear_Search_Date()
        {
            SelectedDate_Search = null;
            _=Load_Date_Teacher_Attendents_Info();
            await Task.CompletedTask;
        }
        //Method load Data Teacher_Attendents
        public async Task Load_Date_Teacher_Attendents_Info()
        {
            var search_date = _dbConnection.LoadDate_Search_Teacher_Attendent();
            if (search_date == null)
            {
                ErrorMessage = $" Error ក្នុងការទាញយកទិន្នន័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }

            Date_Teacher_Info_Attendence.Clear();

            foreach (var date in search_date)
            {
                Date_Teacher_Info_Attendence.Add(date);
            }
            Date_Teacher_Info_Attendence = new ObservableCollection<Class_Schedule>(search_date);

            await Task.CompletedTask;
        }

        private string _DateTime_Attendent;
        public string DateTime_Attendent
        {
            get => _DateTime_Attendent;
            set
            {
                _DateTime_Attendent = value;
                OnPropertyChanged(nameof(DateTime_Attendent));
            }
        }
        //Command Show Data Info from click date
        public ICommand Command_ShowData_Click_Date { get; set; }
        public ICommand Command_Delete_Date_Click_Date { get; set; }

        //Select in ListView
        private Class_Schedule _Selected_Date_Items_Show_Info;
        public Class_Schedule Selected_Date_Items_Show_Info
        {
            get => _Selected_Date_Items_Show_Info;
            set
            {
                if(_Selected_Date_Items_Show_Info != value)
                {
                    _Selected_Date_Items_Show_Info = value;
                    OnPropertyChanged(nameof(Selected_Date_Items_Show_Info));

                    if(_Selected_Date_Items_Show_Info == null)
                    {
                        DateTime_Attendent = string.Empty;
                    }
                    else
                    {
                        DateTime_Attendent = _Selected_Date_Items_Show_Info.DateTime_Attendent;
                        
                    }
                }
            }
        }
        private int _Class_In_Study_Year_Show;
        public int Class_In_Study_Year_Show
        {
            get => _Class_In_Study_Year_Show;
            set
            {
                _Class_In_Study_Year_Show = value;
                OnPropertyChanged(nameof(Class_In_Study_Year_Show));
            }
        }
        private int _Class_In_Semester_Show;
        public int Class_In_Semester_Show
        {
            get => _Class_In_Semester_Show;
            set
            {
                _Class_In_Semester_Show = value;
                OnPropertyChanged(nameof(Class_In_Semester_Show));
            }
        }
        //Method Show Date Info from click date
        public async Task ShowData_Click_Date()
        {
            if(Selected_Date_Items_Show_Info == null)
            {
                ErrorMessage = $" សូមជ្រើសរើសពេលវេលាក្នុងតារាង ជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            DateTime_Attendent = _Selected_Date_Items_Show_Info.DateTime_Attendent;
            Debug.WriteLine($"Selected_Date_Items_Show_Info: {DateTime_Attendent}");

            var data_info_from_click_date = _dbConnection.Get_DataInfo_From_ClickDate(DateTime_Attendent);

            if(data_info_from_click_date == null)
            {
                ErrorMessage = $" Error ក្នុងការទាញយកទិន្នន័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            Classes_Info_Attendence_S.Clear();

            foreach(var data_info in data_info_from_click_date)
            {
                Classes_Info_Attendence_S.Add(data_info);
            }
            Classes_Info_Attendence_S = new ObservableCollection<Class_Schedule>(data_info_from_click_date);
            await Task.CompletedTask;
        }

        //Method Delete Data Info from click date
        public async Task Delete_Date_Click_Date()
        {
            if (Selected_Date_Items_Show_Info == null)
            {
                ErrorMessage = $" សូមជ្រើសរើសពេលវេលាក្នុងតារាង ជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                ErrorMessage_Delete = $"តើអ្នកពិតជាចង់លុបទិន្នន័យទាំងនេះមែនទេ ({Selected_Date_Items_Show_Info.DateTime_Attendent}) ?";
                ErrorImageSource_Delete = new BitmapImage(new Uri("ms-appx:///Assets/Setting/icons8-question.gif"));
                MessageColor_Delete = new SolidColorBrush(Colors.Yellow);
                CurrentOperation = "Delete_Data_DateAttendent";
            }
            

            await Task.CompletedTask;
        }

        //Handle Yes Delete Attendent
        public void HandleYes_Delete_Data_Attendent_Info()
        {
            DateTime_Attendent = _Selected_Date_Items_Show_Info.DateTime_Attendent;

            bool success_delete = _dbConnection.Delete_DataClick_Date(DateTime_Attendent);
            if (success_delete)
            {
                ErrorMessage = " ទិន្នន័យវត្តមានគ្រូបច្ចេកទេសបានលុប ដោយជោគជ័យ";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);

                _ = Load_Date_Teacher_Attendents_Info();
            }
            else
            {
                ErrorMessage = $" ទិន្នន័យលុបបរាជ័យ !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
        }

        private Class_Schedule _First_Selected_Teacher_Info_Attendent;
        public Class_Schedule First_Selected_Teacher_Info_Attendent
        {
            get => _First_Selected_Teacher_Info_Attendent;
            set
            {
                _First_Selected_Teacher_Info_Attendent = value;
                OnPropertyChanged(nameof(First_Selected_Teacher_Info_Attendent));
            }
        }
        //Command Edit Delete Teacher Attendent
        public ICommand Command_Edit_Teacher_Attendent_Info { get; set; }
        public ICommand Command_Delete_Teacher_Attendent_Info { get; set; }

        //Can Edit
        private bool _Can_Edit_Attendet;
        public bool Can_Edit_Attendet
        {
            get => _Can_Edit_Attendet;
            set
            {
                _Can_Edit_Attendet = value;
                OnPropertyChanged(nameof(Can_Edit_Attendet));
            }
        }
        //Method Edit Teacher Attendet
        public async Task Edit_Teacher_Attendent_Info()
        {
            if(Selected_Teacher_Info_Attendent == null || !Selected_Teacher_Info_Attendent.Any())
            {
                Can_Edit_Attendet = false;
                ErrorMessage = $" សូមជ្រើសរើសទិន្នន័យក្នុងតារាងជាមុនសិន !";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                MessageColor = new SolidColorBrush(Colors.Red);
                return;
            }
            Classes_Info_Attendence.Clear();
            foreach (var select_item in Selected_Teacher_Info_Attendent)
            {
                select_item.Class_In_Study_Year = select_item.Class_In_Study_Year_Show.ToString();
                select_item.Class_In_Semester = select_item.Class_In_Semester_Show.ToString();
                Classes_Info_Attendence.Add(select_item);

                Class_In_Study_Year_Select = EducationStudyYear_Combobox
               .FirstOrDefault(stu_year => stu_year.Stu_StudyYear == select_item.SD_Class_Study_Year);
                OnPropertyChanged(nameof(Class_In_Study_Year_Select));

                Class_Seletecd_Date = select_item.Class_Seletecd_Date;

                Class_In_Study_Timeshift_Select = EducationStudyTimeShift_Combobox
               .FirstOrDefault(stu_timeshift => stu_timeshift.Stu_StudyTimeShift == select_item.SD_Class_TimeShift);
                OnPropertyChanged(nameof(Class_In_Study_Timeshift_Select));

                Current_Class_State = select_item.Current_Class_State;
                SetSelectedDate(select_item.DateTime_Attendent);

                Can_Edit_Attendet = true;
            }
            await Task.CompletedTask;
        }
        private int _ID_Show;
        public int ID_Show
        {
            get => _ID_Show;
            set
            {
                _ID_Show = value;
                OnPropertyChanged(nameof(ID_Show));
            }
        }
        public void SetSelectedDate(string dateString)
        {
            try
            {
                if (!string.IsNullOrEmpty(dateString))
                {
                    // Convert "20/02/2025" (DD/MM/YYYY) to DateTimeOffset
                    DateTime parsedDate = DateTime.ParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    SelectedDate_Attendent = new DateTimeOffset(parsedDate);
                }
            }
            catch (FormatException)
            {
                Debug.WriteLine($"Invalid date format: {dateString}");
                SelectedDate_Attendent = null;
            }
        }

        //Commmand Clear
        public ICommand Command_Clear_Teacher_Attendent_Info { get; set; }

        //Method Clear
        public async Task Clear_List_Teacher_Attendent()
        {
            Classes_Info_Attendence.Clear();
            Class_Seletecd_Date = null;

            Class_In_Study_Timeshift_Select = EducationStudyTimeShift_Combobox
           .FirstOrDefault(stu_timeshift => stu_timeshift.Stu_StudyTimeShift == null);
            OnPropertyChanged(nameof(Class_In_Study_Timeshift_Select));

            Current_Class_State = null;
            SelectedDate_Attendent = DateTimeOffset.Now;
            Can_Edit_Attendet = false;

            await Task.CompletedTask;
        }

        //Update Teacher Attendent
        public ICommand Command_Update_Teacher_Attendent { get; set; }

        //Method Update
        public async Task Update_Teacher_Attendent_Info()
        {
            if(Can_Edit_Attendet == true)
            {
                if(Selected_Teacher_Attendent == null || !Selected_Teacher_Attendent.Any())
                {
                    ErrorMessage = $" សូមជ្រើសរើសទិន្នន័យក្នុងតារាងជាមុនសិន !";
                    ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                    MessageColor = new SolidColorBrush(Colors.Red);
                    return;
                }

                Class_In_Study_Year = this.Class_In_Study_Year_Select.Stu_StudyYear;
                string DateTime_Attendent = this.DateTime_Attendent_Value;
                Class_Seletecd_Date = this.Class_Seletecd_Date;
                Class_In_Study_Timeshift = this.Class_In_Study_Timeshift_Select.Stu_StudyTimeShift;
                Current_Class_State = this.Current_Class_State;


                foreach (var update_info in Selected_Teacher_Attendent)
                {
                    Debug.WriteLine("Can Edit OK");
                    Debug.WriteLine(update_info.ID_Show);

                    bool success = _dbConnection.Update_Teacher_Info_Attendent_Info(update_info);

                    if(success)
                    {
                        Debug.WriteLine($"Update {update_info.ID_Show} Success.");
                    }
                    else
                    {
                        ErrorMessage = $" ធ្វើបច្ចុប្បន្នភាពបរាជ័យ !";
                        ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-warning-100.png"));
                        MessageColor = new SolidColorBrush(Colors.Red);
                        return;
                    }

                }
                _ = Clear_List_Teacher_Attendent();
                ErrorMessage = " ទិន្នន័យវត្តមានគ្រូបច្ចេកទេសបានធ្វើបច្ចុប្បន្នភាព ដោយជោគជ័យ";
                ErrorImageSource = new BitmapImage(new Uri("ms-appx:///Assets/icons8-check-96.png"));
                MessageColor = new SolidColorBrush(Colors.Green);

            }
            else
            {
                Debug.WriteLine("No Edit.");
                return;
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
