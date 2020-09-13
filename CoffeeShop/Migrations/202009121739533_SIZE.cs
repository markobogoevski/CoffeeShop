namespace CoffeeShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SIZE : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItemModels", "CoffeeSize", c => c.String());
            DropColumn("dbo.CoffeeModels", "Size");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CoffeeModels", "Size", c => c.String(nullable: false));
            DropColumn("dbo.OrderItemModels", "CoffeeSize");
        }
    }
}
