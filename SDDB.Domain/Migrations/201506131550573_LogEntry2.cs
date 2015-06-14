namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry2 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            AddColumn("PersonLogEntrys", "ManHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("PersonLogEntrys", "PersonHours");
        }
        
        public override void Down()
        {
            AddColumn("PersonLogEntrys", "PersonHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("PersonLogEntrys", "ManHours");
        }
    }
}
