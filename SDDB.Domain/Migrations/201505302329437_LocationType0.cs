namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocationType0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "LocationTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        LocTypeName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        LocTypeAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.LocTypeName, unique: true)
                .Index(t => t.LocTypeAltName, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("LocationTypes", new[] { "LocTypeAltName" });
            DropIndex("LocationTypes", new[] { "LocTypeName" });
            DropTable("LocationTypes");
        }
    }
}
