/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions;

using DateTimeService;

using Microsoft.Extensions.DependencyInjection;

using ServiceExporter;

namespace DemoExtensionMvc
{
    public class DemoExtension : MvcExtension
    {
        public DemoExtension()
        {
            this.Name = nameof(DemoExtension).Replace("Extension", string.Empty);
            this.Copyright = "© Code Solidi Ltd. 2016-2022";
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDateTimeService, DateTimeServiceImpl>();
        }
    }
}
