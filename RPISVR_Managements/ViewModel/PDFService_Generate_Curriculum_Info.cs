using Microsoft.UI.Xaml.Controls;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using RPISVR_Managements.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.ViewModel
{
    public class PDFService_Generate_Curriculum_Info
    {
        public static void CreateReport(List<Curriculum_Info> curriculum_Infos,string curriculum_skill_select,string curriculum_level_select,string curriculum_study_year_select)
        {
            //Set up QuestPDF license(community)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            //Define the path to save the PDF file in the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"កម្មវិធីសិក្សាជំនាញ{curriculum_skill_select}_កម្រិត{curriculum_level_select}_ឆ្នាំទី{curriculum_study_year_select}.pdf");

            //Load Font Khmer
            string baseFontPath = Path.Combine(AppContext.BaseDirectory, "Resources");
            string Tacteing = Path.Combine(baseFontPath, "TACTENG.ttf");
            string KhmerOSsiemreap = Path.Combine(baseFontPath, "KhmerOSsiemreap.ttf");

            //Register the font
            using (var fontStream = File.OpenRead(KhmerOSsiemreap))
            {
                FontManager.RegisterFont(fontStream);
            }

            //Group curriculum by study year
            var groupCurriculum_StudyYear = curriculum_Infos.GroupBy(s => s.Curriculum_Study_Year).ToList();
            var groupCurriculum_Semester = curriculum_Infos.GroupBy(s => s.Curriculum_Semester).ToList();
            //TotalCount_Curriculum
            int TotalCount_Curriculum_Subject = curriculum_Infos.Count;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);

                    //Table Content
                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        //Header Column Show Only one page (Use in Content Page)
                        //If other use in page.Header normal
                        column.Item().Column(headerColumn =>
                        {
                            headerColumn.Item().Text("ព្រះរាជាណាចក្រកម្ពុជា")
                                   .FontFamily("Khmer Muol")
                                   .FontSize(13)
                                   .AlignCenter();

                            headerColumn.Item().Text("ជាតិ សាសនា ព្រះមហាក្សត្រ")
                                .FontFamily("Khmer OS Siemreap")
                                .FontSize(12)
                                .AlignCenter();
                            //Image Tacting
                            headerColumn.Item().AlignCenter().AlignMiddle().Element(imageContainer =>
                            {
                                imageContainer.Width(50).Height(5).Image("Assets/Report_Student_Info_Icon/Tacting(5).png");
                            });

                            //Spacer
                            headerColumn.Item().PaddingVertical(5);


                            //Ministry
                            headerColumn.Item().Text("ក្រសួងការងារ និងបណ្ដុះបណ្ដាលវិជ្ជាជីវៈ")
                                .FontFamily("Khmer Muol")
                                .FontSize(12)
                                .AlignLeft();

                            //University Name 
                            headerColumn.Item().Text("វិទ្យាស្ថានពហុបច្ចេកទេសភូមិភាគតេជោសែនស្វាយរៀង")
                                .FontFamily("Khmer OS Siemreap")
                                .FontSize(11)
                                .AlignLeft();

                            //Spacer
                            headerColumn.Item().PaddingVertical(10);

                            //Title Text
                            headerColumn.Item().Text($"កម្មវិធីសិក្សា")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Class Name Text
                            headerColumn.Item().Text($"ជំនាញ៖ {curriculum_skill_select}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Study Year, Study Skill
                            headerColumn.Item().Text($"កម្រិត៖ {curriculum_level_select} ឆ្នាំទី៖ {curriculum_study_year_select}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();

                            //Spacer
                            headerColumn.Item().PaddingVertical(10);
                        });
                        column.Item().Table(table =>
                        {
                            //Define table columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30); // No Index column                  
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(5); //Curriculum Name KH
                                columns.RelativeColumn(5); //Curriculum Name EN
                                columns.RelativeColumn(3); //Curriculum Teacher
                                columns.RelativeColumn(2); //Curriculum Study Time
                            });

                            //Table Header
                            table.Header(header =>
                            {
                                header.Cell().Border(1).Text("ល.រ").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ឆមាស").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("មុខវិជ្ជាសិក្សា (ខ្មែរ)").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("មុខវិជ្ជាសិក្សា (ឡាតាំង)").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("គ្រូបច្ចេកទេស").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ចំនួនម៉ោង").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                            });

                            
                            foreach(var group_curriculum_studyyear in groupCurriculum_StudyYear)
                            {
                                var curr_studyYear = group_curriculum_studyyear.Key;
                                bool isFirstRow = true;
                                int index = 1;

                                foreach(var group_curriculum_semester in group_curriculum_studyyear.GroupBy(s => s.Curriculum_Semester))
                                {
                                    var semester = group_curriculum_semester.Key;
                                    bool isFirstSemesterRow = true;
                                    int count_semester = group_curriculum_semester.Count();
                                    foreach (var curriculum in group_curriculum_semester)
                                    {
                                        //Study Year Column (only displayed once per group)
                                        if (isFirstRow)
                                        {
                                            table.Cell().ColumnSpan(6)
                                            .Border(1)
                                            .AlignLeft()
                                            .PaddingLeft(15)
                                            .Text($"ឆ្នាំទី៖ {curr_studyYear}")
                                            .FontSize(11)
                                            .FontFamily("Khmer Muol");
                                            isFirstRow = false;
                                        }
                                        //Index
                                        table.Cell().Border(1).AlignCenter().Text(index.ToString()).FontSize(11);

                                        //Semester Column
                                        if (isFirstSemesterRow)
                                        {
                                            table.Cell().RowSpan((uint)count_semester)
                                                 .Border(1)
                                                 .AlignCenter()
                                                 .AlignMiddle()
                                                 .Text($"{semester}")
                                                 .FontSize(11)
                                                 .FontFamily("Khmer OS Siemreap");
                                            isFirstSemesterRow = false;
                                        }
                                        //Infomation
                                        table.Cell().Border(1).AlignLeft().PaddingLeft(3).AlignMiddle().EnsureSpace().Text(curriculum.Curriculum_Name_KH).FontSize(11).FontFamily("Khmer OS Siemreap");
                                        table.Cell().Border(1).AlignLeft().PaddingLeft(3).AlignMiddle().EnsureSpace().Text(curriculum.Curriculum_Name_EN).FontSize(11);
                                        table.Cell().Border(1).AlignLeft().PaddingLeft(3).EnsureSpace().Text(curriculum.Curriculum_Teacher_Name).FontSize(11).FontFamily("Khmer OS Siemreap");
                                        table.Cell().Border(1).AlignCenter().Text($"{curriculum.Curriculum_Total_Time}h").FontSize(11).FontFamily("Khmer OS Siemreap");
                                        index++;
                                    }

                                }    
                            }
                        });
                        column.Item().Column(TotalStudent =>
                        {
                            TotalStudent.Item().Row(row =>
                            {
                                // Right-aligned text
                                row.RelativeItem().AlignRight().Text($"សរុបទាំងអស់ {TotalCount_Curriculum_Subject} មុខវិជ្ជា")
                                    .FontFamily("Khmer OS Siemreap")
                                    .FontSize(11);
                            });
                            column.Item().PaddingTop(10).Column(belowTableColumn =>
                            {
                                belowTableColumn.Item().AlignRight().Text("ថ្ងៃ                        ខែ            ឆ្នាំរោង ឆស័ក ព.ស. ២៥៦៨").AlignCenter().FontFamily("Khmer OS Siemreap").FontSize(11);
                                belowTableColumn.Item().Row(row =>
                                {
                                    // Left-aligned text
                                    row.RelativeItem().AlignLeft().Text("  បានឃើញ និងពិនិត្យត្រឹមត្រូវ")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(11);

                                    // Right-aligned text
                                    row.RelativeItem().AlignCenter().Text("         ស្វាយរៀង, ថ្ងៃទី      ខែ           ឆ្នាំ ២០២៥")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(11);
                                });
                                belowTableColumn.Item().Row(row =>
                                {
                                    // Left
                                    row.RelativeItem().AlignLeft().Text("  ប្រធានដេប៉ាតឺម៉ង់")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(11);
                                    // Right-aligned text
                                    row.RelativeItem().AlignCenter().Text("                           អ្នករៀបចំ")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(11);
                                });
                                belowTableColumn.Item().AlignCenter().Text("បានឃើញ និងឯកភាព").FontFamily("Khmer OS Siemreap").FontSize(11);
                                belowTableColumn.Item().AlignCenter().Text("ស្វាយរៀង, ថ្ងៃទី      ខែ       ឆ្នាំ២០២៥").FontFamily("Khmer OS Siemreap").FontSize(11);
                                belowTableColumn.Item().AlignCenter().Text("នាយកវិទ្យាស្ថាន").FontFamily("Khmer Muol").FontSize(11);
                            });

                        });
                    });
                    // Footer with Page Numbering
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("ទំព័រ ").FontFamily("Khmer OS Siemreap").FontSize(9);
                        text.CurrentPageNumber().FontFamily("Khmer OS Siemreap").FontSize(9);
                        text.Span(" នៃ ").FontFamily("Khmer OS Siemreap").FontSize(9);
                        text.TotalPages().FontFamily("Khmer OS Siemreap").FontSize(9);
                    });
                });
            })
            .GeneratePdf(filePath);
            //Open the PDF file automatically
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
