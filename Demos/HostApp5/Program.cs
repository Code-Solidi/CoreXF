/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HostApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder().Build())
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"))//.AddConsole().AddDebug()
                        .AddFile(options =>
                        {
                            options.FileName = $"{nameof(HostApp)}-";   // log file prefix
                            options.FileSizeLimit = 1024 * 1024;                // IMB rolling limit
                            options.LogDirectory = "LogFiles";                  //
                            options.RetainedFileCountLimit = 10;                // keep up to 10 files
                        })
                        .AddFilter("File", LogLevel.Warning);
                })
                .UseStartup<Startup>();
        }
    }
}