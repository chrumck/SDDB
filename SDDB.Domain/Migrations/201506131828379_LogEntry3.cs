namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry3 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            AddColumn("PersonLogEntrys", "LogEntryDateTime", c => c.String(nullable: false, maxLength: 64, storeType: "nvarchar"));
            DropColumn("PersonLogEntrys", "LogEntryDate");
        }
        
        public override void Down()
        {
            AddColumn("PersonLogEntrys", "LogEntryDate", c => c.String(nullable: false, maxLength: 64, storeType: "nvarchar"));
            DropColumn("PersonLogEntrys", "LogEntryDateTime");
        }
    }
}
