using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using hospitalautomation.Models;
using hospitalautomation.Models.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace hospitalautomation.Controllers
{
    [Route("[controller]")]
    public class DepartmanController : Controller
    {
        private readonly ILogger<DepartmanController> _logger;
        private readonly ApplicationDbContext _context;

        public DepartmanController(ILogger<DepartmanController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("")]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View("Index");
        }


        [HttpGet("DepartmantsInfo")]
        public IActionResult DepartmantsInfo()
        {
       
            var departmentNames = new List<string>
            {
                "Çocuk Acil",
                "Çocuk Yoğun Bakım",
                "Çocuk Hematolojisi ve Onkolojisi"
            };

      
            var departments = _context.Departments
                .Where(d => departmentNames.Contains(d.Name))
                .ToList();

            return View(departments); 
        }


        [HttpGet("get-department")]
        public IActionResult GetDepartment(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(new { });

            var dept = _context.Departments.FirstOrDefault(d => d.Name == name);
            if (dept == null)
                return Json(new { }); 

            return Json(dept); 
        }

        [HttpPost("create-or-update")]
        public async Task<IActionResult> CreateOrUpdate()
        {
            // Request body'yi oku
            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            var department = JsonConvert.DeserializeObject<Department>(body);

            if (department == null || string.IsNullOrWhiteSpace(department.Name))
            {
                return Json(new { error = "Departman bilgileri eksik." });
            }

            var validationErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(department.Name))
            {
                validationErrors.Add("Departman ismi eksik.");
            }
            if (department.PatientCount <= 0)
            {
                validationErrors.Add("Hasta sayısı sıfırdan büyük olmalıdır.");
            }
            if (department.EmptybedCount < 0)
            {
                validationErrors.Add("Boş yatak sayısı negatif olamaz.");
            }
            if (department.StartTime == TimeSpan.Zero)
            {
                validationErrors.Add("Başlangıç saati belirtilmelidir.");
            }
            if (department.EndTime == TimeSpan.Zero)
            {
                validationErrors.Add("Bitiş saati belirtilmelidir.");
            }

            if (validationErrors.Any())
            {
                return Json(new { error = string.Join(" ", validationErrors) });
            }

        
            var existingDepartment = _context.Departments.FirstOrDefault(d => d.Name == department.Name);

            if (existingDepartment != null)
            {
 
                existingDepartment.PatientCount = department.PatientCount;
                existingDepartment.EmptybedCount = department.EmptybedCount;
                existingDepartment.StartTime = department.StartTime;
                existingDepartment.EndTime = department.EndTime;
            }
            else
            {
               
                _context.Departments.Add(department);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = "Departman başarıyla kaydedildi!" });
        }

    }
}
