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
    public class PDFService_Generate_Certificate_Student_Info
    {
        public static void CreateReport(Student_Info item)
        {
            //Set up QuestPDF license(community)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            //Define the path to save the PDF file in the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"{item.Stu_ID}_Report.pdf");

            //Load Font Khmer
            string baseFontPath = Path.Combine(AppContext.BaseDirectory, "Resources");
            string KhmerOSsiemreap = Path.Combine(baseFontPath, "KhmerOSsiemreap.ttf");

            //Register the font
            using (var fontStream = File.OpenRead(KhmerOSsiemreap))
            {
                FontManager.RegisterFont(fontStream);
            }

            string fileName = "RPISSVR_logo.png";
            string filePath_Image = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", fileName);

            byte[] imageBytes = File.ReadAllBytes(filePath_Image);

            // the PDF document
            Document.Create(container =>
            {

                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);


                    //Design Header Section
                    page.Header().Column(column =>
                    {
                        column.Item().Text("ព្រះរាជាណាចក្រកម្ពុជា") // Top title in Khmer
                        .FontFamily("Khmer Muol") // Use the font's name here, not the path
                        .FontSize(13)
                        .AlignCenter();

                        column.Item().Text("ជាតិ សាសនា ព្រះមហាក្សត្រ")
                            .FontFamily("Khmer Muol")
                            .FontSize(12)
                            .AlignCenter();
                        //Image Tacting
                        column.Item().AlignCenter().AlignMiddle().Element(imageContainer =>
                        {
                            imageContainer.Width(50).Height(5).Image("Assets/Report_Student_Info_Icon/Tacting(5).png");
                        });

                        //Spacer
                        column.Item().PaddingVertical(5);

                        //Logo
                        column.Item().AlignLeft().PaddingLeft(50).Element(imageContainer =>
                        {  
                            using (var imageStream = new MemoryStream(imageBytes))
                            {
                               imageContainer.Width(80).Height(80).Image(imageStream);
                            }
                        });

                        //Ministry
                        column.Item().Text("វិទ្យាស្ថានពហុបច្ចេកទេសភូមិភាគតេជោសែនស្វាយរៀង")
                            .FontFamily("Khmer OS Bokor")
                            .FontSize(11)
                            .AlignLeft();

                        //University Name 
                        column.Item().Text("លេខ៖.............................ព.ភ.ត.ស.ស្វរ")
                            .FontFamily("Khmer OS Bokor")
                            .FontSize(11)
                            .AlignLeft();

                        //Spacer
                        column.Item().PaddingVertical(10);

                        //Title Text1
                        column.Item().Text("លិខិតបញ្ជាក់ការសិក្សា")
                        .FontFamily("Khmer Muol")
                        .FontSize(13)
                        .Bold()
                        .AlignCenter();

                        //Title Text2
                        column.Item().Text("នាយកវិទ្យាស្ថានពហុបច្ចេកទេសភូមិភាគតេជោសែនស្វាយរៀង")
                        .FontFamily("Khmer Muol")
                        .FontSize(12)
                        .Bold()
                        .AlignCenter();

                        //Title Text3
                        column.Item().Text("បញ្ជាក់ថា")
                        .FontFamily("Khmer Muol")
                        .FontSize(12)
                        .Bold()
                        .AlignCenter();

                    });

                    //Content with student details
                    page.Content().Column(column =>
                    {
                        //Image Student
                        column.Item().AlignCenter().AlignMiddle().Element(imageContainer =>
                        {
                            if (item.ProfileImageBytes != null && item.ProfileImageBytes.Length > 0)
                            {
                                using (var imageStream = new MemoryStream(item.ProfileImageBytes))
                                {
                                    imageContainer.Width(100).Height(140).Image(imageStream);
                                }
                            }
                            else
                            {
                                imageContainer.Width(100).Height(150).Image("Assets/Student_Logo.png");
                                column.Item().Text("(សិស្សនិស្សិតមិនមានរូបភាព)")
                                   .FontFamily("Khmer OS Siemreap")
                                   .FontSize(9)
                                   .AlignCenter();
                            }
                        });
                        //University Name 
                        column.Item().Text($"លេខសម្គាល់៖ {item.Stu_ID}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignCenter();
                 
                        //Student Name KH
                        column.Item().Text($"   និស្សិតឈ្មោះ        {item.Stu_FirstName_KH} {item.Stu_LastName_KH}")
                            .FontFamily("Khmer Muol")
                            .FontSize(11)
                            .AlignLeft();

                        //Student Name EN
                        column.Item().Text($"   ជាអក្សរឡាតាំង         {item.Stu_FirstName_EN} {item.Stu_LastName_EN}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();

                        //Gender
                        column.Item().Text($"   ភេទ                        {item.Stu_Gender}   សញ្ជាតិ  ខ្មែរ")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student BirthDay
                        column.Item().Text($"   ថ្ងៃខែឆ្នាំកំណើត         {item.Stu_BirthdayDateShow}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();

                        //Student Birth Place
                        column.Item().Text($"   ទីកន្លែងកំណើត         {item.Stu_Birth_Village} {item.Stu_Birth_Commune} {item.Stu_Birth_Distric} {item.Stu_Birth_Province}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();

                        //Student Living
                        column.Item().Text($"   អាស័យដ្ឋានបច្ចុប្បន្ន   {item.Stu_Live_Vill} {item.Stu_Live_Comm} {item.Stu_Live_Dis} {item.Stu_Live_Pro}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();

                        //Spacer
                        column.Item().PaddingVertical(5);

                        //Text Show
                        column.Item().Text($"           ពិតជានិស្សិតកំពុងសិក្សាជំនាញ  {item.Stu_EducationSubjects}  ជំនាន់ទី  {item.Stu_Generation}  ឆ្នាំទី {item.Stu_StudyingTime}  ឆមាសទី {item.Stu_Semester} កម្រិត {item.Stu_EducationLevels} ក្នុងឆ្នាំសិក្សា {item.Stu_StudyYear} នៅវិទ្យាស្ថានពហុបច្ចេកទេសភូមិភាគតេជោសែនស្វាយរៀងពិតប្រាកដមែន។")
                        .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();

                        //Text Show
                        column.Item().Text($"           លិខិតបញ្ជាក់នេះចេញជូនសាមីខ្លួនដើម្បីប្រើប្រាស់តាមមុខការដែលអាចប្រើបាន។")
                        .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();

                        //Footer
                        column.Item().Column(footer_info =>
                        {
                            column.Item().PaddingTop(10).Column(belowTableColumn =>
                            {
                                belowTableColumn.Item().AlignRight().Text("ថ្ងៃ                        ខែ            ឆ្នាំរោង ឆស័ក ព.ស. ២៥៦៨").AlignCenter().FontFamily("Khmer OS Siemreap").FontSize(11);
                                belowTableColumn.Item().Row(row =>
                                {
                                    // Right-aligned text
                                    row.RelativeItem().AlignCenter().PaddingLeft(300).Text("       ស្វាយរៀង, ថ្ងៃទី      ខែ           ឆ្នាំ ២០២៥")
                                        .FontFamily("Khmer OS Siemreap")
                                        .FontSize(11);
                                });
                                belowTableColumn.Item().Row(row =>
                                {
                                    // Right-aligned text
                                    row.RelativeItem().PaddingLeft(300).AlignCenter().Text("               នាយកវិទ្យាស្ថាន")
                                        .FontFamily("Khmer Muol")
                                        .FontSize(11);
                                });
                            });

                        });        
                    });

                    //Footer with page number
                    //page.Footer().AlignCenter().Text(text =>
                    //{
                    //    text.Span("Page ");
                    //    text.CurrentPageNumber();
                    //    text.Span(" of ");
                    //    text.TotalPages();
                    //});
                });
            })
            .GeneratePdf(filePath);

            //Open the PDF file automatically
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
        }


        private static string LoadKhmerFont()
        {
            //Load the font from the project resources
            string fontPath = Path.Combine(AppContext.BaseDirectory, "Resources", "KhmerOSbokor.ttf");

            //Open the font file as a stream
            using (var fontStream = new FileStream(fontPath, FileMode.Open, FileAccess.Read))
            {
                //Register the font using the stream
                FontManager.RegisterFont(fontStream);
            }
            return fontPath;
        }

    }
}
