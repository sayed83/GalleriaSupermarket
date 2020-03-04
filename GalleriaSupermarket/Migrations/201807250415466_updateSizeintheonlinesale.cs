namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSizeintheonlinesale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OnlineSales", "ItemSize", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OnlineSales", "ItemSize");
        }
    }
}
