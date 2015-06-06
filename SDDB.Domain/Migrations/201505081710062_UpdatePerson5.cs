namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePerson5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Persons", "PhoneExt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persons", "PhoneExt", c => c.String(maxLength: 255, storeType: "nvarchar"));
        }
    }
}
