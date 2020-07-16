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