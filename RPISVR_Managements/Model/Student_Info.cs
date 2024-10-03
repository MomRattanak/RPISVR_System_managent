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
        public string Stu_FirstName_KH { get; set; }
        public string Stu_LastName_KH { get; set; }
        public string Stu_FirstName_EN { get; set; }
        public string Stu_LastName_EN { get; set; }
        public string Stu_Gender { get; set; }
        public string Stu_StateFamily { get; set; }
        //public DateTime? Stu_BirthdayDateOnly { get; set; }  // Nullable DateTime
        public string Stu_BirthdayDateOnly { get; set; } 
        public string Stu_EducationLevels { get; set; }
        public string Stu_EducationSubjects { get; set; }
        public string Stu_StudyTimeShift { get; set; }
        public string Stu_PhoneNumber { get; set; }
        public string Stu_EducationType { get; set; }
        public string Stu_NationalID { get; set; }
        public string Stu_StudyingTime { get; set; }
        public string Stu_Birth_Province { get; set; }
        public string Stu_Birth_Distric { get; set; }
        public string Stu_Birth_Commune { get; set; }
        public string Stu_Birth_Village { get; set; }
        public string Stu_Live_Pro { get; set; }
        public string Stu_Live_Dis { get; set; }
        public string Stu_Live_Comm { get; set; }
        public string Stu_Live_Vill { get; set; }
        public string Stu_Jobs { get; set; }
        public string Stu_School { get; set; }
        public string Stu_StudyYear { get; set; }
        public string Stu_Semester { get; set; }
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
    
    }
}
