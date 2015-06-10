namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComponentModel0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "ComponentModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        CompModelName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        CompModelAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Attr01Type = c.Byte(nullable: false),
                        Attr01Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr02Type = c.Byte(nullable: false),
                        Attr02Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr03Type = c.Byte(nullable: false),
                        Attr03Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr04Type = c.Byte(nullable: false),
                        Attr04Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr05Type = c.Byte(nullable: false),
                        Attr05Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr06Type = c.Byte(nullable: false),
                        Attr06Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr07Type = c.Byte(nullable: false),
                        Attr07Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr08Type = c.Byte(nullable: false),
                        Attr08Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr09Type = c.Byte(nullable: false),
                        Attr09Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr10Type = c.Byte(nullable: false),
                        Attr10Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr11Type = c.Byte(nullable: false),
                        Attr11Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr12Type = c.Byte(nullable: false),
                        Attr12Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr13Type = c.Byte(nullable: false),
                        Attr13Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr14Type = c.Byte(nullable: false),
                        Attr14Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Attr15Type = c.Byte(nullable: false),
                        Attr15Desc = c.String(maxLength: 64, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CompModelName, unique: true)
                .Index(t => t.CompModelAltName, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("ComponentModels", new[] { "CompModelAltName" });
            DropIndex("ComponentModels", new[] { "CompModelName" });
            DropTable("ComponentModels");
        }
    }
}
