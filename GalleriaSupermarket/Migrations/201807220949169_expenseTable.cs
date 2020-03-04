namespace GalleriaSupermarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class expenseTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Expenses", "CashOnHand", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Expenses", "CashOnHand");
        }
    }
}
