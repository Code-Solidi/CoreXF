/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.Abstractions.Builder;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace CoreXF.Abstractions.Base
{
    /// <summary>A default implementation of <see cref="IExtensionWithViews">IExtensionWithViews</see>.</summary>
    public class ExtensionBaseWithViews : ExtensionBase, IExtensionWithViews
    {
        /// <summary>
        /// The name of the compiled views assembly
        /// </summary>
        public virtual string Views { get; }
    }
}