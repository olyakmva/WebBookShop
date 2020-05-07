namespace BookShop2020.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ClientId", c => c.String());
        }
    }
}
