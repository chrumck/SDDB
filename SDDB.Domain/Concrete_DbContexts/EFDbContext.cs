using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.Migrations.History;
using System.Threading.Tasks;
using System.Web.Mvc;
using MySql.Data.Entity;

using SDDB.Domain.Entities;
using SDDB.Domain.Abstract;
using SDDB.Domain.Services;
using System;


namespace SDDB.Domain.DbContexts
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class EFDbContext : IdentityDbContext<DBUser>
    {
        //Fields and Properties---------------------------------------------------

        public virtual DbSet<DBResult> DBResults { get; set; }
        
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PersonGroup> PersonGroups { get; set; }
        public virtual DbSet<PersonActivityType> PersonActivityTypes { get; set; }
        public virtual DbSet<PersonLogEntry> PersonLogEntrys { get; set; }
        public virtual DbSet<PersonLogEntryFile> PersonLogEntryFiles { get; set;}
        public virtual DbSet<PersonLogEntryFileData> PersonLogEntryFileDatas { get; set; }
        
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectEvent> ProjectEvents { get; set; }
        
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes { get; set; }

        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<LocationType> LocationTypes { get; set; }

        public virtual DbSet<AssemblyDb> AssemblyDbs { get; set; }
        public virtual DbSet<AssemblyExt> AssemblyExts { get; set; }
        public virtual DbSet<AssemblyType> AssemblyTypes { get; set; }
        public virtual DbSet<AssemblyModel> AssemblyModels { get; set; }
        public virtual DbSet<AssemblyStatus> AssemblyStatuss { get; set; }
        public virtual DbSet<AssemblyLogEntry> AssemblyLogEntrys { get; set; }

        public virtual DbSet<Component> Components { get; set; }
        public virtual DbSet<ComponentExt> ComponentExts { get; set; }
        public virtual DbSet<ComponentType> ComponentTypes { get; set; }
        public virtual DbSet<ComponentModel> ComponentModels { get; set; }
        public virtual DbSet<ComponentStatus> ComponentStatuss { get; set; }
        public virtual DbSet<ComponentLogEntry> ComponentLogEntrys { get; set; }

        //Constructors-------------------------------------------------------------

        public EFDbContext() : base("DbContextConn") 
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        //Methods------------------------------------------------------------------

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // fluent api config goes here

            base.OnModelCreating(modelBuilder);
        }
    }
}
