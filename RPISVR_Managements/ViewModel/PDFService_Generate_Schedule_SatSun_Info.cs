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
    public class PDFService_Generate_Schedule_SatSun_Info
    {
        public static void CreateReport(Class_Schedule class_Schedule, string class_skill, string class_level, string class_studyyear, string class_student_year, string class_semester, string class_generation)
        {
            //Set up QuestPDF license(community)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            //Define the path to save the PDF file in the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"កាលវិភាគថ្នាក់_{class_Schedule.SD_Class_Name}({class_Schedule.Schedule_Name}).pdf");

            //Load Font Khmer
            string baseFontPath = Path.Combine(AppContext.BaseDirectory, "Resources");
            string Tacteing = Path.Combine(baseFontPath, "TACTENG.ttf");
            string KhmerOSsiemreap = Path.Combine(baseFontPath, "KhmerOSsiemreap.ttf");

            //Register the font
            using (var fontStream = File.OpenRead(KhmerOSsiemreap))
            {
                FontManager.RegisterFont(fontStream);
            }

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4.Landscape());

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
                            //headerColumn.Item().Text("ក្រសួងការងារ និងបណ្ដុះបណ្ដាលវិជ្ជាជីវៈ")
                            //    .FontFamily("Khmer Muol")
                            //    .FontSize(12)
                            //    .AlignLeft();

                            //University Name 
                            headerColumn.Item().Text("វិទ្យាស្ថានពហុបច្ចេកទេសភូមិភាគតេជោសែនស្វាយរៀង")
                                .FontFamily("Khmer OS Siemreap")
                                .FontSize(11)
                                .AlignLeft();

                            //Department Name
                            headerColumn.Item().Text($"ការិយាល័យសិក្សា")
                                .FontFamily("Khmer OS Siemreap")
                                .FontSize(11)
                                .AlignLeft();

                            //Spacer
                            headerColumn.Item().PaddingVertical(5);

                            //Title Text
                            headerColumn.Item().Text($"កាលវិភាគ ថ្នាក់ {class_Schedule.SD_Class_Name}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Class Name Text
                            headerColumn.Item().Text($"ជំនាញ៖ {class_skill} កម្រិត៖ {class_level}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Study Year, Study Skill
                            headerColumn.Item().Text($"ឆ្នាំទី៖ {class_student_year} ឆមាស៖ {class_semester} ជំនាន់៖ {class_generation} វេន៖ {class_Schedule.SD_Class_TimeShift} ឆ្នាំសិក្សា៖ {class_studyyear}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();

                            //Spacer
                            headerColumn.Item().PaddingVertical(5);

                            //Start Date 
                            headerColumn.Item().Text($"កាលបរិច្ឆេទអនុវត្តកាលវិភាគ៖ {class_Schedule.DateTime_Start_Schedule_Strating}  (អគារ៖ {class_Schedule.SD_Building_Name} បន្ទប់៖ {class_Schedule.SD_Building_Room}) កាលវិភាគឈ្មោះ៖ {class_Schedule.Schedule_Name}")
                                .FontFamily("Khmer OS Siemreap")
                                .FontSize(11)
                                .AlignLeft();
                        });
                        column.Item().Table(table =>
                        {
                            //Define table columns
                            table.ColumnsDefinition(columns =>
                            {
                                //columns.ConstantColumn(30); // No Index column                  
                                columns.RelativeColumn(5); //Curriculum Date
                                columns.RelativeColumn(5); //Curriculum Satureday
                                columns.RelativeColumn(5); //Curriculum Sunday
                                
                            });

                            //Table Header
                            table.Header(header =>
                            {
                                header.Cell().Border(1).Text("ម៉ោង").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ថ្ងៃសៅរ៍").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ថ្ងៃអាទិត្យ").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                
                            });
                            // Table rows (example data)
                            table.Cell().Border(1).PaddingTop(10).AlignCenter().Text($"{class_Schedule.SD_Start_DateTime_SS1}\n <-> \n{class_Schedule.SD_End_DateTime_SS1}").AlignCenter();
                            table.Cell().Border(1).PaddingTop(2).Text($"{class_Schedule.SD_Skill_Name_Sat1}\n{class_Schedule.SD_TotalTime_Sat1} (h)\n{class_Schedule.SD_Teacher_Sat1}").FontSize(11).FontFamily("Khmer OS Siemreap").AlignCenter();
                            table.Cell().Border(1).PaddingTop(2).Text($"{class_Schedule.SD_Skill_Name_Sun1}\n{class_Schedule.SD_TotalTime_Sun1} (h)\n{class_Schedule.SD_Teacher_Sun1}").FontSize(11).FontFamily("Khmer OS Siemreap").AlignCenter();
                            

                            table.Cell().Border(1).PaddingTop(10).AlignCenter().Text($"{class_Schedule.SD_Start_DateTime_SS2}\n <-> \n{class_Schedule.SD_End_DateTime_SS2}");
                            table.Cell().Border(1).PaddingTop(2).Text($"{class_Schedule.SD_Skill_Name_Sat2}\n{class_Schedule.SD_TotalTime_Sat2} (h)\n{class_Schedule.SD_Teacher_Sat2}").FontSize(11).FontFamily("Khmer OS Siemreap").AlignCenter();
                            table.Cell().Border(1).PaddingTop(2).Text($"{class_Schedule.SD_Skill_Name_Sun2}\n{class_Schedule.SD_TotalTime_Sun2} (h)\n{class_Schedule.SD_Teacher_Sun2}").FontSize(11).FontFamily("Khmer OS Siemreap").AlignCenter();
                            

                        });
                        column.Item().PaddingTop(10).Column(belowTableColumn =>
                        {
                            belowTableColumn.Item().AlignRight().Text("ថ្ងៃ                        ខែ            ឆ្នាំរោង ឆស័ក ព.ស. ២៥៦៨").AlignCenter().FontFamily("Khmer OS Siemreap").FontSize(11);
                            belowTableColumn.Item().Row(row =>
                            {
                                // Left-aligned text
                                row.RelativeItem().PaddingLeft(20).AlignLeft().Text("  បានឃើញ និងពិនិត្យត្រឹមត្រូវ")
                                    .FontFamily("Khmer OS Siemreap")
                                    .FontSize(11);

                                // Right-aligned text
                                row.RelativeItem().PaddingLeft(40).AlignCenter().Text("         ស្វាយរៀង, ថ្ងៃទី      ខែ           ឆ្នាំ ២០២៥")
                                    .FontFamily("Khmer OS Siemreap")
                                    .FontSize(11);
                            });
                            belowTableColumn.Item().Row(row =>
                            {
                                // Left
                                row.RelativeItem().PaddingLeft(25).AlignLeft().Text("  ប្រធានការិយាល័យសិក្សា")
                                    .FontFamily("Khmer Muol")
                                    .FontSize(11);
                                // Right-aligned text
                                row.RelativeItem().PaddingLeft(40).AlignCenter().Text("                           អ្នករៀបចំ")
                                    .FontFamily("Khmer OS Siemreap")
                                    .FontSize(11);
                            });
                            belowTableColumn.Item().AlignCenter().Text("បានឃើញ និងឯកភាព").FontFamily("Khmer OS Siemreap").FontSize(11);
                            belowTableColumn.Item().AlignCenter().Text("ស្វាយរៀង, ថ្ងៃទី      ខែ       ឆ្នាំ២០២៥").FontFamily("Khmer OS Siemreap").FontSize(11);
                            belowTableColumn.Item().AlignCenter().Text("នាយកវិទ្យាស្ថាន").FontFamily("Khmer Muol").FontSize(11);
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
