namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry10 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            AddColumn("AssemblyLogEntrys", "LogEntryDateTime", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("AssemblyLogEntrys", "LogEntryDate");
        }
        
        public override void Down()
        {
            AddColumn("AssemblyLogEntrys", "LogEntryDate", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("AssemblyLogEntrys", "LogEntryDateTime");
        }
    }
}
