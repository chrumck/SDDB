using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("Components")]
    public class Component : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Comp. Name field is required")]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string CompName { get; set; }

        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string CompAltName { get; set; }

        [StringLength(255)]
        public string CompAltName2 { get; set; }

        [Required( ErrorMessage = "Comp. Type field is required")]
        [StringLength(40)]
        [ForeignKey("ComponentType")]
        public string ComponentType_Id { get; set; }

        [Required(ErrorMessage = "Comp. Status field is required")]
        [StringLength(40)]
        [ForeignKey("ComponentStatus")]
        public string ComponentStatus_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("ComponentModel")]
        public string ComponentModel_Id { get; set; }

        [Required(ErrorMessage = "Project field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToProject")]
        public string AssignedToProject_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("AssignedToAssemblyDb")]
        public string AssignedToAssemblyDb_Id { get; set; }

        [StringLength(255)]
        public string PositionInAssy { get; set; }

        [StringLength(255)]
        public string ProgramAddress { get; set; }
                
        [Required]
        [DefaultValue(false)]
        public bool CalibrationReqd { get; set; }

        [DBIsDateISO]
        public DateTime? LastCalibrationDate { get; set; }

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
        public virtual ComponentType ComponentType { get; set; }
        public virtual ComponentStatus ComponentStatus { get; set; }
        public virtual ComponentModel ComponentModel { get; set; }
        public virtual Project AssignedToProject { get; set; }
        public virtual AssemblyDb AssignedToAssemblyDb { get; set; }
                
        public virtual ComponentExt ComponentExt { get; set; }

        [InverseProperty("Component")]
        public virtual ICollection<ComponentLogEntry> ComponentLogEntrys { get; set; }

        //many to many
        

        //Constructors---------------------------------------------------------------------------------------------------------//

        public Component()
        {
            this.ComponentLogEntrys = new HashSet<ComponentLogEntry>();
        }

        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

    }
  



}
