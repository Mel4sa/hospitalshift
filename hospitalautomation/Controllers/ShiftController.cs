using System;
using System.Linq;
using System.Threading.Tasks;
using hospitalautomation.Models;
using hospitalautomation.Models.Context;
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

        [HttpGet("")]
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
