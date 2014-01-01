using System.Linq;
using System.Web.Http;
using AdServer.App_Start;
using AdServer.Models;
using AdServer.Service;

namespace AdServer.Controllers.Api
{
    static class ModelExtensions
    {
        public static DirectoryController.DirectoryDescription ToDescription(this Directory target)
        {
            if (target == null) { return null; }

            return new DirectoryController.DirectoryDescription
            {
                id = target.Id,
                title = target.Title,
                itemsCount = target.ItemsCount,
                description = target.Description
            };
        }

        public static DirectoryController.AdvertisementDescription ToDescription(this Advertisement target)
        {
            if (target == null) { return null; }

            return new DirectoryController.AdvertisementDescription
            {
                id = target.Id,
                title = target.Title,
                description = target.Description,
                url = target.Url,
                parentId = target.DirectoryId
            };
        }
    }

    public class DirectoryController : ApiController
    {
        private readonly IDirectoryService dirService;

        public DirectoryController(IDirectoryService dirService)
        {
            this.dirService = dirService;
        }

        public class DirectoryDescription
        {
            public int id { get; set; }
            public string title { get; set; }
            public int itemsCount { get; set; }
            public string description { get; set; }
            public AdvertisementDescription[] advertisements { get; set; }

            public DirectoryDescription Copy()
            {
                return new DirectoryDescription
                {
                    id = this.id,
                    title = this.title,
                    itemsCount = this.itemsCount,
                    description = this.description
                };
            }
        }

        public class AdvertisementDescription
        {
            public int parentId { get; set; }
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
        }


        // GET api/directory
        public object Get()
        {
            var dir = this.dirService.GetDirectoryList();
            return dir.Select(x => x.ToDescription()).ToArray();
        }

        // GET api/directory/5
        public object Get(int id)
        {
            var theDirectory = this.dirService.GetDirectory(id);

            if (theDirectory == null) { return null; }

            var result = theDirectory.ToDescription();
            result.advertisements = theDirectory.Advertisements.Select(x => x.ToDescription()).ToArray();
            return result;

        }

        // POST api/directory
        public void Post([FromBody]string value)
        {
        }

        // PUT api/directory/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/directory/5
        public void Delete(int id)
        {
        }
    }
}
