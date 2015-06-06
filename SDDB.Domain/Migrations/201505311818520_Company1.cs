namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Company1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            AddColumn("Companys", "Address", c => c.String(maxLength: 255, storeType: "nvarchar"));
            DropColumn("Companys", "Adress");
        }
        
        public override void Down()
        {
            AddColumn("Companys", "Adress", c => c.String(maxLength: 255, storeType: "nvarchar"));
            DropColumn("Companys", "Address");
        }
    }
}
