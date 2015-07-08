namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class person12 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            AddColumn("Persons", "IsActive_bl", c => c.Boolean(nullable: false));
            AddColumn("Persons", "IsCurrentEmployee_bl", c => c.Boolean(nullable: false));
            AddColumn("Persons", "IsSalaried_bl", c => c.Boolean(nullable: false));
            DropColumn("Persons", "IsActive");
            DropColumn("Persons", "IsCurrentEmployee");
            DropColumn("Persons", "IsSalaried");

        }
        
        public override void Down()
        {
            AddColumn("Persons", "IsSalaried", c => c.Boolean(nullable: false));
            AddColumn("Persons", "IsCurrentEmployee", c => c.Boolean(nullable: false));
            AddColumn("Persons", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("Persons", "IsSalaried_bl");
            DropColumn("Persons", "IsCurrentEmployee_bl");
            DropColumn("Persons", "IsActive_bl");
        }
    }
}
