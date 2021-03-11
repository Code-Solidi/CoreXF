using Auditing.Domain;

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

using static Auditing.Domain.IAuditTrail;

namespace Auditing.Helpers
{
    /// <summary>
    /// The change auditor.
    /// </summary>
    public class ChangeAuditor : IChangeAuditor
    {
        private readonly ILogger<ChangeAuditor> logger;
        private readonly IAuditTrailsManager auditor;

        /// <summary>
        /// Gets the save changes handler.
        /// </summary>
        public EventHandler<SavingChangesEventArgs> SavingChanges { get; }

        /// <summary>
        /// Gets or sets the change tracker.
        /// </summary>
        public ChangeTracker ChangeTracker { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public static string Name => nameof(ChangeAuditor);

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public string User { set; private get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeAuditor"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="auditor">The auditor.</param>
        public ChangeAuditor(ILoggerFactory loggerFactory, IAuditTrailsManager auditor = default)
        {
            logger = loggerFactory?.CreateLogger<ChangeAuditor>();
            this.auditor = auditor;
            SavingChanges = SavingChangesHandler;
        }

        /// <summary>
        /// The save changes handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SavingChangesHandler(object sender, SavingChangesEventArgs e)
        {
            void AddTrails(IEnumerable<EntityEntry> entries, List<IAuditTrail> items, ActionKind actionKind)
            {
                string GetEntityIdentity(EntityEntry entityEntry)
                {
                    var result = default(string);
                    try
                    {
                        result = entityEntry.Property("Id")?.CurrentValue.ToString();   // access 'shadow' properties (of owned entities) as well
                    }
                    catch (RuntimeBinderException rbx)
                    {
                        result = rbx.InnerException?.Message ?? rbx.Message;
                        logger?.LogWarning(result);
                    }
                    catch (Exception x)
                    {
                        result = x.InnerException?.Message ?? x.Message;
                        logger?.LogWarning(result);
                    }

                    return result;
                }

                foreach (var entityEntry in entries)
                {
                    var auditTrail = auditor.CreateAuditTrail();
                    auditTrail.Who = User;
                    auditTrail.What = actionKind;
                    auditTrail.When = DateTime.UtcNow;
                    auditTrail.Type = entityEntry.Entity.GetType().FullName;
                    auditTrail.Identity = GetEntityIdentity(entityEntry);
                    items.Add(auditTrail);
                }
            }

            ChangeTracker.DetectChanges();

            if (auditor != default)
            {
                var added = ChangeTracker.Entries().Where(t => t.State == EntityState.Added).ToList();
                var modified = ChangeTracker.Entries().Where(t => t.State == EntityState.Modified).ToList();
                var deleted = ChangeTracker.Entries().Where(t => t.State == EntityState.Deleted).ToList();

                var auditTrails = new List<IAuditTrail>();
                AddTrails(added, auditTrails, ActionKind.Create);
                AddTrails(modified, auditTrails, ActionKind.Update);
                AddTrails(deleted, auditTrails, ActionKind.Delete);

                auditor.Save(auditTrails);
            }
        }
    }
}