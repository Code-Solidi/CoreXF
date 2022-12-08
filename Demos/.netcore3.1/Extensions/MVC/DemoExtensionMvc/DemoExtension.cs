using CoreXF.Abstractions.Base;

using DateTimeService;

using Microsoft.Extensions.DependencyInjection;

using ServiceExporter;

namespace DemoExtensionMvc
{
    public class DemoExtension : ExtensionBaseMvc
    {
        public DemoExtension()
        {
            this.Name = nameof(DemoExtension).Replace("Extension", string.Empty);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDateTimeService, DateTimeServiceImpl>();
        }
    }
}
