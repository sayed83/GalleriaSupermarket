namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSizeintheonlinesaled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OnlineSales", "ItemPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OnlineSales", "ItemPrice");
        }
    }
}
