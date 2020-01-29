using CoreXF.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HostApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration/*, IHostingEnvironment env*/)
        {
            this.Configuration = configuration;
            //this.HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        //public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //var env = this.HostingEnvironment;

            //var extensionsPath = Path.Combine(env.ContentRootPath + this.Configuration["Extensions:Path"]);
            //services.AddExtCore(extensionsPath);
            //var factory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            //services.AddSingleton<IExtensionsManager>(ExtensionsManager.Discover(factory));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddCoreXF(services, this.Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseExtCore();

            app.UseStaticFiles();
            app.UseCookiePolicy();
            //app.UseCoreXF();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(name: "areas", template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}