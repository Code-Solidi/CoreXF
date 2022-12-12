/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions;
using CoreXF.Abstractions.Registry;

using HostApp.WebApi.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Linq;

namespace HostApp.WebApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly IExtensionsRegistry extensionsRegistry;

        public HomeController(IExtensionsRegistry extensionsRegistry, ILogger<HomeController> logger)
        {
            this.extensionsRegistry = extensionsRegistry 
                ?? throw new ArgumentNullException(nameof(extensionsRegistry), $"{nameof(extensionsRegistry)} is null.");
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new ExtensionsModel
            {
                Extensions = this.extensionsRegistry.Extensions.OrderBy(x => x.Name).Cast<WebApiExtension>()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Start(string name)
        {
            var extension = this.extensionsRegistry.GetExtension(name) as WebApiExtension;
            extension?.Start();
            return this.RedirectToAction(nameof(Index));
        }

        public IActionResult Stop(string name)
        {
            var extension = this.extensionsRegistry.GetExtension(name) as WebApiExtension;
            extension?.Stop();
            return this.RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}