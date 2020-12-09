/*
 * Copyright (c) 2016-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.Abstractions.Registry;
using CoreXF.WebApiHost.Models;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;

namespace CoreXF.WebApiHost.Controllers
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
                    Status = extension.Status,
                    Version = extension.Version,
                    Url = extension.Url,
                    Authors = extension.Authors,
                    Location = extension.Location
                };

                model.Add(viewModel);
            };

            return View(model);
        }

        public IActionResult Pause(string item)
        {
            var extension = this.extensionsRegistry.Extensions.SingleOrDefault(x => x.Name.Equals(item, System.StringComparison.OrdinalIgnoreCase));
            var status = extension?.Status;
            extension?.Stop();
            //return Ok(status != extension?.Status);
            return Ok(new { result = status != extension?.Status, status = extension?.Status });
        }

        public IActionResult Play(string item)
        {
            var extension = this.extensionsRegistry.Extensions.SingleOrDefault(x => x.Name.Equals(item, System.StringComparison.OrdinalIgnoreCase));
            var status = extension?.Status;
            extension?.Start();
            return Ok(new { result = status != extension?.Status, status = extension?.Status });
        }
    }
}