namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssyModel1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            AddColumn("AssemblyModels", "Attr01Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr02Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr03Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr04Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr05Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr06Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr07Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr08Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr09Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr10Desc", c => c.String(maxLength: 64, storeType: "nvarchar"));
            DropColumn("AssemblyModels", "Attr01Unit");
            DropColumn("AssemblyModels", "Attr02Unit");
            DropColumn("AssemblyModels", "Attr03Unit");
            DropColumn("AssemblyModels", "Attr04Unit");
            DropColumn("AssemblyModels", "Attr05Unit");
            DropColumn("AssemblyModels", "Attr06Unit");
            DropColumn("AssemblyModels", "Attr07Unit");
            DropColumn("AssemblyModels", "Attr08Unit");
            DropColumn("AssemblyModels", "Attr09Unit");
            DropColumn("AssemblyModels", "Attr10Unit");
        }
        
        public override void Down()
        {
            AddColumn("AssemblyModels", "Attr10Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr09Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr08Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr07Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr06Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr05Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr04Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr03Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr02Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AddColumn("AssemblyModels", "Attr01Unit", c => c.String(maxLength: 64, storeType: "nvarchar"));
            DropColumn("AssemblyModels", "Attr10Desc");
            DropColumn("AssemblyModels", "Attr09Desc");
            DropColumn("AssemblyModels", "Attr08Desc");
            DropColumn("AssemblyModels", "Attr07Desc");
            DropColumn("AssemblyModels", "Attr06Desc");
            DropColumn("AssemblyModels", "Attr05Desc");
            DropColumn("AssemblyModels", "Attr04Desc");
            DropColumn("AssemblyModels", "Attr03Desc");
            DropColumn("AssemblyModels", "Attr02Desc");
            DropColumn("AssemblyModels", "Attr01Desc");
        }
    }
}
