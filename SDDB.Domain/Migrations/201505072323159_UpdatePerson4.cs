namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePerson4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persons", "IsEmployee", c => c.Boolean(nullable: false));
            AddColumn("dbo.Persons", "EmployeePosition", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "IsSalaried", c => c.Boolean());
            DropColumn("dbo.Persons", "Flags");
            DropColumn("dbo.Persons", "EmploeePosition");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persons", "EmploeePosition", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "Flags", c => c.Int());
            DropColumn("dbo.Persons", "IsSalaried");
            DropColumn("dbo.Persons", "EmployeePosition");
            DropColumn("dbo.Persons", "IsEmployee");
        }
    }
}
