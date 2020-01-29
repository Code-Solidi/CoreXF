using Microsoft.AspNetCore.Mvc;

namespace Extension3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}