using EasySystem.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;

namespace EasySystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            GetAPI api = new GetAPI();

            Configuration = configuration;
            configuration.Bind(api);
            string MyAPI = api.MyAPI;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
           .AddJsonOptions(options =>
           {

               var resolver = options.SerializerSettings.ContractResolver;
               if (resolver != null)
               {
                   (resolver as DefaultContractResolver).NamingStrategy = null;
               }
           });
            services.AddMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);
            services.Configure<GetAPI>(Configuration.GetSection("GetAPI"));
            services.AddSingleton<DataProtection>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
                        {
                            routes.MapRoute(
                                name: "default",
                                template: "{controller=Home}/{action=Index}/{id?}");

                            routes.MapRoute(
                               name: "Blogs",
                               template: "{controller=Blogs}/{action=BlogDetail}/{id:int}/{title}");
                        });
        }
    }
}
