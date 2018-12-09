using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using RetroDb.Engine.Frontends;
using RetroDb.Repo;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;

namespace RetroDbBlaze.Server
{
    public class Startup
    {
        private IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        private ILogger _logger;

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ILogger<Startup> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Adds the Server-Side Blazor services, and those registered by the app project's startup.
            services.AddServerSideBlazor<App.Startup>();

            //Setup Uow / EF
            var connString = _configuration["ConnectionStrings:DefaultConnection"];           
            services.AddTransient<IUnitOfWork, UnitOfWork>((factory) =>
            {
                return new UnitOfWork(connString);
            });

            var rlDirectory = _configuration["RocketLauncher:InstallPath"];
            _logger.LogInformation($"Rocketlauncher Path: {rlDirectory}");
            services.AddSingleton(typeof(RocketLauncher), new RocketLauncher(rlDirectory));

            //MVC
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "RetroDb Api", Version = "v1" });
            });

            //SignalR
            services.AddSignalR();

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseResponseCompression();            
            if (_hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var connstring = _configuration["ConnectionStrings:DefaultConnection"];
            app.UseStaticFiles();

            #region Media Files

            try
            {
                PhysicalFileProvider physicalFileProvider;

                //Set up serving hyperspin media if enabled.            
                bool.TryParse(_configuration["Hyperspin:Enabled"], out var result);
                if (result)
                {
                    var hsDirectory = _configuration["Hyperspin:InstallPath"];
                    var mediaPath = _configuration["Hyperspin:MediaPath"];

                    //If user doesn't provide a media path then assume at the hyperspin install path                
                    if (!string.IsNullOrWhiteSpace(mediaPath))
                        physicalFileProvider = new PhysicalFileProvider(mediaPath);
                    else
                        physicalFileProvider = new PhysicalFileProvider(Path.Combine(hsDirectory, "Media"));

                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = physicalFileProvider,
                        RequestPath = "/HsFiles"
                    });

                    _logger.LogInformation($"Assigned Hyperspin media: {physicalFileProvider.Root}");
                }                

                //Set up static rocketlauncher files.
                var rlDirectory = _configuration["RocketLauncher:InstallPath"];
                var rlMediaPath = _configuration["RocketLauncher:MediaPath"];
                //If user doesn't provide a media path then assume at the hyperspin install path            
                if (!string.IsNullOrWhiteSpace(rlMediaPath))
                    physicalFileProvider = new PhysicalFileProvider(rlMediaPath);
                else
                    physicalFileProvider = new PhysicalFileProvider(Path.Combine(rlDirectory, "Media"));

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = physicalFileProvider,
                    RequestPath = "/RlFiles"
                });

                _logger.LogInformation($"Assigned Rocketlauncher media: {physicalFileProvider.Root}");
            }
            catch(Exception ex) { _logger.LogCritical(ex, "Failed creating static paths"); }
            #endregion

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RetroDb API v1");
            });
            app.UseSignalR(route =>
            {
                route.MapHub<RetroDb.Api.Hubs.RocketLauncherHub>("/chathub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
            });
            // Use component registrations and static files from the app project.
            app.UseServerSideBlazor<App.Startup>();
        }
    }
}
