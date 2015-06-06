namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class project4 : DbMigration
    {
        public override void Up()
        //changes by TDO:
        //deleted all 'dbo.'
        {
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
            
            AddColumn("Projects", "ProjectManager_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            CreateIndex("Projects", "ProjectManager_Id");
            AddForeignKey("Projects", "ProjectManager_Id", "Persons", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("PersonProjects", "Project_Id", "Projects");
            DropForeignKey("PersonProjects", "Person_Id", "Persons");
            DropForeignKey("Projects", "ProjectManager_Id", "Persons");
            DropIndex("PersonProjects", new[] { "Project_Id" });
            DropIndex("PersonProjects", new[] { "Person_Id" });
            DropIndex("Projects", new[] { "ProjectManager_Id" });
            DropColumn("Projects", "ProjectManager_Id");
            DropTable("PersonProjects");
        }
    }
}
