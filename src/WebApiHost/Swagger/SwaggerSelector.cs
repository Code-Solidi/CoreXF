/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using Microsoft.AspNetCore.Mvc.ApiExplorer;

using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CoreXF.WebApiHost.Swagger
{
    /// <summary>
    /// NB: Subject to change - dictionary should be locked and unlocked on every op!!
    /// </summary>
    public class SwaggerSelector
    {
        private static SwaggerSelector instance;
        private Dictionary<string, string> extensions;
        private string extension;

        private SwaggerSelector()
        {
            this.extensions = new Dictionary<string, string>();
        }

        public static SwaggerSelector Service => SwaggerSelector.instance ?? (SwaggerSelector.instance = new SwaggerSelector());

        internal void SetExtension(ClaimsPrincipal user, string extension)
        {
            // swagger rewrites everything in the request, even the logged in user!!
            //var username = user?.Identity?.Name ?? "Anonymous";
            //this.extensions[user?.Identity?.Name] = extension;

            this.extension = extension;
        }

        public bool IncludeDocument(ClaimsPrincipal user, ApiDescription x)
        {
            // swagger rewrites everything in the request, even the logged in user!!
            //var username = user?.Identity?.Name ?? "Anonymous";
            //return this.extensions.ContainsKey(username) ? x.RelativePath == this.extensions[username] : true;

            return this.extension != null ? x.RelativePath == this.extension : true; 
        }
    }
}