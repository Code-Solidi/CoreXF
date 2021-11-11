﻿using CoreXF.Abstractions.Base;
using CoreXF.Framework.Settings;

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CoreXF.WebApiHost.Swagger
{
    public class SwaggerSelector
    {
        private static SwaggerSelector instance;
        private string extension;

        /// <summary>
        /// Real singleton!
        /// </summary>
        public static SwaggerSelector Service => SwaggerSelector.instance ?? (SwaggerSelector.instance = new SwaggerSelector());

        internal void SetExtension(string extension)
        {
            this.extension = extension;
        }

        public bool IncludeDocument(ApiDescription x)
        {
            var assembly = ((ControllerActionDescriptor)x.ActionDescriptor)?.ControllerTypeInfo.Assembly;
            var type = assembly?.GetTypes().SingleOrDefault(t => typeof(IExtension).IsAssignableFrom(t));
            var extension = type != null ? (IExtension)Activator.CreateInstance(type) : null;
            return this.extension != null ? this.extension == extension?.Name : true;
        }
    }
}