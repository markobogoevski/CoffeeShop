namespace CoffeeShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserWork : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CoffeeModels", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.CoffeeModels", "User_Id");
            AddForeignKey("dbo.CoffeeModels", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CoffeeModels", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.CoffeeModels", new[] { "User_Id" });
            DropColumn("dbo.CoffeeModels", "User_Id");
        }
    }
}
