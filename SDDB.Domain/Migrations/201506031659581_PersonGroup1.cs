namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonGroup1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            RenameTable(name: "PersonsGroupPersons", newName: "PersonGroupPersons");
            RenameTable(name: "PersonsGroupPerson1", newName: "PersonGroupPerson1");
            RenameColumn(table: "PersonGroupPersons", name: "PersonsGroup_Id", newName: "PersonGroup_Id");
            RenameColumn(table: "PersonGroupPerson1", name: "PersonsGroup_Id", newName: "PersonGroup_Id");
            //RenameIndex(table: "PersonGroupPersons", name: "IX_PersonsGroup_Id", newName: "IX_PersonGroup_Id");
            //RenameIndex(table: "PersonGroupPerson1", name: "IX_PersonsGroup_Id", newName: "IX_PersonGroup_Id");
            Sql(@"ALTER TABLE `SDDB102`.`PersonGroupPersons` DROP INDEX `IX_PersonsGroup_Id` , ADD INDEX `IX_PersonGroup_Id` USING HASH (`PersonGroup_Id` ASC);");
            Sql(@"ALTER TABLE `SDDB102`.`PersonGroupPerson1` DROP INDEX `IX_PersonsGroup_Id` , ADD INDEX `IX_PersonGroup_Id` USING HASH (`PersonGroup_Id` ASC);");

        }
        
        public override void Down()
        {
            Sql(@"ALTER TABLE `SDDB102`.`PersonGroupPersons` DROP INDEX `IX_PersonGroup_Id` , ADD INDEX `IX_PersonsGroup_Id` USING HASH (`PersonGroup_Id` ASC);");
            Sql(@"ALTER TABLE `SDDB102`.`PersonGroupPerson1` DROP INDEX `IX_PersonGroup_Id` , ADD INDEX `IX_PersonsGroup_Id` USING HASH (`PersonGroup_Id` ASC);");
            //RenameIndex(table: "PersonGroupPerson1", name: "IX_PersonGroup_Id", newName: "IX_PersonsGroup_Id");
            //RenameIndex(table: "PersonGroupPersons", name: "IX_PersonGroup_Id", newName: "IX_PersonsGroup_Id");
            RenameColumn(table: "PersonGroupPerson1", name: "PersonGroup_Id", newName: "PersonsGroup_Id");
            RenameColumn(table: "PersonGroupPersons", name: "PersonGroup_Id", newName: "PersonsGroup_Id");
            RenameTable(name: "PersonGroupPerson1", newName: "PersonsGroupPerson1");
            RenameTable(name: "PersonGroupPersons", newName: "PersonsGroupPersons");
        }
    }
}
