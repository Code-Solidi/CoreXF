using System;

using CoreXF.Abstractions;

using DateTimeService;

using Extension2.Models;

using Microsoft.AspNetCore.Mvc;

namespace Extension2.Controllers
{
    [Export]
    public class DefaultController : Controller
    {
        private readonly IDateTimeService dateTime;

        public DefaultController(IDateTimeService dateTime)
        {
            this.dateTime = dateTime;
        }

        public IActionResult Index()
        {
            var not = this.dateTime != null ? string.Empty : "not ";
            var model = new DefaultModel
            {
                DateTime = this.dateTime?.Get() ?? DateTime.Now,
                Greeting = $"DateTimeService is {not}running"
            };

            return this.View(model);
        }
    }
}