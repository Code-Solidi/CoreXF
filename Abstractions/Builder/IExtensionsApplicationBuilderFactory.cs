/*
 * Copyright (c) Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using Microsoft.AspNetCore.Builder;

namespace CoreXF.Abstractions.Builder
{
    public interface IExtensionsApplicationBuilderFactory
    {
        IExtensionsApplicationBuilder CreateBuilder(IApplicationBuilder builder);
    }
}