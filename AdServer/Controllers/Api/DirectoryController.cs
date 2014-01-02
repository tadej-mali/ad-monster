using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AdServer.App_Start;
using AdServer.Data;
using AdServer.Models;
using AdServer.Service;

namespace AdServer.Controllers.Api
{
    static class DtoMapper
    {
        public static DirectoryController.DirectoryDto ToDto(this Directory source)
        {
            if (source == null) { return null; }

            return new DirectoryController.DirectoryDto
            {
                id = source.Id,
                version = source.VersionStampAsLong(),
                title = source.Title,
                itemsCount = source.ItemsCount,
                description = source.Description
            };
        }

        public static Directory ToModel(this DirectoryController.DirectoryDto source, Directory target = null)
        {
            if (target == null) { target = new Directory(); }

            target.Title = source.title;
            target.Description = source.description;

            return target;
        }

        public static DirectoryController.AdvertisementDto ToDto(this Advertisement target)
        {
            if (target == null) { return null; }

            return new DirectoryController.AdvertisementDto
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

        public class DirectoryDto
        {
            public int id { get; set; }
            public long version { get; set; }
            public string title { get; set; }
            public int itemsCount { get; set; }
            public string description { get; set; }
            public AdvertisementDto[] advertisements { get; set; }

            public DirectoryDto Copy()
            {
                return new DirectoryDto
                {
                    id = this.id,
                    title = this.title,
                    itemsCount = this.itemsCount,
                    description = this.description
                };
            }
        }

        public class AdvertisementDto
        {
            public int parentId { get; set; }
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
        }


        // GET api/directory
        public HttpResponseMessage Get()
        {
            var dir = this.dirService.GetDirectoryList();
            return this.Request.CreateResponse(HttpStatusCode.OK, dir.Select(x => x.ToDto()).ToArray());
        }

        // GET api/directory/5
        public HttpResponseMessage Get(int id)
        {
            var theDirectory = this.dirService.GetDirectory(id);

            if (theDirectory == null) { return this.Request.CreateResponse(HttpStatusCode.NotFound); }

            var result = theDirectory.ToDto();
            result.advertisements = theDirectory.Advertisements.Select(x => x.ToDto()).ToArray();

            var response = this.Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        // POST api/directory
        public HttpResponseMessage Post([FromBody]DirectoryDto value)
        {
            var newDirectory = value.ToModel();
            this.dirService.SaveDirectory(newDirectory);

            var response = this.Request.CreateResponse(HttpStatusCode.Created, newDirectory.ToDto());
            return response;
        }

        private HttpResponseMessage TryExecute(Action toDo, Func<HttpResponseMessage> onSuccess)
        {
            try
            {
                toDo();
            }
            catch (DbUpdateConcurrencyException)
            {
                return this.Request.CreateResponse(HttpStatusCode.Conflict);
            }

            return onSuccess();
        }

        // PUT api/directory/5
        public HttpResponseMessage Put(int id, [FromBody]DirectoryDto value)
        {
            if (id != value.id) { return this.Request.CreateResponse(HttpStatusCode.BadRequest); }

            var toUpdate = this.dirService.GetDirectory(id);
            if (toUpdate == null) { return this.Request.CreateResponse(HttpStatusCode.NotFound); }
            if (toUpdate.IsModified(value.version)) { return this.Request.CreateResponse(HttpStatusCode.Conflict); }

            value.ToModel(toUpdate);

            return TryExecute(
                () => this.dirService.SaveDirectory(toUpdate),
                () => this.Request.CreateResponse(HttpStatusCode.OK, toUpdate.ToDto()));
        }

        // DELETE api/directory/5
        [HttpDelete]
        public HttpResponseMessage Delete(int id, long version)
        {
            var toDelete = this.dirService.GetDirectory(id);
            if (toDelete == null) { return this.Request.CreateResponse(HttpStatusCode.NotFound); }
            if (toDelete.IsModified(version)) { return this.Request.CreateResponse(HttpStatusCode.Conflict); }

            return TryExecute(
                () => this.dirService.DeleteDirectory(toDelete),
                () => this.Request.CreateResponse(HttpStatusCode.NoContent));
        }
    }
}
