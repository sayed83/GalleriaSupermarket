namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductRequestTabled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemTransfers", "ItemFrom", c => c.Int(nullable: false));
            AddColumn("dbo.ItemTransfers", "TransQnty", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemTransfers", "TransQnty");
            DropColumn("dbo.ItemTransfers", "ItemFrom");
        }
    }
}
