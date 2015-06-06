namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyToPersonGroup0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            DropForeignKey("CompanyPersons", "Company_Id", "Companys");
            DropForeignKey("CompanyPersons", "Person_Id", "Persons");
            DropForeignKey("CompanyPerson1", "Company_Id", "Companys");
            DropForeignKey("CompanyPerson1", "Person_Id", "Persons");
            DropIndex("Companys", new[] { "CompanyName" });
            DropIndex("Companys", new[] { "CompanyAltName" });
            DropIndex("CompanyPersons", new[] { "Company_Id" });
            DropIndex("CompanyPersons", new[] { "Person_Id" });
            DropIndex("CompanyPerson1", new[] { "Company_Id" });
            DropIndex("CompanyPerson1", new[] { "Person_Id" });
            DropTable("Companys");
            DropTable("CompanyPersons");
            DropTable("CompanyPerson1");

            CreateTable(
                "PersonGroups",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        PrsGroupName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        PrsGroupAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.PrsGroupName, unique: true)
                .Index(t => t.PrsGroupAltName, unique: true);
            
            CreateTable(
                "PersonsGroupPersons",
                c => new
                    {
                        PersonsGroup_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.PersonsGroup_Id, t.Person_Id })
                .ForeignKey("PersonGroups", t => t.PersonsGroup_Id, cascadeDelete: true)
                .ForeignKey("Persons", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.PersonsGroup_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "PersonsGroupPerson1",
                c => new
                    {
                        PersonsGroup_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.PersonsGroup_Id, t.Person_Id })
                .ForeignKey("PersonGroups", t => t.PersonsGroup_Id, cascadeDelete: true)
                .ForeignKey("Persons", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.PersonsGroup_Id)
                .Index(t => t.Person_Id);
        }
        
        public override void Down()
        {
            CreateTable(
                "CompanyPerson1",
                c => new
                    {
                        Company_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.Company_Id, t.Person_Id });
            
            CreateTable(
                "CompanyPersons",
                c => new
                    {
                        Company_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Person_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.Company_Id, t.Person_Id });
            
            CreateTable(
                "Companys",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        CompanyName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        CompanyAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Address = c.String(maxLength: 255, storeType: "nvarchar"),
                        City = c.String(maxLength: 255, storeType: "nvarchar"),
                        ZIP = c.Int(),
                        State = c.String(maxLength: 255, storeType: "nvarchar"),
                        Country = c.String(maxLength: 255, storeType: "nvarchar"),
                        Phone = c.Int(),
                        Phone2 = c.Int(),
                        Fax = c.Int(),
                        Email = c.String(maxLength: 255, storeType: "nvarchar"),
                        Website = c.String(maxLength: 255, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("PersonsGroupPerson1", "Person_Id", "Persons");
            DropForeignKey("PersonsGroupPerson1", "PersonsGroup_Id", "PersonGroups");
            DropForeignKey("PersonsGroupPersons", "Person_Id", "Persons");
            DropForeignKey("PersonsGroupPersons", "PersonsGroup_Id", "PersonGroups");
            DropIndex("PersonsGroupPerson1", new[] { "Person_Id" });
            DropIndex("PersonsGroupPerson1", new[] { "PersonsGroup_Id" });
            DropIndex("PersonsGroupPersons", new[] { "Person_Id" });
            DropIndex("PersonsGroupPersons", new[] { "PersonsGroup_Id" });
            DropIndex("PersonGroups", new[] { "PrsGroupAltName" });
            DropIndex("PersonGroups", new[] { "PrsGroupName" });
            DropTable("PersonsGroupPerson1");
            DropTable("PersonsGroupPersons");
            DropTable("PersonGroups");
            CreateIndex("CompanyPerson1", "Person_Id");
            CreateIndex("CompanyPerson1", "Company_Id");
            CreateIndex("CompanyPersons", "Person_Id");
            CreateIndex("CompanyPersons", "Company_Id");
            CreateIndex("Companys", "CompanyAltName", unique: true);
            CreateIndex("Companys", "CompanyName", unique: true);
            AddForeignKey("CompanyPerson1", "Person_Id", "Persons", "Id", cascadeDelete: true);
            AddForeignKey("CompanyPerson1", "Company_Id", "Companys", "Id", cascadeDelete: true);
            AddForeignKey("CompanyPersons", "Person_Id", "Persons", "Id", cascadeDelete: true);
            AddForeignKey("CompanyPersons", "Company_Id", "Companys", "Id", cascadeDelete: true);
        }
    }
}
