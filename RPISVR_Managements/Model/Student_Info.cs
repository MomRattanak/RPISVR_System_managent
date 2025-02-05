using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.Model
{
    public class Student_Info
    {
        public int ID { get; set; }
        public string Stu_ID { get; set; }
        public string Stu_IDShow { get; set; }
        public string Stu_FirstName_KH { get; set; }
        public string Stu_LastName_KH { get; set; }
        public string Full_Name_KH { get; set; }
        public string Stu_FirstName_EN { get; set; }
        public string Stu_LastName_EN { get; set; }
        public string Full_Name_EN {  get; set; }
        public string Stu_Gender { get; set; }
        public string Stu_GenderShow { get; set; }
        public string Stu_StateFamily { get; set; }
        public string Stu_BirthdayDateOnly { get; set; } 
        public string Stu_BirthdayDateShow { get; set; }
        public string Stu_Generation { get; set; }
        //QRCode Information
        public byte[] QRCodeBytes { get; set; }
        //Education Level
        public int Stu_EducationLevels_ID { get; set; }
        public string Stu_EducationLevels { get; set; }
        public int SelectedEducationLevels_Info { get; set; }
        //Education Subject
        public int Stu_EducationSubject_ID { get; set; }
        public string Stu_EducationSubjects { get; set; }
        public int SelectedEducationSubjects_Info {get; set; }
        //Stu_StudyTimeShift
        public int Stu_StudyTimeShift_ID { get; set; }
        public string Stu_StudyTimeShift { get; set; }
        public int SelectedStu_StudyTimeShift_Info { get; set; }

        public string Stu_PhoneNumber { get; set; }
        //Stu_EducationType
        public int Stu_EducationType_ID { get; set; }
        public string Stu_EducationType { get; set; }
        public int SelectedStu_EducationType_Info { get; set; }

        public string Stu_NationalID { get; set; }
        public string Stu_StudyingTime { get; set; }
        //Select Province
        public int Stu_Birth_Province_ID { get; set; }
        public string Stu_Birth_Province { get; set; }
        public int SelectedBirthProvince_Info { get; set; }
        //Select District
        public int Stu_Birth_District_ID { get; set; }
        public string Stu_Birth_Distric { get; set; }
        public int SelectedBirthDistrict_Info { get; set; }
        //Select Commune
        public int Stu_Birth_Commune_ID { get; set; }
        public string Stu_Birth_Commune { get; set; }
        public int SelectedBirthCommune_Info { get; set; }
        //Select Village
        public int Stu_Birth_Village_ID { get; set; }
        public string Stu_Birth_Village { get; set; }
        public int SelectedBirthVillage_Info { get; set; }
        //Live_Pro
        public int Stu_Live_Pro_ID {  get; set; }
        public string Stu_Live_Pro { get; set; }
        public int SelectedLiveProvince_Info { get; set; }
        //Live_Dis
        public int Stu_Live_Dis_ID { get; set; }
        public string Stu_Live_Dis { get; set; }
        public int SelectedLiveDistrict_Info { get; set; }
        //Live_Comm
        public int Stu_Live_Comm_ID { get; set; }
        public string Stu_Live_Comm { get; set; }
        public int SelectedLiveCommune_Info { get; set; }
        //Live_Vill
        public int Stu_Live_Vill_ID { get; set; }
        public string Stu_Live_Vill { get; set; }
        public string SelectedLiveVillage_Info { get; set; }

        public string Stu_Jobs { get; set; }
        public string Stu_School { get; set; }
        //StudyYear
        public int Stu_StudyYear_ID { get; set; }
        public string Stu_StudyYear { get; set; }
        public int SelectesStu_StudyYear_Info { get; set; }

        public string Stu_Semester { get; set; }
        public string Stu_StatePoor { get; set; }
        public string Stu_Mother_Name { get; set; }
        public string Stu_Mother_Phone { get; set; }
        public string Stu_Mother_Job { get; set; }
        public string Stu_Father_Name { get; set; }
        public string Stu_Father_Phone { get; set; }
        public string Stu_Father_Job { get; set; }
        public string Stu_Image_YesNo { get; set; }
        public byte[] ProfileImageBytes { get; set; }
        public BitmapImage Stu_Image_Source { get; set; }
        public string Stu_Image_Total_Big { get; set; }
        public string Stu_Image_TotalSmall { get; set; }
        public string Stu_Images_Degree_Yes_No { get; set; }
        public BitmapImage Stu_Image_Degree_Source { get; set; }
        public byte[] Stu_Image_Degree_Bytes { get; set; }
        public string Stu_ImageBirth_Cert_YesNo { get; set; }
        public BitmapImage Stu_ImageBirth_Cert_Source { get;set; }
        public byte[] Stu_ImageBirth_Cert_Bytes { get; set; }
        public string Stu_ImageIDNation_YesNo { get; set; }
        public BitmapImage Stu_ImageIDNation_Source { get;set;}
        public byte[] Stu_ImageIDNation_Bytes { get; set; }
        public string Stu_ImagePoor_Card_YesNo { get; set; }
        public BitmapImage Stu_ImagePoor_Card_Source { get; set; }
        public byte[] Stu_Image_Poor_Card_Bytes { get; set; }
        public string Stu_Insert_by_ID { get; set; }
        public DateTime Stu_Insert_DateTime { get; set; }
        public string Stu_Insert_Info {  get; set; }
        public string Stu_Update_By_ID { get; set; }
        public DateTime Stu_Update_DateTime { get; set; }
        public string Stu_Update_Info { get; set; }
        public string Stu_Delete_By_ID { get; set; }
        public DateTime Stu_Delete_DateTime { get; set; }
        public string Stu_Delete_Info { get; set; }

        //Classes
        public string No_Class { get; set; }
        public string Class_ID { get; set; }
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

        public string Class_In_Semester { get; set; }
        public int Class_In_Semester_Select { get; set; }

        public string Class_In_Generation { get; set; }
        public int Class_In_Generation_Select { get; set; }

        public string Class_In_Study_Timeshift { get; set; }
        public int Class_In_Study_Timeshift_Select { get; set; }

        public string Class_In_Study_Type { get; set; }
        public int Class_In_Study_Type_Select { get; set; }

        //Search Class
        public string Search_Class_In_Skill { get; set; }
        public int Search_Class_In_Skill_Select { get; set; }
        public string Search_Class_In_Level { get; set; }
        public int Search_Class_In_Level_Select { get; set; }
        public string Search_Class_In_Study_Year { get; set; }
        public int Search_Class_In_Study_Year_Select { get; set; }
        public string Search_Class_In_Student_Year { get; set; }
        public string Search_Class_Semester { get; set; }
        public string Search_Class_In_Study_Timeshift { get; set; }
        public int Search_Class_In_Study_Timeshift_Select { get; set; }
        public string Search_Class_In_Study_Type { get; set; }
        public int Search_Class_In_Study_Type_Select { get; set; }

        public int Search_Class_In_Skill_Select_State { get; set; }
        public int Search_Class_In_Level_Select_State { get; set; }

        //Prepare Student to class
        public string Total_Count_Students_Class {  get; set; }
        public string Total_Count_Female_Class { get; set; }
        public int Max_Student_InClass { get; set; }
        public int Current_Student_InClass { get; set; }
        public string Current_Class_State { get; set; }

        


    }
}
