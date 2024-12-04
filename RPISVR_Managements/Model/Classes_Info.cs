using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.Model
{
    public class Classes_Info
    {
        public string Search_Class_Search_Name_Generation { get; set; }
        public int Class_ID { get; set; }
        public string Class_Name { get; set; }

        public int Class_In_Skill_ID { get; set; }
        public string Class_In_Skill { get; set; }
        public int Class_In_Skill_Select { get; set; }

        public int Class_In_Level_ID { get; set; }
        public string Class_In_Level { get; set; }
        public int Class_In_Level_Select { get; set; }

        public int Class_In_Study_Year_ID { get; set; }
        public string Class_In_Study_Year { get; set; }
        public int Class_In_Study_Year_Select { get; set; }

        public int Class_In_Student_Year_ID { get; set; }
        public string Class_In_Student_Year { get; set; }
        public int Class_In_Student_Year_Select { get; set; }

        public int Class_In_Semester { get; set; }
        public int Class_In_Semester_Select { get; set; }

        public int Class_In_Generation { get; set; }
        public int Class_In_Generation_Select { get; set; }

        public string Class_In_Study_Timeshift { get; set; }
        public int Class_In_Study_Timeshift_Select { get; set; }
    }
}
