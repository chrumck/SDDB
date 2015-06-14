namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry4 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            DropForeignKey("AssemblyLogEntrys", "AssignedToPersonLogEntry_Id", "PersonLogEntrys");
            DropForeignKey("PersonLogEntrys", "AssignedToLocation_Id", "Locations");
            DropForeignKey("ComponentLogEntrys", "AssignedToPersonLogEntry_Id", "PersonLogEntrys");
            DropIndex("PersonLogEntrys", new[] { "AssignedToLocation_Id" });
            DropIndex("AssemblyLogEntrys", new[] { "AssignedToPersonLogEntry_Id" });
            DropIndex("ComponentLogEntrys", new[] { "AssignedToPersonLogEntry_Id" });
            DropColumn("PersonLogEntrys", "AssignedToLocation_Id");
            DropColumn("AssemblyLogEntrys", "AssignedToPersonLogEntry_Id");
            DropColumn("ComponentLogEntrys", "AssignedToPersonLogEntry_Id");
        }
        
        public override void Down()
        {
            AddColumn("ComponentLogEntrys", "AssignedToPersonLogEntry_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("AssemblyLogEntrys", "AssignedToPersonLogEntry_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("PersonLogEntrys", "AssignedToLocation_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            CreateIndex("ComponentLogEntrys", "AssignedToPersonLogEntry_Id");
            CreateIndex("AssemblyLogEntrys", "AssignedToPersonLogEntry_Id");
            CreateIndex("PersonLogEntrys", "AssignedToLocation_Id");
            AddForeignKey("ComponentLogEntrys", "AssignedToPersonLogEntry_Id", "PersonLogEntrys", "Id");
            AddForeignKey("PersonLogEntrys", "AssignedToLocation_Id", "Locations", "Id");
            AddForeignKey("AssemblyLogEntrys", "AssignedToPersonLogEntry_Id", "PersonLogEntrys", "Id");
        }
    }
}
