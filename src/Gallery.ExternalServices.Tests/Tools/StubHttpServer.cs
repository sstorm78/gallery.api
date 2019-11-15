using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Gallery.ExternalServices.Tests.Tools
{
    /// <summary>
    /// A stub HTTP server that can server canned responses for use in automated tests.
    /// </summary>
    public sealed class StubHttpServer : IDisposable
    {
        private readonly IDictionary<string, StubRoute> _routes;
        private readonly IWebHost _webHost;

        /// <summary>
        /// The URL that this server is listening on.
        /// </summary>
        public string Url => ((IServerAddressesFeature)_webHost.ServerFeatures.ElementAt(0).Value).Addresses.ElementAt(0);

        /// <summary>
        /// Creates and starts a new stub HTTP server. Use the SetupRoute() method to setup one or more routes that return canned responses.
        /// </summary>
        public StubHttpServer()
        {
            _routes = new Dictionary<string, StubRoute>(StringComparer.OrdinalIgnoreCase);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            _webHost = BuildAndStartServer();
        }

        private IWebHost BuildAndStartServer()
        {
            // By setting the port number to 0 the TCP stack will assign the next available port.
            const string url = "http://127.0.0.1:0";

            var host = new WebHostBuilder()
                .UseKestrel()
                .Configure(app =>
                {
                    app.Run(HttpRequestHandler);
                });

            return host.Start(url);
        }

        private async Task HttpRequestHandler(HttpContext context)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var requestFullUrl = context.Request.Path + context.Request.QueryString.ToUriComponent();

            var route = _routes.ContainsKey(requestFullUrl) ? _routes[requestFullUrl] : null;

            if (route == null || !route.HttpMethod.Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync($"No [{context.Request.Method}] route was setup on this stub HTTP server for endpoint [{context.Request.Path}].");
            }
            else
            {
                await route.RequestHandler(context);
            }
        }

        /// <summary>
        /// Setup a new route on this stub HTTP server using a fluent API. You can setup as many routes as you need.
        /// </summary>
        public HttpMethodBuilder SetupRoute(string endpoint)
        {
            var route = new StubRoute
            {
                Endpoint = endpoint
            };

            return new HttpMethodBuilder(this, route);
        }

        private void AddRoute(StubRoute route)
        {
            _routes[route.Endpoint] = route;
        }

        public void Dispose()
        {
            _webHost.Dispose();
        }

        public class HttpMethodBuilder
        {
            private readonly StubHttpServer _stubServer;
            private readonly StubRoute _route;

            public HttpMethodBuilder(StubHttpServer stubServer, StubRoute route)
            {
                _stubServer = stubServer;
                _route = route;
            }

            public StatusCodeOrHangBuilder Get()
            {
                _route.HttpMethod = "GET";
                return new StatusCodeOrHangBuilder(_stubServer, _route);
            }
        }

        public class StatusCodeOrHangBuilder
        {
            private readonly StubHttpServer _stubServer;
            private readonly StubRoute _route;

            public StatusCodeOrHangBuilder(StubHttpServer stubServer, StubRoute route)
            {
                _stubServer = stubServer;
                _route = route;
            }

            /// <summary>
            /// Configures this route to hang for the provided amount of time before returning a response.
            /// </summary>
            public StatusCodeAfterHangBuilder HangsFor(TimeSpan delay)
            {
                _route.Delay = delay;
                return new StatusCodeAfterHangBuilder(_stubServer, _route);
            }

            public ResponseContentBuilder ReturnsStatusCode(HttpStatusCode statusCode)
            {
                _route.StatusCode = statusCode;
                return new ResponseContentBuilder(_stubServer, _route);
            }
        }

        public class StatusCodeAfterHangBuilder
        {
            private readonly StubHttpServer _stubServer;
            private readonly StubRoute _route;

            public StatusCodeAfterHangBuilder(StubHttpServer stubServer, StubRoute route)
            {
                _stubServer = stubServer;
                _route = route;
            }

            public ResponseContentBuilder ThenReturnsStatusCode(HttpStatusCode statusCode)
            {
                _route.StatusCode = statusCode;
                return new ResponseContentBuilder(_stubServer, _route);
            }
        }

        public class ResponseContentBuilder
        {
            private readonly StubHttpServer _stubServer;
            private readonly StubRoute _route;

            public ResponseContentBuilder(StubHttpServer stubServer, StubRoute route)
            {
                _stubServer = stubServer;
                _route = route;
            }

            public StubHttpServer WithNoContent()
            {
                _stubServer.AddRoute(_route);
                return _stubServer;
            }

            public StubHttpServer WithTextContent(string text)
            {
                _route.ResponsEncoding = Encoding.UTF8;
                _route.ResponseContent = text;
                _route.ResponseContentType = "text/plain; charset=UTF-8";

                _stubServer.AddRoute(_route);
                return _stubServer;
            }

            public StubHttpServer WithJsonContent(string json)
            {
                _route.ResponsEncoding = Encoding.UTF8;
                _route.ResponseContent = json;
                _route.ResponseContentType = "application/json; charset=UTF-8";

                _stubServer.AddRoute(_route);
                return _stubServer;
            }

            public StubHttpServer WithJsonContent(object toSerialize)
            {
                var json = JsonConvert.SerializeObject(
                    toSerialize,
                    Formatting.Indented,
                    new JsonSerializerSettings());

                return WithJsonContent(json);
            }

            public ResponseContentBuilder WhenInvoked(Action<HttpContext> callback)
            {
                _route.OnInvokedCallback = callback;
                return this;
            }
        }
    }

    public class StubRoute
    {
        public string HttpMethod { get; set; }

        public string Endpoint { get; set; }

        public TimeSpan Delay { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string ResponseContent { get; set; }

        public Encoding ResponsEncoding { get; set; }

        public string ResponseContentType { get; set; }

        public Action<HttpContext> OnInvokedCallback { get; set; }

        public RequestDelegate RequestHandler
        {
            get
            {
                return async context =>
                {
                    if (Delay > TimeSpan.Zero)
                    {
                        await Task.Delay(Delay);
                    }

                    context.Response.StatusCode = (int)StatusCode;
                    context.Response.ContentType = ResponseContentType;

                    if (!string.IsNullOrEmpty(ResponseContent))
                    {
                        await context.Response.WriteAsync(ResponseContent, ResponsEncoding);
                    }

                    OnInvokedCallback?.Invoke(context);
                };
            }
        }
    }

}
