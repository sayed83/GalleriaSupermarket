namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductRequestTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductRequests",
                c => new
                    {
                        ProductRequestID = c.Int(nullable: false, identity: true),
                        OutletId = c.Int(nullable: false),
                        UserName = c.String(),
                        Description = c.String(),
                        Status = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProductRequestID)
                .ForeignKey("dbo.Outlets", t => t.OutletId)
                .Index(t => t.OutletId);
            
            AddColumn("dbo.ItemTransfers", "AdminApproved", c => c.Boolean(nullable: false));
            AddColumn("dbo.ItemTransfers", "ManagerApproved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductRequests", "OutletId", "dbo.Outlets");
            DropIndex("dbo.ProductRequests", new[] { "OutletId" });
            DropColumn("dbo.ItemTransfers", "ManagerApproved");
            DropColumn("dbo.ItemTransfers", "AdminApproved");
            DropTable("dbo.ProductRequests");
        }
    }
}
