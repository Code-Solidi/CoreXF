/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppExtension
{
    internal class Extension : ExtensionBase
    {
        public override string Name
        {
            get
            {
                var services = new ServiceCollection();
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                /*
                 * Here you can go through package's source code
                 */
                base.ConfigureServices(services, configuration);
                return "Whatever";

            }
        }
    }
}