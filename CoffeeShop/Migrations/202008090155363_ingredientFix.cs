namespace CoffeeShop.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ingredientFix : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.IngredientModels", "QuantityInCoffee");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IngredientModels", "QuantityInCoffee", c => c.Int(nullable: false));
        }
    }
}
