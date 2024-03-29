﻿/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;

namespace CoreXF.Abstractions
{
    /// <summary>A default implementation of <see cref="IMvcExtension">IMvcExtension</see>.</summary>
    public class MvcExtension : AbstractExtension, IMvcExtension
    {
        /// <inheritdoc/>
        public string Views { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcExtension"/> class.
        /// </summary>
        protected MvcExtension()
        {
#if !NET6_0
            this.Views = $"{this.GetType().Assembly.GetName().Name}.Views.dll";
#endif
        }
    }
}