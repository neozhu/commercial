namespace WebApp.Migrations
{
  using System;
  using System.Data.Entity;
  using System.Data.Entity.Migrations;
  using System.Linq;

  internal sealed class Configuration : DbMigrationsConfiguration<WebApp.Models.StoreContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = false;
      AutomaticMigrationDataLossAllowed = true;
    }

    protected override void Seed(WebApp.Models.StoreContext context)
    {
      //  This method will be called after migrating to the latest version.

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method
      //  to avoid creating duplicate seed data.
    }
  }
}
