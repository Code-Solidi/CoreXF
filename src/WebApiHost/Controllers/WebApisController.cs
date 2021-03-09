/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.Abstractions.Registry;
using CoreXF.WebApiHost.Models;
using CoreXF.WebApiHost.Swagger;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;

namespace CoreXF.WebApiHost.Controllers
{
    //[Authorize]
    public class WebApisController : Controller
    {
        private readonly IExtensionsRegistry extensionsRegistry;
        private readonly SwaggerSelector selectorService;

        public WebApisController(IExtensionsRegistry extensionsRegistry, SwaggerSelector selectorService)
        {
            this.extensionsRegistry = extensionsRegistry;
            this.selectorService = selectorService;
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

        public IActionResult Swagger(string extension)
        {
            this.selectorService.SetExtension(extension);
            return this.Accepted();
        }
    }
}