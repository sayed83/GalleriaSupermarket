namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatenew : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Items", "AvailableQnty", c => c.Int(nullable: false));
            AlterColumn("dbo.InternalSales", "OrderQnty", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InternalSales", "OrderQnty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Items", "AvailableQnty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
