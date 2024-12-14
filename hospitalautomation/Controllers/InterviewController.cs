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
    public class InterviewController : Controller
    {
        private readonly ILogger<InterviewController> _logger;

        public InterviewController(ILogger<InterviewController> logger)
        {
            _logger = logger;
        }
[HttpGet("")]
        public IActionResult Intervıewtable()
        {
            return View("Intervıewtable");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}