/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;

using System.Collections.Generic;

namespace CoreXF.Abstractions.Registry
{
    public interface IExtensionsRegistry
    {
        IEnumerable<IExtension> Extensions { get; }

        T GetExtension<T>() where T : IExtension;

        IExtension GetExtension(string name);
    }
}