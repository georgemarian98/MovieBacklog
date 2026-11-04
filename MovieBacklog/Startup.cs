using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MovieBacklog.Data;
using MovieBacklog.Services;
using System;

namespace MovieBacklog
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
            /*
             * This will raise a serios problem on docker. 
             * Do I need to do a live database migation on startup?
             * What if the database file is on a mounted volume?
             * I think the best aproach is:
             *      if the database file extsts do nothing
             *      if it does not exist run the migration
             * */
            string databaseLocation = Environment.GetEnvironmentVariable("DATABASE_LOCATION") ?? "movieBacklog.db";
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite($"Data Source={databaseLocation}"));
            services.AddRazorPages();
            services.AddScoped<IMediaRecordService, MediaService>();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Movies}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
