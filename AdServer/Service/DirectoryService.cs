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

    public class DirectoryService
    {
        private AdvertisingContext dbContext;

        public DirectoryService(AdvertisingContext dbContext = null)
        {
            this.dbContext = dbContext;
        }

        private AdvertisingContext DbContext
        {
            get { return this.dbContext ?? (this.dbContext = new AdvertisingContext()); }
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
            if (!q.WithCount) { return this.DbContext.Set<Directory>().Find(q.Id); }

            var fromDb = this.DbContext.Set<Directory>()
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

            this.DbContext.Directories.Remove(dir);
            this.DbContext.SaveChanges();
        }

        public IList<Directory> GetDirectoryList()
        {
            var fromDb = this.DbContext.Set<Directory>().Select(x => new {Dir = x, Count = x.Advertisements.Count()}).ToList();

            return fromDb.Select(x =>
            {
                x.Dir.ItemsCount = x.Count;
                return x.Dir;
            }).ToList();
        }
    }
}
