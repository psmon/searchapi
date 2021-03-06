﻿using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using SearchApi.Config;
using SearchApi.Repositories;
using SearchApi.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace SearchApi
{
    public class Startup
    {
        private string AppName = "Search API - 검색엔진";
        private string Company = "웹노리";
        private string CompanyUrl = "http://wiki.webnori.com";
        private string DocUrl = "/docs/index.html";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton(Configuration.GetSection("AppSettings").Get<AppSettings>());// * AppSettings

            services.AddDbContext<SearchRepository>();

            services.AddScoped<SearchIndex>();

            services.AddElasticsearch(Configuration);

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = AppName,
                    Description = $"{AppName} ASP.NET Core Web API",
                    //TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = $"{Company} {AppName} Document",
                        Url = DocUrl
                    },
                    License = new License
                    {
                        Name = $"{Company}",
                        Url = CompanyUrl
                    }
                });
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            // API주소룰 소문자로...
            //services.AddRouting(options => options.LowercaseUrls = true);

            services.AddDirectoryBrowser();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles(); // For the wwwroot folder

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ts")),
                RequestPath = "/ts"
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ts")),
                RequestPath = "/ts"
            });

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.               
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", AppName + "V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureCustomExceptionMiddleware();

            app.UseMvc();
        }
    }
}
