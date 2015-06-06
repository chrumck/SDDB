namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class project3 : DbMigration
    {
        public override void Up()
        //changes by TDO:
        //deleted all 'dbo.'
        {
            DropForeignKey("Projects", "ProjectManager_Id", "Persons");
            DropForeignKey("PersonProjects", "Person_Id", "Persons");
            DropForeignKey("PersonProjects", "Project_Id", "Projects");
            DropIndex("Projects", new[] { "ProjectManager_Id" });
            DropIndex("PersonProjects", new[] { "Person_Id" });
            DropIndex("PersonProjects", new[] { "Project_Id" });
            DropColumn("Projects", "ProjectManager_Id");
            DropTable("PersonProjects");
        }
        
        public override void Down()
        {
            CreateTable(
                "PersonProjects",
                c => new
                    {
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Project_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Project_Id });
            
            AddColumn("Projects", "ProjectManager_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            CreateIndex("PersonProjects", "Project_Id");
            CreateIndex("PersonProjects", "Person_Id");
            CreateIndex("Projects", "ProjectManager_Id");
            AddForeignKey("PersonProjects", "Project_Id", "Projects", "Id", cascadeDelete: true);
            AddForeignKey("PersonProjects", "Person_Id", "Persons", "Id", cascadeDelete: true);
            AddForeignKey("Projects", "ProjectManager_Id", "Persons", "Id", cascadeDelete: true);
        }
    }
}
