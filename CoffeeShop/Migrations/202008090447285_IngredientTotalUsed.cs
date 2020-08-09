namespace CoffeeShop.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class IngredientTotalUsed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IngredientModels", "TotalQuantityUsed", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.IngredientModels", "TotalQuantityUsed");
        }
    }
}
