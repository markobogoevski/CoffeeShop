namespace CoffeeShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BasePrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CoffeeModels", "BasePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CoffeeModels", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CoffeeModels", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CoffeeModels", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CoffeeModels", "TotalPrice");
            DropColumn("dbo.CoffeeModels", "BasePrice");
        }
    }
}
