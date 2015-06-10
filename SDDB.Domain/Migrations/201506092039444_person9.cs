namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class person9 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //changed from 'dbo.' to ''

            AlterColumn("Persons", "EmployeeStart", c => c.String(maxLength: 64, storeType: "nvarchar"));
            AlterColumn("Persons", "EmployeeEnd", c => c.String(maxLength: 64, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("Persons", "EmployeeEnd", c => c.DateTime(precision: 0));
            AlterColumn("Persons", "EmployeeStart", c => c.DateTime(precision: 0));
        }
    }
}
