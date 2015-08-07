namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogEntryLastSavedNotRequired : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {
            DropForeignKey("ComponentLogEntrys", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("AssemblyLogEntrys", "LastSavedByPerson_Id", "Persons");
            DropIndex("ComponentLogEntrys", new[] { "LastSavedByPerson_Id" });
            DropIndex("AssemblyLogEntrys", new[] { "LastSavedByPerson_Id" });
            AlterColumn("ComponentLogEntrys", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            AlterColumn("AssemblyLogEntrys", "LastSavedByPerson_Id", c => c.String(maxLength: 40, storeType: "nvarchar"));
            CreateIndex("ComponentLogEntrys", "LastSavedByPerson_Id");
            CreateIndex("AssemblyLogEntrys", "LastSavedByPerson_Id");
            AddForeignKey("ComponentLogEntrys", "LastSavedByPerson_Id", "Persons", "Id");
            AddForeignKey("AssemblyLogEntrys", "LastSavedByPerson_Id", "Persons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("AssemblyLogEntrys", "LastSavedByPerson_Id", "Persons");
            DropForeignKey("ComponentLogEntrys", "LastSavedByPerson_Id", "Persons");
            DropIndex("AssemblyLogEntrys", new[] { "LastSavedByPerson_Id" });
            DropIndex("ComponentLogEntrys", new[] { "LastSavedByPerson_Id" });
            AlterColumn("AssemblyLogEntrys", "LastSavedByPerson_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            AlterColumn("ComponentLogEntrys", "LastSavedByPerson_Id", c => c.String(nullable: false, maxLength: 40, storeType: "nvarchar"));
            CreateIndex("AssemblyLogEntrys", "LastSavedByPerson_Id");
            CreateIndex("ComponentLogEntrys", "LastSavedByPerson_Id");
            AddForeignKey("AssemblyLogEntrys", "LastSavedByPerson_Id", "Persons", "Id", cascadeDelete: true);
            AddForeignKey("ComponentLogEntrys", "LastSavedByPerson_Id", "Persons", "Id", cascadeDelete: true);
        }
    }
}
