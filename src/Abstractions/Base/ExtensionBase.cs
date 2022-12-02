/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Builder;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.Generic;
using System.Reflection;

using static CoreXF.Abstractions.Base.IExtension;

namespace CoreXF.Abstractions.Base
{
    /// <summary>A default implementation of <see cref="IExtension">IExtension</see>.</summary>
    public class ExtensionBase : IBackingService
    {
        /// <summary>The name of the extension. As a convention use the name of the assembly.</summary>
        public string Name { get; set; } = nameof(ExtensionBase);

        /// <summary>The description of the extension.</summary>
        public string Description { get; set; } = "Base extension class, inherit to extend functionality.";

        /// <summary>The URL of the site related to the extension.</summary>
        public string Url { get; set; } = "www.codesolidi.com";

        /// <summary>The extension's version.</summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>The authors of the extension, comma separated.</summary>
        public string Authors { get; set; } = "Code Solidi Ltd.";

        /// <summary>
        /// The assembly location (folder) from which the extension is discovered and loaded
        /// </summary>
        public string Location { get; set; }

        public ExtensionStatus Status { get; private set; } = ExtensionStatus.Running;

        public IEnumerable<TypeInfo> Controllers { get; } = new List<TypeInfo>();

        public virtual void ConfigureMiddleware(IExtensionsApplicationBuilder app)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public void Start() => this.Status = ExtensionStatus.Running;

        public void Stop() => this.Status = ExtensionStatus.Stopped;
    }
}