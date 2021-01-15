/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

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