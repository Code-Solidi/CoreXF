/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System;

namespace CoreXF.Abstractions.Attributes
{
    /// <summary>Decorates a controller as belonging to an extension.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ExportAttribute : Attribute
    {
    }
}