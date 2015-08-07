namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastSavedByPerson_Id : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {
            RenameColumn(table: "AssemblyLogEntrys", name: "EnteredByPerson_Id", newName: "LastSavedByPerson_Id");
            RenameColumn(table: "ComponentLogEntrys", name: "EnteredByPerson_Id", newName: "LastSavedByPerson_Id");
            //RenameIndex(table: "ComponentLogEntrys", name: "IX_EnteredByPerson_Id", newName: "IX_LastSavedByPerson_Id");
            Sql(@"ALTER TABLE `SDDB102`.`ComponentLogEntrys` DROP INDEX `IX_EnteredByPerson_Id` , ADD INDEX `IX_LastSavedByPerson_Id` USING HASH (`LastSavedByPerson_Id` ASC);");
            //RenameIndex(table: "AssemblyLogEntrys", name: "IX_EnteredByPerson_Id", newName: "IX_LastSavedByPerson_Id");
            Sql(@"ALTER TABLE `SDDB102`.`AssemblyLogEntrys` DROP INDEX `IX_EnteredByPerson_Id` , ADD INDEX `IX_LastSavedByPerson_Id` USING HASH (`LastSavedByPerson_Id` ASC);");
            AddColumn("AssemblyDbs", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("Components", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("Projects", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("Documents", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("Persons", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("AssemblyStatuss", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("Locations", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("LocationTypes", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("ComponentStatuss", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("PersonLogEntrys", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("ProjectEvents", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("PersonActivityTypes", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("PersonLogEntryFiles", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("PersonGroups", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("DocumentTypes", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("AssemblyTypes", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("ComponentTypes", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("ComponentExts", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("ComponentModels", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("AssemblyExts", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            CreateIndex("AssemblyDbs", "LastSavedByPerson_Id");
            CreateIndex("Components", "LastSavedByPerson_Id");
            CreateIndex("Projects", "LastSavedByPerson_Id");
            CreateIndex("Persons", "LastSavedByPerson_Id");
            CreateIndex("Documents", "LastSavedByPerson_Id");
            CreateIndex("DocumentTypes", "LastSavedByPerson_Id");
            CreateIndex("AssemblyTypes", "LastSavedByPerson_Id");
            CreateIndex("ComponentTypes", "LastSavedByPerson_Id");
            CreateIndex("PersonLogEntrys", "LastSavedByPerson_Id");
            CreateIndex("Locations", "LastSavedByPerson_Id");
            CreateIndex("LocationTypes", "LastSavedByPerson_Id");
            CreateIndex("ProjectEvents", "LastSavedByPerson_Id");
            CreateIndex("PersonActivityTypes", "LastSavedByPerson_Id");
            CreateIndex("PersonLogEntryFiles", "LastSavedByPerson_Id");
            CreateIndex("PersonGroups", "LastSavedByPerson_Id");
            CreateIndex("ComponentExts", "LastSavedByPerson_Id");
            CreateIndex("ComponentStatuss", "LastSavedByPerson_Id");
            CreateIndex("ComponentModels", "LastSavedByPerson_Id");
            CreateIndex("AssemblyExts", "LastSavedByPerson_Id");
            CreateIndex("AssemblyStatuss", "LastSavedByPerson_Id");
            CreateIndex("AssemblyModels", "LastSavedByPerson_Id");
            AddForeignKey("DocumentTypes", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("Documents", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("AssemblyTypes", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("ComponentTypes", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("Locations", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("LocationTypes", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("ProjectEvents", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("PersonLogEntrys", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("PersonActivityTypes", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("PersonLogEntryFiles", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("Persons", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("PersonGroups", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("Projects", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("ComponentExts", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("ComponentStatuss", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("ComponentModels", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("Components", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("AssemblyExts", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("AssemblyStatuss", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("AssemblyModels", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("AssemblyDbs", "LastSavedByPerson_Id", "Persons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("AssemblyDbs", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("AssemblyModels", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("AssemblyStatuss", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("AssemblyExts", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("Components", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("ComponentModels", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("ComponentStatuss", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("ComponentExts", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("Projects", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("PersonGroups", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("Persons", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("PersonLogEntryFiles", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("PersonActivityTypes", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("PersonLogEntrys", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("ProjectEvents", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("LocationTypes", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("Locations", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("ComponentTypes", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("AssemblyTypes", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("Documents", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("DocumentTypes", "LastSavedByPerson_Id", "Persons");
            DropIndex("AssemblyModels", new[] { "LastSavedByPerson_Id" });
            DropIndex("AssemblyStatuss", new[] { "LastSavedByPerson_Id" });
            DropIndex("AssemblyExts", new[] { "LastSavedByPerson_Id" });
            DropIndex("ComponentModels", new[] { "LastSavedByPerson_Id" });
            DropIndex("ComponentStatuss", new[] { "LastSavedByPerson_Id" });
            DropIndex("ComponentExts", new[] { "LastSavedByPerson_Id" });
            DropIndex("PersonGroups", new[] { "LastSavedByPerson_Id" });
            DropIndex("PersonLogEntryFiles", new[] { "LastSavedByPerson_Id" });
            DropIndex("PersonActivityTypes", new[] { "LastSavedByPerson_Id" });
            DropIndex("ProjectEvents", new[] { "LastSavedByPerson_Id" });
            DropIndex("LocationTypes", new[] { "LastSavedByPerson_Id" });
            DropIndex("Locations", new[] { "LastSavedByPerson_Id" });
            DropIndex("PersonLogEntrys", new[] { "LastSavedByPerson_Id" });
            DropIndex("ComponentTypes", new[] { "LastSavedByPerson_Id" });
            DropIndex("AssemblyTypes", new[] { "LastSavedByPerson_Id" });
            DropIndex("DocumentTypes", new[] { "LastSavedByPerson_Id" });
            DropIndex("Documents", new[] { "LastSavedByPerson_Id" });
            DropIndex("Persons", new[] { "LastSavedByPerson_Id" });
            DropIndex("Projects", new[] { "LastSavedByPerson_Id" });
            DropIndex("Components", new[] { "LastSavedByPerson_Id" });
            DropIndex("AssemblyDbs", new[] { "LastSavedByPerson_Id" });
            DropColumn("AssemblyModels", "LastSavedByPerson_Id");
            DropColumn("AssemblyExts", "LastSavedByPerson_Id");
            DropColumn("ComponentModels", "LastSavedByPerson_Id");
            DropColumn("ComponentExts", "LastSavedByPerson_Id");
            DropColumn("ComponentTypes", "LastSavedByPerson_Id");
            DropColumn("AssemblyTypes", "LastSavedByPerson_Id");
            DropColumn("DocumentTypes", "LastSavedByPerson_Id");
            DropColumn("PersonGroups", "LastSavedByPerson_Id");
            DropColumn("PersonLogEntryFiles", "LastSavedByPerson_Id");
            DropColumn("PersonActivityTypes", "LastSavedByPerson_Id");
            DropColumn("ProjectEvents", "LastSavedByPerson_Id");
            DropColumn("PersonLogEntrys", "LastSavedByPerson_Id");
            DropColumn("ComponentStatuss", "LastSavedByPerson_Id");
            DropColumn("LocationTypes", "LastSavedByPerson_Id");
            DropColumn("Locations", "LastSavedByPerson_Id");
            DropColumn("AssemblyStatuss", "LastSavedByPerson_Id");
            DropColumn("Persons", "LastSavedByPerson_Id");
            DropColumn("Documents", "LastSavedByPerson_Id");
            DropColumn("Projects", "LastSavedByPerson_Id");
            DropColumn("Components", "LastSavedByPerson_Id");
            DropColumn("AssemblyDbs", "LastSavedByPerson_Id");
            RenameIndex(table: "AssemblyLogEntrys", name: "IX_LastSavedByPerson_Id", newName: "IX_EnteredByPerson_Id");
            RenameIndex(table: "ComponentLogEntrys", name: "IX_LastSavedByPerson_Id", newName: "IX_EnteredByPerson_Id");
            RenameColumn(table: "ComponentLogEntrys", name: "LastSavedByPerson_Id", newName: "EnteredByPerson_Id");
            RenameColumn(table: "AssemblyLogEntrys", name: "LastSavedByPerson_Id", newName: "EnteredByPerson_Id");
        }
    }
}
