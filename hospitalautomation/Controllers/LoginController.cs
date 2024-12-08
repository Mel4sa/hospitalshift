using System;
using System.Linq;
using hospitalautomation.Models.Context;
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
public IActionResult Index(LoginViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Email == model.Email);

        if (user != null && user.Password == model.Password)
        {
            TempData["Message"] = "Giriş başarılı!";
            return RedirectToAction("Index");
        }
        else
        {
            ModelState.AddModelError("", "Lütfen e-posta adresinizi ve şifrenizi kontrol ediniz.");
        }
    }

    return View(model);
}
    }

    // Giriş modeli
    public class LoginViewModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}