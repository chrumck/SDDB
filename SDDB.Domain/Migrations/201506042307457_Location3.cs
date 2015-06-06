namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Location3 : DbMigration
    {
        public override void Up()
        {

            //changes by TDO:
            //changed from 'dbo.' to ''

            AddColumn("Locations", "CertOfApprReqd", c => c.Boolean(nullable: false));
            AddColumn("Locations", "RightOfEntryReqd", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Locations", "RightOfEntryReqd");
            DropColumn("Locations", "CertOfApprReqd");
        }
    }
}
