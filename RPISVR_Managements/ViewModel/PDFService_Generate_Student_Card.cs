using Mysqlx.Crud;
using Mysqlx.Resultset;
using MySqlX.XDevAPI.Relational;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RPISVR_Managements.Model;
using RPISVR_Managements.Student_Informations.Report_Student_Informations;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace RPISVR_Managements.ViewModel
{
    public class PDFService_Generate_Student_Card
    {
        public static void CreateCard_Report(List<Student_Info> student_card)
        {
            //Set up QuestPDF license(community)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            string FirstStuID = student_card.First().Stu_ID;
            string LastStuID = student_card.Last().Stu_ID;

            //Define the path to save the PDF file in the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"កាតសម្គាល់ខ្លួនសិស្សនិស្សិតចាប់ពី{FirstStuID}ដល់{LastStuID}.pdf");

            //Load Font Khmer
            string baseFontPath = Path.Combine(AppContext.BaseDirectory, "Resources");
            string Tacteing = Path.Combine(baseFontPath, "TACTENG.ttf");
            string KhmerOSsiemreap = Path.Combine(baseFontPath, "KhmerOSsiemreap.ttf");

            //Register the font
            using (var fontStream = File.OpenRead(KhmerOSsiemreap))
            {
                FontManager.RegisterFont(fontStream);
            }

            //PDF Documemnt
            Document.Create(container =>
            {
                container.Page(page =>
                {

                    //Size CR80
                    //page.Size(85.6f, 54f, Unit.Millimetre);

                    // page.Size(98.5f, 67f, Unit.Millimetre);

                    //Size CR100 (Portrait Orientation)
                    page.Size(67f, 95.5f, Unit.Millimetre);
                    page.Margin(5);



                    //PaddingVertical(5)
                    page.Background()
                        .Image("Assets/Student_Card.png")
                        .FitWidth()
                        .WithCompressionQuality(ImageCompressionQuality.High);

                    //Design Header Section
                    page.Header().PaddingTop(55).Column(column =>
                    {
                        column.Item().Text("ប័ណ្ណសម្គាល់ខ្លួននិស្សិត")
                                   .FontFamily("Khmer Muol")
                                   .FontColor("#1A13D3")
                                   .FontSize(8)
                                   .AlignCenter();

                    });

                    //Content Page
                    page.Content().Column(column =>
                    {
                        foreach (var student in student_card)
                        {
                            //Name KH
                            column.Item().PaddingTop(5).Column(stu_name =>
                            {
                                stu_name.Item().Row(row =>
                                {
                                    // Left-aligned text
                                    row.RelativeItem().AlignLeft().Text("   គោត្តនាម-នាម")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(7);

                                    // Right-aligned text
                                    row.RelativeItem().AlignLeft().Text($":{student.Stu_FirstName_KH} {student.Stu_LastName_KH}")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(6);
                                });
                            });

                            //Name EN
                            column.Item().Column(stu_name_en =>
                            {
                                stu_name_en.Item().Row(row =>
                                {
                                    // Left-aligned text
                                    row.RelativeItem().AlignLeft().Text("   NAME")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(7);

                                    // Right-aligned text
                                    row.RelativeItem().AlignLeft().Text($":{student.Stu_FirstName_EN} {student.Stu_LastName_EN}")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(6);
                                });
                            });

                            //Gender
                            column.Item().Column(stu_gender =>
                            {
                                stu_gender.Item().Row(row =>
                                {
                                    // Left-aligned text
                                    row.RelativeItem().AlignLeft().Text("   ភេទ")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(7);

                                    // Right-aligned text
                                    row.RelativeItem().AlignLeft().Text($":{student.Stu_Gender}")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(6);
                                });
                            });

                            //Birth Day
                            column.Item().Column(stu_birth =>
                            {
                                stu_birth.Item().Row(row =>
                                {
                                    // Left-aligned text
                                    row.RelativeItem().AlignLeft().Text("   ថ្ងៃខែឆ្នាំកំណើត")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(7);

                                    // Right-aligned text
                                    row.RelativeItem().AlignLeft().Text($":{student.Stu_BirthdayDateShow}")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(6);
                                });
                            });

                            //Skill Subject
                            column.Item().Column(stu_skill =>
                            {
                                stu_skill.Item().Row(row =>
                                {
                                    // Left-aligned text
                                    row.RelativeItem().AlignLeft().Text("   ឯកទេស")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(7);

                                    // Right-aligned text
                                    row.RelativeItem().AlignLeft().Text($":{student.Stu_EducationSubjects}")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(6);
                                });
                            });
                            //Stu Grade
                            column.Item().Column(stu_level2 =>
                            {
                                stu_level2.Item().Row(row =>
                                {
                                    // Left-aligned text
                                    row.RelativeItem().AlignLeft().Text("   ជំនាន់ទី")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(7);

                                    // Right-aligned text
                                    row.RelativeItem().AlignLeft().Text($":1")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(6);
                                });
                            });
                            //Study Level
                            column.Item().Column(stu_level =>
                            {
                                stu_level.Item().Row(row =>
                                {                               
                                    // Right-aligned text
                                    row.RelativeItem().AlignLeft().Text($"   កម្រិត:{student.Stu_EducationLevels}")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(6);
                                });
                            });

                            //Date Sign
                            column.Item().PaddingTop(8).Column(Date_sign =>
                            {
                                Date_sign.Item().AlignCenter().Text("                                              ថ្ងៃ                        ខែ            ឆ្នាំរោង ឆស័ក ព.ស. ២៥៦៨").FontFamily("Khmer OS Siemreap").FontSize(4);
                                Date_sign.Item().Row(row =>
                                {                                 
                                    // Right-aligned text
                                    row.RelativeItem().AlignCenter().Text("                                          ស្វាយរៀង, ថ្ងៃទី      ខែ           ឆ្នាំ ២០២៤")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(4);
                                });
                                Date_sign.Item().Row(row =>
                                {
                                    // Right-aligned text
                                    row.RelativeItem().AlignCenter().Text("                                                         ជ.នាយកវិទ្យាស្ថាន")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(4);
                                });
                            });
                            //Image 
                            column.Item().Column(stu_level =>
                            {
                                stu_level.Item().Row(row =>
                                {
                                    // Right-aligned text
                                    row.RelativeItem().PaddingLeft(10).AlignLeft().Element(imageContainer =>
                                    {
                                        if (student.ProfileImageBytes != null && student.ProfileImageBytes.Length > 0)
                                        {
                                            using (var imageStream = new MemoryStream(student.ProfileImageBytes))
                                            {
                                                imageContainer.Width(30).Height(45).Image(imageStream);

                                            }

                                        }
                                        else
                                        {
                                            imageContainer.Width(30).Height(45).Image("Assets/Student_Logo.png");
                                            
                                        }
                                    });

                                    row.RelativeItem().PaddingLeft(-50).AlignLeft().Element(imageContainer2 =>
                                    {
                                        imageContainer2
                                            .Width(40)
                                            .Height(45)
                                            .Image(student.QRCodeBytes);
                                    });
                                        
                                });
                            });
                            
                            //ID 
                            column.Item().PaddingLeft(10).PaddingTop(-3).Text($"លេខសម្គាល់៖ {student.Stu_ID}")
                                .FontFamily("Khmer OS Siemreap")
                                .FontSize(5)
                                .Bold()
                                .AlignLeft();
                            //Study Year
                            column.Item().PaddingLeft(10).Text($"ឆ្នាំសិក្សា៖ {student.Stu_StudyYear}")
                                .FontFamily("Khmer OS Siemreap")
                                .FontSize(5)
                                .Bold()
                                .AlignLeft();

                        }

                    });
                    
                });

            })

            .GeneratePdf(filePath);

            //Open the PDF file automatically
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
