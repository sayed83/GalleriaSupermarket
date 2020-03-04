namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablelostproductld : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LostProducts", "LostQnty", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LostProducts", "LostQnty");
        }
    }
}
