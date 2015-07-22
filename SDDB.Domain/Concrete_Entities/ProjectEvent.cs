using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("ProjectEvents")]
    public class ProjectEvent : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string EventName { get; set; }

        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string EventAltName { get; set; }

        [Required(ErrorMessage = "Project field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToProject")]
        public string AssignedToProject_Id { get; set; }

        [Required(ErrorMessage = "Event Create Date is required")]
        [DBIsDateTimeISO] 
        public DateTime EventCreated { get; set; }

        [Required(ErrorMessage = "Created By Person field is required")]
        [StringLength(40)]
        [ForeignKey("CreatedByPerson")]
        public string CreatedByPerson_Id { get; set; }

        [DBIsDateTimeISO] 
        public DateTime? EventClosed { get; set; }

        [StringLength(40)]
        [ForeignKey("ClosedByPerson")]
        public string ClosedByPerson_Id { get; set; }

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
        public virtual Project AssignedToProject { get; set; }
        public virtual Person CreatedByPerson { get; set; }
        public virtual Person ClosedByPerson { get; set; }

        //many to many
        
        
        //Constructors---------------------------------------------------------------------------------------------------------//
        
        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

    }
  

}
