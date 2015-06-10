namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Component0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "Components",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        CompName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        CompAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        CompAltName2 = c.String(maxLength: 255, storeType: "nvarchar"),
                        ComponentType_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        ComponentStatus_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        ComponentModel_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                        AssignedToProject_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssignedToAssemblyDb_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                        PositionInAssy = c.String(maxLength: 255, storeType: "nvarchar"),
                        ProgramAddress = c.String(maxLength: 255, storeType: "nvarchar"),
                        CalibrationReqd = c.Boolean(nullable: false),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AssemblyDbs", t => t.AssignedToAssemblyDb_Id)
                .ForeignKey("Projects", t => t.AssignedToProject_Id, cascadeDelete: true)
                .ForeignKey("ComponentModels", t => t.ComponentModel_Id)
                .ForeignKey("ComponentStatuss", t => t.ComponentStatus_Id, cascadeDelete: true)
                .ForeignKey("ComponentTypes", t => t.ComponentType_Id, cascadeDelete: true)
                .Index(t => t.CompName, unique: true)
                .Index(t => t.CompAltName, unique: true)
                .Index(t => t.ComponentType_Id)
                .Index(t => t.ComponentStatus_Id)
                .Index(t => t.ComponentModel_Id)
                .Index(t => t.AssignedToProject_Id)
                .Index(t => t.AssignedToAssemblyDb_Id);

            CreateTable(
                "ComponentExts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Attr01 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr02 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr03 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr04 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr05 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr06 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr07 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr08 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr09 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr10 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr11 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr12 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr13 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr14 = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr15 = c.String(maxLength: 255, storeType: "nvarchar"),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Components", t => t.Id)
                .Index(t => t.Id);
        }
        
        public override void Down()
        {
            DropForeignKey("ComponentExts", "Id", "Components");
            DropForeignKey("Components", "ComponentType_Id", "ComponentTypes");
            DropForeignKey("Components", "ComponentStatus_Id", "ComponentStatuss");
            DropForeignKey("Components", "ComponentModel_Id", "ComponentModels");
            DropForeignKey("Components", "AssignedToProject_Id", "Projects");
            DropForeignKey("Components", "AssignedToAssemblyDb_Id", "AssemblyDbs");
            DropIndex("Components", new[] { "AssignedToAssemblyDb_Id" });
            DropIndex("Components", new[] { "AssignedToProject_Id" });
            DropIndex("Components", new[] { "ComponentModel_Id" });
            DropIndex("Components", new[] { "ComponentStatus_Id" });
            DropIndex("Components", new[] { "ComponentType_Id" });
            DropIndex("Components", new[] { "CompAltName" });
            DropIndex("Components", new[] { "CompName" });
            DropIndex("ComponentExts", new[] { "Id" });
            DropTable("Components");
            DropTable("ComponentExts");
        }
    }
}
