using System.Collections.Generic;
using System.Linq;
using AdServer.Data;
using AdServer.Models;

namespace AdServer.Service
{
    public class DirectoryQuery
    {
        public DirectoryQuery(int id)
        {
            this.Id = id;
        }

        public int Id { get; private set; }
        public bool WithCount { get; set; }
    }

    public interface IDirectoryService
    {
        void SaveDirectory(Directory dir);
        Directory GetDirectory(int id);
        Directory GetDirectory(DirectoryQuery q);
        void DeleteDirectory(Directory dir);
        IList<Directory> GetDirectoryList();
    }

    public class DirectoryService : IDirectoryService
    {
        private IDbContext DbContext { get; set; }

        public DirectoryService(IDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public void SaveDirectory(Directory dir)
        {
            this.DbContext.Entry(dir).State = dir.GetSavingState();
            this.DbContext.SaveChanges();
        }

        public Directory GetDirectory(int id)
        {
            return this.GetDirectory(new DirectoryQuery(id));
        }

        public Directory GetDirectory(DirectoryQuery q)
        {
            if (!q.WithCount) { return this.DbContext.GetQuery<Directory>().SingleOrDefault(x => x.Id == q.Id); }

            var fromDb = this.DbContext.GetQuery<Directory>()
                .Where(x => x.Id == q.Id)
                .Select(x => new { Dir = x, Count = x.Advertisements.Count() })
                .SingleOrDefault();
            if (fromDb == null) { return null; }

            fromDb.Dir.ItemsCount = fromDb.Count;
            return fromDb.Dir;
        }

        public void DeleteDirectory(Directory dir)
        {
            this.DbContext.EnforceAttach(dir);
            dir.Advertisements.Fetch();

            this.DbContext.Remove(dir);
            this.DbContext.SaveChanges();
        }

        public IList<Directory> GetDirectoryList()
        {
            var fromDb = this.DbContext.GetQuery<Directory>().Select(x => new { Dir = x, Count = x.Advertisements.Count() }).ToList();

            return fromDb.Select(x =>
            {
                x.Dir.ItemsCount = x.Count;
                return x.Dir;
            }).ToList();
        }
    }
}
