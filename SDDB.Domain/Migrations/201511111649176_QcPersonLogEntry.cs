namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QcPersonLogEntry : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''
        public override void Up()
        {
            AddColumn("PersonLogEntrys", "QcdByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AddColumn("PersonLogEntrys", "QcdDateTime", c => c.DateTime(precision: 0));
            CreateIndex("PersonLogEntrys", "QcdByPerson_Id");
            AddForeignKey("PersonLogEntrys", "QcdByPerson_Id", "Persons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("PersonLogEntrys", "QcdByPerson_Id", "Persons");
            DropIndex("PersonLogEntrys", new[] { "QcdByPerson_Id" });
            DropColumn("PersonLogEntrys", "QcdDateTime");
            DropColumn("PersonLogEntrys", "QcdByPerson_Id");
        }
    }
}
