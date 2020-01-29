// Copyright (c) Code Solidi Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using CoreXF.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace CoreXF.Framework
{
    internal class ExtensionsRegistry : IExtensionsRegistry
    {
        private readonly List<IExtension> extensions = new List<IExtension>();
        private readonly ILogger logger;

        public IEnumerable<IExtension> Extensions => this.extensions;

        public ExtensionsRegistry(ILoggerFactory factory)
        {
            this.logger = factory.CreateLogger<ExtensionsRegistry>();
        }

        public T GetExtension<T>() where T : IExtension
        {
            var found = this.extensions.SingleOrDefault(x => x is T);
            return (T)found;
        }

        public void Register(IExtension extension)
        {
            var found = this.extensions.SingleOrDefault(x => x.Name == extension.Name);
            if (found != null)
            {
                this.logger.LogError($"Extension {extension.Name}, v{extension.Version} has already been registered.");
            }
            else
            {
                this.extensions.Add(extension);
            }
        }
    }
}