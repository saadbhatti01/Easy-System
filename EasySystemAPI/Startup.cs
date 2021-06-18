using EasySystemAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace EasySystemAPI
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
            var EasySysDB = "Server= .; Database= DB_A57897_uniSkills;Trusted_Connection= True; MultipleActiveResultSets= True";
            //var EasySysDB = "Data Source=SQL5052.site4now.net;Initial Catalog=DB_A57897_uniSkills;User Id=DB_A57897_uniSkills_admin;Password=Nopassword0;";
            //var EasySysDB = "Data Source=SQL5074.site4now.net;Initial Catalog=db_a71534_test;User Id=db_a71534_test_admin;Password=Test123*";

            //services.AddCors(c =>
            //{
            //    c.AddPolicy("AllowOrigin", options => options.WithOrigins("https://localhost:60337", "https://www.leskills.com/"));
            //});

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
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

            services.AddDbContext<EasyContext>(options => options.UseSqlServer(EasySysDB));
            //services.AddDbContext<EasyContext>(options => options.UseSqlServer(Configuration.GetConnectionString("EasySystem")));
            services.Configure<SkillsFee>(Configuration.GetSection("SkillsFee"));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors(options => options.AllowAnyOrigin());
            //app.UseCors(options => options.WithOrigins("https://www.leskills.com/", "http://localhost:60337"));
            //app.UseCors(options => options.WithOrigins("https://www.leskills.com/").AllowAnyMethod().AllowAnyHeader());
            app.UseMvc();
        }
    }
}
