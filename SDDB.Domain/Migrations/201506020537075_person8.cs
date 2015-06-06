namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class person8 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            RenameTable(name: "CompanyPersons", newName: "CompanyPerson1");
            CreateTable(
                "CompanyPersons",
                c => new
                    {
                        Company_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.Company_Id, t.Person_Id })
                .ForeignKey("Companys", t => t.Company_Id, cascadeDelete: true)
                .ForeignKey("Persons", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.Company_Id)
                .Index(t => t.Person_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("CompanyPersons", "Person_Id", "Persons");
            DropForeignKey("CompanyPersons", "Company_Id", "Companys");
            DropIndex("CompanyPersons", new[] { "Person_Id" });
            DropIndex("CompanyPersons", new[] { "Company_Id" });
            DropTable("CompanyPersons");
            RenameTable(name: "CompanyPerson1", newName: "CompanyPersons");
        }
    }
}
