using ClosedXML.Excel;
using RPISVR_Managements.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.ViewModel
{
    public class Export_Excel_Curriculum_Info
    {
        public static void ExportToExcel(IEnumerable<Curriculum_Info> curriculum_Infos,string curriculum_skill_select,string curriculum_level_select,string curriculum_study_year_select)
        {
            // Path to save the Excel file
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"កម្មវិធីសិក្សាជំនាញ{curriculum_skill_select}_កម្រិត{curriculum_level_select}_ឆ្នាំទី{curriculum_study_year_select}.xlsx");

            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet
                var worksheet = workbook.Worksheets.Add(curriculum_skill_select);

                //Header Name
                int hearder_name = 1;
                worksheet.Cell(hearder_name, 1).Value = $"កម្មវិធីសិក្សា";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_name, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_name, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_name, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_name, 1, hearder_name, 8).Merge();

                // Header1
                int hearder_1 = 2;
                worksheet.Cell(hearder_1, 1).Value = $"ជំនាញ៖ {curriculum_skill_select}";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_1, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_1, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_1, 1, hearder_1, 8).Merge();

                // Header2
                int hearder_2 = 3;
                worksheet.Cell(hearder_2, 1).Value = $"កម្រិត៖ {curriculum_level_select} ឆ្នាំទី៖ {curriculum_study_year_select}";
                //worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(hearder_2, 1).Style.Font.FontSize = 12;
                worksheet.Cell(hearder_2, 1).Style.Font.FontName = "Khmer Muol";
                worksheet.Cell(hearder_2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center the text
                worksheet.Range(hearder_2, 1, hearder_2, 8).Merge();

                //Header Row
                int header_row = 5;
                worksheet.Cell(header_row, 1).Value = "ល.រ";
                worksheet.Cell(header_row, 2).Value = "ឆ្នាំទី";
                worksheet.Cell(header_row, 3).Value = "ឆមាស";
                worksheet.Cell(header_row, 4).Value = "ឈ្មោះមុខវិជ្ជា (ខ្មែរ)";
                worksheet.Cell(header_row, 5).Value = "ឈ្មោះមុខវិជ្ជា (ឡាតាំង)";
                worksheet.Cell(header_row, 6).Value = "គ្រូបច្ចេកទេស";
                worksheet.Cell(header_row, 7).Value = "ចំនួនម៉ោង";
                worksheet.Cell(header_row, 8).Value = "ពិន្ទុ";

                // Apply styling to the header
                var headerRange = worksheet.Range(header_row, 1, header_row, 8);
                headerRange.Style.Font.FontSize = 11;
                headerRange.Style.Font.FontName = "Khmer Muol";
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Data to Row
                int row = 6;
                int index = 1;

                foreach (var curriculum in curriculum_Infos)
                {
                    worksheet.Cell(row, 1).Value = index;
                    worksheet.Cell(row, 1).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 2).Value = int.Parse($"{curriculum.Curriculum_Study_Year}");
                    worksheet.Cell(row, 2).Style.Font.FontName = "Times New Roman";
                    worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 3).Value = int.Parse(curriculum.Curriculum_Semester);
                    worksheet.Cell(row, 3).Style.Font.FontName = "Times New Roman";
                    worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 4).Value = curriculum.Curriculum_Name_KH;
                    worksheet.Cell(row, 4).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 5).Value = curriculum.Curriculum_Name_EN;
                    worksheet.Cell(row, 5).Style.Font.FontName = "Times New Roman";
                    worksheet.Cell(row, 6).Value = curriculum.Curriculum_Teacher_Name;
                    worksheet.Cell(row, 6).Style.Font.FontName = "Khmer OS Siemreap";
                    worksheet.Cell(row, 7).Value = int.Parse($"{curriculum.Curriculum_Total_Time}");
                    worksheet.Cell(row, 7).Style.Font.FontName = "Times New Roman";
                    worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 8).Value = int.Parse($"{curriculum.Curriculum_Total_Score}");
                    worksheet.Cell(row, 8).Style.Font.FontName = "Times New Roman";
                    worksheet.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
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
