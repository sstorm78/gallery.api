using System.IO;
using Gallery.ExternalServices.Interfaces;
using Gallery.ExternalServices.Typicode;
using Gallery.Models;
using Gallery.Services;
using Gallery.Services.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gallery.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddMvc();
            services.AddSingleton(configurationRoot);
            services.AddScoped<IConfig, Config>();
            services.AddScoped<IGalleryClient, TypiCodeClient>();
            services.AddScoped<IGalleryService, GalleryService>();
            services.AddScoped<ITypicodeClientFactory, TypicodeClientFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
