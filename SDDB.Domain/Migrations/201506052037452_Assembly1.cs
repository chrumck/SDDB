namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assembly1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            RenameTable(name: "Assemblys", newName: "AssemblyDbs");
        }
        
        public override void Down()
        {
            RenameTable(name: "AssemblyDbs", newName: "Assemblys");
        }
    }
}
