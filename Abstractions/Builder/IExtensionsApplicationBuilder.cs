/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;

namespace CoreXF.Abstractions.Builder
{
    public interface IExtensionsApplicationBuilder : IApplicationBuilder
    {
        IApplicationBuilder ExpansionPoint(string shimId);

        IEnumerable<string> GetShims();

        void Populate(IApplicationBuilder app);
    }
}