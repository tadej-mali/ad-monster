using System.Data.Entity.Infrastructure;
using System.Linq;

namespace AdServer.Data
{
    public delegate IQueryable<T> QueryPredicate<T>(IQueryable<T> baseQuery);

    public interface IDbContext
    {
        IQueryable<T> GetQuery<T>() where T : class;

        IQueryable<T> GetQuery<T>(QueryPredicate<T> basePredicate) where T : class;

        void Remove<T>(T entity) where T : class;

        void Attach<T>(T entity) where T : class;

        void SaveChanges();

        DbEntityEntry<T> Entry<T>(T entity) where T : class;
    }
}
