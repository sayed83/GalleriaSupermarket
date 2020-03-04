namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAllTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Brands", "SubCategoryID", c => c.Int(nullable: false));
            CreateIndex("dbo.Brands", "SubCategoryID");
            AddForeignKey("dbo.Brands", "SubCategoryID", "dbo.SubCategories", "SubCategoryID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Brands", "SubCategoryID", "dbo.SubCategories");
            DropIndex("dbo.Brands", new[] { "SubCategoryID" });
            DropColumn("dbo.Brands", "SubCategoryID");
        }
    }
}
