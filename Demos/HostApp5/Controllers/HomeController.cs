/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using CoreXF.Abstractions.Registry;

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
            var extension = this.registry.GetExtension("Extension1");
            this.ViewBag.ExtName = extension?.Name ?? "Extension1 has not been registered";
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
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}