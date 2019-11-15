# Gallery API

Gallery API is a project that demonstrates .Net Core Web API interface to TypiCode API

# Features

  - provides GET /albums?userId=xxx endpoint that returns an array of albums with Id, Title and sub-array of photos in the album

### Installation

Gallery API requires VS2017 with internet access to Nuget
Restore the packages and run the project in the debug mode
Access http://localhost:8256/albums?userId=1 in the browser to query data for userid=1

### Projects

Gallery.API - The Web API project, currently provides only one endpoint GET /albums?userId=xxx
Gallery.ExternalServices - Implementation of a Http Client (based on RestSharp) and interface to TypiCode Web API
Gallery.Models - Models for cross-project objects
Gallery.Services - Business logic of the solution, GalleryService queries relevant TypiCode endpoints and combines the results

### Tests

Gallery API is currently covered with the following tests:

- Gallery.API.Tests - tests the endpoint of the solution, ensures that OK and exception cases are properly handled
- Gallery.ExternalServices.Tests - tests the TypiCode API client, ensures that OK,NotFound & timeout cases are handled properly
- Gallery.Services.Tests - tests the service layer and esures that the data is properly processed and combined as requested
