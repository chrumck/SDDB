namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assembly0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "Assemblys",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssyName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        AssyAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        AssyAltName2 = c.String(maxLength: 255, storeType: "nvarchar"),
                        AssemblyType_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssemblyStatus_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssemblyModel_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                        AssignedToProject_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssyGlobalX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssyGlobalY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssyGlobalZ = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssyLocalXDesign = c.Decimal(precision: 18, scale: 2),
                        AssyLocalYDesign = c.Decimal(precision: 18, scale: 2),
                        AssyLocalZDesign = c.Decimal(precision: 18, scale: 2),
                        AssyLocalXAsBuilt = c.Decimal(precision: 18, scale: 2),
                        AssyLocalYAsBuilt = c.Decimal(precision: 18, scale: 2),
                        AssyLocalZAsBuilt = c.Decimal(precision: 18, scale: 2),
                        LocStationing = c.Decimal(precision: 18, scale: 2),
                        AssyLength = c.Decimal(precision: 18, scale: 2),
                        AssyReadingIntervalSecs = c.Decimal(precision: 18, scale: 2),
                        IsReference = c.Boolean(nullable: false),
                        AssyDateScheduled = c.DateTime(nullable: false, precision: 0),
                        AssyDateExecuted = c.DateTime(precision: 0),
                        TechnicalDetails = c.String(unicode: true, storeType: "text"),
                        PowerSupplyDetails = c.String(unicode: true, storeType: "text"),
                        HSEDetails = c.String(unicode: true, storeType: "text"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AssemblyModels", t => t.AssemblyModel_Id)
                .ForeignKey("AssemblyStatuss", t => t.AssemblyStatus_Id, cascadeDelete: true)
                .ForeignKey("AssemblyTypes", t => t.AssemblyType_Id, cascadeDelete: true)
                .ForeignKey("Projects", t => t.AssignedToProject_Id, cascadeDelete: true)
                .Index(t => t.AssyName, unique: true)
                .Index(t => t.AssyAltName, unique: true)
                .Index(t => t.AssemblyType_Id)
                .Index(t => t.AssemblyStatus_Id)
                .Index(t => t.AssemblyModel_Id)
                .Index(t => t.AssignedToProject_Id);

            CreateTable(
                "AssemblyExts",
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
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Assemblys", t => t.Id)
                .Index(t => t.Id);
           
        }
        
        public override void Down()
        {
            DropForeignKey("AssemblyExts", "Id", "Assemblys");
            DropForeignKey("Assemblys", "AssignedToProject_Id", "Projects");
            DropForeignKey("Assemblys", "AssemblyType_Id", "AssemblyTypes");
            DropForeignKey("Assemblys", "AssemblyStatus_Id", "AssemblyStatuss");
            DropForeignKey("Assemblys", "AssemblyModel_Id", "AssemblyModels");
            DropIndex("Assemblys", new[] { "AssignedToProject_Id" });
            DropIndex("Assemblys", new[] { "AssemblyModel_Id" });
            DropIndex("Assemblys", new[] { "AssemblyStatus_Id" });
            DropIndex("Assemblys", new[] { "AssemblyType_Id" });
            DropIndex("Assemblys", new[] { "AssyAltName" });
            DropIndex("Assemblys", new[] { "AssyName" });
            DropIndex("AssemblyExts", new[] { "Id" });
            DropTable("Assemblys");
            DropTable("AssemblyExts");
        }
    }
}
