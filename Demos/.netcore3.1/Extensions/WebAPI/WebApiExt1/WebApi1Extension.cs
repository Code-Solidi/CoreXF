﻿/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;

namespace WebApiExt1
{
    public class WebApi1Extension : ExtensionBase
    {
        public WebApi1Extension()
        {
            this.Name = nameof(WebApi1Extension).Replace("Extension", string.Empty);
        }
    }
}