using CoreXF.Abstractions.Builder;
using CoreXF.Framework.Registry;
using CoreXF.WebApiHost;
using CoreXF.WebApiHost.Swagger;

using HostApp5.WebApi.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace HostApp5.WebApi
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
            services.AddControllersWithViews().AddCoreXF(services, this.Configuration);

            services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = "https://sumo-soft.com",    // hiro?
                        ValidIssuer = "https://www.sumo-soft.com",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7S79jvOkEdwoRqHx"))
                    };
                });

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
                    return selector.IncludeDocument(httpContext.HttpContext.User, x);
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
                        new string[] { }
                    }
                });
            });

            SeedDb.Initialize(services.BuildServiceProvider());
        }

        public void Configure(IApplicationBuilder original, IExtensionsApplicationBuilderFactory factory, IWebHostEnvironment env)
        {
            var app = factory.CreateBuilder(original);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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

            app.UseAuthorization();
            app.UseCookiePolicy(); //!!

            app.UseCoreXFHost(env);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Populate(original.UseCoreXF());
        }
    }
}