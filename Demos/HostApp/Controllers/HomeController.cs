using CoreXF.Abstractions.Registry;
using Extension1;
using HostApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HostApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IExtensionsRegistry registry;

        public HomeController(IExtensionsRegistry registry)
        {
            this.registry = registry;
        }

        public IActionResult Index()
        {
            var extension = this.registry.GetExtension<TheExtension>();
            this.ViewBag.ExtName = extension?.Name;
            return this.View();
        }

        public IActionResult About()
        {
            this.ViewData["Message"] = "Your application description page.";
            return this.View();
        }

        public IActionResult Contact()
        {
            this.ViewData["Message"] = "Your contact page.";
            return this.View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}