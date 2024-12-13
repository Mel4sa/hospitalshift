using hospitalautomation.Models;
using hospitalautomation.Models.Context;
using hospitalautomation.Models.Dtos;
using hospitalautomation.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

                // Kaydedilen kullanıcı ID'sini alıyoruz
                var userId = user.Id;

                // Role'e göre veri ekleme işlemi
                if (user.Role == UserRole.Assistant)
                {
                    var assistant = new Assistant
                    {
                        UserId = userId, // User tablosundan gelen ID
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Email = userDto.Email,
                        Address = userDto.Address,
                        TelNo = userDto.TelNo,
                    };
                    _context.Assistants.Add(assistant);
                }
                else if (user.Role == UserRole.Instructor)
                {
                    var instructor = new Instructor
                    {
                        UserId = userId, // User tablosundan gelen ID
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Email = userDto.Email,
                        Address = userDto.Address,
                        TelNo = userDto.TelNo,
                    };
                    _context.Instructors.Add(instructor);
                }

                await _context.SaveChangesAsync();
                TempData["Message"] = "Başarılı!";
                return RedirectToAction("Index"); // Başka bir sayfaya yönlendir
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                foreach (var error in errors)
                {
                    Console.WriteLine(error); // Veya loglama yapabilirsiniz
                }
            }
            return View("Index"); // Eğer form geçerli değilse tekrar formu göster
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                    .Where(user => user.Role != UserRole.Admin && !user.IsDeleted)
                    .ToListAsync();

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                Password = user.Password,
                FirstName = user.Role == UserRole.Assistant
                        ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.FirstName
                        : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.FirstName,
                LastName = user.Role == UserRole.Assistant
                        ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.LastName
                        : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.LastName,
                Address = user.Role == UserRole.Assistant
                        ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.Address
                        : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.Address,
                TelNo = user.Role == UserRole.Assistant
                        ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.TelNo
                        : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.TelNo,
            }).ToList();

            return View(userDtos); // UserDto türünde model gönderiliyor.
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                // Kullanıcıyı pasif yap (IsDeleted = true)
                user.IsDeleted = true;

                // Assistants tablosunda kullanıcıya ait veriyi güncelle
                var assistants = _context.Assistants.Where(a => a.UserId == userId);
                foreach (var assistant in assistants)
                {
                    assistant.IsDeleted = true;
                }

                // Instructors tablosunda kullanıcıya ait veriyi güncelle
                var instructors = _context.Instructors.Where(i => i.UserId == userId);
                foreach (var instructor in instructors)
                {
                    instructor.IsDeleted = true;
                }

                // Veritabanındaki değişiklikleri kaydet
                await _context.SaveChangesAsync();

                TempData["Message"] = "Kullanıcı başarıyla silindi!";
            }
            else
            {
                TempData["Error"] = "Kullanıcı bulunamadı!";
            }

            return RedirectToAction("Index");
        }
        [HttpGet("GetUser/{id}")]
public async Task<IActionResult> GetUser(int id)
{
    var user = await _context.Users
        .Where(user => user.Id == id && user.Role != UserRole.Admin && !user.IsDeleted)
        .FirstOrDefaultAsync();

    if (user == null)
    {
        return NotFound();
    }

    var userDto = new UserDto
    {
        Id = user.Id,
        Email = user.Email,
        Role = user.Role,
        Password = user.Password,
        FirstName = user.Role == UserRole.Assistant
                    ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.FirstName
                    : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.FirstName,
        LastName = user.Role == UserRole.Assistant
                    ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.LastName
                    : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.LastName,
        Address = user.Role == UserRole.Assistant
                    ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.Address
                    : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.Address,
        TelNo = user.Role == UserRole.Assistant
                    ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.TelNo
                    : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.TelNo,
    };

    return Json(userDto);
}
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(userDto.Id);
                if (user != null)
                {
                    user.Email = userDto.Email;
                    user.Password = userDto.Password;
                    user.Role = userDto.Role;

                    // Asistan ya da Öğrenim Üyesi güncelleme işlemleri
                    if (userDto.Role == UserRole.Assistant)
                    {
                        var assistant = await _context.Assistants.FirstOrDefaultAsync(a => a.UserId == userDto.Id);
                        if (assistant != null)
                        {
                            assistant.FirstName = userDto.FirstName;
                            assistant.LastName = userDto.LastName;
                            assistant.Email = userDto.Email;
                            assistant.Address = userDto.Address;
                            assistant.TelNo = userDto.TelNo;
                        }
                    }
                    else if (userDto.Role == UserRole.Instructor)
                    {
                        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userDto.Id);
                        if (instructor != null)
                        {
                            instructor.FirstName = userDto.FirstName;
                            instructor.LastName = userDto.LastName;
                            instructor.Email = userDto.Email;
                            instructor.Address = userDto.Address;
                            instructor.TelNo = userDto.TelNo;
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Kullanıcı başarıyla güncellendi!";
                }
                else
                {
                    TempData["Error"] = "Kullanıcı bulunamadı!";
                }

                return RedirectToAction("Index");
            }

            return View("Index");
        }
    }
}