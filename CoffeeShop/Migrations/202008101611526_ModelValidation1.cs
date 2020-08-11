namespace CoffeeShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModelValidation1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CoffeeModels", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.CoffeeModels", "Size", c => c.String(nullable: false));
            AlterColumn("dbo.IngredientModels", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.IngredientModels", "Name", c => c.String());
            AlterColumn("dbo.CoffeeModels", "Size", c => c.String());
            AlterColumn("dbo.CoffeeModels", "Name", c => c.String());
        }
    }
}
