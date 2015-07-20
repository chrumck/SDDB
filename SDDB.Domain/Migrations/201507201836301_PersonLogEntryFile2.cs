namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonLogEntryFile2 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {
            DropColumn("PersonLogEntryFiles", "IsActive_bl");
        }
        
        public override void Down()
        {
            AddColumn("PersonLogEntryFiles", "IsActive_bl", c => c.Boolean(nullable: false));
        }
    }
}
