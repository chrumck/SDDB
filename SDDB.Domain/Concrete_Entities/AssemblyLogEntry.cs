using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;
using System.Collections.Generic;

namespace SDDB.Domain.Entities
{
    [Table("AssemblyLogEntrys")]
    public class AssemblyLogEntry : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Entry Date field is required")]
        [DBIsDateTimeISO]
        public DateTime LogEntryDateTime { get; set; }

        [Required(ErrorMessage = "Assy field is required")]
        [StringLength(40)]
        [ForeignKey("AssemblyDb")]
        public string AssemblyDb_Id { get; set; }
        
        [Required(ErrorMessage = "Person field is required")]
        [StringLength(40)]
        [ForeignKey("EnteredByPerson")]
        public string EnteredByPerson_Id { get; set; }
        
        [Required(ErrorMessage = "Assy. Status field is required")]
        [StringLength(40)]
        [ForeignKey("AssemblyStatus")]
        public string AssemblyStatus_Id { get; set; }

        [Required(ErrorMessage = "Project field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToProject")]
        public string AssignedToProject_Id { get; set; }

        [Required(ErrorMessage = "Location field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToLocation")]
        public string AssignedToLocation_Id { get; set; }

        [Required(ErrorMessage = "Assy. Global Coord's are required")]
        public decimal AssyGlobalX { get; set; }

        [Required(ErrorMessage = "Assy. Global Coord's are required")]
        public decimal AssyGlobalY { get; set; }

        [Required(ErrorMessage = "Assy. Global Coord's are required")]
        public decimal AssyGlobalZ { get; set; }

        public decimal? AssyLocalXDesign { get; set; }

        public decimal? AssyLocalYDesign { get; set; }

        public decimal? AssyLocalZDesign { get; set; }

        public decimal? AssyLocalXAsBuilt { get; set; }

        public decimal? AssyLocalYAsBuilt { get; set; }

        public decimal? AssyLocalZAsBuilt { get; set; }

        public decimal? AssyStationing { get; set; }

        public decimal? AssyLength { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string Comments { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }

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
        public virtual AssemblyDb AssemblyDb { get; set; }
        public virtual Person EnteredByPerson { get; set; }
        
        public virtual AssemblyStatus AssemblyStatus { get; set; }
        public virtual Project AssignedToProject { get; set; }
        public virtual Location AssignedToLocation { get; set; }

        //many to many
        

        //Constructors---------------------------------------------------------------------------------------------------------//

                
        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

    }
  



}
