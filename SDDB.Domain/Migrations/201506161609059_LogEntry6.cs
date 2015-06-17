namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry6 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            AddColumn("PersonLogEntrys", "AssignedToLocation_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            CreateIndex("PersonLogEntrys", "AssignedToLocation_Id");
            AddForeignKey("PersonLogEntrys", "AssignedToLocation_Id", "Locations", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("PersonLogEntrys", "AssignedToLocation_Id", "Locations");
            DropIndex("PersonLogEntrys", new[] { "AssignedToLocation_Id" });
            DropColumn("PersonLogEntrys", "AssignedToLocation_Id");
        }
    }
}
