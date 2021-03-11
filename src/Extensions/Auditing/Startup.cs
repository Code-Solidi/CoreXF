
using Auditing.Application.Commands;
using Auditing.Application.Queries;
using Auditing.Application.UseCases;
using Auditing.Domain;
using Auditing.Helpers;
using Auditing.Persistence;
using Auditing.Persistence.Handlers;

using CoreXF.Tools.CmdQry;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Collections.Generic;

namespace Auditing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAuditTrailsManager, AuditTrailsManager>();
            services.AddScoped<IChangeAuditor, ChangeAuditor>();

            services.AddScoped<AuditBoundedContext>();
            services.AddScoped<ICommandHandler<CreateAuditTrailCommand>>(services
                => new CreateAuditTrailHandler(services.GetRequiredService<AuditBoundedContext>()));
            services.AddScoped<ICommandHandler<AddAuditTrailsCommand>>(services
                => new AddAuditTrailsHandler(services.GetRequiredService<AuditBoundedContext>()));
            services.AddScoped<IQueryHandler<GetAuditTrailsQuery, IEnumerable<IAuditTrail>>>(services
                => new GetAuditTrailsHandler(services.GetRequiredService<AuditBoundedContext>()));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}