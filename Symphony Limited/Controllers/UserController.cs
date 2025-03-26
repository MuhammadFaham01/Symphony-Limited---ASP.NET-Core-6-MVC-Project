using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Symphony_Limited.Data;
using Symphony_Limited.Models;
using Microsoft.Extensions.Logging; // Add this using directive
using System.Threading.Tasks; // Add this using directive for async

namespace Symphony_Limited.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Symphony_LtdContext db;

    public UserController(ILogger<UserController> logger, IHttpContextAccessor httpContextAccessor, Symphony_LtdContext db)
    {
        this.db = db;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Home()
    {

        var viewModel = new HomeViewModel
        {
            Courses = db.Courses.ToList(),
            Users = db.UsersTbls.FirstOrDefault() // Single user
        };

        return View(viewModel);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult FreeCourse()
    {
        var courses = db.Courses.ToList(); // Fetch courses from database
        return View(courses);
    }

    public IActionResult PremiumCourse()
    {

        //var premiumCourses = db.Courses.Where(c => c.Type == "Premium").ToList();
        //return View(premiumCourses);     
        string userEmail = _httpContextAccessor.HttpContext.Session.GetString("usersession");
        if (userEmail == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = db.UsersTbls.FirstOrDefault(u => u.Email == userEmail);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        bool hasPremiumAccess = db.PremiumAccesses
            .Any(pa => pa.UserId == user.Id && pa.Status == "Approved");

        if (hasPremiumAccess)
        {
            var premiumCourses = db.Courses.Where(c => c.Type == "Premium").ToList();
            return View(premiumCourses);
        }
        else
        {
            ViewBag.Message = "You need premium access to view this page.";
            return RedirectToAction("PaymentRequired", "PremiumAccess"); // Create a view named PaymentRequired.cshtml
        }
    }

    public IActionResult Instructor()
    {
        string userEmail = HttpContext.Session.GetString("usersession");

        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "Account");
        }
        var instructors = db.Instructors.ToList(); // Database se instructors fetch karein
        if (instructors == null || !instructors.Any())
        {
            return View(new List<Instructor>()); // Empty list pass karein
        }
        return View(instructors);
    }

    public IActionResult Library()
    {

        string userEmail = HttpContext.Session.GetString("usersession");

        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "Account");
        }
        var books = db.Books.ToList();
        return View(books);
    }
   
    public IActionResult Contact()
    {
        string userEmail = HttpContext.Session.GetString("usersession");

        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "Account");
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Contact(ContactMessage model)
    {
        if (ModelState.IsValid)
        {
            db.ContactMessages.Add(model);
            await db.SaveChangesAsync();

            try
            {
                string adminEmail = "mfaham871@gmail.com";
                string subject = "SymphonyLtd: " + model.Subject;
                string body = $"Name: {model.Name}\nEmail: {model.Email}\nMessage:\n{model.Message}";

                await SendEmailAsync(adminEmail, subject, body);

                // Update session count safely
                int count = _httpContextAccessor.HttpContext.Session.GetInt32("NotificationCount") ?? 0;
                _httpContextAccessor.HttpContext.Session.SetInt32("NotificationCount", count + 1);
                ViewBag.NotificationCount = count + 1;

                ViewBag.Message = "Your message has been sent. We will contact you soon.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email: {ErrorMessage}", ex.Message);
                ViewBag.Message = "An error occurred while sending your message. Please try again later.";
                return View();
            }
        }
        return View(model);
    }


    // Email Sending Method (Async)
    private async Task SendEmailAsync(string to, string subject, string body) // Make async
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com") // Example for gmail
            {
                Port = 587,
                Credentials = new NetworkCredential("mfaham871@gmail.com", "xkxv govz ejiz fyaf"), // Use app password for gmail
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("mfaham871@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage); // Use SendMailAsync
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email: {ErrorMessage}", ex.Message);
            if (ex.InnerException != null)
            {
                _logger.LogError(ex, "Inner Exception: {InnerErrorMessage}", ex.InnerException.Message);
            }
            throw; // Re-throw the exception for the caller to handle
        }
    }




    // Action to show the exam page
    public IActionResult TakeExam(int courseId)
    {
        // Create an exam for the course (if it doesn't exist)
        var exam = db.Exams
            .Include(e => e.Course) // Load the Course
            .FirstOrDefault(e => e.CourseId == courseId);

        if (exam == null)
        {
            // If no exam found, create one
            exam = new Exam { CourseId = courseId };
            db.Exams.Add(exam);
            db.SaveChanges();
            // Generate questions for the exam
            GenerateQuestions(exam.Id); // You'll create this method
        }

        // Fetch the exam with its questions and options
        var examWithQuestions = db.Exams
            .Include(e => e.Course) // Load the Course
            .Include(e => e.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefault(e => e.Id == exam.Id);

        if (examWithQuestions == null)
        {
            return NotFound(); // Exam not found
        }

        return View(examWithQuestions);
    }

    // Method to generate MCQs (you need to implement this)
    private void GenerateQuestions(int examId)
    {
  
        // Example Dummy Questions:
        var question1 = new Question
        {
            ExamId = examId,
            QuestionText = "What is the primary function of the UsersTbl table?"
        };
        db.Questions.Add(question1);
        db.SaveChanges();

        var option1A = new Option
        {
            QuestionId = question1.Id,
            OptionText = "Stores user data",
            IsCorrect = true
        };
        var option1B = new Option
        {
            QuestionId = question1.Id,
            OptionText = "Stores course data",
            IsCorrect = false
        };
        db.Options.AddRange(option1A, option1B);
        db.SaveChanges();

        // Add more questions and options as needed...
    }

    // Action to submit the exam
    [HttpPost]
    public IActionResult SubmitExam(IFormCollection form)
    {
        // Get the current user
        string userEmail = _httpContextAccessor.HttpContext.Session.GetString("usersession");
        var user = db.UsersTbls.FirstOrDefault(u => u.Email == userEmail);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Get the exam ID
        int examId = int.Parse(form["ExamId"]);

        // Calculate the score
        int score = 0;
        var exam = db.Exams
            .Include(e => e.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefault(e => e.Id == examId);

        if (exam != null)
        {
            foreach (var question in exam.Questions)
            {
                string selectedOptionId = form["question_" + question.Id];
                if (!string.IsNullOrEmpty(selectedOptionId))
                {
                    int selectedOptionIdInt = int.Parse(selectedOptionId);
                    var selectedOption = question.Options.FirstOrDefault(o => o.Id == selectedOptionIdInt);
                    if (selectedOption != null && selectedOption.IsCorrect)
                    {
                        score++;
                    }
                }
            }

            // Save the exam result
            var examResult = new ExamResult
            {
                ExamId = examId,
                UserId = user.Id,
                Score = score
            };
            db.ExamResults.Add(examResult);
            db.SaveChanges();

            // Redirect to the certificate generation action
            return RedirectToAction("GenerateCertificate", new { examResultId = examResult.Id });
        }

        return BadRequest("Exam not found.");
    }

    // Action to generate the certificate
    public IActionResult GenerateCertificate(int examResultId)
    {
        // Fetch the exam result with related data
        var examResult = db.ExamResults
            .Include(er => er.Exam)
            .ThenInclude(e => e.Course)
            .Include(er => er.User)
            .FirstOrDefault(er => er.Id == examResultId);

        if (examResult == null)
        {
            return NotFound();
        }

        return View(examResult);
    }


public IActionResult DownloadCertificatePdf(int examResultId)
{
    var examResult = db.ExamResults
        .Include(er => er.Exam)
        .ThenInclude(e => e.Course)
        .Include(er => er.User)
        .FirstOrDefault(er => er.Id == examResultId);

    if (examResult == null)
    {
        return NotFound();
    }

    string fileName = $"Certificate_{examResultId}_{DateTime.Now.Ticks}.pdf";
    string filePath = Path.Combine(Path.GetTempPath(), fileName);

    try
    {
        using (var pdfWriter = new PdfWriter(filePath))
        {
            using (var pdf = new PdfDocument(pdfWriter))
            {
                var document = new Document(pdf);

                // Load fonts
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont italicFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

                // Border Styling
                document.SetMargins(50, 50, 50, 50);
                document.Add(new LineSeparator(new SolidLine()).SetMarginBottom(10));

                // Title
                document.Add(new Paragraph("Certificate of Completion")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(boldFont)
                    .SetFontSize(26)
                    .SetMarginBottom(20));

                document.Add(new LineSeparator(new SolidLine()).SetMarginBottom(10));

                // Recipient Name
                document.Add(new Paragraph("Awarded to")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(normalFont)
                    .SetFontSize(18));

                document.Add(new Paragraph(examResult.User.Username)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(boldFont)
                    .SetFontSize(22)
                    .SetMarginBottom(15));

                // Course Name
                document.Add(new Paragraph("For successfully completing the course:")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(normalFont)
                    .SetFontSize(16));

                document.Add(new Paragraph(examResult.Exam.Course.Title)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(boldFont)
                    .SetFontSize(20)
                    .SetMarginBottom(15));

                // Score
                document.Add(new Paragraph($"Score: {examResult.Score}/100")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(boldFont)
                    .SetFontSize(16)
                    .SetMarginBottom(10));

                // Date
                document.Add(new Paragraph($"Date: {examResult.ResultDate:MMMM dd, yyyy}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(italicFont)
                    .SetFontSize(14));

                // Footer Line
                document.Add(new LineSeparator(new SolidLine()).SetMarginTop(20));

                // Signature Placeholder
                document.Add(new Paragraph("Authorized Signature")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(normalFont)
                    .SetFontSize(14)
                    .SetMarginTop(40));
            }
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/pdf", "Certificate.pdf");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"An error occurred: {ex.Message}");
    }
    finally
    {
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }
}



[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}