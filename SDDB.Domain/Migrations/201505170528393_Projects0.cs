namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Projects0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //unicode changed to true

            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        ProjectName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        ProjectAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        ProjectCode = c.String(maxLength: 255, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProjectName, unique: true)
                .Index(t => t.ProjectAltName, unique: true)
                .Index(t => t.ProjectCode, unique: true);
            
            CreateTable(
                "dbo.DBUserProjects",
                c => new
                    {
                        DBUser_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Project_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.DBUser_Id, t.Project_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.DBUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .Index(t => t.DBUser_Id)
                .Index(t => t.Project_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DBUserProjects", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.DBUserProjects", "DBUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.DBUserProjects", new[] { "Project_Id" });
            DropIndex("dbo.DBUserProjects", new[] { "DBUser_Id" });
            DropIndex("dbo.Projects", new[] { "ProjectCode" });
            DropIndex("dbo.Projects", new[] { "ProjectAltName" });
            DropIndex("dbo.Projects", new[] { "ProjectName" });
            DropTable("dbo.DBUserProjects");
            DropTable("dbo.Projects");
        }
    }
}
