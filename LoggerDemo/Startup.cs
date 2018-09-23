using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggerDemo
{
    public class Startup
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IConfigurationRoot configurationRoot;
        private readonly IHostingEnvironment hostEnvironment;
        
        
        public Startup(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            configurationRoot = builder.Build();

            this.loggerFactory = loggerFactory;
            this.hostEnvironment = hostingEnvironment;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            //Step1

            services.AddMvc();
            services.AddApplicationInsightsTelemetry(configurationRoot.GetSection("ApplicationInsights:InstrumentationKey"));                 
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }           

            //Step2
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=api}/{action=Home}/{id?}");
            });
            
            
        }
    }
}
