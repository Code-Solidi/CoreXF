using Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Diagnostics.CodeAnalysis;

namespace Auditing.Persistence
{
    public class AuditBoundedContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

        [SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant", Justification = "<Pending>")]
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured == false)
            {
                //System.Diagnostics.Debugger.Launch();
                var connectionString = ConfigurationHelper.GetConnectionString("AuditConnection");
                optionsBuilder.UseSqlServer(connectionString);

                var factory = new LoggerFactory();
                factory.AddFile("db-log.log");
                optionsBuilder.UseLoggerFactory(factory);
            }
        }
    }
}