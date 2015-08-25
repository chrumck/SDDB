using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;
using System.Collections.Generic;

namespace SDDB.Domain.Entities
{
    [Table("Locations")]
    public class Location : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string LocName { get; set; }

        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string LocAltName { get; set; }

        [StringLength(255)]
        public string LocAltName2 { get; set; }

        [Required( ErrorMessage = "Loc. Type field is required")]
        [StringLength(40)]
        [ForeignKey("LocationType")]
        public string LocationType_Id { get; set; }

        [Required(ErrorMessage = "Project field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToProject")]
        public string AssignedToProject_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("ContactPerson")]
        public string ContactPerson_Id { get; set; }

        [StringLength(255)]
        public string Address { get; set; }
        
        public decimal? LocX { get; set; }

        public decimal? LocY { get; set; }

        public decimal? LocZ { get; set; }

        public decimal? LocStationing { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool CertOfApprReqd_bl { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool RightOfEntryReqd_bl { get; set; }
        
        [Column(TypeName = "text")] [StringLength(65535)]
        public string AccessInfo { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string Comments { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }

        //TSP Column Wireup ---------------------------------------------------------------------------------------------------//
        [Column(TypeName = "timestamp")]
        public DateTime TSP
        {
            get { return this.tsp.HasValue ? this.tsp.Value : DateTime.Now; }
            set { tsp = value; }
        }
        [NotMapped]
        private DateTime? tsp;

        //LastSavedByPerson_Id -------------------------------------------------------------------------------------------------//
        //[Required(ErrorMessage = "LastSavedByPerson Id field is required")]
        [StringLength(40)]
        [ForeignKey("LastSavedByPerson")]
        public string LastSavedByPerson_Id { get; set; }
        //Navigation Property
        public virtual Person LastSavedByPerson { get; set; }

        //EF Navigation Properties---------------------------------------------------------------------------------------------//

        //one to one
        
        //one to many
        public virtual LocationType LocationType { get; set; }
        public virtual Project AssignedToProject { get; set; }
        public virtual Person ContactPerson { get; set; }

        [InverseProperty("AssignedToLocation")]
        public virtual ICollection<AssemblyDb> LocationAssemblyDbs { get; set; }

        //many to many
        

        //Constructors---------------------------------------------------------------------------------------------------------//

        public Location()
        {
            this.LocationAssemblyDbs = new HashSet<AssemblyDb>();
        }
        
        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

    }
  



}
