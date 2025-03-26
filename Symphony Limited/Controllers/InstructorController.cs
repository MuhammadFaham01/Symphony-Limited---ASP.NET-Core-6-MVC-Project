using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony_Limited.Data;
using Symphony_Limited.Models;

namespace Symphony_Limited.Controllers
{
    public class InstructorController : Controller
    {
        private readonly Symphony_LtdContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InstructorController(Symphony_LtdContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var instructors = _context.Instructors.ToList(); // Database se instructors fetch karein
        if (instructors == null || !instructors.Any()) 
        {
            return View(new List<Instructor>()); // Empty list pass karein
        }
        return View(instructors);
        }        

        public IActionResult Create()
        {
            if (_httpContextAccessor.HttpContext.Session.GetString("usersession") != null &&
                _httpContextAccessor.HttpContext.Session.GetString("userrole") == "Admin")
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Instructor instructor, IFormFile imageFile)
        {
            if (_httpContextAccessor.HttpContext.Session.GetString("usersession") == null ||
                _httpContextAccessor.HttpContext.Session.GetString("userrole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            // FIXED: Corrected ModelState check
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(uploadDir, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }

                    instructor.ImagePath = "/images/" + uniqueFileName;
                }

                _context.Instructors.Add(instructor); // Ensure you're adding to the correct DbSet
                Console.WriteLine("Instructor Added: " + instructor.Name);

                _context.SaveChanges(); // Ensure changes are saved
                Console.WriteLine("Data Saved to Database!");

                return RedirectToAction(nameof(Index));
            }

            // FIXED: Log validation errors for debugging
            Console.WriteLine("ModelState is Invalid!");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(instructor);
        }




        public IActionResult Edit(int? id)
        {
            if (_httpContextAccessor.HttpContext.Session.GetString("usersession") == null ||
                _httpContextAccessor.HttpContext.Session.GetString("userrole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();
            var instructor = _context.Instructors.Find(id);
            if (instructor == null) return NotFound();
            return View(instructor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Instructor instructor, IFormFile imageFile)
        {
            if (_httpContextAccessor.HttpContext.Session.GetString("usersession") == null ||
                _httpContextAccessor.HttpContext.Session.GetString("userrole") != "Admin")  
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != instructor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        // Ensure the images folder exists
                        string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        // Save the new image
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = Path.Combine(uploadDir, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(fileStream);
                        }

                        instructor.ImagePath = "/images/" + uniqueFileName;
                    }

                    _context.Update(instructor);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Instructors.Any(e => e.Id == instructor.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }


        public IActionResult Delete(int? id)
        {
            if (_httpContextAccessor.HttpContext.Session.GetString("usersession") == null ||
                _httpContextAccessor.HttpContext.Session.GetString("userrole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();
            var instructor = _context.Instructors.Find(id);
            if (instructor == null) return NotFound();
            return View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_httpContextAccessor.HttpContext.Session.GetString("usersession") == null ||
                _httpContextAccessor.HttpContext.Session.GetString("userrole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var instructor = _context.Instructors.Find(id);
            if (instructor != null)
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, instructor.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _context.Instructors.Remove(instructor);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}