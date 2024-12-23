using hospitalautomation.Models;
using hospitalautomation.Models.Context;
using hospitalautomation.Models.Dtos;
using hospitalautomation.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hospitalautomation.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context; 

        public UserController(ILogger<UserController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; 
        }


        [HttpGet("Instructortable")]
        public async Task<IActionResult> Instructortable()
        {
            var instructors = await _context.Instructors
               .Where(a => !a.IsDeleted) 
               .ToListAsync();

            return View(instructors);
        }

        [HttpGet("GetInstructorDetails/{id}")]
        public async Task<IActionResult> GetInstructorDetails(int id)
        {
            var instructor = await _context.Instructors
                .Where(a => a.Id == id && !a.IsDeleted)
                .FirstOrDefaultAsync();

            if (instructor == null)
                return NotFound();

            return Json(new
            {
                instructor.FirstName,
                instructor.LastName,
                instructor.Email,
                instructor.Address,
                instructor.TelNo
            });
        }

        [HttpGet("Assistantable")]
        public async Task<IActionResult> Assistantable()
        {
            var assistants = await _context.Assistants
                .Where(a => !a.IsDeleted) 
                .ToListAsync();

            return View(assistants);
        }

        [HttpGet("GetAssistantDetails/{id}")]
        public async Task<IActionResult> GetAssistantDetails(int id)
        {
            var assistant = await _context.Assistants
                .Where(a => a.Id == id && !a.IsDeleted)
                .FirstOrDefaultAsync();

            if (assistant == null)
                return NotFound();

            return Json(new
            {
                assistant.FirstName,
                assistant.LastName,
                assistant.Email,
                assistant.Address,
                assistant.TelNo
            });
        }

        [HttpGet("create")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser()
        {
            return View("Index");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        Email = userDto.Email,
                        Password = userDto.Password,
                        Role = userDto.Role,
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    var userId = user.Id;

                    if (user.Role == UserRole.Asistan)
                    {
                        var assistant = new Assistant
                        {
                            UserId = userId,
                            FirstName = userDto.FirstName,
                            LastName = userDto.LastName,
                            Email = userDto.Email,
                            Address = userDto.Address,
                            TelNo = userDto.TelNo,
                        };
                        _context.Assistants.Add(assistant);
                    }
                    if (user.Role == UserRole.ÖğretimÜyesi)
                    {
                        if (!userDto.DepartmentId.HasValue)
                        {
                            TempData["Error"] = "Bölüm seçilmesi zorunludur.";
                            return RedirectToAction("Index");
                        }

                        var instructor = new Instructor
                        {
                            UserId = userId,
                            FirstName = userDto.FirstName,
                            LastName = userDto.LastName,
                            Email = userDto.Email,
                            Address = userDto.Address,
                            TelNo = userDto.TelNo,
                            DepartmentId = userDto.DepartmentId.Value
                        };

                        _context.Instructors.Add(instructor);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Kullanıcı başarıyla eklendi.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "Formda eksik veya hatalı bilgiler var.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet("")]
        [Authorize(Roles = "Admin")]
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
                FirstName = user.Role == UserRole.Asistan
                        ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.FirstName
                        : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.FirstName,
                LastName = user.Role == UserRole.Asistan
                        ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.LastName
                        : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.LastName,
                Address = user.Role == UserRole.Asistan
                        ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.Address
                        : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.Address,
                TelNo = user.Role == UserRole.Asistan
                        ? _context.Assistants.FirstOrDefault(a => a.UserId == user.Id)?.TelNo
                        : _context.Instructors.FirstOrDefault(i => i.UserId == user.Id)?.TelNo,
            }).ToList();
           
            var departments = await _context.Departments
                .Where(d => !d.IsDeleted)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();

            ViewBag.Departments = departments;
            return View(userDtos); 
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
              
                user.IsDeleted = true;

               
                var assistants = _context.Assistants.Where(a => a.UserId == userId);
                foreach (var assistant in assistants)
                {
                    assistant.IsDeleted = true;
                }

               
                var instructors = _context.Instructors.Where(i => i.UserId == userId);
                foreach (var instructor in instructors)
                {
                    instructor.IsDeleted = true;
                }

                
                await _context.SaveChangesAsync();

                TempData["Message"] = "Kullanıcı başarıyla silindi!";
            }
            else
            {
                TempData["Error"] = "Kullanıcı bulunamadı!";
            }

            return RedirectToAction("Index");
        }
        [HttpGet("get-user/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDeleted)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

           
            if (user.Role == UserRole.Asistan)
            {
                var assistant = await _context.Assistants.FirstOrDefaultAsync(a => a.UserId == userId);
                if (assistant == null) return NotFound("Kullanıcı detayları bulunamadı.");

                return Json(new
                {
                    user.Id,
                    user.Email,
                    user.Role,
                    assistant.FirstName,
                    assistant.LastName,
                    assistant.Address,
                    assistant.TelNo
                });
            }
            else if (user.Role == UserRole.ÖğretimÜyesi)
            {
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
                if (instructor == null) return NotFound("Kullanıcı detayları bulunamadı.");

                return Json(new
                {
                    user.Id,
                    user.Email,
                    user.Role,
                    instructor.FirstName,
                    instructor.LastName,
                    instructor.Address,
                    instructor.TelNo,

                });
            }

            return NotFound("Kullanıcı detayları bulunamadı.");
        }

        [HttpPost("update-user")]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            var user = await _context.Users.FindAsync(userDto.Id);
            if (user == null || user.IsDeleted)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index");
            }

      
            user.Email = userDto.Email;
            user.Password = userDto.Password;
            user.Role = userDto.Role;

      
            if (user.Role == UserRole.Asistan)
            {
                var assistant = await _context.Assistants.FirstOrDefaultAsync(a => a.UserId == userDto.Id);
                if (assistant != null)
                {
                    assistant.FirstName = userDto.FirstName;
                    assistant.LastName = userDto.LastName;
                    assistant.Address = userDto.Address;
                    assistant.TelNo = userDto.TelNo;
                }
            }
            else if (user.Role == UserRole.ÖğretimÜyesi)
            {
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userDto.Id);
                if (instructor != null)
                {
                    instructor.FirstName = userDto.FirstName;
                    instructor.LastName = userDto.LastName;
                    instructor.Address = userDto.Address;
                    instructor.TelNo = userDto.TelNo;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Kullanıcı başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

    }
}