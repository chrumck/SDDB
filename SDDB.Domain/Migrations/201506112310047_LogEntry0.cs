namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntry0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "PersonActivityTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        ActivityTypeName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        ActivityTypeAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ActivityTypeName, unique: true)
                .Index(t => t.ActivityTypeAltName, unique: true);

            CreateTable(
                "ProjectEvents",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        EventName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        EventAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        AssignedToProject_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        EventCreated = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
                        CreatedByPerson_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        EventClosed = c.String(maxLength: 64, storeType: "nvarchar"),
                        ClosedByPerson_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Projects", t => t.AssignedToProject_Id, cascadeDelete: true)
                .ForeignKey("Persons", t => t.ClosedByPerson_Id)
                .ForeignKey("Persons", t => t.CreatedByPerson_Id, cascadeDelete: true)
                .Index(t => t.EventName, unique: true)
                .Index(t => t.EventAltName, unique: true)
                .Index(t => t.AssignedToProject_Id)
                .Index(t => t.CreatedByPerson_Id)
                .Index(t => t.ClosedByPerson_Id);

            CreateTable(
                "PersonLogEntrys",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        LogEntryDate = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
                        AssignedToProject_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssignedToLocation_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        PersonActivityType_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        PersonHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssignedToProjectEvent_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Locations", t => t.AssignedToLocation_Id, cascadeDelete: true)
                .ForeignKey("Projects", t => t.AssignedToProject_Id, cascadeDelete: true)
                .ForeignKey("ProjectEvents", t => t.AssignedToProjectEvent_Id)
                .ForeignKey("PersonActivityTypes", t => t.PersonActivityType_Id, cascadeDelete: true)
                .Index(t => t.AssignedToProject_Id)
                .Index(t => t.AssignedToLocation_Id)
                .Index(t => t.PersonActivityType_Id)
                .Index(t => t.AssignedToProjectEvent_Id);

            CreateTable(
                "PersonLogEntryPersons",
                c => new
                    {
                        PersonLogEntry_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.PersonLogEntry_Id, t.Person_Id })
                .ForeignKey("PersonLogEntrys", t => t.PersonLogEntry_Id, cascadeDelete: true)
                .ForeignKey("Persons", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.PersonLogEntry_Id)
                .Index(t => t.Person_Id);
           
            CreateTable(
                "ComponentLogEntrys",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Component_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssignedToPersonLogEntry_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                        LogEntryDate = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
                        ComponentStatus_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssignedToProject_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssignedToAssemblyDb_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AssemblyDbs", t => t.AssignedToAssemblyDb_Id)
                .ForeignKey("PersonLogEntrys", t => t.AssignedToPersonLogEntry_Id)
                .ForeignKey("Projects", t => t.AssignedToProject_Id, cascadeDelete: true)
                .ForeignKey("ComponentStatuss", t => t.ComponentStatus_Id, cascadeDelete: true)
                .ForeignKey("Persons", t => t.Person_Id, cascadeDelete: true)
                .ForeignKey("Components", t => t.Component_Id, cascadeDelete: true)
                .Index(t => t.Component_Id)
                .Index(t => t.Person_Id)
                .Index(t => t.AssignedToPersonLogEntry_Id)
                .Index(t => t.ComponentStatus_Id)
                .Index(t => t.AssignedToProject_Id)
                .Index(t => t.AssignedToAssemblyDb_Id);
            
            CreateTable(
                "AssemblyLogEntrys",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssemblyDb_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssignedToPersonLogEntry_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                        LogEntryDate = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
                        AssemblyStatus_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssignedToProject_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssignedToLocation_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssyGlobalX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssyGlobalY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssyGlobalZ = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssyLocalXDesign = c.Decimal(precision: 18, scale: 2),
                        AssyLocalYDesign = c.Decimal(precision: 18, scale: 2),
                        AssyLocalZDesign = c.Decimal(precision: 18, scale: 2),
                        AssyLocalXAsBuilt = c.Decimal(precision: 18, scale: 2),
                        AssyLocalYAsBuilt = c.Decimal(precision: 18, scale: 2),
                        AssyLocalZAsBuilt = c.Decimal(precision: 18, scale: 2),
                        AssyStationing = c.Decimal(precision: 18, scale: 2),
                        AssyLength = c.Decimal(precision: 18, scale: 2),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AssemblyStatuss", t => t.AssemblyStatus_Id, cascadeDelete: true)
                .ForeignKey("Locations", t => t.AssignedToLocation_Id, cascadeDelete: true)
                .ForeignKey("PersonLogEntrys", t => t.AssignedToPersonLogEntry_Id)
                .ForeignKey("Projects", t => t.AssignedToProject_Id, cascadeDelete: true)
                .ForeignKey("Persons", t => t.Person_Id, cascadeDelete: true)
                .ForeignKey("AssemblyDbs", t => t.AssemblyDb_Id, cascadeDelete: true)
                .Index(t => t.AssemblyDb_Id)
                .Index(t => t.Person_Id)
                .Index(t => t.AssignedToPersonLogEntry_Id)
                .Index(t => t.AssemblyStatus_Id)
                .Index(t => t.AssignedToProject_Id)
                .Index(t => t.AssignedToLocation_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("AssemblyLogEntrys", "AssemblyDb_Id", "AssemblyDbs");
            DropForeignKey("AssemblyLogEntrys", "Person_Id", "Persons");
            DropForeignKey("AssemblyLogEntrys", "AssignedToProject_Id", "Projects");
            DropForeignKey("AssemblyLogEntrys", "AssignedToPersonLogEntry_Id", "PersonLogEntrys");
            DropForeignKey("AssemblyLogEntrys", "AssignedToLocation_Id", "Locations");
            DropForeignKey("AssemblyLogEntrys", "AssemblyStatus_Id", "AssemblyStatuss");
            DropForeignKey("ComponentLogEntrys", "Component_Id", "Components");
            DropForeignKey("ComponentLogEntrys", "Person_Id", "Persons");
            DropForeignKey("ComponentLogEntrys", "ComponentStatus_Id", "ComponentStatuss");
            DropForeignKey("ComponentLogEntrys", "AssignedToProject_Id", "Projects");
            DropForeignKey("ComponentLogEntrys", "AssignedToPersonLogEntry_Id", "PersonLogEntrys");
            DropForeignKey("ComponentLogEntrys", "AssignedToAssemblyDb_Id", "AssemblyDbs");
            DropForeignKey("PersonLogEntrys", "PersonActivityType_Id", "PersonActivityTypes");
            DropForeignKey("PersonLogEntryPersons", "Person_Id", "Persons");
            DropForeignKey("PersonLogEntryPersons", "PersonLogEntry_Id", "PersonLogEntrys");
            DropForeignKey("PersonLogEntrys", "AssignedToProjectEvent_Id", "ProjectEvents");
            DropForeignKey("ProjectEvents", "CreatedByPerson_Id", "Persons");
            DropForeignKey("ProjectEvents", "ClosedByPerson_Id", "Persons");
            DropForeignKey("ProjectEvents", "AssignedToProject_Id", "Projects");
            DropForeignKey("PersonLogEntrys", "AssignedToProject_Id", "Projects");
            DropForeignKey("PersonLogEntrys", "AssignedToLocation_Id", "Locations");
            DropIndex("PersonLogEntryPersons", new[] { "Person_Id" });
            DropIndex("PersonLogEntryPersons", new[] { "PersonLogEntry_Id" });
            DropIndex("AssemblyLogEntrys", new[] { "AssignedToLocation_Id" });
            DropIndex("AssemblyLogEntrys", new[] { "AssignedToProject_Id" });
            DropIndex("AssemblyLogEntrys", new[] { "AssemblyStatus_Id" });
            DropIndex("AssemblyLogEntrys", new[] { "AssignedToPersonLogEntry_Id" });
            DropIndex("AssemblyLogEntrys", new[] { "Person_Id" });
            DropIndex("AssemblyLogEntrys", new[] { "AssemblyDb_Id" });
            DropIndex("ComponentLogEntrys", new[] { "AssignedToAssemblyDb_Id" });
            DropIndex("ComponentLogEntrys", new[] { "AssignedToProject_Id" });
            DropIndex("ComponentLogEntrys", new[] { "ComponentStatus_Id" });
            DropIndex("ComponentLogEntrys", new[] { "AssignedToPersonLogEntry_Id" });
            DropIndex("ComponentLogEntrys", new[] { "Person_Id" });
            DropIndex("ComponentLogEntrys", new[] { "Component_Id" });
            DropIndex("PersonActivityTypes", new[] { "ActivityTypeAltName" });
            DropIndex("PersonActivityTypes", new[] { "ActivityTypeName" });
            DropIndex("ProjectEvents", new[] { "ClosedByPerson_Id" });
            DropIndex("ProjectEvents", new[] { "CreatedByPerson_Id" });
            DropIndex("ProjectEvents", new[] { "AssignedToProject_Id" });
            DropIndex("ProjectEvents", new[] { "EventAltName" });
            DropIndex("ProjectEvents", new[] { "EventName" });
            DropIndex("PersonLogEntrys", new[] { "AssignedToProjectEvent_Id" });
            DropIndex("PersonLogEntrys", new[] { "PersonActivityType_Id" });
            DropIndex("PersonLogEntrys", new[] { "AssignedToLocation_Id" });
            DropIndex("PersonLogEntrys", new[] { "AssignedToProject_Id" });
            DropTable("PersonLogEntryPersons");
            DropTable("AssemblyLogEntrys");
            DropTable("ComponentLogEntrys");
            DropTable("PersonActivityTypes");
            DropTable("ProjectEvents");
            DropTable("PersonLogEntrys");
        }
    }
}
