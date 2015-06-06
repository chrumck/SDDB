namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePerson2 : DbMigration
    {
        public override void Up()
        {

            //RenameColumn("dbo.Persons", "DirectLine", "Phone"); //doesnt work because there are two databases with the same table name
            //RenameColumn("dbo.Persons", "DirectLineExt", "PhoneExt"); //doesnt work because there are two databases with the same table name
            //RenameColumn("dbo.Persons", "MobilePhone", "PhoneMobile"); //doesnt work because there are two databases with the same table name
            AddColumn("dbo.Persons", "Phone", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "PhoneExt", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "PhoneMobile", c => c.String(maxLength: 255, storeType: "nvarchar"));
            DropColumn("dbo.Persons", "DirectLine");
            DropColumn("dbo.Persons", "DirectLineExt");
            DropColumn("dbo.Persons", "MobilePhone");
        }
        
        public override void Down()
        {
            //RenameColumn("dbo.Persons", "Phone", "DirectLine"); //doesnt work because there are two databases with the same table name
            //RenameColumn("dbo.Persons", "PhoneExt", "DirectLineExt"); //doesnt work because there are two databases with the same table name
            //RenameColumn("dbo.Persons", "PhoneMobile", "MobilePhone"); //doesnt work because there are two databases with the same table name
            AddColumn("dbo.Persons", "MobilePhone", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "DirectLineExt", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "DirectLine", c => c.String(maxLength: 255, storeType: "nvarchar"));
            DropColumn("dbo.Persons", "PhoneMobile");
            DropColumn("dbo.Persons", "PhoneExt");
            DropColumn("dbo.Persons", "Phone");
        }
    }
}
