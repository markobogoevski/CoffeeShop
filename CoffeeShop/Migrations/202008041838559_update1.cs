namespace CoffeeShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IngredientModels", "CoffeeModel_CoffeeId", "dbo.CoffeeModels");
            DropIndex("dbo.IngredientModels", new[] { "CoffeeModel_CoffeeId" });
            CreateTable(
                "dbo.IngredientModelCoffeeModels",
                c => new
                    {
                        IngredientModel_IngredientId = c.Guid(nullable: false),
                        CoffeeModel_CoffeeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.IngredientModel_IngredientId, t.CoffeeModel_CoffeeId })
                .ForeignKey("dbo.IngredientModels", t => t.IngredientModel_IngredientId, cascadeDelete: true)
                .ForeignKey("dbo.CoffeeModels", t => t.CoffeeModel_CoffeeId, cascadeDelete: true)
                .Index(t => t.IngredientModel_IngredientId)
                .Index(t => t.CoffeeModel_CoffeeId);
            
            DropColumn("dbo.IngredientModels", "CoffeeModel_CoffeeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IngredientModels", "CoffeeModel_CoffeeId", c => c.Guid());
            DropForeignKey("dbo.IngredientModelCoffeeModels", "CoffeeModel_CoffeeId", "dbo.CoffeeModels");
            DropForeignKey("dbo.IngredientModelCoffeeModels", "IngredientModel_IngredientId", "dbo.IngredientModels");
            DropIndex("dbo.IngredientModelCoffeeModels", new[] { "CoffeeModel_CoffeeId" });
            DropIndex("dbo.IngredientModelCoffeeModels", new[] { "IngredientModel_IngredientId" });
            DropTable("dbo.IngredientModelCoffeeModels");
            CreateIndex("dbo.IngredientModels", "CoffeeModel_CoffeeId");
            AddForeignKey("dbo.IngredientModels", "CoffeeModel_CoffeeId", "dbo.CoffeeModels", "CoffeeId");
        }
    }
}
