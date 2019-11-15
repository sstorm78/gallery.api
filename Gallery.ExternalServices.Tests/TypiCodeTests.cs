using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Gallery.ExternalServices.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Album = Gallery.ExternalServices.Typicode.Models.Album;
using Photo = Gallery.ExternalServices.Typicode.Models.Photo;

namespace Gallery.ExternalServices.Tests
{
    [TestClass]
    public class TypiCodeTests
    {
        [TestMethod]
        public async Task ShouldReturnOneAlbumForGivenUserid()
        {
            using (var server = new StubHttpServer())
            {
                var result = new List<Album>
                             {
                                 new Album
                                 {
                                     Id = 1,
                                     Title = "test"
                                 }
                             };

                server.SetupRoute("/albums?userid=1")
                    .Get()
                    .ReturnsStatusCode(HttpStatusCode.OK)
                    .WithJsonContent(result);

                var config = new TestConfig
                             {
                                 ExternalServicesTypiCodeUrl = server.Url
                             };

                var sut = new Typicode.TypiCodeClient(config);

                var response = await sut.GetAlbumsByUserId(1);

                response.Should().NotBeNullOrEmpty();
                response.First().Title.Should().Be("test");
;            }

        }

        [TestMethod]
        public async Task ShouldReturnOnePhotoForGivenUserid()
        {
            using (var server = new StubHttpServer())
            {
                var result = new List<Photo>
                             {
                                 new Photo
                                 {
                                     Id = 1,
                                     Title = "test"
                                 }
                             };

                server.SetupRoute("/photos?userid=1")
                    .Get()
                    .ReturnsStatusCode(HttpStatusCode.OK)
                    .WithJsonContent(result);

                var config = new TestConfig
                             {
                                 ExternalServicesTypiCodeUrl = server.Url
                             };

                var sut = new Typicode.TypiCodeClient(config);

                var response = await sut.GetPhotosByUserId(1);

                response.Should().NotBeNullOrEmpty();
                response.First().Title.Should().Be("test");;
            }

        }

        [TestMethod]
        public async Task ShouldThrowExternalServiceHttpExceptionWhenIncorrectURL()
        {
            using (var server = new StubHttpServer())
            {
                //Arrange
                server.SetupRoute("/xxx")
                    .Get()
                    .ReturnsStatusCode(HttpStatusCode.NotFound)
                    .WithNoContent();

                var config = new TestConfig
                             {
                                 ExternalServicesTypiCodeUrl = server.Url
                             };

                var sut = new Typicode.TypiCodeClient(config);

                await Assert.ThrowsExceptionAsync<ExternalServiceHttpException>(async () => await sut.GetAlbumsByUserId(1));
            }

        }

        [TestMethod]
        public async Task ShouldThrowExternalServiceHttpExceptionOnTimeout()
        {
            using (var server = new StubHttpServer())
            {
                //Arrange
                var result = new List<Album>
                             {
                                 new Album
                                 {
                                     Id = 1,
                                     Title = "test"
                                 }
                             };

                server.SetupRoute("/albums?userid=1")
                    .Get()
                    .HangsFor(new TimeSpan(0, 0, 0, 5))
                    .ThenReturnsStatusCode(HttpStatusCode.OK)
                    .WithJsonContent(result);
                
                var config = new TestConfig
                {
                                 ExternalServicesTypiCodeUrl = server.Url,
                                 ExternalServicesTypiCodeTimeoutSeconds = 3
                             };

                var sut = new Typicode.TypiCodeClient(config);

                await Assert.ThrowsExceptionAsync<ExternalServiceHttpException>(async () => await sut.GetAlbumsByUserId(1));
            }

        }
    }
}
