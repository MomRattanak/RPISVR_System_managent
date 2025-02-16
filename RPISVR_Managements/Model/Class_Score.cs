using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.Model
{
    public class Class_Score
    {
        public int Score_Class_ID { get; set; }
        public string Score_Class_Name { get; set; }
        public string Score_Class_State { get; set; }
        public string Score_TimeShift { get; set; }

        public int Score_Type_ID { get; set; }
        public string Score_Type_Name { get; set; }
        public int Selected_Score_Type { get;set; }
        public string Show_Score_Type { get; set; }
        public string State_Score_Type { get; set; }
        public int Selected_State_Skill_Score_Type { get; set; }

        public int Score_Skill_ID { get; set; }
        public int Score_Schedule_ID { get; set; }
        public int Score_Skill_TotalTime { get; set; }
        public string Score_Skill_Name { get; set; }
        public string Score_Skill_TeacherName { get;set; }
        public string Score_Skill_Day { get; set; }
        public int Selected_Skill_Name { get; set; }

        public int Score_Stu_ID { get; set; }
        public string Score_Student_ID { get; set; }
        public string Score_Student_Name { get; set; }
        public string Score_Student_Gender { get; set; }
        public string Score_Student_BirthDay { get; set; }
        public int Student_Score { get;set; }
        public byte[] ProfileImageBytes { get; set; }
        public BitmapImage Stu_Image_Source { get; set; }

        //Setting Score
        public int Setting_Score_ID { get; set; }
        public int Setting_Score1 { get; set; }
        public int Setting_Score2 { get; set; }
        public string Setting_Letter_Grade { get; set; }
        public int Setting_GPA_Value { get;set; }
        public string Setting_Grade_System { get;set; }

        //Calculate Score
        public int Total_Score { get; set; }
        public int Total_Count_Score_Type { get; set; }
        public float Total_Score_Average { get; set; }
        public float Average_Student { get; set; }
        public int Rank_Student { get; set; }
        public string Grade_Letter { get; set; }
        public int GPA_Value { get; set; }
        public string Grade_System { get; set; }
        public int Total_Students { get; set; }

        public float Total_Score_Show { get; set; }
        public float Total_Score_Average_Show { get; set; }
        public string Student_Pass_State { get; set; }

        //Report Transcript
        public string Report_StudyYear { get; set; }
        public string Report_Student_Year { get; set; }
        public string Report_Study_Semester { get; set; }
        public string Report_Study_Subject { get; set; }
        public int Report_Study_Credit { get; set; }
        //public string Report_TotalScore { get; set; }
        //public string Report_Study_Description { get; set; }
        //public string Report_Status { get; set; }
        //public string Report_Date { get; set; }

    }
}
