using AutoMapper;
using Filters;
using IdentityLearning.Infrastructure;
using IdentityLearning.Infrastructure.AutoMapper;
using MailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Middlewares;
using SharedServices.Context;
using SharedServices.GraphModel;
using SharedServices.Models.IdentityModels;
using System;
using System.Linq;
using UploadService;

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

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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
                var type = typeof(PersmissionsEnum);
                var members = type.GetFields().Where(x => x.FieldType == typeof(PersmissionsEnum));
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
                c.AccessDeniedPath = "/account/AccessDenied";
                c.LoginPath = "/account/login";
                c.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                c.Cookie.Name = "Authorize";
                c.SlidingExpiration = true;
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(20);
            });

            services.AddScoped<IsUserExistFilter>();
            services.AddScoped<FileUpload>();
            services.AddHostedService<TimedHostedService>();
            services.AddScoped<GraphRepository>();
            services.AddAntiforgery();
            services.AddAutoMapper(c => c.AddProfile(typeof(UserProfile)));

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {


            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/error");
                //force using https
                app.UseHsts();
            }
            app.UseDeviceChecker();
            app.UseBrowserChecker();
            app.UseCheckIp();
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


            InitializeDatabase(app);
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<TestIdentityDbContext>();
                if (context.Roles.Any(c => c.Name == "SuperAdmin"))
                    return;
                PasswordHasher<ApplicationUser> hashing = new PasswordHasher<ApplicationUser>(null);
                var userId = Guid.NewGuid().ToString();
                ApplicationUser user = new ApplicationUser()
                {
                    Email = "admin@admin.com",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    UserName = "admin@admin.com",
                    NormalizedUserName = "ADMIN@ADMIN.COM",
                    Id = userId,
                    IsActive = true,
                    EmailConfirmed = true
                };
                user.PasswordHash = hashing.HashPassword(user, "admin");
                context.Users.Add(user);
                var roleId = Guid.NewGuid().ToString();
                Role role = new Role()
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    Id = roleId
                };
                context.Roles.Add(role);

                var allClaims = ClaimToPermission.GetAllClaims();

                foreach (var item in allClaims)
                {
                    context.RoleClaims.Add(new IdentityRoleClaim<string>()
                    {
                        ClaimType = "AccessLevel",
                        ClaimValue = item.Key,
                        RoleId = role.Id
                    });
                }
                context.SaveChanges();
                context.UserRoles.Add(new IdentityUserRole<string>()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                });

                context.SaveChanges();
                context.Dispose();
            }

        }

    }
}
