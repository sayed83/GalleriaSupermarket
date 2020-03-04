namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablelostproduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LostProducts",
                c => new
                    {
                        LostProductID = c.Int(nullable: false, identity: true),
                        ItemID = c.Int(nullable: false),
                        OutletID = c.Int(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        AddedBy = c.String(),
                    })
                .PrimaryKey(t => t.LostProductID)
                .ForeignKey("dbo.Items", t => t.ItemID)
                .ForeignKey("dbo.Outlets", t => t.OutletID)
                .Index(t => t.ItemID)
                .Index(t => t.OutletID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LostProducts", "OutletID", "dbo.Outlets");
            DropForeignKey("dbo.LostProducts", "ItemID", "dbo.Items");
            DropIndex("dbo.LostProducts", new[] { "OutletID" });
            DropIndex("dbo.LostProducts", new[] { "ItemID" });
            DropTable("dbo.LostProducts");
        }
    }
}
