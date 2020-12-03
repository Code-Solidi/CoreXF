/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.WebAPI.Abstractions.Builder;

using Microsoft.AspNetCore.Builder;

namespace CoreXF.Framework.Builder
{
    public class ExtensionsApplicationBuilderFactory : IExtensionsApplicationBuilderFactory
    {
        private static IExtensionsApplicationBuilder builder;   // NB: consider Lazy<T>

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