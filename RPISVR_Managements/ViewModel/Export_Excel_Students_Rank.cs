using ClosedXML.Excel;
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
    public class Export_Excel_Students_Rank
    {
        public static void ExportToExcel(IEnumerable<Class_Score> Student_Rank_Info, string Class_Name, string Class_In_Skill, string Class_In_Study_Timeshift, string Class_In_Level, string Class_In_Study_Year, string Class_In_Student_Year, string Class_In_Semester, string Class_In_Generation, string Class_In_Study_Type)
        {
            Debug.WriteLine("Student Rank in class export to Excel Success.");

            // Path to save the Excel file
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"តារាងចំណាត់ថ្នាក់និស្សិត_{Class_In_Skill}_{Class_In_Study_Year}.xlsx");

            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet
                var worksheet = workbook.Worksheets.Add(Class_Name);

                //Header Name
                int hearder_name = 1;
                worksheet.Cell(hearder_name, 1).Value = $"តារាងចំណាត់ថ្នាក់និស្សិត";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_name, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_name, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_name, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_name, 1, hearder_name, 11).Merge();

                // Header1
                int hearder_1 = 2;
                worksheet.Cell(hearder_1, 1).Value = $"ថ្នាក់៖ {Class_Name} ឆ្នាំសិក្សា៖ {Class_In_Study_Year} កម្រិតសិក្សា៖ {Class_In_Level}";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_1, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_1, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_1, 1, hearder_1, 11).Merge();

                // Header2
                int hearder_2 = 3;
                worksheet.Cell(hearder_2, 1).Value = $"ជំនាញ៖ {Class_In_Skill} ឆ្នាំទី៖ {Class_In_Student_Year} ឆមាស៖ {Class_In_Semester} ជំនាន់ទី៖ {Class_In_Generation}";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_2, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_2, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_2, 1, hearder_2, 11).Merge();

                // Header3
                int hearder_3 = 4;
                worksheet.Cell(hearder_3, 1).Value = $"{Class_In_Study_Type} {Class_In_Study_Timeshift}";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_3, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_3, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_3, 1, hearder_3, 11).Merge();


                //Header Row
                int header_row = 6;
                worksheet.Cell(header_row, 1).Value = "ល.រ";
                worksheet.Cell(header_row, 2).Value = "អត្តលេខ";
                worksheet.Cell(header_row, 3).Value = "គោត្តនាម-នាម";
                worksheet.Cell(header_row, 4).Value = "ភេទ";
                worksheet.Cell(header_row, 5).Value = "ថ្ងៃខែឆ្នាំកំណើត";
                worksheet.Cell(header_row, 6).Value = "ពិន្ទុសរុប";
                worksheet.Cell(header_row, 7).Value = "មធ្យមភាគ";
                worksheet.Cell(header_row, 8).Value = "ចំណាត់ថ្នាក់";
                worksheet.Cell(header_row, 9).Value = "និទ្ទេស";
                worksheet.Cell(header_row, 10).Value = "កម្រិត";
                worksheet.Cell(header_row, 11).Value = "ស្ថានភាព";

                // Apply styling to the header
                var headerRange = worksheet.Range(header_row, 1, header_row, 11);
                headerRange.Style.Font.FontSize = 11;
                headerRange.Style.Font.FontName = "Khmer Muol";
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Data to Row
                int row = 7;
                int index = 1;
                foreach (var student_inclass in Student_Rank_Info)
                {
                    worksheet.Cell(row, 1).Value = index;
                    worksheet.Cell(row, 1).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 2).Value = student_inclass.Score_Student_ID;
                    worksheet.Cell(row, 2).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 3).Value = student_inclass.Score_Student_Name;
                    worksheet.Cell(row, 3).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 4).Value = student_inclass.Score_Student_Gender;
                    worksheet.Cell(row, 4).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 5).Value = student_inclass.Score_Student_BirthDay;
                    worksheet.Cell(row, 5).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 6).Value = $"{student_inclass.Total_Score_Average}";
                    worksheet.Cell(row, 6).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 7).Value = student_inclass.Average_Student;
                    worksheet.Cell(row, 7).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 8).Value = student_inclass.Rank_Student;
                    worksheet.Cell(row, 8).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 9).Value = student_inclass.Grade_Letter;
                    worksheet.Cell(row, 9).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 10).Value = student_inclass.Grade_System;
                    worksheet.Cell(row, 10).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 11).Value = student_inclass.Student_Pass_State;
                    worksheet.Cell(row, 11).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
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
