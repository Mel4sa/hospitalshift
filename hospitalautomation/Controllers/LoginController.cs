using System.Linq;
using hospitalautomation.Models.Context;
using hospitalautomation.Models.Enum;
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
            // TempData'daki mesajı ViewBag'e aktar
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }

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
                    // Başarılı giriş sonrası mesajı TempData'ya kaydet
                    TempData["Message"] = "Giriş başarılı!";

                    // Kullanıcıyı Home sayfasına yönlendir
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
        public IActionResult AdminLogin()
        {
            // Admin login sayfasını döndür
            return View();
        }

        // Admin giriş kontrolü (POST)
        [HttpPost]
        public IActionResult LoginbyAdmin(LoginViewModel model)
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
                    // Eğer adminse, admin sayfasına yönlendir
                    TempData["Message"] = "Admin Girişi Başarılı!";
                    return RedirectToAction("Index", "AdminHome");  // AdminHome controller'ındaki Index action'ına yönlendir
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz giriş veya admin yetkisi yok.");
                }
            }

            return View("AdminLogin", model);  // Admin giriş sayfasına geri döner
        }
    }

    // Giriş modeli
    public class LoginViewModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}