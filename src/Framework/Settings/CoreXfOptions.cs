/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Framework.Settings
{
    public class CoreXfOptions
    {
        public bool UseViewComponents { get; set; } = true;

        public bool UseTagHelpers { get; set; } = true;

        public bool UseFileProviders { get; set; } = true;

        public string Location { get; set; } = "/Extensions";
    }
}