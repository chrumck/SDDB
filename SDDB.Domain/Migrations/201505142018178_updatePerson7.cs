namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePerson7 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //unicode changed to true

            AddColumn("dbo.Persons", "IsCurrentEmployee", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Persons", "EmployeeDetails", c => c.String(unicode: true, storeType: "text"));
            AlterColumn("dbo.Persons", "IsSalaried", c => c.Boolean(nullable: false));
            DropColumn("dbo.Persons", "IsCurrent");
            DropColumn("dbo.Persons", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persons", "Discriminator", c => c.String(nullable: false, maxLength: 128, storeType: "nvarchar"));
            AddColumn("dbo.Persons", "IsCurrent", c => c.Boolean());
            AlterColumn("dbo.Persons", "IsSalaried", c => c.Boolean());
            AlterColumn("dbo.Persons", "EmployeeDetails", c => c.String(maxLength: 255, storeType: "nvarchar"));
            DropColumn("dbo.Persons", "IsCurrentEmployee");
        }
    }
}
