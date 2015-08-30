using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("PersonLogEntrys")]
    public class PersonLogEntry : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }
                
        [Required(ErrorMessage = "Entry Date field is required")]
        [DBIsDateTimeISO] 
        public DateTime LogEntryDateTime { get; set; }

        [Required(ErrorMessage = "Entered By Person field is required")]
        [StringLength(40)]
        [ForeignKey("EnteredByPerson")]
        public string EnteredByPerson_Id { get; set; }

        [Required(ErrorMessage = "Activity Type field is required")]
        [StringLength(40)]
        [ForeignKey("PersonActivityType")]
        public string PersonActivityType_Id { get; set; }

        [Required(ErrorMessage = "Hours Worked field is required")]
        public decimal ManHours { get; set; }

        [Required(ErrorMessage = "Project field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToProject")]
        public string AssignedToProject_Id { get; set; }

        [Required(ErrorMessage = "Location field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToLocation")]
        public string AssignedToLocation_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("AssignedToProjectEvent")]
        public string AssignedToProjectEvent_Id { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string Comments { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }

        //TSP Column Wireup ----------------------------------------------------------------
        [Column(TypeName = "timestamp")]
        public DateTime TSP
        {
            get { return this.tsp.HasValue ? this.tsp.Value : DateTime.Now; }
            set { tsp = value; }
        }
        [NotMapped]
        private DateTime? tsp;

        //LastSavedByPerson_Id -------------------------------------------------------------
        //[Required(ErrorMessage = "LastSavedByPerson Id field is required")]
        [StringLength(40)]
        [ForeignKey("LastSavedByPerson")]
        public string LastSavedByPerson_Id { get; set; }
        //Navigation Property
        public virtual Person LastSavedByPerson { get; set; }

        //EF Navigation Properties---------------------------------------------------------------------------------------------//

        //one to one
        
        //one to many
        public virtual Person EnteredByPerson { get; set; }
        public virtual PersonActivityType PersonActivityType { get; set; }
        public virtual Project AssignedToProject { get; set; }
        public virtual Location AssignedToLocation { get; set; }
        public virtual ProjectEvent AssignedToProjectEvent { get; set; }

        [InverseProperty("AssignedToPersonLogEntry")]
        public virtual ICollection<PersonLogEntryFile> PersonLogEntryFiles { get; set; }

        //many to many

        [InverseProperty("PersonPrsLogEntrys")]
        public virtual ICollection<Person> PrsLogEntryPersons { get; set; }

        [InverseProperty("AssemblyDbPrsLogEntrys")]
        public virtual ICollection<AssemblyDb> PrsLogEntryAssemblyDbs { get; set; }
               
 
        //Non-persistent Properties--------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }
        

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntry()
        {
            this.PersonLogEntryFiles = new HashSet<PersonLogEntryFile>();
            this.PrsLogEntryPersons = new HashSet<Person>();
            this.PrsLogEntryAssemblyDbs = new HashSet<AssemblyDb>();
        }

    }
  
}
