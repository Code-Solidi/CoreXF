﻿/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Framework.Builder
{
    public partial class ExtensionsApplicationBuilder
    {
        public interface IIdentityGenerator
        {
            string GetId(string current = null);
        }
    }
}