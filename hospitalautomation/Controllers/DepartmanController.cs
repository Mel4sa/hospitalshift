using System.Text.Json.Serialization;
using hospitalautomation.Models;
using hospitalautomation.Models.Context;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet("get-department")]
        public IActionResult GetDepartment(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(new {});

            var dept = _context.Departments.FirstOrDefault(d => d.Name == name);
            if (dept == null)
                return Json(new {}); // Boş obje dönüyoruz, front-end bunu kontrol eder

            return Json(dept); // Departman bilgilerini JSON olarak dönüyoruz
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

            var department = JsonConverter.DeserializeObject<Department>(body);

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

            // Aynı isme sahip departman var mı diye kontrol et
            var existingDepartment = _context.Departments.FirstOrDefault(d => d.Name == department.Name);

            if (existingDepartment != null)
            {
                // Güncelleme
                existingDepartment.PatientCount = department.PatientCount;
                existingDepartment.EmptybedCount = department.EmptybedCount;
                existingDepartment.StartTime = department.StartTime;
                existingDepartment.EndTime = department.EndTime;
            }
            else
            {
                // Yeni oluşturma
                _context.Departments.Add(department);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = "Departman başarıyla kaydedildi!" });
        }
    }
}
