using Microsoft.AspNetCore.Mvc;

namespace DemoExtensionMvc.Controllers
{
    public class ScopedCssController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
