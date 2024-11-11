using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using RPISVR_Managements.List_and_Reports.Students_Name_Table;
using RPISVR_Managements.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using VerifyTests;
using Windows.Graphics.Imaging;

namespace RPISVR_Managements.ViewModel
{

    public class PdfReportService_Student_Info
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

                        //Spacer
                        column.Item().PaddingVertical(5);


                        //Ministry
                        column.Item().Text("ក្រសួងការងារ និងបណ្ដុះបណ្ដាលវិជ្ជាជីវៈ")
                            .FontFamily("Khmer Muol")
                            .FontSize(12)
                            .AlignLeft();
                        
                        //University Name 
                        column.Item().Text("វិទ្យាស្ថានពហុបច្ចេកទេសភូមិភាគតេជោសែនស្វាយរៀង")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();

                        //Spacer
                        column.Item().PaddingVertical(10);

                        //Title Text
                        column.Item().Text("ព័ត៌មានសិស្សនិស្សិត")
                        .FontFamily("Khmer Muol") 
                        .FontSize(13)
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
                                    imageContainer.Width(100).Height(150).Image(imageStream);
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
                        //Text
                        column.Item().Text("ព័ត៌មានទូទៅ")
                            .FontFamily("Khmer Muol")
                            .FontSize(11)
                            .Bold()
                            .AlignLeft();
                        //Student Name
                        column.Item().Text($"   និស្សិតឈ្មោះ            {item.Stu_FirstName_KH} {item.Stu_LastName_KH} ({item.Stu_FirstName_EN} {item.Stu_LastName_EN})")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Gender
                        column.Item().Text($"   ភេទ                        {item.Stu_Gender}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student BirthDay
                        column.Item().Text($"   ថ្ងៃខែឆ្នាំកំណើត         {item.Stu_BirthdayDateShow}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Phone Number
                        column.Item().Text($"   លេខទូរស័ព្ទ              0{item.Stu_PhoneNumber}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Living
                        column.Item().Text($"   អាស័យដ្ឋានបច្ចុប្បន្ន   {item.Stu_Live_Vill} {item.Stu_Live_Comm} {item.Stu_Live_Dis} {item.Stu_Live_Pro}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Birth Place
                        column.Item().Text($"   ទីកន្លែងកំណើត         {item.Stu_Birth_Village} {item.Stu_Birth_Commune} {item.Stu_Birth_Distric} {item.Stu_Birth_Province}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Text
                        column.Item().Text("ព័ត៌មានសិក្សា")
                            .FontFamily("Khmer Muol")
                            .FontSize(11)
                            .Bold()
                            .AlignLeft();
                        //Student Skill Subject
                        column.Item().Text($"    សិក្សាជំនាញ             {item.Stu_EducationSubjects}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Education Level
                        column.Item().Text($"    កម្រិតសិក្សា              {item.Stu_EducationLevels}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Year
                        column.Item().Text($"    ឆ្នាំទី                        {item.Stu_StudyingTime}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Study Time
                        column.Item().Text($"    វេនសិក្សា                  {item.Stu_StudyTimeShift}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Study Type
                        column.Item().Text($"    ប្រភេទសិក្សា             {item.Stu_EducationType}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        // Student State Poor
                        column.Item().Text($"    ប្រភេទសិស្សនិស្សិត    {item.Stu_StatePoor}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Study Year
                        column.Item().Text($"    ឆ្នាំសិក្សា                   {item.Stu_StudyYear}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Text
                        column.Item().Text("ព័ត៌មានគ្រួសារ")
                            .FontFamily("Khmer Muol")
                            .FontSize(11)
                            .Bold()
                            .AlignLeft();
                        //Student Family
                        column.Item().Text($"    ស្ថានភាពគ្រួសារ          {item.Stu_StateFamily}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Mother
                        column.Item().Text($"    ម្ដាយឈ្មោះ៖ {item.Stu_Mother_Name} ,មុខរបរ៖ {item.Stu_Mother_Job} ,លេខទូរស័ព្ទ៖ 0{item.Stu_Mother_Phone}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
                        //Student Father
                        column.Item().Text($"    ឪពុកឈ្មោះ៖ {item.Stu_Father_Name} ,មុខរបរ៖ {item.Stu_Father_Job} ,លេខទូរស័ព្ទ៖ 0{item.Stu_Father_Phone}")
                            .FontFamily("Khmer OS Siemreap")
                            .FontSize(11)
                            .AlignLeft();
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
