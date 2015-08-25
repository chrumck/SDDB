namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class location5 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {
            AddColumn("Locations", "LocAltName2", c => c.String(maxLength: 255, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("Locations", "LocAltName2");
        }
    }
}
