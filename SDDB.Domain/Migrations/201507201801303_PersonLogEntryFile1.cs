namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonLogEntryFile1 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {
            AddColumn("PersonLogEntryFiles", "FileSize", c => c.Long(nullable: false));
            AddColumn("PersonLogEntryFileDatas", "ChunkNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("PersonLogEntryFileDatas", "ChunkNumber");
            DropColumn("PersonLogEntryFiles", "FileSize");
        }
    }
}
