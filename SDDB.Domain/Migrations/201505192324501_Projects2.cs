namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Projects2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "ProjectManager_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            CreateIndex("dbo.Projects", "ProjectManager_Id");
            AddForeignKey("dbo.Projects", "ProjectManager_Id", "dbo.Persons", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "ProjectManager_Id", "dbo.Persons");
            DropIndex("dbo.Projects", new[] { "ProjectManager_Id" });
            DropColumn("dbo.Projects", "ProjectManager_Id");
        }
    }
}
