/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.WebAPI.Abstractions.Builder;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace CoreXF.WebAPI.Abstractions.Base
{
    /// <summary>A default implementation of <see cref="IExtension">IExtension</see>.</summary>
    public class ExtensionBase : IExtension, IBackingService
    {
        /// <summary>The name of the extension. As a convention use the name of the assembly.</summary>
        public virtual string Name => nameof(ExtensionBase);

        /// <summary>The description of the extension.</summary>
        public virtual string Description => "Base extension class, inherit to extend functionality.";

        /// <summary>The URL of the site related to the extension.</summary>
        public virtual string Url => "www.codesolidi.com";

        /// <summary>The extension's version.</summary>
        public virtual string Version => "1.0.0";

        /// <summary>The authors of the extension, comma separated.</summary>
        public virtual string Authors => "Code Solidi Ltd.";

        public string Location { get; set; }

        public virtual void ConfigureMiddleware(IExtensionsApplicationBuilder app)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var assembly = this.GetType().Assembly;
            var startUpName = $"{assembly.GetName().Name}.Startup";
            var startup = assembly.GetType(startUpName);
            if (startup != null)
            {
                dynamic instance = Activator.CreateInstance(startup, configuration);
                instance.ConfigureServices(services);
            }
        }
    }
}