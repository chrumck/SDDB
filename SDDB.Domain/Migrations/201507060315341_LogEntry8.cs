namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry8 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            AddColumn("ComponentLogEntrys", "LastCalibrationDate", c => c.DateTime(precision: 0));
            AddColumn("ComponentLogEntrys", "IsActive_bl", c => c.Boolean(nullable: false));
            DropColumn("ComponentLogEntrys", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("ComponentLogEntrys", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("ComponentLogEntrys", "IsActive_bl");
            DropColumn("ComponentLogEntrys", "LastCalibrationDate");
        }
    }
}
