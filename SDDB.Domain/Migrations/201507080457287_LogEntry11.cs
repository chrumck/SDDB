namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry11 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            AddColumn("AssemblyLogEntrys", "IsActive_bl", c => c.Boolean(nullable: false));
            DropColumn("AssemblyLogEntrys", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("AssemblyLogEntrys", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("AssemblyLogEntrys", "IsActive_bl");
        }
    }
}
