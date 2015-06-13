namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Logentry1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            DropForeignKey("PersonLogEntrys", "AssignedToLocation_Id", "Locations");
            DropIndex("PersonLogEntrys", new[] { "AssignedToLocation_Id" });

            AlterColumn("PersonLogEntrys", "AssignedToLocation_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            CreateIndex("PersonLogEntrys", "AssignedToLocation_Id");
            AddForeignKey("PersonLogEntrys", "AssignedToLocation_Id", "Locations", "Id");

            CreateTable(
                "AssemblyDbPersonLogEntries",
                c => new
                    {
                        AssemblyDb_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        PersonLogEntry_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.AssemblyDb_Id, t.PersonLogEntry_Id })
                .ForeignKey("AssemblyDbs", t => t.AssemblyDb_Id, cascadeDelete: true)
                .ForeignKey("PersonLogEntrys", t => t.PersonLogEntry_Id, cascadeDelete: true)
                .Index(t => t.AssemblyDb_Id)
                .Index(t => t.PersonLogEntry_Id);
            
            
        }
        
        public override void Down()
        {
            DropForeignKey("PersonLogEntrys", "AssignedToLocation_Id", "Locations");
            DropForeignKey("AssemblyDbPersonLogEntries", "PersonLogEntry_Id", "PersonLogEntrys");
            DropForeignKey("AssemblyDbPersonLogEntries", "AssemblyDb_Id", "AssemblyDbs");
            DropIndex("AssemblyDbPersonLogEntries", new[] { "PersonLogEntry_Id" });
            DropIndex("AssemblyDbPersonLogEntries", new[] { "AssemblyDb_Id" });
            DropIndex("PersonLogEntrys", new[] { "AssignedToLocation_Id" });
            AlterColumn("PersonLogEntrys", "AssignedToLocation_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            DropTable("AssemblyDbPersonLogEntries");
            CreateIndex("PersonLogEntrys", "AssignedToLocation_Id");
            AddForeignKey("PersonLogEntrys", "AssignedToLocation_Id", "Locations", "Id", cascadeDelete: true);
        }
    }
}
