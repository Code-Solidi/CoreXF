using DateTimeService;

using DemoExtensionMvc.Models;

using Microsoft.AspNetCore.Mvc;

using System;

namespace DemoExtensionMvc.Controllers
{
    public class InjectedController : Controller
    {
        private readonly IDateTimeService dateTime;

        public InjectedController(IDateTimeService dateTime)
        {
            this.dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime), $"{nameof(dateTime)} is null.");
        }

        public IActionResult Index()
        {
            var model = new GreetingsModel
            {
                Greetings = "Hello from DemoExtensionMvc.",
                DateTime = this.dateTime.Get()
            };

            return View(model);
        }
    }
}