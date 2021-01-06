using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Miscelaneous
{
    /// <summary>
    /// Ideas from https://www.meziantou.net/2017/08/14/entity-framework-core-history-audit-table.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class AuditingDbContext : DbContext
    {
        public AuditingDbContext()
        {
        }

        public AuditingDbContext(DbContextOptions options) : base(options)
        {
        }

        public IQueryable<AuditTransaction> AuditTransactions
            => this.Set<AuditTransaction>().Include(x => x.Audits).OrderBy(x => x.DateTime);

        public override int SaveChanges(bool acceptAllChangesOnSuccess = true)
        {
            var auditEntries = this.BeforeSaveChanges();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            this.AfterSaveChanges(auditEntries);
            return result;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.SaveChangesAsync(true, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var auditEntries = this.BeforeSaveChanges();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            this.AfterSaveChanges(auditEntries);
            return result;
        }

        public void Undo()
        {
            var transactionSet = this.Set<AuditTransaction>();
            var tx = transactionSet.Include(x => x.Audits).LastOrDefault();
            if (tx != null)
            {
                foreach (var audit in tx.Audits)
                {
                    //var id = AuditingDbContext.GetEntityId(audit);
                    var entity = this.GetEntity(audit);
                    switch (audit.State)
                    {
                        case EntityState.Added:
                            this.Remove(audit, entity);
                            break;

                        case EntityState.Deleted:
                            this.Restore(audit, entity);
                            break;

                        case EntityState.Modified:
                            this.Update(audit, entity);
                            break;
                    }
                }

                transactionSet.Remove(tx);
                base.SaveChanges(true);
            }
        }

        public object FindIgnoreQueryFilters(Type entityType, object id)
        {
            var set = this.GetSet(entityType);
            var result = set.SingleOrDefault(x => this.HasKey(x, (Guid)id));
            return result;
        }

        public object FindIgnoreQueryFilters(Type entityType, string property, object value)
        {
            var set = this.GetSet(entityType);
            var result = set.SingleOrDefault(x => this.HasPropertyValue(x, property, value));
            return result;
        }

        public AuditEntry GetAuditEntry(Guid audittedEntityId, EntityState state = EntityState.Added)
        {
            var result = this.Set<AuditEntry>().SingleOrDefault(x => x.EntityId == audittedEntityId && x.State == state);
            return result;
        }

        public AuditTransaction GetAuditTransaction(Guid audittedEntityId, EntityState state = EntityState.Added)
        {
            var entry = this.Set<AuditEntry>().Include(x => x.Transaction).LastOrDefault(x => x.EntityId == audittedEntityId && x.State == state);
            var result = entry.Transaction;
            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuditTransaction>().ToTable("Transactions", "Audits");
            modelBuilder.Entity<AuditTransaction>().HasKey(e => e.Id);
            modelBuilder.Entity<AuditTransaction>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<AuditTransaction>().Property(e => e.DateTime).IsRequired();
            modelBuilder.Entity<AuditTransaction>().HasMany(x => x.Audits).WithOne(x => x.Transaction).IsRequired();

            modelBuilder.Entity<AuditEntry>().ToTable("Entries", "Audits");
            modelBuilder.Entity<AuditEntry>().HasKey(e => e.Id);
            modelBuilder.Entity<AuditEntry>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<AuditEntry>().Ignore(x => x.HasTemporaryProperties);
            modelBuilder.Entity<AuditEntry>().Ignore(x => x.TemporaryProperties);

            modelBuilder.Entity<AuditEntry>().Property(e => e.Type).HasMaxLength(1024).IsRequired();
            modelBuilder.Entity<AuditEntry>().Property(e => e.PrimaryKeys).HasMaxLength(1024);
        }

        private class IdentifiedObject
        {
            public Guid Id { get; set; }
        }

        private object GetEntity(AuditEntry audit)
        {
            var type = Type.GetType(audit.Type) ?? Assembly.GetAssembly(this.GetType()).GetType(audit.Type);
            var set = this.GetSet(type);
            var result = set.SingleOrDefault(x => this.HasKey(x, audit.EntityId));
            return result;
        }

        private bool HasKey(object x, Guid entityId)
        {
            var e = (dynamic)x;
            return e.Id == entityId;
        }

        private bool HasPropertyValue(object x, string property, object value)
        {
            var pi = x.GetType().GetProperty(property);
            var result = pi.GetValue(x).Equals(value);
            return result;
        }

        private IQueryable<object> GetSet(Type type)
        {
            var method = this.GetType().GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);
            method = method.MakeGenericMethod(type);

            var set = method.Invoke(this, null) as IQueryable<object>;

            method = typeof(EntityFrameworkQueryableExtensions).GetMethod(nameof(EntityFrameworkQueryableExtensions.IgnoreQueryFilters)
                , BindingFlags.Public | BindingFlags.Static);
            method = method.MakeGenericMethod(type);

            var result = method.Invoke(null, new[] { set });
            return result as IQueryable<object>;
        }

        private IQueryable<T> GetSet<T>()
        {
            var type = typeof(T);
            var method = this.GetType().GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);
            method = method.MakeGenericMethod(type);

            var set = method.Invoke(this, null) as IQueryable<object>;

            method = typeof(EntityFrameworkQueryableExtensions).GetMethod(nameof(EntityFrameworkQueryableExtensions.IgnoreQueryFilters), BindingFlags.Public | BindingFlags.Static);
            method = method.MakeGenericMethod(type);

            var result = method.Invoke(null, new[] { set });
            return result as IQueryable<T>;
        }

        private static AuditEntry ExamineProps(EntityEntry entry)
        {
            var auditEntry = new AuditEntry
            {
                Type = entry.Entity.GetType().FullName,
                State = entry.State
            };

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.AddTemporaryProperty(property);
                }
                else if (property.Metadata.IsPrimaryKey())
                {
                    if (property.Metadata.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        auditEntry.EntityId = (Guid)(property.CurrentValue.GetType() != typeof(Guid)
                            ? new GuidConverter().ConvertFrom(property.CurrentValue)
                            : property.CurrentValue);
                    }

                    auditEntry.AddPrimaryKey(property.Metadata.Name, property.CurrentValue);
                }
                else if (entry.State == EntityState.Modified && property.IsModified && property.CurrentValue != property.OriginalValue)
                {
                    auditEntry.AddPropertyValue(property.Metadata.Name, property.OriginalValue);
                }
            }

            return auditEntry;
        }

        //private static Guid GetEntityId(AuditEntry audit)
        //{
        //    var anon = new { Id = Guid.Empty };
        //    var instance = (dynamic)JsonConvert.DeserializeObject(audit.PrimaryKeys, anon.GetType());
        //    var id = instance.Id;
        //    return id;
        //}

        private static void SoftDelete(IEnumerable<EntityEntry> entries)
        {
            // execute the foreach as a result from a "soft delete" option - enabled/disabled
            foreach (var entry in entries.Where(e => e.State == EntityState.Deleted))
            {
                try
                {
                    var isDeleted = entry.Property("IsDeleted").CurrentValue;
                    if ((bool)isDeleted == false)
                    {
                        entry.CurrentValues["IsDeleted"] = true;
                        entry.State = EntityState.Modified;
                    }

                    // pass thru: else "real" delete requested (perhaps by Admin?)
                }
                catch (InvalidOperationException)
                {
                    // do nothing, no IsDeleted property
                }
            }
        }

        private void AfterSaveChanges(AuditTransaction transaction)
        {
            if (transaction.Audits.Any())
            {
                foreach (var auditEntry in transaction.Audits)
                {
                    // get the final value of the temporary properties
                    foreach (var prop in auditEntry.TemporaryProperties)
                    {
                        if (prop.Metadata.IsPrimaryKey())
                        {
                            auditEntry.AddPrimaryKey(prop.Metadata.Name, prop.CurrentValue);
                        }
                        else
                        {
                            auditEntry.AddPropertyValue(prop.Metadata.Name, prop.CurrentValue);
                        }
                    }
                }

                this.Set<AuditTransaction>().Add(transaction);
                var result = base.SaveChanges(true);
            }
        }

        private AuditTransaction BeforeSaveChanges()
        {
            this.ChangeTracker.DetectChanges();
            var entries = this.ChangeTracker.Entries();

            AuditingDbContext.SoftDelete(entries);

            var transaction = new AuditTransaction
            {
                DateTime = DateTime.UtcNow,
                Audits = new List<AuditEntry>()
            };

            var skipStates = new EntityState[] { EntityState.Detached, EntityState.Unchanged };
            foreach (var entry in entries)
            {
                if (entry.Entity is AuditEntry == false && skipStates.All(x => x != entry.State))
                {
                    var auditEntry = AuditingDbContext.ExamineProps(entry);
                    transaction.Audits.Add(auditEntry);
                }
            }

            return transaction;
        }

        private void Remove(AuditEntry audit, object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.Remove(entity);
        }

        private void Restore(AuditEntry audit, object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.SoftUndelete(entity);
            this.Add(entity);
        }

        private void Update(AuditEntry audit, object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(audit.Values);
            foreach (var value in values)
            {
                var propType = this.Entry(entity).Property(value.Key).CurrentValue.GetType();

                // todo: redesign the code below. doesn't look good!
                try
                {
                    if (propType.IsEnum)
                    {
                        this.Entry(entity).Property(value.Key).CurrentValue = Enum.Parse(propType, Enum.GetName(propType, value.Value));
                    }
                    else
                    {
                        this.Entry(entity).Property(value.Key).CurrentValue = value.Value;
                    }
                }
                catch
                {
                    try
                    {
                        this.Entry(entity).Property(value.Key).CurrentValue = Convert.ChangeType(value.Value, propType);
                    }
                    catch
                    {
                        this.Entry(entity).Property(value.Key).CurrentValue = TypeDescriptor.GetConverter(propType).ConvertFrom(value.Value);
                    }
                }
            }

            this.Update(entity);
        }

        private void SoftUndelete(object entity)
        {
            var pi = entity.GetType().GetProperty("IsDeleted");
            if (pi != null)
            {
                pi.SetValue(entity, false);
            }
        }

        public class AuditEntry
        {
            private Dictionary<string, object> primaryKeyValues;

            private Dictionary<string, object> propertyValues;

            public AuditEntry()
            {
                this.primaryKeyValues = new Dictionary<string, object>();
                this.propertyValues = new Dictionary<string, object>();
                this.TemporaryProperties = new List<PropertyEntry>();
            }

            public enum OperationKind { Created, Modified, Deleted }

            public static AuditEntry Shim => new AuditEntry() { State = EntityState.Added, Type = "Shim" };

            public bool HasTemporaryProperties => this.TemporaryProperties.Any();

            public Guid Id { get; set; }

            public Guid EntityId { get; set; }

            public string PrimaryKeys
            {
                get => JsonConvert.SerializeObject(this.primaryKeyValues);
                set => this.primaryKeyValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
            }

            public EntityState State { get; set; }

            public List<PropertyEntry> TemporaryProperties { get; }

            public AuditTransaction Transaction { get; internal set; }

            public string Type { get; set; }

            public string Values
            {
                get => JsonConvert.SerializeObject(this.propertyValues);
                set => this.propertyValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
            }

            internal void AddPrimaryKey(string name, object value)
            {
                this.primaryKeyValues[name] = value;
            }

            internal void AddPropertyValue(string name, object value)
            {
                this.propertyValues[name] = value;
            }

            internal void AddTemporaryProperty(PropertyEntry property)
            {
                // value will be generated by the database, get the value after saving
                this.TemporaryProperties.Add(property);
            }
        }

        public class AuditTransaction
        {
            public AuditTransaction()
            {
                this.DateTime = DateTime.UtcNow;
            }

            public ICollection<AuditEntry> Audits { get; set; }

            public DateTime DateTime { get; set; }

            public Guid Id { get; set; }
        }
    }
}