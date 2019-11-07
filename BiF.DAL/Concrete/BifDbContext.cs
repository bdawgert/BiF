using System.Data.Entity;
using BiF.DAL.Models;


namespace BiF.DAL.Concrete
{
    public class BifDbContext : DbContext //IdentityDbContext<BifIdentityUser>
    {
        public BifDbContext() : base("BifDbContext") { }

        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<IdentityUser> Users { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityClaim> Claims { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<SignUp> SignUps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<IdentityUser>()
                .HasOptional(x => x.Profile);

            modelBuilder.Entity<Match>()
                .HasRequired(c => c.Sender)
                .WithMany(x => x.SendingMatches)
                .HasForeignKey(x => x.SenderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Match>()
                .HasRequired(c => c.Recipient)
                .WithMany(x => x.ReceivingMatches)
                .HasForeignKey(x => x.RecipientId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Match>()
                .HasRequired(c => c.Exchange)
                .WithMany(x => x.Matches)
                .HasForeignKey(x => x.ExchangeId)
                .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<Exchange>()
                .HasRequired(c => c.Creator)
                .WithMany().HasForeignKey(x => x.CreatorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Exchange>()
                .HasRequired(c => c.Updater)
                .WithMany().HasForeignKey(x => x.UpdaterId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }

}
