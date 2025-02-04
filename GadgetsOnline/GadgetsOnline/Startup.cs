using System;
using System.Threading.Tasks;
using GadgetsOnline.Models;
using GadgetsOnline.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GadgetsOnline
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigurationManager.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<GadgetsOnlineEntities>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("GadgetsOnlineEntities")
            ));

            services.AddScoped<Inventory>();

            services.AddScoped<ShoppingCart>();

            services.AddScoped<OrderProcessing>();

            services.AddDistributedMemoryCache(); // Required for session state
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set your desired timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                var config = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                var db = serviceScope.ServiceProvider.GetRequiredService<GadgetsOnlineEntities>().Database;

                logger.LogInformation(config.GetConnectionString("GadgetsOnlineEntities"));
                logger.LogInformation("Migrating database...");

                var maxRetries = 3;
                var retryCount = 0;
                var delay = TimeSpan.FromSeconds(5);

                while (retryCount < maxRetries)
                {
                    try
                    {
                        if (!db.CanConnect())
                        {
                            try
                            {
                                db.OpenConnection();
                            }
                            catch (Exception ex)
                            {
                                logger.LogInformation(ex, "Error occured while connecting to database {Ex_Message}, {Ex_Inner_Message}", ex?.Message, ex.InnerException?.ToString());
                            }

                            logger.LogWarning("Database not ready. Attempt {RetryCount} of {MaxRetries}", retryCount + 1, maxRetries);
                            Task.Delay(delay);
                            retryCount++;
                            continue;
                        }

                        db.Migrate();
                        logger.LogInformation("Database migration completed successfully");
                        break;
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        if (retryCount == maxRetries)
                        {
                            logger.LogError(ex, "Database migration failed after {MaxRetries} attempts", maxRetries);
                            throw;
                        }

                        logger.LogWarning(ex, "Database migration attempt {RetryCount} of {MaxRetries} failed", retryCount, maxRetries);
                        Task.Delay(delay);
                    }
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //Added Middleware

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public static class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; }
    }
}
