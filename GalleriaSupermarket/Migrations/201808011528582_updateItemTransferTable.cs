namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateItemTransferTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ItemTransfers", "AdminApproved");
            DropColumn("dbo.ItemTransfers", "TransQnty");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemTransfers", "TransQnty", c => c.Int(nullable: false));
            AddColumn("dbo.ItemTransfers", "AdminApproved", c => c.Boolean(nullable: false));
        }
    }
}
