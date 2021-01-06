using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace EventBus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder().Build())
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"))//.AddConsole().AddDebug()
                        .AddFile(options =>
                        {
                            options.FileName = $"{nameof(EventBus)}-";  // log file prefix
                            options.FileSizeLimit = 1024 * 1024;        // IMB rolling limit
                            options.LogDirectory = "LogFiles";          // 
                            options.RetainedFileCountLimit = 10;        // keep up to 10 files
                        })
                        .AddFilter("File", LogLevel.Warning);
                })
                .UseKestrel((ctx, opt) =>
                {
                    var address = ctx.Configuration.GetValue<string>("HttpServer:address") ?? "127.0.0.1";
                    var ipAddress = IPAddress.Parse(address);
                    var port = ctx.Configuration.GetValue<int>("HttpServer:port");
                    opt.Listen(ipAddress, port == 0 ? 5000 : port);
                    var httpsPort = ctx.Configuration.GetValue<int>("HttpServer:httpsPort");
                    if (httpsPort != 0)
                    {
                        opt.Listen(ipAddress, httpsPort, listenOptions =>
                        {
                            listenOptions.UseHttps("certificate.pfx", "password");
                        });
                    }
                })
                .UseStartup<Startup>();
        }
    }
}