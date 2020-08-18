
using SharedServices.Models.IdentityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SharedServices.Models.Entities;

namespace SharedServices.Context
{

    public static class DatabaseFacadeExtensions
    {
        public static bool Exists(this DatabaseFacade source)
        {
            return source.GetService<IRelationalDatabaseCreator>().Exists();
        }
    }

    public class TestIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly object balanceLock = new object();
        public DbSet<ViewerCounter> ViewerCounter { get; set; }
        public DbSet<DeviceCounter> DeviceCounter { get; set; }
        public DbSet<BrowserCounter> BrowserCounter { get; set; }
        public TestIdentityDbContext(DbContextOptions options) : base(options)
        {
            lock (balanceLock)
            {
                
                if (!Database.Exists())
                    Database.EnsureCreated();

            }

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //this.user
            base.OnModelCreating(builder);
        }

    }
}
