using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrotterWatch.Core.Rbl.Provider;
using TrotterWatch.Models;

namespace TestApplication
{
    public class Startup
    {
        private readonly IEnumerable<IRblProvider> _providers;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _providers = new List<IRblProvider>
            {
                new RblProvider("Spamhaus", "zen.spamhaus.org", RblType.Both)
            };
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();

            app.UseTrotterWatch(new TrotterWatchOptions
            {
                RblProviders = _providers,
                Logger = loggerFactory.CreateLogger("TrotterWatchLogger")
            });

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
