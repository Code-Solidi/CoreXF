﻿/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using DateTimeService;

using System;

namespace ServiceExporter
{
    public class DateTimeServiceImpl : IDateTimeService
    {
        public DateTime Get() => DateTime.UtcNow;
    }
}