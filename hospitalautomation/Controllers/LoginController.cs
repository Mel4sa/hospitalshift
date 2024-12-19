using System.Linq;
using System.Security.Claims;
using hospitalautomation.Models.Context;
using hospitalautomation.Models.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hospitalautomation.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.Email == model.Email);

                if (user != null && user.Password == model.Password)
                {
            
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()) 
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(1) 
                    };

                   
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    TempData["Message"] = "Giriş başarılı!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Lütfen e-posta adresinizi ve şifrenizi kontrol ediniz.");
                }
            }

            return View("Index", model);
        }


        [HttpGet]
        public IActionResult LoginByAdmin()
        {
            return View();
        }

       [HttpPost]
        public IActionResult LoginByAdmin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = _context.Users
                    .FirstOrDefault(u => u.Email == model.Email &&
                                        u.Password == model.Password &&
                                        u.Role == UserRole.Admin);  

                if (user != null)
                {
                 
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                        new Claim(ClaimTypes.Email, user.Email),           
                        new Claim(ClaimTypes.Role, user.Role.ToString())         
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,                 
                        ExpiresUtc = DateTime.UtcNow.AddHours(1)  
                    };

                 
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    TempData["Message"] = "Admin Girişi Başarılı!";
                    return RedirectToAction("Index", "AdminHome");  
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz giriş veya admin yetkisi yok.");
                }
            }

            return View("LoginByAdmin", model);  
        }

    }


    public class LoginViewModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}