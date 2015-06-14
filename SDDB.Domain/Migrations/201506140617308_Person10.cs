namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Person10 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {
            AlterColumn("Persons", "EmployeeStart", c => c.DateTime(precision: 0));
            AlterColumn("Persons", "EmployeeEnd", c => c.DateTime(precision: 0));
            AlterColumn("ComponentLogEntrys", "LogEntryDate", c => c.DateTime(nullable: false, precision: 0));
            AlterColumn("AssemblyLogEntrys", "LogEntryDate", c => c.DateTime(nullable: false, precision: 0));
        }
        
        public override void Down()
        {
            AlterColumn("AssemblyLogEntrys", "LogEntryDate", c => c.String(nullable: false, maxLength: 64, storeType: "nvarchar"));
            AlterColumn("ComponentLogEntrys", "LogEntryDate", c => c.String(nullable: false, maxLength: 64, storeType: "nvarchar"));
            AlterColumn("Persons", "EmployeeEnd", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AlterColumn("Persons", "EmployeeStart", c => c.String(maxLength: 64, storeType: "nvarchar"));
        }
    }
}
