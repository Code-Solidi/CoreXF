using CoreXF.WebAPI.Abstractions.Registry;
using CoreXF.WebApiHostApp.Models;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

namespace CoreXF.WebApiHostApp.Controllers
{
    public class MicroservicesController : Controller
    {
        private readonly IExtensionsRegistry extensionsRegistry;

        public MicroservicesController(IExtensionsRegistry extensionsRegistry)
        {
            this.extensionsRegistry = extensionsRegistry;
        }

        public IActionResult Index()
        {
            var model = new List<MicroserviceModel>();
            foreach (var extension in this.extensionsRegistry.Extensions)
            {
                var viewModel = new MicroserviceModel
                {
                    Name = extension.Name,
                    Description = extension.Description,
                    Status = MicroserviceModel.MsStatus.Running,
                    Version = extension.Version,
                    Url = extension.Url,
                    Authors = extension.Authors,
                    Location = extension.Location
                };

                model.Add(viewModel);
            };

            return View(model);
        }
    }
}