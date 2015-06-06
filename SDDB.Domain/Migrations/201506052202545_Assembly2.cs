namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assembly2 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            DropColumn("AssemblyDbs", "AssyDateScheduled");
            DropColumn("AssemblyDbs", "AssyDateExecuted");
        }
        
        public override void Down()
        {
            AddColumn("AssemblyDbs", "AssyDateExecuted", c => c.DateTime(precision: 0));
            AddColumn("AssemblyDbs", "AssyDateScheduled", c => c.DateTime(nullable: false, precision: 0));
        }
    }
}
