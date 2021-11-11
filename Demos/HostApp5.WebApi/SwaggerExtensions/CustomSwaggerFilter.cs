/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using CoreXF.Abstractions.Registry;

using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Linq;

namespace HostApp5.WebApi.SwaggerExtensions
{
    public class CustomSwaggerFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IExtensionsRegistry registry;

        public CustomSwaggerFilter(IHttpContextAccessor contextAccessor, IExtensionsRegistry registry)
        {
            this.contextAccessor = contextAccessor;
            this.registry = registry;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            //var extension = this.registry.Extensions.First();
            //var nonMobileRoutes = swaggerDoc.Paths
            //    .Where(x => !x.Key.ToLower().Contains("public"))
            //    .ToList();
         
            //nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
        }
    }
}