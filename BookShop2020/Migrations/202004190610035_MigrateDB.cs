namespace BookShop2020.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LastName = c.String(maxLength: 50),
                        Name = c.String(maxLength: 50),
                        Address = c.String(),
                        TotalOrdersCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrdersNumber = c.Int(nullable: false),
                        ReviewsNumber = c.Int(nullable: false),
                        CurrentDiscount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.Books", "Price", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Books", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("dbo.Clients");
        }
    }
}
