using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Gallery.API.Controllers;
using Gallery.ExternalServices;
using Gallery.Models;
using Gallery.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Gallery.API.Tests
{
    [TestFixture]
    public class AlbumControllerShould
    {

        [Test]
        public async Task ShouldReturnOKResultWithOneAlbum()
        {
            //Arrange
            var galleryServiceMock = new Mock<IGalleryService>();
            

            var albums = new List<Album>
                         {
                             new Album(1,"test",new List<Photo>())
                         };

            galleryServiceMock.Setup(m => m.GetAlbumsByUser(It.IsAny<int>())).ReturnsAsync(albums);

            var sut = new AlbumController(galleryServiceMock.Object);

            //Act
            var result = await sut.GetAlbumsByUser(1);

            //Assert
            result.Should().BeOfType<OkObjectResult>();
            ((List<Album>) ((OkObjectResult) result).Value).First().Title.Should().Be("test");
        }

        [Test]
        public async Task ShouldReturnOKResultWithEmptyList()
        {
            //Arrange
            var galleryServiceMock = new Mock<IGalleryService>();

            galleryServiceMock.Setup(m => m.GetAlbumsByUser(It.IsAny<int>())).ReturnsAsync(new List<Album>());

            var sut = new AlbumController(galleryServiceMock.Object);

            //Act
            var result = await sut.GetAlbumsByUser(1);

            //Assert
            result.Should().BeOfType<OkObjectResult>();
            ((List<Album>) ((OkObjectResult) result).Value).Count.Should().Be(0);
        }

        [Test]
        public async Task ShouldReturnNotfoundWhenServiceThrowsAnException()
        {
            //Arrange
            var galleryServiceMock = new Mock<IGalleryService>();

            galleryServiceMock.Setup(m => m.GetAlbumsByUser(It.IsAny<int>())).ThrowsAsync(new ExternalServiceHttpException(500, "Server error"));

            var sut = new AlbumController(galleryServiceMock.Object);

            //Act
            var result = await sut.GetAlbumsByUser(1);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
