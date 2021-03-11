using Microsoft.Extensions.Configuration;

using System;

namespace Helpers
{
    public static class ConfigurationHelper
    {
        public static string GetConnectionString(string name)
        {
            var environment = EnvironmentHelper.GetEnvironment();
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment}.json", false, true)
                .Build();

            var connectionString = config.GetConnectionString(name);
            return connectionString;
        }
    }
}