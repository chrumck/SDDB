namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntryDateIndex : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            CreateIndex("PersonLogEntrys", "LogEntryDateTime");
            CreateIndex("ComponentLogEntrys", "LogEntryDateTime");
            CreateIndex("AssemblyLogEntrys", "LogEntryDateTime");
        }
        
        public override void Down()
        {
            DropIndex("AssemblyLogEntrys", new[] { "LogEntryDateTime" });
            DropIndex("ComponentLogEntrys", new[] { "LogEntryDateTime" });
            DropIndex("PersonLogEntrys", new[] { "LogEntryDateTime" });
        }
    }
}
