using System;

namespace Helpers
{
    public static class EnvironmentHelper
    {
        public static string GetEnvironment()
        {
#if DEBUG
            // nb: no other way, alas!
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
#endif
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process)?.ToLower();
            if (string.IsNullOrEmpty(environmentName))
            {
                return "Production";
            }

            return environmentName;
        }
    }
}