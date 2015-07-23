namespace SDDB.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeToBl : DbMigration
    {
        public override void Up()
        {
            RenameColumn("AssemblyDbs", "IsReference", "IsReference_bl");
            RenameColumn("AssemblyDbs", "IsActive", "IsActive_bl");
            RenameColumn("Components", "CalibrationReqd", "CalibrationReqd_bl");
            RenameColumn("Components", "IsActive", "IsActive_bl");
            RenameColumn("Projects", "IsActive", "IsActive_bl");
            RenameColumn("Documents", "IsActive", "IsActive_bl");
            RenameColumn("AspNetUsers", "LDAPAuthenticated", "LDAPAuthenticated_bl");
            RenameColumn("AssemblyStatuss", "IsActive", "IsActive_bl");
            RenameColumn("Locations", "CertOfApprReqd", "CertOfApprReqd_bl");
            RenameColumn("Locations", "RightOfEntryReqd", "RightOfEntryReqd_bl");
            RenameColumn("Locations", "IsActive", "IsActive_bl");
            RenameColumn("LocationTypes", "IsActive", "IsActive_bl");
            RenameColumn("ComponentStatuss", "IsActive", "IsActive_bl");
            RenameColumn("ProjectEvents", "IsActive", "IsActive_bl");
            RenameColumn("PersonActivityTypes", "IsActive", "IsActive_bl");
            RenameColumn("PersonGroups", "IsActive", "IsActive_bl");
            RenameColumn("DocumentTypes", "IsActive", "IsActive_bl");
            RenameColumn("AssemblyTypes", "IsActive", "IsActive_bl");
            RenameColumn("ComponentTypes", "IsActive", "IsActive_bl");
            RenameColumn("ComponentModels", "IsActive", "IsActive_bl");
            RenameColumn("AssemblyModels", "IsActive", "IsActive_bl");
            //AddColumn("dbo.AssemblyDbs", "IsReference_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AssemblyDbs", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Components", "CalibrationReqd_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Components", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Projects", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Documents", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AspNetUsers", "LDAPAuthenticated_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AssemblyStatuss", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Locations", "CertOfApprReqd_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Locations", "RightOfEntryReqd_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Locations", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.LocationTypes", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.ComponentStatuss", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.ProjectEvents", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.PersonActivityTypes", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.PersonGroups", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.DocumentTypes", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AssemblyTypes", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.ComponentTypes", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.ComponentModels", "IsActive_bl", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AssemblyModels", "IsActive_bl", c => c.Boolean(nullable: false));
            //DropColumn("dbo.AssemblyDbs", "IsReference");
            //DropColumn("dbo.AssemblyDbs", "IsActive");
            //DropColumn("dbo.Components", "CalibrationReqd");
            //DropColumn("dbo.Components", "IsActive");
            //DropColumn("dbo.Projects", "IsActive");
            //DropColumn("dbo.Documents", "IsActive");
            //DropColumn("dbo.AspNetUsers", "LDAPAuthenticated");
            //DropColumn("dbo.AssemblyStatuss", "IsActive");
            //DropColumn("dbo.Locations", "CertOfApprReqd");
            //DropColumn("dbo.Locations", "RightOfEntryReqd");
            //DropColumn("dbo.Locations", "IsActive");
            //DropColumn("dbo.LocationTypes", "IsActive");
            //DropColumn("dbo.ComponentStatuss", "IsActive");
            //DropColumn("dbo.ProjectEvents", "IsActive");
            //DropColumn("dbo.PersonActivityTypes", "IsActive");
            //DropColumn("dbo.PersonGroups", "IsActive");
            //DropColumn("dbo.DocumentTypes", "IsActive");
            //DropColumn("dbo.AssemblyTypes", "IsActive");
            //DropColumn("dbo.ComponentTypes", "IsActive");
            //DropColumn("dbo.ComponentModels", "IsActive");
            //DropColumn("dbo.AssemblyModels", "IsActive");
        }
        
        public override void Down()
        {
            RenameColumn("AssemblyDbs", "IsReference_bl", "IsReference");
            RenameColumn("AssemblyDbs", "IsActive_bl", "IsActive");
            RenameColumn("Components", "CalibrationReqd_bl", "CalibrationReqd");
            RenameColumn("Components", "IsActive_bl", "IsActive");
            RenameColumn("Projects", "IsActive_bl", "IsActive");
            RenameColumn("Documents", "IsActive_bl", "IsActive");
            RenameColumn("AspNetUsers", "LDAPAuthenticated_bl", "LDAPAuthenticated");
            RenameColumn("AssemblyStatuss", "IsActive_bl", "IsActive");
            RenameColumn("Locations", "CertOfApprReqd_bl", "CertOfApprReqd");
            RenameColumn("Locations", "RightOfEntryReqd_bl", "RightOfEntryReqd");
            RenameColumn("Locations", "IsActive_bl", "IsActive");
            RenameColumn("LocationTypes", "IsActive_bl", "IsActive");
            RenameColumn("ComponentStatuss", "IsActive_bl", "IsActive");
            RenameColumn("ProjectEvents", "IsActive_bl", "IsActive");
            RenameColumn("PersonActivityTypes", "IsActive_bl", "IsActive");
            RenameColumn("PersonGroups", "IsActive_bl", "IsActive");
            RenameColumn("DocumentTypes", "IsActive_bl", "IsActive");
            RenameColumn("AssemblyTypes", "IsActive_bl", "IsActive");
            RenameColumn("ComponentTypes", "IsActive_bl", "IsActive");
            RenameColumn("ComponentModels", "IsActive_bl", "IsActive");
            RenameColumn("AssemblyModels", "IsActive_bl", "IsActive");
            //AddColumn("dbo.AssemblyModels", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.ComponentModels", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.ComponentTypes", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AssemblyTypes", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.DocumentTypes", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.PersonGroups", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.PersonActivityTypes", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.ProjectEvents", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.ComponentStatuss", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.LocationTypes", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Locations", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Locations", "RightOfEntryReqd", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Locations", "CertOfApprReqd", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AssemblyStatuss", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AspNetUsers", "LDAPAuthenticated", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Documents", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Projects", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Components", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.Components", "CalibrationReqd", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AssemblyDbs", "IsActive", c => c.Boolean(nullable: false));
            //AddColumn("dbo.AssemblyDbs", "IsReference", c => c.Boolean(nullable: false));
            //DropColumn("dbo.AssemblyModels", "IsActive_bl");
            //DropColumn("dbo.ComponentModels", "IsActive_bl");
            //DropColumn("dbo.ComponentTypes", "IsActive_bl");
            //DropColumn("dbo.AssemblyTypes", "IsActive_bl");
            //DropColumn("dbo.DocumentTypes", "IsActive_bl");
            //DropColumn("dbo.PersonGroups", "IsActive_bl");
            //DropColumn("dbo.PersonActivityTypes", "IsActive_bl");
            //DropColumn("dbo.ProjectEvents", "IsActive_bl");
            //DropColumn("dbo.ComponentStatuss", "IsActive_bl");
            //DropColumn("dbo.LocationTypes", "IsActive_bl");
            //DropColumn("dbo.Locations", "IsActive_bl");
            //DropColumn("dbo.Locations", "RightOfEntryReqd_bl");
            //DropColumn("dbo.Locations", "CertOfApprReqd_bl");
            //DropColumn("dbo.AssemblyStatuss", "IsActive_bl");
            //DropColumn("dbo.AspNetUsers", "LDAPAuthenticated_bl");
            //DropColumn("dbo.Documents", "IsActive_bl");
            //DropColumn("dbo.Projects", "IsActive_bl");
            //DropColumn("dbo.Components", "IsActive_bl");
            //DropColumn("dbo.Components", "CalibrationReqd_bl");
            //DropColumn("dbo.AssemblyDbs", "IsActive_bl");
            //DropColumn("dbo.AssemblyDbs", "IsReference_bl");
        }
    }
}
