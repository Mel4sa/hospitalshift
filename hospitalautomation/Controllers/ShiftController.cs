using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hospitalautomation.Controllers
{
    [Route("[controller]")]
    public class ShiftController : Controller
    {
        private readonly ILogger<ShiftController> _logger;

        public ShiftController(ILogger<ShiftController> logger)
        {
            _logger = logger;
        }
[HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}