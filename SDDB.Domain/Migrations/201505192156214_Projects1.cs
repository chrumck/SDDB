namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Projects1 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //deleted all 'dbo.'

            DropForeignKey("DBUserProjects", "DBUser_Id", "AspNetUsers");
            DropForeignKey("DBUserProjects", "Project_Id", "Projects");
            DropIndex("Projects", new[] { "ProjectCode" });
            DropIndex("DBUserProjects", new[] { "DBUser_Id" });
            DropIndex("DBUserProjects", new[] { "Project_Id" });
            CreateTable(
                "PersonProjects",
                c => new
                    {
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Project_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Project_Id })
                .ForeignKey("Persons", t => t.Person_Id, cascadeDelete: true)
                .ForeignKey("Projects", t => t.Project_Id, cascadeDelete: true)
                .Index(t => t.Person_Id)
                .Index(t => t.Project_Id);
            
            AlterColumn("Projects", "ProjectCode", c => c.String(nullable: false, maxLength: 255, storeType: "nvarchar"));
            CreateIndex("Projects", "ProjectCode", unique: true);
            DropTable("DBUserProjects");
        }
        
        public override void Down()
        {
            CreateTable(
                "DBUserProjects",
                c => new
                    {
                        DBUser_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Project_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.DBUser_Id, t.Project_Id });
            
            DropForeignKey("PersonProjects", "Project_Id", "Projects");
            DropForeignKey("PersonProjects", "Person_Id", "Persons");
            DropIndex("PersonProjects", new[] { "Project_Id" });
            DropIndex("PersonProjects", new[] { "Person_Id" });
            DropIndex("Projects", new[] { "ProjectCode" });
            AlterColumn("Projects", "ProjectCode", c => c.String(maxLength: 255, storeType: "nvarchar"));
            DropTable("PersonProjects");
            CreateIndex("DBUserProjects", "Project_Id");
            CreateIndex("DBUserProjects", "DBUser_Id");
            CreateIndex("Projects", "ProjectCode", unique: true);
            AddForeignKey("DBUserProjects", "Project_Id", "Projects", "Id", cascadeDelete: true);
            AddForeignKey("DBUserProjects", "DBUser_Id", "AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
