using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using hospitalautomation.Models;
using hospitalautomation.Models.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hospitalautomation.Controllers
{
    [Route("[controller]")]
    public class ShiftController : Controller
    {
        private readonly ILogger<ShiftController> _logger;
        private readonly ApplicationDbContext _context;

        public ShiftController(ILogger<ShiftController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        [HttpGet("ShiftInfo")]
        public async Task<IActionResult> ShiftInfo()
        {
            // Giriş yapan kullanıcının ID'sini alıyoruz
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // UserId'ye göre Assistant'ı buluyoruz
            var assistant = await _context.Assistants.FirstOrDefaultAsync(a => a.UserId == int.Parse(userId));
            if (assistant == null)
            {
                TempData["Error"] = "Nöbet bilgileri bulunamadı.";
                return RedirectToAction("Index");
            }

            // Assistant'a ait nöbetleri çekiyoruz
            var shifts = await _context.Shifts
                .Where(s => s.AssistantId == assistant.Id)
                .Include(s => s.Department) // Department bilgilerini de dahil ediyoruz
                .ToListAsync();

            return View("ShiftInfo", shifts); // ShiftInfo view'ine gönderiyoruz
        }

        [HttpGet("")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var shifts = await _context.Shifts
                .Include(s => s.Assistant)
                .Include(s => s.Department)
                .ToListAsync();

            var assistants = await _context.Assistants.ToListAsync();
            var departments = await _context.Departments.ToListAsync();

            ViewBag.Assistants = assistants;
            ViewBag.Departments = departments;

            return View("Index", shifts);
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int assistantId, int departmentId, DateTime shiftDate, DateTime startTime, DateTime endTime)
        {
            if (assistantId <= 0 || departmentId <= 0 || shiftDate == default || startTime == default || endTime == default)
            {
                TempData["Error"] = "Lütfen tüm alanları eksiksiz doldurun.";
                return RedirectToAction("Index");
            }

            // Aynı asistana, aynı gün nöbet var mı?
            bool alreadyAssigned = await _context.Shifts.AnyAsync(s =>
                s.AssistantId == assistantId &&
                s.ShiftDate.Date == shiftDate.Date
            );

            if (alreadyAssigned)
            {
                TempData["Error"] = "Aynı asistan aynı gün birden fazla nöbete yazılamaz.";
                return RedirectToAction("Index");
            }

            var shift = new Shift
            {
                AssistantId = assistantId,
                DepartmentId = departmentId,
                ShiftDate = shiftDate,
                StartTime = startTime,
                EndTime = endTime
            };

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Nöbet başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
            {
                TempData["Error"] = "Silinecek nöbet bulunamadı.";
                return RedirectToAction("Index");
            }

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Nöbet başarıyla silindi!";
            return RedirectToAction("Index");
        }

        [HttpPost("update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int assistantId, int departmentId, DateTime shiftDate, DateTime startTime, DateTime endTime)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
            {
                TempData["Error"] = "Güncellenecek nöbet bulunamadı.";
                return RedirectToAction("Index");
            }

            bool alreadyAssigned = await _context.Shifts.AnyAsync(s =>
                s.Id != id &&
                s.AssistantId == assistantId &&
                s.ShiftDate.Date == shiftDate.Date
            );

            if (alreadyAssigned)
            {
                TempData["Error"] = "Aynı asistan aynı gün birden fazla nöbete yazılamaz.";
                return RedirectToAction("Index");
            }

            shift.AssistantId = assistantId;
            shift.DepartmentId = departmentId;
            shift.ShiftDate = shiftDate;
            shift.StartTime = startTime;
            shift.EndTime = endTime;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Nöbet başarıyla güncellendi!";
            return RedirectToAction("Index");
        }
    }
}
