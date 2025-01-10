using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.Model
{
    public class Curriculum_Info
    {
        //Curriculum Model
        public int C_ID { get; set; }
        public string Curriculum_ID { get; set; }
        public string Curriculum_Name_KH { get; set; }
        public string Curriculum_Name_EN { get; set; }  
        public int Curriculum_Skill_ID { get; set; }
        public string Curriculum_Skill_Name { get; set; }
        public int SelectedCurriculum_Skill_ID { get; set; }
        public int Curriculum_Level_ID {  get; set; }
        public string Curriculum_Level_Name { get; set; }
        public int SelectedCurriculum_Level_ID { get; set; }
        public int Curriculum_Teacher_ID { get; set; }
        public string Curriculum_Teacher_Name { get; set; }
        public int SelectedCurriculum_Teacher_ID { get; set; }
        public string Curriculum_Study_Year { get; set; }
        public string Curriculum_Semester { get; set; }
        public int Curriculum_Total_Time { get; set; }
        public int Curriculum_Total_Score { get; set; }

        //Search_Table_Curriculum
        public int Selected_Search_Curriculum_Skill_ID { get; set; }
        public int Selected_Search_Curriculum_Level_ID { get; set; }
        public string Curriculum_Search_Study_Year { get; set; }
    }
}
