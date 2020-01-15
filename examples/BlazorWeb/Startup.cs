using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorStrap;
using EFCore.DbContextFactory.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.AttributeBuilder;

namespace BlazorWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            services.AddBootstrapCSS();

            //
            // Simplified setup with dependencies
            //
            services.AddPiranha(options =>
            {
                options.AddRazorRuntimeCompilation = true;

                options.UseFileStorage();
                options.UseImageSharp();
                options.UseManager();
                options.UseTinyMCE();
                options.UseMemoryCache();
                options.UseEF(db =>
                    db.UseSqlServer(Configuration.GetConnectionString("piranha")));
                options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                    db.UseSqlServer(Configuration.GetConnectionString("piranha")));
            });

            services.AddSqlServerDbContextFactory<Db>("piranha");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApi api)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            App.Init(api);

            // Configure cache level
            App.CacheLevel = Piranha.Cache.CacheLevel.Full;

            // Build content types
            new ContentTypeBuilder(api)
                .AddAssembly(typeof(Startup).Assembly)
                .Build()
                .DeleteOrphans();

            // Configure editor
            Piranha.Manager.Editor.EditorConfig.FromFile("editorconfig.json");

            /**
             *
             * Test another culture in the UI
             *
            var cultureInfo = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
             */

            //
            // Simplified setup with dependencies
            //
            app.UsePiranha(options => {
                options.UseManager();
                options.UseTinyMCE();
                options.UseIdentity();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            Seed.RunAsync(api).GetAwaiter().GetResult();
        }
    }
}
