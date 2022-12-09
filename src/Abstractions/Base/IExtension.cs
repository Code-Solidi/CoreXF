/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace CoreXF.Abstractions.Base
{
    /// <summary>
    /// Defines properties of an extensions. This interface must be implemented in order to define an extension.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// The name of the extension. As a convention use the name of the assembly.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the extension.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The URL of the site related to the extension.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// The extension's version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// The authors of the extension, comma separated.
        /// </summary>
        string Authors { get; }

        /// <summary>
        /// The copyright information about the extension.
        /// </summary>
        string Copyright { get; }

        /// <summary>
        /// The deployment location of the extension, usually a dedicated folder in the "Extensions" folder in host app
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Configures extension's DI.
        /// </summary>
        /// <param name="services"></param>
        void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Configures the extension from containing assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        void Configure(Assembly assembly);

        //void ConfigureMiddleware(IExtensionsApplicationBuilder app);
    }
}