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
                    // Kullanıcı bilgilerini Claims ile sakla
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Kullanıcı ID'si
                new Claim(ClaimTypes.Email, user.Email), // Kullanıcı E-posta
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Kullanıcı Rolü
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Kalıcı oturum
                        ExpiresUtc = DateTime.UtcNow.AddHours(1) // Oturum süresi
                    };

                    // Oturum başlat
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

        // Admin giriş sayfası (GET)
        [HttpGet]
        public IActionResult LoginByAdmin()
        {
            // Admin login sayfasını döndür
            return View();
        }

        // Admin giriş kontrolü (POST)
       [HttpPost]
        public IActionResult LoginByAdmin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcıyı ve rolünü kontrol et
                var user = _context.Users
                    .FirstOrDefault(u => u.Email == model.Email &&
                                        u.Password == model.Password &&
                                        u.Role == UserRole.Admin);  // UserRole.Admin enum kontrolü

                if (user != null)
                {
                    // Admin bilgilerini Claims ile sakla
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Kullanıcı ID'si
                        new Claim(ClaimTypes.Email, user.Email),                 // Kullanıcı E-posta
                        new Claim(ClaimTypes.Role, user.Role.ToString())         // Kullanıcı Rolü (Admin)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,                       // Kalıcı oturum
                        ExpiresUtc = DateTime.UtcNow.AddHours(1)   // Oturum süresi
                    };

                    // Oturum başlat
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    TempData["Message"] = "Admin Girişi Başarılı!";
                    return RedirectToAction("Index", "AdminHome");  // AdminHome controller'ındaki Index action'ına yönlendir
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz giriş veya admin yetkisi yok.");
                }
            }

            return View("LoginByAdmin", model);  // Admin giriş sayfasına geri döner
        }

    }

    // Giriş modeli
    public class LoginViewModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}