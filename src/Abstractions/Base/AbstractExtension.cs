/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CoreXF.Abstractions.Base
{
    /// <summary>A default implementation of <see cref="IExtension">IExtension</see>.</summary>
    public abstract class AbstractExtension : IExtension
    {
        private readonly IDictionary<string, object> properties;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractExtension"/> class.
        /// </summary>
        protected AbstractExtension()
        {
            this.properties = new Dictionary<string, object>();
        }

        /// <inheritdoc/>>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // do nothing, override to define behaviour
        }

        /// <inheritdoc/>>
        public virtual void Configure(Assembly assembly)
        {
            var titleAttribute = AbstractExtension.GetAttribute<AssemblyTitleAttribute>(assembly);
            this.Name ??= titleAttribute?.Title ?? string.Empty;

            var copyrightAttribute = AbstractExtension.GetAttribute<AssemblyCopyrightAttribute>(assembly);
            this.Copyright ??= copyrightAttribute?.Copyright ?? string.Empty;

            var descriptionAttribute = AbstractExtension.GetAttribute<AssemblyDescriptionAttribute>(assembly);
            this.Description ??= descriptionAttribute?.Description ?? string.Empty;

            var companyAttribute = AbstractExtension.GetAttribute<AssemblyCompanyAttribute>(assembly);
            this.Authors ??= companyAttribute?.Company ?? string.Empty;

            var versionAttribute = AbstractExtension.GetAttribute<AssemblyVersionAttribute>(assembly);
            this.Version ??= versionAttribute?.Version ?? string.Empty;

            this.Location ??= Path.GetDirectoryName(assembly.Location);
        }

        /// <summary>
        /// Gets the property named "name".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>A <typeparamref name="T"></typeparamref></returns>
        public T Get<T>(string name) => this.properties.ContainsKey(name) ? (T)this.properties[name] : default;

        /// <summary>
        /// Sets the property named "name".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void Set<T>(string name, T value) => this.properties[name] = value;

        /// <summary>
        /// Gets the attribute <typeparamref name="T"></typeparamref>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A <typeparamref name="T"></typeparamref></returns>
        private static T GetAttribute<T>(Assembly assembly) where T : Attribute => (T)assembly.GetCustomAttribute(typeof(T));
    }
}