namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssyModel0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "AssemblyModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        AssyModelName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        AssyModelAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr01Type = c.Byte(nullable: false),
                        Attr01Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr02Type = c.Byte(nullable: false),
                        Attr02Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr03Type = c.Byte(nullable: false),
                        Attr03Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr04Type = c.Byte(nullable: false),
                        Attr04Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr05Type = c.Byte(nullable: false),
                        Attr05Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr06Type = c.Byte(nullable: false),
                        Attr06Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr07Type = c.Byte(nullable: false),
                        Attr07Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr08Type = c.Byte(nullable: false),
                        Attr08Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr09Type = c.Byte(nullable: false),
                        Attr09Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr10Type = c.Byte(nullable: false),
                        Attr10Unit = c.String(maxLength: 64, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AssyModelName, unique: true)
                .Index(t => t.AssyModelAltName, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("AssemblyModels", new[] { "AssyModelAltName" });
            DropIndex("AssemblyModels", new[] { "AssyModelName" });
            DropTable("AssemblyModels");
        }
    }
}
