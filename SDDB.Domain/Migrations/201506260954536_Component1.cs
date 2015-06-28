namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Component1 : DbMigration
    {
        //changes by TDO:
        //changed from 'dbo.' to ''

        public override void Up()
        {
            AddColumn("Components", "LastCalibrationDate", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("Components", "LastCalibrationDate");
        }
    }
}
