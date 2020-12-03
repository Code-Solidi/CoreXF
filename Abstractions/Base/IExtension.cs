/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.WebAPI.Abstractions.Builder;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreXF.WebAPI.Abstractions.Base
{
    /// <summary>
    /// Defines properties of an extensions. This interface must be implemented in order to define an extension.
    /// </summary>
    public interface IExtension
    {
        /// <summary>The name of the extension. As a convention use the name of the assembly.</summary>
        string Name { get; }

        /// <summary>The description of the extension.</summary>
        string Description { get; }

        /// <summary>The URL of the site related to the extension.</summary>
        string Url { get; }

        /// <summary>The extension's version.</summary>
        string Version { get; }

        /// <summary>The authors of the extension, comma separated.</summary>
        string Authors { get; }

        /// <summary>
        /// The deployment location of the extension, usually a dedicated folder in the "Extensions" folder in host app
        /// </summary>
        string Location { get; set; }

        void ConfigureServices(IServiceCollection services, IConfiguration configuration);

        void ConfigureMiddleware(IExtensionsApplicationBuilder app);
    }
}