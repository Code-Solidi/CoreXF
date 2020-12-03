/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using Microsoft.AspNetCore.Builder;

namespace CoreXF.WebAPI.Abstractions.Builder
{
    public interface IExtensionsApplicationBuilderFactory
    {
        IExtensionsApplicationBuilder CreateBuilder(IApplicationBuilder builder);
    }
}