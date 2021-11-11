/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Collections.Generic;

namespace HostApp5.WebApi.SwaggerExtensions
{
    public class AddCommonParameterOperationFilter : IOperationFilter
    {
        /// <summary>Adds parameter(s) to the specified operation (API call).</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            //var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

            //if (descriptor != null && !descriptor.ControllerName.StartsWith("Weather"))
            //{
            //    operation.Parameters.Add(new OpenApiParameter()
            //    {
            //        Name = "extension",
            //        In = ParameterLocation.Query,
            //        Description = "The extension to show",
            //        Required = true
            //    });

            //    operation.Parameters.Add(new OpenApiParameter()
            //    {
            //        Name = "nonce",
            //        In = ParameterLocation.Query,
            //        Description = "The random value",
            //        Required = true
            //    });

            //    operation.Parameters.Add(new OpenApiParameter()
            //    {
            //        Name = "sign",
            //        In = ParameterLocation.Query,
            //        Description = "The signature",
            //        Required = true
            //    });
            //}
        }
    }
}