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

                // Role'e göre veri ekleme işlemi
                if (user.Role == UserRole.Assistant)
                {
                    var assistant = new Assistant
                    {
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
                return RedirectToAction(); // Başka bir sayfaya yönlendir
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
                    .ToListAsync(); var userDtos = users.Select(user => new UserDto
                    {
                        Email = user.Email,
                        Role = user.Role,
                        Password = user.Password,
                        FirstName = user.Role == UserRole.Assistant
                                ? _context.Assistants.FirstOrDefault(a => a.Email == user.Email)?.FirstName
                                : _context.Instructors.FirstOrDefault(i => i.Email == user.Email)?.FirstName,
                        LastName = user.Role == UserRole.Assistant
                                ? _context.Assistants.FirstOrDefault(a => a.Email == user.Email)?.LastName
                                : _context.Instructors.FirstOrDefault(i => i.Email == user.Email)?.LastName,
                        Address = user.Role == UserRole.Assistant
                                ? _context.Assistants.FirstOrDefault(a => a.Email == user.Email)?.Address
                                : _context.Instructors.FirstOrDefault(i => i.Email == user.Email)?.Address,
                        TelNo = user.Role == UserRole.Assistant
                                ? _context.Assistants.FirstOrDefault(a => a.Email == user.Email)?.TelNo
                                : _context.Instructors.FirstOrDefault(i => i.Email == user.Email)?.TelNo,
                    }).ToList();

            return View(userDtos); // UserDto türünde model gönderiliyor.
        }
        [HttpGet("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                FirstName = user.Role == UserRole.Assistant
                    ? _context.Assistants.FirstOrDefault(a => a.Email == user.Email)?.FirstName
                    : _context.Instructors.FirstOrDefault(i => i.Email == user.Email)?.FirstName,
                LastName = user.Role == UserRole.Assistant
                    ? _context.Assistants.FirstOrDefault(a => a.Email == user.Email)?.LastName
                    : _context.Instructors.FirstOrDefault(i => i.Email == user.Email)?.LastName,
                Address = user.Role == UserRole.Assistant
                    ? _context.Assistants.FirstOrDefault(a => a.Email == user.Email)?.Address
                    : _context.Instructors.FirstOrDefault(i => i.Email == user.Email)?.Address,
                TelNo = user.Role == UserRole.Assistant
                    ? _context.Assistants.FirstOrDefault(a => a.Email == user.Email)?.TelNo
                    : _context.Instructors.FirstOrDefault(i => i.Email == user.Email)?.TelNo,
            };

            return PartialView("_UpdateUserModal", userDto);
        }

        [HttpPost("update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(int id, UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = userDto.Email;
                user.Role = userDto.Role;
                user.Password = userDto.Password;

                if (user.Role == UserRole.Assistant)
                {
                    var assistant = await _context.Assistants.FirstOrDefaultAsync(a => a.Email == user.Email);
                    if (assistant != null)
                    {
                        assistant.FirstName = userDto.FirstName;
                        assistant.LastName = userDto.LastName;
                        assistant.Address = userDto.Address;
                        assistant.TelNo = userDto.TelNo;
                    }
                }
                else if (user.Role == UserRole.Instructor)
                {
                    var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Email == user.Email);
                    if (instructor != null)
                    {
                        instructor.FirstName = userDto.FirstName;
                        instructor.LastName = userDto.LastName;
                        instructor.Address = userDto.Address;
                        instructor.TelNo = userDto.TelNo;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Message"] = "Güncelleme başarılı!";
                return RedirectToAction("Index");
            }
 if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                foreach (var error in errors)
                {
                    Console.WriteLine(error); // Veya loglama yapabilirsiniz
                }
            }
            return View("Index");
        }

    }
}