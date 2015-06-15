namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class person11 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {

            RenameColumn(table: "ComponentLogEntrys", name: "Person_Id", newName: "EnteredByPerson_Id");
            RenameColumn(table: "AssemblyLogEntrys", name: "Person_Id", newName: "EnteredByPerson_Id");
            //RenameIndex(table: "AssemblyLogEntrys", name: "IX_Person_Id", newName: "IX_EnteredByPerson_Id");
            Sql(@"ALTER TABLE `SDDB102`.`AssemblyLogEntrys` DROP INDEX `IX_Person_Id` , ADD INDEX `IX_EnteredByPerson_Id` USING HASH (`EnteredByPerson_Id` ASC);");
            //RenameIndex(table: "ComponentLogEntrys", name: "IX_Person_Id", newName: "IX_EnteredByPerson_Id");
            Sql(@"ALTER TABLE `SDDB102`.`ComponentLogEntrys` DROP INDEX `IX_Person_Id` , ADD INDEX `IX_EnteredByPerson_Id` USING HASH (`EnteredByPerson_Id` ASC);");
            AddColumn("PersonLogEntrys", "EnteredByPerson_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            CreateIndex("PersonLogEntrys", "EnteredByPerson_Id");
            AddForeignKey("PersonLogEntrys", "EnteredByPerson_Id", "Persons", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("PersonLogEntrys", "EnteredByPerson_Id", "Persons");
            DropIndex("PersonLogEntrys", new[] { "EnteredByPerson_Id" });
            DropColumn("PersonLogEntrys", "EnteredByPerson_Id");
            //RenameIndex(table: "ComponentLogEntrys", name: "IX_EnteredByPerson_Id", newName: "IX_Person_Id");
            Sql(@"ALTER TABLE `SDDB102`.`AssemblyLogEntrys` DROP INDEX `IX_EnteredByPerson_Id` , ADD INDEX `IX_Person_Id` USING HASH (`EnteredByPerson_Id` ASC);");
            //RenameIndex(table: "AssemblyLogEntrys", name: "IX_EnteredByPerson_Id", newName: "IX_Person_Id");
            Sql(@"ALTER TABLE `SDDB102`.`ComponentLogEntrys` DROP INDEX `IX_EnteredByPerson_Id` , ADD INDEX `IX_Person_Id` USING HASH (`EnteredByPerson_Id` ASC);");
            RenameColumn(table: "AssemblyLogEntrys", name: "EnteredByPerson_Id", newName: "Person_Id");
            RenameColumn(table: "ComponentLogEntrys", name: "EnteredByPerson_Id", newName: "Person_Id");
        }
    }
}
