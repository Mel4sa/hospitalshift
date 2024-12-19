using System;
using System.Linq;
using System.Threading.Tasks;
using hospitalautomation.Models;
using hospitalautomation.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace hospitalautomation.Controllers
{
    [Route("[controller]")]
    public class EmergencyController : Controller
    {
        private readonly ILogger<EmergencyController> _logger;
        private readonly ApplicationDbContext _context;

        public EmergencyController(ILogger<EmergencyController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        [HttpGet("EmergencyInfo")]
        public async Task<IActionResult> EmergencyInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Kullanıcı bilgisi bulunamadı.";
                return RedirectToAction("Index");
            }

            int currentUserId = int.Parse(userId);

            var emergencies = await _context.MailEmergencies
                .Where(me => me.UserId == currentUserId) 
                .Include(me => me.Emergency) 
                .OrderByDescending(me => me.Emergency.CreatedAt) 
                .Select(me => new
                {
                    SenderName = _context.Users.Where(u => u.Id == me.Emergency.UserId).Select(u => u.Email).FirstOrDefault(),
                    me.Emergency.Title,
                    me.Emergency.Content,
                    CreatedDate = me.Emergency.CreatedAt.ToString("dd MMMM yyyy"),
                    CreatedTime = me.Emergency.CreatedAt.ToString("HH:mm")
                })
                .ToListAsync();

            return View("EmergencyInfo", emergencies);
        }

        [HttpGet("")]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Title, string Content, DateTime CreationDate, DateTime CreationTime)
        {
            int userId = 1; 

            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content)
                || CreationDate == default || CreationTime == default)
            {
                TempData["Error"] = "Lütfen tüm alanları doldurun.";
                return RedirectToAction("Index");
            }

           
            DateTime createdAt = new DateTime(CreationDate.Year, CreationDate.Month, CreationDate.Day,
                                              CreationTime.Hour, CreationTime.Minute, CreationTime.Second);

            var emergency = new Emergency
            {
                UserId = userId,
                Title = Title,
                Content = Content,
                CreatedAt = createdAt
            };

            _context.Emergencies.Add(emergency);
            await _context.SaveChangesAsync(); 

            
            var users = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.Email))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();


            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUser = "melisaxsimsek@gmail.com";
            var smtpPass = "levqhxgwtjcwramo";

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                var mailBody = Content + Environment.NewLine +
                               "Oluşturulma Tarihi: " + createdAt.ToString("yyyy-MM-dd HH:mm");
                foreach (var usr in users)
                {
                    var mail = new MailMessage
                    {
                        From = new MailAddress(smtpUser, "Hastane Otomasyon"),
                        Subject = "Acil Durum: " + emergency.Title,
                        Body = mailBody
                    };
                    mail.To.Add(usr.Email);
                    try
                    {
                        await client.SendMailAsync(mail);
                        var mailEmergency = new MailEmergency
                        {
                            UserId = usr.Id,
                            EmergencyId = emergency.Id,
                            MailContent = mailBody
                        };
                        _context.MailEmergencies.Add(mailEmergency);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Mail gönderim hatası");
                    }
                }
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "Acil durum kaydedildi, kullanıcılara mail gönderildi ve MailEmergency tablosuna kayıtlar eklendi.";
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
