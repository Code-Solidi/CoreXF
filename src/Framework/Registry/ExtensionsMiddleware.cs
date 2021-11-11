/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoreXF.Framework.Registry
{
    public class ExtensionsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;
        private readonly IApplicationLifetime applicationLifetime;

        public ExtensionsMiddleware(RequestDelegate next, IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.applicationLifetime = applicationLifetime;
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //var requestUrl = httpContext.Request.Host.ToString() + httpContext.Request.Path;
            //this.logger.LogInformation($"REQUEST: {requestUrl}");

            //var parts = httpContext.Request.Path.HasValue
            //    ? httpContext.Request.Path.Value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
            //    : new string[0];

            await this.next.Invoke(httpContext);
        }
    }
}