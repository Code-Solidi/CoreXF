﻿/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using System;

using DateTimeService;

namespace ServiceExporter
{
    internal class DateTimeServiceImpl : IDateTimeService
    {
        public DateTime Get() => DateTime.UtcNow;
    }
}