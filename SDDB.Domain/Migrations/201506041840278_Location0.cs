namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Location0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "Locations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        LocName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        LocAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        LocTypeId = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssignedToProject_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        ContactPerson_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                        Adress = c.String(maxLength: 255, storeType: "nvarchar"),
                        City = c.String(maxLength: 128, storeType: "nvarchar"),
                        ZIP = c.String(maxLength: 64, storeType: "nvarchar"),
                        State = c.String(maxLength: 64, storeType: "nvarchar"),
                        Country = c.String(maxLength: 64, storeType: "nvarchar"),
                        LocX = c.Decimal(precision: 18, scale: 2),
                        LocY = c.Decimal(precision: 18, scale: 2),
                        LocZ = c.Decimal(precision: 18, scale: 2),
                        LocStationing = c.Decimal(precision: 18, scale: 2),
                        AccessInfo = c.String(unicode: true, storeType: "text"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Persons", t => t.ContactPerson_Id)
                .ForeignKey("LocationTypes", t => t.LocTypeId, cascadeDelete: true)
                .ForeignKey("Projects", t => t.AssignedToProject_Id, cascadeDelete: true)
                .Index(t => t.LocName, unique: true)
                .Index(t => t.LocAltName, unique: true)
                .Index(t => t.LocTypeId)
                .Index(t => t.AssignedToProject_Id)
                .Index(t => t.ContactPerson_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Locations", "AssignedToProject_Id", "Projects");
            DropForeignKey("Locations", "LocTypeId", "LocationTypes");
            DropForeignKey("Locations", "ContactPerson_Id", "Persons");
            DropIndex("Locations", new[] { "ContactPerson_Id" });
            DropIndex("Locations", new[] { "AssignedToProject_Id" });
            DropIndex("Locations", new[] { "LocTypeId" });
            DropIndex("Locations", new[] { "LocAltName" });
            DropIndex("Locations", new[] { "LocName" });
            DropTable("Locations");
        }
    }
}
