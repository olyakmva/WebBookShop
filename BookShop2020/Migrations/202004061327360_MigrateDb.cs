namespace BookShop2020.Migrations
{
   using System.Data.Entity.Migrations;
    
    public partial class MigrateDb : DbMigration
    {
        public override void Up()
            {
                AddColumn("dbo.Books", "Number", c => c.Int(nullable: false));
                AddColumn("dbo.Books", "ImageUrl", c => c.String());
            }
    }
}

