namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateExpensetable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Expenses", "CashOnHand", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Expenses", "CashOnHand", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
