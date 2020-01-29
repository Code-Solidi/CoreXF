using CoreXF.Abstractions;

using Microsoft.AspNetCore.Mvc;

namespace Extension2.Controllers
{
    [Export]
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}