using System;
using System.Collections.Generic;
using System.Text;
using Gallery.Models;

namespace Gallery.Services.Tests
{
    public class TestConfig : Gallery.Models.IConfig
    {
        public string ExternalServicesTypiCodeUrl { get; set; }

        public int ExternalServicesTypiCodeTimeoutSeconds { get; set; }
    }
}
