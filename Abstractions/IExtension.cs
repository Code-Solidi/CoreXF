// Copyright (c) Code Solidi Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreXF.Abstractions
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

        void ConfigureServices(IServiceCollection services, IConfiguration configuration);

        void ConfigureMiddleware(IExtensionsApplicationBuilder app);
    }
}