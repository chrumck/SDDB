namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry5 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            AlterColumn("PersonLogEntrys", "LogEntryDateTime", c => c.DateTime(nullable: false, precision: 0));
        }
        
        public override void Down()
        {
            AlterColumn("PersonLogEntrys", "LogEntryDateTime", c => c.String(nullable: false, maxLength: 64, storeType: "nvarchar"));
        }
    }
}
