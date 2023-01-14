/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoreXF.Framework.Registry
{
    /// <summary>
    /// A base class intended for overriding in extensions. Used as a starting point for middleware exposed by the extension. 
    /// (NB: not used for now, see the readme.md in Builder folder.) 
    /// </summary>
    public class ExtensionsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;
        private readonly IApplicationLifetime applicationLifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionsMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="applicationLifetime">The application lifetime.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public ExtensionsMiddleware(RequestDelegate next, IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.applicationLifetime = applicationLifetime;
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        /// <summary>
        /// Invokes the <see cref="Task"/>.
        /// </summary>
        /// <param name="httpContext">The http context.</param>
        /// <returns>A Task.</returns>
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