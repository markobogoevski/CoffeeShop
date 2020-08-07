namespace CoffeeShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuantityIngredient : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IngredientInCoffeeModels",
                c => new
                    {
                        IngredientInCoffeeId = c.Guid(nullable: false),
                        CoffeeId = c.Guid(nullable: false),
                        IngredientId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IngredientInCoffeeId)
                .ForeignKey("dbo.CoffeeModels", t => t.CoffeeId, cascadeDelete: true)
                .ForeignKey("dbo.IngredientModels", t => t.IngredientId, cascadeDelete: true)
                .Index(t => t.CoffeeId)
                .Index(t => t.IngredientId);
            
            DropColumn("dbo.IngredientModels", "TotalQuantityUsed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IngredientModels", "TotalQuantityUsed", c => c.Int(nullable: false));
            DropForeignKey("dbo.IngredientInCoffeeModels", "IngredientId", "dbo.IngredientModels");
            DropForeignKey("dbo.IngredientInCoffeeModels", "CoffeeId", "dbo.CoffeeModels");
            DropIndex("dbo.IngredientInCoffeeModels", new[] { "IngredientId" });
            DropIndex("dbo.IngredientInCoffeeModels", new[] { "CoffeeId" });
            DropTable("dbo.IngredientInCoffeeModels");
        }
    }
}
