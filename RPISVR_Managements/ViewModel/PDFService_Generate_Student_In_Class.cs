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
    public class PDFService_Generate_Student_In_Class
    {
        public static void CreateReport(List<Student_Info> student_in_class,string class_id,string class_name,string class_in_skill,string class_in_level,string class_in_study_year,string class_in_student_year,string class_in_semester,string class_in_generation,string class_study_time_shift,string class_in_study_type)
        {
            //Set up QuestPDF license(community)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            //Define the path to save the PDF file in the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"បញ្ជីឈ្មោះសិស្សនិស្សិតថ្នាក់({class_name})_ជំនាញ({class_in_skill}).pdf");

            //Load Font Khmer
            string baseFontPath = Path.Combine(AppContext.BaseDirectory, "Resources");
            string Tacteing = Path.Combine(baseFontPath, "TACTENG.ttf");
            string KhmerOSsiemreap = Path.Combine(baseFontPath, "KhmerOSsiemreap.ttf");

            //Register the font
            using (var fontStream = File.OpenRead(KhmerOSsiemreap))
            {
                FontManager.RegisterFont(fontStream);
            }

            //Count Total Student
            int totalStudents = student_in_class.Count;
            // Count female students
            int femaleStudents = student_in_class.Count(student_in_class_female => student_in_class_female.Stu_Gender == "ស្រី");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);

                    // Table Content Section
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
                            headerColumn.Item().Text($"បញ្ជីរាយនាមនិស្សិត")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Class Name Text
                            headerColumn.Item().Text($"ថ្នាក់រៀន៖ {class_name}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Study Year, Study Skill
                            headerColumn.Item().Text($"ឆ្នាំសិក្សា {class_in_study_year} ជំនាញ {class_in_skill}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Study Level, Study Year, Semester
                            headerColumn.Item().Text($"កម្រិត {class_in_level} ជំនាន់ទី {class_in_generation}  ឆ្នាំទី {class_in_student_year} ឆមាស {class_in_semester}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Study Type, Time Shift
                            headerColumn.Item().Text($"{class_in_study_type} {class_study_time_shift}")
                                .FontFamily("Khmer Muol")
                                .FontSize(11)
                                .AlignCenter();
                            //Spacer
                            headerColumn.Item().PaddingVertical(10);
                        });
                        column.Item().Table(table =>
                        {
                            // Define table columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30); // No Index column
                                columns.RelativeColumn(5); // Name KH column
                                columns.RelativeColumn(5); // Name EN column
                                columns.RelativeColumn(2); // Gender column
                                columns.RelativeColumn(5); // Date of Birth column
                                columns.RelativeColumn(4); // Phone Number column
                                columns.RelativeColumn(4); // Type Student column
                            });
                            // Table Header
                            table.Header(header =>
                            {
                                header.Cell().Border(1).Text("ល.រ").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("គោត្តនាម-នាម").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("អក្សរឡាតាំង").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ភេទ").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ថ្ងៃខែឆ្នាំកំណើត").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("លេខទូរស័ព្ទ").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                                header.Cell().Border(1).Text("ប្រភេទសិស្ស").FontSize(11).FontFamily("Khmer Muol").AlignCenter();
                            });
                            // Table Rows
                            int index = 1;
                            foreach (var student_info in student_in_class)
                            {
                                //Index
                                table.Cell().Border(1).AlignCenter().Text(index.ToString()).FontSize(11);
                                //Full Infomation
                                table.Cell().Border(1).AlignLeft().PaddingLeft(3).EnsureSpace().Text(student_info.Full_Name_KH).FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignLeft().PaddingLeft(3).EnsureSpace().Text(student_info.Full_Name_EN).FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignCenter().Text(student_info.Stu_Gender).FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignCenter().Text(student_info.Stu_BirthdayDateShow).FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignLeft().PaddingLeft(3).Text($"0{student_info.Stu_PhoneNumber}").FontSize(11).FontFamily("Khmer OS Siemreap");
                                table.Cell().Border(1).AlignCenter().Text(student_info.Stu_StatePoor).FontSize(11).FontFamily("Khmer OS Siemreap");

                                index++;
                            }
                        });
                        column.Item().Column(TotalStudent =>
                        {
                            TotalStudent.Item().Row(row =>
                            {
                                // Right-aligned text
                                row.RelativeItem().AlignRight().Text($"សិស្សនិស្សិតសរុប {totalStudents} នាក់ ស្រី {femaleStudents} នាក់")
                                    .FontFamily("Khmer OS Siemreap")
                                    .FontSize(11);
                            });
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
            })

            .GeneratePdf(filePath);

            //Open the PDF file automatically
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
