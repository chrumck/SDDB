namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDBResult1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //unicode changed to true
            CreateTable(
                "dbo.DBResults",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        DtStart = c.DateTime(nullable: false, precision: 0),
                        DtEnd = c.DateTime(precision: 0),
                        ActionName = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
                        ControllerName = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
                        UserName = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
                        ServiceName = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
                        StatusCode = c.Int(nullable: false),
                        StatusDescription = c.String(unicode: true, storeType: "text"),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.DBServiceResults");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DBServiceResults",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Created = c.DateTime(nullable: false, precision: 0),
                        TriggeredByAction = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        TriggeredByController = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        TriggeredByUser = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        ServiceName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        ReturnedObjType = c.String(maxLength: 255, storeType: "nvarchar"),
                        StatusCode = c.Int(nullable: false),
                        StatusDescription = c.String(unicode: true, storeType: "text"),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.DBResults");
        }
    }
}
