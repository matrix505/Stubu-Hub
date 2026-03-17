using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCWEB.DAL.Abstract;
using MVCWEB.Enums;
using MVCWEB.ViewModel;

namespace MVCWEB.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProjectRepository _projectRepository;
 

        public HomeController(
            ILogger<HomeController> logger,
            IProjectRepository projectRepository
            )
        {
            _logger = logger;
            _projectRepository = projectRepository;
         
        }
       
        public IActionResult Index()
        {
            if(User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index","Dashboard");
            }
            return View();
        }
        public IActionResult Browse()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

