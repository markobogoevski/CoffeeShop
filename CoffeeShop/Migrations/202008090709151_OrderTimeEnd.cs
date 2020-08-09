namespace CoffeeShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderTimeEnd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "OrderFinishTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "OrderFinishTime");
        }
    }
}
