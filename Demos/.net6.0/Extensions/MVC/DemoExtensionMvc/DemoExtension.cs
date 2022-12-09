using CoreXF.Abstractions;

using DateTimeService;

using ServiceExporter;

namespace DemoExtensionMvc
{
    public class DemoExtension : MvcExtension
    {
        public DemoExtension()
        {
            this.Name = nameof(DemoExtension).Replace("Extension", string.Empty);
            this.Copyright = "© Code Solidi Ltd. 2019-2022";
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDateTimeService, DateTimeServiceImpl>();
        }
    }
}
