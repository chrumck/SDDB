namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssyStatus0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "AssemblyStatuss",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssyStatusName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        AssyStatusAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AssyStatusName, unique: true)
                .Index(t => t.AssyStatusAltName, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("AssemblyStatuss", new[] { "AssyStatusAltName" });
            DropIndex("AssemblyStatuss", new[] { "AssyStatusName" });
            DropTable("AssemblyStatuss");
        }
    }
}
