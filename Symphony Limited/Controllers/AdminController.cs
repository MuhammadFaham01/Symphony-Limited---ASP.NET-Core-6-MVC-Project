using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony_Limited.Data;
using Symphony_Limited.Models;

namespace Symphony_Limited.Controllers
{
    public class AdminController : Controller
    {
        private readonly Symphony_LtdContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminController(Symphony_LtdContext db, IHttpContextAccessor httpContextAccessor)
        {
            this.db=db;
            _httpContextAccessor=httpContextAccessor;
        }


        public IActionResult Home()
        {
            if (HttpContext.Session.GetString("usersession") != null && HttpContext.Session.GetString("userrole") == "Admin")
            {
                ViewBag.mysession = HttpContext.Session.GetString("usersession");

                int totalUsers = db.UsersTbls.Count();
                ViewBag.TotalUsers = totalUsers;

                int totalcourse = db.Courses.Count();
                ViewBag.TotalCourse = totalcourse;

                int totalbooks = db.Books.Count();
                ViewBag.TotalBooks = totalbooks;

                int teachers = db.Instructors.Count();
                ViewBag.Instructors = teachers;

                int messageCount = db.ContactMessages.Any() ? db.ContactMessages.Count() : 0;
                ViewBag.NotificationCount = messageCount;

                var senderNames = db.ContactMessages
                    .Select(m => m.Name)
                    .Distinct()
                    .ToList();
                ViewBag.SenderNames = senderNames;

                return View();
            }
            return RedirectToAction("Login", "Account");
        }


        public IActionResult userdetails(UsersTbl user)
        {

            return View();
        }

        public IActionResult ManageUsers()
        {
            if (HttpContext.Session.GetString("usersession") != null && HttpContext.Session.GetString("userrole") == "Admin")
            {
               
                
                var allUsers = db.UsersTbls.ToList();
                return View(allUsers);
            }
            return RedirectToAction("Login", "Account");
        }

     


       
        public IActionResult myprofile()
        {
            string userEmail = HttpContext.Session.GetString("usersession"); 

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = db.UsersTbls.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [HttpPost]
        public JsonResult MarkNotificationsAsRead()
        {
            try
            {
                string userEmail = HttpContext.Session.GetString("usersession");




                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the error (important for debugging)
                Console.Error.WriteLine(ex.Message);

                return Json(new { success = false });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult myprofile(UsersTbl updatedUser, string OldPassword, string NewPassword)
        {
            string userEmail = HttpContext.Session.GetString("usersession");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = db.UsersTbls.FirstOrDefault(u => u.Email == userEmail);

            if (user != null)
            {
                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;

                if (!string.IsNullOrEmpty(NewPassword))
                {
                    var passwordHasher = new PasswordHasher<UsersTbl>();

                    var result = passwordHasher.VerifyHashedPassword(user, user.Password, OldPassword);

                    if (result == PasswordVerificationResult.Success)
                    {
                        user.Password = passwordHasher.HashPassword(user, NewPassword);
                        db.SaveChanges();
                        ViewBag.Message = "Password updated successfully!";
                        ViewBag.IsSuccess = true; // ✅ Success Alert
                    }
                    else
                    {
                        ViewBag.Message = "Incorrect current password!";
                        ViewBag.IsSuccess = false; // ❌ Error Alert
                        return View(user);
                    }
                }
                else
                {
                    db.SaveChanges();
                    ViewBag.Message = "Profile updated successfully!";
                    ViewBag.IsSuccess = true; // ✅ Success Alert
                }
            }

            return View(user);
        }
    }
}


