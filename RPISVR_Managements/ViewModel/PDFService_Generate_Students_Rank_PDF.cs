using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using RPISVR_Managements.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.ViewModel
{
    public class PDFService_Generate_Students_Rank_PDF
    {
        public static void CreateReport(List<Class_Score> student_rank_info, string Class_Name, string Class_In_Skill, string Class_In_Study_Timeshift, string Class_In_Level, string Class_In_Study_Year, string Class_In_Student_Year, string Class_In_Semester, string Class_In_Generation, string Class_In_Study_Type)
        {
            //Set up QuestPDF license(community)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            //Define the path to save the PDF file in the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"តារាងចំណាត់ថ្នាក់{Class_Name}_{Class_In_Skill}_{Class_In_Study_Year}.pdf");

            //Load Font Khmer
            string baseFontPath = Path.Combine(AppContext.BaseDirectory, "Resources");
            string Tacteing = Path.Combine(baseFontPath, "TACTENG.ttf");
            string KhmerOSsiemreap = Path.Combine(baseFontPath, "KhmerOSsiemreap.ttf");

            //Register the font
            using (var fontStream = File.OpenRead(KhmerOSsiemreap))
            {
                FontManager.RegisterFont(fontStream);
            }

            //TotalCount_Student
            int total_count_student = student_rank_info.Count;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4.Landscape());

                    //Table Content
                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        // Header Column Show Only one page(Use in Content Page)
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
                            headerColumn.Item().Text($"តារាងចំណាត់ថ្នាក់និស្សិត")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Class Name Text
                            headerColumn.Item().Text($"ជំនាញ៖ {Class_In_Skill}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Study Year, Study Skill
                            headerColumn.Item().Text($"កម្រិត៖ {Class_In_Level}  ឆ្នាំទី៖ {Class_In_Student_Year} ឆមាស៖ {Class_In_Semester} ជំនាន់៖ {Class_In_Generation}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();

                            //Timeshift,Type,
                            headerColumn.Item().Text($"វេនសិក្សា៖ {Class_In_Study_Timeshift} {Class_In_Study_Type} ឆ្នាំសិក្សា៖ {Class_In_Study_Year}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();

                            //Spacer
                            headerColumn.Item().PaddingVertical(10);
                        });

                        //Table
                        column.Item().Table(table =>
                        {
                            //Define table columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30); // No Index column                  
                                columns.RelativeColumn(3); //Student ID
                                columns.RelativeColumn(3); //Student Name
                                columns.RelativeColumn(2); //Student Gender
                                columns.RelativeColumn(3); //Student Birth Day
                                columns.RelativeColumn(2); //Student Score
                                columns.RelativeColumn(2); //Student Average
                                columns.RelativeColumn(2); //Student Rank
                                columns.RelativeColumn(2); //Student Grade
                                columns.RelativeColumn(2); //Student Grade System
                                columns.RelativeColumn(2); //Student Pass State

                            });

                            //Table Header
                            table.Header(header =>
                            {
                                header.Cell().Border(1).Text("ល.រ").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("លេខសម្គាល់").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("គោត្តនាម-នាម").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ភេទ").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ថ្ងៃខែឆ្នាំកំណើត").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ពិន្ទុសរុប").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("មធ្យមភាគ").FontSize(11).FontFamily("Khmer Muol").AlignCenter();

                                header.Cell().Border(1).Text("ចំណាត់ថ្នាក់").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("និទ្ទេស").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("កម្រិត").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ស្ថានភាព").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                            });

                            int index = 1;
                            foreach (var student_infos in student_rank_info)
                            {
                                //Index
                                table.Cell().Border(1).AlignCenter().Text(index.ToString()).FontSize(11);
                                //Infomation
                                table.Cell().Border(1).AlignLeft().PaddingLeft(3).AlignMiddle().EnsureSpace().Text(student_infos.Score_Student_ID).FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignLeft().PaddingLeft(5).AlignMiddle().EnsureSpace().Text(student_infos.Score_Student_Name).FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignCenter().PaddingLeft(3).EnsureSpace().Text(student_infos.Score_Student_Gender).FontSize(11).FontFamily("Khmer OS Siemreap");

                                table.Cell().Border(1).AlignCenter().PaddingLeft(3).AlignMiddle().EnsureSpace().Text(student_infos.Score_Student_BirthDay).FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignCenter().PaddingLeft(3).AlignMiddle().EnsureSpace().Text($"{student_infos.Total_Score_Average}").FontSize(11).FontFamily("Khmer OS Siemreap");

                                table.Cell().Border(1).AlignCenter().PaddingLeft(3).AlignMiddle().EnsureSpace().Text($"{student_infos.Average_Student}").FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignCenter().PaddingLeft(3).AlignMiddle().EnsureSpace().Text($"{student_infos.Rank_Student}").FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignCenter().PaddingLeft(3).AlignMiddle().EnsureSpace().Text(student_infos.Grade_Letter).FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignCenter().PaddingLeft(3).AlignMiddle().EnsureSpace().Text($"{student_infos.Grade_System}").FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignCenter().PaddingLeft(3).AlignMiddle().EnsureSpace().Text($"{student_infos.Student_Pass_State}").FontSize(11).FontFamily("Khmer OS Siemreap");

                                index++;
                            }
                        });

                        //Footer
                        column.Item().Column(footer_info =>
                        {
                            footer_info.Item().Row(row =>
                            {
                                // Right-aligned text
                                row.RelativeItem().AlignLeft().Text($"និសិ្សតសរុប៖ {total_count_student} នាក់")
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
                                    row.RelativeItem().AlignLeft().Text("ប្រធានការិយាល័យសិក្សា")
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
                });
            })
            .GeneratePdf(filePath);
            //Open the PDF file automatically
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
