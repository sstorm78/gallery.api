using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Gallery.ExternalServices;
using Gallery.ExternalServices.Interfaces;
using Gallery.ExternalServices.Typicode.Models;
using Gallery.Services.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gallery.Services.Tests
{
    [TestClass]
    public class GalleryServiceTests
    {
        private readonly Mock<IGalleryClient> _galleryClient;
        
        [TestMethod]
        public async Task Should_return_one_album_with_two_photos()
        {
            //Arrange
            var typicodeClientFactoryMock = new Mock<ITypicodeClientFactory>();
            var galleryClientMock = new Mock<IGalleryClient>();

            var albums = new List<Album>
                         {
                             new Album
                             {
                                 Id = 1
                             }
                         };

            var photos = new List<Photo>
                         {
                             new Photo
                             {
                                 AlbumId = 1
                             },
                             new Photo
                             {
                                 AlbumId = 1
                             }
                         };

            galleryClientMock.Setup(m => m.GetAlbumsByUserId(It.IsAny<int>())).ReturnsAsync(albums);
            galleryClientMock.Setup(m => m.GetPhotosByUserId(It.IsAny<int>())).ReturnsAsync(photos);

            typicodeClientFactoryMock.Setup(i => i.GetClient()).Returns(galleryClientMock.Object);

            var sut = new GalleryService(typicodeClientFactoryMock.Object);

            //Act
            var result = await sut.GetAlbumsByUser(1);

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.First().Id.Should().Be(1);
            result.First().Photos.Count.Should().Be(2);
        }

        [TestMethod]
        public async Task Should_return_one_album_with_no_photos()
        {
            //Arrange
            var galleryClientMock = new Mock<IGalleryClient>();
            var typicodeClientFactoryMock = new Mock<ITypicodeClientFactory>();

            var albums = new List<Album>
                         {
                             new Album
                             {
                                 Id = 1
                             }
                         };

            var photos = new List<Photo>
                         {
                             new Photo
                             {
                                 AlbumId = 2
                             },
                             new Photo
                             {
                                 AlbumId = 2
                             }
                         };

            galleryClientMock.Setup(m => m.GetAlbumsByUserId(It.IsAny<int>())).ReturnsAsync(albums);
            galleryClientMock.Setup(m => m.GetPhotosByUserId(It.IsAny<int>())).ReturnsAsync(photos);
            typicodeClientFactoryMock.Setup(i => i.GetClient()).Returns(galleryClientMock.Object);

            var sut = new GalleryService(typicodeClientFactoryMock.Object);

            //Act
            var result = await sut.GetAlbumsByUser(1);

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.First().Id.Should().Be(1);
            result.First().Photos.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task Should_return_empty_list_when_no_albums_found_for_given_user()
        {
            //Arrange
            var typicodeClientFactoryMock = new Mock<ITypicodeClientFactory>();
            var galleryClientMock = new Mock<IGalleryClient>();

            galleryClientMock.Setup(m => m.GetAlbumsByUserId(It.IsAny<int>())).ReturnsAsync((List<Album>)null);
            galleryClientMock.Setup(m => m.GetPhotosByUserId(It.IsAny<int>())).ReturnsAsync((List<Photo>)null);
            typicodeClientFactoryMock.Setup(i => i.GetClient()).Returns(galleryClientMock.Object);

            var sut = new GalleryService(typicodeClientFactoryMock.Object);

            //Act
            var result = await sut.GetAlbumsByUser(1);

            //Assert
            result.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public async Task Should_throw_exception_when_remote_server_returns_error_500()
        {
            //Arrange
            var galleryClientMock = new Mock<IGalleryClient>();
            var typicodeClientFactoryMock = new Mock<ITypicodeClientFactory>();

            galleryClientMock.Setup(m => m.GetAlbumsByUserId(It.IsAny<int>())).ThrowsAsync(new ExternalServiceHttpException(500, "Server error"));
            galleryClientMock.Setup(m => m.GetPhotosByUserId(It.IsAny<int>())).ReturnsAsync((List<Photo>)null);
            typicodeClientFactoryMock.Setup(i => i.GetClient()).Returns(galleryClientMock.Object);

            var sut = new GalleryService(typicodeClientFactoryMock.Object);

            await Assert.ThrowsExceptionAsync<ExternalServiceHttpException>(async () => await sut.GetAlbumsByUser(1));
        }

        [TestMethod]
        public async Task Should_throw_exception_when_remote_server_returns_error_404()
        {
            //Arrange
            var galleryClientMock = new Mock<IGalleryClient>();
            var typicodeClientFactoryMock = new Mock<ITypicodeClientFactory>();

            galleryClientMock.Setup(m => m.GetAlbumsByUserId(It.IsAny<int>())).ThrowsAsync(new ExternalServiceHttpException(404, "not found"));
            galleryClientMock.Setup(m => m.GetPhotosByUserId(It.IsAny<int>())).ReturnsAsync((List<Photo>)null);
            typicodeClientFactoryMock.Setup(i => i.GetClient()).Returns(galleryClientMock.Object);

            var sut = new GalleryService(typicodeClientFactoryMock.Object);

            await Assert.ThrowsExceptionAsync<ExternalServiceHttpException>(async () => await sut.GetAlbumsByUser(1));
        }
    }
}
