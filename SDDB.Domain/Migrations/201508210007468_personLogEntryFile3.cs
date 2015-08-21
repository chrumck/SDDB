namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class personLogEntryFile3 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {
            AlterColumn("PersonLogEntryFiles", "FileSize", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("PersonLogEntryFiles", "FileSize", c => c.Long(nullable: false));
        }
    }
}
