using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCWEB.ViewModel;
using System.Runtime.CompilerServices;

namespace MVCWEB.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            ILogger<AuthController> logger)
        {
            _logger = logger;
        }
        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel rvm)
        {
            _logger.LogInformation(rvm.LastName);
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
