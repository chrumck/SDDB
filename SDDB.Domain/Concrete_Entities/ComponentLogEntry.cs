using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("ComponentLogEntrys")]
    public class ComponentLogEntry : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Component field is required")]
        [StringLength(40)]
        [ForeignKey("Component")]
        public string Component_Id { get; set; }

        [Required(ErrorMessage = "Person field is required")]
        [StringLength(40)]
        [ForeignKey("Person")]
        public string Person_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("AssignedToPersonLogEntry")]
        public string AssignedToPersonLogEntry_Id { get; set; }

        [Required(ErrorMessage = "Entry Date field is required")]
        [DBIsDateTimeISO] [StringLength(64)]
        public string LogEntryDate { get; set; }
                
        [Required(ErrorMessage = "Comp. Status field is required")]
        [StringLength(40)]
        [ForeignKey("ComponentStatus")]
        public string ComponentStatus_Id { get; set; }
        
        [Required(ErrorMessage = "Project field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToProject")]
        public string AssignedToProject_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("AssignedToAssemblyDb")]
        public string AssignedToAssemblyDb_Id { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string Comments { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        //TSP Column Wireup ---------------------------------------------------------------------------------------------------//
        [ConcurrencyCheck]
        [Column(TypeName = "timestamp")]
        public DateTime TSP
        {
            get { return this.tsp.HasValue ? this.tsp.Value : DateTime.Now; }
            set { tsp = value; }
        }
        [NotMapped]
        private DateTime? tsp;

        //EF Navigation Properties---------------------------------------------------------------------------------------------//

        //one to one
        
        //one to many
        public virtual Component Component { get; set; }
        public virtual Person Person { get; set; }
        public virtual PersonLogEntry AssignedToPersonLogEntry { get; set; }

        public virtual ComponentStatus ComponentStatus { get; set; }
        public virtual Project AssignedToProject { get; set; }
        public virtual AssemblyDb AssignedToAssemblyDb { get; set; }

        //many to many
        

        //Constructors---------------------------------------------------------------------------------------------------------//

        
        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

    }
  



}
