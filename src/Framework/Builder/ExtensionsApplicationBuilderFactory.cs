/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Builder;

using Microsoft.AspNetCore.Builder;

using System.Diagnostics.CodeAnalysis;

namespace CoreXF.Framework.Builder
{
    public class ExtensionsApplicationBuilderFactory : IExtensionsApplicationBuilderFactory
    {
        private static IExtensionsApplicationBuilder builder;   // NB: consider Lazy<T>

        [SuppressMessage("Critical Code Smell", "S2696:Instance members should not write to \"static\" fields", Justification = "<Pending>")]
        public IExtensionsApplicationBuilder CreateBuilder(IApplicationBuilder builder)
        {
            if (ExtensionsApplicationBuilderFactory.builder == null)
            {
                ExtensionsApplicationBuilderFactory.builder = new ExtensionsApplicationBuilder(builder);
            }

            return ExtensionsApplicationBuilderFactory.builder;
        }
    }
}