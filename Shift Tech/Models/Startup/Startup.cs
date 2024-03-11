using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shift_Tech.DbModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Numerics;
using System;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Shift_Tech.Models.Startup
{

    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<ShopDbContext>((options) =>
            {
                SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
                options.UseSqlite("Data Source=D:\\Mein progectos\\Shift Tech\\Shift Tech\\ShopDb.db");
            });
            services.AddControllersWithViews();

            services.AddScoped<Shift_Tech.Models.Liqpay.LiqPay>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
            new CultureInfo("en-US"),
            new CultureInfo("uk-UA"),
        };
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ShopDbContext>()
            .AddDefaultTokenProviders();
            services.AddAuthentication()
         .AddGoogle(options =>
         {
             options.ClientId = "733461572156-9dv5q5pi0t98i2lqnr1i1pe3gge76dba.apps.googleusercontent.com";
             options.ClientSecret = "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";
         });
            services.AddControllersWithViews()
                .AddViewLocalization();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(120);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
    app.UseCookiePolicy();
            app.UseRequestLocalization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
        
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRequestLocalization();

            app.Use(async (context, next) =>
            {
                var selectedLanguage = context.Request.Cookies["SelectedLanguage"];

                if (!string.IsNullOrEmpty(selectedLanguage))
                {
                    CultureInfo.CurrentCulture = new CultureInfo(selectedLanguage);
                    CultureInfo.CurrentUICulture = new CultureInfo(selectedLanguage);
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Shop}/{action=Index}");

            });

        }

    }
}
