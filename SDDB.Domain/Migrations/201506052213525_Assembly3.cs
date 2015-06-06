namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assembly3 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            AddColumn("AssemblyDbs", "AssyStationing", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("AssemblyDbs", "LocStationing");
        }
        
        public override void Down()
        {
            AddColumn("AssemblyDbs", "LocStationing", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("AssemblyDbs", "AssyStationing");
        }
    }
}
