using CoreXF.Abstractions.Builder;
using CoreXF.Framework.Registry;
using CoreXF.WebApiHost;
using CoreXF.WebApiHost.Swagger;

using HostApp.WebApi.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace HostApp.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options
                => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews().AddCoreXF(services, this.Configuration);
            services.AddRazorPages();

            //services.AddIdentity<IdentityUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<SwaggerSelector>(_ => SwaggerSelector.Service);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(setup =>
            {
                setup.DocInclusionPredicate((name, x) =>
                {
                    var provider = services.BuildServiceProvider();
                    var selector = provider.GetRequiredService<SwaggerSelector>();
                    var httpContext = provider.GetService<IHttpContextAccessor>();
                    return selector.IncludeDocument(x);
                });

                setup.SwaggerDoc(name: "v3", new OpenApiInfo
                {
                    Title = "CoreXF APIs",
                    Version = "v3",
                });

                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        System.Array.Empty<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(/*IApplicationBuilder app, IWebHostEnvironment env*/IApplicationBuilder original, IExtensionsApplicationBuilderFactory factory, IWebHostEnvironment env)
        {
            var app = factory.CreateBuilder(original);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v3/swagger.json", "CoreXF APIs");
                options.InjectStylesheet("/swagger-ui/custom.css"); // NB: not available from a nuget package, make it work!
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCoreXFHost(env);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.Populate(original.UseCoreXF());
        }
    }
}
