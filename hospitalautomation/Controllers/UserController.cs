using hospitalautomation.Models;
using hospitalautomation.Models.Context;
using hospitalautomation.Models.Dtos;
using hospitalautomation.Models.Enum;
using Microsoft.AspNetCore.Mvc;

namespace hospitalautomation.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
         private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context; // DbContext'i enjekte ettik

        public UserController(ILogger<UserController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // DbContext'i constructor ile alıyoruz
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet("create")]
        public IActionResult CreateUser()
        {
            return View("Index");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcıyı User entity'sine dönüştürüyoruz
                var user = new User
                {
                    Email = userDto.Email,
                    Password = userDto.Password,
                    Role = userDto.Role,
                    // Diğer gerekli alanları da ekleyin
                };

                // Kullanıcıyı veritabanına kaydediyoruz
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Role'e göre veri ekleme işlemi
                if (user.Role == UserRole.Assistant)
                {
                    var assistant = new Assistant
                    {
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Email = userDto.Email,
                        Address = userDto.Address,
                    };
                    _context.Assistants.Add(assistant);
                }
                else if (user.Role == UserRole.Instructor)
                {
                    var instructor = new Instructor
                    {
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Email = userDto.Email,
                        Address = userDto.Address,
                        
                    };
                    _context.Instructors.Add(instructor);
                }

                await _context.SaveChangesAsync();
                TempData["Message"] = "Başarılı!";
                return RedirectToAction(); // Başka bir sayfaya yönlendir
            }

            return View("Index"); // Eğer form geçerli değilse tekrar formu göster
        }
    }
}