using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.Model
{
    public class Class_Schedule
    {
        public int Index { get; set; }
        public int ID_Show { get; set; }
        public int Schedule_ID { get; set; }
        public string Schedule_Name { get; set; }
        public int Class_ID_Schedule { get; set; }
        public string Schedule_State { get; set; }
        public string SD_Class_Name { get; set; }
        public string SD_Class_TimeShift { get; set; }
        public string SD_Class_Study_Year { get; set; }
        public string DateTime_Attendent { get; set; }
        public string Class_Seletecd_Date { get;set; }
        public string Current_Class_State { get; set; } 

        public TimeSpan SD_Start_DateTime_MF1 { get; set; }
        public TimeSpan SD_End_DateTime_MF1 { get; set; }
        public TimeSpan SD_Start_DateTime_MF2 { get;set; }
        public TimeSpan SD_End_DateTime_MF2 { get; set; }

        public TimeSpan SD_Start_DateTime_SS1 { get; set; }
        public TimeSpan SD_End_DateTime_SS1 { get; set; }
        public TimeSpan SD_Start_DateTime_SS2 { get; set; }
        public TimeSpan SD_End_DateTime_SS2 { get; set; }

        public int TotalTime_Calculate { get; set; }
        public int Number_of_Hours_left { get; set; }
        public int Number_of_Hours_Current { get; set; }

        public string Text_Reason_Attendent { get; set; }
        public string Teacher_Attendent { get; set; }
        public string Teacher_Attendent_Show { get; set; }
        public bool IsAttendent { get; set; }
        public string Status { get; set; }
        public string Stu_BirthdayDateOnly { get; set; }

        //All TextBox
        public int SD_Skill_ID { get; set; }
        public string SD_Skill_Name { get; set; }
        public int SD_Teacher_ID { get; set; }
        public string SD_Teacher_Name { get; set; }
        public string Class_In_Skill { get; set; }
        public string Class_In_Study_Year { get; set; }
        public string Class_In_Semester { get; set; }
        public int Class_In_Generation { get; set; }

        //Monday
        public int SelectedSkill_SD_Mon1 { get; set; }
        public string SD_Skill_Name_Mon1 { get;set; }        
        public int SelectedTeacher_SD_Mon1 { get;set; }
        public string SD_Teacher_Mon01 { get;set; }
        public int SD_TotalTime_Mon1 { get; set; }

        public int SelectedSkill_SD_Mon2 { get; set; }
        public string SD_Skill_Name_Mon2 { get; set; }
        public int SelectedTeacher_SD_Mon2 { get; set; }
        public string SD_Teacher_Mon02 { get; set; }
        public int SD_TotalTime_Mon2 { get; set; }

        //Tuesday
        public int SelectedSkill_SD_Tues1 { get; set; }
        public string SD_Skill_Name_Tues1 { get; set; }
        public int SelectedTeacher_SD_Tues1 { get; set; }
        public string SD_Teacher_Tues01 { get; set; }
        public int SD_TotalTime_Tues1 { get; set; }

        public int SelectedSkill_SD_Tues2 { get; set; }
        public string SD_Skill_Name_Tues2 { get; set; }
        public int SelectedTeacher_SD_Tues2 { get; set; }
        public string SD_Teacher_Tues02 { get; set; }
        public int SD_TotalTime_Tues2 { get; set; }

        //Wednesay
        public int SelectedSkill_SD_Wed1 { get; set; }
        public string SD_Skill_Name_Wed1 { get; set; }
        public int SelectedTeacher_SD_Wed1 { get; set; }
        public string SD_Teacher_Wed1 { get; set; }
        public int SD_TotalTime_Wed1 { get; set; }

        public int SelectedSkill_SD_Wed2 { get; set; }
        public string SD_Skill_Name_Wed2 { get; set; }
        public int SelectedTeacher_SD_Wed2 { get; set; }
        public string SD_Teacher_Wed2 { get; set; }
        public int SD_TotalTime_Wed2 { get; set; }

        //Thursday
        public int SelectedSkill_SD_Thur1 { get; set; }
        public string SD_Skill_Name_Thur1 { get; set; }
        public int SelectedTeacher_SD_Thur1 { get; set; }
        public string SD_Teacher_Thur1 { get; set; }
        public int SD_TotalTime_Thur1 { get; set; }

        public int SelectedSkill_SD_Thur2 { get; set; }
        public string SD_Skill_Name_Thur2 { get; set; }
        public int SelectedTeacher_SD_Thur2 { get; set; }
        public string SD_Teacher_Thur2 { get; set; }
        public int SD_TotalTime_Thur2 { get; set; }

        //Friday
        public int SelectedSkill_SD_Fri1 { get; set; }
        public string SD_Skill_Name_Fri1 { get; set; }
        public int SelectedTeacher_SD_Fri1 { get; set; }
        public string SD_Teacher_Fri1 { get; set; }
        public int SD_TotalTime_Fri1 { get; set; }

        public int SelectedSkill_SD_Fri2 { get; set; }
        public string SD_Skill_Name_Fri2 { get; set; }
        public int SelectedTeacher_SD_Fri2 { get; set; }
        public string SD_Teacher_Fri2 { get; set; }
        public int SD_TotalTime_Fri2 { get; set; }

        //ItemsSource="{Binding Schedule_Teacher_Name_Combobox}"
        //DisplayMemberPath="SD_Teacher_Name"
        //SelectedValuePath="SD_Teacher_ID"

        //Satureday
        public int SelectedSkill_SD_Sat1 { get; set; }
        public string SD_Skill_Name_Sat1 { get; set; }
        public int SelectedTeacher_SD_Sat1 { get; set; }
        public string SD_Teacher_Sat1 { get; set; }
        public int SD_TotalTime_Sat1 { get; set; }

        public int SelectedSkill_SD_Sat2 { get; set; }
        public string SD_Skill_Name_Sat2 { get; set; }
        public int SelectedTeacher_SD_Sat2 { get; set; }
        public string SD_Teacher_Sat2 { get; set; }
        public int SD_TotalTime_Sat2 { get; set; }

        //Sunday
        public int SelectedSkill_SD_Sun1 { get; set; }
        public string SD_Skill_Name_Sun1 { get; set; }
        public int SelectedTeacher_SD_Sun1 { get; set; }
        public string SD_Teacher_Sun1 { get; set; }
        public int SD_TotalTime_Sun1 { get; set; }

        public int SelectedSkill_SD_Sun2 { get; set; }
        public string SD_Skill_Name_Sun2 { get; set; }
        public int SelectedTeacher_SD_Sun2 { get; set; }
        public string SD_Teacher_Sun2 { get; set; }
        public int SD_TotalTime_Sun2 { get; set; }

        //DateTime Start+Building
        public string DateTime_Start_Schedule_Strating { get; set; }
        public DateTime DateTime_StartSchedule_Date { get; set; }
        public string SD_Building_Name { get; set; }
        public string SD_Building_Room { get; set; }

        public int Selected_Date_Items_Show_Info { get; set; }
        public int Class_In_Study_Year_Show { get; set; }
        public int Class_In_Semester_Show { get; set; }
        //public int Class_In_Generation_Show { get; set; }


    }
}
