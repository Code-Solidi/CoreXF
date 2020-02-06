/*
 * Copyright (c) Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
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