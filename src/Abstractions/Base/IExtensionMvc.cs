﻿/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Abstractions.Base
{
    /// <summary>
    /// The extension with views.
    /// </summary>
    public interface IExtensionMvc : IExtension
    {
        /// <summary>
        /// Views assembly name, usually assembly-name.Views.dll
        /// </summary>
        public string Views { get; }
    }
}