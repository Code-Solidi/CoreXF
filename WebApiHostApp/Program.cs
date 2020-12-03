using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreXF.WebApiHostApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                //.UseConfiguration(new ConfigurationBuilder().Build())
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"))//.AddConsole().AddDebug()
                        .AddFile(options =>
                        {
                            options.FileName = $"{nameof(CoreXF.WebApiHostApp)}-";  // log file prefix
                            options.FileSizeLimit = 1024 * 1024;                    // IMB rolling limit
                            options.LogDirectory = "LogFiles";                      //
                            options.RetainedFileCountLimit = 10;                    // keep up to 10 files
                        })
                        .AddFilter("File", LogLevel.Warning);
                });

        //.UseStartup<Startup>();
    }
}