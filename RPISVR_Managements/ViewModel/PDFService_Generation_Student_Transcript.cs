using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RPISVR_Managements.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RPISVR_Managements.ViewModel
{
    public class PDFService_Generation_Student_Transcript
    {
        public static void CreateReport(List<Class_Score> item,Student_Info student_Info,string Full_Name_EN,string Stu_Skill_English,string Stu_Gender_English,string Stu_Place_Birth_English,string Stu_Degree_English,string Stu_Date_Graduation,string Stu_Internship_Text,int Stu_Internship_Credit,string Stu_Internship_Grade)
        {
            //Set up QuestPDF license(community)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            //Define the path to save the PDF file in the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, $"{student_Info.Stu_ID}_Report.pdf");

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

            //Date
            DateOnly Date_Addmit = DateOnly.FromDateTime(DateTime.Today);

            // Format DateOnly to "dd-MMMM-yyyy"
            string Date_Addmit_to_RPITSSVR = Date_Addmit.ToString("dd-MMMM-yyyy", new CultureInfo("en-US"));

            //String Other Gradit
            string Other_Gradit_Transferrend = "N/A";
            //Document PDF
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(10);
                    page.Size(PageSizes.A4);

                    //Design Header Section
                    page.Header().Column(column =>
                    {
                        // Top title in Khmer1
                        column.Item().Text("ព្រះរាជាណាចក្រកម្ពុជា")
                        .FontFamily("Khmer Muol") // Use the font's name here, not the path
                        .FontSize(11)
                        .AlignCenter();

                        // Top title in Englist
                        column.Item().Text("KINGDOM OF CAMBODIA")
                        .FontFamily("Times New Roman (Headings CS)") // Use the font's name here, not the path
                        .FontSize(9)
                        .Bold()
                        .AlignCenter();

                        // Top title in Khmer2
                        column.Item().Text("ជាតិ សាសនា ព្រះមហាក្សត្រ")
                            .FontFamily("Khmer Muol")
                            .FontSize(11)
                            .AlignCenter();

                        // Top title in Englist
                        column.Item().Text("NATION RELIGION KING")
                        .FontFamily("Times New Roman (Headings CS)") // Use the font's name here, not the path
                        .FontSize(9)
                        .Bold()
                        .AlignCenter();
                        ////Image Tacting
                        //column.Item().AlignCenter().AlignMiddle().Element(imageContainer =>
                        //{
                        //    imageContainer.Width(50).Height(5).Image("Assets/Report_Student_Info_Icon/Tacting(5).png");
                        //});

                        //Spacer
                        column.Item().PaddingVertical(5);

                        ////Logo
                        //column.Item().AlignLeft().PaddingLeft(50).Element(imageContainer =>
                        //{
                        //    using (var imageStream = new MemoryStream(imageBytes))
                        //    {
                        //        imageContainer.Width(80).Height(80).Image(imageStream);
                        //    }
                        //});

                        //Ministry
                        column.Item().Text("ក្រសួងការងារ និងបណ្ដុះបណ្ដាលវិជ្ជាជីវៈ")
                            .FontFamily("Khmer Muol")
                            .FontSize(11)
                            .AlignLeft();
                        //University Name 
                        column.Item().Text("វិទ្យាស្ថានពហុបច្ចេកទេសភូមិភាគតេជោសែនស្វាយរៀង")
                            .FontFamily("Khmer OS Bokor")
                            .FontSize(11)
                            .AlignLeft();
                        //University Name EN
                        column.Item().Text("REGIONAL POLYTECHNIC INSTITUTE TECHO SEN SVAY RIENG")
                        .FontFamily("Times New Roman (Headings CS)") // Use the font's name here, not the path
                        .FontSize(9)
                        .Bold()
                        .AlignLeft();
                        //No-
                        column.Item().Text("No:.............................RPITSSR")
                            .FontFamily("Times New Roman (Headings CS)")
                            .FontSize(8)
                            .Bold()
                            .AlignLeft();

                        //Spacer
                        //column.Item().PaddingVertical(10);

                        //Title Text1
                        column.Item().Text("OFFICIAL TRANSCRIPT OF STUDENT RECORDS")
                        .FontFamily("Times New Roman (Headings CS)")
                        .FontSize(9)
                        .Bold()
                        .AlignCenter();

                        //Title Issue Date
                        column.Item().Text("Issue Date:.............................")
                        .FontFamily("Times New Roman (Headings CS)")
                        .FontSize(8)
                        .Bold()
                        .AlignRight();

                    });

                    //Table
                    page.Content().Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            // Define table columns (relative to available space)
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Name
                                columns.RelativeColumn(3); // Date
                                columns.RelativeColumn(4); // Place
                                columns.RelativeColumn(2); // Gender
                            });
                            // = Name ROW =
                            table.Cell().Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Name: ").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             "+Full_Name_EN).FontSize(10).Bold().FontFamily("Times New Roman (Headings CS)");
                                    });
                            // = Date of Birth =
                            table.Cell().Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Date of Birth: ").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("               " + student_Info.Stu_BirthdayDateOnly).FontSize(10).FontFamily("Times New Roman (Headings CS)");
                                    });
                            // = Place of Birth =
                            table.Cell().Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Place of Birth: ").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("               " + Stu_Place_Birth_English).FontSize(10).FontFamily("Times New Roman (Headings CS)");
                                    });
                            // = Gender =
                            table.Cell().Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Gender: ").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("               " + Stu_Gender_English).FontSize(10).FontFamily("Times New Roman (Headings CS)");
                                    });
                            // == Student No ==
                            table.Cell().Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Student ID: ").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             " + student_Info.Stu_ID).FontSize(10).FontFamily("Times New Roman (Headings CS)");
                                    });
                            // == Date Admitted to RPITSSR ==
                            table.Cell().Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Date Admitted to RPITSSR: ").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             " + Date_Addmit_to_RPITSSVR).FontSize(10).FontFamily("Times New Roman (Headings CS)");
                                    });
                            // == Other Gredit Transferred Institution
                            table.Cell().ColumnSpan(2).Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Other Credit Transferred Institution: ").Bold()
                                            .FontSize(9).FontFamily("Times New Roman (Headings CS)");

                                        text.Span("\n"); // Move content to a new line

                                        text.Span(Other_Gradit_Transferrend).FontSize(10)
                                            .FontFamily("Times New Roman (Headings CS)");
                                    });

                            // === Skill ===
                            table.Cell().ColumnSpan(2).RowSpan(2).Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Skill: ").Bold()
                                            .FontSize(9).FontFamily("Times New Roman (Headings CS)");

                                        text.Span("\n"); // Move content to a new line

                                        text.Span("                                         "+Stu_Skill_English).Bold().FontSize(10)
                                            .FontFamily("Times New Roman (Headings CS)");
                                    });
                            // === Degree ===
                            table.Cell().ColumnSpan(2).Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Degree: ").Bold()
                                            .FontSize(9).FontFamily("Times New Roman (Headings CS)");

                                        text.Span("\n"); // Move content to a new line

                                        text.Span("                             "+ Stu_Degree_English).FontSize(10)
                                            .FontFamily("Times New Roman (Headings CS)");
                                    });
                            // Date of Graduation
                            table.Cell().ColumnSpan(2).Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("Date of Graduation: ").Bold()
                                            .FontSize(9).FontFamily("Times New Roman (Headings CS)");

                                        text.Span("\n"); // Move content to a new line

                                        text.Span("                             " + Stu_Date_Graduation).FontSize(10)
                                            .FontFamily("Times New Roman (Headings CS)");
                                    });

                        });

                        //Spacer
                        column.Item().PaddingVertical(3);

                        //int Total_Credit = 0;
                        int totalCreditsSemester1 = 0;
                        int totalCreditsSemester2 = 0;

                        //Student Subject Info
                        column.Item().Table(table2 =>
                        {
                            // Define table columns (relative to available space)
                            table2.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Course No1
                                columns.RelativeColumn(3); // Course Title1
                                columns.RelativeColumn(1); // No of Credit1
                                columns.RelativeColumn(1); // Grade1
                                columns.RelativeColumn(2); // Course No2
                                columns.RelativeColumn(3); // Course Title2
                                columns.RelativeColumn(1); // No of Credit2
                                columns.RelativeColumn(1); // Grade2
                            });

                            // Table Header
                            table2.Header(header =>
                            {
                                header.Cell().Border(1).PaddingTop(4).Text("Course No").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)").AlignCenter();
                                header.Cell().Border(1).PaddingTop(4).Text("Course Title").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)").AlignCenter();
                                header.Cell().Border(1).Padding(1).Text("No of Credit").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)").AlignCenter();
                                header.Cell().Border(1).PaddingTop(4).Text("Grade").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)").AlignCenter();
                                header.Cell().Border(1).PaddingTop(4).Text("Course No").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)").AlignCenter();
                                header.Cell().Border(1).PaddingTop(4).Text("Course Title").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)").AlignCenter();
                                header.Cell().Border(1).Padding(1).Text("No of Credit").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)").AlignCenter();
                                header.Cell().Border(1).PaddingTop(4).Text("Grade").Bold().FontSize(9).FontFamily("Times New Roman (Headings CS)").AlignCenter();
                            });

                            // **GROUP BY Study Year**
                            var groupedByYear = item.GroupBy(s => s.Report_StudyYear);

                            foreach (var yearGroup in groupedByYear)
                            {
                                // **GROUP SEMESTERS WITHIN THE STUDY YEAR**
                                var semester1Courses = yearGroup.Where(s => int.TryParse(s.Report_Study_Semester, out int semester) && semester == 1).ToList();
                                var semester2Courses = yearGroup.Where(s => int.TryParse(s.Report_Study_Semester, out int semester) && semester == 2).ToList();

                                // **Convert ReadOnlySpan<byte> to String and Parse Credits**
                                 totalCreditsSemester1 = semester1Courses
                                    .Sum(s => int.TryParse(s.Report_Study_Credit.ToString(), out int credits) ? credits : 0);

                                 totalCreditsSemester2 = semester2Courses
                                    .Sum(s => int.TryParse(s.Report_Study_Credit.ToString(), out int credits) ? credits : 0);

                                int maxRows = Math.Max(semester1Courses.Count, semester2Courses.Count);

                                // **Detecting RowSpan for Course No1**
                                
                                int semester1RowSpan = semester1Courses.Count; // Get the total number of rows for Semester 1

                                for (int i = 0; i < maxRows; i++)
                                {
                                    var semester1 = (i < semester1Courses.Count) ? semester1Courses[i] : null;
                                    var semester2 = (i < semester2Courses.Count) ? semester2Courses[i] : null;

                                    // **Ensure Course No1 only appears in the first row of Semester 1**
                                    if (semester1 != null)
                                    {
                                        if (i == 0) // Only apply RowSpan in the first row of this semester
                                        {
                                            table2.Cell().RowSpan((uint)semester1RowSpan) // Merge Course No1 into one cell
                                                .Border(1).AlignLeft().PaddingLeft(3).AlignMiddle().EnsureSpace()
                                                .Text($"{yearGroup.Key}, Sem:{semester1.Report_Study_Semester}").FontSize(8).FontFamily("Times New Roman (Headings CS)");
                                        }

                                        table2.Cell().Border(1).AlignLeft().PaddingLeft(5).AlignMiddle().EnsureSpace()
                                            .Text(semester1.Report_Study_Subject).FontSize(8).FontFamily("Khmer OS Siemreap");

                                        table2.Cell().Border(1).AlignCenter().Padding(4).EnsureSpace()
                                            .Text($"{semester1.Report_Study_Credit}").AlignCenter().FontSize(8).FontFamily("Times New Roman (Headings CS)");

                                        table2.Cell().Border(1).AlignCenter().PaddingLeft(3).AlignMiddle().EnsureSpace()
                                            .Text(semester1.Grade_Letter).FontSize(8).FontFamily("Times New Roman (Headings CS)");

                                    }
                                    else
                                    {
                                        table2.Cell().ColumnSpan(4).Border(1).Text(""); // Empty cells if no Semester 1 course
                                    }

                                    // **Semester 2 Logic (Similar to Semester 1)**
                                    if (semester2 != null)
                                    {
                                        if (i == 0) // Apply RowSpan only in the first row of Semester 2
                                        {
                                            int semester2RowSpan = semester2Courses.Count;
                                            table2.Cell().RowSpan((uint)semester2RowSpan)
                                                .Border(1).AlignLeft().PaddingLeft(3).AlignMiddle().EnsureSpace()
                                                .Text($"{yearGroup.Key}, Sem:{semester2.Report_Study_Semester}").FontSize(8).FontFamily("Times New Roman (Headings CS)");
                                        }

                                        table2.Cell().Border(1).AlignLeft().PaddingLeft(5).AlignMiddle().EnsureSpace()
                                            .Text(semester2.Report_Study_Subject).FontSize(8).FontFamily("Khmer OS Siemreap");

                                        table2.Cell().Border(1).AlignCenter().Padding(4).EnsureSpace()
                                            .Text($"{semester2.Report_Study_Credit}").FontSize(8).FontFamily("Times New Roman (Headings CS)");

                                        table2.Cell().Border(1).AlignCenter().PaddingLeft(3).AlignMiddle().EnsureSpace()
                                            .Text(semester2.Grade_Letter).FontSize(8).FontFamily("Times New Roman (Headings CS)");
                                    }
                                    else
                                    {
                                        table2.Cell().ColumnSpan(4).Border(1).Text(""); // Empty cells if no Semester 2 course
                                    }

                                    
                                }

                            }  
                        });

                        // **Calculate Total Credits**
                        // Ensure Report_Study_Credit is converted to a string before parsing
                        int Total_Credit = item.Sum(s => int.TryParse(s.Report_Study_Credit.ToString(), out int credits) ? credits : 0);


                        // **Calculate Cumulative GPA**
                        double totalGradePoints = 0;
                        Dictionary<string, double> gradeToGpa = new Dictionary<string, double>()
{
                                {"A", 4.0}, {"B+", 3.5}, {"B", 3.0},
                                {"C+", 2.5}, {"C", 2.0}, {"D", 1.0},
                                {"F", 0.0} // Failed courses contribute 0 to GPA
};

                        foreach (var course in item)
                        {
                            if (int.TryParse(course.Report_Study_Credit.ToString(), out int credits) &&
                                gradeToGpa.TryGetValue(course.Grade_Letter, out double gpaValue))
                            {
                                totalGradePoints += (credits * gpaValue); // Multiply credits by GPA value
                            }
                        }

                        // **Final Cumulative GPA Calculation**
                        //double Cumulative_GPA = Total_Credit > 0 ? totalGradePoints / Total_Credit : 0;
                        double Cumulative_GPA = Total_Credit > 0 ? Math.Round(totalGradePoints / Total_Credit, 2) : 0;


                        //Credit, GPA
                        column.Item().Table(table3 =>
                        {
                            // Define table columns (relative to available space)
                            table3.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(6); // Text Credit
                                columns.RelativeColumn(7); // Text GPA
                                columns.RelativeColumn(1); // ifo
                                
                            });
                            // Credit
                            table3.Cell().ColumnSpan(2) // Merge Course No1 into one cell
                                                .Border(1).AlignLeft().PaddingLeft(3).AlignMiddle().EnsureSpace()
                                                .Text($"Total Number of Credit: ").FontSize(8).FontFamily("Times New Roman (Headings CS)");
                            table3.Cell().Border(1).Padding(3).AlignCenter().EnsureSpace().Text($"{Total_Credit}").FontSize(9);


                            //GPA
                            table3.Cell().ColumnSpan(2) // Merge Course No1 into one cell
                                                .Border(1).AlignLeft().PaddingLeft(3).AlignMiddle().EnsureSpace()
                                                .Text($"Cumulative Grade Point Average(GPA): ").FontSize(8).FontFamily("Times New Roman (Headings CS)");
                            table3.Cell().Border(1).Padding(3).AlignCenter().EnsureSpace().Text($"{Cumulative_GPA}").FontSize(9);
                        });

                        //Detiels Image
                        column.Item().Table(table4 =>
                        {
                            // Define table columns (relative to available space)
                            table4.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(6); // Text Credit
                                columns.RelativeColumn(8); // Text, Image                             
                            });

                            //Info Text
                            table4.Cell().Border(1).Padding(2).BorderColor(Colors.Black)
                                    .Text(text =>
                                    {
                                        text.Span("                             Grading System: ").Bold()
                                            .FontSize(9).FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             4           A           Excellent").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             3.5        B+         Very Good").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             3           B           Good").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             2.5        C+         Fair").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             2           C           Passed").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             1.5        D           Poor").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             1           E            Very Poor").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             0           F            Failed").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("                           AU         Audit Student/No Credit").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("                           Cr          No Point Offered").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("                           I             Incomplete").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("                           IP           In Progress").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("             Note        1 Credit of Theory = 15 Hours").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("                              1 Credit of Practice = 30 Hours").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("                              1 Credit of Research = 45 Hours").FontSize(7)
                                            .FontFamily("Times New Roman (Headings CS)");
                                    });

                            // **Signature and Image Section**
                            table4.Cell().Border(1).PaddingTop(5).BorderColor(Colors.Black)
                                .Column(column =>
                                {
                                    column.Item().AlignMiddle().AlignCenter().Text(text =>
                                    {
                                        text.Span("Official transcript not valid without signature").Bold()
                                            .FontSize(9).FontFamily("Times New Roman (Headings CS)");

                                        text.Span("\n");
                                        text.Span("and seal of the RPITSSR").FontSize(10).Bold()
                                            .FontFamily("Times New Roman (Headings CS)");

                                        text.Span("\n");
                                        text.Span("Director").FontSize(11).Bold()
                                            .FontFamily("Times New Roman (Headings CS)");
                                        text.Span("\n");
                                        text.Span("\n");
                                    });

                                    ////Spacer
                                    //column.Item().PaddingVertical(20);

                                    // **Add Image Inside the Same Cell**
                                    column.Item().PaddingBottom(-19).AlignBottom().AlignLeft().Element(imageContainer =>
                                    {
                                        if (student_Info.ProfileImageBytes != null && student_Info.ProfileImageBytes.Length > 0)
                                        {
                                            using (var imageStream = new MemoryStream(student_Info.ProfileImageBytes))
                                            {
                                                imageContainer.Width(60).Height(100).Image(imageStream);
                                            }
                                        }
                                        else
                                        {
                                            imageContainer.Width(60).Height(100).Image("Assets/Student_Logo.png");
                                            column.Item().Text("(សិស្សនិស្សិតមិនមានរូបភាព)")
                                               .FontFamily("Khmer OS Siemreap")
                                               .FontSize(9)
                                               .AlignCenter();
                                        }
                                    });
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
