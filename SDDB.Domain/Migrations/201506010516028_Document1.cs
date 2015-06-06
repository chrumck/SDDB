namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Document1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            RenameColumn(table: "Documents", name: "BelongsToProject_Id", newName: "AssignedToProject_Id");
            //RenameIndex(table: "Documents", name: "IX_BelongsToProject_Id", newName: "IX_AssignedToProject_Id");
            Sql(@"ALTER TABLE `SDDB102`.`Documents` DROP INDEX `IX_BelongsToProject_Id` , ADD INDEX `IX_AssignedToProject_Id` USING HASH (`AssignedToProject_Id` ASC);");
        }
        
        public override void Down()
        {
            RenameColumn(table: "Documents", name: "AssignedToProject_Id", newName: "BelongsToProject_Id");
            //RenameIndex(table: "Documents", name: "IX_AssignedToProject_Id", newName: "IX_BelongsToProject_Id");
            Sql(@"ALTER TABLE `SDDB102`.`Documents` DROP INDEX `IX_AssignedToProject_Id` , ADD INDEX `IX_BelongsToProject_Id` USING HASH (`BelongsToProject_Id` ASC);");
        }
    }
}
