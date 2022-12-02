/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using Microsoft.AspNetCore.Builder;

using System.Collections.Generic;

namespace CoreXF.Abstractions.Builder
{
    /// <summary>
    /// The extensions application builder.
    /// </summary>
    public interface IExtensionsApplicationBuilder : IApplicationBuilder
    {
        IApplicationBuilder ExpansionPoint(string shimId);

        IEnumerable<string> GetShims();

        void Populate(IApplicationBuilder app);
    }
}