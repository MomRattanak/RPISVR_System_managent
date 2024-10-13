using System;
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

namespace RPISVR_Managements.Model
{
    public class DatabaseConnection
    {
        private string _connectionString;
        private MySqlConnection _dbConnection;

        public DatabaseConnection()
        {
            _connectionString = "Server=127.0.0.1;Port=3306;Database=rpisvr_system;User ID=root;Password=;";

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

        //
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

                    string query = "INSERT INTO student_infomations(ID,stu_id, stu_firstname_kh, stu_lastname_kh, stu_firstname_en, stu_lastname_en, stu_birthday_dateonly, stu_gender, stu_state_family, stu_education_level, stu_education_subject, stu_study_time_shift, stu_education_types, stu_study_year, stu_semester, stu_phone_number, stu_nation_id, stu_studying_time, stu_jobs, stu_school, stu_birth_province, stu_birth_distric, stu_birth_commune, stu_birth_village, stu_live_province, stu_live_distric, stu_live_commune, stu_live_village, stu_mother_name, stu_mother_job, stu_mother_phone_number, stu_father_name, stu_father_job, stu_father_phone_number, stu_image_yes_no, stu_image_source, stu_image_total_big, stu_image_total_small, stu_image_degree_yes_no, stu_image_degree_source, stu_image_birth_cert_yes_no, stu_image_birth_cert_source, stu_image_id_nation_yes_no, stu_image_id_nation_source, stu_image_poor_card_yes_no, stu_image_poor_card_source, stu_insert_by_id, stu_insert_datetime, stu_insert_info, stu_update_by_id, stu_update_datetime, stu_update_info, stu_delete_by_id, stu_delete_datetime, stu_delete_info) " +
                        "VALUES(@id,@stu_id, @stu_firstname_kh, @stu_lastname_kh, @stu_firstname_en, @stu_lastname_en, @stu_birthday_dateonly, @stu_gender, @stu_state_family, @stu_education_level, @stu_education_subject, " +
                        "@stu_study_time_shift, @stu_education_types, @stu_study_year, @stu_semester, @stu_phone_number, @stu_nation_id, @stu_studying_time, @stu_jobs, @stu_school, @stu_birth_province, @stu_birth_distric, @stu_birth_commune, @stu_birth_village, " +
                        "@stu_live_province, @stu_live_distric, @stu_live_commune, @stu_live_village, @stu_mother_name, @stu_mother_job, @stu_mother_phone_number, @stu_father_name, @stu_father_job, @stu_father_phone_number, @stu_image_yes_no, @stu_image_source, @stu_image_total_big, @stu_image_total_small, " +
                        "@stu_image_degree_yes_no, @stu_image_degree_source, @stu_image_birth_cert_yes_no, @stu_image_birth_cert_source, @stu_image_id_nation_yes_no, @stu_image_id_nation_source, @stu_image_poor_card_yes_no, @stu_image_poor_card_source, @stu_insert_by_id, @stu_insert_datetime, @stu_insert_info, " +
                        "@stu_update_by_id, @stu_update_datetime, @stu_update_info, @stu_delete_by_id, @stu_delete_datetime, @stu_delete_info)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@id", "ID");
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
        public List<Student_Info> GetStudents_Info(int page, int pageSize)
        {
            List<Student_Info> students_info = new List<Student_Info>();
            try
            {
                int offset = (page - 1) * pageSize; // Calculate the offset
                string query = "SELECT ID, stu_id, stu_firstname_kh, stu_lastname_kh, stu_firstname_en, stu_lastname_en, " +
                       "stu_birthday_dateonly, stu_gender, stu_state_family, stu_education_level, stu_education_subject, stu_study_time_shift, " +
                       "stu_education_types, stu_study_year, stu_semester, stu_phone_number, stu_nation_id, stu_studying_time, stu_jobs, stu_school, " +
                       "stu_birth_province, stu_birth_distric, stu_birth_commune, stu_birth_village, stu_live_province, stu_live_distric, stu_live_commune, stu_live_village, " +
                       "stu_mother_name, stu_mother_job, stu_mother_phone_number, stu_father_name, stu_father_job, stu_father_phone_number, stu_image_yes_no, stu_image_source, " +
                       "stu_image_total_big, stu_image_total_small, stu_image_degree_yes_no, stu_image_degree_source, stu_image_birth_cert_yes_no, stu_image_birth_cert_source, " +
                       "stu_image_id_nation_yes_no, stu_image_id_nation_source, stu_image_poor_card_yes_no, stu_image_poor_card_source, stu_insert_by_id, stu_insert_datetime,stu_insert_info, " +
                       "stu_update_by_id, stu_update_datetime,stu_update_info,stu_delete_by_id,stu_delete_datetime,stu_delete_info FROM student_infomations ORDER BY stu_id DESC LIMIT @pageSize OFFSET @offset";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@offset", offset);

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
                        "stu_image_id_nation_yes_no = @stu_image_id_nation_yes_no, stu_image_id_nation_source = @stu_image_id_nation_source, stu_image_poor_card_yes_no = @stu_image_poor_card_yes_no, stu_image_poor_card_source = @stu_image_poor_card_source, stu_update_by_id = @stu_update_by_id, stu_update_datetime = @stu_update_datetime, stu_update_info = @stu_update_info WHERE stu_id = @stu_id";

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
                    string query = "INSERT INTO province_info (PV_ID, province_name_kh, province_name_en) VALUES(@PV_ID, @province_name_kh, @province_name_en)";

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
                    
                    string query = "SELECT PV_ID, province_name_kh, province_name_en FROM province_info ORDER BY PV_ID DESC";
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
                    string query = "UPDATE province_info SET province_name_kh = @province_name_kh, province_name_en = @province_name_en WHERE PV_ID = @PV_ID";

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

                string query = "SELECT MAX(province_id) AS P_ID, MAX(PV_ID) AS Last_PV_ID FROM province_info";
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
                    string query = "DELETE FROM province_info WHERE PV_ID = @pv_id";

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
                string query = "SELECT province_id, province_name_kh FROM province_info";
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
                    string query = "INSERT INTO district_info (DS_ID, district_name_kh, district_name_en, province_id) VALUES(@DS_ID, @district_name_kh, @district_name_en, @province_id)";

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
                        "FROM district_info d  " +
                        "JOIN province_info p ON d.province_id = p.province_id " +
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
                    
                    string query = "UPDATE district_info SET district_name_kh = @district_name_kh, district_name_en = @district_name_en, province_id = @province_id WHERE DS_ID = @DS_ID";

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

                string query = "SELECT MAX(district_id) AS D_ID, MAX(DS_ID) AS Last_DS_ID FROM district_info";
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
                    string query = "DELETE FROM district_info WHERE DS_ID = @DS_ID";

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
                string query = "SELECT district_id, district_name_kh FROM district_info WHERE province_id =@ProvinceID";

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
                    string query = "INSERT INTO commune_info (CM_ID, commune_name_kh, commune_name_en, district_id) VALUES(@CM_ID, @commune_name_kh, @commune_name_en, @district_id)";

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
                      "FROM commune_info c " +
                      "JOIN district_info d ON c.district_id = d.district_id " +
                      "JOIN province_info p ON d.province_id = p.province_id " +
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

                    string query = "UPDATE commune_info SET commune_name_kh = @commune_name_kh, commune_name_en = @commune_name_en, district_id = @district_id WHERE CM_ID = @CM_ID";

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

                string query = "SELECT MAX(commune_id) AS C_ID, MAX(CM_ID) AS Last_CM_ID FROM commune_info";
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
                    string query = "DELETE FROM commune_info WHERE CM_ID = @CM_ID";

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

    }
}
