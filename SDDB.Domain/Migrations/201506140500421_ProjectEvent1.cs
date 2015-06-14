namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectEvent1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            AlterColumn("ProjectEvents", "EventCreated", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("ProjectEvents", "EventClosed", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            AlterColumn("ProjectEvents", "EventClosed", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AlterColumn("ProjectEvents", "EventCreated", c => c.String(nullable: false, maxLength: 64, storeType: "nvarchar"));
        }
    }
}
