using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
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

    public class DbContextProxy<TD> : IDbContext where TD : DbContext
    {
        private readonly TD context;

        public DbContextProxy(TD context)
        {
            this.context = context;
        }

        public IQueryable<T> GetQuery<T>() where T : class
        {
            return this.context.Set<T>();
        }

        public IQueryable<T> GetQuery<T>(QueryPredicate<T> basePredicate) where T : class
        {
            var baseQuery = this.GetQuery<T>();
            return basePredicate(baseQuery);
        }

        public void Remove<T>(T entity) where T : class
        {
            this.context.Set<T>().Remove(entity);
        }

        public void Attach<T>(T entity) where T : class
        {
            this.context.Set<T>().Attach(entity);
        }

        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        public DbEntityEntry<T> Entry<T>(T entity) where T : class
        {
            return this.context.Entry(entity);
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