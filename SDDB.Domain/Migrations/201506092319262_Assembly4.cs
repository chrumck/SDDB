namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assembly4 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            AddColumn("AssemblyDbs", "AssignedToLocation_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            CreateIndex("AssemblyDbs", "AssignedToLocation_Id");
            AddForeignKey("AssemblyDbs", "AssignedToLocation_Id", "Locations", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("AssemblyDbs", "AssignedToLocation_Id", "Locations");
            DropIndex("AssemblyDbs", new[] { "AssignedToLocation_Id" });
            DropColumn("AssemblyDbs", "AssignedToLocation_Id");
        }
    }
}
