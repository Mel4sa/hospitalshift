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

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Title, string Content, DateTime CreationDate, DateTime CreationTime)
        {
            int userId = 1; // Bu örnekte giriş yapmış kullanıcı yok, sabit verdik.

            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content)
                || CreationDate == default || CreationTime == default)
            {
                TempData["Error"] = "Lütfen tüm alanları doldurun.";
                return RedirectToAction("Index");
            }

            // Tarih ve saati birleştir
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
            await _context.SaveChangesAsync(); // Emergency kaydedildi ve ID elde ettik

            // Kullanıcıların emaillerini ve Id'lerini çekiyoruz
            var users = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.Email))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            // SMTP ayarları
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUser = "hbilalcinal@gmail.com";
            var smtpPass = "tqktaustbvneybed";

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                // Gönderilen mail içeriği
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

                        // Mail gönderim başarılı ise MailEmergency tablosuna kayıt ekleyelim
                        var mailEmergency = new MailEmergency
                        {
                            UserId = usr.Id,
                            EmergencyId = emergency.Id,
                            MailContent = mailBody
                        };

                        _context.MailEmergencies.Add(mailEmergency);
                        // Burada SaveChangesAsync'i mail gönderim döngüsünün sonunda 1 kere çağırabiliriz.
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Mail gönderim hatası");
                    }
                }

                // Tüm mailler gönderildikten sonra toplu olarak kaydedelim
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
