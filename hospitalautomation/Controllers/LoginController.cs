using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hospitalautomation.Controllers
{

    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Sabit kullanıcı doğrulama
                if (model.Email == "user@example.com" && model.Password == "password123")
                {
                    ViewBag.Message = "Giriş başarılı!";
                    return View();
                }

                // Giriş başarısız
                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
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
