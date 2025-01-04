using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Spreadsheet;
using RPISVR_Managements.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.ViewModel
{
    public class Export_Excel_Students_In_Class
    {
        public static void ExportToExcel(IEnumerable<Student_Info> selectedStudent_inClass,string class_id,string class_name,string class_in_skill,string class_in_level,string class_in_study_year,string class_in_student_year,string class_in_semester,string class_in_generation,string class_study_time_shift,string class_in_study_type)
        {
            Debug.WriteLine("Student in class export to Excel Success.");

            // Path to save the Excel file
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"ទិន្នន័យសិស្សនិស្សិតថ្នាក់_{class_name}.xlsx");

            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet
                var worksheet = workbook.Worksheets.Add(class_name);

                //Header Name
                int hearder_name = 1;
                worksheet.Cell(hearder_name, 1).Value = $"បញ្ជីរាយនាមសិស្សនិស្សិត";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_name, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_name, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_name, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_name, 1, hearder_name, 7).Merge();

                // Header1
                int hearder_1 = 2;
                worksheet.Cell(hearder_1, 1).Value = $"ថ្នាក់៖ {class_name} ឆ្នាំសិក្សា៖ {class_in_study_year} កម្រិតសិក្សា៖ {class_in_level}";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_1, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_1, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_1, 1, hearder_1, 7).Merge();

                // Header2
                int hearder_2 = 3;
                worksheet.Cell(hearder_2, 1).Value = $"ជំនាញ៖ {class_in_skill} ឆ្នាំទី៖ {class_in_student_year} ឆមាស៖ {class_in_semester} ជំនាន់ទី៖ {class_in_generation}";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_2, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_2, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_2, 1, hearder_2, 7).Merge();

                // Header3
                int hearder_3 = 4;
                worksheet.Cell(hearder_3, 1).Value = $"{class_in_study_type} {class_study_time_shift}";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_3, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_3, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_3, 1, hearder_3, 7).Merge();


                //Header Row
                int header_row = 6;
                worksheet.Cell(header_row, 1).Value = "ល.រ";
                worksheet.Cell(header_row, 2).Value = "គោត្តនាម-នាម";
                worksheet.Cell(header_row, 3).Value = "អក្សរឡាតាំង";
                worksheet.Cell(header_row, 4).Value = "ភេទ";
                worksheet.Cell(header_row, 5).Value = "ថ្ងៃខែឆ្នាំកំណើត";
                worksheet.Cell(header_row, 6).Value = "លេខទូរស័ព្ទ";
                worksheet.Cell(header_row, 7).Value = "ប្រភេទសិស្ស";

                // Apply styling to the header
                var headerRange = worksheet.Range(header_row, 1, header_row, 7);
                headerRange.Style.Font.FontSize = 11;
                headerRange.Style.Font.FontName = "Khmer Muol";
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Data to Row
                int row = 7;
                int index = 1;
                foreach (var student_inclass in selectedStudent_inClass)
                {
                    worksheet.Cell(row, 1).Value = index;
                    worksheet.Cell(row, 1).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 2).Value = student_inclass.Full_Name_KH;
                    worksheet.Cell(row, 2).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 3).Value = student_inclass.Full_Name_EN;
                    worksheet.Cell(row, 3).Style.Font.FontName = "Times New Roman";
                    worksheet.Cell(row, 4).Value = student_inclass.Stu_Gender;
                    worksheet.Cell(row, 4).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 5).Value = student_inclass.Stu_BirthdayDateShow;
                    worksheet.Cell(row, 5).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 6).Value = $"0{student_inclass.Stu_PhoneNumber}";
                    worksheet.Cell(row, 6).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 7).Value = student_inclass.Stu_StatePoor;
                    worksheet.Cell(row, 7).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    row++;
                    index++;
                }
                    // Adjust column widths to fit content
                    worksheet.Columns().AdjustToContents();
                // Save the workbook
                workbook.SaveAs(filePath);
            }
            // Open the file after creation
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
