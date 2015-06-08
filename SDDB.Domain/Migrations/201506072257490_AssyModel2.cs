namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssyModel2 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            AddColumn("AssemblyExts", "Attr11", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("AssemblyExts", "Attr12", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("AssemblyExts", "Attr13", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("AssemblyExts", "Attr14", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("AssemblyExts", "Attr15", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr11Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyModels", "Attr11Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr12Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyModels", "Attr12Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr13Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyModels", "Attr13Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr14Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyModels", "Attr14Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr15Type", c => c.Byte(nullable: false));
            AddColumn("AssemblyModels", "Attr15Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("AssemblyModels", "Attr15Desc");
            DropColumn("AssemblyModels", "Attr15Type");
            DropColumn("AssemblyModels", "Attr14Desc");
            DropColumn("AssemblyModels", "Attr14Type");
            DropColumn("AssemblyModels", "Attr13Desc");
            DropColumn("AssemblyModels", "Attr13Type");
            DropColumn("AssemblyModels", "Attr12Desc");
            DropColumn("AssemblyModels", "Attr12Type");
            DropColumn("AssemblyModels", "Attr11Desc");
            DropColumn("AssemblyModels", "Attr11Type");
            DropColumn("AssemblyExts", "Attr15");
            DropColumn("AssemblyExts", "Attr14");
            DropColumn("AssemblyExts", "Attr13");
            DropColumn("AssemblyExts", "Attr12");
            DropColumn("AssemblyExts", "Attr11");
        }
    }
}
