namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePerson : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //unicode changed to true

            AddColumn("dbo.Persons", "Initials", c => c.String(nullable: false, maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "DirectLine", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "DirectLineExt", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "MobilePhone", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "Email", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "PersonComments", c => c.String(unicode: true, storeType: "text"));
            AddColumn("dbo.Persons", "Flags", c => c.Int());
            AddColumn("dbo.Persons", "EmploeePosition", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "EmployeeStart", c => c.DateTime(precision: 0));
            AddColumn("dbo.Persons", "EmployeeEnd", c => c.DateTime(precision: 0));
            AddColumn("dbo.Persons", "EmployeeDetails", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "Discriminator", c => c.String(nullable: false, maxLength: 128, storeType: "nvarchar"));
            CreateIndex("dbo.Persons", "Initials", unique: true);
            DropColumn("dbo.Persons", "PersonFlags");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persons", "PersonFlags", c => c.Int(nullable: false));
            DropIndex("dbo.Persons", new[] { "Initials" });
            DropColumn("dbo.Persons", "Discriminator");
            DropColumn("dbo.Persons", "EmployeeDetails");
            DropColumn("dbo.Persons", "EmployeeEnd");
            DropColumn("dbo.Persons", "EmployeeStart");
            DropColumn("dbo.Persons", "EmploeePosition");
            DropColumn("dbo.Persons", "Flags");
            DropColumn("dbo.Persons", "PersonComments");
            DropColumn("dbo.Persons", "Email");
            DropColumn("dbo.Persons", "MobilePhone");
            DropColumn("dbo.Persons", "DirectLineExt");
            DropColumn("dbo.Persons", "DirectLine");
            DropColumn("dbo.Persons", "Initials");
        }
    }
}
