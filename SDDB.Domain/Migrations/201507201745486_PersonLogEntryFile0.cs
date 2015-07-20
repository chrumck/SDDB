namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonLogEntryFile0 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        //changed unicode to true

        public override void Up()
        {
            CreateTable(
                "PersonLogEntryFiles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        FileName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        FileType = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        FileDateTime = c.DateTime(nullable: false, precision: 0),
                        AssignedToPersonLogEntry_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Comments = c.String(unicode: true, storeType: "text"),
                        IsActive_bl = c.Boolean(nullable: false),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("PersonLogEntrys", t => t.AssignedToPersonLogEntry_Id, cascadeDelete: true)
                .Index(t => t.AssignedToPersonLogEntry_Id);
            
            CreateTable(
                "PersonLogEntryFileDatas",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Data = c.Binary(nullable: false, storeType: "blob"),
                        PersonLogEntryFile_Id = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        TSP = c.DateTime(nullable: false, precision: 0, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("PersonLogEntryFiles", t => t.PersonLogEntryFile_Id, cascadeDelete: true)
                .Index(t => t.PersonLogEntryFile_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("PersonLogEntryFiles", "AssignedToPersonLogEntry_Id", "PersonLogEntrys");
            DropForeignKey("PersonLogEntryFileDatas", "PersonLogEntryFile_Id", "PersonLogEntryFiles");
            DropIndex("PersonLogEntryFileDatas", new[] { "PersonLogEntryFile_Id" });
            DropIndex("PersonLogEntryFiles", new[] { "AssignedToPersonLogEntry_Id" });
            DropTable("PersonLogEntryFileDatas");
            DropTable("PersonLogEntryFiles");
        }
    }
}
