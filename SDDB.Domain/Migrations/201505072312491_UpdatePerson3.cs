namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePerson3 : DbMigration
    {
        public override void Up()
        {
            //changes by TDO:
            //unicode changed to true

            AddColumn("dbo.Persons", "Comments", c => c.String(unicode: true, storeType: "text"));
            DropColumn("dbo.Persons", "PersonComments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persons", "PersonComments", c => c.String(unicode: true, storeType: "text"));
            DropColumn("dbo.Persons", "Comments");
        }
    }
}
