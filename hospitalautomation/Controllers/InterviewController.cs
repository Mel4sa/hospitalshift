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
using Microsoft.EntityFrameworkCore;

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
                    Status = i.IsDeleted
        ? "İptal"
        : (i.ShiftDate >= DateTime.Today ? "Aktif" : "Geçmiş")
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

            bool isAsistan = User.IsInRole("Asistan");
            ViewBag.IsAsistan = isAsistan;

            if (!int.TryParse(userId, out int numericUserId))
            {
                _logger.LogError("User ID parsing failed.");
                return BadRequest("Geçersiz kullanıcı kimliği.");
            }

            List<Interview> interviews = new List<Interview>();
            DateTime today = DateTime.Today;

            if (isInstructor)
            {
                var instructor = _context.Instructors.FirstOrDefault(i => i.UserId == numericUserId);
                if (instructor == null)
                {
                    _logger.LogError("Instructor bulunamadı.");
                    return BadRequest("Instructor bulunamadı.");
                }

                interviews = _context.Interviews
                    .Where(i => i.InstructorId == instructor.Id && !i.IsDeleted && i.ShiftDate >= today) 
                    .OrderBy(i => i.ShiftDate)
                    .ToList();
            }
            else if (isAsistan)
            {
                var assistant = _context.Assistants.FirstOrDefault(i => i.UserId == numericUserId);
                if (assistant == null)
                {
                    _logger.LogError("Assistant bulunamadı.");
                    return BadRequest("Assistant bulunamadı.");
                }

                interviews = _context.Interviews
                    .Where(i => i.AssistantId == assistant.Id && !i.IsDeleted && i.ShiftDate >= today)
                    .OrderBy(i => i.ShiftDate)
                    .ToList();
            }
            else
            {
                _logger.LogError("User role is neither Instructor nor Asistan.");
                return BadRequest("Geçersiz kullanıcı rolü.");
            }

            // Kullanıcıya ait zamanı geçmemiş randevuyu getir
            var futureInterview = interviews.FirstOrDefault();
            ViewBag.FutureInterview = futureInterview;

            var instructors = _context.Instructors
                .Include(i => i.Department)
                .Where(i => !i.IsDeleted)
                .ToList();

            var assistants = _context.Assistants
                .Where(a => !a.IsDeleted)
                .ToList();

            var viewModel = new InterViewViewModel
            {
                Instructors = instructors,
                Assistants = assistants,
                Interviews = interviews
            };

            return View(viewModel);
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");

            // Assistant doğrulama
            var assistant = _context.Assistants.FirstOrDefault(a => a.UserId == int.Parse(userId));
            if (assistant == null)
            {
                TempData["ErrorMessage"] = "Bu kullanıcıya bağlı bir asistan bulunamadı.";
                return RedirectToAction("Index");
            }

            // Instructor doğrulama
            var instructor = _context.Instructors.FirstOrDefault(i => i.Id == instructorId && !i.IsDeleted);
            if (instructor == null)
            {
                TempData["ErrorMessage"] = "Seçilen öğretim üyesi bulunamadı.";
                return RedirectToAction("Index");
            }

            // Mevcut randevuyu bul
            var existingInterview = _context.Interviews
                .FirstOrDefault(i => i.AssistantId == assistant.Id && i.ShiftDate >= DateTime.Today);

            if (existingInterview != null)
            {
                // Mevcut randevuyu güncelle
                existingInterview.InstructorId = instructorId;
                existingInterview.ShiftDate = shiftDate;
                existingInterview.StartTime = startTime;
                existingInterview.EndTime = endTime;

                _context.Interviews.Update(existingInterview);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Randevu başarıyla güncellendi.";
            }
            else
            {
                // Yeni randevu oluştur
                var newInterview = new Interview
                {
                    AssistantId = assistant.Id,
                    InstructorId = instructorId,
                    ShiftDate = shiftDate,
                    StartTime = startTime,
                    EndTime = endTime,
                };

                _context.Interviews.Add(newInterview);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Randevu başarıyla oluşturuldu.";
            }

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
public class InterViewViewModel
{
    public List<Instructor> Instructors { get; set; }
    public List<Assistant> Assistants { get; set; }
    public List<Interview> Interviews { get; set; }
}