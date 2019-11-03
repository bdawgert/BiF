using System.Data.Entity.Migrations;

namespace BiF.DAL.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<BiF.DAL.Concrete.BifDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BiF.DAL.Concrete.BifDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
