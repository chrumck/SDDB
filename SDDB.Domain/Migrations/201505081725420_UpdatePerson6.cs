namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePerson6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persons", "IsCurrent", c => c.Boolean());
            DropColumn("dbo.Persons", "IsEmployee");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persons", "IsEmployee", c => c.Boolean(nullable: false));
            DropColumn("dbo.Persons", "IsCurrent");
        }
    }
}
