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
    }
}
