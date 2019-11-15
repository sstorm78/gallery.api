using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Gallery.Services.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Services.Tests.Factories
{
    [TestFixture]
    public class TypicodeClientFactoryShould
    {
        public void GetClientShouldReturnNewInstanceOfTypicodeClient()
        {
            var settings = new TestConfig
                           {
                               ExternalServicesTypiCodeUrl = "http://test"
                           };

            var sut = new Gallery.Services.Tests.Factories.TypicodeClientFactory(settings);


        }
    }
}
