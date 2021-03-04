using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;

namespace AutoWrapper.Samples.AspNetCore.DedicatedRequestResponseLogOutput
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
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var autoWrapperLogger = new LoggerConfiguration()
                .SetSerilogPlusDefaultConfiguration()
                .WriteTo.File(new RenderedCompactJsonFormatter(),"App_Data/Logs/log_autowrapper.json")
                .CreateLogger();
            
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions()
            {
                EnableExceptionLogging = true,
                EnableResponseLogging = true,
                ShouldLogRequestData = true,
                ShouldLogResponseData = true,
                UseApiProblemDetailsException = true,
                Logger = autoWrapperLogger,
            });
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}