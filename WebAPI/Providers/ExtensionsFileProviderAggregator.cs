/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System.Collections.Generic;

using CoreXF.Abstractions.Base;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace CoreXF.Framework.Providers
{
    internal class ExtensionsFileProviderAggregator
    {
        private readonly ILoggerFactory loggerFactory;

        public CompositeFileProvider Composite { get; }

        public ExtensionsFileProviderAggregator(ILoggerFactory loggerFactory, IEnumerable<IExtension> extensions, IFileProvider provdier)
        {
            this.loggerFactory = loggerFactory;

            var extensionProviders = new List<IFileProvider>();
            foreach (var extension in extensions)
            {
                extensionProviders.Add(new ExtensionsFileProvider(loggerFactory, extension));
            }

            //extensionProviders.Reverse();

            this.Composite = new CompositeFileProvider(new List<IFileProvider>(extensionProviders)
            {
                provdier
            });
        }

        //public IFileProvider AddProvider(ExtensionsFileProvider fileProvider)
        //{
        //    var providers = new List<IFileProvider>(this.Composite.FileProviders);
        //    var extensionsProviders = providers.Where(x => x is ExtensionsFileProvider).Cast<ExtensionsFileProvider>();
        //    var found = extensionsProviders.SingleOrDefault(x => x.Extension.Equals(fileProvider.Extension));
        //    if (found == default(ExtensionsFileProvider))
        //    {
        //        providers.Add(fileProvider);
        //        this.Composite = new CompositeFileProvider(providers);
        //    }

        //    return this.Composite;
        //}

        //public IFileProvider RemoveProvider(string pluginId)
        //{
        //    var providers = new List<IFileProvider>(this.Composite.FileProviders);
        //    var extensionProviders = providers.Where(x => x is ExtensionsFileProvider).Cast<ExtensionsFileProvider>();
        //    var found = extensionProviders.SingleOrDefault(x => x.Extension.Equals(pluginId));
        //    if (found != default(ExtensionsFileProvider))
        //    {
        //        providers.Remove(found);
        //        this.Composite = new CompositeFileProvider(providers);
        //    }

        //    return found;
        //}
    }
}