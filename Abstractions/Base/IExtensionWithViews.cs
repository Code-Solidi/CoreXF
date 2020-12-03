/*
 * Copyright (c) 2016-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

namespace CoreXF.Abstractions.Base
{
    public interface IExtensionWithViews : IExtension
    {
        /// <summary>
        /// Views assembly name, usually assembly-name.Views.dll
        /// </summary>
        public string Views { get; }
    }
}