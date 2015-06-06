namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Location2 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            //AddColumn("Locations", "Address", c => c.String(maxLength: 255, storeType: "nvarchar"));
            //DropColumn("Locations", "Adress");
            RenameColumn(table: "Locations", name: "Adress", newName: "Address");
        }
        
        public override void Down()
        {
            //AddColumn("Locations", "Adress", c => c.String(maxLength: 255, storeType: "nvarchar"));
            //DropColumn("Locations", "Address");
            RenameColumn(table: "Locations", name: "Address", newName: "Adress");
        }
    }
}
