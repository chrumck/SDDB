namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDBResult3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DBResults", "UserHostAddress", c => c.String(nullable: false, maxLength: 255, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DBResults", "UserHostAddress");
        }
    }
}
