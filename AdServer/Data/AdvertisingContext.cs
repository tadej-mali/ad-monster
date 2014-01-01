using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Transactions;
using AdServer.Models;

namespace AdServer.Data
{
    // TODO: http://msdn.microsoft.com/en-us/data/jj592676.aspx

    public class AdvertisingContext : DbContext
    {
        public DbSet<Directory> Directories { get; set; }

        public AdvertisingContext()
        {
            this.Configuration.LazyLoadingEnabled = true;
            Database.SetInitializer<AdvertisingContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Directory>().HasMany(x => x.Advertisements).WithRequired().HasForeignKey(x => x.DirectoryId).WillCascadeOnDelete();
        }
    }

    public static class EntityExtensions
    {
        public static EntityState GetSavingState(this IEntityWithId target)
        {
            return target.Id == 0 ? EntityState.Added : EntityState.Modified;
        }

        public static EntityState GetTrackingState(this object target, DbContext ctx)
        {
            return ctx.Entry(target).State;
        }

        public static void EnforceAttach<T>(this DbContext ctx, T target) where T : class
        {
            var state = target.GetTrackingState(ctx);
            if (state == EntityState.Detached)
            {
                ctx.Set<T>().Attach(target);
            }
        }

        public static int Fetch<T>(this ICollection<T> target)
        {
            if (target == null) { return 0; }

            return target.Count;
        }
    }

    public interface ITransactionScope : IDisposable
    {
        void Commit();
    }

    public class TransactionScopeProxy : ITransactionScope
    {
        private readonly TransactionScope innerScope;
        private readonly bool shallCommit;

        public TransactionScopeProxy(bool shallCommit = true) : this(new TransactionScope(), shallCommit)
        {}

        public TransactionScopeProxy(TransactionScope innerScope, bool shallCommit = true)
        {
            this.innerScope = innerScope;
            this.shallCommit = shallCommit;
        }

        public void Dispose()
        {
            this.innerScope.Dispose();
        }

        public void Commit()
        {
            if (this.shallCommit)
            {
                this.innerScope.Complete();
            }
        }
    }
}