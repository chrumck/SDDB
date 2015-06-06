namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompStatus0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "ComponentStatuss",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        CompStatusName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        CompStatusAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CompStatusName, unique: true)
                .Index(t => t.CompStatusAltName, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("ComponentStatuss", new[] { "CompStatusAltName" });
            DropIndex("ComponentStatuss", new[] { "CompStatusName" });
            DropTable("ComponentStatuss");
        }
    }
}
