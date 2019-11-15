using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Gallery.ExternalServices;
using Gallery.ExternalServices.Interfaces;
using Gallery.ExternalServices.Typicode.Models;
using Gallery.Services.Factories;
using Moq;
using NUnit.Framework;

namespace Gallery.Services.Tests
{
    [TestFixture]
    public class GalleryServiceShould
    {       
        [Test]
        public async Task ShouldReturnOneAlbumWithTwoPhotos()
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

        [Test]
        public async Task ShouldReturnOneAlbumWithNoPhotos()
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

        [Test]
        public async Task ShouldReturnEmptyListWhenNoAlbumsFoundForGivenUser()
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

        [Test]
        public void ShouldThrowExceptionWhenRemoteServerReturnsError500()
        {
            //Arrange
            var galleryClientMock = new Mock<IGalleryClient>();
            var typicodeClientFactoryMock = new Mock<ITypicodeClientFactory>();

            galleryClientMock.Setup(m => m.GetAlbumsByUserId(It.IsAny<int>())).ThrowsAsync(new ExternalServiceHttpException(500, "Server error"));
            galleryClientMock.Setup(m => m.GetPhotosByUserId(It.IsAny<int>())).ReturnsAsync((List<Photo>)null);
            typicodeClientFactoryMock.Setup(i => i.GetClient()).Returns(galleryClientMock.Object);

            var sut = new GalleryService(typicodeClientFactoryMock.Object);

            var ex = Assert.ThrowsAsync<ExternalServiceHttpException>(() => sut.GetAlbumsByUser(1));
            Assert.That(ex.Message, Is.EqualTo("Server error"));
        }

        [Test]
        public void ShouldThrowExceptionWhenRemoteServerReturnsError404()
        {
            //Arrange
            var galleryClientMock = new Mock<IGalleryClient>();
            var typicodeClientFactoryMock = new Mock<ITypicodeClientFactory>();

            galleryClientMock.Setup(m => m.GetAlbumsByUserId(It.IsAny<int>())).ThrowsAsync(new ExternalServiceHttpException(404, "not found"));
            galleryClientMock.Setup(m => m.GetPhotosByUserId(It.IsAny<int>())).ReturnsAsync((List<Photo>)null);
            typicodeClientFactoryMock.Setup(i => i.GetClient()).Returns(galleryClientMock.Object);

            var sut = new GalleryService(typicodeClientFactoryMock.Object);

            var ex = Assert.ThrowsAsync<ExternalServiceHttpException>(() => sut.GetAlbumsByUser(2));
            Assert.That(ex.Message, Is.EqualTo("not found"));
        }
    }
}
