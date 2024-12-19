using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using hospitalautomation.Models;
using hospitalautomation.Models.Context;
using hospitalautomation.Models.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace hospitalautomation.Controllers
{
    [Route("[controller]")]
    public class InterviewController : Controller
    {
        private readonly ILogger<InterviewController> _logger;
        private readonly ApplicationDbContext _context;
        public InterviewController(ILogger<InterviewController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("Interviewtable")]
        [Authorize(Roles = "Admin")]
        public IActionResult Interviewtable()
        {
            var interviews = _context.Interviews
                .Where(i => !i.IsDeleted)
                .OrderBy(i => i.ShiftDate)
                .Select(i => new InterviewDto
                {
                    AssistantName = _context.Assistants
                        .Where(a => a.Id == i.AssistantId)
                        .Select(a => $"{a.FirstName} {a.LastName}")
                        .FirstOrDefault(),
                    InstructorName = _context.Instructors
                        .Where(inst => inst.Id == i.InstructorId)
                        .Select(inst => $"{inst.FirstName} {inst.LastName}")
                        .FirstOrDefault(),
                    ShiftDate = i.ShiftDate,
                    Status = i.ShiftDate >= DateTime.Today ? "Aktif" : "Geçmiş"
                })
                .ToList();

            return View(interviews);
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı giriş yapmamış.");
            }

            bool isInstructor = User.IsInRole("ÖğretimÜyesi");
            ViewBag.IsInstructor = isInstructor;

            int numericUserId;
            if (!int.TryParse(userId, out numericUserId))
            {
                _logger.LogError("User ID parsing failed.");
                return BadRequest("Geçersiz kullanıcı kimliği.");
            }

            List<Interview> interviews;

           if (isInstructor)
            {
                // InstructorId'yi Instructor tablosundan UserId ile bulma
                var instructor = _context.Instructors.FirstOrDefault(i => i.UserId == numericUserId);
                // InstructorId ile Interviews filtreleme
                 interviews = _context.Interviews
                        .Where(i => i.InstructorId == instructor.Id && !i.IsDeleted)
                        .OrderBy(i => i.ShiftDate)
                        .ToList();
            }
            else
            {
                var assistant = _context.Assistants.FirstOrDefault(i => i.UserId == numericUserId);

                interviews = _context.Interviews
                .Where(i => i.AssistantId == assistant.Id && !i.IsDeleted)
                .OrderBy(i => i.ShiftDate)
                .ToList();
            }

            ViewBag.Instructors = _context.Instructors.ToList();
            ViewBag.Assistants = _context.Assistants.ToList();
            ViewBag.FutureInterview = interviews
                .Where(i => i.ShiftDate >= DateTime.Today)
                .OrderBy(i => i.ShiftDate)
                .ThenBy(i => i.StartTime)
                .FirstOrDefault();

            return View(interviews);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        // Randevu Alma İşlemi
       [HttpPost("CreateInterview")]
public IActionResult CreateInterview(int instructorId, DateTime shiftDate, DateTime startTime, DateTime endTime)
{
    // Giriş yapan kullanıcının UserId'sini al
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
        return RedirectToAction("Index", "Login");

    // UserId ile Assistant tablosundan eşleşen kaydı al
    var assistant = _context.Assistants.FirstOrDefault(a => a.UserId == int.Parse(userId));
    if (assistant == null)
    {
        TempData["ErrorMessage"] = "Bu kullanıcıya bağlı bir asistan bulunamadı.";
        return RedirectToAction("Index");
    }

    // Randevu çakışma kontrolü
    var existingInterview = _context.Interviews
        .FirstOrDefault(i => i.InstructorId == instructorId
                             && i.ShiftDate == shiftDate
                             && i.StartTime <= endTime
                             && i.EndTime >= startTime);

    if (existingInterview != null)
    {
        TempData["ErrorMessage"] = "Bu öğretim üyesi için belirtilen tarihte başka bir randevu var.";
        return RedirectToAction("Index");
    }

    // Yeni randevu oluşturma
    var newInterview = new Interview
    {
        AssistantId = assistant.Id, // Asistan Id'sini buraya yaz
        InstructorId = instructorId,
        ShiftDate = shiftDate,
        StartTime = startTime,
        EndTime = endTime,
    };

    _context.Interviews.Add(newInterview);
    _context.SaveChanges();

    TempData["SuccessMessage"] = "Randevu başarıyla oluşturuldu.";
    return RedirectToAction("Index");
}

        // Randevu İptali
        [HttpPost("CancelInterview")]
        public IActionResult CancelInterview(int interviewId)
        {
            var interview = _context.Interviews.Find(interviewId);
            if (interview == null)
                return NotFound("Randevu bulunamadı.");

               interview.IsDeleted = true;
                _context.Interviews.Update(interview);
                _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
