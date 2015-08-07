using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("Documents")]
    public class Document : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string DocName { get; set; }

        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string DocAltName { get; set; }

        [Required( ErrorMessage = "Doc. Type field is required")]
        [StringLength(40)]
        [ForeignKey("DocumentType")]
        public string DocumentType_Id { get; set; }

        [StringLength(255)]
        public string DocLastVersion { get; set; }

        [StringLength(40)]
        [ForeignKey("AuthorPerson")]
        public string AuthorPerson_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("ReviewerPerson")]
        public string ReviewerPerson_Id { get; set; }

        [Required(ErrorMessage = "Project field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToProject")]
        public string AssignedToProject_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("RelatesToAssyType")]
        public string RelatesToAssyType_Id { get; set; }

        [StringLength(40)]
        [ForeignKey("RelatesToCompType")]
        public string RelatesToCompType_Id { get; set; }

        [StringLength(255)]
        public string DocFilePath { get; set; }

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
        public virtual DocumentType DocumentType { get; set; }
        public virtual Person AuthorPerson { get; set; }
        public virtual Person ReviewerPerson { get; set; }
        public virtual Project AssignedToProject { get; set; }
        public virtual AssemblyType RelatesToAssyType { get; set; }
        public virtual ComponentType RelatesToCompType { get; set; }

        //many to many
        

        //Constructors---------------------------------------------------------------------------------------------------------//

        
        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

    }
  



}
