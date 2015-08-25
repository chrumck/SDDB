namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class location4 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {
            DropColumn("Locations", "City");
            DropColumn("Locations", "ZIP");
            DropColumn("Locations", "State");
            DropColumn("Locations", "Country");
        }
        
        public override void Down()
        {
            AddColumn("Locations", "Country", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("Locations", "State", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("Locations", "ZIP", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("Locations", "City", c => c.String(maxLength: 128, storeType: "nvarchar"));
        }
    }
}
