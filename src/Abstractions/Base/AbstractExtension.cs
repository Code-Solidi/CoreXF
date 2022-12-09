/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Reflection;

namespace CoreXF.Abstractions.Base
{
    /// <summary>A default implementation of <see cref="IExtension">IExtension</see>.</summary>
    public class AbstractExtension : IExtension
    {
        /// <inheritdoc/>>
        public string Name { get; protected set; }

        /// <inheritdoc/>>
        public string Description { get; protected set; } = "Base extension class, inherit to extend functionality.";

        /// <inheritdoc/>>
        public string Url { get; protected set; } = "www.codesolidi.com";

        /// <inheritdoc/>>
        public string Version { get; protected set; } = "1.0.0";

        /// <inheritdoc/>>
        public string Authors { get; protected set; } = "Code Solidi Ltd.";

        /// <inheritdoc/>>
        public string Location { get; protected set; }

        /// <inheritdoc/>>
        public string Copyright { get; protected set; }

        /// <inheritdoc/>>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // do nothing, override to define behaviour
        }

        /// <inheritdoc/>>
        public virtual void Configure(Assembly assembly)
        {
            var title = this.Get<AssemblyTitleAttribute>(assembly);
            this.Name = title?.Title ?? string.Empty;

            var copyright = this.Get<AssemblyCopyrightAttribute>(assembly);
            this.Copyright = copyright?.Copyright ?? string.Empty;

            var description = this.Get<AssemblyDescriptionAttribute>(assembly);
            this.Description = description?.Description ?? string.Empty;

            var company = this.Get<AssemblyCompanyAttribute>(assembly);
            this.Authors = company?.Company ?? string.Empty;

            var version = this.Get<AssemblyVersionAttribute>(assembly);
            this.Version = version?.Version ?? string.Empty;

            this.Location = Path.GetDirectoryName(assembly.Location);
        }

        private T Get<T>(Assembly assembly) where T : Attribute => (T)assembly.GetCustomAttribute(typeof(T));
    }
}