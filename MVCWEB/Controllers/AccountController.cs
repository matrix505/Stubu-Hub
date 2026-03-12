using Microsoft.AspNetCore.Mvc;

namespace MVCWEB.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
