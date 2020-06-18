using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.Db;

namespace WebApplication1
{
    public class Startup
    {
        private readonly String CookieAuthScheme = "CookieAuth";
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer("Server=.;Database=CookieAuthDatabase;Trusted_Connection=True;")
            );
            services.
                AddAuthentication(CookieAuthScheme).
                AddCookie(CookieAuthScheme, config =>
                {
                    config.Cookie.Name = "user.cookie";
                    config.LoginPath = "/Home/Login";
                    config.Cookie.MaxAge = TimeSpan.FromDays(7);
                    config.ExpireTimeSpan = TimeSpan.FromDays(7);
                });
            services.AddControllersWithViews();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            //determines user's identity
            app.UseAuthentication();
            //determines what a user is able to do
            app.UseAuthorization();
            app.UseEndpoints(opt => {
                opt.MapDefaultControllerRoute();
            });
        }
    }
}
