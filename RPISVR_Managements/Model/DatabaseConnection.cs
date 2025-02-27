﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using Windows.Gaming.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using System.Data.Common;
using Windows.ApplicationModel.Email.DataProvider;
using Windows.Graphics.Printing;
using Windows.UI.Core;
using QuestPDF.Helpers;
using System.Reflection.PortableExecutable;
using static Mysqlx.Expect.Open.Types;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using MySqlX.XDevAPI.Common;
using System.Collections;
using DocumentFormat.OpenXml.VariantTypes;
using Org.BouncyCastle.Asn1.Cmp;
using RPISVR_Managements.ViewModel;
using RPISVR_Managements.List_and_Reports.Curriculum;
using RPISVR_Managements.List_and_Reports.Schedule;
using System.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.UI.Xaml.Controls.Primitives;
using QuestPDF.Infrastructure;
using DocumentFormat.OpenXml.Office2013.Excel;
using MySqlX.XDevAPI;
using static QuestPDF.Helpers.Colors;

namespace RPISVR_Managements.Model
{
    public class DatabaseConnection
    {
        //public string ConnectionString { get; set; }
        public string _connectionString { get; set; }
        private MySqlConnection _dbConnection;
        DateTime Create_Datetime = DateTime.Now;
        DateTime Update_Datetime = DateTime.Now;
        string User_Create = "MOM RATTANAK";
        string User_Update = "MOM RATTANAK";
        public DatabaseConnection()
        {
            //Local
            _connectionString = "Server=127.0.0.1;Port=3306;Database=rpisvr_system;User ID=root;Password=;";
            
            //Server
            //_connectionString = "Server=88.222.215.127;Port=3306;Database=rpisvr_system;User ID=admin;Password=admin@123;charset=utf8mb4;";

        }

        // Method to open a connection to the database
        public MySqlConnection OpenConnection()
        {
            _dbConnection = new MySqlConnection(_connectionString);
            MySqlConnection connection = new MySqlConnection(_connectionString);

            try
            {
                connection.Open();
                Debug.WriteLine("Connection Opened and Connected.");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Connection Error: " + ex.Message);
            }
            return connection;
        }
        // Method to close the connection
        public void CloseConnection(MySqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
                Debug.WriteLine("Connection Closed.");
            }
        }

        
        //Method to Load Class to ListView
        //GetClass_Info
        //Method to Insert Class Information
        public bool Insert_Class_Information(Student_Info classes_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO classes(class_name,class_in_skill,class_in_level,class_in_study_year,class_in_student_year,class_in_semester,class_in_generation,class_in_study_timeshift,class_in_study_type) " +
                        "VALUES(@class_name,@class_in_skill,@class_in_level,@class_in_study_year,@class_in_student_year,@class_in_semester,@class_in_generation,@class_in_study_timeshift,@class_in_study_type)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //cmd.Parameters.AddWithValue("@id", "ID");
                    cmd.Parameters.AddWithValue("@class_name", classes_info.Class_Name);
                    cmd.Parameters.AddWithValue("@class_in_skill", classes_info.Class_In_Skill);
                    cmd.Parameters.AddWithValue("@class_in_level", classes_info.Class_In_Level);
                    cmd.Parameters.AddWithValue("@class_in_study_year", classes_info.Class_In_Study_Year);
                    cmd.Parameters.AddWithValue("@class_in_student_year", classes_info.Class_In_Student_Year);
                    cmd.Parameters.AddWithValue("@class_in_semester", classes_info.Class_In_Semester);
                    cmd.Parameters.AddWithValue("@class_in_generation", classes_info.Class_In_Generation);
                    cmd.Parameters.AddWithValue("@class_in_study_timeshift", classes_info.Class_In_Study_Timeshift);
                    cmd.Parameters.AddWithValue("@class_in_study_type", classes_info.Class_In_Study_Type);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }

        }

        //Method to Count Class
        public int GetTotalClassCount()
        {
            int count1 = 0;

            string query = "SELECT COUNT(*) FROM classes";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    count1 = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count1;
        }
        //Method to get Count Student Selected Class
        public (string Total_Count, string Total_Female_Count) Get_Count_Total_and_Female_Students_Classes(string Class_In_Study_Year, string Class_In_Level, string Class_In_Skill, string Class_In_Student_Year, string Class_In_Study_Timeshift)
        {
            string Total_Count = "0";
            string Total_Female_Count = "0";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(ID) AS Total_Count, COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS Total_Female_Count FROM student_infomations WHERE stu_study_year = @Class_In_Study_Year AND stu_education_level = @Class_In_Level AND stu_education_subject = @Class_In_Skill AND stu_studying_time = @Class_In_Student_Year AND stu_study_time_shift = @Class_In_Study_Timeshift";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Class_In_Study_Year", Class_In_Study_Year);
                    cmd.Parameters.AddWithValue("@Class_In_Level", Class_In_Level);
                    cmd.Parameters.AddWithValue("@Class_In_Skill", Class_In_Skill);
                    cmd.Parameters.AddWithValue("@Class_In_Student_Year", Class_In_Student_Year);
                    cmd.Parameters.AddWithValue("@Class_In_Study_Timeshift", Class_In_Study_Timeshift);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Total_Count = reader.IsDBNull(0) ? "0" : reader.GetValue(0).ToString();
                            Total_Female_Count = reader.IsDBNull(1) ? "0" : reader.GetValue(1).ToString();
                        }
                    }
                }
            }
            return (Total_Count, Total_Female_Count);
        }
        
        //Method to Select Last ID and Stu_ID
        public (int ID, string Stu_ID) Get_ID_and_Stu_ID()
        {
            int ID = 0;
            string Last_Stu_ID = "RPI0000000";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(ID) AS ID, MAX(stu_id) AS Last_Stu_ID FROM student_infomations";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //ID = reader.GetInt32("ID");
                            //Last_Stu_ID = reader.GetString("Last_Stu_ID");
                            ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("ID");
                            Last_Stu_ID = reader.IsDBNull(1) ? "RPI0000000" : reader.GetString("Last_Stu_ID");
                        }
                    }
                }
            }
            ID++;
            string Stu_ID = IncrementStuID(Last_Stu_ID);
            return (ID, Stu_ID);

        }

        //Method to Increase Stu_ID
        public string IncrementStuID(String currentStuID)
        {
            // Assuming the format is always "RPI" + 7-digit number
            string prefix = "RPI";
            string numericPart = currentStuID.Substring(3); // Extract the numeric part after "RPI"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 7-digit string with leading zeros
            string newNumericPart = number.ToString("D7");

            // Combine the prefix and the incremented numeric part
            string Stu_ID = prefix + newNumericPart;

            return Stu_ID;
        }

        //Method to Insert Student_Information to Database
        public bool Insert_Student_Information(Student_Info student_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO student_infomations(stu_id, stu_firstname_kh, stu_lastname_kh, stu_firstname_en, stu_lastname_en, stu_birthday_dateonly, stu_gender, stu_state_family, stu_education_level, stu_education_subject, stu_study_time_shift, stu_education_types, stu_study_year, stu_semester, stu_phone_number, stu_nation_id, stu_studying_time, stu_jobs, stu_school, stu_birth_province, stu_birth_distric, stu_birth_commune, stu_birth_village, stu_live_province, stu_live_distric, stu_live_commune, stu_live_village, stu_mother_name, stu_mother_job, stu_mother_phone_number, stu_father_name, stu_father_job, stu_father_phone_number, stu_image_yes_no, stu_image_source, stu_image_total_big, stu_image_total_small, stu_image_degree_yes_no, stu_image_degree_source, stu_image_birth_cert_yes_no, stu_image_birth_cert_source, stu_image_id_nation_yes_no, stu_image_id_nation_source, stu_image_poor_card_yes_no, stu_image_poor_card_source, stu_insert_by_id, stu_insert_datetime, stu_insert_info, stu_update_by_id, stu_update_datetime, stu_update_info, stu_delete_by_id, stu_delete_datetime, stu_delete_info,stu_state_poor,stu_generation) " +
                        "VALUES(@stu_id, @stu_firstname_kh, @stu_lastname_kh, @stu_firstname_en, @stu_lastname_en, @stu_birthday_dateonly, @stu_gender, @stu_state_family, @stu_education_level, @stu_education_subject, " +
                        "@stu_study_time_shift, @stu_education_types, @stu_study_year, @stu_semester, @stu_phone_number, @stu_nation_id, @stu_studying_time, @stu_jobs, @stu_school, @stu_birth_province, @stu_birth_distric, @stu_birth_commune, @stu_birth_village, " +
                        "@stu_live_province, @stu_live_distric, @stu_live_commune, @stu_live_village, @stu_mother_name, @stu_mother_job, @stu_mother_phone_number, @stu_father_name, @stu_father_job, @stu_father_phone_number, @stu_image_yes_no, @stu_image_source, @stu_image_total_big, @stu_image_total_small, " +
                        "@stu_image_degree_yes_no, @stu_image_degree_source, @stu_image_birth_cert_yes_no, @stu_image_birth_cert_source, @stu_image_id_nation_yes_no, @stu_image_id_nation_source, @stu_image_poor_card_yes_no, @stu_image_poor_card_source, @stu_insert_by_id, @stu_insert_datetime, @stu_insert_info, " +
                        "@stu_update_by_id, @stu_update_datetime, @stu_update_info, @stu_delete_by_id, @stu_delete_datetime, @stu_delete_info,@stu_state_poor,@stu_generation)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //cmd.Parameters.AddWithValue("@id", "ID");
                    cmd.Parameters.AddWithValue("@stu_id", student_info.Stu_ID);
                    cmd.Parameters.AddWithValue("@stu_firstname_kh", student_info.Stu_FirstName_KH);
                    cmd.Parameters.AddWithValue("@stu_lastname_kh", student_info.Stu_LastName_KH);
                    cmd.Parameters.AddWithValue("@stu_firstname_en", student_info.Stu_FirstName_EN);
                    cmd.Parameters.AddWithValue("@stu_lastname_en", student_info.Stu_LastName_EN);
                    cmd.Parameters.AddWithValue("@stu_birthday_dateonly", student_info.Stu_BirthdayDateOnly);
                    cmd.Parameters.AddWithValue("@stu_gender", student_info.Stu_Gender);
                    cmd.Parameters.AddWithValue("@stu_state_family", student_info.Stu_StateFamily);
                    cmd.Parameters.AddWithValue("@stu_education_level", student_info.Stu_EducationLevels);
                    cmd.Parameters.AddWithValue("@stu_education_subject", student_info.Stu_EducationSubjects);
                    cmd.Parameters.AddWithValue("@stu_study_time_shift", student_info.Stu_StudyTimeShift);
                    cmd.Parameters.AddWithValue("@stu_education_types", student_info.Stu_EducationType);
                    cmd.Parameters.AddWithValue("@stu_study_year", student_info.Stu_StudyYear);
                    cmd.Parameters.AddWithValue("@stu_semester", student_info.Stu_Semester);
                    cmd.Parameters.AddWithValue("@stu_phone_number", student_info.Stu_PhoneNumber);
                    cmd.Parameters.AddWithValue("@stu_nation_id", student_info.Stu_NationalID);
                    cmd.Parameters.AddWithValue("@stu_studying_time", student_info.Stu_StudyingTime);
                    cmd.Parameters.AddWithValue("@stu_jobs", student_info.Stu_Jobs);
                    cmd.Parameters.AddWithValue("@stu_school", student_info.Stu_School);
                    cmd.Parameters.AddWithValue("@stu_birth_province", student_info.Stu_Birth_Province);
                    cmd.Parameters.AddWithValue("@stu_birth_distric", student_info.Stu_Birth_Distric);
                    cmd.Parameters.AddWithValue("@stu_birth_commune", student_info.Stu_Birth_Commune);
                    cmd.Parameters.AddWithValue("@stu_birth_village", student_info.Stu_Birth_Village);
                    cmd.Parameters.AddWithValue("@stu_live_province", student_info.Stu_Live_Pro);
                    cmd.Parameters.AddWithValue("@stu_live_distric", student_info.Stu_Live_Dis);
                    cmd.Parameters.AddWithValue("@stu_live_commune", student_info.Stu_Live_Comm);
                    cmd.Parameters.AddWithValue("@stu_live_village", student_info.Stu_Live_Vill);
                    cmd.Parameters.AddWithValue("@stu_mother_name", student_info.Stu_Mother_Name);
                    cmd.Parameters.AddWithValue("@stu_mother_job", student_info.Stu_Mother_Job);
                    cmd.Parameters.AddWithValue("@stu_mother_phone_number", student_info.Stu_Mother_Phone);
                    cmd.Parameters.AddWithValue("@stu_father_name", student_info.Stu_Father_Name);
                    cmd.Parameters.AddWithValue("@stu_father_job", student_info.Stu_Father_Job);
                    cmd.Parameters.AddWithValue("@stu_father_phone_number", student_info.Stu_Father_Phone);
                    cmd.Parameters.AddWithValue("@stu_image_yes_no", student_info.Stu_Image_YesNo);
                    cmd.Parameters.AddWithValue("@stu_image_source", student_info.ProfileImageBytes);
                    cmd.Parameters.AddWithValue("@stu_image_total_big", student_info.Stu_Image_Total_Big);
                    cmd.Parameters.AddWithValue("@stu_image_total_small", student_info.Stu_Image_TotalSmall);
                    cmd.Parameters.AddWithValue("@stu_image_degree_yes_no", student_info.Stu_Images_Degree_Yes_No);
                    cmd.Parameters.AddWithValue("@stu_image_degree_source", student_info.Stu_Image_Degree_Bytes);
                    cmd.Parameters.AddWithValue("@stu_image_birth_cert_yes_no", student_info.Stu_ImageBirth_Cert_YesNo);
                    cmd.Parameters.AddWithValue("@stu_image_birth_cert_source", student_info.Stu_ImageBirth_Cert_Bytes);
                    cmd.Parameters.AddWithValue("@stu_image_id_nation_yes_no", student_info.Stu_ImageIDNation_YesNo);
                    cmd.Parameters.AddWithValue("@stu_image_id_nation_source", student_info.Stu_ImageIDNation_Bytes);
                    cmd.Parameters.AddWithValue("@stu_image_poor_card_yes_no", student_info.Stu_ImagePoor_Card_YesNo);
                    cmd.Parameters.AddWithValue("@stu_image_poor_card_source", student_info.Stu_Image_Poor_Card_Bytes);
                    cmd.Parameters.AddWithValue("@stu_insert_by_id", student_info.Stu_Insert_by_ID);
                    cmd.Parameters.AddWithValue("@stu_insert_datetime", student_info.Stu_Insert_DateTime);
                    cmd.Parameters.AddWithValue("@stu_insert_info", student_info.Stu_Insert_Info);
                    cmd.Parameters.AddWithValue("@stu_update_by_id", student_info.Stu_Update_By_ID);
                    cmd.Parameters.AddWithValue("@stu_update_datetime", student_info.Stu_Update_DateTime);
                    cmd.Parameters.AddWithValue("@stu_update_info", student_info.Stu_Update_Info);
                    cmd.Parameters.AddWithValue("@stu_delete_by_id", student_info.Stu_Delete_By_ID);
                    cmd.Parameters.AddWithValue("@stu_delete_datetime", student_info.Stu_Delete_DateTime);
                    cmd.Parameters.AddWithValue("@stu_delete_info", student_info.Stu_Delete_Info);
                    cmd.Parameters.AddWithValue("@stu_state_poor", student_info.Stu_StatePoor);
                    cmd.Parameters.AddWithValue("@stu_generation", student_info.Stu_Generation);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
        

        //Method to fetch Student Information from Database
        public List<Student_Info> GetStudents_Info(int page, int pageSize, string Search_ID_Name_Insert)
        {
            List<Student_Info> students_info = new List<Student_Info>();
            try
            {
                int offset = (page - 1) * pageSize; // Calculate the offset
                string query = string.IsNullOrEmpty(Search_ID_Name_Insert)
        ? "SELECT * FROM student_infomations ORDER BY stu_id DESC LIMIT @Offset, @PageSize"
        : "SELECT * FROM student_infomations WHERE stu_id LIKE @Search_ID_Name_Insert || stu_firstname_kh LIKE @Search_ID_Name_Insert || stu_lastname_kh LIKE @Search_ID_Name_Insert || stu_firstname_en LIKE @Search_ID_Name_Insert || stu_lastname_en LIKE @Search_ID_Name_Insert || stu_phone_number LIKE @Search_ID_Name_Insert ORDER BY stu_id DESC LIMIT @Offset, @PageSize ";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrWhiteSpace(Search_ID_Name_Insert))
                        {
                            cmd.Parameters.AddWithValue("@Search_ID_Name_Insert", $"%{Search_ID_Name_Insert}%");
                        }
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.CommandTimeout = 30;  // Set a timeout of 30 seconds
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Student_Info students = new Student_Info
                                {
                                    ID = reader.GetInt32("ID"),
                                    Stu_ID = reader.GetString("stu_id"),
                                    Stu_FirstName_KH = reader.GetString("stu_firstname_kh"),
                                    Stu_LastName_KH = reader.GetString("stu_lastname_kh"),
                                    Stu_FirstName_EN = reader.GetString("stu_firstname_en"),
                                    Stu_LastName_EN = reader.GetString("stu_lastname_en"),
                                    Stu_BirthdayDateOnly = reader.GetString("stu_birthday_dateonly"),
                                    Stu_Gender = reader.GetString("stu_gender"),
                                    Stu_StateFamily = reader.GetString("stu_state_family"),
                                    Stu_EducationLevels = reader.GetString("stu_education_level"),
                                    Stu_EducationSubjects = reader.GetString("stu_education_subject"),
                                    Stu_StudyTimeShift = reader.GetString("stu_study_time_shift"),
                                    Stu_PhoneNumber = reader.GetString("stu_phone_number"),
                                    Stu_EducationType = reader.GetString("stu_education_types"),
                                    Stu_NationalID = reader.GetString("stu_nation_id"),
                                    Stu_StudyingTime = reader.GetString("stu_studying_time"),
                                    Stu_Birth_Province = reader.IsDBNull(reader.GetOrdinal("stu_birth_province")) ? string.Empty : reader.GetString("stu_birth_province"),
                                    Stu_Birth_Distric = reader.IsDBNull(reader.GetOrdinal("stu_birth_distric")) ? string.Empty : reader.GetString("stu_birth_distric"),
                                    Stu_Birth_Commune = reader.IsDBNull(reader.GetOrdinal("stu_birth_commune")) ? string.Empty : reader.GetString("stu_birth_commune"),
                                    Stu_Birth_Village = reader.IsDBNull(reader.GetOrdinal("stu_birth_village")) ? string.Empty : reader.GetString("stu_birth_village"),
                                    Stu_Live_Pro = reader.IsDBNull(reader.GetOrdinal("stu_live_province")) ? string.Empty : reader.GetString("stu_live_province"),
                                    Stu_Live_Dis = reader.IsDBNull(reader.GetOrdinal("stu_live_distric")) ? string.Empty : reader.GetString("stu_live_distric"),
                                    Stu_Live_Comm = reader.IsDBNull(reader.GetOrdinal("stu_live_commune")) ? string.Empty : reader.GetString("stu_live_commune"),
                                    Stu_Live_Vill = reader.IsDBNull(reader.GetOrdinal("stu_live_village")) ? string.Empty : reader.GetString("stu_live_village"),
                                    Stu_Jobs = reader.IsDBNull(reader.GetOrdinal("stu_jobs")) ? string.Empty : reader.GetString("stu_jobs"),
                                    Stu_School = reader.IsDBNull(reader.GetOrdinal("stu_school")) ? string.Empty : reader.GetString("stu_school"),
                                    Stu_StudyYear = reader.GetString("stu_study_year"),
                                    Stu_Semester = reader.GetString("stu_semester"),
                                    Stu_Mother_Name = reader.IsDBNull(reader.GetOrdinal("stu_mother_name")) ? string.Empty : reader.GetString("stu_mother_name"),
                                    Stu_Mother_Phone = reader.IsDBNull(reader.GetOrdinal("stu_mother_phone_number")) ? string.Empty : reader.GetString("stu_mother_phone_number"),
                                    Stu_Mother_Job = reader.IsDBNull(reader.GetOrdinal("stu_mother_job")) ? string.Empty : reader.GetString("stu_mother_job"),
                                    Stu_Father_Name = reader.IsDBNull(reader.GetOrdinal("stu_father_name")) ? string.Empty : reader.GetString("stu_father_name"),
                                    Stu_Father_Phone = reader.IsDBNull(reader.GetOrdinal("stu_father_phone_number")) ? string.Empty : reader.GetString("stu_father_phone_number"),
                                    Stu_Father_Job = reader.IsDBNull(reader.GetOrdinal("stu_father_job")) ? string.Empty : reader.GetString("stu_father_job"),
                                    Stu_Image_YesNo = reader.GetString("stu_image_yes_no"),
                                    ProfileImageBytes = reader.IsDBNull(reader.GetOrdinal("stu_image_source")) ? null : (byte[])reader["stu_image_source"],
                                    Stu_Image_Total_Big = reader.IsDBNull(reader.GetOrdinal("stu_image_total_big")) ? string.Empty : reader.GetString("stu_image_total_big"),
                                    Stu_Image_TotalSmall = reader.GetString("stu_image_total_small"),
                                    Stu_Images_Degree_Yes_No = reader.GetString("stu_image_degree_yes_no"),
                                    Stu_Image_Degree_Bytes = reader.IsDBNull(reader.GetOrdinal("stu_image_degree_source")) ? null : (byte[])reader["stu_image_degree_source"],
                                    Stu_ImageBirth_Cert_YesNo = reader.GetString("stu_image_birth_cert_yes_no"),
                                    Stu_ImageBirth_Cert_Bytes = reader.IsDBNull(reader.GetOrdinal("stu_image_birth_cert_source")) ? null : (byte[])reader["stu_image_birth_cert_source"],
                                    Stu_ImageIDNation_YesNo = reader.GetString("stu_image_id_nation_yes_no"),
                                    //Stu_ImageIDNation_Bytes = this.Stu_ImageIDNation_Bytes,
                                    Stu_ImagePoor_Card_YesNo = reader.GetString("stu_image_poor_card_yes_no"),
                                    //Stu_Image_Poor_Card_Bytes = this.Stu_Image_Poor_Card_Bytes,
                                    Stu_Insert_by_ID = reader.GetString("stu_insert_by_id"),
                                    Stu_Insert_DateTime = reader.GetDateTime("stu_insert_datetime"),
                                    Stu_Insert_Info = reader.IsDBNull(reader.GetOrdinal("stu_insert_info")) ? string.Empty : reader.GetString("stu_insert_info"),
                                    Stu_Update_By_ID = reader.GetString("stu_update_by_id"),
                                    Stu_Update_DateTime = reader.GetDateTime("stu_update_datetime"),
                                    Stu_Update_Info = reader.IsDBNull(reader.GetOrdinal("stu_update_info")) ? string.Empty : reader.GetString("stu_update_info"),
                                    Stu_Delete_By_ID = reader.GetString("stu_delete_by_id"),
                                    Stu_Delete_DateTime = reader.GetDateTime("stu_delete_datetime"),
                                    Stu_Delete_Info = reader.IsDBNull(reader.GetOrdinal("stu_delete_info")) ? string.Empty : reader.GetString("stu_delete_info"),
                                    Stu_StatePoor = reader.IsDBNull(reader.GetOrdinal("stu_state_poor")) ? string.Empty : reader.GetString("stu_state_poor"),
                                    Stu_Generation = reader.IsDBNull(reader.GetOrdinal("stu_generation")) ? string.Empty : reader.GetString("stu_generation")
                                };



                                // Read the image as byte array from MySQL  
                                // Stu_Image
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_source")))
                                {
                                    // First, get the size of the image byte array from the database
                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, null, 0, 0);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.ProfileImageBytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_Image_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage
                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("stu_image_source is NULL for student " + students.Stu_ID);
                                }



                                //Stu_Images_Degree
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_degree_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_degree_source"), 0, null, 0, 0);
                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_degree_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_Image_Degree_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_Image_Degree_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                //Stu_ImageBirth_Cert
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_birth_cert_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_birth_cert_source"), 0, null, 0, 0);
                                    Debug.WriteLine("Byte size for student " + students.Stu_ID + ": " + byteSize);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_birth_cert_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_ImageBirth_Cert_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_ImageBirth_Cert_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                //Stu_Image_IDNation
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_id_nation_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_id_nation_source"), 0, null, 0, 0);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_id_nation_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_ImageIDNation_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_ImageIDNation_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                //Stu_ImagePoor_Card
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_poor_card_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_poor_card_source"), 0, null, 0, 0);


                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_poor_card_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_Image_Poor_Card_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_ImagePoor_Card_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                students_info.Add(students);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"MySql Get Student to ListView Error:{ex.Message}");
            }
            return students_info;
        }

        public int GetTotalStudentsCount()
        {
            int count = 0;

            string query = "SELECT COUNT(*) FROM student_infomations";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count;
        }
        //Method to Update Class_Information
        public bool Update_Classes_Information(Student_Info classes_info)
        {
            try
            {
                using(MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE classes SET class_name = @class_name,class_in_skill = @class_in_skill,class_in_level = @class_in_level,class_in_study_year = @class_in_study_year, class_in_student_year = @class_in_student_year, class_in_semester = @class_in_semester, class_in_generation = @class_in_generation, class_in_study_timeshift = @class_in_study_timeshift, class_in_study_type = @class_in_study_type WHERE class_id = @class_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@class_id", classes_info.Class_ID);
                    cmd.Parameters.AddWithValue("@class_name", classes_info.Class_Name);
                    cmd.Parameters.AddWithValue("@class_in_skill", classes_info.Class_In_Skill);
                    cmd.Parameters.AddWithValue("@class_in_level", classes_info.Class_In_Level);
                    cmd.Parameters.AddWithValue("@class_in_study_year", classes_info.Class_In_Study_Year);
                    cmd.Parameters.AddWithValue("@class_in_student_year", classes_info.Class_In_Student_Year);
                    cmd.Parameters.AddWithValue("@class_in_semester", classes_info.Class_In_Semester);
                    cmd.Parameters.AddWithValue("@class_in_generation", classes_info.Class_In_Generation);
                    cmd.Parameters.AddWithValue("@class_in_study_timeshift", classes_info.Class_In_Study_Timeshift);
                    cmd.Parameters.AddWithValue("@class_in_study_type", classes_info.Class_In_Study_Type);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }catch (MySqlException ex)
            {
                Debug.WriteLine($"Update class error: {ex.ToString()}");
                return false;
            }
        }

        
        //Method to Update Student_Information to Database
        public bool Update_Student_Information(Student_Info student_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE student_infomations SET stu_firstname_kh = @stu_firstname_kh, stu_lastname_kh = @stu_lastname_kh, stu_firstname_en = @stu_firstname_en, stu_lastname_en = @stu_lastname_en, " +
                        "stu_birthday_dateonly = @stu_birthday_dateonly, stu_gender = @stu_gender, stu_state_family = @stu_state_family, stu_education_level = @stu_education_level, stu_education_subject = @stu_education_subject, " +
                        "stu_study_time_shift = @stu_study_time_shift, stu_education_types = @stu_education_types, stu_study_year = @stu_study_year, stu_semester = @stu_semester, stu_phone_number = @stu_phone_number, stu_nation_id = @stu_nation_id, " +
                        "stu_studying_time = @stu_studying_time, stu_jobs = @stu_jobs, stu_school = @stu_school, stu_birth_province = @stu_birth_province, stu_birth_distric = @stu_birth_distric, stu_birth_commune = @stu_birth_commune, stu_birth_village = @stu_birth_village, " +
                        "stu_live_province = @stu_live_province, stu_live_distric =  @stu_live_distric, stu_live_commune = @stu_live_commune, stu_live_village = @stu_live_village, stu_mother_name = @stu_mother_name, stu_mother_job = @stu_mother_job, stu_mother_phone_number =@stu_mother_phone_number, " +
                        "stu_father_name = @stu_father_name, stu_father_job = @stu_father_job, stu_father_phone_number = @stu_father_phone_number, stu_image_yes_no = @stu_image_yes_no, stu_image_source = @stu_image_source, stu_image_total_big = @stu_image_total_big, " +
                        "stu_image_total_small = @stu_image_total_small, stu_image_degree_yes_no = @stu_image_degree_yes_no, stu_image_degree_source = @stu_image_degree_source, stu_image_birth_cert_yes_no = @stu_image_birth_cert_yes_no, stu_image_birth_cert_source = @stu_image_birth_cert_source, " +
                        "stu_image_id_nation_yes_no = @stu_image_id_nation_yes_no, stu_image_id_nation_source = @stu_image_id_nation_source, stu_image_poor_card_yes_no = @stu_image_poor_card_yes_no, stu_image_poor_card_source = @stu_image_poor_card_source, stu_update_by_id = @stu_update_by_id, stu_update_datetime = @stu_update_datetime, stu_update_info = @stu_update_info, stu_state_poor = @stu_state_poor, stu_generation = @stu_generation WHERE stu_id = @stu_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@stu_id", student_info.Stu_ID);
                    cmd.Parameters.AddWithValue("@stu_firstname_kh", student_info.Stu_FirstName_KH);
                    cmd.Parameters.AddWithValue("@stu_lastname_kh", student_info.Stu_LastName_KH);
                    cmd.Parameters.AddWithValue("@stu_firstname_en", student_info.Stu_FirstName_EN);
                    cmd.Parameters.AddWithValue("@stu_lastname_en", student_info.Stu_LastName_EN);
                    cmd.Parameters.AddWithValue("@stu_birthday_dateonly", student_info.Stu_BirthdayDateOnly);
                    cmd.Parameters.AddWithValue("@stu_gender", student_info.Stu_Gender);
                    cmd.Parameters.AddWithValue("@stu_state_family", student_info.Stu_StateFamily);
                    cmd.Parameters.AddWithValue("@stu_education_level", student_info.Stu_EducationLevels);
                    cmd.Parameters.AddWithValue("@stu_education_subject", student_info.Stu_EducationSubjects);
                    cmd.Parameters.AddWithValue("@stu_study_time_shift", student_info.Stu_StudyTimeShift);
                    cmd.Parameters.AddWithValue("@stu_education_types", student_info.Stu_EducationType);
                    cmd.Parameters.AddWithValue("@stu_study_year", student_info.Stu_StudyYear);
                    cmd.Parameters.AddWithValue("@stu_semester", student_info.Stu_Semester);
                    cmd.Parameters.AddWithValue("@stu_phone_number", student_info.Stu_PhoneNumber);
                    cmd.Parameters.AddWithValue("@stu_nation_id", student_info.Stu_NationalID);
                    cmd.Parameters.AddWithValue("@stu_studying_time", student_info.Stu_StudyingTime);
                    cmd.Parameters.AddWithValue("@stu_jobs", student_info.Stu_Jobs);
                    cmd.Parameters.AddWithValue("@stu_school", student_info.Stu_School);
                    cmd.Parameters.AddWithValue("@stu_birth_province", student_info.Stu_Birth_Province);
                    cmd.Parameters.AddWithValue("@stu_birth_distric", student_info.Stu_Birth_Distric);
                    cmd.Parameters.AddWithValue("@stu_birth_commune", student_info.Stu_Birth_Commune);
                    cmd.Parameters.AddWithValue("@stu_birth_village", student_info.Stu_Birth_Village);
                    cmd.Parameters.AddWithValue("@stu_live_province", student_info.Stu_Live_Pro);
                    cmd.Parameters.AddWithValue("@stu_live_distric", student_info.Stu_Live_Dis);
                    cmd.Parameters.AddWithValue("@stu_live_commune", student_info.Stu_Live_Comm);
                    cmd.Parameters.AddWithValue("@stu_live_village", student_info.Stu_Live_Vill);
                    cmd.Parameters.AddWithValue("@stu_mother_name", student_info.Stu_Mother_Name);
                    cmd.Parameters.AddWithValue("@stu_mother_job", student_info.Stu_Mother_Job);
                    cmd.Parameters.AddWithValue("@stu_mother_phone_number", student_info.Stu_Mother_Phone);
                    cmd.Parameters.AddWithValue("@stu_father_name", student_info.Stu_Father_Name);
                    cmd.Parameters.AddWithValue("@stu_father_job", student_info.Stu_Father_Job);
                    cmd.Parameters.AddWithValue("@stu_father_phone_number", student_info.Stu_Father_Phone);
                    cmd.Parameters.AddWithValue("@stu_image_yes_no", student_info.Stu_Image_YesNo);
                    cmd.Parameters.AddWithValue("@stu_image_source", student_info.ProfileImageBytes);
                    cmd.Parameters.AddWithValue("@stu_image_total_big", student_info.Stu_Image_Total_Big);
                    cmd.Parameters.AddWithValue("@stu_image_total_small", student_info.Stu_Image_TotalSmall);
                    cmd.Parameters.AddWithValue("@stu_image_degree_yes_no", student_info.Stu_Images_Degree_Yes_No);
                    cmd.Parameters.AddWithValue("@stu_image_degree_source", student_info.Stu_Image_Degree_Bytes);
                    cmd.Parameters.AddWithValue("@stu_image_birth_cert_yes_no", student_info.Stu_ImageBirth_Cert_YesNo);
                    cmd.Parameters.AddWithValue("@stu_image_birth_cert_source", student_info.Stu_ImageBirth_Cert_Bytes);
                    cmd.Parameters.AddWithValue("@stu_image_id_nation_yes_no", student_info.Stu_ImageIDNation_YesNo);
                    cmd.Parameters.AddWithValue("@stu_image_id_nation_source", student_info.Stu_ImageIDNation_Bytes);
                    cmd.Parameters.AddWithValue("@stu_image_poor_card_yes_no", student_info.Stu_ImagePoor_Card_YesNo);
                    cmd.Parameters.AddWithValue("@stu_image_poor_card_source", student_info.Stu_Image_Poor_Card_Bytes);
                    cmd.Parameters.AddWithValue("@stu_update_by_id", student_info.Stu_Update_By_ID);
                    cmd.Parameters.AddWithValue("@stu_update_datetime", student_info.Stu_Update_DateTime);
                    cmd.Parameters.AddWithValue("@stu_update_info", student_info.Stu_Update_Info);
                    cmd.Parameters.AddWithValue("@stu_state_poor", student_info.Stu_StatePoor);
                    cmd.Parameters.AddWithValue("@stu_generation", student_info.Stu_Generation);


                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Update Error: " + ex.ToString());
                return false;
            }
        }

        // Helper method to convert byte array to BitmapImage
        private BitmapImage ConvertBytesToImage(byte[] imageBytes)
        {
            using (var stream = new MemoryStream(imageBytes))
            {
                BitmapImage bitmapImage = new BitmapImage();
                stream.Position = 0;
                bitmapImage.SetSource(stream.AsRandomAccessStream());
                return bitmapImage;
            }
        }


        //Method for Addition_Information

        //Start Education Level
        //Get Edu_Level_ID
        //Method to Select Last Edu_Level_ID
        public (int Edu_ID, string Edu_Level_ID) Get_Edu_ID_and_Edu_Level_ID()
        {
            int Edu_ID = 0;
            string Last_Edu_Level_ID = "EDU_L000";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(ID) AS ID, MAX(edu_level_id) AS Last_Edu_Level_ID FROM education_level_info";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //ID = reader.GetInt32("ID");
                            //Last_Stu_ID = reader.GetString("Last_Stu_ID");
                            Edu_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("ID");
                            Last_Edu_Level_ID = reader.IsDBNull(1) ? "EDU_L000" : reader.GetString("Last_Edu_Level_ID");
                        }

                    }
                }
            }
            Edu_ID++;
            string Edu_Level_ID = IncrementEdu_Level_ID(Last_Edu_Level_ID);

            return (Edu_ID, Edu_Level_ID);

        }
        //Method to Increase Edu_Level_ID
        public string IncrementEdu_Level_ID(String currentEdu_Level_ID)
        {
            // Assuming the format is always "RPI" + 3-digit number
            string prefix = "EDU_L";
            string numericPart = currentEdu_Level_ID.Substring(5); // Extract the numeric part after "EDU_L"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 3-digit string with leading zeros
            string newNumericPart = number.ToString("D3");

            // Combine the prefix and the incremented numeric part
            string Edu_Level_ID = prefix + newNumericPart;

            return Edu_Level_ID;
        }

        //Method to Insert_Education_Levels to Database 
        public bool Insert_Education_Levels(Education_Levels education_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO education_level_info(edu_level_id, edu_level_name_kh, edu_level_name_en, edu_level_name_short)" +
                        "VALUES (@edu_level_id,@edu_level_name_kh,@edu_level_name_en,@edu_level_name_short)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //cmd.Parameters.AddWithValue("@id", "ID");
                    cmd.Parameters.AddWithValue("@edu_level_id", education_info.Edu_Level_ID);
                    cmd.Parameters.AddWithValue("@edu_level_name_kh", education_info.Edu_Level_Name_KH);
                    cmd.Parameters.AddWithValue("@edu_level_name_en", education_info.Edu_Level_Name_EN);
                    cmd.Parameters.AddWithValue("@edu_level_name_short", education_info.Edu_Level_Name_Short);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql Insert Education_Level Error: " + ex.ToString());
                return false;
            }
        }

        //Method to fetch educatinon_level Information from Database
        public List<Education_Levels> LoadEducation_Level()
        {
            Debug.WriteLine("Starting LoadEducation_Level method...");

            List<Education_Levels> education_level_info = new List<Education_Levels>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Debug.WriteLine("Database connection established.");

                    string query = "SELECT edu_level_id, edu_level_name_kh, edu_level_name_en, edu_level_name_short FROM education_level_info ORDER BY edu_level_id DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Debug.WriteLine("Query executed, reading data...");
                            while (reader.Read())
                            {
                                Education_Levels education_levels = new Education_Levels()
                                {
                                    Edu_Level_ID = reader.GetString("edu_level_id"),
                                    Edu_Level_Name_KH = reader.IsDBNull(reader.GetOrdinal("edu_level_name_kh")) ? string.Empty : reader.GetString("edu_level_name_kh"),
                                    Edu_Level_Name_EN = reader.IsDBNull(reader.GetOrdinal("edu_level_name_en")) ? string.Empty : reader.GetString("edu_level_name_en"),
                                    Edu_Level_Name_Short = reader.IsDBNull(reader.GetOrdinal("edu_level_name_short")) ? string.Empty : reader.GetString("edu_level_name_short"),
                                };
                                education_level_info.Add(education_levels);
                            }
                        }
                    }
                }
                Debug.WriteLine("Data Education_Level loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql LoadEducation_Level Error: " + ex.ToString());
            }

            Debug.WriteLine("Exiting LoadEducation_Level method...");
            return education_level_info;
        }

        //Delete Education_Level
        public bool Delete_Education_Level_Information(Education_Levels education_level_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM education_level_info WHERE edu_level_id = @edu_level_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_level_id", education_level_info.Edu_Level_ID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Delete Education Level Error: " + ex.ToString());
                return false;
            }
        }

        //Update Education_Level
        public bool Update_Education_Level_Information(Education_Levels education_level_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE education_level_info SET edu_level_name_kh = @edu_level_name_kh,edu_level_name_en = @edu_level_name_en,edu_level_name_short = @edu_level_name_short WHERE edu_level_id = @edu_level_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_level_id", education_level_info.Edu_Level_ID);
                    cmd.Parameters.AddWithValue("@edu_level_name_kh", education_level_info.Edu_Level_Name_KH);
                    cmd.Parameters.AddWithValue("@edu_level_name_en", education_level_info.Edu_Level_Name_EN);
                    cmd.Parameters.AddWithValue("@edu_level_name_short", education_level_info.Edu_Level_Name_Short);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Update Education Level Error: " + ex.ToString());
                return false;
            }
        }
        //End Education Level

        //Start Education Skill
        //Method to Insert_Education_Skills to Database 
        public bool Insert_Education_Skills(Education_Skills education_skill_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO education_skill_info(edu_skill_id, edu_skill_name_kh, edu_skill_name_en, edu_skill_name_short)" +
                        "VALUES (@edu_skill_id,@edu_skill_name_kh,@edu_skill_name_en,@edu_skill_name_short)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //cmd.Parameters.AddWithValue("@id", "ID");
                    cmd.Parameters.AddWithValue("@edu_skill_id", education_skill_info.Skill_ID);
                    cmd.Parameters.AddWithValue("@edu_skill_name_kh", education_skill_info.Skill_Name_KH);
                    cmd.Parameters.AddWithValue("@edu_skill_name_en", education_skill_info.Skill_Name_EN);
                    cmd.Parameters.AddWithValue("@edu_skill_name_short", education_skill_info.Skill_Name_Short);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql Insert Education_Skill Error: " + ex.ToString());
                return false;
            }
        }

        //Method to fetch education_skill Information
        public List<Education_Skills> LoadEducation_Skill()
        {
            Debug.WriteLine("Starting LoadEducation_Skill method...");
            List<Education_Skills> education_skill_info = new List<Education_Skills>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Debug.WriteLine("Database connection established.");

                    string query = "SELECT edu_skill_id, edu_skill_name_kh, edu_skill_name_en, edu_skill_name_short FROM education_skill_info ORDER BY edu_skill_id DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Debug.WriteLine("Query executed, reading data...");
                            while (reader.Read())
                            {
                                Education_Skills education_skills = new Education_Skills()
                                {
                                    Skill_ID = reader.GetString("edu_skill_id"),
                                    Skill_Name_KH = reader.IsDBNull(reader.GetOrdinal("edu_skill_name_kh")) ? string.Empty : reader.GetString("edu_skill_name_kh"),
                                    Skill_Name_EN = reader.IsDBNull(reader.GetOrdinal("edu_skill_name_en")) ? string.Empty : reader.GetString("edu_skill_name_en"),
                                    Skill_Name_Short = reader.IsDBNull(reader.GetOrdinal("edu_skill_name_short")) ? string.Empty : reader.GetString("edu_skill_name_short"),
                                };
                                education_skill_info.Add(education_skills);
                            }
                        }
                    }
                }
                Debug.WriteLine("Data Education_Skills loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql LoadEducation_Skills Error: " + ex.ToString());
            }

            Debug.WriteLine("Exiting LoadEducation_Skill method...");
            return education_skill_info;
        }
        //Get Skill_ID
        public (int Sk_ID, string Skill_ID) Get_Sk_ID_and_Skill_ID()
        {
            int Sk_ID = 0;
            string Last_Skill_ID = "EDU_S000";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(ID) AS ID, MAX(edu_skill_id) AS Last_Skill_ID FROM education_skill_info";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Sk_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("ID");
                            Last_Skill_ID = reader.IsDBNull(1) ? "EDU_S000" : reader.GetString("Last_Skill_ID");
                        }
                    }
                }
            }
            Sk_ID++;
            string Skill_ID = IncrementSkill_ID(Last_Skill_ID);

            return (Sk_ID, Skill_ID);
        }
        //Method to Increase Skill_ID
        public string IncrementSkill_ID(String currentSkill_ID)
        {
            // Assuming the format is always "RPI" + 3-digit number
            string prefix = "EDU_S";
            string numericPart = currentSkill_ID.Substring(5); // Extract the numeric part after "EDU_L"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 3-digit string with leading zeros
            string newNumericPart = number.ToString("D3");

            // Combine the prefix and the incremented numeric part
            string Skill_ID = prefix + newNumericPart;

            return Skill_ID;
        }
        //Update Education_Skill
        public bool Update_Education_Skill_Information(Education_Skills education_skill_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE education_skill_info SET edu_skill_name_kh = @edu_skill_name_kh,edu_skill_name_en = @edu_skill_name_en,edu_skill_name_short = @edu_skill_name_short WHERE edu_skill_id = @edu_skill_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_skill_id", education_skill_info.Skill_ID);
                    cmd.Parameters.AddWithValue("@edu_skill_name_kh", education_skill_info.Skill_Name_KH);
                    cmd.Parameters.AddWithValue("@edu_skill_name_en", education_skill_info.Skill_Name_EN);
                    cmd.Parameters.AddWithValue("@edu_skill_name_short", education_skill_info.Skill_Name_Short);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Update Education_Skill Error: " + ex.ToString());
                return false;
            }
        }
        // Delete Education_Skill
        public bool Delete_Education_Skill_Information(Education_Skills education_skill_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM education_skill_info WHERE edu_skill_id = @edu_skill_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_skill_id", education_skill_info.Skill_ID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Delete Education_Skill Error: " + ex.ToString());
                return false;
            }
        }
        //End Education Skill

        //Start Education StudyTimeShift
        //Method to Insert_Education_StudyTimeShifts to Database 
        public bool Insert_Education_StudyTimeShifts(Education_StudyTimeShift education_studytimeshift_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO education_studytimeshift_info(edu_studytimeshift_id, edu_studytimeshift_name_kh, edu_studytimeshift_name_en, edu_studytimeshift_name_short)" +
                        "VALUES (@edu_studytimeshift_id,@edu_studytimeshift_name_kh,@edu_studytimeshift_name_en,@edu_studytimeshift_name_short)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //cmd.Parameters.AddWithValue("@id", "ID");
                    cmd.Parameters.AddWithValue("@edu_studytimeshift_id", education_studytimeshift_info.StudyTimeShift_ID);
                    cmd.Parameters.AddWithValue("@edu_studytimeshift_name_kh", education_studytimeshift_info.StudyTimeShift_Name_KH);
                    cmd.Parameters.AddWithValue("@edu_studytimeshift_name_en", education_studytimeshift_info.StudyTimeShift_Name_EN);
                    cmd.Parameters.AddWithValue("@edu_studytimeshift_name_short", education_studytimeshift_info.StudyTimeShift_Name_Short);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql Insert Education_StudyTimeShift Error: " + ex.ToString());
                return false;
            }
        }
        //Get STS_ID
        public (int STS_ID, string StudyTimeShift_ID) Get_STS_ID_and_StudyTimeShift_ID()
        {
            int STS_ID = 0;
            string Last_StudyTimeShift_ID = "EDU_ST000";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(ID) AS ID, MAX(edu_studytimeshift_id) AS Last_StudyTimeShift_ID FROM education_studytimeshift_info";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            STS_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("ID");
                            Last_StudyTimeShift_ID = reader.IsDBNull(1) ? "EDU_ST000" : reader.GetString("Last_StudyTimeShift_ID");
                        }
                    }
                }
            }
            STS_ID++;
            string StudyTimeShift_ID = IncrementStudyTimeShift_ID(Last_StudyTimeShift_ID);

            return (STS_ID, StudyTimeShift_ID);
        }
        //Method to Increase Skill_ID
        public string IncrementStudyTimeShift_ID(String currentStudyTimeShift_ID)
        {
            // Assuming the format is always "EDU_ST" + 3-digit number
            string prefix = "EDU_ST";
            string numericPart = currentStudyTimeShift_ID.Substring(6); // Extract the numeric part after "EDU_ST"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 3-digit string with leading zeros
            string newNumericPart = number.ToString("D3");

            // Combine the prefix and the incremented numeric part
            string StudyTimeShift_ID = prefix + newNumericPart;
            return StudyTimeShift_ID;
        }
        //Method to fetch education_studytimeshift Information
        public List<Education_StudyTimeShift> LoadEducation_StudyTimeShift()
        {
            Debug.WriteLine("Starting LoadEducation_StudyTimeShift method...");
            List<Education_StudyTimeShift> education_studytimeshift_info = new List<Education_StudyTimeShift>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Debug.WriteLine("Database connection established.");

                    string query = "SELECT edu_studytimeshift_id, edu_studytimeshift_name_kh, edu_studytimeshift_name_en, edu_studytimeshift_name_short FROM education_studytimeshift_info ORDER BY edu_studytimeshift_id DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Debug.WriteLine("Query executed, reading data...");
                            while (reader.Read())
                            {
                                Education_StudyTimeShift education_studytimeshifts = new Education_StudyTimeShift()
                                {
                                    StudyTimeShift_ID = reader.GetString("edu_studytimeshift_id"),
                                    StudyTimeShift_Name_KH = reader.IsDBNull(reader.GetOrdinal("edu_studytimeshift_name_kh")) ? string.Empty : reader.GetString("edu_studytimeshift_name_kh"),
                                    StudyTimeShift_Name_EN = reader.IsDBNull(reader.GetOrdinal("edu_studytimeshift_name_en")) ? string.Empty : reader.GetString("edu_studytimeshift_name_en"),
                                    StudyTimeShift_Name_Short = reader.IsDBNull(reader.GetOrdinal("edu_studytimeshift_name_short")) ? string.Empty : reader.GetString("edu_studytimeshift_name_short"),
                                };
                                education_studytimeshift_info.Add(education_studytimeshifts);
                            }
                        }
                    }
                }
                Debug.WriteLine("Data Education_StudyTimeShifts loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql LoadEducation_Skills Error: " + ex.ToString());
            }

            Debug.WriteLine("Exiting LoadEducation_Skill method...");
            return education_studytimeshift_info;
        }
        //Update Education_StudyTimeShift
        public bool Update_Education_StudyTimeShift_Information(Education_StudyTimeShift education_studytimeshift_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE education_studytimeshift_info SET edu_studytimeshift_name_kh = @edu_studytimeshift_name_kh,edu_studytimeshift_name_en = @edu_studytimeshift_name_en,edu_studytimeshift_name_short = @edu_studytimeshift_name_short WHERE edu_studytimeshift_id = @edu_studytimeshift_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_studytimeshift_id", education_studytimeshift_info.StudyTimeShift_ID);
                    cmd.Parameters.AddWithValue("@edu_studytimeshift_name_kh", education_studytimeshift_info.StudyTimeShift_Name_KH);
                    cmd.Parameters.AddWithValue("@edu_studytimeshift_name_en", education_studytimeshift_info.StudyTimeShift_Name_EN);
                    cmd.Parameters.AddWithValue("@edu_studytimeshift_name_short", education_studytimeshift_info.StudyTimeShift_Name_Short);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Update Education_StudyTimeShift Error: " + ex.ToString());
                return false;
            }
        }
        // Delete Education_StudyTimeShift
        public bool Delete_Education_StudyTimeShift_Information(Education_StudyTimeShift education_studytimeshift_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM education_studytimeshift_info WHERE edu_studytimeshift_id = @edu_studytimeshift_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_studytimeshift_id", education_studytimeshift_info.StudyTimeShift_ID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Delete Education_Skill Error: " + ex.ToString());
                return false;
            }
        }
        //End Education_StudyTimeShift

        //Start Education_TypeStudy
        //Method to Insert_Education_TypeStudys to Database 
        public bool Insert_Education_TypeStudys(Education_TypeStudy education_typestudy_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO education_typestudy_info(edu_typestudy_id, edu_typestudy_name_kh, edu_typestudy_name_en, edu_typestudy_name_short)" +
                        "VALUES (@edu_typestudy_id,@edu_typestudy_name_kh,@edu_typestudy_name_en,@edu_typestudy_name_short)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //cmd.Parameters.AddWithValue("@id", "ID");
                    cmd.Parameters.AddWithValue("@edu_typestudy_id", education_typestudy_info.TypeStudy_ID);
                    cmd.Parameters.AddWithValue("@edu_typestudy_name_kh", education_typestudy_info.TypeStudy_Name_KH);
                    cmd.Parameters.AddWithValue("@edu_typestudy_name_en", education_typestudy_info.TypeStudy_Name_EN);
                    cmd.Parameters.AddWithValue("@edu_typestudy_name_short", education_typestudy_info.TypeStudy_Name_Short);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql Insert Education_TypeStudy Error: " + ex.ToString());
                return false;
            }
        }
        //Method to fetch educatinon_typestudy Information from Database
        public List<Education_TypeStudy> LoadEducation_TypeStudy()
        {
            Debug.WriteLine("Starting LoadEducation_TypeStudy method...");

            List<Education_TypeStudy> education_typestudy_info = new List<Education_TypeStudy>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Debug.WriteLine("Database connection established.");

                    string query = "SELECT edu_typestudy_id, edu_typestudy_name_kh, edu_typestudy_name_en, edu_typestudy_name_short FROM education_typestudy_info ORDER BY edu_typestudy_id DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Debug.WriteLine("Query executed, reading data...");
                            while (reader.Read())
                            {
                                Education_TypeStudy education_typestudys = new Education_TypeStudy()
                                {
                                    TypeStudy_ID = reader.GetString("edu_typestudy_id"),
                                    TypeStudy_Name_KH = reader.IsDBNull(reader.GetOrdinal("edu_typestudy_name_kh")) ? string.Empty : reader.GetString("edu_typestudy_name_kh"),
                                    TypeStudy_Name_EN = reader.IsDBNull(reader.GetOrdinal("edu_typestudy_name_en")) ? string.Empty : reader.GetString("edu_typestudy_name_en"),
                                    TypeStudy_Name_Short = reader.IsDBNull(reader.GetOrdinal("edu_typestudy_name_short")) ? string.Empty : reader.GetString("edu_typestudy_name_short"),
                                };
                                education_typestudy_info.Add(education_typestudys);
                            }
                        }
                    }
                }
                Debug.WriteLine("Data Education_Level loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql LoadEducation_Level Error: " + ex.ToString());
            }
            Debug.WriteLine("Exiting LoadEducation_Level method...");
            return education_typestudy_info;
        }
        //Update Education_TypeStudy
        public bool Update_Education_TypeStudy_Information(Education_TypeStudy education_typestudy_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE education_typestudy_info SET edu_typestudy_name_kh = @edu_typestudy_name_kh,edu_typestudy_name_en = @edu_typestudy_name_en,edu_typestudy_name_short = @edu_typestudy_name_short WHERE edu_typestudy_id = @edu_typestudy_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_typestudy_id", education_typestudy_info.TypeStudy_ID);
                    cmd.Parameters.AddWithValue("@edu_typestudy_name_kh", education_typestudy_info.TypeStudy_Name_KH);
                    cmd.Parameters.AddWithValue("@edu_typestudy_name_en", education_typestudy_info.TypeStudy_Name_EN);
                    cmd.Parameters.AddWithValue("@edu_typestudy_name_short", education_typestudy_info.TypeStudy_Name_Short);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Update Education_TypeStudy Error: " + ex.ToString());
                return false;
            }
        }
        //Method to Select Last TS_ID and TypeStudy_ID
        public (int TS_ID, string TypeStudy_ID) Get_TS_ID_and_TypeStudy_ID()
        {
            int TS_ID = 0;
            string Last_TypeStudy_ID = "EDU_TS000";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(ID) AS ID, MAX(edu_typestudy_id) AS Last_TypeStudy_ID FROM education_typestudy_info";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //ID = reader.GetInt32("ID");
                            //Last_Stu_ID = reader.GetString("Last_Stu_ID");
                            TS_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("ID");
                            Last_TypeStudy_ID = reader.IsDBNull(1) ? "EDU_TS000" : reader.GetString("Last_TypeStudy_ID");
                        }

                    }
                }
            }
            TS_ID++;
            string TypeStudy_ID = IncrementTypeStudy_ID(Last_TypeStudy_ID);

            return (TS_ID, TypeStudy_ID);

        }

        //Method to Increase TS_ID
        public string IncrementTypeStudy_ID(String currentTypeStudy_ID)
        {
            // Assuming the format is always "RPI" + 3-digit number
            string prefix = "EDU_TS";
            string numericPart = currentTypeStudy_ID.Substring(6); // Extract the numeric part after "EDU_TS"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 7-digit string with leading zeros
            string newNumericPart = number.ToString("D3");

            // Combine the prefix and the incremented numeric part
            string TypeStudy_ID = prefix + newNumericPart;

            return TypeStudy_ID;
        }
        // Delete Education_TypeStudy
        public bool Delete_Education_TypeStudy_Information(Education_TypeStudy education_typestudy_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM education_typestudy_info WHERE edu_typestudy_id = @edu_typestudy_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_typestudy_id", education_typestudy_info.TypeStudy_ID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Delete Education_TypeStudy Error: " + ex.ToString());
                return false;
            }
        }
        //End Education_TypeStudy

        //Start Education_StudyYear
        //Method to Insert_Education_StudyYear to Database 
        public bool Insert_Education_StudyYears(Education_StudyYear education_studyyear_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO education_studyyear_info (edu_studyyear_id, edu_studyyear_name) VALUES(@edu_studyyear_id, @edu_studyyear_name)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //cmd.Parameters.AddWithValue("@id", "ID");
                    cmd.Parameters.AddWithValue("@edu_studyyear_id", education_studyyear_info.Edu_StudyYear_ID);
                    cmd.Parameters.AddWithValue("@edu_studyyear_name", education_studyyear_info.Edu_StudyYear_Name);
                    
                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Insert Education_StudyYear Error: {ex.ToString()}");
                return false;
            }
        }
        //Method to fetch educatinon_studyyear Information from Database
        public List<Education_StudyYear> LoadEducation_StudyYear()
        {
            Debug.WriteLine("Starting LoadEducation_StudyYear method...");

            List<Education_StudyYear> education_studyyear_info = new List<Education_StudyYear>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Debug.WriteLine("Database connection established.");

                    string query = "SELECT edu_studyyear_id, edu_studyyear_name FROM education_studyyear_info ORDER BY edu_studyyear_id DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Debug.WriteLine("Query executed, reading data...");
                            while (reader.Read())
                            {
                                Education_StudyYear education_studyyears = new Education_StudyYear()
                                {
                                    Edu_StudyYear_ID = reader.GetString("edu_studyyear_id"),
                                    Edu_StudyYear_Name = reader.IsDBNull(reader.GetOrdinal("edu_studyyear_name")) ? string.Empty : reader.GetString("edu_studyyear_name"),
                                };
                                education_studyyear_info.Add(education_studyyears);
                            }
                        }
                    }
                }
                Debug.WriteLine("Data Education_StudyYear loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql LoadEducation_StudyYear Error: " + ex.ToString());
            }
            Debug.WriteLine("Exiting LoadEducation_Level method...");
            return education_studyyear_info;
        }
        //Update Education_StudyYear
        public bool Update_Education_StudyYear_Information(Education_StudyYear education_studyyear_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE education_studyyear_info SET edu_studyyear_name = @edu_studyyear_name WHERE edu_studyyear_id = @edu_studyyear_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_studyyear_id", education_studyyear_info.Edu_StudyYear_ID);
                    cmd.Parameters.AddWithValue("@edu_studyyear_name", education_studyyear_info.Edu_StudyYear_Name);
                   
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Update Education_StudyYear Error: " + ex.ToString());
                return false;
            }
        }
        //Method to Select Last SY_ID and Edu_StudyYear_ID
        public (int SY_ID, string Edu_StudyYear_ID) Get_SY_ID_and_Edu_StudyYear_ID()
        {
            int SY_ID = 0;
            string Last_StudyYear_ID = "EDU_SY000";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(ID) AS ID, MAX(edu_studyyear_id) AS Last_StudyYear_ID FROM education_studyyear_info";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //ID = reader.GetInt32("ID");
                            //Last_Stu_ID = reader.GetString("Last_Stu_ID");
                            SY_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("ID");
                            Last_StudyYear_ID = reader.IsDBNull(1) ? "EDU_SY000" : reader.GetString("Last_StudyYear_ID");
                        }

                    }
                }
            }
            SY_ID++;
            string Edu_StudyYear_ID = IncrementStudyYear_ID(Last_StudyYear_ID);

            return (SY_ID, Edu_StudyYear_ID);

        }
        //Method to Increase SY_ID
        public string IncrementStudyYear_ID(String currentStudyYear_ID)
        {
            // Assuming the format is always "EDU_SY" + 3-digit number
            string prefix = "EDU_SY";
            string numericPart = currentStudyYear_ID.Substring(6); // Extract the numeric part after "EDU_SY"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 3-digit string with leading zeros
            string newNumericPart = number.ToString("D3");

            // Combine the prefix and the incremented numeric part
            string Edu_StudyYear_ID = prefix + newNumericPart;

            return Edu_StudyYear_ID;
        }
        //Delete Education_StudyYear
        public bool Delete_Education_StudyYear_Information(Education_StudyYear education_studyyear_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM education_studyyear_info WHERE edu_studyyear_id = @edu_studyyear_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@edu_studyyear_id", education_studyyear_info.Edu_StudyYear_ID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Delete Education_TypeStudy Error: " + ex.ToString());
                return false;
            }
        }
        //End Education_StudyYear

        //Start Add_Province
        public bool Insert_Provinces(Provinces_Info provinces_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Province_Info (PV_ID, province_name_kh, province_name_en) VALUES(@PV_ID, @province_name_kh, @province_name_en)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@PV_ID", provinces_info.PV_ID);
                    cmd.Parameters.AddWithValue("@province_name_kh", provinces_info.Province_Name_KH);
                    cmd.Parameters.AddWithValue("@province_name_en", provinces_info.Province_Name_EN);
                    

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Insert Provinces Info Error: {ex.ToString()}");
                return false;
            }
        }
        //Data Province to ListView
        public List<Provinces_Info> LoadProvinces_Info()
        {
            Debug.WriteLine("Starting LoadProvince Info method...");

            List<Provinces_Info> province_info = new List<Provinces_Info>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    string query = "SELECT PV_ID, province_name_kh, province_name_en FROM Province_Info ORDER BY PV_ID DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Debug.WriteLine("Query executed, reading data...");
                            while (reader.Read())
                            {
                                Provinces_Info provinces = new Provinces_Info()
                                {
                                    PV_ID = reader.GetString("PV_ID"),
                                    Province_Name_KH = reader.IsDBNull(reader.GetOrdinal("province_name_kh")) ? string.Empty : reader.GetString("province_name_kh"),
                                    Province_Name_EN = reader.IsDBNull(reader.GetOrdinal("province_name_en")) ? string.Empty : reader.GetString("province_name_en"),
                                };
                                province_info.Add(provinces);
                            }
                        }
                    }
                }
                Debug.WriteLine("Data Province loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql Load Provinces Error: " + ex.ToString());
            }
            return province_info;
        }
        //Update Province
        public bool Update_Provinces_Information(Provinces_Info provinces_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Province_Info SET province_name_kh = @province_name_kh, province_name_en = @province_name_en WHERE PV_ID = @PV_ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@PV_ID", provinces_info.PV_ID);
                    cmd.Parameters.AddWithValue("@province_name_kh", provinces_info.Province_Name_KH);
                    cmd.Parameters.AddWithValue("@province_name_en", provinces_info.Province_Name_EN);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Update Province Error: " + ex.ToString());
                return false;
            }
        }
        //Method to Select Last P_ID and PV_ID
        public (int P_ID, string PV_ID) Get_P_ID_and_PV_ID()
        {
            int P_ID = 0;
            string Last_PV_ID = "PRO_000";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(province_id) AS P_ID, MAX(PV_ID) AS Last_PV_ID FROM Province_Info";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //ID = reader.GetInt32("ID");
                            //Last_Stu_ID = reader.GetString("Last_Stu_ID");
                            P_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("P_ID");
                            Last_PV_ID = reader.IsDBNull(1) ? "PRO_000" : reader.GetString("Last_PV_ID");
                        }

                    }
                }
            }
            P_ID++;
            string PV_ID = IncrementPV_ID(Last_PV_ID);

            return (P_ID, PV_ID);

        }
        //Method to Increase PV_ID
        public string IncrementPV_ID(String currentPV_ID)
        {
            // Assuming the format is always "PRO_" + 3-digit number
            string prefix = "PRO_";
            string numericPart = currentPV_ID.Substring(4); // Extract the numeric part after "PRO_"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 3-digit string with leading zeros
            string newNumericPart = number.ToString("D3");

            // Combine the prefix and the incremented numeric part
            string PV_ID = prefix + newNumericPart;

            return PV_ID;
        }
        
        //Delete Province
        public bool Delete_Province_Information(Provinces_Info province_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Province_Info WHERE PV_ID = @pv_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@pv_id", province_info.PV_ID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Delete Province_Info Error: " + ex.ToString());
                return false;
            }
        }
        //End Province_Info

        
        //Start District
        //Get data to Combobox
        public List<Districts_Info> GetProvince_toCombobox()
        {
            List<Districts_Info> provinces = new List<Districts_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT province_id, province_name_kh FROM Province_Info";
                MySqlCommand command = new MySqlCommand(query, connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        provinces.Add(new Districts_Info
                        {
                            Province_ID = reader.GetInt32("province_id"),
                            District_In_Pro = reader.GetString("province_name_kh")
                        });
                    }
                }
            }

            return provinces;
        }
        //Save District Info
        public bool Insert_Districts(Districts_Info districts_Info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO District_Info (DS_ID, district_name_kh, district_name_en, province_id) VALUES(@DS_ID, @district_name_kh, @district_name_en, @province_id)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@DS_ID", districts_Info.DS_ID);
                    cmd.Parameters.AddWithValue("@district_name_kh", districts_Info.District_Name_KH);
                    cmd.Parameters.AddWithValue("@district_name_en", districts_Info.District_Name_EN);
                    cmd.Parameters.AddWithValue("@province_id", districts_Info.SelectedProvince);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Insert District Info Error: {ex.ToString()}");
                return false;
            }
        }
        //Load Data District to ListView
        public List<Districts_Info> LoadDistricts_Info()
        {
            Debug.WriteLine("Starting LoadDistrict Info method...");

            List<Districts_Info> district_info = new List<Districts_Info>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT d.DS_ID, " +
                        "d.district_name_kh, " +
                        "d.district_name_en, " +
                        "d.province_id, " +
                        "p.province_name_kh " +
                        "FROM District_Info d  " +
                        "JOIN Province_Info p ON d.province_id = p.province_id " +
                        "ORDER BY DS_ID DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Debug.WriteLine("Query executed, reading data...");
                            while (reader.Read())
                            {
                                Districts_Info districts = new Districts_Info()
                                {
                                    DS_ID = reader.GetString("DS_ID"),
                                    District_Name_KH = reader.IsDBNull(reader.GetOrdinal("district_name_kh")) ? string.Empty : reader.GetString("district_name_kh"),
                                    District_Name_EN = reader.IsDBNull(reader.GetOrdinal("district_name_en")) ? string.Empty : reader.GetString("district_name_en"),
                                    Province_ID = reader.GetInt32("province_id"),
                                    District_In_Pro = reader.IsDBNull(reader.GetOrdinal("province_name_kh")) ? string.Empty : reader.GetString("province_name_kh"),
                                };
                                district_info.Add(districts);
                            }
                        }
                    }
                }
                Debug.WriteLine("Data District loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql Load District Error: " + ex.ToString());
            }
            return district_info;
        }
        //Update District Info
        public bool Update_Districts_Information(Districts_Info districts_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    string query = "UPDATE District_Info SET district_name_kh = @district_name_kh, district_name_en = @district_name_en, province_id = @province_id WHERE DS_ID = @DS_ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@DS_ID", districts_info.DS_ID);
                    cmd.Parameters.AddWithValue("@district_name_kh", districts_info.District_Name_KH);
                    cmd.Parameters.AddWithValue("@district_name_en", districts_info.District_Name_EN);
                    cmd.Parameters.AddWithValue("@province_id", districts_info.Province_ID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Update District Error: " + ex.ToString());
                return false;
            }
        }
        //Method to Select Last D_ID and DS_ID
        public (int D_ID, string DS_ID) Get_D_ID_and_DS_ID()
        {
            int D_ID = 0;
            string Last_DS_ID = "DIS_000";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(district_id) AS D_ID, MAX(DS_ID) AS Last_DS_ID FROM District_Info";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //ID = reader.GetInt32("ID");
                            //Last_Stu_ID = reader.GetString("Last_Stu_ID");
                            D_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("D_ID");
                            Last_DS_ID = reader.IsDBNull(1) ? "DIS_000" : reader.GetString("Last_DS_ID");
                        }

                    }
                }
            }
            D_ID++;
            string DS_ID = IncrementDS_ID(Last_DS_ID);

            return (D_ID, DS_ID);

        }
        //Method to Increase DS_ID
        public string IncrementDS_ID(String currentDS_ID)
        {
            // Assuming the format is always "DIS_" + 3-digit number
            string prefix = "DIS_";
            string numericPart = currentDS_ID.Substring(4); // Extract the numeric part after "DIS_"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 3-digit string with leading zeros
            string newNumericPart = number.ToString("D3");

            // Combine the prefix and the incremented numeric part
            string DS_ID = prefix + newNumericPart;

            return DS_ID;
        }
        //Delete District Info
        public bool Delete_District_Information(Districts_Info districts_Info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM District_Info WHERE DS_ID = @DS_ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@DS_ID", districts_Info.DS_ID);
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Delete District Error: " + ex.Message);
                return false;
            }
        }
        //End District Info

        //Start Commune Info

        //Get data to Combobox
        public List<Communes_Info> GetDistrict_toCombobox(int provinceID)
        {
            List<Communes_Info> districts = new List<Communes_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT district_id, district_name_kh FROM District_Info WHERE province_id =@ProvinceID";

                // Log the query execution to confirm correct provinceId is used
                Debug.WriteLine($"Executing Query with Province_ID: {provinceID}");

                MySqlCommand command = new MySqlCommand(query, connection);
                //Get ID By Select Province
                command.Parameters.AddWithValue("@ProvinceID", provinceID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {       
                    while (reader.Read())
                    {
                        districts.Add(new Communes_Info
                        {
                            District_ID = reader.GetInt32("district_id"),
                            Commune_In_Dis = reader.GetString("district_name_kh")
                        });
  
                    }
                   
                }
            }  
            return districts;       
        }
        
        //Save District
        public bool Insert_Communes(Communes_Info communes_Info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Commune_Info (CM_ID, commune_name_kh, commune_name_en, district_id) VALUES(@CM_ID, @commune_name_kh, @commune_name_en, @district_id)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@CM_ID", communes_Info.CM_ID);
                    cmd.Parameters.AddWithValue("@commune_name_kh", communes_Info.Commune_Name_KH);
                    cmd.Parameters.AddWithValue("@commune_name_en", communes_Info.Commune_Name_EN);
                    cmd.Parameters.AddWithValue("@district_id", communes_Info.SelectedDistrict_Incomm);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Insert Commune Error: {ex.ToString()}");
                return false;
            }
        }
        //Load Data Commune to ListView
        public List<Communes_Info> LoadCommunes_Info()
        {
            Debug.WriteLine("Starting LoadCommune Info method...");

            List<Communes_Info> commune_info = new List<Communes_Info>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT c.CM_ID, c.commune_name_kh, c.commune_name_en, c.district_id, " +
                      "d.district_name_kh, p.province_name_kh " +
                      "FROM Commune_Info c " +
                      "JOIN District_Info d ON c.district_id = d.district_id " +
                      "JOIN Province_Info p ON d.province_id = p.province_id " +
                      "ORDER BY c.CM_ID DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Debug.WriteLine("Query executed, reading data...");
                            while (reader.Read())
                            {
                                Communes_Info communces = new Communes_Info()
                                {
                                    CM_ID = reader.GetString("CM_ID"),
                                    Commune_Name_KH = reader.IsDBNull(reader.GetOrdinal("commune_name_kh")) ? string.Empty : reader.GetString("commune_name_kh"),
                                    Commune_Name_EN = reader.IsDBNull(reader.GetOrdinal("commune_name_en")) ? string.Empty : reader.GetString("commune_name_en"),
                                    District_ID = reader.GetInt32("district_id"),
                                    Commune_In_Dis = reader.IsDBNull(reader.GetOrdinal("district_name_kh")) ? string.Empty : reader.GetString("district_name_kh"),
                                    Commune_In_Pro = reader.IsDBNull(reader.GetOrdinal("province_name_kh")) ? string.Empty : reader.GetString("province_name_kh")
                                };
                                commune_info.Add(communces);
                            }
                        }
                    }
                }
                Debug.WriteLine("Data Command loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql Load Command Error: " + ex.ToString());
            }
            return commune_info;
        }
        //Update Communce Info
        public bool Update_Communes_Information(Communes_Info communes_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Commune_Info SET commune_name_kh = @commune_name_kh, commune_name_en = @commune_name_en, district_id = @district_id WHERE CM_ID = @CM_ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@CM_ID", communes_info.CM_ID);
                    cmd.Parameters.AddWithValue("@commune_name_kh", communes_info.Commune_Name_KH);
                    cmd.Parameters.AddWithValue("@commune_name_en", communes_info.Commune_Name_EN);
                    cmd.Parameters.AddWithValue("@district_id", communes_info.District_ID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Update Commune Error: " + ex.ToString());
                return false;
            }
        }
        //Get Last C_ID, CM_ID
        public (int C_ID, string CM_ID) Get_C_ID_and_CM_ID()
        {
            int C_ID = 0;
            string Last_CM_ID = "COM_000";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(commune_id) AS C_ID, MAX(CM_ID) AS Last_CM_ID FROM Commune_Info";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //ID = reader.GetInt32("ID");
                            //Last_Stu_ID = reader.GetString("Last_Stu_ID");
                            C_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("C_ID");
                            Last_CM_ID = reader.IsDBNull(1) ? "COM_000" : reader.GetString("Last_CM_ID");
                        }

                    }
                }
            }
            C_ID++;
            string CM_ID = IncrementCM_ID(Last_CM_ID);

            return (C_ID, CM_ID);

        }
        //Method to Increase DS_ID
        public string IncrementCM_ID(String currentCM_ID)
        {
            // Assuming the format is always "COM_" + 3-digit number
            string prefix = "COM_";
            string numericPart = currentCM_ID.Substring(4); // Extract the numeric part after "DIS_"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 3-digit string with leading zeros
            string newNumericPart = number.ToString("D3");

            // Combine the prefix and the incremented numeric part
            string CM_ID = prefix + newNumericPart;

            return CM_ID;
        }
        //Delete Commune Info
        public bool Delete_Commune_Information(Communes_Info communes_Info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Commune_Info WHERE CM_ID = @CM_ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@CM_ID", communes_Info.CM_ID);
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Delete Commune Error: " + ex.Message);
                return false;
            }
        }
        //End Commune Info

        //Start Village Info
        //Get data to Combobox Commune
        public List<Village_Info> GetCommune_toCombobox(int districtID)
        {
            List<Village_Info> cummunes = new List<Village_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT commune_id, commune_name_kh FROM Commune_Info WHERE district_id =@district_id";

                // Log the query execution to confirm correct DistrictID is used
                Debug.WriteLine($"Executing Query with District_ID: {districtID}");

                MySqlCommand command = new MySqlCommand(query, connection);
                //Get ID By Select Province
                command.Parameters.AddWithValue("@district_id", districtID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cummunes.Add(new Village_Info
                        {
                            Commune_ID = reader.GetInt32("commune_id"),
                            Village_In_Comm = reader.GetString("commune_name_kh")
                        });
                    }
                }
            }
            return cummunes;
        }
        //Insert Village
        public bool Insert_Villages(Village_Info village_Info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Village_Info (VL_ID, village_name_kh, village_name_en, commune_id) VALUES(@VL_ID, @village_name_kh, @village_name_en, @commune_id)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@VL_ID", village_Info.VL_ID);
                    cmd.Parameters.AddWithValue("@village_name_kh", village_Info.Village_Name_KH);
                    cmd.Parameters.AddWithValue("@village_name_en", village_Info.Village_Name_EN);
                    cmd.Parameters.AddWithValue("@commune_id", village_Info.SelectedCommune_InVill);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Insert Village Error: {ex.ToString()}");
                return false;
            }
        }
        //Load Data Village to ListView
        public List<Village_Info> LoadVillages_Info()
        {
            Debug.WriteLine("Starting LoadVillage Info method...");

            List<Village_Info> village_info = new List<Village_Info>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                   
                    string query = "SELECT v.VL_ID, v.village_name_kh, v.village_name_en, v.commune_id, " +
                      "c.commune_name_kh,d.district_name_kh, p.province_name_kh " +
                      "FROM Village_Info v " +
                      "JOIN Commune_Info c ON v.commune_id = c.commune_id " +
                      "JOIN District_Info d ON c.district_id = d.district_id " +
                      "JOIN Province_Info p ON d.province_id = p.province_id " +
                      "ORDER BY v.VL_ID DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Debug.WriteLine("Query executed, reading data...");
                            while (reader.Read())
                            {
                                Village_Info villages = new Village_Info()
                                {
                                    VL_ID = reader.GetString("VL_ID"),
                                    Village_Name_KH = reader.IsDBNull(reader.GetOrdinal("village_name_kh")) ? string.Empty : reader.GetString("village_name_kh"),
                                    Village_Name_EN = reader.IsDBNull(reader.GetOrdinal("village_name_en")) ? string.Empty : reader.GetString("village_name_en"),
                                    Commune_ID = reader.GetInt32("commune_id"),
                                    Village_In_Comm = reader.IsDBNull(reader.GetOrdinal("commune_name_kh")) ? string.Empty : reader.GetString("commune_name_kh"),
                                    Village_In_Dis = reader.IsDBNull(reader.GetOrdinal("district_name_kh")) ? string.Empty : reader.GetString("district_name_kh"),
                                    Village_In_Pro = reader.IsDBNull(reader.GetOrdinal("province_name_kh")) ? string.Empty : reader.GetString("province_name_kh")
                                };
                                village_info.Add(villages);
                            }
                        }
                    }
                }
                Debug.WriteLine("Data Village loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MySql Load Village Error: " + ex.ToString());
            }
            return village_info;
        }
        //Update Village
        public bool Update_Villages_Information(Village_Info village_Info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Village_Info SET village_name_kh = @village_name_kh, village_name_en = @village_name_en, commune_id = @commune_id WHERE VL_ID = @VL_ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@VL_ID", village_Info.VL_ID);
                    cmd.Parameters.AddWithValue("@village_name_kh", village_Info.Village_Name_KH);
                    cmd.Parameters.AddWithValue("@village_name_en", village_Info.Village_Name_EN);
                    cmd.Parameters.AddWithValue("@commune_id", village_Info.Commune_ID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Update Village Error: " + ex.ToString());
                return false;
            }
        }
        //Get Last V_ID, VL_ID
        public (int V_ID, string VL_ID) Get_V_ID_and_VL_ID()
        {
            int V_ID = 0;
            string Last_VL_ID = "VIL_000";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(village_id) AS V_ID, MAX(VL_ID) AS Last_VL_ID FROM Village_Info";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //ID = reader.GetInt32("ID");
                            //Last_Stu_ID = reader.GetString("Last_Stu_ID");
                            V_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("V_ID");
                            Last_VL_ID = reader.IsDBNull(1) ? "VIL_000" : reader.GetString("Last_VL_ID");
                        }

                    }
                }
            }
            V_ID++;
            string VL_ID = IncrementVL_ID(Last_VL_ID);

            return (V_ID, VL_ID);

        }
        //Method to Increase VL_ID
        public string IncrementVL_ID(String currentVL_ID)
        {
            // Assuming the format is always "VIL_" + 3-digit number
            string prefix = "VIL_";
            string numericPart = currentVL_ID.Substring(4); // Extract the numeric part after "VIL_"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 3-digit string with leading zeros
            string newNumericPart = number.ToString("D3");

            // Combine the prefix and the incremented numeric part
            string VL_ID = prefix + newNumericPart;

            return VL_ID;
        }
        //Delete Village Info
        public bool Delete_Village_Information(Village_Info villages_Info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Village_Info WHERE VL_ID = @VL_ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@VL_ID", villages_Info.VL_ID);
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Delete Village Error: " + ex.Message);
                return false;
            }
        }
        //Get Data to Combobox Student_Info
        //Province
        public List<Student_Info> GetProvince_toCombobox_Student_info()
        {
            List<Student_Info> provinces = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT province_id, province_name_kh FROM Province_Info";
                MySqlCommand command = new MySqlCommand(query, connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        provinces.Add(new Student_Info
                        {
                            Stu_Birth_Province_ID = reader.GetInt32("province_id"),
                            Stu_Birth_Province = reader.GetString("province_name_kh")
                        });
                    }
                }
            }

            return provinces;
        }
        //Live Provice
        public List<Student_Info> GetLive_Province_toCombobox_Student_info()
        {
            List<Student_Info> live_provinces = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT province_id, province_name_kh FROM Province_Info";
                MySqlCommand command = new MySqlCommand(query, connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        live_provinces.Add(new Student_Info
                        {
                            Stu_Live_Pro_ID = reader.GetInt32("province_id"),
                            Stu_Live_Pro = reader.GetString("province_name_kh")
                        });
                    }
                }
            }
            return live_provinces;
        }

        //District
        public List<Student_Info> GetBirthDistrict_toCombobox(int ProvinceID)
        {
            List<Student_Info> districts = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT district_id, district_name_kh FROM District_Info WHERE province_id =@ProvinceID";

                // Log the query execution to confirm correct provinceId is used
                Debug.WriteLine($"Executing Query with Province_ID: {ProvinceID}");

                MySqlCommand command = new MySqlCommand(query, connection);
                //Get ID By Select Province
                command.Parameters.AddWithValue("@ProvinceID", ProvinceID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        districts.Add(new Student_Info
                        {
                            Stu_Birth_District_ID = reader.GetInt32("district_id"),
                            Stu_Birth_Distric = reader.GetString("district_name_kh")
                        });

                    }

                }
            }
            return districts;
        }
        //Live District
        public List<Student_Info> GetLiveDistrict_toCombobox(int ProvinceID)
        {
            List<Student_Info> districts = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT district_id, district_name_kh FROM District_Info WHERE province_id =@ProvinceID";

                // Log the query execution to confirm correct provinceId is used
                Debug.WriteLine($"Executing Query with Province_ID: {ProvinceID}");

                MySqlCommand command = new MySqlCommand(query, connection);
                //Get ID By Select Province
                command.Parameters.AddWithValue("@ProvinceID", ProvinceID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        districts.Add(new Student_Info
                        {
                            Stu_Live_Dis_ID = reader.GetInt32("district_id"),
                            Stu_Live_Dis = reader.GetString("district_name_kh")
                        });

                    }

                }
            }
            return districts;
        }
        //Commune
        public List <Student_Info> GetBirthCommune_toCombobox(int DistrictID)
        {
            List<Student_Info> cummunes = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT commune_id, commune_name_kh FROM Commune_Info WHERE district_id =@district_id";

                // Log the query execution to confirm correct DistrictID is used
                Debug.WriteLine($"Executing Query with District_ID: {DistrictID}");

                MySqlCommand command = new MySqlCommand(query, connection);
                //Get ID By Select Province
                command.Parameters.AddWithValue("@district_id", DistrictID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cummunes.Add(new Student_Info
                        {
                            Stu_Birth_Commune_ID = reader.GetInt32("commune_id"),
                            Stu_Birth_Commune = reader.GetString("commune_name_kh")
                        });
                    }
                }
            }
            return cummunes;
        }
        //Live Commune
        public List<Student_Info> GetLiveCommune_toCombobox(int DistrictID)
        {
            List<Student_Info> cummunes = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT commune_id, commune_name_kh FROM Commune_Info WHERE district_id =@district_id";

                // Log the query execution to confirm correct DistrictID is used
                Debug.WriteLine($"Executing Query with District_ID: {DistrictID}");

                MySqlCommand command = new MySqlCommand(query, connection);
                //Get ID By Select Province
                command.Parameters.AddWithValue("@district_id", DistrictID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cummunes.Add(new Student_Info
                        {
                            Stu_Live_Comm_ID = reader.GetInt32("commune_id"),
                            Stu_Live_Comm = reader.GetString("commune_name_kh")
                        });
                    }
                }
            }
            return cummunes;
        }
        //Village
        public List<Student_Info> GetBirthVillage_toCombobox(int CommuneID)
        {
            List<Student_Info> villages = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT village_id, village_name_kh FROM Village_Info WHERE commune_id =@commune_id";

                Debug.WriteLine($"Executing Query with Commue_ID: {CommuneID}");

                MySqlCommand command = new MySqlCommand(query, connection);
               
                command.Parameters.AddWithValue("@commune_id", CommuneID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        villages.Add(new Student_Info
                        {
                            Stu_Birth_Village_ID = reader.GetInt32("village_id"),
                            Stu_Birth_Village = reader.GetString("village_name_kh")
                        });
                    }
                }
            }
            return villages;
        }
        //Live_Village
        public List<Student_Info> GetLiveVillage_toCombobox(int CommuneID)
        {
            List<Student_Info> live_villages = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT village_id, village_name_kh FROM Village_Info WHERE commune_id =@commune_id";

                Debug.WriteLine($"Executing Query with Commue_ID: {CommuneID}");

                MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@commune_id", CommuneID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        live_villages.Add(new Student_Info
                        {
                            Stu_Live_Vill_ID = reader.GetInt32("village_id"),
                            Stu_Live_Vill = reader.GetString("village_name_kh")
                        });
                    }
                }
            }
            return live_villages;
        }
        //EducationLevel
        public List<Student_Info> GetEducationLevel_toCombobox_Student_info()
        {
            List<Student_Info> educaationilevels = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT ID, edu_level_name_kh FROM education_level_info"; 
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        educaationilevels.Add(new Student_Info
                        {
                            Stu_EducationLevels_ID = reader.GetInt32("ID"),
                            Stu_EducationLevels = reader.GetString("edu_level_name_kh")
                        });
                    }
                }
            }
            return educaationilevels;
        }
        //EducationSkillSubject
        public List<Student_Info> GetEducationSkillSubject_toCombobox_Student_Info()
        {
            List<Student_Info> educationtskillsubjects = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT ID, edu_skill_name_kh FROM education_skill_info";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        educationtskillsubjects.Add(new Student_Info
                        {
                            Stu_EducationSubject_ID = reader.GetInt32("ID"),
                            Stu_EducationSubjects = reader.GetString("edu_skill_name_kh")
                        });
                    }
                }
            }
            return educationtskillsubjects;
        }
        //EducationStudyTimeShift
        public List<Student_Info> GetEducationStudyTimeShift_toCombobox_Student_Info()
        {
            List<Student_Info> educationtstudytimeshift = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT ID, edu_studytimeshift_name_kh FROM education_studytimeshift_info";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        educationtstudytimeshift.Add(new Student_Info
                        {
                            Stu_StudyTimeShift_ID = reader.GetInt32("ID"),
                            Stu_StudyTimeShift = reader.GetString("edu_studytimeshift_name_kh")
                        });
                    }
                }
            }
            return educationtstudytimeshift;
        }
        //EducationStudyTimeType
        public List<Student_Info> GetEducationStudyType_toCombobox_Student_Info()
        {
            List<Student_Info> educationtstudytype = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT ID, edu_typestudy_name_kh FROM education_typestudy_info";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        educationtstudytype.Add(new Student_Info
                        {
                            Stu_EducationType_ID = reader.GetInt32("ID"),
                            Stu_EducationType = reader.GetString("edu_typestudy_name_kh")
                        });
                    }
                }
            }
            return educationtstudytype;
        }
        //EducationStudyYear
        public List<Student_Info> GetEducationStudyYear_toCombobox_Student_Info()
        {
            List<Student_Info> educationtstudyyear = new List<Student_Info>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT ID, edu_studyyear_name FROM education_studyyear_info";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        educationtstudyyear.Add(new Student_Info
                        {
                            Stu_StudyYear_ID = reader.GetInt32("ID"),
                            Stu_StudyYear = reader.GetString("edu_studyyear_name")
                        });
                    }
                }
            }
            return educationtstudyyear;
        }
        
        //Search by TextBox Name+ID
        public List<Student_Info> GetStudents_Check_Stu_Info(int page, int pageSize, string Search_ID_Name)
        {
            List<Student_Info> students_info = new List<Student_Info>();
            try
            {
                int offset = (page - 1) * pageSize;
                string query = string.IsNullOrEmpty(Search_ID_Name)
        ? "SELECT * FROM student_infomations ORDER BY stu_id DESC LIMIT @Offset, @PageSize"
        : "SELECT * FROM student_infomations WHERE stu_id LIKE @Search_ID_Name || stu_firstname_kh LIKE @Search_ID_Name || stu_lastname_kh LIKE @Search_ID_Name || stu_firstname_en LIKE @Search_ID_Name || stu_lastname_en LIKE @Search_ID_Name ORDER BY stu_id DESC LIMIT @Offset, @PageSize ";

                // string query = "SELECT * FROM student_infomations WHERE stu_id LIKE @Search_ID_Name";


                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrWhiteSpace(Search_ID_Name))
                        {
                            cmd.Parameters.AddWithValue("@Search_ID_Name", $"%{Search_ID_Name}%");
                        }

                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.CommandTimeout = 30;  // Set a timeout of 30 seconds
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Student_Info students = new Student_Info
                                {
                                    ID = reader.GetInt32("ID"),
                                    Stu_ID = reader.GetString("stu_id"),
                                    Stu_FirstName_KH = reader.GetString("stu_firstname_kh"),
                                    Stu_LastName_KH = reader.GetString("stu_lastname_kh"),
                                    Stu_FirstName_EN = reader.GetString("stu_firstname_en"),
                                    Stu_LastName_EN = reader.GetString("stu_lastname_en"),
                                    Stu_BirthdayDateOnly = reader.GetString("stu_birthday_dateonly"),
                                    Stu_Gender = reader.GetString("stu_gender"),
                                    Stu_StateFamily = reader.GetString("stu_state_family"),
                                    Stu_EducationLevels = reader.GetString("stu_education_level"),
                                    Stu_EducationSubjects = reader.GetString("stu_education_subject"),
                                    Stu_StudyTimeShift = reader.GetString("stu_study_time_shift"),
                                    Stu_PhoneNumber = reader.GetString("stu_phone_number"),
                                    Stu_EducationType = reader.GetString("stu_education_types"),
                                    Stu_NationalID = reader.GetString("stu_nation_id"),
                                    Stu_StudyingTime = reader.GetString("stu_studying_time"),
                                    Stu_Birth_Province = reader.IsDBNull(reader.GetOrdinal("stu_birth_province")) ? string.Empty : reader.GetString("stu_birth_province"),
                                    Stu_Birth_Distric = reader.IsDBNull(reader.GetOrdinal("stu_birth_distric")) ? string.Empty : reader.GetString("stu_birth_distric"),
                                    Stu_Birth_Commune = reader.IsDBNull(reader.GetOrdinal("stu_birth_commune")) ? string.Empty : reader.GetString("stu_birth_commune"),
                                    Stu_Birth_Village = reader.IsDBNull(reader.GetOrdinal("stu_birth_village")) ? string.Empty : reader.GetString("stu_birth_village"),
                                    Stu_Live_Pro = reader.IsDBNull(reader.GetOrdinal("stu_live_province")) ? string.Empty : reader.GetString("stu_live_province"),
                                    Stu_Live_Dis = reader.IsDBNull(reader.GetOrdinal("stu_live_distric")) ? string.Empty : reader.GetString("stu_live_distric"),
                                    Stu_Live_Comm = reader.IsDBNull(reader.GetOrdinal("stu_live_commune")) ? string.Empty : reader.GetString("stu_live_commune"),
                                    Stu_Live_Vill = reader.IsDBNull(reader.GetOrdinal("stu_live_village")) ? string.Empty : reader.GetString("stu_live_village"),
                                    Stu_Jobs = reader.IsDBNull(reader.GetOrdinal("stu_jobs")) ? string.Empty : reader.GetString("stu_jobs"),
                                    Stu_School = reader.IsDBNull(reader.GetOrdinal("stu_school")) ? string.Empty : reader.GetString("stu_school"),
                                    Stu_StudyYear = reader.GetString("stu_study_year"),
                                    Stu_Semester = reader.GetString("stu_semester"),
                                    Stu_Mother_Name = reader.IsDBNull(reader.GetOrdinal("stu_mother_name")) ? string.Empty : reader.GetString("stu_mother_name"),
                                    Stu_Mother_Phone = reader.IsDBNull(reader.GetOrdinal("stu_mother_phone_number")) ? string.Empty : reader.GetString("stu_mother_phone_number"),
                                    Stu_Mother_Job = reader.IsDBNull(reader.GetOrdinal("stu_mother_job")) ? string.Empty : reader.GetString("stu_mother_job"),
                                    Stu_Father_Name = reader.IsDBNull(reader.GetOrdinal("stu_father_name")) ? string.Empty : reader.GetString("stu_father_name"),
                                    Stu_Father_Phone = reader.IsDBNull(reader.GetOrdinal("stu_father_phone_number")) ? string.Empty : reader.GetString("stu_father_phone_number"),
                                    Stu_Father_Job = reader.IsDBNull(reader.GetOrdinal("stu_father_job")) ? string.Empty : reader.GetString("stu_father_job"),
                                    Stu_Image_YesNo = reader.GetString("stu_image_yes_no"),
                                    ProfileImageBytes = reader.IsDBNull(reader.GetOrdinal("stu_image_source")) ? null : (byte[])reader["stu_image_source"],
                                    Stu_Image_Total_Big = reader.IsDBNull(reader.GetOrdinal("stu_image_total_big")) ? string.Empty : reader.GetString("stu_image_total_big"),
                                    Stu_Image_TotalSmall = reader.GetString("stu_image_total_small"),
                                    Stu_Images_Degree_Yes_No = reader.GetString("stu_image_degree_yes_no"),
                                    Stu_Image_Degree_Bytes = reader.IsDBNull(reader.GetOrdinal("stu_image_degree_source")) ? null : (byte[])reader["stu_image_degree_source"],
                                    Stu_ImageBirth_Cert_YesNo = reader.GetString("stu_image_birth_cert_yes_no"),
                                    Stu_ImageBirth_Cert_Bytes = reader.IsDBNull(reader.GetOrdinal("stu_image_birth_cert_source")) ? null : (byte[])reader["stu_image_birth_cert_source"],
                                    Stu_ImageIDNation_YesNo = reader.GetString("stu_image_id_nation_yes_no"),
                                    //Stu_ImageIDNation_Bytes = this.Stu_ImageIDNation_Bytes,
                                    Stu_ImagePoor_Card_YesNo = reader.GetString("stu_image_poor_card_yes_no"),
                                    //Stu_Image_Poor_Card_Bytes = this.Stu_Image_Poor_Card_Bytes,
                                    Stu_Insert_by_ID = reader.GetString("stu_insert_by_id"),
                                    Stu_Insert_DateTime = reader.GetDateTime("stu_insert_datetime"),
                                    Stu_Insert_Info = reader.IsDBNull(reader.GetOrdinal("stu_insert_info")) ? string.Empty : reader.GetString("stu_insert_info"),
                                    Stu_Update_By_ID = reader.GetString("stu_update_by_id"),
                                    Stu_Update_DateTime = reader.GetDateTime("stu_update_datetime"),
                                    Stu_Update_Info = reader.IsDBNull(reader.GetOrdinal("stu_update_info")) ? string.Empty : reader.GetString("stu_update_info"),
                                    Stu_Delete_By_ID = reader.GetString("stu_delete_by_id"),
                                    Stu_Delete_DateTime = reader.GetDateTime("stu_delete_datetime"),
                                    Stu_Delete_Info = reader.IsDBNull(reader.GetOrdinal("stu_delete_info")) ? string.Empty : reader.GetString("stu_delete_info")


                                };



                                // Read the image as byte array from MySQL  
                                // Stu_Image
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_source")))
                                {
                                    // First, get the size of the image byte array from the database
                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, null, 0, 0);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.ProfileImageBytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_Image_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage
                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("stu_image_source is NULL for student " + students.Stu_ID);
                                }



                                //Stu_Images_Degree
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_degree_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_degree_source"), 0, null, 0, 0);
                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_degree_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_Image_Degree_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_Image_Degree_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                //Stu_ImageBirth_Cert
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_birth_cert_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_birth_cert_source"), 0, null, 0, 0);
                                    Debug.WriteLine("Byte size for student " + students.Stu_ID + ": " + byteSize);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_birth_cert_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_ImageBirth_Cert_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_ImageBirth_Cert_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                //Stu_Image_IDNation
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_id_nation_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_id_nation_source"), 0, null, 0, 0);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_id_nation_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_ImageIDNation_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_ImageIDNation_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                //Stu_ImagePoor_Card
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_poor_card_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_poor_card_source"), 0, null, 0, 0);


                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_poor_card_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_Image_Poor_Card_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_ImagePoor_Card_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                students_info.Add(students);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"MySql Get Student to ListView Error:{ex.Message}");
            }
            return students_info;
        }

        public async Task<List<(string skill, int total_count, int female_count)>> GetStudentBarGraphDataAsync()
        {
            var data = new List<(string skill, int total_count, int female_count)>();

            string query = "SELECT " +
                "stu_education_subject, " +
                "COUNT(stu_id) AS student_count, " +
                "COUNT(CASE WHEN stu_gender = 'ស្រី' THEN 1 END) AS female_student_count " +
                "FROM student_infomations " +
                "GROUP BY stu_education_subject;";

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();


                    while (await reader.ReadAsync())
                    {
                        string skill = reader.GetString("stu_education_subject");
                        int total_count = reader.GetInt32("student_count");
                        int female_count = reader.GetInt32("female_student_count");
                        data.Add((skill, total_count, female_count));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Database Error: " + ex.Message);
                }
            }

            return data;
        }
        
        //Search by Combobox
        public List<Student_Info> GetStudents_Check_Stu_Info_by_Combobox(int page, int pageSize, String Search_Edu_Level, String Search_Edu_Skill_Subject, String Search_Edu_StudyTimeShift, String Search_Edu_TypeStudy, String Search_Edu_StudyYear)
        {
            List<Student_Info> students_info = new List<Student_Info>();
            try
            {
                int offset = (page - 1) * pageSize;              
                string query = "SELECT * FROM student_infomations";

                List<string> conditions = new List<string>();
                if (!string.IsNullOrEmpty(Search_Edu_Level))
                {
                    conditions.Add("stu_education_level = @Search_Edu_Level");
                }
                if (!string.IsNullOrEmpty(Search_Edu_Skill_Subject))
                {
                    conditions.Add("stu_education_subject = @Search_Edu_Skill_Subject");
                }
                if (!string.IsNullOrEmpty(Search_Edu_StudyTimeShift))
                {
                    conditions.Add("stu_study_time_shift = @Search_Edu_StudyTimeShift");
                }
                if (!string.IsNullOrEmpty(Search_Edu_TypeStudy))
                {
                    conditions.Add("stu_education_types = @Search_Edu_TypeStudy");
                }
                if (!string.IsNullOrEmpty(Search_Edu_StudyYear))
                {
                    conditions.Add("stu_study_year = @Search_Edu_StudyYear");
                }
                // Check if there are any conditions to add to the WHERE clause
                if (conditions.Count > 0)
                {
                    // Add the WHERE clause by joining conditions with " AND "
                    query += " WHERE " + string.Join(" AND ", conditions);
                }

                // Add ORDER BY and LIMIT clauses
                query += " ORDER BY stu_id DESC LIMIT @Offset, @PageSize";
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Search_Edu_StudyYear", Search_Edu_StudyYear);
                        cmd.Parameters.AddWithValue("@Search_Edu_TypeStudy", Search_Edu_TypeStudy);
                        cmd.Parameters.AddWithValue("@Search_Edu_StudyTimeShift", Search_Edu_StudyTimeShift);
                        cmd.Parameters.AddWithValue("@Search_Edu_Skill_Subject", Search_Edu_Skill_Subject);
                        cmd.Parameters.AddWithValue("@Search_Edu_Level", Search_Edu_Level);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.CommandTimeout = 30;  // Set a timeout of 30 seconds
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Student_Info students = new Student_Info
                                {
                                    ID = reader.GetInt32("ID"),
                                    Stu_ID = reader.GetString("stu_id"),
                                    Stu_FirstName_KH = reader.GetString("stu_firstname_kh"),
                                    Stu_LastName_KH = reader.GetString("stu_lastname_kh"),
                                    Stu_FirstName_EN = reader.GetString("stu_firstname_en"),
                                    Stu_LastName_EN = reader.GetString("stu_lastname_en"),
                                    Stu_BirthdayDateOnly = reader.GetString("stu_birthday_dateonly"),
                                    Stu_Gender = reader.GetString("stu_gender"),
                                    Stu_StateFamily = reader.GetString("stu_state_family"),
                                    Stu_EducationLevels = reader.GetString("stu_education_level"),
                                    Stu_EducationSubjects = reader.GetString("stu_education_subject"),
                                    Stu_StudyTimeShift = reader.GetString("stu_study_time_shift"),
                                    Stu_PhoneNumber = reader.GetString("stu_phone_number"),
                                    Stu_EducationType = reader.GetString("stu_education_types"),
                                    Stu_NationalID = reader.GetString("stu_nation_id"),
                                    Stu_StudyingTime = reader.GetString("stu_studying_time"),
                                    Stu_Birth_Province = reader.IsDBNull(reader.GetOrdinal("stu_birth_province")) ? string.Empty : reader.GetString("stu_birth_province"),
                                    Stu_Birth_Distric = reader.IsDBNull(reader.GetOrdinal("stu_birth_distric")) ? string.Empty : reader.GetString("stu_birth_distric"),
                                    Stu_Birth_Commune = reader.IsDBNull(reader.GetOrdinal("stu_birth_commune")) ? string.Empty : reader.GetString("stu_birth_commune"),
                                    Stu_Birth_Village = reader.IsDBNull(reader.GetOrdinal("stu_birth_village")) ? string.Empty : reader.GetString("stu_birth_village"),
                                    Stu_Live_Pro = reader.IsDBNull(reader.GetOrdinal("stu_live_province")) ? string.Empty : reader.GetString("stu_live_province"),
                                    Stu_Live_Dis = reader.IsDBNull(reader.GetOrdinal("stu_live_distric")) ? string.Empty : reader.GetString("stu_live_distric"),
                                    Stu_Live_Comm = reader.IsDBNull(reader.GetOrdinal("stu_live_commune")) ? string.Empty : reader.GetString("stu_live_commune"),
                                    Stu_Live_Vill = reader.IsDBNull(reader.GetOrdinal("stu_live_village")) ? string.Empty : reader.GetString("stu_live_village"),
                                    Stu_Jobs = reader.IsDBNull(reader.GetOrdinal("stu_jobs")) ? string.Empty : reader.GetString("stu_jobs"),
                                    Stu_School = reader.IsDBNull(reader.GetOrdinal("stu_school")) ? string.Empty : reader.GetString("stu_school"),
                                    Stu_StudyYear = reader.GetString("stu_study_year"),
                                    Stu_Semester = reader.GetString("stu_semester"),
                                    Stu_Mother_Name = reader.IsDBNull(reader.GetOrdinal("stu_mother_name")) ? string.Empty : reader.GetString("stu_mother_name"),
                                    Stu_Mother_Phone = reader.IsDBNull(reader.GetOrdinal("stu_mother_phone_number")) ? string.Empty : reader.GetString("stu_mother_phone_number"),
                                    Stu_Mother_Job = reader.IsDBNull(reader.GetOrdinal("stu_mother_job")) ? string.Empty : reader.GetString("stu_mother_job"),
                                    Stu_Father_Name = reader.IsDBNull(reader.GetOrdinal("stu_father_name")) ? string.Empty : reader.GetString("stu_father_name"),
                                    Stu_Father_Phone = reader.IsDBNull(reader.GetOrdinal("stu_father_phone_number")) ? string.Empty : reader.GetString("stu_father_phone_number"),
                                    Stu_Father_Job = reader.IsDBNull(reader.GetOrdinal("stu_father_job")) ? string.Empty : reader.GetString("stu_father_job"),
                                    Stu_Image_YesNo = reader.GetString("stu_image_yes_no"),
                                    ProfileImageBytes = reader.IsDBNull(reader.GetOrdinal("stu_image_source")) ? null : (byte[])reader["stu_image_source"],
                                    Stu_Image_Total_Big = reader.IsDBNull(reader.GetOrdinal("stu_image_total_big")) ? string.Empty : reader.GetString("stu_image_total_big"),
                                    Stu_Image_TotalSmall = reader.GetString("stu_image_total_small"),
                                    Stu_Images_Degree_Yes_No = reader.GetString("stu_image_degree_yes_no"),
                                    Stu_Image_Degree_Bytes = reader.IsDBNull(reader.GetOrdinal("stu_image_degree_source")) ? null : (byte[])reader["stu_image_degree_source"],
                                    Stu_ImageBirth_Cert_YesNo = reader.GetString("stu_image_birth_cert_yes_no"),
                                    Stu_ImageBirth_Cert_Bytes = reader.IsDBNull(reader.GetOrdinal("stu_image_birth_cert_source")) ? null : (byte[])reader["stu_image_birth_cert_source"],
                                    Stu_ImageIDNation_YesNo = reader.GetString("stu_image_id_nation_yes_no"),
                                    //Stu_ImageIDNation_Bytes = this.Stu_ImageIDNation_Bytes,
                                    Stu_ImagePoor_Card_YesNo = reader.GetString("stu_image_poor_card_yes_no"),
                                    //Stu_Image_Poor_Card_Bytes = this.Stu_Image_Poor_Card_Bytes,
                                    Stu_Insert_by_ID = reader.GetString("stu_insert_by_id"),
                                    Stu_Insert_DateTime = reader.GetDateTime("stu_insert_datetime"),
                                    Stu_Insert_Info = reader.IsDBNull(reader.GetOrdinal("stu_insert_info")) ? string.Empty : reader.GetString("stu_insert_info"),
                                    Stu_Update_By_ID = reader.GetString("stu_update_by_id"),
                                    Stu_Update_DateTime = reader.GetDateTime("stu_update_datetime"),
                                    Stu_Update_Info = reader.IsDBNull(reader.GetOrdinal("stu_update_info")) ? string.Empty : reader.GetString("stu_update_info"),
                                    Stu_Delete_By_ID = reader.GetString("stu_delete_by_id"),
                                    Stu_Delete_DateTime = reader.GetDateTime("stu_delete_datetime"),
                                    Stu_Delete_Info = reader.IsDBNull(reader.GetOrdinal("stu_delete_info")) ? string.Empty : reader.GetString("stu_delete_info")


                                };



                                // Read the image as byte array from MySQL  
                                // Stu_Image
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_source")))
                                {
                                    // First, get the size of the image byte array from the database
                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, null, 0, 0);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.ProfileImageBytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_Image_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage
                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("stu_image_source is NULL for student " + students.Stu_ID);
                                }



                                //Stu_Images_Degree
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_degree_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_degree_source"), 0, null, 0, 0);
                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_degree_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_Image_Degree_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_Image_Degree_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                //Stu_ImageBirth_Cert
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_birth_cert_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_birth_cert_source"), 0, null, 0, 0);
                                    Debug.WriteLine("Byte size for student " + students.Stu_ID + ": " + byteSize);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_birth_cert_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_ImageBirth_Cert_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_ImageBirth_Cert_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                //Stu_Image_IDNation
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_id_nation_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_id_nation_source"), 0, null, 0, 0);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_id_nation_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_ImageIDNation_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_ImageIDNation_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                //Stu_ImagePoor_Card
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_poor_card_source")))
                                {

                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_poor_card_source"), 0, null, 0, 0);


                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_poor_card_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            students.Stu_Image_Poor_Card_Bytes = imageBytes;  // Store the image bytes for future use
                                            students.Stu_ImagePoor_Card_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage

                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + students.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + students.Stu_ID);
                                    }
                                }

                                students_info.Add(students);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"MySql Get Student to ListView Error:{ex.Message}");
            }
            return students_info;
        }

        //Report search by education level
        public List<Student_Info> GetStudents_Report_Stu_Info_by_Solarship(string student_education_level, string student_education_study_year, string student_education_study_type)
        {
            List<Student_Info> student_info_report = new List<Student_Info>();
            try
            {
                string query;
                if (student_education_level == null && student_education_study_year == null && student_education_study_type == null)
                {
                     query = "SELECT * FROM student_infomations FROM student_infomations ORDER BY FIELD(stu_education_level, 'បរិញ្ញាបត្របច្ចេកវិទ្យា', 'សញ្ញាបត្រវិស្វករ', 'បរិញ្ញាបត្រ','សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស','បរិញ្ញាបត្ររង','សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ៣','សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ២','សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ១','ជំនាញបច្ចេកទេស និងវិជ្ជាជីវៈ (1.5M)','កម្រិតវិញ្ញាបនបត្រវិជ្ជាជីវៈ'), stu_education_level";
                }
                else if(student_education_level == "បរិញ្ញាបត្របច្ចេកវិទ្យា"|| student_education_level == "សញ្ញាបត្រវិស្វករ" || student_education_level == "បរិញ្ញាបត្រ")
                {
                    query = "SELECT * FROM student_infomations WHERE stu_education_level IN ('បរិញ្ញាបត្របច្ចេកវិទ្យា', 'សញ្ញាបត្រវិស្វករ', 'បរិញ្ញាបត្រ')";

                    List<string> conditions = new List<string>();
                    if (!string.IsNullOrEmpty(student_education_study_year))
                    {
                        conditions.Add("stu_study_year = @student_education_study_year");
                    }
                    if (!string.IsNullOrEmpty(student_education_study_type))
                    {
                        conditions.Add("stu_education_types = @student_education_study_type");
                    }

                    // Add conditions to WHERE clause if there are any
                    if (conditions.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", conditions);
                    }

                    // Add ORDER BY clause
                    query += " ORDER BY FIELD(stu_education_level, 'បរិញ្ញាបត្របច្ចេកវិទ្យា', 'សញ្ញាបត្រវិស្វករ', 'បរិញ្ញាបត្រ'), stu_education_level";

                }
                else if(student_education_level == "សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស" || student_education_level == "បរិញ្ញាបត្ររង")
                {

                    query = "SELECT * FROM student_infomations WHERE stu_education_level IN ('សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស', 'បរិញ្ញាបត្ររង')";

                    List<string> conditions = new List<string>();
                    if (!string.IsNullOrEmpty(student_education_study_year))
                    {
                        conditions.Add("stu_study_year = @student_education_study_year");
                    }
                    if (!string.IsNullOrEmpty(student_education_study_type))
                    {
                        conditions.Add("stu_education_types = @student_education_study_type");
                    }

                    // Add conditions to WHERE clause if there are any
                    if (conditions.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", conditions);
                    }

                    // Add ORDER BY clause
                    query += " ORDER BY FIELD(stu_education_level, 'សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស', 'បរិញ្ញាបត្ររង'), stu_education_level";

                }
                else
                {
                    query = "SELECT * FROM student_infomations";

                    List<string> conditions = new List<string>();
                    if (!string.IsNullOrEmpty(student_education_level))
                    {
                        conditions.Add("stu_education_level = @student_education_level");
                    }
                    if (!string.IsNullOrEmpty(student_education_study_year))
                    {
                        conditions.Add("stu_study_year = @student_education_study_year");
                    }
                    if (!string.IsNullOrEmpty(student_education_study_type))
                    {
                        conditions.Add("stu_education_types = @student_education_study_type");
                    }

                    // Add WHERE clause if conditions exist
                    if (conditions.Count > 0)
                    {
                        query += " WHERE " + string.Join(" AND ", conditions);
                    }

                    // Add ORDER BY clause
                    query += " ORDER BY FIELD(stu_education_level, 'បរិញ្ញាបត្របច្ចេកវិទ្យា', 'សញ្ញាបត្រវិស្វករ', 'បរិញ្ញាបត្រ', 'សញ្ញាបត្រជាន់ខ្ពស់បច្ចេកទេស', 'បរិញ្ញាបត្ររង', 'សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ៣', 'សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ២', 'សញ្ញាបត្របច្ចេកទេស និងវិជ្ជាជីវៈ១', 'ជំនាញបច្ចេកទេស និងវិជ្ជាជីវៈ (1.5M)', 'កម្រិតវិញ្ញាបនបត្រវិជ្ជាជីវៈ'), stu_education_level";

                }
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int ID_Number = 1;
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@student_education_study_year", student_education_study_year);
                        cmd.Parameters.AddWithValue("@student_education_level", student_education_level);
                        cmd.Parameters.AddWithValue("@student_education_study_type", student_education_study_type);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Student_Info student_report = new Student_Info
                                {
                                    ID = ID_Number,
                                    Stu_FirstName_KH = reader.GetString("stu_firstname_kh"),
                                    Stu_LastName_KH = reader.GetString("stu_lastname_kh"),
                                    Full_Name_KH = reader.GetString("stu_firstname_kh") + " " + reader.GetString("stu_lastname_kh"),
                                    Stu_LastName_EN = reader.GetString("stu_lastname_en"),
                                    Stu_FirstName_EN = reader.GetString("stu_firstname_en"),
                                    Full_Name_EN = reader.GetString("stu_firstname_en") + " " + reader.GetString("stu_lastname_en"),
                                    Stu_Gender = reader.GetString("stu_gender"),
                                    Stu_BirthdayDateOnly = reader.GetString("stu_birthday_dateonly"),
                                    Stu_PhoneNumber = reader.GetString("stu_phone_number"),
                                    Stu_StatePoor = reader.IsDBNull(reader.GetOrdinal("stu_state_poor")) ? string.Empty : reader.GetString("stu_state_poor"),
                                    Stu_EducationSubjects = reader.GetString("stu_education_subject"),
                                    Stu_EducationLevels = reader.GetString("stu_education_level"),
                                    Stu_StudyYear = reader.GetString("stu_study_year"),
                                    Stu_EducationType = reader.GetString("stu_education_types"),
                                    Stu_StudyTimeShift = reader.GetString("stu_study_time_shift")
                                };
                                ID_Number++;
                                student_info_report.Add(student_report);
                            }
                        }
                    }

                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"MySql Get Student_Info Report to ListView Error:{ex.Message}");
            }
            return student_info_report;
        }


        int No_Classes = 0;
        //Search by Name, Generation Class
        public List<Student_Info> GetClass_Info(int page, int pageSize, string Search_Name_Generation)
        {
            List<Student_Info> class_info = new List<Student_Info>();
            try
            {
                string query1 = "SELECT COUNT(*) AS TotalCount FROM classes";
                int offset = (page - 1) * pageSize;
                string query = string.IsNullOrEmpty(Search_Name_Generation)
                            ? "SELECT * FROM classes ORDER BY class_id DESC LIMIT @Offset, @PageSize"
                            : "SELECT * FROM classes WHERE class_name LIKE @Search_Name_Generation || class_in_generation LIKE @Search_Name_Generation ORDER BY class_id DESC LIMIT @Offset, @PageSize";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var cmd = new MySqlCommand(query1, connection))
                    {
                        No_Classes = Convert.ToInt32(cmd.ExecuteScalar()); // Assign the total count to No_Classes
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrWhiteSpace(Search_Name_Generation))
                        {
                            cmd.Parameters.AddWithValue("@Search_Name_Generation", $"%{Search_Name_Generation}%");
                        }

                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.CommandTimeout = 30;  // Set a timeout of 30 seconds

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            //int No_Classes = 1;
                            while (reader.Read())
                            {
                                
                                Student_Info classes = new Student_Info
                                {
                                    No_Class = No_Classes.ToString(),
                                    Class_ID = reader.GetInt32("class_id").ToString(),
                                    Class_Name = reader.IsDBNull(reader.GetOrdinal("class_name")) ? string.Empty : reader.GetString("class_name"),
                                    Class_In_Skill = reader.IsDBNull(reader.GetOrdinal("class_in_skill")) ? string.Empty : reader.GetString("class_in_skill"),
                                    Class_In_Study_Year = reader.IsDBNull(reader.GetOrdinal("class_in_study_year")) ? string.Empty : reader.GetString("class_in_study_year"),
                                    Class_In_Level = reader.IsDBNull(reader.GetOrdinal("class_in_level")) ? string.Empty : reader.GetString("class_in_level"),
                                    Class_In_Student_Year = reader.IsDBNull(reader.GetOrdinal("class_in_student_year")) ? string.Empty : reader.GetString("class_in_student_year"),
                                    Class_In_Semester = reader.IsDBNull(reader.GetOrdinal("class_in_semester")) ? string.Empty : reader.GetString("class_in_semester"),
                                    Class_In_Generation = reader.IsDBNull(reader.GetOrdinal("class_in_generation")) ? string.Empty : reader.GetString("class_in_generation"),
                                    Class_In_Study_Timeshift = reader.IsDBNull(reader.GetOrdinal("class_in_study_timeshift")) ? string.Empty : reader.GetString("class_in_study_timeshift"),
                                    Class_In_Study_Type = reader.IsDBNull(reader.GetOrdinal("class_in_study_type")) ? string.Empty : reader.GetString("class_in_study_type"),
                                    Max_Student_InClass = reader.IsDBNull(reader.GetOrdinal("Class_Real_Student"))
                                                            ? 0 // Default value if NULL
                                                            : reader.GetInt32("Class_Total_Student_Set"),
                                    Current_Student_InClass = reader.IsDBNull(reader.GetOrdinal("Class_Real_Student"))
                                                            ? 0 // Default value if NULL
                                                            : reader.GetInt32("Class_Real_Student"),
                                    Current_Class_State = reader.IsDBNull(reader.GetOrdinal("Class_State")) ? string.Empty : reader.GetString("Class_State"),

                                };
                                No_Classes--;
                                class_info.Add(classes);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql Get Class to ListView Error:{ex.Message}");
            }
            return class_info;
        }

        //Search Class Combobox
        public List<Student_Info> GetClasses_Check_Info_by_Combobox(int page, int classSize, string Search_Study_Year, string Search_Skill, string Search_Level, string Search_Student_Year, string Search_Semester, string Search_Study_Timeshift, string Search_Study_Type)
        {
            List<Student_Info> class_info = new List<Student_Info>();
            try
            {
                string query1 = "SELECT COUNT(*) AS TotalCount FROM classes";
                int offset = (page - 1) * classSize;
                string query = "SELECT * FROM classes";

                List<string> conditions = new List<string>();
                if (!string.IsNullOrEmpty(Search_Study_Year))
                {
                    conditions.Add("class_in_study_year = @Search_Study_Year");
                }
                if (!string.IsNullOrEmpty(Search_Skill))
                {
                    conditions.Add("class_in_skill = @Search_Skill");
                }
                if (!string.IsNullOrEmpty(Search_Level))
                {
                    conditions.Add("class_in_level = @Search_Level");
                }
                if (!string.IsNullOrEmpty(Search_Student_Year))
                {
                    conditions.Add("class_in_student_year = @Search_Student_Year");
                }
                if (!string.IsNullOrEmpty(Search_Semester))
                {
                    conditions.Add("class_in_semester = @Search_Semester");
                }
                if (!string.IsNullOrEmpty(Search_Study_Timeshift))
                {
                    conditions.Add("class_in_study_timeshift = @Search_Study_Timeshift");
                }
                if (!string.IsNullOrEmpty(Search_Study_Type))
                {
                    conditions.Add("class_in_study_type = @Search_Study_Type");
                }
                // Check if there are any conditions to add to the WHERE clause
                if (conditions.Count > 0)
                {
                    // Add the WHERE clause by joining conditions with " AND "
                    query += " WHERE " + string.Join(" AND ", conditions);
                }

                // Add ORDER BY and LIMIT clauses
                query += " ORDER BY class_id DESC LIMIT @Offset, @ClassSize";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query1, connection))
                    {
                        No_Classes = Convert.ToInt32(cmd.ExecuteScalar()); // Assign the total count to No_Classes
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Search_Study_Year", Search_Study_Year);
                        cmd.Parameters.AddWithValue("@Search_Skill", Search_Skill);
                        cmd.Parameters.AddWithValue("@Search_Level", Search_Level);
                        cmd.Parameters.AddWithValue("@Search_Student_Year", Search_Student_Year);
                        cmd.Parameters.AddWithValue("@Search_Semester", Search_Semester);
                        cmd.Parameters.AddWithValue("@Search_Study_Timeshift", Search_Study_Timeshift);
                        cmd.Parameters.AddWithValue("@Search_Study_Type", Search_Study_Type);
                        cmd.Parameters.AddWithValue("@ClassSize", classSize);
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.CommandTimeout = 30;  // Set a timeout of 30 seconds
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            //int No_Classes = 1;
                            while (reader.Read())
                            {
                                Student_Info classes = new Student_Info
                                {
                                    No_Class = No_Classes.ToString(),
                                    Class_ID = reader.GetInt32("class_id").ToString(),
                                    Class_Name = reader.IsDBNull(reader.GetOrdinal("class_name")) ? string.Empty : reader.GetString("class_name"),
                                    Class_In_Skill = reader.IsDBNull(reader.GetOrdinal("class_in_skill")) ? string.Empty : reader.GetString("class_in_skill"),
                                    Class_In_Study_Year = reader.IsDBNull(reader.GetOrdinal("class_in_study_year")) ? string.Empty : reader.GetString("class_in_study_year"),
                                    Class_In_Level = reader.IsDBNull(reader.GetOrdinal("class_in_level")) ? string.Empty : reader.GetString("class_in_level"),
                                    Class_In_Student_Year = reader.IsDBNull(reader.GetOrdinal("class_in_student_year")) ? string.Empty : reader.GetString("class_in_student_year"),
                                    Class_In_Semester = reader.IsDBNull(reader.GetOrdinal("class_in_semester")) ? string.Empty : reader.GetString("class_in_semester"),
                                    Class_In_Generation = reader.IsDBNull(reader.GetOrdinal("class_in_generation")) ? string.Empty : reader.GetString("class_in_generation"),
                                    Class_In_Study_Timeshift = reader.IsDBNull(reader.GetOrdinal("class_in_study_timeshift")) ? string.Empty : reader.GetString("class_in_study_timeshift"),
                                    Class_In_Study_Type = reader.IsDBNull(reader.GetOrdinal("class_in_study_type")) ? string.Empty : reader.GetString("class_in_study_type"),
                                    Max_Student_InClass = reader.IsDBNull(reader.GetOrdinal("Class_Real_Student"))
                                                            ? 0 // Default value if NULL
                                                            : reader.GetInt32("Class_Total_Student_Set"),
                                    Current_Student_InClass = reader.IsDBNull(reader.GetOrdinal("Class_Real_Student"))
                                                            ? 0 // Default value if NULL
                                                            : reader.GetInt32("Class_Real_Student"),
                                    Current_Class_State = reader.IsDBNull(reader.GetOrdinal("Class_State")) ? string.Empty : reader.GetString("Class_State"),

                                };
                                No_Classes--;
                                class_info.Add(classes);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Search class error: {ex.Message}");
            }
            return class_info;
        }

        //Delete Multi Class
        public void Delete_Class_Info(List<String> class_id_info)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    foreach (var class_id in class_id_info)
                    {
                        string query = "DELETE FROM classes WHERE class_id = @class_id";
                        using (var command = new MySqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@class_id", class_id);
                            command.ExecuteNonQuery();
                            Debug.WriteLine($"Deleted Class ID: {class_id}");
                        }
                    }
                }
                   

            }catch (Exception ex)
            {
                Debug.WriteLine($"Delete mutli class error: {ex.Message}");
            }
        }

        //Method to Select Student to show in Class.
        //int Stu_ID_Show = 1;
        public List<Student_Info> Display_Student_List_in_Class(int Max_Student_InClass, string Class_In_Study_Year, string Class_In_Level, string Class_In_Skill, string Class_In_Student_Year, string Class_In_Study_Timeshift)
        {
            List<Student_Info> student_class_show = new List<Student_Info>();
            try
            {
                //string query_count = "SELECT COUNT(*) AS TotalCount FROM student_infomations WHERE stu_study_year = @Class_In_Study_Year && stu_education_level = @Class_In_Level && stu_education_subject = @Class_In_Skill && stu_studying_time = @Class_In_Student_Year && stu_study_time_shift = @Class_In_Study_Timeshift LIMIT @Max_Student_InClass";
                //string query = "SELECT * FROM student_infomations WHERE stu_study_year = @Class_In_Study_Year && stu_education_level = @Class_In_Level && stu_education_subject = @Class_In_Skill && stu_studying_time = @Class_In_Student_Year && stu_study_time_shift = @Class_In_Study_Timeshift && stu_classes IS NULL ORDER BY RAND() LIMIT @Max_Student_InClass";

                string query = @"
                                SELECT * 
                                FROM student_infomations 
                                WHERE 
                                    stu_study_year = @Class_In_Study_Year
                                    AND stu_education_level = @Class_In_Level
                                    AND stu_education_subject = @Class_In_Skill
                                    AND stu_studying_time = @Class_In_Student_Year
                                    AND stu_study_time_shift = @Class_In_Study_Timeshift
                                    AND stu_classes IS NULL
                                    AND (stu_firstname_kh REGEXP '^[ក-អ]' 
                                    OR stu_firstname_kh REGEXP '^[អឥឦឧឩឪឫឬឭឮឯឰឱឲ]')
                                ORDER BY 
                                    CASE 
                                        WHEN stu_firstname_kh REGEXP '[ក-អ]' THEN 1
                                        WHEN stu_firstname_kh REGEXP '^[អឥឦឧឩឪឫឬឭឮឯឰឱឲ]' THEN 2
                                        ELSE 3
                                    END, 
                                    stu_firstname_kh ASC
                                    LIMIT @Max_Student_InClass";


                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    //Count
                    //using (var cmd = new MySqlCommand(query_count, connection))
                    //{
                    //    cmd.Parameters.AddWithValue("@Class_In_Study_Year", Class_In_Study_Year);
                    //    cmd.Parameters.AddWithValue("@Class_In_Level", Class_In_Level);
                    //    cmd.Parameters.AddWithValue("@Class_In_Skill", Class_In_Skill);
                    //    cmd.Parameters.AddWithValue("@Class_In_Student_Year", Class_In_Student_Year);
                    //    cmd.Parameters.AddWithValue("@Class_In_Study_Timeshift", Class_In_Study_Timeshift);
                    //    cmd.Parameters.AddWithValue("@Max_Student_InClass", Max_Student_InClass);
                    //    cmd.CommandTimeout = 30;

                    //    Stu_ID_Show = Convert.ToInt32(cmd.ExecuteScalar()); // Assign the total count to No_Classes
                    //}
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Class_In_Study_Year", Class_In_Study_Year);
                        cmd.Parameters.AddWithValue("@Class_In_Level", Class_In_Level);
                        cmd.Parameters.AddWithValue("@Class_In_Skill", Class_In_Skill);
                        cmd.Parameters.AddWithValue("@Class_In_Student_Year", Class_In_Student_Year);
                        cmd.Parameters.AddWithValue("@Class_In_Study_Timeshift", Class_In_Study_Timeshift);
                        cmd.Parameters.AddWithValue("@Max_Student_InClass", Max_Student_InClass);
                        cmd.CommandTimeout = 30;

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            int Stu_ID_Show = 1;
                            while (reader.Read())
                            {
                                
                                Student_Info student_in_class = new Student_Info
                                {
                                    ID = reader.GetInt32("ID"),
                                    Stu_ID = reader.GetString("stu_id"),
                                    Stu_FirstName_KH = reader.GetString("stu_firstname_kh"),
                                    Stu_LastName_KH = reader.GetString("stu_lastname_kh"),
                                    Stu_BirthdayDateOnly = reader.GetString("stu_birthday_dateonly"),
                                    Stu_Gender = reader.GetString("stu_gender"),
                                    Stu_IDShow = Stu_ID_Show.ToString()
                                };
                                
                                // Stu_Image
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_source")))
                                {
                                    // First, get the size of the image byte array from the database
                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, null, 0, 0);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            student_in_class.ProfileImageBytes = imageBytes;  // Store the image bytes for future use
                                            student_in_class.Stu_Image_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage
                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + student_in_class.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + student_in_class.Stu_ID);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("stu_image_source is NULL for student " + student_in_class.Stu_ID);
                                }
                                Stu_ID_Show++;
                                student_class_show.Add(student_in_class);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Select Class for show students error: {ex.ToString()}");
            }
            return student_class_show;
        }

        public List<Student_Info> Display_Student_List_in_Class2(string class_id)
        {
            List<Student_Info> student_class_show = new List<Student_Info>();
            try
            {
                string query = "SELECT si.*\r\nFROM student_infomations si\r\nJOIN class_enrollments ce ON si.ID = ce.stu_id\r\nWHERE ce.class_id = @class_id";
                

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@class_id", class_id);
                        
                        cmd.CommandTimeout = 30;

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            int Stu_ID_Show = 1;
                            while (reader.Read())
                            {

                                Student_Info student_in_class = new Student_Info
                                {
                                    ID = reader.GetInt32("ID"),
                                    Stu_ID = reader.GetString("stu_id"),
                                    Stu_FirstName_KH = reader.GetString("stu_firstname_kh"),
                                    Stu_FirstName_EN = reader.GetString("stu_firstname_en"),
                                    Stu_LastName_KH = reader.GetString("stu_lastname_kh"),
                                    Stu_LastName_EN = reader.GetString("stu_lastname_en"),
                                    Stu_BirthdayDateOnly = reader.GetString("stu_birthday_dateonly"),
                                    Stu_Gender = reader.GetString("stu_gender"),
                                    Stu_PhoneNumber = reader.GetString("stu_phone_number"),
                                    Stu_StatePoor = reader.GetString("stu_state_poor"),
                                    Stu_IDShow = Stu_ID_Show.ToString()
                                };

                                // Stu_Image
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_source")))
                                {
                                    // First, get the size of the image byte array from the database
                                    long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, null, 0, 0);

                                    if (byteSize > 0)
                                    {
                                        // Initialize the byte array with the correct size
                                        byte[] imageBytes = new byte[byteSize];

                                        // Now, read the image data into the byte array
                                        reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, imageBytes, 0, (int)byteSize);

                                        // If the image byte array is not empty, process it
                                        if (imageBytes != null && imageBytes.Length > 0)
                                        {
                                            student_in_class.ProfileImageBytes = imageBytes;  // Store the image bytes for future use
                                            student_in_class.Stu_Image_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage
                                        }
                                        else
                                        {
                                            Debug.WriteLine("No image data found for student " + student_in_class.Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte size is 0 for student " + student_in_class.Stu_ID);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("stu_image_source is NULL for student " + student_in_class.Stu_ID);
                                }
                                Stu_ID_Show++;
                                student_class_show.Add(student_in_class);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Select Class for show students error: {ex.ToString()}");
            }
            return student_class_show;
        }


        //Method for select count total student in class
        public int GetTotalStudentsInClass(string class_id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query_select_total_stu = "SELECT COUNT(*) AS student_total_count FROM class_enrollments WHERE class_id = @class_id";

                    using (var cmd = new MySqlCommand(query_select_total_stu, connection))
                    {
                        cmd.Parameters.AddWithValue("@class_id", class_id);

                        // Execute the query and retrieve the count
                        object result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int total_students))
                        {
                            return total_students; // Return the total count
                        }
                        else
                        {
                            return 0; // Return 0 if no result is found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving total students in class: {ex.Message}");
                return -1; // Return -1 in case of an error
            }
        }

        //Method for Insert Multi Student to Class Mysql
        public bool Insert_Students_to_Class(List<int> students_id, string class_id, int max_student_class)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Use a single query to insert multiple rows
                    using (var transaction = connection.BeginTransaction())
                    {
                        string query_insert_stu = "INSERT INTO class_enrollments (stu_id, class_id) VALUES (@stu_id, @class_id)";
                        string query_update_classes = "UPDATE classes SET Class_Total_Student_Set = @Class_Total_Student_Set WHERE class_id = @class_id";
                        string query_update_student = "UPDATE student_infomations SET stu_classes = @stu_classes WHERE ID = @students_id";
                        //string query_select_total_stu = "SELECT COUNT(*) AS student_total_count FROM class_enrollments WHERE class_id = @class_id";
                        int totalStudents = GetTotalStudentsInClass(class_id);
                        //Insert Student to Class
                        using (var cmd = new MySqlCommand(query_insert_stu, connection, transaction))
                        {
                            // Reuse the command for each student
                            cmd.Parameters.Add("@stu_id", MySqlDbType.Int32);
                            cmd.Parameters.Add("@class_id", MySqlDbType.VarChar);
                            cmd.Parameters.Add("@total_student_set", MySqlDbType.Int32);

                            foreach (var student_id in students_id)
                            {
                                cmd.Parameters["@stu_id"].Value = student_id;
                                cmd.Parameters["@class_id"].Value = class_id;
                                cmd.Parameters["@total_student_set"].Value = max_student_class;

                                cmd.ExecuteNonQuery();
                            }
                        }
                        
                        // Update the classes table
                        using (var cmd = new MySqlCommand(query_update_classes, connection, transaction))
                        {
                            cmd.Parameters.Add("@class_id", MySqlDbType.VarChar);
                            cmd.Parameters.Add("@Class_Total_Student_Set", MySqlDbType.Int32);
                            

                            cmd.Parameters["@class_id"].Value = class_id;
                            cmd.Parameters["@Class_Total_Student_Set"].Value = max_student_class;
                            

                            cmd.ExecuteNonQuery();
                        }

                        // Update the student_infomations table
                        using (var cmd = new MySqlCommand(query_update_student, connection, transaction))
                        {
                            cmd.Parameters.Add("@students_id", MySqlDbType.Int32);
                            cmd.Parameters.Add("@stu_classes", MySqlDbType.VarChar);

                            string stu_classes = "Yes";
                            foreach (var student_id in students_id)
                            {
                                cmd.Parameters["@students_id"].Value = student_id;
                                cmd.Parameters["@stu_classes"].Value = stu_classes;

                                cmd.ExecuteNonQuery();
                            }
                        }
                        // Commit the transaction after all inserts
                        transaction.Commit();
                    }
                }

                return true; // Return true if all inserts succeed
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inserting students into the class: {ex.Message}");
                return false; // Return false in case of an error
            }
        }

        //Method for delete multi student in class
        public bool Delete_Students_in_Class(List<int> students_id, string class_id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Use a single query to insert multiple rows
                    using (var transaction = connection.BeginTransaction())
                    {
                        string query_delete_stu_in_class = "DELETE FROM class_enrollments WHERE stu_id = @stu_id AND class_id = @class_id";
                        string query_update_student_class = "UPDATE student_infomations SET stu_classes = @stu_classes WHERE ID = @students_id";
                        //string query_update_count_stu = "UPDATE classes SET Class_Real_Student = @student_select_count WHERE class_id = @class_id";

                        int totalStudents = GetTotalStudentsInClass(class_id);

                        using (var cmd = new MySqlCommand(query_delete_stu_in_class, connection, transaction))
                        {
                            // Reuse the command for each student
                            cmd.Parameters.Add("@stu_id", MySqlDbType.Int32);
                            cmd.Parameters.Add("@class_id", MySqlDbType.VarChar);
                            

                            foreach (var student_id in students_id)
                            {
                                cmd.Parameters["@stu_id"].Value = student_id;
                                cmd.Parameters["@class_id"].Value = class_id;


                                // Execute the query
                                int rowsAffected = cmd.ExecuteNonQuery();

                                // If no rows were affected for any student, rollback the transaction 
                                if (rowsAffected == 0)
                                {
                                    Debug.WriteLine($"Failed to delete: No matching record for stu_id = {student_id}, class_id = {class_id}");
                                    transaction.Rollback();
                                    return false;
                                }
                            }
                        }

                        // Update the student_infomations table
                        using (var cmd = new MySqlCommand(query_update_student_class, connection, transaction))
                        {
                            cmd.Parameters.Add("@students_id", MySqlDbType.Int32);
                            cmd.Parameters.Add("@stu_classes", MySqlDbType.VarChar);

                            
                            foreach (var student_id in students_id)
                            {
                                cmd.Parameters["@students_id"].Value = student_id;
                                cmd.Parameters["@stu_classes"].Value = DBNull.Value;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        
                        // Commit the transaction if all deletions succeed
                        transaction.Commit();
                        return true;
                    }
                }
               
            }catch (Exception ex)
            {
                Debug.WriteLine($"Delete student in class error: {ex.ToString()}");
                return false;
            }
        }

        //Method Update total count student count
        public void UpdateStudentSelectCount(string class_id, int student_select_count)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE classes SET Class_Real_Student = @student_select_count WHERE class_id = @class_id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@student_select_count", student_select_count);
                        cmd.Parameters.AddWithValue("@class_id", class_id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating student select count: {ex.Message}");
            }
        }
        
        //Method to Insert Teacher
        public bool Insert_TeacherInfomations(Teacher_Informatioins teacher_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO teacher_informatin(Techer_ID,Techer_Name_KH,Techer_Name_EN,Techer_Phone) " +
                        "VALUES(@Techer_ID,@Techer_Name_KH,@Techer_Name_EN,@Techer_Phone)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //cmd.Parameters.AddWithValue("@id", "ID");
                    cmd.Parameters.AddWithValue("@Techer_ID", teacher_info.Teacher_ID);
                    cmd.Parameters.AddWithValue("@Techer_Name_KH", teacher_info.Teacher_Name_KH);
                    cmd.Parameters.AddWithValue("@Techer_Name_EN", teacher_info.Teacher_Name_EN);
                    cmd.Parameters.AddWithValue("@Techer_Phone", teacher_info.Teacher_Phone);


                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Insert Teacher Info Error: {ex.ToString()}");
                return false;
            }
        }
       
        //Check Skill Schedule Sat Sun Before Insert
        public async Task<(string SD_Skill_Name_Sat1_C,string SD_Skill_Name_Sat2_C,string SD_Skill_Name_Sun1_C, string SD_Skill_Name_Sun2_C)> GetSkill_Schedule_Info_Check(string SD_Class_Name)
        {
            try
            {
                const string query_check = "SELECT * FROM class_schedule_sat_sun WHERE sd_class_name = @SD_Class_Name";

                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query_check, conn))
                    {
                        //cmd.Parameters.AddWithValue("@SD_Skill_Name_Sat1", SD_Skill_Name_Sat1);
                        //cmd.Parameters.AddWithValue("@SD_Skill_Name_Sat2", SD_Skill_Name_Sat2);
                        //cmd.Parameters.AddWithValue("@SD_Skill_Name_Sun1", SD_Skill_Name_Sun1);
                        //cmd.Parameters.AddWithValue("@SD_Skill_Name_Sun2", SD_Skill_Name_Sun2);
                        cmd.Parameters.AddWithValue("@SD_Class_Name", SD_Class_Name);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (await reader.ReadAsync())
                            {
                                return (

                                    SD_Skill_Name_Sat1_C: reader["sd_skill_name_sat1"].ToString(),
                                    //SD_Teacher_Sat1_C: reader["sd_teacher_name_sat1"].ToString(),
                                    //SD_TotalTime_Sat1_C: reader.GetInt32("sd_totaltime_skill_sat1"),

                                    SD_Skill_Name_Sat2_C: reader["sd_skill_name_sat2"].ToString(),
                                    SD_Skill_Name_Sun1_C: reader["sd_skill_name_sun1"].ToString(),
                                    SD_Skill_Name_Sun2_C: reader["sd_skill_name_sun2"].ToString()
                                );
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database check skill schedule before insert update: {ex.Message}");            
            }
            return (null,null, null, null);
        }

        //Check Teacher info before insert
        public async Task<(string Teacher_Name_KH1,string Teacher_Name_EN1,string Teacher_Phone1)> GetTeacher_Info_Check(string Teacher_Name_KH, string Teacher_Name_EN, string Teacher_Phone)
        {
            try
            {
                const string query_check = "SELECT Techer_Name_KH,Techer_Name_EN,Techer_Phone FROM teacher_informatin WHERE Techer_Name_KH = @Techer_Name_KH AND Techer_Name_EN = @Techer_Name_EN AND Techer_Phone = @Techer_Phone";

                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query_check, conn))
                    {
                        cmd.Parameters.AddWithValue("@Techer_Name_KH",Teacher_Name_KH);
                        cmd.Parameters.AddWithValue("@Techer_Name_EN",Teacher_Name_EN);
                        cmd.Parameters.AddWithValue("@Techer_Phone",Teacher_Phone);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (await reader.ReadAsync())
                            {
                                return (
                                    
                                    Teacher_Name_KH1: reader["Techer_Name_KH"].ToString(),
                                    Teacher_Name_EN1: reader["Techer_Name_EN"].ToString(),
                                    Teacher_Phone1: reader["Techer_Phone"].ToString()
                                   
                                );
                            }
                        }
                    }
                }    
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Check Teacher before Insert Error: {ex.ToString()}");
                
            }
            // Return default values if no match is found
            return (null, null, null);
        }

        //Method Check Student Before Insert
        public async Task<(string Stu_FirstName_KH1, string Stu_LastName_KH1, string Stu_Gender1, string Stu_BirthdayDateOnly1, string Stu_EducationType1, string Stu_StudyYear1)> GetStudents_Check_Student_Info(string Stu_FirstName_KH, string Stu_LastName_KH, string Stu_Gender, string Stu_BirthdayDateOnly, string Stu_EducationType, string Stu_StudyYear)
        {
            const string query = "SELECT stu_firstname_kh, stu_lastname_kh, stu_gender, stu_birthday_dateonly, stu_education_types, stu_study_year " +
                             "FROM student_infomations WHERE stu_firstname_kh = @Stu_FirstName_KH && stu_lastname_kh = @Stu_LastName_KH && stu_gender = @Stu_Gender && stu_birthday_dateonly = @Stu_BirthdayDateOnly && stu_education_types = @Stu_EducationType && stu_study_year = @Stu_StudyYear";

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Stu_FirstName_KH", Stu_FirstName_KH);
                    cmd.Parameters.AddWithValue("@Stu_LastName_KH", Stu_LastName_KH);
                    cmd.Parameters.AddWithValue("@Stu_Gender", Stu_Gender);
                    cmd.Parameters.AddWithValue("@Stu_BirthdayDateOnly", Stu_BirthdayDateOnly);
                    cmd.Parameters.AddWithValue("@Stu_EducationType", Stu_EducationType);
                    cmd.Parameters.AddWithValue("@Stu_StudyYear", Stu_StudyYear);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (await reader.ReadAsync())
                        {
                            return (
                                Stu_FirstName_KH1: reader["stu_firstname_kh"].ToString(),
                                Stu_LastName_KH1: reader["stu_lastname_kh"].ToString(),
                                Stu_Gender1: reader["stu_gender"].ToString(),
                                Stu_BirthdayDateOnly1: reader["stu_birthday_dateonly"].ToString(),
                                Stu_EducationType1: reader["stu_education_types"].ToString(),
                                Stu_StudyYear1: reader["stu_study_year"].ToString()
                            );
                        }
                    }

                }
            }
            return (null,null,null,null,null,null);
        }

        //Method Check Class Before Insert
        public async Task<(string Class_In_Skill1, string Class_In_Study_Year1, string Class_In_Level1, string Class_In_Student_Year1, string Class_In_Semester1, string Class_In_Generation1, string Class_In_Study_Timeshift1, string Class_In_Study_Type1)>GetClasses_Check_Info(string Class_In_Skill, string Class_In_Study_Year, string Class_In_Level, string Class_In_Student_Year, string Class_In_Semester, string Class_In_Generation, string Class_In_Study_Timeshift, string Class_In_Study_Type)
        {
            const string query = "SELECT * FROM classes WHERE class_in_skill = @Class_In_Skill AND class_in_level = @Class_In_Level AND class_in_study_year = @Class_In_Study_Year AND class_in_student_year = @Class_In_Student_Year AND class_in_semester = @Class_In_Semester AND class_in_generation = @Class_In_Generation AND class_in_study_timeshift = @Class_In_Study_Timeshift AND class_in_study_type = @Class_In_Study_Type";
 
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@Class_In_Skill", Class_In_Skill);
                        cmd.Parameters.AddWithValue("@Class_In_Study_Year", Class_In_Study_Year);
                        cmd.Parameters.AddWithValue("@Class_In_Level", Class_In_Level);
                        cmd.Parameters.AddWithValue("@Class_In_Student_Year", Class_In_Student_Year);
                        cmd.Parameters.AddWithValue("@Class_In_Semester", Class_In_Semester);
                        cmd.Parameters.AddWithValue("@Class_In_Generation", Class_In_Generation);
                        cmd.Parameters.AddWithValue("@Class_In_Study_Timeshift", Class_In_Study_Timeshift);
                        cmd.Parameters.AddWithValue("@Class_In_Study_Type", Class_In_Study_Type);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (await reader.ReadAsync())
                            {
                                return (
                                    Class_In_Skill1: reader["class_in_skill"].ToString(),
                                    Class_In_Study_Year1: reader["class_in_study_year"].ToString(),
                                    Class_In_Level1: reader["class_in_level"].ToString(),
                                    Class_In_Student_Year1: reader["class_in_student_year"].ToString(),
                                    Class_In_Semester1: reader["class_in_semester"].ToString(),
                                    Class_In_Generation1: reader["class_in_generation"].ToString(),
                                    Class_In_Study_Timeshift1: reader["class_in_study_timeshift"].ToString(),
                                    Class_In_Study_Type1: reader["class_in_study_type"].ToString()
                                );
                            }
                        }

                    }
                }
                return (null, null, null, null, null,null,null,null);     
        }

        //Method Fetch Teacher Infomation.
        public List<Teacher_Informatioins> GetFetchTeacher_Info(string search_text)
        {
            List<Teacher_Informatioins> teacher_info = new List<Teacher_Informatioins>();
            try
            {
                string query = string.IsNullOrEmpty(search_text)
                           ? "SELECT * FROM teacher_informatin ORDER BY Techer_ID DESC"
                           : "SELECT * FROM teacher_informatin WHERE Techer_ID LIKE @search_text || Techer_Name_KH LIKE @search_text || Techer_Name_EN LIKE @search_text || Techer_Phone LIKE @search_text ORDER BY Techer_ID DESC";
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrWhiteSpace(search_text))
                        {
                            cmd.Parameters.AddWithValue("@search_text", $"%{search_text}%");
                        }
                        cmd.CommandTimeout = 30;  // Set a timeout of 30 seconds
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Teacher_Informatioins teachers = new Teacher_Informatioins
                                {
                                    T_ID = reader.GetInt32("ID"),
                                    Teacher_ID = reader.GetString("Techer_ID"),
                                    Teacher_Name_KH = reader.GetString("Techer_Name_KH"),
                                    Teacher_Name_EN = reader.GetString("Techer_Name_EN"),
                                    Teacher_Phone = reader.GetString("Techer_Phone")
                                };
                                teacher_info.Add(teachers);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Error get teacher info: {ex.ToString()}");
                return null;
            }
            return teacher_info;
        }

        //Method update teacher
        public bool Update_Teacher_Info(Teacher_Informatioins teacher_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE teacher_informatin SET Techer_Name_KH =@Teacher_Name_KH,Techer_Name_EN = @Teacher_Name_EN,Techer_Phone = @Teacher_Phone WHERE Techer_ID = @Teacher_ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    {
                        cmd.Parameters.AddWithValue("@Teacher_ID", teacher_info.Teacher_ID);
                        cmd.Parameters.AddWithValue("@Teacher_Name_KH", teacher_info.Teacher_Name_KH);
                        cmd.Parameters.AddWithValue("@Teacher_Name_EN", teacher_info.Teacher_Name_EN);
                        cmd.Parameters.AddWithValue("@Teacher_Phone", teacher_info.Teacher_Phone);
                    }
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;  // Return true if rows were updated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Update Error: " + ex.ToString());
                return false;
            }
        }

        //Method Delete Curriculum
        public bool Delete_Curriculum_Info(string curriculum_id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM curriculum_info WHERE curriculum_id = @curriculum_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@curriculum_id", curriculum_id);
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Delete curriculum info error: {ex.ToString()}");
                return false;
            }
        }

        //Method Delete Teacher
        public bool Delete_Teacher_Info(string teacher_id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM teacher_informatin WHERE Techer_ID = @Teacher_ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@Teacher_ID", teacher_id);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Delete Teacher Error: " + ex.ToString());
                return false;
            }
        }

        //Method Get CurriculumID
        public (int C_ID, string Curriculum_ID) Get_CurriculumID()
        {
            int C_ID = 0;
            string Last_Curriculum_ID = "RPI_C0000";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT MAX(C_ID) as C_ID, MAX(curriculum_id) AS Last_Curriculum_ID FROM curriculum_info";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            C_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("C_ID");
                            Last_Curriculum_ID = reader.IsDBNull(1) ? "RPI_C0000" : reader.GetString("Last_Curriculum_ID");
                        }
                    }
                }
            }
            C_ID++;
            string Curriculum_ID = IncrementCurriculum_ID(Last_Curriculum_ID);
            return (C_ID, Curriculum_ID);
        }

        //Method to Increase Curriculum_ID
        public string IncrementCurriculum_ID(String currentCurriculumID)
        {
            // Assuming the format is always "RPI_C" + 4-digit number
            string prefix = "RPI_C";
            string numericPart = currentCurriculumID.Substring(5); // Extract the numeric part after "RPI_C"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 4-digit string with leading zeros
            string newNumericPart = number.ToString("D4");

            // Combine the prefix and the incremented numeric part
            string Curriculum_ID = prefix + newNumericPart;

            return Curriculum_ID;
        }

        //Method Get TeacherID
        public (int T_ID, string Teacher_ID) GetTeacher_ID()
        {
            int T_ID = 0;
            string Last_teacher_ID = "RPI_T0000";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT MAX(ID) AS T_ID, MAX(Techer_ID) AS Last_Teacher_ID FROM teacher_informatin";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            T_ID = reader.IsDBNull(0) ? 0 : reader.GetInt32("T_ID");
                            Last_teacher_ID = reader.IsDBNull(1) ? "RPI_T0000" : reader.GetString("Last_Teacher_ID");
                        }
                    }
                }
            }
            T_ID++;
            string Teacher_ID = IncrementTeacherID(Last_teacher_ID);
            return (T_ID, Teacher_ID);
        }

        //Method to Increase Teaacher_ID
        public string IncrementTeacherID(String currentTeacherID)
        {
            // Assuming the format is always "RPI" + 7-digit number
            string prefix = "RPI_T";
            string numericPart = currentTeacherID.Substring(5); // Extract the numeric part after "RPI_T"

            // Convert the numeric part to an integer, increment by 1
            int number = int.Parse(numericPart) + 1;

            // Reformat the number back to a 4-digit string with leading zeros
            string newNumericPart = number.ToString("D4");

            // Combine the prefix and the incremented numeric part
            string Teacher_ID = prefix + newNumericPart;

            return Teacher_ID;
        }

        //Method to fetch Curriculum TotalTime Table
        public List<Curriculum_Info> GetFetchCurriculum_TotalTime_Info(string Curriculum_Level_Name, string Curriculum_Skill_Name, string Curriculum_Search_Study_Year)
        {
            List<Curriculum_Info> curriculum_totaltime = new List<Curriculum_Info>();
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(_connectionString))
                    {
                        connection.Open();
                        //Select With Relationship
                        string query = string.IsNullOrEmpty(Curriculum_Search_Study_Year)
                            ? @"SELECT 
                                    ci.curr_study_year AS Year,
                                    ci.curr_semester AS Semester,
                                    ti.Techer_Name_KH AS Teacher_Name,
                                    SUM(ci.curr_total_time) AS Total_Time
                                FROM 
                                    curriculum_info ci
                                LEFT JOIN 
                                    teacher_informatin ti ON ci.curr_teacher_id = ti.ID
                                LEFT JOIN 
                                    education_skill_info si ON ci.curr_skill_id = si.ID -- Assuming the correct join column for skills
                                LEFT JOIN 
                                    education_level_info li ON ci.curr_level_id = li.ID -- Assuming the correct join column for levels
                                WHERE 
                                    si.edu_skill_name_kh = @Curriculum_Skill_Name
                                    AND li.edu_level_name_kh = @Curriculum_Level_Name                                  
                                GROUP BY 
                                    ci.curr_study_year, ci.curr_semester, ti.Techer_Name_KH
                                ORDER BY 
                                    CAST(ci.curr_study_year AS UNSIGNED) ASC,
                                    CAST(ci.curr_semester AS UNSIGNED) ASC"
                            : @"SELECT 
                                    ci.curr_study_year AS Year,
                                    ci.curr_semester AS Semester,
                                    ti.Techer_Name_KH AS Teacher_Name,
                                    SUM(ci.curr_total_time) AS Total_Time
                                FROM 
                                    curriculum_info ci
                                LEFT JOIN 
                                    teacher_informatin ti ON ci.curr_teacher_id = ti.ID
                                LEFT JOIN 
                                    education_skill_info si ON ci.curr_skill_id = si.ID -- Assuming the correct join column for skills
                                LEFT JOIN 
                                    education_level_info li ON ci.curr_level_id = li.ID -- Assuming the correct join column for levels
                                WHERE 
                                    si.edu_skill_name_kh = @Curriculum_Skill_Name
                                    AND li.edu_level_name_kh = @Curriculum_Level_Name
                                    AND ci.curr_study_year = @Curriculum_Search_Study_Year
                                GROUP BY 
                                    ci.curr_study_year, ci.curr_semester, ti.Techer_Name_KH
                                ORDER BY 
                                    CAST(ci.curr_study_year AS UNSIGNED) ASC,
                                    CAST(ci.curr_semester AS UNSIGNED) ASC";

                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Curriculum_Skill_Name", Curriculum_Skill_Name);
                            cmd.Parameters.AddWithValue("@Curriculum_Level_Name", Curriculum_Level_Name);
                            cmd.Parameters.AddWithValue("@Curriculum_Search_Study_Year", Curriculum_Search_Study_Year);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Curriculum_Info curriculum_list = new Curriculum_Info()
                                    {                    
                                        Curriculum_Teacher_Name = reader.GetString("Teacher_Name"),
                                        Curriculum_Study_Year = reader.GetString("Year"),
                                        Curriculum_Semester = reader.GetString("Semester"),
                                        Curriculum_Total_Time = reader.GetInt32("Total_Time"),                        
                                    };
                                    curriculum_totaltime.Add(curriculum_list);
                                }
                            }

                        }
                    }
                }
                catch(MySqlException ex)
                {
                    Debug.WriteLine($"Database error: {ex.Message}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Load Curriculum Teacher TotalTime Error: {e.ToString()}");                   
                }
                
            }
            return curriculum_totaltime;
        }

        //Method to fetch Curriculum Info Table
        public List<Curriculum_Info> GetFetchCurriculum_Table_Info(string Curriculum_Level_Name, string Curriculum_Skill_Name,string Curriculum_Search_Study_Year)
        {
            List<Curriculum_Info> curriculum_table = new List<Curriculum_Info>();
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(_connectionString))
                    {
                        connection.Open();

                        //Select With Relationship
                        string query = string.IsNullOrEmpty(Curriculum_Search_Study_Year)
                           ? @"SELECT ci.*,
                                      si.ID,
                                      si.edu_skill_name_kh,
                                      ti.ID,
                                      ti.Techer_Name_KH,
                                      li.ID,
                                      li.edu_level_name_kh
                                      FROM curriculum_info ci
                                           LEFT JOIN education_skill_info si ON ci.curr_skill_id = si.ID
                                           LEFT JOIN teacher_informatin ti ON ci.curr_teacher_id = ti.ID
                                           LEFT JOIN education_level_info li ON ci.curr_level_id = li.ID
                                           WHERE si.edu_skill_name_kh = @Curriculum_Skill_Name
                                                 AND li.edu_level_name_kh = @Curriculum_Level_Name
                                           ORDER BY CAST(ci.curr_study_year AS UNSIGNED) ASC,
                                                    CAST(ci.curr_semester AS UNSIGNED) ASC"    
                           : @"SELECT ci.*,
                                      si.ID,
                                      si.edu_skill_name_kh,
                                      ti.ID,
                                      ti.Techer_Name_KH,
                                      li.ID,
                                      li.edu_level_name_kh
                                      FROM curriculum_info ci
                                           LEFT JOIN education_skill_info si ON ci.curr_skill_id = si.ID
                                           LEFT JOIN teacher_informatin ti ON ci.curr_teacher_id = ti.ID
                                           LEFT JOIN education_level_info li ON ci.curr_level_id = li.ID
                                           WHERE si.edu_skill_name_kh = @Curriculum_Skill_Name
                                              AND li.edu_level_name_kh = @Curriculum_Level_Name
                                              AND ci.curr_study_year = @Curriculum_Search_Study_Year
                                           ORDER BY CAST(ci.curr_study_year AS UNSIGNED) ASC,
                                                    CAST(ci.curr_semester AS UNSIGNED) ASC";
                        
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Curriculum_Skill_Name", Curriculum_Skill_Name);
                            cmd.Parameters.AddWithValue("@Curriculum_Level_Name", Curriculum_Level_Name);
                            cmd.Parameters.AddWithValue("@Curriculum_Search_Study_Year", Curriculum_Search_Study_Year);
                            

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Curriculum_Info curriculum_list = new Curriculum_Info()
                                    {
                                        Curriculum_ID = reader.GetString("curriculum_id"),
                                        Curriculum_Name_KH = reader.GetString("curr_name_kh"),
                                        Curriculum_Name_EN = reader.GetString("curr_name_en"),
                                        Curriculum_Skill_ID = reader.GetInt32("curr_skill_id"),
                                        Curriculum_Skill_Name = reader.GetString("edu_skill_name_kh"),
                                        Curriculum_Teacher_ID = reader.GetInt32("curr_teacher_id"),
                                        Curriculum_Teacher_Name = reader.GetString("Techer_Name_KH"),
                                        Curriculum_Study_Year = reader.GetString("curr_study_year"),
                                        Curriculum_Semester = reader.GetString("curr_semester"),
                                        Curriculum_Total_Time = reader.GetInt32("curr_total_time"),
                                        Curriculum_Total_Score = reader.GetInt32("curr_total_score"),
                                        Curriculum_Level_ID = reader.GetInt32("curr_level_id"),
                                        Curriculum_Level_Name = reader.GetString("edu_level_name_kh"),
                                    };
                                    curriculum_table.Add(curriculum_list);
                                }
                            }
                        }
                    }
                }
                catch(MySqlException ex)
                {
                    Debug.WriteLine($"Database error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Load Curriculum Table Error: {ex.ToString()}");
                    
                }             
            }
            return curriculum_table;
        }

        //Method to fetch Curriculum Info
        public List<Curriculum_Info> GetFetchCurriculum_Info(string search_text)
        {
            Debug.WriteLine("Start Loading Curriculum Info List.");
            List<Curriculum_Info> curriculum_info = new List<Curriculum_Info>();
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(_connectionString))
                    {
                        connection.Open();

                        //Select With Relationship
                        string query = string.IsNullOrEmpty(search_text)
                           ? @"SELECT ci.*,
                                      si.ID,
                                      si.edu_skill_name_kh,
                                      ti.ID,
                                      ti.Techer_Name_KH,
                                      li.ID,
                                      li.edu_level_name_kh
                                      FROM curriculum_info ci
                                           LEFT JOIN education_skill_info si ON ci.curr_skill_id = si.ID
                                           LEFT JOIN teacher_informatin ti ON ci.curr_teacher_id = ti.ID
                                           LEFT JOIN education_level_info li ON ci.curr_level_id = li.ID
                                           ORDER BY ci.curriculum_id DESC"
                           : @"SELECT ci.*,
                                      si.ID,
                                      si.edu_skill_name_kh,
                                      ti.ID,
                                      ti.Techer_Name_KH,
                                      li.ID,
                                      li.edu_level_name_kh
                                      FROM curriculum_info ci
                                           LEFT JOIN education_skill_info si ON ci.curr_skill_id = si.ID
                                           LEFT JOIN teacher_informatin ti ON ci.curr_teacher_id = ti.ID
                                           LEFT JOIN education_level_info li ON ci.curr_level_id = li.ID
                                           WHERE ci.curriculum_id LIKE @search_text 
                                              OR ci.curr_name_kh LIKE @search_text 
                                              OR ci.curr_name_en LIKE @search_text 
                                              OR ci.curr_total_time LIKE @search_text 
                                              OR ci.curr_total_score LIKE @search_text 
                                              OR ti.Techer_Name_KH LIKE @search_text 
                                              OR si.edu_skill_name_kh LIKE @search_text
                                              OR li.edu_level_name_kh LIKE @search_text
                                           ORDER BY ci.curriculum_id DESC";
                           
                       

                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            if (!string.IsNullOrWhiteSpace(search_text))
                            {
                                cmd.Parameters.AddWithValue("@search_text", $"%{search_text}%");
                            }
                            cmd.CommandTimeout = 30;  // Set a timeout of 30 seconds

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Curriculum_Info curriculum_list = new Curriculum_Info()
                                    {                                 
                                        Curriculum_ID = reader.GetString("curriculum_id"),
                                        Curriculum_Name_KH = reader.GetString("curr_name_kh"),
                                        Curriculum_Name_EN = reader.GetString("curr_name_en"),
                                        Curriculum_Skill_ID = reader.GetInt32("curr_skill_id"),
                                        Curriculum_Skill_Name = reader.GetString("edu_skill_name_kh"),
                                        Curriculum_Teacher_ID = reader.GetInt32("curr_teacher_id"),
                                        Curriculum_Teacher_Name = reader.GetString("Techer_Name_KH"),
                                        Curriculum_Study_Year = reader.GetString("curr_study_year"),
                                        Curriculum_Semester = reader.GetString("curr_semester"),
                                        Curriculum_Total_Time = reader.GetInt32("curr_total_time"),
                                        Curriculum_Total_Score = reader.GetInt32("curr_total_score"),
                                        Curriculum_Level_ID = reader.GetInt32("curr_level_id"),
                                        Curriculum_Level_Name = reader.GetString("edu_level_name_kh"),
                                    };
                                    curriculum_info.Add(curriculum_list);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Loading Curriculum info list Error: {e.Message}");
                    return null;
                }
                return curriculum_info;
            }
        }

        //Method to fetch Teacher Info to Combobox
        public List<Curriculum_Info> GetTeacherInfo_List_Curriculum()
        {
            Debug.WriteLine("Start Loading Teacher Info List.");
            List<Curriculum_Info> teacher_info = new List<Curriculum_Info>();
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(_connectionString))
                    {
                        connection.Open();

                        string query = "SELECT ID,Techer_Name_KH FROM teacher_informatin ORDER BY " +
                            "CASE " +
                            "WHEN Techer_Name_KH REGEXP '[ក-អ]' THEN 1 " +
                            "WHEN Techer_Name_KH REGEXP '^[អឥឦឧឩឪឫឬឭឮឯឰឱឲ]' THEN 2 " +
                            "ELSE 3 " +
                            "END," +
                            "Techer_Name_KH ASC";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Curriculum_Info curriculum_teacher = new Curriculum_Info()
                                    {
                                        Curriculum_Teacher_ID = reader.GetInt32("ID"),
                                        Curriculum_Teacher_Name = reader.GetString("Techer_Name_KH"),
                                    };
                                    teacher_info.Add(curriculum_teacher);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Load Teacher Info Error: {ex.ToString()}");
                    return null;
                }

            }
            return teacher_info;
        }

        //Method to fetch Skill Info to Combobox
        public List<Curriculum_Info> GetSkillInfo_List_Curriculum()
        {
            Debug.WriteLine("Start Loading Skill Info in Curriculum.");
            List<Curriculum_Info>skill_info_curriculum = new List<Curriculum_Info>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID,edu_skill_name_kh FROM education_skill_info ORDER BY " +
                            "CASE " +
                            "WHEN edu_skill_name_kh REGEXP '[ក-អ]' THEN 1 " +
                            "WHEN edu_skill_name_kh REGEXP '^[អឥឦឧឩឪឫឬឭឮឯឰឱឲ]' THEN 2 " +
                            "ELSE 3 " +
                            "END," +
                            "edu_skill_name_kh ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Curriculum_Info curriculum_skill = new Curriculum_Info()
                                {
                                    Curriculum_Skill_ID = reader.GetInt32("ID"),
                                    Curriculum_Skill_Name = reader.GetString("edu_skill_name_kh"),
                                };
                                skill_info_curriculum.Add(curriculum_skill);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Load Skill Info Curriculum Error: {ex.ToString()}");
                return null;
            }
            return skill_info_curriculum;
        }

        //Method to fetck Level Info to Combobox
        public List<Curriculum_Info> GetLevelInfo_List_Curriculum()
        {
            Debug.WriteLine("Start Loading level Info in Curriculum.");
            List<Curriculum_Info> level_curriculum_info = new List<Curriculum_Info>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID,edu_level_name_kh FROM education_level_info ORDER BY " +
                            "CASE " +
                            "WHEN edu_level_name_kh REGEXP '[ក-អ]' THEN 1 " +
                            "WHEN edu_level_name_kh REGEXP '^[អឥឦឧឩឪឫឬឭឮឯឰឱឲ]' THEN 2 " +
                            "ELSE 3 " +
                            "END," +
                            "edu_level_name_kh ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Curriculum_Info curriculum_level = new Curriculum_Info()
                                {
                                    Curriculum_Level_ID = reader.GetInt32("ID"),
                                    Curriculum_Level_Name = reader.GetString("edu_level_name_kh"),
                                };
                                level_curriculum_info.Add(curriculum_level);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Loading level curriculum error: {ex.ToString()}");
                return null;
            }
            return level_curriculum_info;
        }

        //Method to Insert Curriculum
        public bool Insert_CurriculumInfomation(Curriculum_Info curriculum_Info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO curriculum_info(curriculum_id,curr_name_kh,curr_name_en,curr_skill_id,curr_teacher_id,curr_study_year,curr_semester,curr_total_time,curr_total_score,curr_level_id) " +
                        "VALUES(@Curriculum_ID,@Curriculum_Name_KH,@Curriculum_Name_EN,@Curriculum_Skill_ID,@Curriculum_Teacher_ID,@Curriculum_Study_Year,@Curriculum_Semester,@Curriculum_Total_Time,@Curriculum_Total_Score,@Curriculum_Level_ID) ";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@Curriculum_ID", curriculum_Info.Curriculum_ID);
                    cmd.Parameters.AddWithValue("@Curriculum_Name_KH", curriculum_Info.Curriculum_Name_KH);
                    cmd.Parameters.AddWithValue("@Curriculum_Name_EN", curriculum_Info.Curriculum_Name_EN);
                    cmd.Parameters.AddWithValue("@Curriculum_Skill_ID", curriculum_Info.Curriculum_Skill_ID);
                    cmd.Parameters.AddWithValue("@Curriculum_Teacher_ID", curriculum_Info.Curriculum_Teacher_ID);
                    cmd.Parameters.AddWithValue("@Curriculum_Study_Year", curriculum_Info.Curriculum_Study_Year);
                    cmd.Parameters.AddWithValue("@Curriculum_Semester", curriculum_Info.Curriculum_Semester);
                    cmd.Parameters.AddWithValue("@Curriculum_Total_Time", curriculum_Info.Curriculum_Total_Time);
                    cmd.Parameters.AddWithValue("@Curriculum_Total_Score", curriculum_Info.Curriculum_Total_Score);
                    cmd.Parameters.AddWithValue("@Curriculum_Level_ID", curriculum_Info.Curriculum_Level_ID);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Insert Curriculum Error: {ex.ToString()}");
                return false;
            }
        }

        //Method update curriculum
        public bool Update_Curriculum_Info(Curriculum_Info curriculum_Info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE curriculum_info SET curr_name_kh = @curr_name_kh,curr_name_en = @curr_name_en,curr_study_year = @curr_study_year,curr_semester = @curr_semester,curr_skill_id = @curr_skill_id,curr_teacher_id = @curr_teacher_id,curr_total_time = @curr_total_time,curr_total_score = @curr_total_score,curr_level_id = @curr_level_id WHERE curriculum_id = @curriculum_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    {
                        cmd.Parameters.AddWithValue("@curriculum_id", curriculum_Info.Curriculum_ID);
                        cmd.Parameters.AddWithValue("@curr_name_kh", curriculum_Info.Curriculum_Name_KH);
                        cmd.Parameters.AddWithValue("@curr_name_en", curriculum_Info.Curriculum_Name_EN);
                        cmd.Parameters.AddWithValue("@curr_study_year", curriculum_Info.Curriculum_Study_Year);
                        cmd.Parameters.AddWithValue("@curr_semester", curriculum_Info.Curriculum_Semester);
                        cmd.Parameters.AddWithValue("@curr_skill_id", curriculum_Info.Curriculum_Skill_ID);
                        cmd.Parameters.AddWithValue("@curr_teacher_id", curriculum_Info.Curriculum_Teacher_ID);
                        cmd.Parameters.AddWithValue("@curr_total_time", curriculum_Info.Curriculum_Total_Time);
                        cmd.Parameters.AddWithValue("@curr_total_score", curriculum_Info.Curriculum_Total_Score);
                        cmd.Parameters.AddWithValue("@curr_level_id", curriculum_Info.Curriculum_Level_ID);

                    }
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Update curriculum error: {ex.ToString()}");
                return false;
            }
        }

        //Check Curriculum Info Before Insert
        public async Task<(string Curriculum_Name_KH1, string Curriculum_Name_EN1, int Curriculum_Skill_ID1, int Curriculum_Teacher_ID1, string Curriculum_Study_Year1, string Curriculum_Semester1, int Curriculum_Total_Time1, int Curriculum_Total_Score1,int Curriculum_Level_ID1)> GetCurriculum_Info_Check(string Curriculum_Name_KH, string Curriculum_Name_EN, int Curriculum_Skill_ID, int Curriculum_Teacher_ID, string Curriculum_Study_Year, string Curriculum_Semester, int Curriculum_Total_Time, int Curriculum_Total_Score, int Curriculum_Level_ID)
        {
            try
            {
                const string query_check = "SELECT curr_name_kh,curr_name_en,curr_skill_id,curr_teacher_id,curr_study_year,curr_semester,curr_total_time,curr_total_score,curr_level_id FROM curriculum_info WHERE curr_name_kh = @curr_name_kh AND curr_name_en = @curr_name_en AND curr_skill_id = @curr_skill_id AND curr_teacher_id = @curr_teacher_id AND curr_study_year = @curr_study_year AND curr_semester = @curr_semester AND curr_total_time = @curr_total_time AND curr_total_score =@curr_total_score AND curr_level_id = @curr_level_id";

                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(query_check, conn))
                    {
                        cmd.Parameters.AddWithValue("@curr_name_kh", Curriculum_Name_KH);
                        cmd.Parameters.AddWithValue("@curr_name_en", Curriculum_Name_EN);
                        cmd.Parameters.AddWithValue("@curr_skill_id", Curriculum_Skill_ID);
                        cmd.Parameters.AddWithValue("@curr_level_id", Curriculum_Level_ID);
                        cmd.Parameters.AddWithValue("@curr_teacher_id", Curriculum_Teacher_ID);
                        cmd.Parameters.AddWithValue("@curr_study_year", Curriculum_Study_Year);
                        cmd.Parameters.AddWithValue("@curr_semester", Curriculum_Semester);
                        cmd.Parameters.AddWithValue("@curr_total_time", Curriculum_Total_Time);
                        cmd.Parameters.AddWithValue("@curr_total_score", Curriculum_Total_Score);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (await reader.ReadAsync())
                            {
                                return (

                                    Curriculum_Name_KH1: reader["curr_name_kh"].ToString(),
                                    Curriculum_Name_EN1: reader["curr_name_en"].ToString(),
                                    Curriculum_Skill_ID1: Convert.ToInt32(reader["curr_skill_id"]),                                
                                    Curriculum_Teacher_ID1: Convert.ToInt32(reader["curr_teacher_id"]),
                                    Curriculum_Study_Year1: reader["curr_study_year"].ToString(),
                                    Curriculum_Semester1: reader["curr_semester"].ToString(),
                                    Curriculum_Total_Time1: Convert.ToInt32(reader["curr_total_time"]),
                                    Curriculum_Total_Score1: Convert.ToInt32(reader["curr_total_score"]),
                                    Curriculum_Level_ID1: Convert.ToInt32(reader["curr_level_id"])

                                );
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Check curriculum info before insert error: {ex.ToString()}");
            }
            // Return default values if no match is found
            return (null, null, 0, 0, null, null, 0, 0, 0);
        }

        //Load Data Combobox Skill in Schedule List
        int skill_id;
        int level_id;
        public List<Class_Schedule> GetSkill_toCombobox_Class_Schedule(string Class_In_Skill,string Class_In_Level, string Class_In_Student_Year,string Class_In_Semester)
        {
            
            try
            {
                string query_skill = "SELECT ID FROM education_skill_info WHERE edu_skill_name_kh = @edu_skill_name_kh";
                using (MySqlConnection con = new MySqlConnection(_connectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query_skill, con);
                    cmd.Parameters.AddWithValue("@edu_skill_name_kh", Class_In_Skill);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            skill_id = reader.GetInt32("ID");
                        }                   
                    }
                }             
            }
            catch(MySqlException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //Level ID
            try
            {
                string query_level = "SELECT ID FROM education_level_info WHERE edu_level_name_kh = @edu_level_name_kh";

                using(MySqlConnection con = new MySqlConnection(_connectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query_level, con);
                    cmd.Parameters.AddWithValue("@edu_level_name_kh", Class_In_Level);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            level_id = reader.GetInt32("ID");
                        }

                    }
                }
            }
            catch(MySqlException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Debug.WriteLine($"level id: {level_id}");
            Debug.WriteLine($"skill id: {skill_id}");

            List<Class_Schedule> skill_schedule = new List<Class_Schedule>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                                SELECT C_ID, curr_name_kh, curr_name_en 
                                FROM curriculum_info 
                                WHERE curr_skill_id = @curr_skill_id 
                                  AND curr_level_id = @curr_level_id 
                                  AND curr_study_year = @curr_study_year 
                                  AND curr_semester = @curr_semester 
                                ORDER BY 
                                    CASE 
                                        WHEN curr_name_kh REGEXP '[ក-អ]' THEN 1 
                                        WHEN curr_name_kh REGEXP '^[អឥឦឧឩឪឫឬឭឮឯឰឱឲ]' THEN 2 
                                        ELSE 3 
                                    END, 
                                    curr_name_kh ASC;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@curr_skill_id", skill_id);
                        command.Parameters.AddWithValue("@curr_level_id", level_id);
                        command.Parameters.AddWithValue("@curr_study_year", Class_In_Student_Year);
                        command.Parameters.AddWithValue("@curr_semester", Class_In_Semester);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                skill_schedule.Add(new Class_Schedule
                                {
                                    SD_Skill_ID = reader.GetInt32("C_ID"),
                                    SD_Skill_Name = reader.GetString("curr_name_kh")
                                });
                            }
                        }
                    }     
                }              
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
            return skill_schedule;
        }
        
        //Load Data Combobox Teacher in Schedule List
        public List<Class_Schedule> GetTeacher_toCombobox_Class_Schedule()
        {
            List<Class_Schedule> teacher_schedule = new List<Class_Schedule>();
            try
            {
                
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                                SELECT ID, Techer_Name_KH, Techer_Name_EN 
                                FROM teacher_informatin                                
                                ORDER BY 
                                    CASE 
                                        WHEN Techer_Name_KH REGEXP '[ក-អ]' THEN 1 
                                        WHEN Techer_Name_KH REGEXP '^[អឥឦឧឩឪឫឬឭឮឯឰឱឲ]' THEN 2 
                                        ELSE 3 
                                    END, 
                                    Techer_Name_KH ASC;";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                teacher_schedule.Add(new Class_Schedule
                                {
                                    SD_Teacher_ID = reader.GetInt32("ID"),
                                    SD_Teacher_Name = reader.GetString("Techer_Name_KH")
                                });
                            }
                        }
                    }
                }
            }catch(MySqlException ex)
            {
                Debug.WriteLine($"Database error: {ex.Message}");
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            return teacher_schedule;
        }       

        //Load Teacher Name, Total Time Selected Skill
        public async Task<(string teacher_name, int total_Time1)> GetTeacher_Time_SelecetedSkill(string skill_name)
        {
            try
            {
                string query = "SELECT " +
                       "ci.curr_total_time AS TotalTime, " +
                       "ti.Techer_Name_KH AS Teacher_Name " +
                       "FROM curriculum_info ci " +
                       "LEFT JOIN teacher_informatin ti ON ci.curr_teacher_id = ti.ID " +
                       "WHERE ci.curr_name_kh = @curr_name_kh;";
                using (MySqlConnection con = new MySqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@curr_name_kh", skill_name);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (await reader.ReadAsync())
                            {
                                return (

                                    teacher_name: reader["Teacher_Name"].ToString(),
                                    total_Time1: Convert.ToInt32(reader["TotalTime"])                                 
                                );
                            }
                        }
                    }
                }
            }
            catch(MySqlException ex)
            {
                Debug.WriteLine($"Database error get teacher and time: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            return (null, 0);
        }

        //Method Insert class schedule
        public bool Save_ScheduleInfo(Class_Schedule class_Schedule)
        {
            try
            {
                using(MySqlConnection con = new MySqlConnection(_connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO class_schedule_mon_fri_info(sd_class_id, sd_class_name, sd_class_timeshift, sd_start_time_1, sd_end_time_1, sd_start_time_2, sd_end_time_2, sd_mon_skill_1, sd_name_teacher_mon_1, sd_totaltime_skill_mon_1, sd_mon_skill_2, sd_name_teacher_mon_2, sd_totaltime_skill_mon_2, sd_tues_skill_1, sd_name_teacher_tues_1, sd_totaltime_skill_tues_1, sd_tues_skill_2, sd_name_teacher_tues_2, sd_totaltime_skill_tues_2, sd_wed_skill_1, sd_name_teacher_wed_1, sd_totaltime_skill_wed_1, sd_wed_skill_2, sd_name_teacher_wed_2, sd_totaltime_skill_wed_2, sd_thurs_skill_1, sd_name_teacher_thurs_1, sd_totaltime_skill_thurs_1, sd_thurs_skill_2, sd_name_teacher_thurs_2, sd_totaltime_skill_thurs_2, sd_fri_skill_1, sd_name_teacher_fri_1, sd_totaltime_skill_fri_1, sd_fri_skill_2, sd_name_teacher_fri_2, sd_totaltime_skill_fri_2, sd_datetime_start_schedule, sd_building_name, sd_building_room, sd_datetime_create, sd_user_create)" +
                        "VALUES(@sd_class_id, @sd_class_name, @sd_class_timeshift, @sd_start_time_1, @sd_end_time_1, @sd_start_time_2, @sd_end_time_2, @sd_mon_skill_1, @sd_name_teacher_mon_1, @sd_totaltime_skill_mon_1, @sd_mon_skill_2, @sd_name_teacher_mon_2, @sd_totaltime_skill_mon_2, @sd_tues_skill_1, @sd_name_teacher_tues_1, @sd_totaltime_skill_tues_1, @sd_tues_skill_2, @sd_name_teacher_tues_2, @sd_totaltime_skill_tues_2, @sd_wed_skill_1, @sd_name_teacher_wed_1, @sd_totaltime_skill_wed_1, @sd_wed_skill_2, @sd_name_teacher_wed_2, @sd_totaltime_skill_wed_2, @sd_thurs_skill_1, @sd_name_teacher_thurs_1, @sd_totaltime_skill_thurs_1, @sd_thurs_skill_2, @sd_name_teacher_thurs_2, @sd_totaltime_skill_thurs_2, @sd_fri_skill_1, @sd_name_teacher_fri_1, @sd_totaltime_skill_fri_1, @sd_fri_skill_2, @sd_name_teacher_fri_2, @sd_totaltime_skill_fri_2, @sd_datetime_start_schedule, @sd_building_name, @sd_building_room, @sd_datetime_create, @sd_user_create)";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@sd_class_id", class_Schedule.Class_ID_Schedule);
                        cmd.Parameters.AddWithValue("@sd_class_name", class_Schedule.SD_Class_Name);
                        cmd.Parameters.AddWithValue("@sd_class_timeshift",class_Schedule.SD_Class_TimeShift);
                        cmd.Parameters.AddWithValue("@sd_start_time_1", class_Schedule.SD_Start_DateTime_MF1);
                        cmd.Parameters.AddWithValue("@sd_end_time_1", class_Schedule.SD_End_DateTime_MF1);
                        cmd.Parameters.AddWithValue("@sd_start_time_2", class_Schedule.SD_Start_DateTime_MF2);
                        cmd.Parameters.AddWithValue("@sd_end_time_2", class_Schedule.SD_End_DateTime_MF2);

                        cmd.Parameters.AddWithValue("@sd_mon_skill_1", class_Schedule.SD_Skill_Name_Mon1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_mon_1", class_Schedule.SD_Teacher_Mon01);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_mon_1", class_Schedule.SD_TotalTime_Mon1);
                        cmd.Parameters.AddWithValue("@sd_mon_skill_2", class_Schedule.SD_Skill_Name_Mon2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_mon_2", class_Schedule.SD_Teacher_Mon02);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_mon_2", class_Schedule.SD_TotalTime_Mon2);

                        cmd.Parameters.AddWithValue("@sd_tues_skill_1", class_Schedule.SD_Skill_Name_Tues1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_tues_1", class_Schedule.SD_Teacher_Tues01);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_tues_1", class_Schedule.SD_TotalTime_Tues1);
                        cmd.Parameters.AddWithValue("@sd_tues_skill_2", class_Schedule.SD_Skill_Name_Tues2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_tues_2", class_Schedule.SD_Teacher_Tues02);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_tues_2", class_Schedule.SD_TotalTime_Tues2);

                        cmd.Parameters.AddWithValue("@sd_wed_skill_1", class_Schedule.SD_Skill_Name_Wed1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_wed_1", class_Schedule.SD_Teacher_Wed1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_wed_1", class_Schedule.SD_TotalTime_Wed1);
                        cmd.Parameters.AddWithValue("@sd_wed_skill_2", class_Schedule.SD_Skill_Name_Wed2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_wed_2", class_Schedule.SD_Teacher_Wed2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_wed_2", class_Schedule.SD_TotalTime_Wed2);

                        cmd.Parameters.AddWithValue("@sd_thurs_skill_1", class_Schedule.SD_Skill_Name_Thur1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_thurs_1", class_Schedule.SD_Teacher_Thur1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_thurs_1", class_Schedule.SD_TotalTime_Thur1);
                        cmd.Parameters.AddWithValue("@sd_thurs_skill_2", class_Schedule.SD_Skill_Name_Thur2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_thurs_2", class_Schedule.SD_Teacher_Thur2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_thurs_2", class_Schedule.SD_TotalTime_Thur2);

                        cmd.Parameters.AddWithValue("@sd_fri_skill_1", class_Schedule.SD_Skill_Name_Fri1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_fri_1", class_Schedule.SD_Teacher_Fri1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_fri_1", class_Schedule.SD_TotalTime_Fri1);
                        cmd.Parameters.AddWithValue("@sd_fri_skill_2", class_Schedule.SD_Skill_Name_Fri2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_fri_2", class_Schedule.SD_Teacher_Fri2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_fri_2", class_Schedule.SD_TotalTime_Fri2);

                        cmd.Parameters.AddWithValue("@sd_datetime_start_schedule", class_Schedule.DateTime_Start_Schedule_Strating);
                        cmd.Parameters.AddWithValue("@sd_building_name", class_Schedule.SD_Building_Name);
                        cmd.Parameters.AddWithValue("@sd_building_room", class_Schedule.SD_Building_Room);
                        cmd.Parameters.AddWithValue("@sd_datetime_create", Create_Datetime);
                        cmd.Parameters.AddWithValue("@sd_user_create", User_Create);
                        


                        int result = cmd.ExecuteNonQuery();
                        return result == 1;
                    }                 
                }
            }
            catch (MySqlException e)
            {
                Debug.WriteLine($"Save Schedule Info Databbase Error: {e.Message}");
                return false;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Save Schedule Info Error: {ex.Message}");
                return false;
            }
        }

        //Method to Load Schedule Info to List Schedule
        public List<Class_Schedule> Load_Schedule_Table_Info(int class_id)
        {
            List<Class_Schedule> class_schedule_info = new List<Class_Schedule>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM class_schedule_mon_fri_info WHERE sd_class_id = @class_id";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@class_id", class_id);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Schedule schedule_info = new Class_Schedule()
                                    {
                                        Schedule_ID = reader.GetInt32("ID"),
                                        Class_ID_Schedule = reader.GetInt32("sd_class_id"),
                                        SD_Class_Name = reader.GetString("sd_class_name"),
                                        SD_Class_TimeShift = reader.GetString("sd_class_timeshift"),

                                        SD_Start_DateTime_MF1 = reader.GetTimeSpan("sd_start_time_1"),
                                        SD_End_DateTime_MF1 = reader.GetTimeSpan("sd_end_time_1"),
                                        SD_Start_DateTime_MF2 = reader.GetTimeSpan("sd_start_time_2"),
                                        SD_End_DateTime_MF2 = reader.GetTimeSpan("sd_end_time_2"),

                                        SD_Skill_Name_Mon1 = reader.GetString("sd_mon_skill_1"),
                                        SD_Teacher_Mon01 = reader.GetString("sd_name_teacher_mon_1"),
                                        SD_TotalTime_Mon1 = reader.GetInt32("sd_totaltime_skill_mon_1"),

                                        SD_Skill_Name_Mon2 = reader.GetString("sd_mon_skill_2"),
                                        SD_Teacher_Mon02 = reader.GetString("sd_name_teacher_mon_2"),
                                        SD_TotalTime_Mon2 = reader.GetInt32("sd_totaltime_skill_mon_2"),

                                        SD_Skill_Name_Tues1 = reader.GetString("sd_tues_skill_1"),
                                        SD_Teacher_Tues01 = reader.GetString("sd_name_teacher_tues_1"),
                                        SD_TotalTime_Tues1 = reader.GetInt32("sd_totaltime_skill_tues_1"),

                                        SD_Skill_Name_Tues2 = reader.GetString("sd_tues_skill_2"),
                                        SD_Teacher_Tues02 = reader.GetString("sd_name_teacher_tues_2"),
                                        SD_TotalTime_Tues2 = reader.GetInt32("sd_totaltime_skill_tues_2"),

                                        SD_Skill_Name_Wed1 = reader.GetString("sd_wed_skill_1"),
                                        SD_Teacher_Wed1 = reader.GetString("sd_name_teacher_wed_1"),
                                        SD_TotalTime_Wed1 = reader.GetInt32("sd_totaltime_skill_wed_1"),

                                        SD_Skill_Name_Wed2 = reader.GetString("sd_wed_skill_2"),
                                        SD_Teacher_Wed2 = reader.GetString("sd_name_teacher_wed_2"),
                                        SD_TotalTime_Wed2 = reader.GetInt32("sd_totaltime_skill_wed_2"),

                                        SD_Skill_Name_Thur1 = reader.GetString("sd_thurs_skill_1"),
                                        SD_Teacher_Thur1 = reader.GetString("sd_name_teacher_thurs_1"),
                                        SD_TotalTime_Thur1 = reader.GetInt32("sd_totaltime_skill_thurs_1"),

                                        SD_Skill_Name_Thur2 = reader.GetString("sd_thurs_skill_2"),
                                        SD_Teacher_Thur2 = reader.GetString("sd_name_teacher_thurs_2"),
                                        SD_TotalTime_Thur2 = reader.GetInt32("sd_totaltime_skill_thurs_2"),

                                        SD_Skill_Name_Fri1 = reader.GetString("sd_fri_skill_1"),
                                        SD_Teacher_Fri1 = reader.GetString("sd_name_teacher_fri_1"),
                                        SD_TotalTime_Fri1 = reader.GetInt32("sd_totaltime_skill_fri_1"),

                                        SD_Skill_Name_Fri2 = reader.GetString("sd_fri_skill_2"),
                                        SD_Teacher_Fri2 = reader.GetString("sd_name_teacher_fri_2"),
                                        SD_TotalTime_Fri2 = reader.GetInt32("sd_totaltime_skill_fri_2"),

                                        DateTime_Start_Schedule_Strating = reader.GetString("sd_datetime_start_schedule"),
                                        SD_Building_Name = reader.GetString("sd_building_name"),
                                        SD_Building_Room = reader.GetString("sd_building_room"),

                                    };
                                    class_schedule_info.Add(schedule_info);
                                }

                            }


                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Database load schedule error: {ex.Message}");
                }
            };
            return class_schedule_info;
        }

        //Method Update class schedule
        public bool UpdateSchedule(Class_Schedule class_Schedule)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query_update = "UPDATE class_schedule_mon_fri_info SET sd_class_name = @sd_class_name, sd_class_timeshift = @sd_class_timeshift, sd_start_time_1 = @sd_start_time_1, sd_end_time_1 = @sd_end_time_1, sd_start_time_2 = @sd_start_time_2, sd_end_time_2 = @sd_end_time_2, sd_mon_skill_1 = @sd_mon_skill_1, sd_name_teacher_mon_1 = @sd_name_teacher_mon_1, sd_totaltime_skill_mon_1 = @sd_totaltime_skill_mon_1, sd_mon_skill_2 = @sd_mon_skill_2, sd_name_teacher_mon_2 = @sd_name_teacher_mon_2, sd_totaltime_skill_mon_2 = @sd_totaltime_skill_mon_2, sd_tues_skill_1 = @sd_tues_skill_1, sd_name_teacher_tues_1 = @sd_name_teacher_tues_1, sd_totaltime_skill_tues_1 = @sd_totaltime_skill_tues_1, sd_tues_skill_2 = @sd_tues_skill_2, sd_name_teacher_tues_2 = @sd_name_teacher_tues_2, sd_totaltime_skill_tues_2 = @sd_totaltime_skill_tues_2, sd_wed_skill_1 = @sd_wed_skill_1, sd_name_teacher_wed_1 = @sd_name_teacher_wed_1, sd_totaltime_skill_wed_1 = @sd_totaltime_skill_wed_1, sd_wed_skill_2 = @sd_wed_skill_2, sd_name_teacher_wed_2 = @sd_name_teacher_wed_2, sd_totaltime_skill_wed_2 = @sd_totaltime_skill_wed_2, sd_thurs_skill_1 = @sd_thurs_skill_1, sd_name_teacher_thurs_1 = @sd_name_teacher_thurs_1, sd_totaltime_skill_thurs_1 = @sd_totaltime_skill_thurs_1, sd_thurs_skill_2 = @sd_thurs_skill_2, sd_name_teacher_thurs_2 = @sd_name_teacher_thurs_2, sd_totaltime_skill_thurs_2 = @sd_totaltime_skill_thurs_2, sd_fri_skill_1 = @sd_fri_skill_1, sd_name_teacher_fri_1 = @sd_name_teacher_fri_1, sd_totaltime_skill_fri_1 = @sd_totaltime_skill_fri_1, sd_fri_skill_2 = @sd_fri_skill_2, sd_name_teacher_fri_2 = @sd_name_teacher_fri_2, sd_totaltime_skill_fri_2 = @sd_totaltime_skill_fri_2, sd_datetime_start_schedule = @sd_datetime_start_schedule, sd_building_name = @sd_building_name, sd_building_room = @sd_building_room, sd_datetime_update = @sd_datetime_update, sd_user_update = @sd_user_update WHERE ID = @Schedule_ID";

                    MySqlCommand cmd = new MySqlCommand(query_update, conn);
                    {
                       
                        cmd.Parameters.AddWithValue("@sd_class_name", class_Schedule.SD_Class_Name);
                        cmd.Parameters.AddWithValue("@sd_class_timeshift", class_Schedule.SD_Class_TimeShift);
                        cmd.Parameters.AddWithValue("@sd_start_time_1", class_Schedule.SD_Start_DateTime_MF1);
                        cmd.Parameters.AddWithValue("@sd_end_time_1", class_Schedule.SD_End_DateTime_MF1);
                        cmd.Parameters.AddWithValue("@sd_start_time_2", class_Schedule.SD_Start_DateTime_MF2);
                        cmd.Parameters.AddWithValue("@sd_end_time_2", class_Schedule.SD_End_DateTime_MF2);

                        cmd.Parameters.AddWithValue("@sd_mon_skill_1", class_Schedule.SD_Skill_Name_Mon1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_mon_1", class_Schedule.SD_Teacher_Mon01);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_mon_1", class_Schedule.SD_TotalTime_Mon1);
                        cmd.Parameters.AddWithValue("@sd_mon_skill_2", class_Schedule.SD_Skill_Name_Mon2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_mon_2", class_Schedule.SD_Teacher_Mon02);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_mon_2", class_Schedule.SD_TotalTime_Mon2);

                        cmd.Parameters.AddWithValue("@sd_tues_skill_1", class_Schedule.SD_Skill_Name_Tues1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_tues_1", class_Schedule.SD_Teacher_Tues01);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_tues_1", class_Schedule.SD_TotalTime_Tues1);
                        cmd.Parameters.AddWithValue("@sd_tues_skill_2", class_Schedule.SD_Skill_Name_Tues2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_tues_2", class_Schedule.SD_Teacher_Tues02);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_tues_2", class_Schedule.SD_TotalTime_Tues2);

                        cmd.Parameters.AddWithValue("@sd_wed_skill_1", class_Schedule.SD_Skill_Name_Wed1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_wed_1", class_Schedule.SD_Teacher_Wed1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_wed_1", class_Schedule.SD_TotalTime_Wed1);
                        cmd.Parameters.AddWithValue("@sd_wed_skill_2", class_Schedule.SD_Skill_Name_Wed2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_wed_2", class_Schedule.SD_Teacher_Wed2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_wed_2", class_Schedule.SD_TotalTime_Wed2);

                        cmd.Parameters.AddWithValue("@sd_thurs_skill_1", class_Schedule.SD_Skill_Name_Thur1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_thurs_1", class_Schedule.SD_Teacher_Thur1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_thurs_1", class_Schedule.SD_TotalTime_Thur1);
                        cmd.Parameters.AddWithValue("@sd_thurs_skill_2", class_Schedule.SD_Skill_Name_Thur2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_thurs_2", class_Schedule.SD_Teacher_Thur2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_thurs_2", class_Schedule.SD_TotalTime_Thur2);

                        cmd.Parameters.AddWithValue("@sd_fri_skill_1", class_Schedule.SD_Skill_Name_Fri1);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_fri_1", class_Schedule.SD_Teacher_Fri1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_fri_1", class_Schedule.SD_TotalTime_Fri1);
                        cmd.Parameters.AddWithValue("@sd_fri_skill_2", class_Schedule.SD_Skill_Name_Fri2);
                        cmd.Parameters.AddWithValue("@sd_name_teacher_fri_2", class_Schedule.SD_Teacher_Fri2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_fri_2", class_Schedule.SD_TotalTime_Fri2);

                        cmd.Parameters.AddWithValue("@sd_datetime_start_schedule", class_Schedule.DateTime_Start_Schedule_Strating);
                        cmd.Parameters.AddWithValue("@sd_building_name", class_Schedule.SD_Building_Name);
                        cmd.Parameters.AddWithValue("@sd_building_room", class_Schedule.SD_Building_Room);
                        cmd.Parameters.AddWithValue("@sd_datetime_update", Update_Datetime);
                        cmd.Parameters.AddWithValue("@sd_user_update", User_Update);

                        cmd.Parameters.AddWithValue("@Schedule_ID", class_Schedule.Schedule_ID);
                    }

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }catch(MySqlException e)
            {
                Debug.WriteLine($"Datebase update schedule error: {e.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Update schedule error: {ex.Message}");
                return false;
            }

        }

        //Method Delete Schedule
        public bool DeleteSchedule(int scheduleID)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM class_schedule_mon_fri_info WHERE ID = @scheduleID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@scheduleID", scheduleID);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
                }
            }
            catch(MySqlException ex)
            {
                Debug.WriteLine($"Database delete schedule error: {ex.Message}");
                return false;
            }catch (Exception ex)
            {
                Debug.WriteLine($"Delete schedule error: {ex.Message}");
                return false;
            }
        }

        //Method Insert class Schedule Sat-Sun
        public bool Save_Schedule_SatSun_Info(Class_Schedule class_schedule_sat_sun)
        {
            try
            {
                using(MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO class_schedule_sat_sun(sd_class_id, sd_class_name, sd_class_timeshift, sd_schedule_name, sd_start_time_ss1, sd_end_time_ss1, sd_start_time_ss2, sd_end_time_ss2, sd_skill_name_sat1, sd_teacher_name_sat1, sd_totaltime_skill_sat1, sd_skill_name_sat2, sd_teacher_name_sat2, sd_totaltime_skill_sat2, sd_skill_name_sun1, sd_teacher_name_sun1, sd_totaltime_skill_sun1, sd_skill_name_sun2, sd_teacher_name_sun2, sd_totaltime_skill_sun2, sd_datetime_start_schedule, sd_building_name, sd_building_room, sd_datetime_create, sd_user_create)" +
                        "VALUES(@sd_class_id, @sd_class_name, @sd_class_timeshift, @sd_schedule_name, @sd_start_time_ss1, @sd_end_time_ss1, @sd_start_time_ss2, @sd_end_time_ss2, @sd_skill_name_sat1, @sd_teacher_name_sat1, @sd_totaltime_skill_sat1, @sd_skill_name_sat2, @sd_teacher_name_sat2, @sd_totaltime_skill_sat2, @sd_skill_name_sun1, @sd_teacher_name_sun1, @sd_totaltime_skill_sun1, @sd_skill_name_sun2, @sd_teacher_name_sun2, @sd_totaltime_skill_sun2, @sd_datetime_start_schedule, @sd_building_name, @sd_building_room, @sd_datetime_create, @sd_user_create)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@sd_class_id", class_schedule_sat_sun.Class_ID_Schedule);
                        cmd.Parameters.AddWithValue("@sd_class_name", class_schedule_sat_sun.SD_Class_Name);
                        cmd.Parameters.AddWithValue("@sd_class_timeshift", class_schedule_sat_sun.SD_Class_TimeShift);
                        cmd.Parameters.AddWithValue("@sd_schedule_name", class_schedule_sat_sun.Schedule_Name);
                        cmd.Parameters.AddWithValue("@sd_start_time_ss1", class_schedule_sat_sun.SD_Start_DateTime_SS1);
                        cmd.Parameters.AddWithValue("@sd_end_time_ss1", class_schedule_sat_sun.SD_End_DateTime_SS1);
                        cmd.Parameters.AddWithValue("@sd_start_time_ss2", class_schedule_sat_sun.SD_Start_DateTime_SS2);
                        cmd.Parameters.AddWithValue("@sd_end_time_ss2", class_schedule_sat_sun.SD_End_DateTime_SS2);

                        cmd.Parameters.AddWithValue("@sd_skill_name_sat1", class_schedule_sat_sun.SD_Skill_Name_Sat1);
                        cmd.Parameters.AddWithValue("@sd_teacher_name_sat1", class_schedule_sat_sun.SD_Teacher_Sat1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_sat1", class_schedule_sat_sun.SD_TotalTime_Sat1);
                        cmd.Parameters.AddWithValue("@sd_skill_name_sat2", class_schedule_sat_sun.SD_Skill_Name_Sat2);
                        cmd.Parameters.AddWithValue("@sd_teacher_name_sat2", class_schedule_sat_sun.SD_Teacher_Sat2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_sat2", class_schedule_sat_sun.SD_TotalTime_Sat2);

                        cmd.Parameters.AddWithValue("@sd_skill_name_sun1", class_schedule_sat_sun.SD_Skill_Name_Sun1);
                        cmd.Parameters.AddWithValue("@sd_teacher_name_sun1", class_schedule_sat_sun.SD_Teacher_Sun1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_sun1", class_schedule_sat_sun.SD_TotalTime_Sun1);
                        cmd.Parameters.AddWithValue("@sd_skill_name_sun2", class_schedule_sat_sun.SD_Skill_Name_Sun2);
                        cmd.Parameters.AddWithValue("@sd_teacher_name_sun2", class_schedule_sat_sun.SD_Teacher_Sun2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_sun2", class_schedule_sat_sun.SD_TotalTime_Sun2);

                        cmd.Parameters.AddWithValue("@sd_datetime_start_schedule", class_schedule_sat_sun.DateTime_Start_Schedule_Strating);
                        cmd.Parameters.AddWithValue("@sd_building_name", class_schedule_sat_sun.SD_Building_Name);
                        cmd.Parameters.AddWithValue("@sd_building_room", class_schedule_sat_sun.SD_Building_Room);
                        cmd.Parameters.AddWithValue("@sd_datetime_create", Create_Datetime);
                        cmd.Parameters.AddWithValue("@sd_user_create", User_Create);

                        int result = cmd.ExecuteNonQuery();
                        return result == 1;
                    }
                }
            }catch(MySqlException e)
            {
                Debug.WriteLine($"Database error insert schedule sat sun: {e.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error insert schedule sat sun: {ex.StackTrace}");
                return false;
            }
        }

        //Method Load Schedule Sat Sun
        public List<Class_Schedule> GetFetchSchedule_Table_Info(string class_id)
        {
            List<Class_Schedule> class_schedule_info = new List<Class_Schedule>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = "SELECT ID,sd_schedule_name FROM class_schedule_sat_sun WHERE sd_class_id = @sd_class_id ORDER BY sd_schedule_name DESC";

                        using(MySqlCommand cmd = new MySqlCommand(query,conn))
                        {
                            cmd.Parameters.AddWithValue("@sd_class_id", class_id);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    Class_Schedule schedule_info = new Class_Schedule()
                                    {
                                        Schedule_ID = reader.GetInt32("ID"),
                                        Schedule_Name = reader.GetString("sd_schedule_name"),
                                    };
                                    class_schedule_info.Add(schedule_info);
                                }
                            }
                        }
                        
                    }
                }
                catch(MySqlException ex)
                {
                    Debug.WriteLine($"Database fetch data schedule sat sun error: {ex.StackTrace}");
                    return null;
                }
                return class_schedule_info;
            }
        }

        //Load Schedule Info by Select Table Schedule Info SatSun
        public List<Class_Schedule> GetSchedule_Info_BySelectedTable(int schedule_id)
        {
            List<Class_Schedule> schedule_ss_info = new List<Class_Schedule>();

            try
            {
                using (MySqlConnection con = new MySqlConnection(_connectionString))
                {
                    con.Open();
                    string query = "SELECT * FROM class_schedule_sat_sun WHERE ID = @schedule_id";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@schedule_id", schedule_id);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                schedule_ss_info.Add(new Class_Schedule
                                {
                                    Schedule_ID = reader.GetInt32("ID"),
                                    Schedule_Name = reader.GetString("sd_schedule_name"),
                                    SD_Start_DateTime_SS1 = reader.GetTimeSpan("sd_start_time_ss1"),
                                    SD_End_DateTime_SS1 = reader.GetTimeSpan("sd_end_time_ss1"),
                                    SD_Start_DateTime_SS2 = reader.GetTimeSpan("sd_start_time_ss2"),
                                    SD_End_DateTime_SS2 = reader.GetTimeSpan("sd_end_time_ss2"),
                                    SD_Skill_Name_Sat1 = reader.GetString("sd_skill_name_sat1"),
                                    SD_Skill_Name_Sat2 = reader.GetString("sd_skill_name_sat2"),
                                    SD_Teacher_Sat1 = reader.GetString("sd_teacher_name_sat1"),
                                    SD_Teacher_Sat2 = reader.GetString("sd_teacher_name_sat2"),
                                    SD_TotalTime_Sat1 = reader.GetInt32("sd_totaltime_skill_sat1"),
                                    SD_TotalTime_Sat2 = reader.GetInt32("sd_totaltime_skill_sat2"),

                                    SD_Skill_Name_Sun1 = reader.GetString("sd_skill_name_sun1"),
                                    SD_Skill_Name_Sun2 = reader.GetString("sd_skill_name_sun2"),
                                    SD_Teacher_Sun1 = reader.GetString("sd_teacher_name_sun1"),
                                    SD_Teacher_Sun2 = reader.GetString("sd_teacher_name_sun2"),
                                    SD_TotalTime_Sun1 = reader.GetInt32("sd_totaltime_skill_sun1"),
                                    SD_TotalTime_Sun2 = reader.GetInt32("sd_totaltime_skill_sun2"),

                                    DateTime_Start_Schedule_Strating = reader.GetString("sd_datetime_start_schedule"),
                                    SD_Building_Name = reader.GetString("sd_building_name"),
                                    SD_Building_Room = reader.GetString("sd_building_room")

                                });
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database error GetSchedule_Info_BySelectedTable: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error GetSchedule_Info_BySelectedTable: {ex.Message}");
                return null;
            }
            return schedule_ss_info;
        }

        //Method Update Schedule SatSun
        public bool UpdateSchedule_SatSun(Class_Schedule schedule_ss_info_update)
        {
            try
            {
                string query_update = "UPDATE class_schedule_sat_sun SET " +
                      "sd_schedule_name = @sd_schedule_name, " +
                      "sd_start_time_ss1 = @sd_start_time_ss1, " +
                      "sd_end_time_ss1 = @sd_end_time_ss1, " +
                      "sd_start_time_ss2 = @sd_start_time_ss2, " +
                      "sd_end_time_ss2 = @sd_end_time_ss2, " +
                      "sd_skill_name_sat1 = @sd_skill_name_sat1, " +
                      "sd_teacher_name_sat1 = @sd_teacher_name_sat1, " +
                      "sd_totaltime_skill_sat1 = @sd_totaltime_skill_sat1, " +
                      "sd_skill_name_sat2 = @sd_skill_name_sat2, " +
                      "sd_teacher_name_sat2 = @sd_teacher_name_sat2, " +
                      "sd_totaltime_skill_sat2 = @sd_totaltime_skill_sat2, " +
                      "sd_skill_name_sun1 = @sd_skill_name_sun1, " +
                      "sd_teacher_name_sun1 = @sd_teacher_name_sun1, " +
                      "sd_totaltime_skill_sun1 = @sd_totaltime_skill_sun1, " +
                      "sd_skill_name_sun2 = @sd_skill_name_sun2, " +
                      "sd_teacher_name_sun2 = @sd_teacher_name_sun2, " +
                      "sd_totaltime_skill_sun2 = @sd_totaltime_skill_sun2, " +
                      "sd_datetime_start_schedule = @sd_datetime_start_schedule, " +
                      "sd_building_name = @sd_building_name, " +
                      "sd_building_room = @sd_building_room, " +
                      "sd_datetime_update = @sd_datetime_update, " +
                      "sd_user_update = @sd_user_update " +
                      "WHERE ID = @Schedule_ID";


                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query_update, conn);
                    {
                        cmd.Parameters.AddWithValue("@Schedule_ID", schedule_ss_info_update.Schedule_ID);
                        cmd.Parameters.AddWithValue("@sd_schedule_name", schedule_ss_info_update.Schedule_Name);
                        cmd.Parameters.AddWithValue("@sd_start_time_ss1", schedule_ss_info_update.SD_Start_DateTime_SS1);
                        cmd.Parameters.AddWithValue("@sd_end_time_ss1", schedule_ss_info_update.SD_End_DateTime_SS1);
                        cmd.Parameters.AddWithValue("@sd_start_time_ss2", schedule_ss_info_update.SD_Start_DateTime_SS2);
                        cmd.Parameters.AddWithValue("@sd_end_time_ss2", schedule_ss_info_update.SD_End_DateTime_SS2);
                        cmd.Parameters.AddWithValue("@sd_skill_name_sat1", schedule_ss_info_update.SD_Skill_Name_Sat1);
                        cmd.Parameters.AddWithValue("@sd_teacher_name_sat1", schedule_ss_info_update.SD_Teacher_Sat1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_sat1", schedule_ss_info_update.SD_TotalTime_Sat1);
                        cmd.Parameters.AddWithValue("@sd_skill_name_sat2", schedule_ss_info_update.SD_Skill_Name_Sat2);
                        cmd.Parameters.AddWithValue("@sd_teacher_name_sat2", schedule_ss_info_update.SD_Teacher_Sat2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_sat2", schedule_ss_info_update.SD_TotalTime_Sat2);
                        cmd.Parameters.AddWithValue("@sd_skill_name_sun1", schedule_ss_info_update.SD_Skill_Name_Sun1);
                        cmd.Parameters.AddWithValue("@sd_teacher_name_sun1", schedule_ss_info_update.SD_Teacher_Sun1);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_sun1", schedule_ss_info_update.SD_TotalTime_Sun1);
                        cmd.Parameters.AddWithValue("@sd_skill_name_sun2", schedule_ss_info_update.SD_Skill_Name_Sun2);
                        cmd.Parameters.AddWithValue("@sd_teacher_name_sun2", schedule_ss_info_update.SD_Teacher_Sun2);
                        cmd.Parameters.AddWithValue("@sd_totaltime_skill_sun2", schedule_ss_info_update.SD_TotalTime_Sun2);
                        cmd.Parameters.AddWithValue("@sd_datetime_start_schedule", schedule_ss_info_update.DateTime_Start_Schedule_Strating);
                        cmd.Parameters.AddWithValue("@sd_building_name", schedule_ss_info_update.SD_Building_Name);
                        cmd.Parameters.AddWithValue("@sd_building_room", schedule_ss_info_update.SD_Building_Room);
                        cmd.Parameters.AddWithValue("@sd_datetime_update",Update_Datetime);
                        cmd.Parameters.AddWithValue("@sd_user_update", User_Update);
                    }
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch(MySqlException ex)
            {
                Debug.WriteLine($"Database error GetSchedule_Info_BySelectedTable :{ex.Message}");
                return false;
            }catch (Exception ex)
            {
                Debug.WriteLine($"Error GetSchedule_Info_BySelectedTable :{ex.Message}");
                return false;
            }
        }

        //Method Delete Schedule SatSun
        public bool DeleteSchedule_SatSun(int schedule_id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM class_schedule_sat_sun WHERE ID = @schedule_id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@schedule_id", schedule_id);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;

                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database error DeleteSchedule_SatSun: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error DeleteSchedule_SatSun: {ex.Message}");
                return false;
            }
            
        }

        //Method Update Class State
        public bool Update_Class_State(string class_id,string current_state)
        {
            try
            {
                string query = "UPDATE classes SET Class_State = @Class_State WHERE class_id = @class_id";

                using(MySqlConnection  connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    {
                        cmd.Parameters.AddWithValue("@class_id", class_id);
                        cmd.Parameters.AddWithValue("@Class_State", current_state);
                    }
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }

            }catch (MySqlException ex)
            {
                Debug.WriteLine($"Database error Update_Class_State: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error Update_Class_State: {ex.Message}");
                return false;
            }
        }

        //Search class Name, Class State
        public List<Student_Info> GetClass_State_Info(string Search_Class_Name_State)
        {
            List<Student_Info> class_info = new List<Student_Info>();
            try
            {
                string query1 = "SELECT COUNT(*) AS TotalCount FROM classes";

                string query = string.IsNullOrEmpty(Search_Class_Name_State)
                            ? "SELECT * FROM classes ORDER BY class_id DESC"
                            : "SELECT class_id,class_name,Class_State FROM classes WHERE class_name LIKE @Search_Class_Name_State || Class_State LIKE @Search_Class_Name_State ORDER BY class_id DESC";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var cmd = new MySqlCommand(query1, connection))
                    {
                        No_Classes = Convert.ToInt32(cmd.ExecuteScalar()); // Assign the total count to No_Classes
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrWhiteSpace(Search_Class_Name_State))
                        {
                            cmd.Parameters.AddWithValue("@Search_Class_Name_State", $"%{Search_Class_Name_State}%");
                        }

                        cmd.CommandTimeout = 30;  // Set a timeout of 30 seconds

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            //int No_Classes = 1;
                            while (reader.Read())
                            {

                                Student_Info classes = new Student_Info
                                {
                                    No_Class = No_Classes.ToString(),
                                    Class_ID = reader.GetInt32("class_id").ToString(),
                                    Class_Name = reader.IsDBNull(reader.GetOrdinal("class_name")) ? string.Empty : reader.GetString("class_name"),
                                    Current_Class_State = reader.IsDBNull(reader.GetOrdinal("Class_State")) ? string.Empty : reader.GetString("Class_State"),

                                };
                                No_Classes--;
                                class_info.Add(classes);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MySql GetClass_State_Info to ListView Error:{ex.Message}");
            }
            return class_info;
        }

        //Load Data Score Type
        public List<Class_Score> GetScoreType_toCombobox_info()
        {
            List<Class_Score> score_type = new List<Class_Score>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT ID,score_type_kh FROM education_score_type";

                MySqlCommand command = new MySqlCommand(query, connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        score_type.Add(new Class_Score
                        {
                            Score_Type_ID = reader.GetInt32("ID"),
                            Score_Type_Name = reader.GetString("score_type_kh")
                        });
                    }
                }
            }
            return score_type;
        }

        //Load Skill Class Score SatSun
        public List<Class_Score> GetFetchSchedule_Skill_SatSunTable_Info(string class_id)
        {
            List<Class_Score> class_skill_info = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = @"
                SELECT DISTINCT Schedule_ID,Time,skill, day, Teacher 
                FROM (
                    SELECT ID as Schedule_ID,sd_totaltime_skill_sat1 as Time, sd_class_id, sd_skill_name_sat1 AS skill, sd_teacher_name_sat1 AS Teacher, 'ថ្ងៃសៅរ៍' AS day FROM class_schedule_sat_sun
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_sat2, sd_class_id, sd_skill_name_sat2, sd_teacher_name_sat1, 'ថ្ងៃសៅរ៍' FROM class_schedule_sat_sun
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_sun1, sd_class_id, sd_skill_name_sun1, sd_teacher_name_sun1, 'ថ្ងៃអាទិត្យ' FROM class_schedule_sat_sun
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_sun2, sd_class_id, sd_skill_name_sun2, sd_teacher_name_sun2, 'ថ្ងៃអាទិត្យ' FROM class_schedule_sat_sun           
                ) AS skills_table
                WHERE sd_class_id = @sd_class_id AND skill IS NOT NULL
                ORDER BY day, skill";


                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@sd_class_id", class_id);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score schedule_info = new Class_Score()
                                    {
                                        Score_Schedule_ID = reader.GetInt32("Schedule_ID"),
                                        Score_Skill_Day = reader.GetString("day"),
                                        Score_Skill_TotalTime = reader.GetInt32("Time"),
                                        Score_Skill_Name = reader.GetString("skill"),
                                        Score_Skill_TeacherName = reader.GetString("Teacher"),
                                    };
                                    class_skill_info.Add(schedule_info);
                                }
                            }
                        }

                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Database fetch data schedule SatSub error: {ex.StackTrace}");
                    return null;
                }
                return class_skill_info;
            }
        }

        //Load Skill Class Score MonFri
        public List<Class_Score> GetFetchSchedule_Skill_MonFriTable_Info(string class_id)
        {
            List<Class_Score> class_skill_info = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = @"
                SELECT DISTINCT Schedule_ID,skill,Time, day, Teacher 
                FROM (
                    SELECT ID as Schedule_ID,sd_totaltime_skill_mon_1 as Time, sd_class_id, sd_mon_skill_1 AS skill, sd_name_teacher_mon_1 AS Teacher, 'ថ្ងៃចន្ទ' AS day FROM class_schedule_mon_fri_info
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_mon_2, sd_class_id, sd_mon_skill_2, sd_name_teacher_mon_2, 'ថ្ងៃចន្ទ' FROM class_schedule_mon_fri_info
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_tues_1, sd_class_id, sd_tues_skill_1, sd_name_teacher_tues_1, 'ថ្ងៃអង្គារ' FROM class_schedule_mon_fri_info
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_tues_2, sd_class_id, sd_tues_skill_2,sd_name_teacher_tues_2, 'ថ្ងៃអង្គារ' FROM class_schedule_mon_fri_info
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_wed_1, sd_class_id, sd_wed_skill_1, sd_name_teacher_wed_1, 'ថ្ងៃពុធ' FROM class_schedule_mon_fri_info
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_wed_2, sd_class_id, sd_wed_skill_2, sd_name_teacher_wed_2, 'ថ្ងៃពុធ' FROM class_schedule_mon_fri_info
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_thurs_1, sd_class_id, sd_thurs_skill_1, sd_name_teacher_thurs_1, 'ថ្ងៃព្រហ' FROM class_schedule_mon_fri_info
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_thurs_2, sd_class_id, sd_thurs_skill_2, sd_name_teacher_thurs_2, 'ថ្ងៃព្រហ' FROM class_schedule_mon_fri_info
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_fri_1, sd_class_id, sd_fri_skill_1, sd_name_teacher_fri_1, 'ថ្ងៃសុក្រ' FROM class_schedule_mon_fri_info
                    UNION ALL
                    SELECT ID,sd_totaltime_skill_fri_2, sd_class_id, sd_fri_skill_2, sd_name_teacher_fri_2, 'ថ្ងៃសុក្រ' FROM class_schedule_mon_fri_info
                ) AS skills_table
                WHERE sd_class_id = @sd_class_id AND skill IS NOT NULL
                ORDER BY day, skill";


                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@sd_class_id", class_id);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score schedule_info = new Class_Score()
                                    {
                                        Score_Schedule_ID = reader.GetInt32("Schedule_ID"),
                                        Score_Skill_Day = reader.GetString("day"),
                                        Score_Skill_TotalTime = reader.GetInt32("Time"),
                                        Score_Skill_Name = reader.GetString("skill"),
                                        Score_Skill_TeacherName = reader.GetString("Teacher"),
                                    };
                                    class_skill_info.Add(schedule_info);
                                }
                            }
                        }

                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Database fetch data schedule MonFri error: {ex.StackTrace}");
                    return null;
                }
                return class_skill_info;
            }
        }

        //Load Student, Score to List
        public List<Class_Score> GetFetch_Student_Info_For_Score(string class_id)
        {
            List<Class_Score> class_student_info = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        
                        string query = "SELECT si.ID,si.stu_id,si.stu_firstname_kh,si.stu_lastname_kh,si.stu_gender,si.stu_birthday_dateonly FROM class_enrollments ce JOIN student_infomations si ON ce.stu_id = si.ID WHERE ce.class_id = @class_id";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@class_id", class_id);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score student_list_info = new Class_Score()
                                    {
                                        Score_Stu_ID = reader.GetInt32("ID"),
                                        Score_Student_ID = reader.GetString("stu_id"),
                                        Score_Student_Name = reader.GetString("stu_firstname_kh") + " " + reader.GetString("stu_lastname_kh"),
                                        Score_Student_Gender = reader.GetString("stu_gender"),
                                        Score_Student_BirthDay = reader.GetString("stu_birthday_dateonly")
                                    };
                                    class_student_info.Add(student_list_info);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Datasbase GetFetch_Student_Info_For_Score Error: {ex.Message}");
                    return null;
                }
                return class_student_info;
            }
        }

        //Save Student Score MonFri
        public bool Save_Student_Score_MonFri_Info(Class_Score student_score_info,string Class_ID,int Score_Schedule_ID,string Score_Skill_Name,int Score_Skill_TotalTime,string Score_Skill_TeacherName,string Score_Type_Name)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO class_schedule_score_mon_fri_info(class_id, schedule_id, subject_name, total_subject_time, teacher_name, stu_id, student_id, student_name, student_gender,stu_birthday, student_score, student_score_type, insert_time, insert_user)" +
                        "VALUES(@class_id, @schedule_id, @subject_name, @total_subject_time, @teacher_name, @stu_id, @student_id, @student_name, @student_gender,@stu_birthday, @student_score, @student_score_type, @insert_time, @insert_user)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@class_id",Convert.ToInt32(Class_ID));
                        cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                        cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);
                        cmd.Parameters.AddWithValue("@total_subject_time", Score_Skill_TotalTime);
                        cmd.Parameters.AddWithValue("@teacher_name", Score_Skill_TeacherName);
                        cmd.Parameters.AddWithValue("@stu_id", student_score_info.Score_Stu_ID);
                        cmd.Parameters.AddWithValue("@student_id", student_score_info.Score_Student_ID);
                        cmd.Parameters.AddWithValue("@student_name", student_score_info.Score_Student_Name);
                        cmd.Parameters.AddWithValue("@student_gender", student_score_info.Score_Student_Gender);
                        cmd.Parameters.AddWithValue("@stu_birthday", student_score_info.Score_Student_BirthDay);
                        cmd.Parameters.AddWithValue("@student_score", student_score_info.Student_Score);
                        cmd.Parameters.AddWithValue("@student_score_type", Score_Type_Name);
                        cmd.Parameters.AddWithValue("@insert_time", Create_Datetime);
                        cmd.Parameters.AddWithValue("@insert_user", User_Create);

                        int result = cmd.ExecuteNonQuery();
                        return result == 1;
                    };
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"MySQL Error in Save_Student_MonFri_Info: {ex.Number} - {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Error in Save_Student_MonFri_Info: {ex.Message}");
                return false;
            }

        }

        //Save Student Score SatSun
        public bool Save_Student_Score_SatSun_Info(Class_Score student_score_info, string Class_ID, int Score_Schedule_ID, string Score_Skill_Name, int Score_Skill_TotalTime, string Score_Skill_TeacherName,string Score_Type_Name)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO class_schedule_score_sat_sun_info(class_id, schedule_id, subject_name, total_subject_time, teacher_name, stu_id, student_id, student_name, student_gender,stu_birthday, student_score, student_score_type, insert_time, insert_user)" +
                        "VALUES(@class_id, @schedule_id, @subject_name, @total_subject_time, @teacher_name, @stu_id, @student_id, @student_name, @student_gender,@stu_birthday, @student_score, @student_score_type, @insert_time, @insert_user)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@class_id",Convert.ToInt32(Class_ID));
                    cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                    cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);
                    cmd.Parameters.AddWithValue("@total_subject_time", Score_Skill_TotalTime);
                    cmd.Parameters.AddWithValue("@teacher_name", Score_Skill_TeacherName);
                    cmd.Parameters.AddWithValue("@stu_id", student_score_info.Score_Stu_ID);
                    cmd.Parameters.AddWithValue("@student_id", student_score_info.Score_Student_ID);
                    cmd.Parameters.AddWithValue("@student_name", student_score_info.Score_Student_Name);
                    cmd.Parameters.AddWithValue("@student_gender", student_score_info.Score_Student_Gender);
                    cmd.Parameters.AddWithValue("@stu_birthday", student_score_info.Score_Student_BirthDay);
                    cmd.Parameters.AddWithValue("@student_score", student_score_info.Student_Score);
                    cmd.Parameters.AddWithValue("@student_score_type", Score_Type_Name);
                    cmd.Parameters.AddWithValue("@insert_time", Create_Datetime);
                    cmd.Parameters.AddWithValue("@insert_user", User_Create);

                    int result = cmd.ExecuteNonQuery();
                    return result == 1;
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Error Database Save_Student_SatSun_Info {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Save_Student_SatSun_Info {ex.Message}");
                return false;
            }
        }

        //Load Skill Class Score State MonFri
        public List<Class_Score> GetFetchSchedule_Skill_MonFriTable_State_Info(int Score_Schedule_ID,string Score_Skill_Name)
        {
            List<Class_Score> class_skill_state_info = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = @"SELECT DISTINCT student_score_type 
                                FROM rpisvr_system.class_schedule_score_mon_fri_info
                                WHERE subject_name = @subject_name 
                                AND schedule_id = @schedule_id";


                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                            cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score skill_state_info = new Class_Score()
                                    {
                                        Show_Score_Type = reader.GetString("student_score_type"),
                                        State_Score_Type = "មាន"
                                    };
                                    
                                    class_skill_state_info.Add(skill_state_info);
                                }
                            }
                        }

                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Database fetch data skill state error: {ex.StackTrace}");
                    return null;
                }
                return class_skill_state_info;
            }
        }

        //Load Skill Class Score State SatSun
        public List<Class_Score> GetFetchSchedule_Skill_SatSunTable_State_Info(int Score_Schedule_ID, string Score_Skill_Name)
        {
            List<Class_Score> class_skill_state_info = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = @"SELECT DISTINCT student_score_type 
                                FROM rpisvr_system.class_schedule_score_sat_sun_info
                                WHERE subject_name = @subject_name 
                                AND schedule_id = @schedule_id";


                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                            cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score skill_state_info = new Class_Score()
                                    {
                                        Show_Score_Type = reader.GetString("student_score_type"),
                                        State_Score_Type = "មាន"
                                    };

                                    class_skill_state_info.Add(skill_state_info);
                                }
                            }
                        }

                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Database fetch data skill state error: {ex.StackTrace}");
                    return null;
                }
                return class_skill_state_info;
            }
        }

        //Load Student Info, Score For Edit
        public List<Class_Score> SelectFetch_Student_Info_For_Score(string Class_ID, int Score_Schedule_ID, string Score_Skill_Name, string Show_Score_Type)
        {
            List<Class_Score> class_load_student_info = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = @"
                SELECT stu_id, student_id, student_name, student_gender, stu_birthday, student_score, student_score_type 
                FROM rpisvr_system.class_schedule_score_mon_fri_info
                WHERE class_id = @class_id 
                AND schedule_id = @schedule_id 
                AND subject_name = @subject_name 
                AND student_score_type = @student_score_type 
                ORDER BY 
                    CASE
                        WHEN student_name REGEXP '^[ក-អ]' THEN 1
                        WHEN student_name REGEXP '^[អឥឦឧឩឪឫឬឭឮឯឰឱឲ]' THEN 2
                        ELSE 3
                    END, 
                    student_name ASC";


                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                            cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);
                            cmd.Parameters.AddWithValue("@student_score_type", Show_Score_Type);
                            if (int.TryParse(Class_ID, out int parsedClassID))
                            {
                                cmd.Parameters.AddWithValue("@class_id", parsedClassID);
                            }
                            else
                            {
                                Debug.WriteLine("Invalid Class_ID format");
                                return null;
                            }
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score load_student_info = new Class_Score()
                                    {
                                        Score_Stu_ID = reader.GetInt32("stu_id"),
                                        Score_Student_ID = reader.GetString("student_id"),
                                        Score_Student_Name = reader.GetString("student_name"),
                                        Score_Student_Gender = reader.GetString("student_gender"),
                                        Score_Student_BirthDay = reader.GetString("stu_birthday"),
                                        Student_Score = reader.GetInt32("student_score"),
                                        Score_Type_Name = reader.GetString("student_score_type")  
                                    };

                                    class_load_student_info.Add(load_student_info);
                                }
                            }
                        }

                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Database fetch data student info error: {ex.StackTrace}");
                    return null;
                }
                return class_load_student_info;
            }
        }

        //Update Student Score MonFri
        public bool Update_Student_Score_MonFri_Info(Class_Score student_score_info, string Class_ID, int Score_Schedule_ID, string Score_Skill_Name, string Score_Type_Name)
        {
            try
            {
                using(MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE class_schedule_score_mon_fri_info SET student_score = @student_score,update_time = @update_time,update_user = @update_user WHERE student_id = @student_id AND schedule_id = @schedule_id AND subject_name = @subject_name AND student_score_type = @student_score_type";

                    using (MySqlConnection connection = new MySqlConnection(_connectionString))
                    {
                        connection.Open();

                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        {
                            cmd.Parameters.AddWithValue("@student_score", student_score_info.Student_Score);
                            cmd.Parameters.AddWithValue("@update_time", Update_Datetime);
                            cmd.Parameters.AddWithValue("@update_user", User_Update);

                            cmd.Parameters.AddWithValue("@student_id", student_score_info.Score_Student_ID);
                            cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                            cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);
                            cmd.Parameters.AddWithValue("@student_score_type", Score_Type_Name);
                        }
                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }

                }
            }catch (MySqlException ex)
            {
                Debug.WriteLine($"Update error student score database: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Update error student score: {ex.Message}");
                return false;
            }
        }

        //Update Student Score SatSun
        public bool Update_Student_Score_SatSun_Info(Class_Score student_score_info, string Class_ID, int Score_Schedule_ID, string Score_Skill_Name, string Score_Type_Name)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE class_schedule_score_sat_sun_info SET student_score = @student_score,update_time = @update_time,update_user = @update_user WHERE student_id = @student_id AND schedule_id = @schedule_id AND subject_name = @subject_name AND student_score_type = @student_score_type";

                    using (MySqlConnection connection = new MySqlConnection(_connectionString))
                    {
                        connection.Open();

                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        {
                            cmd.Parameters.AddWithValue("@student_score", student_score_info.Student_Score);
                            cmd.Parameters.AddWithValue("@update_time", Update_Datetime);
                            cmd.Parameters.AddWithValue("@update_user", User_Update);

                            cmd.Parameters.AddWithValue("@student_id", student_score_info.Score_Student_ID);
                            cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                            cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);
                            cmd.Parameters.AddWithValue("@student_score_type", Score_Type_Name);
                        }
                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }

                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Update error student score database: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update error student score: {ex.Message}");
                return false;
            }
        }

        //Method Check Student Score MonFri Type Before Insert
        public async Task<(string Score_Student_ID1,string Score_Skill_Name1,int Score_Schedule_ID1,string Score_Type_Name1)> Check_Student_Score_MonFri_Info(string Score_Student_ID, string Score_Skill_Name, int Score_Schedule_ID, string Score_Type_Name)
        {
            const string query = "SELECT schedule_id,subject_name,student_id,student_score_type FROM class_schedule_score_mon_fri_info WHERE schedule_id = @schedule_id AND subject_name = @subject_name AND student_id = @student_id AND student_score_type = @student_score_type";

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                    cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);
                    cmd.Parameters.AddWithValue("@student_id", Score_Student_ID);
                    cmd.Parameters.AddWithValue("@student_score_type", Score_Type_Name);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (await reader.ReadAsync())
                        {
                            return (
                                Score_Student_ID1: reader["student_id"].ToString(),
                                Score_Skill_Name1: reader["subject_name"].ToString(),
                                Score_Schedule_ID1: reader.GetInt32("schedule_id"),
                                Score_Type_Name1: reader["student_score_type"].ToString()
                               
                            );
                        }
                    }

                }
            }
            return (null, null, 0, null);
        }

        //Method Check Student Score SatSun Type Before Insert
        public async Task<(string Score_Student_ID1, string Score_Skill_Name1, int Score_Schedule_ID1, string Score_Type_Name1)> Check_Student_Score_SatSun_Info(string Score_Student_ID, string Score_Skill_Name, int Score_Schedule_ID, string Score_Type_Name)
        {
            const string query = "SELECT schedule_id,subject_name,student_id,student_score_type FROM class_schedule_score_sat_sun_info WHERE schedule_id = @schedule_id AND subject_name = @subject_name AND student_id = @student_id AND student_score_type = @student_score_type";

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                    cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);
                    cmd.Parameters.AddWithValue("@student_id", Score_Student_ID);
                    cmd.Parameters.AddWithValue("@student_score_type", Score_Type_Name);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (await reader.ReadAsync())
                        {
                            return (
                                Score_Student_ID1: reader["student_id"].ToString(),
                                Score_Skill_Name1: reader["subject_name"].ToString(),
                                Score_Schedule_ID1: reader.GetInt32("schedule_id"),
                                Score_Type_Name1: reader["student_score_type"].ToString()

                            );
                        }
                    }

                }
            }
            return (null, null, 0, null);
        }

        //Method Delete Student Score MonFri
        public bool Delete_Student_Score_Info_MonFri(string Class_ID,int Score_Schedule_ID,string Score_Skill_Name,string Show_Score_Type)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM class_schedule_score_mon_fri_info WHERE class_id = @class_id AND schedule_id = @schedule_id AND subject_name = @subject_name AND student_score_type = @student_score_type";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@class_id", Class_ID);
                    cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                    cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);
                    cmd.Parameters.AddWithValue("@student_score_type", Show_Score_Type);
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;

                }
            }
            catch(MySqlException ex)
            {
                Debug.WriteLine($"Error Database Delete_Student_Info_MonFri: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error Delete_Student_Info_MonFri: {ex.Message}");
                return false;
            }
        }

        //Method Delete Student Score SatSun
        public bool Delete_Student_Score_Info_SatSun(string Class_ID, int Score_Schedule_ID, string Score_Skill_Name, string Show_Score_Type)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM class_schedule_score_sat_sun_info WHERE class_id = @class_id AND schedule_id = @schedule_id AND subject_name = @subject_name AND student_score_type = @student_score_type";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@class_id", Class_ID);
                    cmd.Parameters.AddWithValue("@schedule_id", Score_Schedule_ID);
                    cmd.Parameters.AddWithValue("@subject_name", Score_Skill_Name);
                    cmd.Parameters.AddWithValue("@student_score_type", Show_Score_Type);
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;

                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Error Database Delete_Student_Info_MonFri: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Delete_Student_Info_MonFri: {ex.Message}");
                return false;
            }
        }

        //Method to Get Student Info
        public List<Class_Score> SelectFetch_Students_Info_Score(string class_id)
        {
            List<Class_Score> class_student_info = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = "SELECT si.ID,si.stu_id,si.stu_firstname_kh,si.stu_lastname_kh,si.stu_gender,si.stu_birthday_dateonly,si.stu_image_source FROM class_enrollments ce JOIN student_infomations si ON ce.stu_id = si.ID WHERE ce.class_id = @class_id ORDER BY CASE WHEN stu_firstname_kh REGEXP '^[ក-អ]' THEN 1 WHEN stu_firstname_kh REGEXP '^[អឥឦឧឩឪឫឬឭឮឯឰឱឲ]' THEN 2 ELSE 3 END, stu_firstname_kh ASC";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@class_id", class_id);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score student_list_info = new Class_Score()
                                    {
                                        Score_Stu_ID = reader.GetInt32("ID"),
                                        Score_Student_ID = reader.GetString("stu_id"),
                                        Score_Student_Name = reader.GetString("stu_firstname_kh") + " " + reader.GetString("stu_lastname_kh"),
                                        Score_Student_Gender = reader.GetString("stu_gender"),
                                        Score_Student_BirthDay = reader.GetString("stu_birthday_dateonly")
                                        //ProfileImageBytes = reader.IsDBNull(reader.GetOrdinal("stu_image_source")) ? null : (byte[])reader["stu_image_source"]
                                    };

                                    // Read the image as byte array from MySQL
                                // Stu_Image
                                if (!reader.IsDBNull(reader.GetOrdinal("stu_image_source")))
                                    {
                                        // First, get the size of the image byte array from the database
                                        long byteSize = reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, null, 0, 0);

                                        if (byteSize > 0)
                                        {
                                            // Initialize the byte array with the correct size
                                            byte[] imageBytes = new byte[byteSize];

                                            // Now, read the image data into the byte array
                                            reader.GetBytes(reader.GetOrdinal("stu_image_source"), 0, imageBytes, 0, (int)byteSize);

                                            // If the image byte array is not empty, process it
                                            if (imageBytes != null && imageBytes.Length > 0)
                                            {
                                                student_list_info.ProfileImageBytes = imageBytes;  // Store the image bytes for future use
                                                student_list_info.Stu_Image_Source = ConvertBytesToImage(imageBytes);  // Convert the byte array to a BitmapImage
                                            }
                                            else
                                            {
                                                Debug.WriteLine("No image data found for student " + student_list_info.Score_Stu_ID);
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("Byte size is 0 for student " + student_list_info.Score_Stu_ID);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("stu_image_source is NULL for student " + student_list_info.Score_Stu_ID);
                                    }

                                    class_student_info.Add(student_list_info);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Datasbase GetFetch_Student_Info_For_Score Error: {ex.Message}");
                    return null;
                }
                return class_student_info;
            }
        }

        //Method to get Score Type Info MonFri
        public List<Class_Score> GetFetch_Student_Score_MonFri_Types_Total(string student_id,string class_id)
        {
            List<Class_Score> score_type = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = "SELECT subject_name,total_subject_time,student_score_type,student_score FROM class_schedule_score_mon_fri_info  WHERE class_id = @class_id AND student_id = @student_id ORDER BY subject_name";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@class_id", class_id);
                            cmd.Parameters.AddWithValue("@student_id", student_id);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score student_list_info = new Class_Score()
                                    {
                                        Score_Skill_Name = reader.GetString("subject_name"),
                                        Score_Skill_TotalTime = reader.GetInt32("total_subject_time"),
                                        Show_Score_Type = reader.GetString("student_score_type"),
                                        Student_Score = reader.GetInt32("student_score")
                                    };
                                    score_type.Add(student_list_info);
                                }           
                            }     
                                
                        }
                        conn.Close();
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Datasbase GetFetch_Student_Info_ScoreType Error: {ex.Message}");
                    return null;
                }
                return score_type;
            }
        }

        //Method to get Score Type Info SatSun
        public List<Class_Score> GetFetch_Student_Score_SatSun_Types_Total(string student_id, string class_id)
        {
            List<Class_Score> score_type = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = "SELECT subject_name,total_subject_time,student_score_type,student_score FROM class_schedule_score_mon_fri_info  WHERE class_id = @class_id AND student_id = @student_id ORDER BY subject_name";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@class_id", class_id);
                            cmd.Parameters.AddWithValue("@student_id", student_id);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score student_list_info = new Class_Score()
                                    {
                                        Score_Skill_Name = reader.GetString("subject_name"),
                                        Score_Skill_TotalTime = reader.GetInt32("total_subject_time"),
                                        Show_Score_Type = reader.GetString("student_score_type"),
                                        Student_Score = reader.GetInt32("student_score")
                                    };
                                    score_type.Add(student_list_info);
                                }
                            }

                        }
                        conn.Close();
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Datasbase GetFetch_Student_Info_ScoreType Error: {ex.Message}");
                    return null;
                }
                return score_type;
            }
        }

        //Method to Save Setting Score
        public bool Save_Setting_Score(Class_Score score_setting)
        {
            try
            {
                using(MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO class_setting_score (score_setting_first,score_setting_second,score_setting_letter_grade,score_setting_GPA_value,score_setting_grade_system) VALUES(@score_setting_first,@score_setting_second,@score_setting_letter_grade,@score_setting_GPA_value,@score_setting_grade_system)";

                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@score_setting_first", score_setting.Setting_Score1);
                        cmd.Parameters.AddWithValue("@score_setting_second", score_setting.Setting_Score2);
                        cmd.Parameters.AddWithValue("@score_setting_letter_grade", score_setting.Setting_Letter_Grade);
                        cmd.Parameters.AddWithValue("@score_setting_GPA_value", score_setting.Setting_GPA_Value);
                        cmd.Parameters.AddWithValue("@score_setting_grade_system", score_setting.Setting_Grade_System);

                        int result = cmd.ExecuteNonQuery();
                        return result == 1;

                    }
                }
            }catch(MySqlException ex)
            {
                Debug.WriteLine($"Error Database Save_Setting_Score: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error Save_Setting_Score: {ex.Message}");
                return false;
            }

        }

        //Method Load Setting Score
        public List<Class_Score>Load_Setting_Score_ToTable()
        {
            List<Class_Score> setting_score_list = new List<Class_Score>();
            {

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM class_setting_score";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score setting_score = new Class_Score()
                                    {
                                        Setting_Score_ID = reader.GetInt32("ID"),
                                        Setting_Score1 = reader.GetInt32("score_setting_first"),
                                        Setting_Score2 = reader.GetInt32("score_setting_second"),
                                        Setting_Letter_Grade = reader.GetString("score_setting_letter_grade"),
                                        Setting_GPA_Value = reader.GetInt32("score_setting_GPA_value"),
                                        Setting_Grade_System = reader.GetString("score_setting_grade_system")
                                    };
                                    setting_score_list.Add(setting_score);
                                }
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Error Database Load_Setting_Score_ToTable: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error Load_Setting_Score_ToTable: {ex.Message}");
                    return null;
                }
                return setting_score_list;
            }
        }

        //Method Update Setting Score
        public bool Update_Setting_Score(Class_Score setting_score_info)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE class_setting_score SET score_setting_first = @score_setting_first, score_setting_second = @score_setting_second, score_setting_letter_grade = @score_setting_letter_grade, score_setting_GPA_value = @score_setting_GPA_value, score_setting_grade_system = @score_setting_grade_system WHERE ID = @ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    {
                        cmd.Parameters.AddWithValue("@score_setting_first", setting_score_info.Setting_Score1);
                        cmd.Parameters.AddWithValue("@score_setting_second", setting_score_info.Setting_Score2);
                        cmd.Parameters.AddWithValue("@score_setting_letter_grade", setting_score_info.Setting_Letter_Grade);
                        cmd.Parameters.AddWithValue("@score_setting_GPA_value", setting_score_info.Setting_GPA_Value);
                        cmd.Parameters.AddWithValue("@score_setting_grade_system", setting_score_info.Setting_Grade_System);
                        cmd.Parameters.AddWithValue("@ID", setting_score_info.Setting_Score_ID);
                    }
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;

                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Error Database Update_Setting_Score: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Update_Setting_Score: {ex.Message}");
                return false;
            }
        }

        //Method Delete Setting Score
        public bool Delete_Setting_Score(int setting_score_id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM class_setting_score WHERE ID = @ID";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ID", setting_score_id);
                    
                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;

                }
            }
            catch(MySqlException ex)
            {
                Debug.WriteLine($"Error Database Delete_Setting_Score: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error Delete_Setting_Score: {ex.Message}");
                return false;
            }
        }

        //Method get setting score
        public List<Class_Score> GetSetting_Score_Info()
        {
            List<Class_Score> setting_score_value = new List<Class_Score>();
            {
                try
                {
                    using(MySqlConnection connection = new MySqlConnection(_connectionString))
                    {
                        connection.Open();
                        string query = "SELECT * FROM class_setting_score";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score setting_score = new Class_Score()
                                    {
                                        Setting_Score_ID = reader.GetInt32("ID"),
                                        Setting_Score1 = reader.GetInt32("score_setting_first"),
                                        Setting_Score2 = reader.GetInt32("score_setting_second"),
                                        Setting_Letter_Grade = reader.GetString("score_setting_letter_grade"),
                                        Setting_GPA_Value = reader.GetInt32("score_setting_GPA_value"),
                                        Setting_Grade_System = reader.GetString("score_setting_grade_system")
                                    };
                                    setting_score_value.Add(setting_score);
                                }
                            }
                        }
                    }
                }catch(MySqlException ex)
                {
                    Debug.WriteLine($"Error Database GetSetting_Score_Info {ex.Message}");
                    return null;
                }catch (Exception ex)
                {
                    Debug.WriteLine($"Error GetSetting_Score_Info {ex.Message}");
                    return null;
                }
                return setting_score_value;
            }
        }

        //Method to get total score monfri
        public List<Class_Score> GetTotalStudents_Score_BySubject(string Score_Student_ID,string Class_ID)
        {
            List<Class_Score> class_score_list = new List<Class_Score>();
            {
                try
                {
                    using(MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT class_id,subject_name,student_id,student_name,SUM(student_score) AS Total_Score,COUNT(student_score_type) AS Count_Score_Type FROM class_schedule_score_mon_fri_info where student_id = @student_id AND class_id = @class_id group by class_id,subject_name,student_id,student_name order by subject_name,student_id;";

                        using(MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@student_id", Score_Student_ID);
                            cmd.Parameters.AddWithValue("@class_id",Class_ID);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score setting_score = new Class_Score()
                                    {
                                        Score_Skill_Name = reader.GetString("subject_name"),
                                        Total_Score = reader.GetInt32("Total_Score"),
                                        Total_Count_Score_Type = reader.GetInt32("Count_Score_Type"),
                                        Total_Score_Average = (float)reader.GetInt32("Total_Score") / reader.GetInt32("Count_Score_Type")
                                    };
                                    class_score_list.Add(setting_score);
                                }
                            }
                        }
                    }
                }catch(MySqlException ex)
                {
                    Debug.WriteLine($"Error Database GetTotalStudents_Score_BySubject: {ex.Message}");
                    return null;
                }catch(Exception ex)
                {
                    Debug.WriteLine($"Error GetTotalStudents_Score_BySubject: {ex.Message}");
                    return null;
                }
                return class_score_list;
            }
        }

        //Method to get total score satsun
        public List<Class_Score> GetTotalStudents_Score_BySubject1(string Score_Student_ID, string Class_ID)
        {
            List<Class_Score> class_score_list = new List<Class_Score>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT class_id,subject_name,student_id,student_name,SUM(student_score) AS Total_Score,COUNT(student_score_type) AS Count_Score_Type FROM class_schedule_score_sat_sun_info where student_id = @student_id AND class_id = @class_id group by class_id,subject_name,student_id,student_name order by subject_name,student_id;";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@student_id", Score_Student_ID);
                            cmd.Parameters.AddWithValue("@class_id", Class_ID);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Class_Score setting_score = new Class_Score()
                                    {
                                        Score_Skill_Name = reader.GetString("subject_name"),
                                        Total_Score = reader.GetInt32("Total_Score"),
                                        Total_Count_Score_Type = reader.GetInt32("Count_Score_Type"),
                                        Total_Score_Average = (float)reader.GetInt32("Total_Score") / reader.GetInt32("Count_Score_Type")
                                    };
                                    class_score_list.Add(setting_score);
                                }
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Error Database GetTotalStudents_Score_BySubject: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error GetTotalStudents_Score_BySubject: {ex.Message}");
                    return null;
                }
                return class_score_list;
            }
        }

        //Method to get Student Rank
        public List<Class_Score> GetStudents_Rank_Info(string class_id,string class_timeshift)
        {
            List<Class_Score> class_student_ranks = new List<Class_Score>();
            {
                try
                {
                    using(MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        if(class_timeshift == "វេនសៅរ៍អាទិត្យ")
                        {
                            string query = "WITH Score_Aggregate AS (\r\n    -- Calculate Total_Score_of_subject\r\n    SELECT \r\n        student_id, \r\n        student_name,\r\n        student_gender,\r\n        stu_birthday,\r\n        subject_name,\r\n        SUM(student_score) / COUNT(DISTINCT student_score_type) AS Total_Score_of_subject\r\n    FROM class_schedule_score_sat_sun_info\r\n    GROUP BY student_id, student_name, student_gender, stu_birthday, subject_name\r\n),\r\nTotal_Score_Calculation AS (\r\n    -- Calculate Total_Score\r\n    SELECT \r\n        student_id, \r\n        student_name,\r\n        student_gender,\r\n        stu_birthday,\r\n        SUM(Total_Score_of_subject)  AS Total_Score\r\n    FROM Score_Aggregate\r\n    GROUP BY student_id, student_name, student_gender, stu_birthday\r\n),\r\nAverage_Score AS (\r\n    -- Calculate the Average\r\n    SELECT \r\n        tsc.student_id,\r\n        tsc.student_name,\r\n        tsc.student_gender,\r\n        tsc.stu_birthday,\r\n        tsc.Total_Score,\r\n        tsc.Total_Score / (SELECT COUNT(DISTINCT subject_name) FROM class_schedule_score_mon_fri_info) AS Average\r\n    FROM Total_Score_Calculation tsc\r\n)\r\n-- Assign Rank\r\nSELECT \r\n    student_id,\r\n    student_name,\r\n    student_gender,\r\n    stu_birthday,\r\n    Total_Score,\r\n    Average,\r\n    RANK() OVER (ORDER BY Total_Score DESC) AS Ranks\r\nFROM Average_Score";


                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@class_id", class_id);

                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Class_Score student_score_rank = new Class_Score()
                                        {
                                            Score_Student_ID = reader.GetString("student_id"),
                                            Score_Student_Name = reader.GetString("student_name"),
                                            Score_Student_Gender = reader.GetString("student_gender"),
                                            Score_Student_BirthDay = reader.GetString("stu_birthday"),
                                            Total_Score_Average = reader.GetFloat("Total_Score"),
                                            Average_Student = reader.GetFloat("Average"),
                                            Rank_Student = reader.GetInt32("Ranks")

                                        };
                                        class_student_ranks.Add(student_score_rank);
                                    }
                                }
                            }
                        }
                        else
                        {
                            string query = "WITH Score_Aggregate AS (\r\n    -- Calculate Total_Score_of_subject\r\n    SELECT \r\n        student_id, \r\n        student_name,\r\n        student_gender,\r\n        stu_birthday,\r\n        subject_name,\r\n        SUM(student_score) / COUNT(DISTINCT student_score_type) AS Total_Score_of_subject\r\n    FROM class_schedule_score_mon_fri_info\r\n    GROUP BY student_id, student_name, student_gender, stu_birthday, subject_name\r\n),\r\nTotal_Score_Calculation AS (\r\n    -- Calculate Total_Score\r\n    SELECT \r\n        student_id, \r\n        student_name,\r\n        student_gender,\r\n        stu_birthday,\r\n        SUM(Total_Score_of_subject)  AS Total_Score\r\n    FROM Score_Aggregate\r\n    GROUP BY student_id, student_name, student_gender, stu_birthday\r\n),\r\nAverage_Score AS (\r\n    -- Calculate the Average\r\n    SELECT \r\n        tsc.student_id,\r\n        tsc.student_name,\r\n        tsc.student_gender,\r\n        tsc.stu_birthday,\r\n        tsc.Total_Score,\r\n        tsc.Total_Score / (SELECT COUNT(DISTINCT subject_name) FROM class_schedule_score_mon_fri_info) AS Average\r\n    FROM Total_Score_Calculation tsc\r\n)\r\n-- Assign Rank\r\nSELECT \r\n    student_id,\r\n    student_name,\r\n    student_gender,\r\n    stu_birthday,\r\n    Total_Score,\r\n    Average,\r\n    RANK() OVER (ORDER BY Total_Score DESC) AS Ranks\r\nFROM Average_Score";
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@class_id", class_id);

                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Class_Score student_score_rank = new Class_Score()
                                        {
                                            Score_Student_ID = reader.GetString("student_id"),
                                            Score_Student_Name = reader.GetString("student_name"),
                                            Score_Student_Gender = reader.GetString("student_gender"),
                                            Score_Student_BirthDay = reader.GetString("stu_birthday"),
                                            Total_Score_Average = reader.GetFloat("Total_Score"),
                                            Average_Student = reader.GetFloat("Average"),
                                            Rank_Student = reader.GetInt32("Ranks")

                                        };
                                        class_student_ranks.Add(student_score_rank);
                                    }
                                }
                            }
                        }
                        
                    }
                }catch (MySqlException ex)
                {
                    Debug.WriteLine($"Error Database GetStudents_Rank_Info: {ex.Message}");
                    return null;
                }catch (Exception ex)
                {
                    Debug.WriteLine($"Error GetStudents_Rank_Info: {ex.Message}");
                    return null;
                }
                return class_student_ranks;
            }
        }

        //Method to Up Students Class
        public bool Up_Students_Class(string Semester,string Student_Year,string Up_Study_Year,string Class_State,string Student_ID)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE student_infomations SET stu_semester = @stu_semester,stu_studying_time = @stu_studying_time,stu_study_year = @stu_study_year,stu_classes = @stu_classes WHERE stu_id = @stu_id";

                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@stu_semester", Semester);
                        cmd.Parameters.AddWithValue("@stu_studying_time", Student_Year);
                        cmd.Parameters.AddWithValue("@stu_study_year", Up_Study_Year);
                        cmd.Parameters.AddWithValue("@stu_id", Student_ID);

                        // Check if Class_State is null or empty, then set it to NULL
                        cmd.Parameters.AddWithValue("@stu_classes", string.IsNullOrEmpty(Class_State) ? (object)DBNull.Value : Class_State);


                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }catch(MySqlException ex)
            {
                Debug.WriteLine($"Error Database Up_Students_Class: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error Up_Students_Class: {ex.Message}");
                return false;
            }
        }

        //Method to get class info for students transcript
        public List<Student_Info> Get_Class_Student_Transcript(int Student_ID)
        {
            List<Student_Info> class_infos_list = new List<Student_Info>();
            {


                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT c.*\r\nFROM classes c\r\nJOIN class_enrollments ce ON c.class_id = ce.class_id\r\nWHERE ce.stu_id = @stu_id";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@stu_id", Student_ID);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Student_Info class_info = new Student_Info()
                                    {
                                        Class_ID_Show = reader.GetInt32("class_id"),
                                        Class_Name = reader.GetString("class_name"),
                                        Class_In_Skill = reader.GetString("class_in_skill"),
                                        Class_In_Student_Year = reader.GetString("class_in_student_year"),
                                        Class_In_Semester = reader.GetString("class_in_semester"),
                                        Class_In_Study_Year = reader.GetString("class_in_study_year"),
                                        Current_Class_State = reader.GetString("Class_State")
                                    };
                                    class_infos_list.Add(class_info);
                                }
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Error Database Get_Class_Student_Transcript: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error Get_Class_Student_Transcript: {ex.Message}");
                    return null;
                }
                return class_infos_list;
            }
        }

        //Method to get subject, score students transcript
        public List<Class_Score> GetSubject_Score_Transcript(int class_id, string student_id)
        {
            List<Class_Score> class_infos_transcript = new List<Class_Score>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    c.class_in_study_year,
                    c.class_in_student_year,
                    c.class_in_semester,
                    cs.subject_name,
                    cs.total_subject_time,
                    cs.student_id,
                    cs.student_name,
                    SUM(cs.student_score) AS Total_Score,
                    COUNT(cs.student_score_type) AS Count_Score_Type
                FROM 
                    (
                        SELECT class_id, schedule_id, subject_name, total_subject_time, teacher_name, 
                        stu_id, student_id, student_name, student_gender, stu_birthday, student_score, 
                        student_score_type, insert_time, insert_user, update_time, update_user
                        FROM class_schedule_score_mon_fri_info

                        UNION ALL

                        SELECT class_id, schedule_id, subject_name, total_subject_time, teacher_name, 
                        stu_id, student_id, student_name, student_gender, stu_birthday, student_score, 
                        student_score_type, insert_time, insert_user, update_time, update_user
                        FROM class_schedule_score_sat_sun_info
                    ) cs
                JOIN 
                    classes c ON cs.class_id = c.class_id
                WHERE 
                    cs.student_id = @student_id 
                    AND cs.class_id = @class_id
                GROUP BY 
                    cs.class_id, c.class_in_study_year, c.class_in_student_year, 
                    c.class_in_semester, cs.subject_name, cs.student_id, cs.student_name
                ORDER BY 
                    c.class_in_semester";

                    Debug.WriteLine($"Executing SQL Query: {query}");
                    Debug.WriteLine($"Query Parameters: student_id = {student_id}, class_id = {class_id}");

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", student_id);
                        cmd.Parameters.AddWithValue("@class_id", class_id);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Debug.WriteLine("SQL Query returned no data.");
                            }

                            while (reader.Read())
                            {
                                Class_Score class_info = new Class_Score()
                                {
                                    Report_StudyYear = reader.GetString("class_in_study_year"),
                                    Report_Student_Year = reader.GetString("class_in_student_year"),
                                    Report_Study_Semester = reader.GetString("class_in_semester"),
                                    Report_Study_Subject = reader.GetString("subject_name"),
                                    Total_Score = reader.GetInt32("Total_Score"),
                                    Total_Count_Score_Type = reader.GetInt32("Count_Score_Type"),
                                    Total_Score_Average = (float)reader.GetInt32("Total_Score") / reader.GetInt32("Count_Score_Type"),
                                    Score_Skill_TotalTime = reader.GetInt32("total_subject_time")
                                };
                                class_infos_transcript.Add(class_info);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Error: {ex.Message}");
            }

            return class_infos_transcript;
        }

        //Method to get stu skill in english
        public string GetStu_SkillInfo_English(string stu_skill)
        {
            string Stu_Skill_English = ""; // Local variable to store the skill name

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT edu_skill_name_en FROM education_skill_info WHERE edu_skill_name_kh = @edu_skill_name_kh";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@edu_skill_name_kh", stu_skill);

                        using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                        {
                            if (reader.Read()) // Check if data exists
                            {
                                Stu_Skill_English = reader.GetString("edu_skill_name_en"); // Get value
                            }
                            else
                            {
                                Debug.WriteLine("No skill found for the given Khmer skill name.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database Error GetStu_SkillInfo_English: {ex.Message}");
                return "Error";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Error GetStu_SkillInfo_English: {ex.Message}");
                return "Error";
            }

            return Stu_Skill_English; 
        }

        //Method to get stu gender in english
        public string GetStu_Gender_English(string stu_gender)
        {
            string Stu_Gender_English = ""; // Local variable to store the skill name

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT Name_EN FROM gender WHERE Name_KH = @Name_KH";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name_KH", stu_gender);

                        using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                        {
                            if (reader.Read()) // Check if data exists
                            {
                                Stu_Gender_English = reader.GetString("Name_EN"); // Get value
                            }
                            else
                            {
                                Debug.WriteLine("No gender found for the given Khmer gender name.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database Error GetStu_Gender_English: {ex.Message}");
                return "Error";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Error GetStu_Gender_English: {ex.Message}");
                return "Error";
            }

            return Stu_Gender_English;
        }

        //Method to get stu birth place in english
        public string GetStu_BirthPlace_English(string stu_provice)
        {
            string Stu_Provice_English = ""; // Local variable to store the skill name

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT province_name_en FROM province_info WHERE province_name_kh = @province_name_kh";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@province_name_kh", stu_provice);

                        using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                        {
                            if (reader.Read()) // Check if data exists
                            {
                                Stu_Provice_English = reader.GetString("province_name_en"); // Get value
                            }
                            else
                            {
                                Debug.WriteLine("No province_name_en found for the given Khmer province_name_en.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database Error GetStu_BirthPlace_English: {ex.Message}");
                return "Error";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Error GetStu_BirthPlace_English: {ex.Message}");
                return "Error";
            }

            return Stu_Provice_English;
        }

        //Method to get stu degree in english
        public string GetStu_Degree_English(string stu_degree)
        {
            string Stu_Degree_English = "";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT edu_level_name_en FROM education_level_info WHERE edu_level_name_kh = @edu_level_name_kh";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@edu_level_name_kh", stu_degree);

                        using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                        {
                            if (reader.Read()) // Check if data exists
                            {
                                Stu_Degree_English = reader.GetString("edu_level_name_en"); // Get value
                            }
                            else
                            {
                                Debug.WriteLine("No edu_level_name_en found for the given Khmer edu_level_name_kh.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database Error GetStu_Degree_English: {ex.Message}");
                return "Error";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Error GetStu_Degree_English: {ex.Message}");
                return "Error";
            }

            return Stu_Degree_English;
        }

        //Method to get class for teacher Time
        public List<Class_Schedule> GetClasses_Progress(string Class_In_Study_Year,string Class_Seletecd_Date,string Class_In_Study_Timeshift,string Current_Class_State)
        {
            List<Class_Schedule> class_infos = new List<Class_Schedule>();
            {
                try
                {
                    string query = "";
                    if(Class_Seletecd_Date =="ថ្ងៃចន្ទ")
                    {
                        query = @"SELECT 
                                    cs.sd_name_teacher_mon_1 AS teacher_name,
                                    CASE 
                                        WHEN cs.sd_mon_skill_1 = cs.sd_mon_skill_2 THEN cs.sd_mon_skill_1  -- Show only one if same subject
                                        ELSE cs.sd_mon_skill_2  -- Show second subject if different
                                    END AS subject_name,
                                    CASE 
                                        WHEN cs.sd_mon_skill_1 = cs.sd_mon_skill_2 THEN cs.sd_totaltime_skill_mon_1 + cs.sd_totaltime_skill_mon_2  
                                        ELSE cs.sd_totaltime_skill_mon_2  
                                    END AS total_time_skill,
                                    cs.sd_start_time_1,
                                    cs.sd_end_time_2,
                                    CASE 
                                        WHEN cs.sd_mon_skill_1 = cs.sd_mon_skill_2 
                                        THEN HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_end_time_2) - TIME_TO_SEC(cs.sd_start_time_1) 
                                                - (TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1))
                                            )
                                        ) 
                                        ELSE HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1)
                                            )
                                        )
                                    END AS time_today,
                                    c.class_in_skill,
                                    c.class_in_student_year,
                                    c.class_in_semester,
                                    c.class_in_generation,
                                    cs.ID,
                                    cs.sd_class_id
                                FROM class_schedule_mon_fri_info cs
                                JOIN classes c ON cs.sd_class_id = c.class_id
                                WHERE 
                                    c.class_in_study_year = @class_in_study_year
                                    AND cs.sd_class_timeshift = @sd_class_timeshift 
                                    AND c.Class_State = @class_state;
                        ";
                    }
                    else if(Class_Seletecd_Date =="ថ្ងៃអង្គារ")
                    {
                        query = @"SELECT 
                                    cs.sd_name_teacher_tues_1 AS teacher_name,
                                    CASE 
                                        WHEN cs.sd_tues_skill_1 = cs.sd_tues_skill_2 THEN cs.sd_tues_skill_1  -- Show only one if same subject
                                        ELSE cs.sd_tues_skill_2  -- Show second subject if different
                                    END AS subject_name,
                                    CASE 
                                        WHEN cs.sd_totaltime_skill_tues_1 = cs.sd_totaltime_skill_tues_2 THEN cs.sd_totaltime_skill_tues_1 + cs.sd_totaltime_skill_tues_2  
                                        ELSE cs.sd_totaltime_skill_tues_2  
                                    END AS total_time_skill,
                                    cs.sd_start_time_1,
                                    cs.sd_end_time_2,
                                    CASE 
                                        WHEN cs.sd_tues_skill_1 = cs.sd_tues_skill_2 
                                        THEN HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_end_time_2) - TIME_TO_SEC(cs.sd_start_time_1) 
                                                - (TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1))
                                            )
                                        ) 
                                        ELSE HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1)
                                            )
                                        )
                                    END AS time_today,
                                    c.class_in_skill,
                                    c.class_in_student_year,
                                    c.class_in_semester,
                                    c.class_in_generation,
                                    cs.ID,
                                    cs.sd_class_id
                                FROM class_schedule_mon_fri_info cs
                                JOIN classes c ON cs.sd_class_id = c.class_id
                                WHERE 
                                    c.class_in_study_year = @class_in_study_year
                                    AND cs.sd_class_timeshift = @sd_class_timeshift 
                                    AND c.Class_State = @class_state;
                        ";
                    }
                    else if(Class_Seletecd_Date == "ថ្ងៃពុធ")
                    {
                        query = @"SELECT 
                                    cs.sd_name_teacher_wed_1 AS teacher_name,
                                    CASE 
                                        WHEN cs.sd_wed_skill_1 = cs.sd_wed_skill_2 THEN cs.sd_wed_skill_1  -- Show only one if same subject
                                        ELSE cs.sd_wed_skill_2  -- Show second subject if different
                                    END AS subject_name,
                                    CASE 
                                        WHEN cs.sd_totaltime_skill_wed_1 = cs.sd_totaltime_skill_wed_2 THEN cs.sd_totaltime_skill_wed_1 + cs.sd_totaltime_skill_wed_2  
                                        ELSE cs.sd_totaltime_skill_wed_2  
                                    END AS total_time_skill,
                                    cs.sd_start_time_1,
                                    cs.sd_end_time_2,
                                    CASE 
                                        WHEN cs.sd_wed_skill_1 = cs.sd_wed_skill_2 
                                        THEN HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_end_time_2) - TIME_TO_SEC(cs.sd_start_time_1) 
                                                - (TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1))
                                            )
                                        ) 
                                        ELSE HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1)
                                            )
                                        )
                                    END AS time_today,
                                    c.class_in_skill,
                                    c.class_in_student_year,
                                    c.class_in_semester,
                                    c.class_in_generation,
                                    cs.ID,
                                    cs.sd_class_id
                                FROM class_schedule_mon_fri_info cs
                                JOIN classes c ON cs.sd_class_id = c.class_id
                                WHERE 
                                    c.class_in_study_year = @class_in_study_year
                                    AND cs.sd_class_timeshift = @sd_class_timeshift 
                                    AND c.Class_State = @class_state;
                        ";
                    }
                    else if(Class_Seletecd_Date == "ថ្ងៃព្រហស្បត្តិ")
                    {
                        query = @"SELECT 
                                    cs.sd_name_teacher_thurs_1 AS teacher_name,
                                    CASE 
                                        WHEN cs.sd_thurs_skill_1 = cs.sd_thurs_skill_2 THEN cs.sd_thurs_skill_1  -- Show only one if same subject
                                        ELSE cs.sd_thurs_skill_2  -- Show second subject if different
                                    END AS subject_name,
                                    CASE 
                                        WHEN cs.sd_totaltime_skill_thurs_1 = cs.sd_totaltime_skill_thurs_2 THEN cs.sd_totaltime_skill_thurs_1 + cs.sd_totaltime_skill_thurs_2  
                                        ELSE cs.sd_totaltime_skill_thurs_2  
                                    END AS total_time_skill,
                                    cs.sd_start_time_1,
                                    cs.sd_end_time_2,
                                    CASE 
                                        WHEN cs.sd_thurs_skill_1 = cs.sd_thurs_skill_2 
                                        THEN HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_end_time_2) - TIME_TO_SEC(cs.sd_start_time_1) 
                                                - (TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1))
                                            )
                                        ) 
                                        ELSE HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1)
                                            )
                                        )
                                    END AS time_today,
                                    c.class_in_skill,
                                    c.class_in_student_year,
                                    c.class_in_semester,
                                    c.class_in_generation,
                                    cs.ID,
                                    cs.sd_class_id
                                FROM class_schedule_mon_fri_info cs
                                JOIN classes c ON cs.sd_class_id = c.class_id
                                WHERE 
                                    c.class_in_study_year = @class_in_study_year
                                    AND cs.sd_class_timeshift = @sd_class_timeshift 
                                    AND c.Class_State = @class_state;
                        ";
                    }
                    else if(Class_Seletecd_Date == "ថ្ងៃសុក្រ")
                    {
                        query = @"SELECT 
                                    cs.sd_name_teacher_fri_1 AS teacher_name,
                                    CASE 
                                        WHEN cs.sd_fri_skill_1 = cs.sd_fri_skill_2 THEN cs.sd_fri_skill_1  -- Show only one if same subject
                                        ELSE cs.sd_fri_skill_2  -- Show second subject if different
                                    END AS subject_name,
                                    CASE 
                                        WHEN cs.sd_totaltime_skill_fri_1 = cs.sd_totaltime_skill_fri_2 THEN cs.sd_totaltime_skill_fri_1 + cs.sd_totaltime_skill_fri_2  
                                        ELSE cs.sd_totaltime_skill_fri_2  
                                    END AS total_time_skill,
                                    cs.sd_start_time_1,
                                    cs.sd_end_time_2,
                                    CASE 
                                        WHEN cs.sd_fri_skill_1 = cs.sd_fri_skill_2 
                                        THEN HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_end_time_2) - TIME_TO_SEC(cs.sd_start_time_1) 
                                                - (TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1))
                                            )
                                        ) 
                                        ELSE HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_start_time_2) - TIME_TO_SEC(cs.sd_end_time_1)
                                            )
                                        )
                                    END AS time_today,
                                    c.class_in_skill,
                                    c.class_in_student_year,
                                    c.class_in_semester,
                                    c.class_in_generation,
                                    cs.ID,
                                    cs.sd_class_id
                                FROM class_schedule_mon_fri_info cs
                                JOIN classes c ON cs.sd_class_id = c.class_id
                                WHERE 
                                    c.class_in_study_year = @class_in_study_year
                                    AND cs.sd_class_timeshift = @sd_class_timeshift 
                                    AND c.Class_State = @class_state;
                        ";
                    }
                    else if(Class_Seletecd_Date == "ថ្ងៃសៅរ៍")
                    {
                        query = @"SELECT 
                                    cs.sd_teacher_name_sat1 AS teacher_name,
                                    CASE 
                                        WHEN cs.sd_skill_name_sat1 = cs.sd_skill_name_sat2 THEN cs.sd_skill_name_sat1  -- Show only one if same subject
                                        ELSE cs.sd_skill_name_sat2  -- Show second subject if different
                                    END AS subject_name,
                                    CASE 
                                        WHEN cs.sd_totaltime_skill_sat1 = cs.sd_totaltime_skill_sat2 THEN cs.sd_totaltime_skill_sat1 + cs.sd_totaltime_skill_sat2  
                                        ELSE cs.sd_totaltime_skill_sat2  
                                    END AS total_time_skill,
                                    cs.sd_start_time_ss1 AS sd_start_time_1,
                                    cs.sd_end_time_ss2 AS sd_end_time_2,
                                    CASE 
                                        WHEN cs.sd_skill_name_sat1 = cs.sd_skill_name_sat2 
                                        THEN HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_end_time_ss2) - TIME_TO_SEC(cs.sd_start_time_ss1) 
                                                - (TIME_TO_SEC(cs.sd_start_time_ss2) - TIME_TO_SEC(cs.sd_end_time_ss1))
                                            )
                                        ) 
                                        ELSE HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_start_time_ss2) - TIME_TO_SEC(cs.sd_end_time_ss1)
                                            )
                                        )
                                    END AS time_today,
                                    c.class_in_skill,
                                    c.class_in_student_year,
                                    c.class_in_semester,
                                    c.class_in_generation,
                                    cs.ID,
                                    cs.sd_class_id
                                FROM class_schedule_sat_sun cs
                                JOIN classes c ON cs.sd_class_id = c.class_id
                                WHERE 
                                    c.class_in_study_year = @class_in_study_year
                                    AND cs.sd_class_timeshift = @sd_class_timeshift 
                                    AND c.Class_State = @class_state;
                        ";
                    }
                    else if(Class_Seletecd_Date == "ថ្ងៃអាទិត្យ")
                    {
                        query = @"SELECT 
                                    cs.sd_teacher_name_sun1 AS teacher_name,
                                    CASE 
                                        WHEN cs.sd_skill_name_sun1 = cs.sd_skill_name_sun2 THEN cs.sd_skill_name_sun1  -- Show only one if same subject
                                        ELSE cs.sd_skill_name_sun2  -- Show second subject if different
                                    END AS subject_name,
                                    CASE 
                                        WHEN cs.sd_totaltime_skill_sun1 = cs.sd_totaltime_skill_sun2 THEN cs.sd_totaltime_skill_sun1 + cs.sd_totaltime_skill_sun2  
                                        ELSE cs.sd_totaltime_skill_sun2  
                                    END AS total_time_skill,
                                    cs.sd_start_time_ss1 AS sd_start_time_1,
                                    cs.sd_end_time_ss2 AS sd_end_time_2,
                                    CASE 
                                        WHEN cs.sd_skill_name_sat1 = cs.sd_skill_name_sat2 
                                        THEN HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_end_time_ss2) - TIME_TO_SEC(cs.sd_start_time_ss1) 
                                                - (TIME_TO_SEC(cs.sd_start_time_ss2) - TIME_TO_SEC(cs.sd_end_time_ss1))
                                            )
                                        ) 
                                        ELSE HOUR(
                                            SEC_TO_TIME(
                                                TIME_TO_SEC(cs.sd_start_time_ss2) - TIME_TO_SEC(cs.sd_end_time_ss1)
                                            )
                                        )
                                    END AS time_today,
                                    c.class_in_skill,
                                    c.class_in_student_year,
                                    c.class_in_semester,
                                    c.class_in_generation,
                                    cs.ID,
                                    cs.sd_class_id
                                FROM class_schedule_sat_sun cs
                                JOIN classes c ON cs.sd_class_id = c.class_id
                                WHERE 
                                    c.class_in_study_year = @class_in_study_year
                                    AND cs.sd_class_timeshift = @sd_class_timeshift 
                                    AND c.Class_State = @class_state;
                        ";
                    }

                    using(MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        using(MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@class_in_study_year", Class_In_Study_Year);
                            cmd.Parameters.AddWithValue("@sd_class_timeshift", Class_In_Study_Timeshift);
                            cmd.Parameters.AddWithValue("@class_state", Current_Class_State);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                int Index_N = 1;
                                bool IsAttendent = true;
                                while (reader.Read())
                                {
                                    Class_Schedule class_info = new Class_Schedule()
                                    {
                                        Index = Index_N,
                                        IsAttendent = IsAttendent,
                                        SD_Teacher_Name = reader.GetString("teacher_name"),
                                        SD_Skill_Name = reader.GetString("subject_name"),
                                        SD_TotalTime_Mon2 = reader.GetInt32("total_time_skill"), //SD_TotalTime_Mon2 For all Total time of Monday
                                        SD_Start_DateTime_MF1 = reader.GetTimeSpan("sd_start_time_1"),
                                        SD_End_DateTime_MF2 = reader.GetTimeSpan("sd_end_time_2"),
                                        TotalTime_Calculate = reader.GetInt32("time_today"),

                                        Class_In_Skill = reader.GetString("class_in_skill"),
                                        Class_In_Study_Year = reader.GetString("class_in_student_year"),
                                        Class_In_Semester = reader.GetString("class_in_semester"),
                                        Class_In_Generation = reader.GetInt32("class_in_generation"),
                                        Schedule_ID = reader.GetInt32("ID"),
                                        Class_ID_Schedule = reader.GetInt32("sd_class_id"),

                                    };
                                    Index_N++;
                                    class_infos.Add(class_info);
                                }
                            }

                        }
                    }
                }catch(MySqlException ex)
                {
                    Debug.WriteLine($"Error Database GetClasses_Progress: {ex.Message}");
                    return null;
                }catch (Exception ex)
                {
                    Debug.WriteLine($"Error GetClasses_Progress: {ex.Message}");
                    return null;
                }
                return class_infos;
            }
        }

        //Method Save Teacher Attendent
        public bool Save_TeacherAttendent(Class_Schedule teacher_attendent,string Class_In_Study_Year,string DateTime_Attendent,string Class_Seletecd_Date,string Class_In_Study_Timeshift,string Current_Class_State)
        {
            try
            {
                int TotalTime_Calculate = 0;
                if (teacher_attendent.IsAttendent == false)
                {
                    TotalTime_Calculate = 0;
                }
                else
                {
                    TotalTime_Calculate = teacher_attendent.TotalTime_Calculate;
                }
                using(MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO teacher_attendent_info(class_id, is_attendent, teacher_name, subject_name, subject_time, subject_time_today, time_start, time_stop, skill_name, stu_year, stu_semester, stu_generation, study_year, day_attendent, study_timeshift, class_state, date_attendent, text_reason, user_insert, time_insert)" +
                        "VALUES(@class_id, @is_attendent, @teacher_name, @subject_name, @subject_time, @subject_time_today, @time_start, @time_stop, @skill_name, @stu_year, @stu_semester, @stu_generation, @study_year, @day_attendent, @study_timeshift, @class_state, @date_attendent, @text_reason, @user_insert, @time_insert)";
                        
                    using(MySqlCommand cmd = new MySqlCommand(query,conn))
                    {
                        cmd.Parameters.AddWithValue("@class_id", teacher_attendent.Class_ID_Schedule);
                        cmd.Parameters.AddWithValue("@is_attendent", teacher_attendent.IsAttendent);
                        cmd.Parameters.AddWithValue("@teacher_name", teacher_attendent.SD_Teacher_Name);
                        cmd.Parameters.AddWithValue("@subject_name", teacher_attendent.SD_Skill_Name);
                        cmd.Parameters.AddWithValue("@subject_time", teacher_attendent.SD_TotalTime_Mon2);
                        cmd.Parameters.AddWithValue("@subject_time_today",TotalTime_Calculate);
                        cmd.Parameters.AddWithValue("@time_start", teacher_attendent.SD_Start_DateTime_MF1);
                        cmd.Parameters.AddWithValue("@time_stop", teacher_attendent.SD_End_DateTime_MF2);
                        cmd.Parameters.AddWithValue("@skill_name", teacher_attendent.Class_In_Skill);
                        cmd.Parameters.AddWithValue("@stu_year", teacher_attendent.Class_In_Study_Year);
                        cmd.Parameters.AddWithValue("@stu_semester", teacher_attendent.Class_In_Semester);
                        cmd.Parameters.AddWithValue("@stu_generation", teacher_attendent.Class_In_Generation);
                        cmd.Parameters.AddWithValue("@study_year", Class_In_Study_Year);
                        cmd.Parameters.AddWithValue("@day_attendent", Class_Seletecd_Date);
                        cmd.Parameters.AddWithValue("@study_timeshift", Class_In_Study_Timeshift);
                        cmd.Parameters.AddWithValue("@class_state", Current_Class_State);
                        cmd.Parameters.AddWithValue("@date_attendent", DateTime_Attendent);
                        cmd.Parameters.AddWithValue("@text_reason", teacher_attendent.Text_Reason_Attendent);
                        cmd.Parameters.AddWithValue("@user_insert", User_Create);
                        cmd.Parameters.AddWithValue("@time_insert", Create_Datetime);

                        cmd.ExecuteNonQuery();
                    }
                }
            }catch(MySqlException ex)
            {
                Debug.WriteLine($"Error Datebase Save_TeacherAttendent: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error Save_TeacherAttendent: {ex.Message}");
                return false;
            }
            return true;
        }

        //Method Check Teacher Attendent Before Insert
        public async Task<(string DateTime_Attendent1, string Class_In_Study_Year1, string Class_Seletecd_Date1, string Class_In_Study_Timeshift1, int Class_ID_Schedule1, string Current_Class_State1 )> Check_Teacher_Attendent_Info(string DateTime_Attendent, string Class_In_Study_Year, string Class_Seletecd_Date, string Class_In_Study_Timeshift, int Class_ID_Schedule, string Current_Class_State)
        {
            const string query = "SELECT date_attendent,study_year,day_attendent,study_timeshift,class_state,class_id FROM teacher_attendent_info WHERE date_attendent = @date_attendent AND study_year = @study_year AND day_attendent = @day_attendent AND study_timeshift = @study_timeshift AND class_state = @class_state AND class_id = @class_id";

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@date_attendent", DateTime_Attendent);
                    cmd.Parameters.AddWithValue("@study_year", Class_In_Study_Year);
                    cmd.Parameters.AddWithValue("@day_attendent", Class_Seletecd_Date);
                    cmd.Parameters.AddWithValue("@study_timeshift", Class_In_Study_Timeshift);
                    cmd.Parameters.AddWithValue("@class_state", Current_Class_State);
                    cmd.Parameters.AddWithValue("@class_id", Class_ID_Schedule);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (await reader.ReadAsync())
                        {
                            return (
                                DateTime_Attendent1: reader["date_attendent"].ToString(),
                                Class_In_Study_Year1: reader["study_year"].ToString(),
                                Class_Seletecd_Date1: reader["day_attendent"].ToString(),
                                Class_In_Study_Timeshift1: reader["study_timeshift"].ToString(),
                                Class_ID_Schedule1: reader.GetInt32("class_id"),
                                Current_Class_State1: reader["class_state"].ToString()
                            );
                        }
                    }

                }
            }
            return (null, null, null,null,0,null);
        }

        //Method to search date
        public List<Class_Schedule> GetDate_Search_Teacher_Attendent(string FormattedDate_SelectedDate_Search)
        {
            List<Class_Schedule> date_search_info = new List<Class_Schedule>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    // Corrected SQL query
                    string query = "SELECT DISTINCT date_attendent FROM teacher_attendent_info WHERE date_attendent LIKE @SearchPattern ORDER BY date_attendent DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Corrected parameter usage (appending % to search for matches)
                        cmd.Parameters.AddWithValue("@SearchPattern", $"%{FormattedDate_SelectedDate_Search}%");

                        using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                        {
                            while (reader.Read())
                            {
                                // Create an instance of Class_Schedule and populate the data
                                Class_Schedule schedule = new Class_Schedule
                                {
                                    DateTime_Attendent = reader.GetString("date_attendent")
                                };

                                date_search_info.Add(schedule);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database Error in GetDate_Search_Teacher_Attendent: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetDate_Search_Teacher_Attendent: {ex.Message}");
                return null;
            }

            return date_search_info;
        }

        //Method to Load data when open
        public List<Class_Schedule> LoadDate_Search_Teacher_Attendent()
        {
            List<Class_Schedule> date_search_info = new List<Class_Schedule>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    // Corrected SQL query
                    string query = "SELECT DISTINCT date_attendent FROM teacher_attendent_info ORDER BY date_attendent DESC LIMIT 31";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                        {
                            while (reader.Read())
                            {
                                // Create an instance of Class_Schedule and populate the data
                                Class_Schedule schedule = new Class_Schedule
                                {
                                    DateTime_Attendent = reader.GetString("date_attendent")
                                };

                                date_search_info.Add(schedule);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Database Error in LoadDate_Search_Teacher_Attendent: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadDate_Search_Teacher_Attendent: {ex.Message}");
                return null;
            }

            return date_search_info;
        }

        //Method to Load data by click date
        public List<Class_Schedule> Get_DataInfo_From_ClickDate(string DateTime_Attendent)
        {
            List<Class_Schedule> date_info = new List<Class_Schedule>();
            {
                try
                {
                    using(MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM teacher_attendent_info WHERE date_attendent = @date_attendent ORDER BY study_timeshift DESC";

                        using(MySqlCommand cmd = new MySqlCommand(query,conn))
                        {
                            cmd.Parameters.AddWithValue("@date_attendent", DateTime_Attendent);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                int Index_N = 1;
                                //bool IsAttendent = true;
                                while (reader.Read())
                                {
                                    Class_Schedule class_info = new Class_Schedule()
                                    {
                                        Index = Index_N,
                                        ID_Show = reader.GetInt32("ID"),
                                        IsAttendent = reader.GetBoolean("is_attendent"),
                                        SD_Teacher_Name = reader.GetString("teacher_name"),
                                        SD_Skill_Name = reader.GetString("subject_name"),
                                        SD_TotalTime_Mon2 = reader.GetInt32("subject_time"), //SD_TotalTime_Mon2 For all Total time of Monday
                                        SD_Start_DateTime_MF1 = reader.GetTimeSpan("time_start"),
                                        SD_End_DateTime_MF2 = reader.GetTimeSpan("time_stop"),
                                        TotalTime_Calculate = reader.GetInt32("subject_time_today"),

                                        Class_In_Skill = reader.GetString("skill_name"),
                                        Class_In_Study_Year_Show = reader.GetInt32("stu_year"),
                                        Class_In_Semester_Show = reader.GetInt32("stu_semester"),
                                        Class_In_Generation = reader.GetInt32("stu_generation"),
                                        Text_Reason_Attendent = reader.IsDBNull(reader.GetOrdinal("text_reason")) ? string.Empty : reader.GetString("text_reason"),

                                        SD_Class_Study_Year = reader.GetString("study_year"),
                                        Class_Seletecd_Date = reader.GetString("day_attendent"),
                                        SD_Class_TimeShift = reader.GetString("study_timeshift"),
                                        Current_Class_State = reader.GetString("class_state"),
                                        DateTime_Attendent = reader.GetString("date_attendent"),

                                        Schedule_ID = reader.GetInt32("ID"),
                                        Class_ID_Schedule = reader.GetInt32("class_id"),

                                    };
                                    Index_N++;
                                    date_info.Add(class_info);
                                }
                            }
                        }
                    }
                }catch(MySqlException ex)
                {
                    Debug.WriteLine($"Error Database Get_DataInfo_From_ClickDate: {ex.Message}");
                    return null;
                }catch (Exception ex)
                {
                    Debug.WriteLine($"Error Get_DataInfo_From_ClickDate: {ex.Message}");
                    return null;
                }
            }return date_info;
        }

        //Method to delete data by click date
        public bool Delete_DataClick_Date(string DateTime_Attendent)
        {
            try
            {
                using(MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM teacher_attendent_info WHERE date_attendent = @date_attendent";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@date_attendent", DateTime_Attendent);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Optionally, you can check if any rows were affected to confirm the delete happened
                    return rowsAffected > 0;
  
                }

            }catch(MySqlException ex)
            {
                Debug.WriteLine($"Error Database Delete_DataClick_Date: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error Delete_DataClick_Date: {ex.Message}");
                return false;
            }
        }

        //Method to update data
        public bool Update_Teacher_Info_Attendent_Info(Class_Schedule update_info)
        {
            try
            {
                using(MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE teacher_attendent_info SET is_attendent = @is_attendent, subject_time_today = @subject_time_today,text_reason = @text_reason,user_update = @user_update,time_update = @time_update WHERE ID = @ID";

                    using(MySqlCommand cmd = new MySqlCommand(query,conn))
                    {
                        cmd.Parameters.AddWithValue("@is_attendent", update_info.IsAttendent);
                        cmd.Parameters.AddWithValue("@subject_time_today", update_info.TotalTime_Calculate);
                        cmd.Parameters.AddWithValue("@text_reason", update_info.Text_Reason_Attendent);
                        cmd.Parameters.AddWithValue("@user_update", User_Update);
                        cmd.Parameters.AddWithValue("@time_update", Update_Datetime);
                        cmd.Parameters.AddWithValue("@ID", update_info.ID_Show);

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }catch(MySqlException ex)
            {
                Debug.WriteLine($"Error Database Update_Teacher_Info_Attendent_Info: {ex.Message}");
                return false;
            }catch(Exception ex)
            {
                Debug.WriteLine($"Error Update_Teacher_Info_Attendent_Info: {ex.Message}");
                return false;
            }       
        }       

        //Method to get MissAttendent
        public List<Class_Schedule> Get_MissAttendent(string FormattedDate_SelectedDate_Search_Attendent,bool IsAttendent)
        {
            List<Class_Schedule> data_info = new List<Class_Schedule>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT date_attendent,teacher_name,is_attendent,text_reason,subject_name,subject_time,time_start,time_stop,skill_name,stu_year,stu_semester,stu_generation,study_timeshift FROM teacher_attendent_info WHERE is_attendent = @is_attendent AND date_attendent LIKE @SearchPattern";

                        using(MySqlCommand cmd = new MySqlCommand(query,conn))
                        {
                            cmd.Parameters.AddWithValue("@SearchPattern", $"%{FormattedDate_SelectedDate_Search_Attendent}%");
                            cmd.Parameters.AddWithValue("@is_attendent", IsAttendent);
                            using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                            {
                                while (reader.Read())
                                {
                                    // Create an instance of Class_Schedule and populate the data
                                    Class_Schedule miss_attendent = new Class_Schedule
                                    {
                                        
                                        IsAttendent = reader.GetBoolean("is_attendent"),
                                        SD_Teacher_Name = reader.GetString("teacher_name"),
                                        SD_Skill_Name = reader.GetString("subject_name"),
                                        SD_TotalTime_Mon2 = reader.GetInt32("subject_time"), //SD_TotalTime_Mon2 For all Total time of Monday
                                        SD_Start_DateTime_MF1 = reader.GetTimeSpan("time_start"),
                                        SD_End_DateTime_MF2 = reader.GetTimeSpan("time_stop"),

                                        Class_In_Skill = reader.GetString("skill_name"),
                                        Class_In_Study_Year_Show = reader.GetInt32("stu_year"),
                                        Class_In_Semester_Show = reader.GetInt32("stu_semester"),
                                        Class_In_Generation = reader.GetInt32("stu_generation"),
                                        Text_Reason_Attendent = reader.IsDBNull(reader.GetOrdinal("text_reason")) ? string.Empty : reader.GetString("text_reason"),
                                        SD_Class_TimeShift = reader.GetString("study_timeshift"),     
                                        DateTime_Attendent = reader.GetString("date_attendent"),
                                    };

                                    data_info.Add(miss_attendent);
                                }
                            }

                        }
                    }
                }catch(MySqlException ex)
                {
                    Debug.WriteLine($"Error Database Get_MissAttendent: {ex.Message}");
                    return null;
                }catch(Exception ex)
                {
                    Debug.WriteLine($"Error Get_MissAttendent: {ex.Message}");
                    return null;
                }       
            }
            return data_info;
        }

        //Method to Load MissAttendent
        public List<Class_Schedule> GetLoad_Data_Teacher_MissAttendent(bool IsAttendent)
        {
            List<Class_Schedule> data_info = new List<Class_Schedule>();
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT date_attendent,teacher_name,is_attendent,text_reason,subject_name,subject_time,time_start,time_stop,skill_name,stu_year,stu_semester,stu_generation,study_timeshift FROM teacher_attendent_info WHERE is_attendent = @is_attendent ORDER BY date_attendent DESC LIMIT 31";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@is_attendent", IsAttendent);
                            using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                            {
                                while (reader.Read())
                                {
                                    // Create an instance of Class_Schedule and populate the data
                                    Class_Schedule miss_attendent = new Class_Schedule
                                    {

                                        IsAttendent = reader.GetBoolean("is_attendent"),
                                        SD_Teacher_Name = reader.GetString("teacher_name"),
                                        SD_Skill_Name = reader.GetString("subject_name"),
                                        SD_TotalTime_Mon2 = reader.GetInt32("subject_time"), //SD_TotalTime_Mon2 For all Total time of Monday
                                        SD_Start_DateTime_MF1 = reader.GetTimeSpan("time_start"),
                                        SD_End_DateTime_MF2 = reader.GetTimeSpan("time_stop"),

                                        Class_In_Skill = reader.GetString("skill_name"),
                                        Class_In_Study_Year_Show = reader.GetInt32("stu_year"),
                                        Class_In_Semester_Show = reader.GetInt32("stu_semester"),
                                        Class_In_Generation = reader.GetInt32("stu_generation"),
                                        Text_Reason_Attendent = reader.IsDBNull(reader.GetOrdinal("text_reason")) ? string.Empty : reader.GetString("text_reason"),
                                        SD_Class_TimeShift = reader.GetString("study_timeshift"),
                                        DateTime_Attendent = reader.GetString("date_attendent"),
                                    };

                                    data_info.Add(miss_attendent);
                                }
                            }

                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Error Database Get_MissAttendent: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error Get_MissAttendent: {ex.Message}");
                    return null;
                }
            }
            return data_info;
        }

        //Method to load DateTime Subject
        public List<Class_Schedule> GetTotal_Subject_Time_Info(int Class_ID_Schedule)
        {
            List<Class_Schedule> data_info = new List<Class_Schedule>();
            {
                try
                {
                    using(MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT \r\n    teacher_name,subject_name,subject_time,\r\n    SUM(subject_time_today) AS total_subject_time,\r\n    (subject_time - SUM(subject_time_today)) AS count_total,\r\n    study_timeshift,time_start,time_stop,skill_name,stu_year,stu_semester,stu_generation\r\nFROM teacher_attendent_info \r\nWHERE class_id = @class_id \r\nGROUP BY subject_name, subject_time order by stu_year desc";

                        using(MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@class_id", Class_ID_Schedule);

                            using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                            {
                                while (reader.Read())
                                {
                                    // Create an instance of Class_Schedule and populate the data
                                    Class_Schedule time_subject = new Class_Schedule
                                    {
                                        SD_Teacher_Name = reader.GetString("teacher_name"),
                                        SD_Skill_Name = reader.GetString("subject_name"),
                                        SD_TotalTime_Mon2 = reader.GetInt32("subject_time"), //SD_TotalTime_Mon2 For all Total time of Monday
                                        SD_Start_DateTime_MF1 = reader.GetTimeSpan("time_start"),
                                        SD_End_DateTime_MF2 = reader.GetTimeSpan("time_stop"),

                                        Class_In_Skill = reader.GetString("skill_name"),
                                        Class_In_Study_Year_Show = reader.GetInt32("stu_year"),
                                        Class_In_Semester_Show = reader.GetInt32("stu_semester"),
                                        Class_In_Generation = reader.GetInt32("stu_generation"),
                                        SD_Class_TimeShift = reader.GetString("study_timeshift"),

                                        Number_of_Hours_Current = reader.GetInt32("total_subject_time"),
                                        Number_of_Hours_left = reader.GetInt32("count_total")
                                    };

                                    data_info.Add(time_subject);
                                }
                            }
                        }
                    }
                }catch(MySqlException ex)
                {
                    Debug.WriteLine($"Error Database GetTotal_Subject_Time_Info: {ex.Message}");
                    return null;
                }catch (Exception ex)
                {
                    Debug.WriteLine($"Error GetTotal_Subject_Time_Info: {ex.Message}");
                    return null;
                }
                return data_info;
            }
        }

        //Method load GetDateTime_Info_BySubject
        public List<Class_Schedule> GetDateTime_Info_BySubject(string SD_Skill_Name,int SD_TotalTime_Mon2,string Class_In_Skill,int Class_In_Study_Year_Show,int Class_In_Semester_Show)
        {
            List<Class_Schedule> data_datetime = new List<Class_Schedule>();
            {
                try
                {
                    using(MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT date_attendent,day_attendent,subject_time_today,is_attendent,text_reason FROM teacher_attendent_info WHERE skill_name = @skill_name AND subject_time = @subject_time AND subject_name = @subject_name AND stu_year = @stu_year AND stu_semester = @stu_semester";

                        using(MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@skill_name", Class_In_Skill);
                            cmd.Parameters.AddWithValue("@subject_time", SD_TotalTime_Mon2);
                            cmd.Parameters.AddWithValue("@subject_name", SD_Skill_Name);
                            cmd.Parameters.AddWithValue("@stu_year", Class_In_Study_Year_Show);
                            cmd.Parameters.AddWithValue("@stu_semester", Class_In_Semester_Show);

                            using (MySqlDataReader reader = cmd.ExecuteReader()) // Execute query
                            {
                                while (reader.Read())
                                {
                                    // Create an instance of Class_Schedule and populate the data
                                    Class_Schedule data_time_subject = new Class_Schedule
                                    {
                                        DateTime_Attendent = reader.GetString("date_attendent"),
                                        Class_Seletecd_Date = reader.GetString("day_attendent"),
                                        TotalTime_Calculate = reader.GetInt32("subject_time_today"), //SD_TotalTime_Mon2 For all Total time of Monday
                                        IsAttendent = reader.GetBoolean("is_attendent"),
                                        Text_Reason_Attendent = reader.IsDBNull(reader.GetOrdinal("text_reason")) ? string.Empty : reader.GetString("text_reason")

                                    };

                                    data_datetime.Add(data_time_subject);
                                }
                            }

                        }
                    }
                }catch(MySqlException ex)
                {
                    Debug.WriteLine($"Error Database GetDateTime_Info_BySubject: {ex.Message}");
                    return null;
                }catch(Exception ex)
                {
                    Debug.WriteLine($"Error GetDateTime_Info_BySubject: {ex.Message}");
                    return null;
                }
                return data_datetime;
            }
        }
    }
}

