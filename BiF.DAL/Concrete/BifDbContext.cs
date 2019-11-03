using System.Data.Entity;
using BiF.DAL.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BiF.DAL.Concrete
{
    public class BifDbContext : IdentityDbContext<BifIdentityUser>
    {
        public BifDbContext() : base("BifDbContext") { }

        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        //public DbSet<BifIdentityUser> Users { get; set; }

        public static BifDbContext Create() {
            return new BifDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            base.OnModelCreating(modelBuilder);
        }
    }

}
