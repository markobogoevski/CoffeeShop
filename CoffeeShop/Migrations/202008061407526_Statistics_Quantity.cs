namespace CoffeeShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Statistics_Quantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CoffeeModels", "QuantityInStock", c => c.Int(nullable: false));
            AddColumn("dbo.CoffeeModels", "QuantitySoldLastWeek", c => c.Int(nullable: false));
            AddColumn("dbo.CoffeeModels", "TotalQuantitySold", c => c.Int(nullable: false));
            AddColumn("dbo.CoffeeModels", "IncomeCoef", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.IngredientModels", "QuantityInStock", c => c.Int(nullable: false));
            AddColumn("dbo.IngredientModels", "QuantityUsedLastWeek", c => c.Int(nullable: false));
            AddColumn("dbo.IngredientModels", "TotalQuantityUsed", c => c.Int(nullable: false));
            AddColumn("dbo.IngredientModels", "QuantityInCoffee", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.IngredientModels", "QuantityInCoffee");
            DropColumn("dbo.IngredientModels", "TotalQuantityUsed");
            DropColumn("dbo.IngredientModels", "QuantityUsedLastWeek");
            DropColumn("dbo.IngredientModels", "QuantityInStock");
            DropColumn("dbo.CoffeeModels", "IncomeCoef");
            DropColumn("dbo.CoffeeModels", "TotalQuantitySold");
            DropColumn("dbo.CoffeeModels", "QuantitySoldLastWeek");
            DropColumn("dbo.CoffeeModels", "QuantityInStock");
        }
    }
}
