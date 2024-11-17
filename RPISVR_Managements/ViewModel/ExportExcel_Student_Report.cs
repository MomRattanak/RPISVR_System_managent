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
    public class ExportExcel_Student_Report
    {
        public static void ExportToExcel(IEnumerable<Student_Info> selectedStudents, string Education_Level_Text)
        {
            Debug.WriteLine("Excel OK");
            // Path to save the Excel file
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"ទិន្នន័យសិស្សនិស្សិត.xlsx");

            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet
                var worksheet = workbook.Worksheets.Add($"ទិន្នន័យសិស្សនិស្សិត");

                
                // Header
                worksheet.Cell(1, 1).Value = $"កម្រិតសិក្សា៖ {Education_Level_Text}";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(1, 1).Style.Font.FontSize = 12;
                worksheet.Cell(1, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(1, 1, 1, 13).Merge();

                // Sub-header
                //worksheet.Cell(2, 1).Value = "Generated on: " + System.DateTime.Now.ToString("dd MMM yyyy");
                //worksheet.Cell(2, 1).Style.Font.FontSize = 10; // Set font size
                //worksheet.Cell(2, 1).Style.Font.FontName = "Arial"; // Set a different font family for sub-header
                //worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; // Align left

                //Header Row
                worksheet.Cell(3, 1).Value = "ល.រ";
                worksheet.Cell(3, 2).Value = "គោត្តនាម-នាម";
                worksheet.Cell(3, 3).Value = "អក្សរឡាតាំង";
                worksheet.Cell(3, 4).Value = "ភេទ";
                worksheet.Cell(3, 5).Value = "ថ្ងៃខែឆ្នាំកំណើត";
                worksheet.Cell(3, 6).Value = "លេខទូរស័ព្ទ";
                worksheet.Cell(3, 7).Value = "ប្រភេទសិស្ស";
                worksheet.Cell(3, 8).Value = "កម្រិតសិក្សា";
                worksheet.Cell(3, 9).Value = "ជំនាញ";
                worksheet.Cell(3, 10).Value = "ឆ្នាំសិក្សា";
                worksheet.Cell(3, 11).Value = "វេនសិក្សា";
                worksheet.Cell(3, 12).Value = "ប្រភេទសិក្សា";
                worksheet.Cell(3, 13).Value = "វេនសិក្សា";

                // Apply styling to the header
                var headerRange = worksheet.Range(3, 1, 3, 13);
                headerRange.Style.Font.FontSize = 11;
                headerRange.Style.Font.FontName = "Khmer Muol";
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Data to Row
                int row = 4;
                int index = 1;
                foreach (var student in selectedStudents)
                {
                    

                    worksheet.Cell(row, 1).Value = index;
                    worksheet.Cell(row, 1).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 2).Value = student.Full_Name_KH;
                    worksheet.Cell(row, 2).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 3).Value = student.Full_Name_EN; 
                    worksheet.Cell(row, 3).Style.Font.FontName = "Times New Roman";
                    worksheet.Cell(row, 4).Value = student.Stu_Gender;
                    worksheet.Cell(row, 4).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 5).Value = student.Stu_BirthdayDateOnly;
                    worksheet.Cell(row, 5).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 6).Value = $"0{student.Stu_PhoneNumber}";
                    worksheet.Cell(row, 6).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 7).Value = student.Stu_StatePoor;
                    worksheet.Cell(row, 7).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 8).Value = student.Stu_EducationLevels;
                    worksheet.Cell(row, 8).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 9).Value = student.Stu_EducationSubjects;
                    worksheet.Cell(row, 9).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 10).Value = student.Stu_StudyYear;
                    worksheet.Cell(row, 10).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 11).Value = student.Stu_StudyTimeShift;
                    worksheet.Cell(row, 11).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 12).Value = student.Stu_EducationType;
                    worksheet.Cell(row, 12).Style.Font.FontName = "Khmer OS Siemreap"; 
                    worksheet.Cell(row, 13).Value = student.Stu_StudyTimeShift;
                    worksheet.Cell(row, 13).Style.Font.FontName = "Khmer OS Siemreap";
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
