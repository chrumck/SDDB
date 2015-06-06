namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Location1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            RenameColumn(table: "Locations", name: "LocTypeId", newName: "LocationType_Id");
            //RenameIndex(table: "Locations", name: "IX_LocTypeId", newName: "IX_LocationType_Id");
            Sql(@"ALTER TABLE `SDDB102`.`Locations` DROP INDEX `IX_LocTypeId` , ADD INDEX `IX_LocationType_Id` USING HASH (`LocationType_Id` ASC);");
        }
        
        public override void Down()
        {
            Sql(@"ALTER TABLE `SDDB102`.`Locations` DROP INDEX `IX_LocationType_Id` , ADD INDEX `IX_LocTypeId` USING HASH (`LocationType_Id` ASC);");
            //RenameIndex(table: "Locations", name: "IX_LocationType_Id", newName: "IX_LocTypeId");
            RenameColumn(table: "Locations", name: "LocationType_Id", newName: "LocTypeId");
        }
    }
}
