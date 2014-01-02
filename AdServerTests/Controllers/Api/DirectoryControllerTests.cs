using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AdServer.Controllers.Api;
using AdServer.Data;
using AdServer.Models;
using AdServer.Service;
using Moq;
using NUnit.Framework;

namespace AdServerTests.Controllers.Api
{
    [TestFixture]
    public class DirectoryControllerTests
    {
        [Test]
        public void GetDirectory_Existent_HttpOk()
        {
            const int dirId = 42;

            var dirMock = new Mock<IDirectoryService>();
            dirMock.Setup(d => d.GetDirectory(dirId)).Returns(new Directory {Id = dirId, VersionStamp = new byte[8], Advertisements = new Advertisement[0]});

            var controller = SetupController(new DirectoryController(dirMock.Object));

            var response = controller.Get(dirId);
            var result = ContentValueAs<DirectoryController.DirectoryDto>(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.id, dirId);
        }

        private static T ContentValueAs<T>(HttpResponseMessage response)
        {
            T result;
            response.TryGetContentValue(out result);
            return result;
        }

        [Test]
        public void GetDirectory_NonExistent_HttpNotFound()
        {
            const int dirId = 42;

            var dirMock = new Mock<IDirectoryService>();
            dirMock.Setup(d => d.GetDirectory(dirId)).Returns((Directory)null);

            var controller = SetupController(new DirectoryController(dirMock.Object));

            var response = controller.Get(dirId);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void CreateDirectory_New_HttpCreated()
        {
            var dirMock = new Mock<IDirectoryService>();
            dirMock.Setup(d => d.SaveDirectory(It.IsAny<Directory>())).Callback<Directory>(x => x.VersionStamp = new byte[8]);

            var controller = SetupController(new DirectoryController(dirMock.Object));

            const string testTitle = "TestTitle";
            var response = controller.Post(new DirectoryController.DirectoryDto {title = testTitle});
            var result = ContentValueAs<DirectoryController.DirectoryDto>(response);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.title, testTitle);
        }


        [Test]
        public void UpdateDirectory_MismatchedId_HttpBadRequest()
        {
            const int dirId = 42;

            var dirMock = new Mock<IDirectoryService>();
            var controller = SetupController(new DirectoryController(dirMock.Object));

            var response = controller.Put(dirId, new DirectoryController.DirectoryDto { id = dirId + 1 });

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void UpdateDirectory_NonExistent_HttpNotFound()
        {
            const int dirId = 42;

            var dirMock = new Mock<IDirectoryService>();
            var controller = SetupController(new DirectoryController(dirMock.Object));

            var response = controller.Put(dirId, new DirectoryController.DirectoryDto { id = dirId });

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void UpdateDirectory_DirectoryHasChanged_HttpConflict()
        {
            const int dirId = 42;
            const long oldVersion = 24;

            var dirMock = new Mock<IDirectoryService>();
            dirMock.Setup(d => d.GetDirectory(dirId)).Returns(new Directory { Id = dirId, VersionStamp = new byte[8] });

            var controller = SetupController(new DirectoryController(dirMock.Object));

            var response = controller.Put(dirId, new DirectoryController.DirectoryDto { id = dirId, version = oldVersion });

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Test]
        public void UpdateDirectory_ConcurrencyException_HttpConflict()
        {
            const int dirId = 42;
            const long oldVersion = 24;

            var dirMock = new Mock<IDirectoryService>();
            dirMock.Setup(d => d.GetDirectory(dirId)).Returns(new Directory { Id = dirId, VersionStamp = oldVersion.ToByte() });
            dirMock.Setup(d => d.SaveDirectory(It.IsAny<Directory>())).Throws<DbUpdateConcurrencyException>();

            var controller = SetupController(new DirectoryController(dirMock.Object));

            var response = controller.Put(dirId, new DirectoryController.DirectoryDto { id = dirId, version = oldVersion });

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        public void UpdateDirectory_Success_HttpOk()
        {
            const int dirId = 42;
            const long oldVersion = 24;

            var dirMock = new Mock<IDirectoryService>();
            dirMock.Setup(d => d.GetDirectory(dirId)).Returns(new Directory { Id = dirId, VersionStamp = oldVersion.ToByte() });
            dirMock.Setup(d => d.SaveDirectory(It.IsAny<Directory>())).Throws<DbUpdateConcurrencyException>();

            var controller = SetupController(new DirectoryController(dirMock.Object));

            var response = controller.Put(dirId, new DirectoryController.DirectoryDto { id = dirId, version = oldVersion });
            var result = ContentValueAs<DirectoryController.DirectoryDto>(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.id, dirId);
        }

        private static T SetupController<T>(T controller) where T : ApiController
        {
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            return controller;
        }
    }
}
