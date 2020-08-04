namespace CoffeeShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CoffeeModels",
                c => new
                    {
                        CoffeeId = c.Guid(nullable: false),
                        Name = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Size = c.String(),
                        ImgUrl = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CoffeeId);
            
            CreateTable(
                "dbo.IngredientModels",
                c => new
                    {
                        IngredientId = c.Guid(nullable: false),
                        Name = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImgUrl = c.String(),
                        Description = c.String(),
                        CoffeeModel_CoffeeId = c.Guid(),
                    })
                .PrimaryKey(t => t.IngredientId)
                .ForeignKey("dbo.CoffeeModels", t => t.CoffeeModel_CoffeeId)
                .Index(t => t.CoffeeModel_CoffeeId);
            
            CreateTable(
                "dbo.OrderItemModels",
                c => new
                    {
                        OrderItemId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Coffee_CoffeeId = c.Guid(),
                        Order_OrderId = c.Guid(),
                    })
                .PrimaryKey(t => t.OrderItemId)
                .ForeignKey("dbo.CoffeeModels", t => t.Coffee_CoffeeId)
                .ForeignKey("dbo.OrderModels", t => t.Order_OrderId)
                .Index(t => t.Coffee_CoffeeId)
                .Index(t => t.Order_OrderId);
            
            CreateTable(
                "dbo.OrderModels",
                c => new
                    {
                        OrderId = c.Guid(nullable: false),
                        Address = c.String(nullable: false),
                        OrderStatus = c.String(nullable: false),
                        OrderTime = c.DateTime(nullable: false),
                        OrderRating = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.OrderModels", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderItemModels", "Order_OrderId", "dbo.OrderModels");
            DropForeignKey("dbo.OrderItemModels", "Coffee_CoffeeId", "dbo.CoffeeModels");
            DropForeignKey("dbo.IngredientModels", "CoffeeModel_CoffeeId", "dbo.CoffeeModels");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.OrderModels", new[] { "User_Id" });
            DropIndex("dbo.OrderItemModels", new[] { "Order_OrderId" });
            DropIndex("dbo.OrderItemModels", new[] { "Coffee_CoffeeId" });
            DropIndex("dbo.IngredientModels", new[] { "CoffeeModel_CoffeeId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.OrderModels");
            DropTable("dbo.OrderItemModels");
            DropTable("dbo.IngredientModels");
            DropTable("dbo.CoffeeModels");
        }
    }
}
