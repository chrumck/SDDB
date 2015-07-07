namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry9 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            AddColumn("ComponentLogEntrys", "LogEntryDateTime", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("ComponentLogEntrys", "LogEntryDate");
        }
        
        public override void Down()
        {
            AddColumn("ComponentLogEntrys", "LogEntryDate", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("ComponentLogEntrys", "LogEntryDateTime");
        }
    }
}
