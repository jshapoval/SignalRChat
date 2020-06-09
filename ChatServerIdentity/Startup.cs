using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServerIdentity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatServerIdentity
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
            // Добавляем сервис валидатора пароля
            services.AddTransient<IPasswordValidator<User>,
                CustomPasswordValidator>(serv => new CustomPasswordValidator(3));
            //Добавляем сервис валидатора логина
            services.AddTransient<IUserValidator<User>, CustomUserValidator>();

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>();

            //services.AddIdentity<User, IdentityRole>()
            //.AddEntityFrameworkStores<ApplicationContext>()
            //.AddDefaultTokenProviders();

            services.AddControllersWithViews();

            services.AddSignalR();
        }


        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();    // подключение аутентификации
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