using Microsoft.AspNetCore.Mvc;

namespace Extension2.Controllers
{
    public class SkippedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}