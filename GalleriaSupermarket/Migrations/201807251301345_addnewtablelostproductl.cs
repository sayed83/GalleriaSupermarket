namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablelostproductl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LostProducts", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LostProducts", "IsDeleted");
        }
    }
}
