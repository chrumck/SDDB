namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry7 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            //AddColumn("PersonLogEntrys", "IsActive_bl", c => c.Boolean(nullable: false));
            //DropColumn("PersonLogEntrys", "IsActive");
            RenameColumn("PersonLogEntrys", "IsActive", "IsActive_bl");
        }
        
        public override void Down()
        {
            //AddColumn("PersonLogEntrys", "IsActive", c => c.Boolean(nullable: false));
            //DropColumn("PersonLogEntrys", "IsActive_bl");
            RenameColumn("PersonLogEntrys", "IsActive_bl", "IsActive");
        }
    }
}
