namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Document0 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''
            //changed unicode to true

            CreateTable(
                "AssemblyTypes",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    AssyTypeName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                    AssyTypeAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                    Comments = c.String(unicode: true, storeType: "text"),
                    IsActive = c.Boolean(nullable: false),
                    TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AssyTypeName, unique: true)
                .Index(t => t.AssyTypeAltName, unique: true);

            CreateTable(
                "ComponentTypes",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    CompTypeName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                    CompTypeAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                    Comments = c.String(unicode: true, storeType: "text"),
                    IsActive = c.Boolean(nullable: false),
                    TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CompTypeName, unique: true)
                .Index(t => t.CompTypeAltName, unique: true);

            CreateTable(
                "Documents",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    DocName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                    DocAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                    DocumentType_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    DocLastVersion = c.String(maxLength: 255, storeType: "nvarchar"),
                    AuthorPerson_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                    ReviewerPerson_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                    BelongsToProject_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    RelatesToAssyType_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                    RelatesToCompType_Id = c.String(maxLength: 40, storeType: "nvarchar"),
                    DocFilePath = c.String(maxLength: 255, storeType: "nvarchar"),
                    Comments = c.String(unicode: true, storeType: "text"),
                    IsActive = c.Boolean(nullable: false),
                    TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Persons", t => t.AuthorPerson_Id)
                .ForeignKey("Persons", t => t.ReviewerPerson_Id)
                .ForeignKey("Projects", t => t.BelongsToProject_Id, cascadeDelete: true)
                .ForeignKey("DocumentTypes", t => t.DocumentType_Id, cascadeDelete: true)
                .ForeignKey("AssemblyTypes", t => t.RelatesToAssyType_Id)
                .ForeignKey("ComponentTypes", t => t.RelatesToCompType_Id)
                .Index(t => t.DocName, unique: true)
                .Index(t => t.DocAltName, unique: true)
                .Index(t => t.DocumentType_Id)
                .Index(t => t.AuthorPerson_Id)
                .Index(t => t.ReviewerPerson_Id)
                .Index(t => t.BelongsToProject_Id)
                .Index(t => t.RelatesToAssyType_Id)
                .Index(t => t.RelatesToCompType_Id);

            CreateTable(
                "DocumentTypes",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                    DocTypeName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                    DocTypeAltName = c.String(maxLength: 255, storeType: "nvarchar"),
                    Comments = c.String(unicode: true, storeType: "text"),
                    IsActive = c.Boolean(nullable: false),
                    TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.DocTypeName, unique: true)
                .Index(t => t.DocTypeAltName, unique: true);

        }

        public override void Down()
        {
            DropForeignKey("Documents", "RelatesToCompType_Id", "ComponentTypes");
            DropForeignKey("Documents", "RelatesToAssyType_Id", "AssemblyTypes");
            DropForeignKey("Documents", "DocumentType_Id", "DocumentTypes");
            DropForeignKey("Documents", "BelongsToProject_Id", "Projects");
            DropForeignKey("Documents", "ReviewerPerson_Id", "Persons");
            DropForeignKey("Documents", "AuthorPerson_Id", "Persons");
            DropIndex("DocumentTypes", new[] { "DocTypeAltName" });
            DropIndex("DocumentTypes", new[] { "DocTypeName" });
            DropIndex("Documents", new[] { "RelatesToCompType_Id" });
            DropIndex("Documents", new[] { "RelatesToAssyType_Id" });
            DropIndex("Documents", new[] { "BelongsToProject_Id" });
            DropIndex("Documents", new[] { "ReviewerPerson_Id" });
            DropIndex("Documents", new[] { "AuthorPerson_Id" });
            DropIndex("Documents", new[] { "DocumentType_Id" });
            DropIndex("Documents", new[] { "DocAltName" });
            DropIndex("Documents", new[] { "DocName" });
            DropIndex("ComponentTypes", new[] { "CompTypeAltName" });
            DropIndex("ComponentTypes", new[] { "CompTypeName" });
            DropIndex("AssemblyTypes", new[] { "AssyTypeAltName" });
            DropIndex("AssemblyTypes", new[] { "AssyTypeName" });
            DropTable("DocumentTypes");
            DropTable("Documents");
            DropTable("ComponentTypes");
            DropTable("AssemblyTypes");
        }
    }
}
