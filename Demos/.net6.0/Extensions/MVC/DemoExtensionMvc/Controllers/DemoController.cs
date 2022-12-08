using Microsoft.AspNetCore.Mvc;

namespace DemoExtensionMvc.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
