namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelRemove : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            //DropForeignKey("Components", "ComponentModel_Id", "ComponentModels");
            //DropIndex("Components", new[] { "ComponentModel_Id" });
            //DropColumn("Components", "ComponentModel_Id");
            
            //DropForeignKey("AssemblyLogEntrys", "AssignedToProject_Id", "Projects");
            //DropIndex("AssemblyLogEntrys", new[] { "AssignedToProject_Id" });
            //DropColumn("AssemblyLogEntrys", "AssignedToProject_Id");
           
            DropForeignKey("AssemblyDbs", "AssemblyModel_Id", "AssemblyModels");
            DropForeignKey("AssemblyDbs", "AssignedToProject_Id", "Projects");
            DropIndex("AssemblyDbs", new[] { "AssemblyModel_Id" });
            DropIndex("AssemblyDbs", new[] { "AssignedToProject_Id" });
            DropColumn("AssemblyDbs", "AssemblyModel_Id");
            DropColumn("AssemblyDbs", "AssignedToProject_Id");
            
            DropForeignKey("AssemblyModels", "LastSavedByPerson_Id", "Persons");
            DropIndex("AssemblyModels", new[] { "AssyModelName" });
            DropIndex("AssemblyModels", new[] { "AssyModelAltName" });
            DropIndex("AssemblyModels", new[] { "LastSavedByPerson_Id" });
            DropTable("AssemblyModels");

            DropForeignKey("ComponentModels", "LastSavedByPerson_Id", "Persons");
            DropIndex("ComponentModels", new[] { "CompModelName" });
            DropIndex("ComponentModels", new[] { "CompModelAltName" });
            DropIndex("ComponentModels", new[] { "LastSavedByPerson_Id" });
            DropTable("ComponentModels");

            AddColumn("AssemblyTypes", "Attr01Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr01Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr02Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr02Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr03Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr03Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr04Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr04Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr05Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr05Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr06Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr06Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr07Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr07Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr08Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr08Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr09Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr09Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr10Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr10Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr11Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr11Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr12Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr12Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr13Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr13Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr14Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr14Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "Attr15Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyTypes", "Attr15Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr01Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr01Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr02Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr02Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr03Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr03Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr04Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr04Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr05Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr05Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr06Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr06Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr07Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr07Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr08Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr08Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr09Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr09Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr10Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr10Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr11Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr11Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr12Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr12Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr13Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr13Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr14Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr14Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "Attr15Type", c => c.Byte(nullable: false));
            AddColumn("ComponentTypes", "Attr15Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            CreateTable(
                "AssemblyModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssyModelName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        AssyModelAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr01Type = c.Byte(nullable: false),
                        Attr01Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr02Type = c.Byte(nullable: false),
                        Attr02Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr03Type = c.Byte(nullable: false),
                        Attr03Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr04Type = c.Byte(nullable: false),
                        Attr04Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr05Type = c.Byte(nullable: false),
                        Attr05Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr06Type = c.Byte(nullable: false),
                        Attr06Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr07Type = c.Byte(nullable: false),
                        Attr07Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr08Type = c.Byte(nullable: false),
                        Attr08Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr09Type = c.Byte(nullable: false),
                        Attr09Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr10Type = c.Byte(nullable: false),
                        Attr10Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr11Type = c.Byte(nullable: false),
                        Attr11Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr12Type = c.Byte(nullable: false),
                        Attr12Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr13Type = c.Byte(nullable: false),
                        Attr13Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr14Type = c.Byte(nullable: false),
                        Attr14Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr15Type = c.Byte(nullable: false),
                        Attr15Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Comments = c.String(unicode: false, storeType: "text"),
                        IsActive_bl = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                        LastSavedByPerson_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "ComponentModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        CompModelName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        CompModelAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr01Type = c.Byte(nullable: false),
                        Attr01Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr02Type = c.Byte(nullable: false),
                        Attr02Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr03Type = c.Byte(nullable: false),
                        Attr03Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr04Type = c.Byte(nullable: false),
                        Attr04Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr05Type = c.Byte(nullable: false),
                        Attr05Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr06Type = c.Byte(nullable: false),
                        Attr06Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr07Type = c.Byte(nullable: false),
                        Attr07Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr08Type = c.Byte(nullable: false),
                        Attr08Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr09Type = c.Byte(nullable: false),
                        Attr09Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr10Type = c.Byte(nullable: false),
                        Attr10Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr11Type = c.Byte(nullable: false),
                        Attr11Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr12Type = c.Byte(nullable: false),
                        Attr12Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr13Type = c.Byte(nullable: false),
                        Attr13Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr14Type = c.Byte(nullable: false),
                        Attr14Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr15Type = c.Byte(nullable: false),
                        Attr15Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Comments = c.String(unicode: false, storeType: "text"),
                        IsActive_bl = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                        LastSavedByPerson_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("AssemblyLogEntrys", "AssignedToProject_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            AddColumn("Components", "ComponentModel_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("AssemblyDbs", "AssignedToProject_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            AddColumn("AssemblyDbs", "AssemblyModel_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            DropColumn("ComponentTypes", "Attr15Desc");
            DropColumn("ComponentTypes", "Attr15Type");
            DropColumn("ComponentTypes", "Attr14Desc");
            DropColumn("ComponentTypes", "Attr14Type");
            DropColumn("ComponentTypes", "Attr13Desc");
            DropColumn("ComponentTypes", "Attr13Type");
            DropColumn("ComponentTypes", "Attr12Desc");
            DropColumn("ComponentTypes", "Attr12Type");
            DropColumn("ComponentTypes", "Attr11Desc");
            DropColumn("ComponentTypes", "Attr11Type");
            DropColumn("ComponentTypes", "Attr10Desc");
            DropColumn("ComponentTypes", "Attr10Type");
            DropColumn("ComponentTypes", "Attr09Desc");
            DropColumn("ComponentTypes", "Attr09Type");
            DropColumn("ComponentTypes", "Attr08Desc");
            DropColumn("ComponentTypes", "Attr08Type");
            DropColumn("ComponentTypes", "Attr07Desc");
            DropColumn("ComponentTypes", "Attr07Type");
            DropColumn("ComponentTypes", "Attr06Desc");
            DropColumn("ComponentTypes", "Attr06Type");
            DropColumn("ComponentTypes", "Attr05Desc");
            DropColumn("ComponentTypes", "Attr05Type");
            DropColumn("ComponentTypes", "Attr04Desc");
            DropColumn("ComponentTypes", "Attr04Type");
            DropColumn("ComponentTypes", "Attr03Desc");
            DropColumn("ComponentTypes", "Attr03Type");
            DropColumn("ComponentTypes", "Attr02Desc");
            DropColumn("ComponentTypes", "Attr02Type");
            DropColumn("ComponentTypes", "Attr01Desc");
            DropColumn("ComponentTypes", "Attr01Type");
            DropColumn("AssemblyTypes", "Attr15Desc");
            DropColumn("AssemblyTypes", "Attr15Type");
            DropColumn("AssemblyTypes", "Attr14Desc");
            DropColumn("AssemblyTypes", "Attr14Type");
            DropColumn("AssemblyTypes", "Attr13Desc");
            DropColumn("AssemblyTypes", "Attr13Type");
            DropColumn("AssemblyTypes", "Attr12Desc");
            DropColumn("AssemblyTypes", "Attr12Type");
            DropColumn("AssemblyTypes", "Attr11Desc");
            DropColumn("AssemblyTypes", "Attr11Type");
            DropColumn("AssemblyTypes", "Attr10Desc");
            DropColumn("AssemblyTypes", "Attr10Type");
            DropColumn("AssemblyTypes", "Attr09Desc");
            DropColumn("AssemblyTypes", "Attr09Type");
            DropColumn("AssemblyTypes", "Attr08Desc");
            DropColumn("AssemblyTypes", "Attr08Type");
            DropColumn("AssemblyTypes", "Attr07Desc");
            DropColumn("AssemblyTypes", "Attr07Type");
            DropColumn("AssemblyTypes", "Attr06Desc");
            DropColumn("AssemblyTypes", "Attr06Type");
            DropColumn("AssemblyTypes", "Attr05Desc");
            DropColumn("AssemblyTypes", "Attr05Type");
            DropColumn("AssemblyTypes", "Attr04Desc");
            DropColumn("AssemblyTypes", "Attr04Type");
            DropColumn("AssemblyTypes", "Attr03Desc");
            DropColumn("AssemblyTypes", "Attr03Type");
            DropColumn("AssemblyTypes", "Attr02Desc");
            DropColumn("AssemblyTypes", "Attr02Type");
            DropColumn("AssemblyTypes", "Attr01Desc");
            DropColumn("AssemblyTypes", "Attr01Type");
            CreateIndex("AssemblyModels", "LastSavedByPerson_Id");
            CreateIndex("AssemblyModels", "AssyModelAltName", unique: true);
            CreateIndex("AssemblyModels", "AssyModelName", unique: true);
            CreateIndex("AssemblyLogEntrys", "AssignedToProject_Id");
            CreateIndex("ComponentModels", "LastSavedByPerson_Id");
            CreateIndex("ComponentModels", "CompModelAltName", unique: true);
            CreateIndex("ComponentModels", "CompModelName", unique: true);
            CreateIndex("Components", "ComponentModel_Id");
            CreateIndex("AssemblyDbs", "AssignedToProject_Id");
            CreateIndex("AssemblyDbs", "AssemblyModel_Id");
            AddForeignKey("AssemblyDbs", "AssignedToProject_Id", "Projects", "Id", cascadeDelete: true);
            AddForeignKey("AssemblyDbs", "AssemblyModel_Id", "AssemblyModels", "Id");
            AddForeignKey("AssemblyModels", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("AssemblyLogEntrys", "AssignedToProject_Id", "Projects", "Id", cascadeDelete: true);
            AddForeignKey("Components", "ComponentModel_Id", "ComponentModels", "Id");
            AddForeignKey("ComponentModels", "LastSavedByPerson_Id", "Persons", "Id");
        }
    }
}
