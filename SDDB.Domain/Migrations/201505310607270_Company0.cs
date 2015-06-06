namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Company0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "Companys",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        CompanyName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        CompanyAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                        Adress = c.String(maxLength: 255, storeType: "nvarchar"),
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
                .PrimaryKey(t => t.Id)
                .Index(t => t.CompanyName, unique: true)
                .Index(t => t.CompanyAltName, unique: true);
            
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
            DropIndex("Companys", new[] { "CompanyAltName" });
            DropIndex("Companys", new[] { "CompanyName" });
            DropTable("CompanyPersons");
            DropTable("Companys");
        }
    }
}
