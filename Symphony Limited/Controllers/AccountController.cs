using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Symphony_Limited.Data;
using Symphony_Limited.Models;

namespace Symphony_Limited.Controllers
{
    public class AccountController : Controller
    {
        private readonly Symphony_LtdContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(Symphony_LtdContext db, IHttpContextAccessor httpContextAccessor)
        {
            this.db=db;
            _httpContextAccessor=httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UsersTbl user)
        {
            if (db.UsersTbls.Any(x => x.Email == user.Email))
            {
                ViewBag.Message = "Email already exists!";
                return View();
            }

            // Password Hashing
            var passwordHasher = new PasswordHasher<UsersTbl>();
            user.Password = passwordHasher.HashPassword(user, user.Password);

            user.Role = "User"; // Default role is User
            db.UsersTbls.Add(user);
            db.SaveChanges();

            ViewBag.Message = "Registration successful! Please login.";
            return RedirectToAction("login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UsersTbl user)
        {
            var myUser = db.UsersTbls.FirstOrDefault(x => x.Email == user.Email);

            if (myUser != null)
            {
                var passwordHasher = new PasswordHasher<UsersTbl>();
                var result = passwordHasher.VerifyHashedPassword(myUser, myUser.Password, user.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("username", myUser.Username);
                    HttpContext.Session.SetString("usersession", myUser.Email);
                    HttpContext.Session.SetString("userrole", myUser.Role);

                    return myUser.Role == "Admin" ? RedirectToAction("Home", "Admin") : RedirectToAction("Home", "User");
                }
            }

            ViewBag.Message = "Login Failed. Please check email and password.";
            return View();
        }


        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("usersession") != null)
            {
                HttpContext.Session.Remove("usersession");
            }
            return RedirectToAction("Login", "Account"); // Or any other redirect you want
        }


        [HttpGet]
        public IActionResult GenerateHashedPassword()
        {
            var passwordHasher = new PasswordHasher<object>();
            string plainPassword = "faham123"; // Admin ka actual password
            string hashedPassword = passwordHasher.HashPassword(null, plainPassword);

            return Content("Hashed Password: " + hashedPassword);
        }



    }
}
