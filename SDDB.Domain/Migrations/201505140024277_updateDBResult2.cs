namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDBResult2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DBResults", "DtEnd", c => c.DateTime(nullable: false, precision: 0));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DBResults", "DtEnd", c => c.DateTime(precision: 0));
        }
    }
}
