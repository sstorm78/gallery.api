using FluentAssertions;
using Gallery.ExternalServices.Typicode;
using NUnit.Framework;

namespace Gallery.Services.Tests.Factories
{
    [TestFixture]
    public class TypicodeClientFactoryShould
    {
        [Test]
        public void GetClientShouldReturnNewInstanceOfTypicodeClient()
        {
            var settings = new TestConfig
                           {
                               ExternalServicesTypiCodeUrl = "http://test"
                           };

            var sut = new Services.Factories.TypicodeClientFactory(settings);

            var result = sut.GetClient();

            result.Should().NotBeNull();
            result.Should().BeOfType<TypiCodeClient>();
        }
    }
}
