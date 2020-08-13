using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityLearning.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using IdentityLearning.Infrastructure;
using IdentityLearning.Services;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace IdentityLearning
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();


            services.AddControllersWithViews(config =>
            {
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(15);
            });

            services.AddDbContext<TestIdentityDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"]);
            });

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;

            })
                .AddEntityFrameworkStores<TestIdentityDbContext>()
                .AddDefaultTokenProviders();

            //services.AddScoped<IAuthorizationService, PolicyAuthorize>();
            services.AddAuthorization(config =>
            {

                var type = typeof(persmission);
                var members = type.GetFields().Where(x => x.FieldType == typeof(persmission));
                foreach (var item in members)
                {
                    config.AddPolicy(item.Name,
                    policy => policy.RequireClaim("AccessLevel", item.Name));
                }

            });

            services.AddAuthentication().AddGoogle(config =>
            {
                config.ClientId = "933619901017-lvcj1k09vetbbfa3a4lg4aln1dtsqg8h.apps.googleusercontent.com";
                config.ClientSecret = "PW79CzRq1NwQccZxGVpVfnfD";
                config.SignInScheme = IdentityConstants.ExternalScheme;

            });
            services.ConfigureApplicationCookie(c =>
            {

                c.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                c.Cookie.Name = "Authorize";
                c.SlidingExpiration = true;
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(20);
            });

            services.AddScoped<IsUserExistFilter>();
            services.AddScoped<AccountService>();


        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            // app.UseStatusCodePages();
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=home}/{action=index}/{id?}");
            });

            //SeedData();
        }


    }
}
